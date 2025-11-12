// =================================================================
// SERVIÇO DE MONITORAMENTO DO BANCO DE DADOS
// =================================================================
const cron = require('node-cron');
const nodemailer = require('nodemailer');
const { spawn } = require('child_process');
const fs = require('fs').promises;
const path = require('path');
const { getDatabaseSize, formatBytes, query, isPostgresEnabled } = require('../config/postgresql');

// Configurações
const SIZE_THRESHOLD_GB = 4; // 4GB threshold
const SIZE_THRESHOLD_BYTES = SIZE_THRESHOLD_GB * 1024 * 1024 * 1024;
const BACKUP_DIR = path.join(__dirname, '../../data/backups');
const CHECK_INTERVAL = '0 */6 * * *'; // A cada 6 horas

// Verifica se monitoramento está ativo
let isMonitoringActive = false;
let cronJob = null;

// Cria diretório de backups se não existir
async function ensureBackupDir() {
    try {
        await fs.mkdir(BACKUP_DIR, { recursive: true });
    } catch (err) {
        console.error('❌ Erro ao criar diretório de backups:', err);
    }
}

// Obtém configuração de email do ambiente ou banco
function getEmailConfig() {
    return {
        host: process.env.SMTP_HOST || 'smtp.gmail.com',
        port: parseInt(process.env.SMTP_PORT || '587', 10),
        secure: process.env.SMTP_SECURE === 'true',
        auth: {
            user: process.env.SMTP_USER,
            pass: process.env.SMTP_PASS
        }
    };
}

// Envia email com backup anexo
async function sendBackupEmail(backupPath, backupSize) {
    if (!process.env.SMTP_USER || !process.env.SMTP_PASS || !process.env.BACKUP_EMAIL_TO) {
        console.warn('⚠️  Email não configurado. Configure SMTP_USER, SMTP_PASS e BACKUP_EMAIL_TO');
        return false;
    }
    
    try {
        const transporter = nodemailer.createTransport(getEmailConfig());
        
        const backupFileName = path.basename(backupPath);
        const backupStats = await fs.stat(backupPath);
        
        const mailOptions = {
            from: process.env.SMTP_USER,
            to: process.env.BACKUP_EMAIL_TO,
            subject: `⚠️ Backup Automático do Banco de Dados - ${formatBytes(backupSize)}`,
            html: `
                <h2>Backup Automático do Banco de Dados</h2>
                <p>O banco de dados atingiu o limite de <strong>${SIZE_THRESHOLD_GB}GB</strong> e um backup foi criado automaticamente.</p>
                
                <h3>Detalhes do Backup:</h3>
                <ul>
                    <li><strong>Tamanho do Banco:</strong> ${formatBytes(backupSize)}</li>
                    <li><strong>Arquivo:</strong> ${backupFileName}</li>
                    <li><strong>Tamanho do Backup:</strong> ${formatBytes(backupStats.size)}</li>
                    <li><strong>Data/Hora:</strong> ${new Date().toLocaleString('pt-BR')}</li>
                    <li><strong>Banco:</strong> ${process.env.PGDATABASE}@${process.env.PGHOST}</li>
                </ul>
                
                <p><strong>Ação Recomendada:</strong> Faça o download do backup anexo e armazene em local seguro.</p>
                
                <hr>
                <p style="color: #666; font-size: 12px;">
                    Este é um email automático do sistema de monitoramento de banco de dados.
                </p>
            `,
            attachments: [
                {
                    filename: backupFileName,
                    path: backupPath
                }
            ]
        };
        
        await transporter.sendMail(mailOptions);
        console.log('✅ Email de backup enviado com sucesso para:', process.env.BACKUP_EMAIL_TO);
        return true;
        
    } catch (err) {
        console.error('❌ Erro ao enviar email de backup:', err);
        return false;
    }
}

// Cria backup do banco PostgreSQL usando pg_dump
async function createBackup() {
    try {
        await ensureBackupDir();
        
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
        const backupFileName = `backup-${timestamp}.sql`;
        const backupPath = path.join(BACKUP_DIR, backupFileName);
        
        console.log('📦 Criando backup do banco de dados...');
        
        // CORREÇÃO DE SEGURANÇA: Usar spawn com env ao invés de interpolação de string
        // para evitar command injection através da senha
        return new Promise((resolve, reject) => {
            const pgDump = spawn('pg_dump', [
                '-h', process.env.PGHOST,
                '-p', String(process.env.PGPORT || 5432),
                '-U', process.env.PGUSER,
                '-d', process.env.PGDATABASE,
                '-F', 'p',
                '-f', backupPath
            ], {
                env: {
                    ...process.env,
                    PGPASSWORD: process.env.PGPASSWORD
                }
            });
            
            let errorOutput = '';
            
            pgDump.stderr.on('data', (data) => {
                errorOutput += data.toString();
            });
            
            pgDump.on('close', async (code) => {
                if (code !== 0) {
                    console.error('❌ pg_dump error:', errorOutput);
                    reject(new Error(`pg_dump exited with code ${code}`));
                    return;
                }
                
                try {
                    const stats = await fs.stat(backupPath);
                    console.log(`✅ Backup criado: ${backupFileName} (${formatBytes(stats.size)})`);
                    
                    resolve({
                        path: backupPath,
                        filename: backupFileName,
                        size: stats.size
                    });
                } catch (err) {
                    reject(err);
                }
            });
            
            pgDump.on('error', (err) => {
                console.error('❌ Erro ao executar pg_dump:', err);
                reject(err);
            });
        });
        
    } catch (err) {
        console.error('❌ Erro ao criar backup:', err);
        throw err;
    }
}

