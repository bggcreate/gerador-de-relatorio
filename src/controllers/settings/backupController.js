const fs = require('fs');
const path = require('path');
const archiver = require('archiver');
const db = require('../../config/db-adapter').getDatabase();
const googleDriveService = require('../../../services/googleDriveService');

async function sendBackupEmail(req, res) {
    try {
        const { emailTo } = req.body;
        
        if (!emailTo) {
            return res.status(400).json({ error: 'Email de destino é obrigatório' });
        }

        const backupDir = path.join(__dirname, '../../../data/backups');
        if (!fs.existsSync(backupDir)) {
            fs.mkdirSync(backupDir, { recursive: true });
        }

        const timestamp = new Date().toISOString().split('T')[0];
        const filename = `backup_database_${timestamp}_${Date.now()}.db`;
        const backupPath = path.join(backupDir, filename);

        const dbPath = require('../../config/db-adapter').DB_PATH;
        
        if (dbPath.startsWith('PostgreSQL@')) {
            return res.status(400).json({ 
                error: 'Backup de PostgreSQL deve ser feito através do painel de controle do provedor' 
            });
        }

        fs.copyFileSync(dbPath, backupPath);
        
        const stats = fs.statSync(backupPath);
        const sizeBytes = stats.size;

        const result = await googleDriveService.enviarBackupPorEmail(backupPath, emailTo);

        db.run(
            `INSERT INTO db_backups (filename, filepath, size_bytes, backup_type, sent_to_email, email_sent_at, created_by, status) 
             VALUES (?, ?, ?, ?, ?, datetime('now'), ?, ?)`,
            [filename, backupPath, sizeBytes, 'manual', 1, req.session.username, 'success'],
            function(err) {
                if (err) {
                    console.error('Erro ao registrar backup:', err);
                }
            }
        );

        fs.unlinkSync(backupPath);

        res.json({
            success: true,
            message: `Backup enviado para ${emailTo}`,
            filename,
            sizeBytes,
            sizeMB: (sizeBytes / 1024 / 1024).toFixed(2)
        });

    } catch (error) {
        console.error('Erro ao enviar backup por email:', error);
        res.status(500).json({ error: 'Erro ao enviar backup por email' });
    }
}

async function createManualBackup(req, res) {
    try {
        const backupDir = path.join(__dirname, '../../../data/backups');
        if (!fs.existsSync(backupDir)) {
            fs.mkdirSync(backupDir, { recursive: true });
        }

        const timestamp = new Date().toISOString().split('T')[0];
        const filename = `backup_database_${timestamp}_${Date.now()}.db`;
        const backupPath = path.join(backupDir, filename);

        const dbPath = require('../../config/db-adapter').DB_PATH;
        
        if (dbPath.startsWith('PostgreSQL@')) {
            return res.status(400).json({ 
                error: 'Backup de PostgreSQL deve ser feito através do painel de controle do provedor',
                message: 'Para PostgreSQL, use as ferramentas nativas de backup do seu provedor'
            });
        }

        fs.copyFileSync(dbPath, backupPath);
        
        const stats = fs.statSync(backupPath);
        const sizeBytes = stats.size;

        db.run(
            `INSERT INTO db_backups (filename, filepath, size_bytes, backup_type, created_by, status) 
             VALUES (?, ?, ?, ?, ?, ?)`,
            [filename, backupPath, sizeBytes, 'manual', req.session.username, 'success'],
            function(err) {
                if (err) {
                    console.error('Erro ao registrar backup:', err);
                    return res.status(500).json({ error: 'Erro ao registrar backup' });
                }

                res.json({
                    success: true,
                    message: 'Backup criado com sucesso',
                    filename,
                    filepath: backupPath,
                    sizeBytes,
                    sizeMB: (sizeBytes / 1024 / 1024).toFixed(2),
                    downloadUrl: `/api/settings/backup/download/${this.lastID}`
                });
            }
        );

    } catch (error) {
        console.error('Erro ao criar backup manual:', error);
        res.status(500).json({ error: 'Erro ao criar backup manual' });
    }
}

