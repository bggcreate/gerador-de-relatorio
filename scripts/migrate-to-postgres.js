#!/usr/bin/env node
// =================================================================
// SCRIPT DE MIGRAÇÃO SQLITE -> POSTGRESQL
// =================================================================
const sqlite3 = require('sqlite3').verbose();
const { Pool } = require('pg');
const path = require('path');
const fs = require('fs');
const { v4: uuidv4 } = require('uuid');

require('dotenv').config();

// Configuração
const SQLITE_DB_PATH = process.env.SQLITE_DB_PATH || path.join(__dirname, '../data/database.db');
const INSTANCE_UUID = process.env.INSTANCE_UUID || uuidv4();

// Verifica se PostgreSQL está configurado
if (!process.env.PGHOST || !process.env.PGDATABASE || !process.env.PGUSER || !process.env.PGPASSWORD) {
    console.error('❌ Erro: Configure as variáveis de ambiente do PostgreSQL:');
    console.error('   PGHOST, PGDATABASE, PGUSER, PGPASSWORD');
    process.exit(1);
}

// Pool PostgreSQL
const pgPool = new Pool({
    host: process.env.PGHOST,
    port: process.env.PGPORT || 5432,
    database: process.env.PGDATABASE,
    user: process.env.PGUSER,
    password: process.env.PGPASSWORD,
    ssl: process.env.PGSSLMODE === 'require' ? { rejectUnauthorized: false } : false
});

// Tabelas a migrar (em ordem de dependências)
const TABLES = [
    'usuarios',
    'lojas',
    'relatorios',
    'demandas',
    'vendedores',
    'logs',
    'temp_tokens',
    'estoque_tecnico',
    'assistencias',
    'pdf_tickets',
    'pdf_rankings'
];

// Tabelas que recebem source_instance
const TABLES_WITH_SOURCE = [
    'relatorios',
    'demandas',
    'vendedores',
    'logs',
    'estoque_tecnico',
    'assistencias',
    'pdf_tickets',
    'pdf_rankings'
];

async function runMigration() {
    console.log('🚀 Iniciando migração SQLite -> PostgreSQL...\n');
    
    // Verifica se SQLite existe
    if (!fs.existsSync(SQLITE_DB_PATH)) {
        console.error(`❌ Arquivo SQLite não encontrado: ${SQLITE_DB_PATH}`);
        process.exit(1);
    }
    
    const sqliteDb = new sqlite3.Database(SQLITE_DB_PATH);
    
    try {
        // 1. Criar schema no PostgreSQL
        console.log('📋 Etapa 1: Criando schema no PostgreSQL...');
        const schemaSQL = fs.readFileSync(path.join(__dirname, 'migrations/001_create_schema.sql'), 'utf8');
        await pgPool.query(schemaSQL);
        console.log('✅ Schema criado\n');
        
        // 2. Registrar instância
        console.log('📋 Etapa 2: Registrando instância...');
        const instanceName = process.env.INSTANCE_NAME || `Migração ${new Date().toISOString()}`;
        await pgPool.query(
            `INSERT INTO instance_config (instance_uuid, instance_name, created_at)
             VALUES ($1, $2, NOW())
             ON CONFLICT (instance_uuid) DO UPDATE
             SET instance_name = $2, last_updated = NOW()`,
            [INSTANCE_UUID, instanceName]
        );
        console.log(`✅ Instância registrada: ${INSTANCE_UUID}\n`);
        
        // 3. Migrar dados tabela por tabela
        console.log('📋 Etapa 3: Migrando dados...\n');
        
        for (const table of TABLES) {
            await migrateTable(sqliteDb, table);
        }
        
        console.log('\n✅ Migração concluída com sucesso!');
        console.log(`\n📊 Resumo:`);
        console.log(`   - Banco SQLite: ${SQLITE_DB_PATH}`);
        console.log(`   - Banco PostgreSQL: ${process.env.PGDATABASE}@${process.env.PGHOST}`);
        console.log(`   - Instance UUID: ${INSTANCE_UUID}`);
        
    } catch (error) {
        console.error('\n❌ Erro durante migração:', error);
        process.exit(1);
    } finally {
        sqliteDb.close();
        await pgPool.end();
    }
}

async function migrateTable(sqliteDb, tableName) {
    return new Promise((resolve, reject) => {
        console.log(`   Migrando tabela: ${tableName}...`);
        
        sqliteDb.all(`SELECT * FROM ${tableName}`, async (err, rows) => {
            if (err) {
                if (err.message.includes('no such table')) {
                    console.log(`   ⚠️  Tabela ${tableName} não existe no SQLite, pulando...`);
                    return resolve();
                }
                return reject(err);
            }
            
            if (!rows || rows.length === 0) {
                console.log(`   ✅ ${tableName}: 0 registros (vazia)`);
                return resolve();
            }
            
            try {
                let migrated = 0;
                const client = await pgPool.connect();
                
                try {
                    await client.query('BEGIN');
                    
                    for (const row of rows) {
                        // CORREÇÃO: Incluir ID para preservar foreign keys
                        const columns = Object.keys(row);
                        
                        // Adiciona source_instance se a tabela suporta
                        if (TABLES_WITH_SOURCE.includes(tableName)) {
                            columns.push('source_instance');
                            row.source_instance = INSTANCE_UUID;
                        }
                        
                        const placeholders = columns.map((_, i) => `$${i + 1}`).join(', ');
                        const values = columns.map(col => row[col]);
                        
                        const sql = `INSERT INTO ${tableName} (${columns.join(', ')})
                                    VALUES (${placeholders})`;
                        
                        await client.query(sql, values);
                        migrated++;
                    }
                    
                    // CORREÇÃO: Resetar sequence após migração para evitar conflitos futuros
                    if (migrated > 0) {
                        const maxIdResult = await client.query(
                            `SELECT MAX(id) as max_id FROM ${tableName}`
                        );
                        const maxId = maxIdResult.rows[0].max_id || 0;
                        
                        // Ajusta a sequence para começar após o último ID migrado
                        await client.query(
                            `SELECT setval(pg_get_serial_sequence('${tableName}', 'id'), $1, true)`,
                            [maxId]
                        );
                    }
                    
                    await client.query('COMMIT');
                    console.log(`   ✅ ${tableName}: ${migrated} registros migrados`);
                    
                } catch (error) {
                    await client.query('ROLLBACK');
                    throw error;
                } finally {
                    client.release();
                }
                
                resolve();
                
            } catch (error) {
                reject(error);
            }
        });
    });
}

// Executar migração
runMigration();
