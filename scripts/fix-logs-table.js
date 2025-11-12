const { Pool } = require('pg');

const pool = new Pool({
    host: process.env.PGHOST,
    port: process.env.PGPORT || 5432,
    database: process.env.PGDATABASE,
    user: process.env.PGUSER,
    password: process.env.PGPASSWORD,
    ssl: false
});

async function addColumns() {
    const client = await pool.connect();
    try {
        console.log('Adicionando colunas extras na tabela logs...');
        
        await client.query(`
            ALTER TABLE logs 
            ADD COLUMN IF NOT EXISTS ip_address TEXT,
            ADD COLUMN IF NOT EXISTS user_agent TEXT,
            ADD COLUMN IF NOT EXISTS event_type TEXT,
            ADD COLUMN IF NOT EXISTS route TEXT,
            ADD COLUMN IF NOT EXISTS payload_hash TEXT
        `);
        
        console.log('✅ Colunas adicionadas com sucesso!');
    } catch (error) {
        console.error('Erro:', error.message);
    } finally {
        client.release();
        await pool.end();
    }
}

addColumns();