function getBackupHistory(req, res) {
    try {
        const { limit = 50 } = req.query;
        
        db.all(
            `SELECT * FROM db_backups ORDER BY created_at DESC LIMIT ?`,
            [parseInt(limit, 10)],
            (err, rows) => {
                if (err) {
                    console.error('Erro ao buscar histórico:', err);
                    return res.status(500).json({ error: 'Erro ao buscar histórico' });
                }

                const backups = rows.map(backup => ({
                    ...backup,
                    sizeMB: (backup.size_bytes / 1024 / 1024).toFixed(2),
                    downloadUrl: `/api/settings/backup/download/${backup.id}`
                }));

                res.json({ backups });
            }
        );
    } catch (error) {
        console.error('Erro ao buscar histórico:', error);
        res.status(500).json({ error: 'Erro ao buscar histórico de backups' });
    }
}

function downloadBackup(req, res) {
    try {
        const { id } = req.params;

        db.get(
            `SELECT * FROM db_backups WHERE id = ?`,
            [id],
            (err, backup) => {
                if (err || !backup) {
                    return res.status(404).json({ error: 'Backup não encontrado' });
                }

                if (!fs.existsSync(backup.filepath)) {
                    return res.status(404).json({ error: 'Arquivo de backup não encontrado' });
                }

                res.download(backup.filepath, backup.filename);
            }
        );
    } catch (error) {
        console.error('Erro ao baixar backup:', error);
        res.status(500).json({ error: 'Erro ao baixar backup' });
    }
}

function updateBackupConfig(req, res) {
    try {
        const { autoBackupEnabled, autoBackupFrequency, autoBackupEmail } = req.body;

        const updates = [];
        if (autoBackupEnabled !== undefined) {
            updates.push({ key: 'auto_backup_enabled', value: autoBackupEnabled ? '1' : '0' });
        }
        if (autoBackupFrequency) {
            updates.push({ key: 'auto_backup_frequency', value: autoBackupFrequency });
        }
        if (autoBackupEmail) {
            updates.push({ key: 'auto_backup_email', value: autoBackupEmail });
        }

        let completed = 0;
        const total = updates.length;

        if (total === 0) {
            return res.json({ success: true, message: 'Nenhuma configuração para atualizar' });
        }

        updates.forEach(update => {
            db.run(
                `INSERT INTO system_settings (setting_key, setting_value, updated_by, updated_at) 
                 VALUES (?, ?, ?, datetime('now'))
                 ON CONFLICT(setting_key) DO UPDATE SET 
                 setting_value = excluded.setting_value,
                 updated_by = excluded.updated_by,
                 updated_at = datetime('now')`,
                [update.key, update.value, req.session.username],
                (err) => {
                    if (err) {
                        console.error('Erro ao atualizar configuração:', err);
                    }
                    completed++;
                    if (completed === total) {
                        res.json({ success: true, message: 'Configurações atualizadas' });
                    }
                }
            );
        });
    } catch (error) {
        console.error('Erro ao atualizar configurações:', error);
        res.status(500).json({ error: 'Erro ao atualizar configurações' });
    }
}

function getBackupConfig(req, res) {
    try {
        db.all(
            `SELECT setting_key, setting_value FROM system_settings WHERE setting_key LIKE 'auto_backup%'`,
            [],
            (err, rows) => {
                if (err) {
                    console.error('Erro ao buscar configurações:', err);
                    return res.status(500).json({ error: 'Erro ao buscar configurações' });
                }

                const config = {
                    autoBackupEnabled: false,
                    autoBackupFrequency: 'weekly',
                    autoBackupEmail: ''
                };

                rows.forEach(row => {
                    if (row.setting_key === 'auto_backup_enabled') {
                        config.autoBackupEnabled = row.setting_value === '1';
                    } else if (row.setting_key === 'auto_backup_frequency') {
                        config.autoBackupFrequency = row.setting_value;
                    } else if (row.setting_key === 'auto_backup_email') {
                        config.autoBackupEmail = row.setting_value;
                    }
                });

                res.json(config);
            }
        );
    } catch (error) {
        console.error('Erro ao buscar configurações:', error);
        res.status(500).json({ error: 'Erro ao buscar configurações' });
    }
}

module.exports = {
    sendBackupEmail,
    createManualBackup,
    getBackupHistory,
    downloadBackup,
    updateBackupConfig,
    getBackupConfig
};
