// =================================================================
// CONFIGURAÇÃO POSTGRESQL - Conexão com Tembo.io
// =================================================================
const { Pool } = require('pg');

// Configuração de conexão PostgreSQL
const poolConfig = {
    host: process.env.PGHOST,
    port: process.env.PGPORT || 5432,
    database: process.env.PGDATABASE,
    user: process.env.PGUSER,
    password: process.env.PGPASSWORD,
    ssl: {
        rejectUnauthorized: false
    },
    max: 20, // Máximo de conexões no pool
    idleTimeoutMillis: 30000,
    connectionTimeoutMillis: 10000,
};

// Verifica se PostgreSQL está configurado
const isPostgresEnabled = () => {
    return !!(process.env.PGHOST && process.env.PGDATABASE && process.env.PGUSER && process.env.PGPASSWORD);
};

// Pool de conexões
let pool = null;

// Inicializa o pool de conexões
const getPool = () => {
    if (!isPostgresEnabled()) {
        return null;
    }
    
    if (!pool) {
        pool = new Pool(poolConfig);
        
        pool.on('error', (err) => {
            console.error('❌ Erro inesperado no pool PostgreSQL:', err);
        });
        
        console.log('✅ Pool de conexões PostgreSQL criado');
    }
    
    return pool;
};

// Testa a conexão
const testConnection = async () => {
    if (!isPostgresEnabled()) {
        return false;
    }
    
    try {
        const pool = getPool();
        const result = await pool.query('SELECT NOW()');
        console.log('✅ Conexão PostgreSQL bem-sucedida:', result.rows[0].now);
        return true;
    } catch (err) {
        console.error('❌ Erro ao conectar ao PostgreSQL:', err.message);
        return false;
    }
};

// Função auxiliar para executar queries
const query = async (text, params) => {
    const pool = getPool();
    if (!pool) {
        throw new Error('PostgreSQL não está configurado');
    }
    return pool.query(text, params);
};

// Função auxiliar para transações
const transaction = async (callback) => {
    const pool = getPool();
    if (!pool) {
        throw new Error('PostgreSQL não está configurado');
    }
    
    const client = await pool.connect();
    try {
        await client.query('BEGIN');
        const result = await callback(client);
        await client.query('COMMIT');
        return result;
    } catch (err) {
        await client.query('ROLLBACK');
        throw err;
    } finally {
        client.release();
    }
};

// Fecha o pool de conexões (para shutdown gracioso)
const closePool = async () => {
    if (pool) {
        await pool.end();
        pool = null;
        console.log('✅ Pool de conexões PostgreSQL fechado');
    }
};

// Obtém o tamanho do banco de dados em bytes
const getDatabaseSize = async () => {
    try {
        const result = await query(
            `SELECT pg_database_size($1) as size`,
            [process.env.PGDATABASE]
        );
        return parseInt(result.rows[0].size, 10);
    } catch (err) {
        console.error('❌ Erro ao obter tamanho do banco:', err.message);
        return 0;
    }
};

// Formata bytes para formato legível
const formatBytes = (bytes) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
};

module.exports = {
    isPostgresEnabled,
    getPool,
    testConnection,
    query,
    transaction,
    closePool,
    getDatabaseSize,
    formatBytes
};
