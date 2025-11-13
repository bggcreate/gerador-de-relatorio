#!/usr/bin/env node
// =================================================================
// SCRIPT DE UNIFICAÇÃO DE BANCOS DE DADOS
// Mescla múltiplos backups/instâncias em um único banco consolidado
// =================================================================
const { Pool } = require('pg');
const fs = require('fs').promises;
const path = require('path');
const readline = require('readline');

require('dotenv').config();

// Configuração do banco de destino (consolidado)
const targetPool = new Pool({
    host: process.env.PGHOST,
    port: process.env.PGPORT || 5432,
    database: process.env.PGDATABASE,
    user: process.env.PGUSER,
    password: process.env.PGPASSWORD,
    ssl: process.env.PGSSLMODE === 'require' ? { rejectUnauthorized: false } : false
});

// Tabelas a mesclar (excluindo tabelas de configuração)
const TABLES_TO_MERGE = [
    'relatorios',
    'demandas',
    'vendedores',
    'logs',
    'estoque_tecnico',
    'assistencias',
    'pdf_tickets',
    'pdf_rankings'
];

// Interface readline para perguntas
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

function question(query) {
    return new Promise(resolve => rl.question(query, resolve));
}

// Conecta a um banco de dados PostgreSQL remoto
async function connectToSourceDatabase(config) {
    const pool = new Pool({
        host: config.host,
        port: config.port || 5432,
        database: config.database,
        user: config.user,
        password: config.password,
        ssl: config.ssl === 'true' ? { rejectUnauthorized: false } : false
    });
    
    try {
        await pool.query('SELECT 1');
        console.log(`✅ Conectado ao banco de origem: ${config.database}@${config.host}`);
        return pool;
    } catch (err) {
        console.error(`❌ Erro ao conectar ao banco de origem:`, err.message);
        throw err;
    }
}

// Obtém estatísticas de uma instância
async function getInstanceStats(pool, instanceUuid) {
    const stats = {};
    
    for (const table of TABLES_TO_MERGE) {
        try {
            const result = await pool.query(
                `SELECT COUNT(*) as count FROM ${table} WHERE source_instance = $1`,
                [instanceUuid]
            );
            stats[table] = parseInt(result.rows[0].count, 10);
        } catch (err) {
            stats[table] = 0;
        }
    }
    
    return stats;
}

// Verifica se registros já existem no banco de destino
async function checkDuplicates(sourcePool, instanceUuid) {
    try {
        const result = await targetPool.query(
            `SELECT COUNT(*) as count FROM instance_config WHERE instance_uuid = $1`,
            [instanceUuid]
        );
        
        return parseInt(result.rows[0].count, 10) > 0;
    } catch (err) {
        return false;
    }
}

// Mescla dados de uma tabela
async function mergeTable(sourcePool, tableName, instanceUuid) {
    try {
        // Busca dados da origem
        const result = await sourcePool.query(
            `SELECT * FROM ${tableName} WHERE source_instance = $1`,
            [instanceUuid]
        );
        
        if (result.rows.length === 0) {
            console.log(`   ${tableName}: 0 registros (vazia)`);
            return 0;
        }
        
        const client = await targetPool.connect();
        let inserted = 0;
        
        try {
            await client.query('BEGIN');
            
            for (const row of result.rows) {
                const columns = Object.keys(row).filter(col => col !== 'id');
                const placeholders = columns.map((_, i) => `$${i + 1}`).join(', ');
                const values = columns.map(col => row[col]);
                
                const sql = `INSERT INTO ${tableName} (${columns.join(', ')})
                            VALUES (${placeholders})
                            ON CONFLICT DO NOTHING`;
                
                const insertResult = await client.query(sql, values);
                if (insertResult.rowCount > 0) inserted++;
            }
            
            await client.query('COMMIT');
            console.log(`   ${tableName}: ${inserted}/${result.rows.length} registros inseridos`);
            
        } catch (error) {
            await client.query('ROLLBACK');
            throw error;
        } finally {
            client.release();
        }
        
        return inserted;
        
    } catch (err) {
        console.error(`   ❌ Erro ao mesclar ${tableName}:`, err.message);
        return 0;
    }
}