// Registra backup no banco
async function registerBackup(filename, filepath, sizeBytes, emailSent) {
    try {
        const instanceUuid = process.env.INSTANCE_UUID || null;
        
        await query(
            `INSERT INTO db_backups (filename, filepath, size_bytes, sent_to_email, email_sent_at, backup_type, source_instance, notes)
             VALUES ($1, $2, $3, $4, $5, $6, $7, $8)`,
            [
                filename,
                filepath,
                sizeBytes,
                emailSent,
                emailSent ? new Date() : null,
                'automatic',
                instanceUuid,
                `Backup automático ao atingir ${SIZE_THRESHOLD_GB}GB`
            ]
        );
        
        console.log('✅ Backup registrado no banco');
    } catch (err) {
        console.error('❌ Erro ao registrar backup:', err);
    }
}

// Verifica tamanho do banco e executa backup se necessário
async function checkDatabaseSize() {
    try {
        if (!isPostgresEnabled()) {
            console.log('⚠️  PostgreSQL não configurado, monitoramento desabilitado');
            return;
        }
        
        const sizeBytes = await getDatabaseSize();
        const sizeFormatted = formatBytes(sizeBytes);
        const percentUsed = (sizeBytes / SIZE_THRESHOLD_BYTES * 100).toFixed(2);
        
        console.log(`📊 Tamanho do banco: ${sizeFormatted} (${percentUsed}% do limite de ${SIZE_THRESHOLD_GB}GB)`);
        
        if (sizeBytes >= SIZE_THRESHOLD_BYTES) {
            console.warn(`⚠️  LIMITE ATINGIDO! Criando backup automático...`);
            
            // Cria backup
            const backup = await createBackup();
            
            // Envia por email
            const emailSent = await sendBackupEmail(backup.path, sizeBytes);
            
            // Registra no banco
            await registerBackup(backup.filename, backup.path, backup.size, emailSent);
            
            console.log('✅ Processo de backup automático concluído');
        }
        
    } catch (err) {
        console.error('❌ Erro ao verificar tamanho do banco:', err);
    }
}

// Inicia monitoramento automático
function startMonitoring() {
    if (!isPostgresEnabled()) {
        console.log('⚠️  PostgreSQL não configurado, monitoramento não iniciado');
        return;
    }
    
    if (isMonitoringActive) {
        console.log('⚠️  Monitoramento já está ativo');
        return;
    }
    
    console.log(`🔍 Iniciando monitoramento de banco de dados (limite: ${SIZE_THRESHOLD_GB}GB)`);
    console.log(`📅 Verificação agendada: ${CHECK_INTERVAL}`);
    
    // Verifica imediatamente
    checkDatabaseSize();
    
    // Agenda verificações periódicas
    cronJob = cron.schedule(CHECK_INTERVAL, () => {
        console.log('\n🔍 Executando verificação periódica do tamanho do banco...');
        checkDatabaseSize();
    });
    
    isMonitoringActive = true;
    console.log('✅ Monitoramento iniciado');
}

// Para monitoramento
function stopMonitoring() {
    if (cronJob) {
        cronJob.stop();
        cronJob = null;
    }
    isMonitoringActive = false;
    console.log('🛑 Monitoramento parado');
}

// Backup manual
async function manualBackup() {
    try {
        console.log('📦 Iniciando backup manual...');
        
        const sizeBytes = await getDatabaseSize();
        const backup = await createBackup();
        
        await registerBackup(backup.filename, backup.path, backup.size, false);
        
        return {
            success: true,
            filename: backup.filename,
            size: formatBytes(backup.size),
            dbSize: formatBytes(sizeBytes),
            path: backup.path
        };
        
    } catch (err) {
        console.error('❌ Erro no backup manual:', err);
        return {
            success: false,
            error: err.message
        };
    }
}

// Obtém histórico de backups
async function getBackupHistory(limit = 20) {
    try {
        const result = await query(
            `SELECT * FROM db_backups
             ORDER BY created_at DESC
             LIMIT $1`,
            [limit]
        );
        
        return result.rows.map(row => ({
            ...row,
            size_formatted: formatBytes(row.size_bytes)
        }));
        
    } catch (err) {
        console.error('❌ Erro ao obter histórico de backups:', err);
        return [];
    }
}

module.exports = {
    startMonitoring,
    stopMonitoring,
    checkDatabaseSize,
    manualBackup,
    getBackupHistory,
    SIZE_THRESHOLD_GB,
    BACKUP_DIR
};