// Registra instância mesclada
async function registerMergedInstance(instanceUuid, instanceName) {
    try {
        await targetPool.query(
            `INSERT INTO instance_config (instance_uuid, instance_name, created_at, last_updated)
             VALUES ($1, $2, NOW(), NOW())
             ON CONFLICT (instance_uuid) DO UPDATE
             SET last_updated = NOW()`,
            [instanceUuid, instanceName]
        );
    } catch (err) {
        console.error('❌ Erro ao registrar instância:', err);
    }
}

// Processo principal de mesclagem
async function mergeDatabase(sourceConfig) {
    console.log('\n🔄 Iniciando processo de mesclagem...\n');
    
    let sourcePool;
    
    try {
        // Conecta ao banco de origem
        sourcePool = await connectToSourceDatabase(sourceConfig);
        
        // Obtém UUID da instância de origem
        const instanceResult = await sourcePool.query(
            `SELECT instance_uuid, instance_name FROM instance_config LIMIT 1`
        );
        
        if (instanceResult.rows.length === 0) {
            console.error('❌ Nenhuma instância encontrada no banco de origem');
            return;
        }
        
        const { instance_uuid, instance_name } = instanceResult.rows[0];
        console.log(`📋 Instância de origem: ${instance_name} (${instance_uuid})\n`);
        
        // Verifica duplicatas
        const hasDuplicates = await checkDuplicates(sourcePool, instance_uuid);
        if (hasDuplicates) {
            const answer = await question('⚠️  Esta instância já foi mesclada. Continuar mesmo assim? (s/n): ');
            if (answer.toLowerCase() !== 's') {
                console.log('❌ Mesclagem cancelada');
                return;
            }
        }
        
        // Obtém estatísticas
        console.log('📊 Estatísticas da instância de origem:');
        const stats = await getInstanceStats(sourcePool, instance_uuid);
        let totalRecords = 0;
        for (const [table, count] of Object.entries(stats)) {
            console.log(`   - ${table}: ${count} registros`);
            totalRecords += count;
        }
        console.log(`   TOTAL: ${totalRecords} registros\n`);
        
        if (totalRecords === 0) {
            console.log('⚠️  Nenhum registro para mesclar');
            return;
        }
        
        // Confirma mesclagem
        const confirm = await question(`Deseja mesclar ${totalRecords} registros? (s/n): `);
        if (confirm.toLowerCase() !== 's') {
            console.log('❌ Mesclagem cancelada');
            return;
        }
        
        console.log('\n🚀 Mesclando dados...\n');
        
        // Mescla cada tabela
        let totalInserted = 0;
        for (const table of TABLES_TO_MERGE) {
            const inserted = await mergeTable(sourcePool, table, instance_uuid);
            totalInserted += inserted;
        }
        
        // Registra instância
        await registerMergedInstance(instance_uuid, instance_name);
        
        console.log(`\n✅ Mesclagem concluída!`);
        console.log(`   - Total inserido: ${totalInserted} registros`);
        console.log(`   - Instância: ${instance_name} (${instance_uuid})`);
        
    } catch (err) {
        console.error('\n❌ Erro durante mesclagem:', err);
    } finally {
        if (sourcePool) {
            await sourcePool.end();
        }
    }
}

// Modo interativo
async function interactiveMode() {
    console.log('='.repeat(60));
    console.log('UNIFICAÇÃO DE BANCOS DE DADOS POSTGRESQL');
    console.log('='.repeat(60));
    console.log('\nEste script mescla dados de outro banco PostgreSQL para o banco atual.');
    console.log('Certifique-se de ter as credenciais do banco de origem.\n');
    
    const host = await question('Host do banco de origem: ');
    const port = await question('Porta (padrão 5432): ') || '5432';
    const database = await question('Nome do banco: ');
    const user = await question('Usuário: ');
    const password = await question('Senha: ');
    const ssl = await question('Usar SSL? (s/n, padrão n): ') || 'n';
    
    const sourceConfig = {
        host,
        port: parseInt(port, 10),
        database,
        user,
        password,
        ssl: ssl.toLowerCase() === 's' ? 'true' : 'false'
    };
    
    await mergeDatabase(sourceConfig);
}

// Execução
async function main() {
    try {
        await interactiveMode();
    } catch (err) {
        console.error('❌ Erro:', err);
    } finally {
        rl.close();
        await targetPool.end();
    }
}

// Verificar configuração
if (!process.env.PGHOST || !process.env.PGDATABASE) {
    console.error('❌ Configure as variáveis de ambiente do banco de destino:');
    console.error('   PGHOST, PGDATABASE, PGUSER, PGPASSWORD');
    process.exit(1);
}

main();
