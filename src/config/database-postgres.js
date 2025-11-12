// =================================================================
// POSTGRESQL DATABASE WRAPPER - Compatível com interface SQLite
// =================================================================
const { getPool, query: pgQuery } = require('./postgresql');
const bcrypt = require('bcrypt');

let isInitialized = false;

function initDatabase(callback) {
    const pool = getPool();
    if (!pool) {
        const error = new Error('PostgreSQL não está configurado');
        return callback ? callback(error) : console.error(error);
    }
    
    console.log('✅ PostgreSQL inicializado (schema já existe)');
    
    // Verificar e criar usuário admin se não existir
    ensureAdminUser(callback);
}

async function ensureAdminUser(callback) {
    try {
        const result = await pgQuery('SELECT COUNT(*) as count FROM usuarios WHERE username = $1', ['admin']);
        
        if (result.rows[0].count === 0) {
            console.log('Criando usuário admin padrão...');
            const hashedPassword = await bcrypt.hash('admin', 10);
            await pgQuery(
                'INSERT INTO usuarios (username, password, role, password_hashed) VALUES ($1, $2, $3, $4)',
                ['admin', hashedPassword, 'admin', 1]
            );
            console.log('✅ Usuário admin criado');
        }
        
        isInitialized = true;
        if (callback) callback(null);
    } catch (error) {
        console.error('Erro ao verificar/criar admin:', error);
        if (callback) callback(error);
    }
}

// Wrapper que emula a interface do SQLite
class PostgreSQLWrapper {
    constructor() {
        this.pool = getPool();
    }
    
    // Emula db.run() do SQLite
    run(sql, params = [], callback) {
        // Converte AUTOINCREMENT para SERIAL no SQL, ? para $1, $2, etc
        sql = this.convertSQL(sql, params);
        const { convertedSQL, convertedParams } = this.convertParams(sql, params);
        
        // Adiciona RETURNING id para INSERT statements
        let finalSQL = convertedSQL;
        if (/^\s*INSERT\s+INTO/i.test(convertedSQL) && !/RETURNING/i.test(convertedSQL)) {
            finalSQL = convertedSQL + ' RETURNING id';
        }
        
        pgQuery(finalSQL, convertedParams)
            .then(result => {
                if (callback) {
                    // Retorna no formato do SQLite: function(err, result)
                    // result tem lastID e changes
                    callback.call({ 
                        lastID: result.rows[0]?.id || null, 
                        changes: result.rowCount 
                    }, null);
                }
            })
            .catch(err => {
                if (callback) callback.call(this, err);
            });
    }
    
    // Emula db.get() do SQLite
    get(sql, params = [], callback) {
        sql = this.convertSQL(sql, params);
        const { convertedSQL, convertedParams } = this.convertParams(sql, params);
        
        pgQuery(convertedSQL, convertedParams)
            .then(result => {
                if (callback) callback(null, result.rows[0] || null);
            })
            .catch(err => {
                if (callback) callback(err, null);
            });
    }
    
    // Emula db.all() do SQLite
    all(sql, params = [], callback) {
        sql = this.convertSQL(sql, params);
        const { convertedSQL, convertedParams } = this.convertParams(sql, params);
        
        pgQuery(convertedSQL, convertedParams)
            .then(result => {
                if (callback) callback(null, result.rows || []);
            })
            .catch(err => {
                if (callback) callback(err, []);
            });
    }
    
    // Converte SQL do SQLite para PostgreSQL
    convertSQL(sql, params) {
        // Remove AUTOINCREMENT (PostgreSQL usa SERIAL)
        sql = sql.replace(/INTEGER PRIMARY KEY AUTOINCREMENT/gi, 'SERIAL PRIMARY KEY');
        sql = sql.replace(/AUTOINCREMENT/gi, '');
        
        // Converte DATETIME para TIMESTAMP
        sql = sql.replace(/DATETIME/gi, 'TIMESTAMP');
        
        // Converte funções de data/hora do SQLite para PostgreSQL
        sql = sql.replace(/datetime\s*\(\s*['"]now['"]\s*\)/gi, 'NOW()');
        sql = sql.replace(/date\s*\(\s*['"]now['"]\s*\)/gi, 'CURRENT_DATE');
        sql = sql.replace(/time\s*\(\s*['"]now['"]\s*\)/gi, 'CURRENT_TIME');
        sql = sql.replace(/CURRENT_TIMESTAMP/gi, 'NOW()');
        
        // Converte IF NOT EXISTS em ALTER TABLE
        if (sql.includes('ALTER TABLE') && !sql.includes('IF EXISTS')) {
            // PostgreSQL pode precisar de ADD COLUMN IF NOT EXISTS
            sql = sql.replace(/ADD COLUMN (\w+)/gi, 'ADD COLUMN IF NOT EXISTS $1');
        }
        
        return sql;
    }
    
    // Converte parâmetros ? para $1, $2, etc (formato PostgreSQL)
    convertParams(sql, params) {
        let convertedSQL = sql;
        const convertedParams = [];
        
        if (Array.isArray(params) && params.length > 0) {
            let paramIndex = 1;
            convertedSQL = sql.replace(/\?/g, () => `$${paramIndex++}`);
            convertedParams.push(...params);
        }
        
        return { convertedSQL, convertedParams };
    }
    
    // Método serialize não é necessário no PostgreSQL
    serialize(callback) {
        if (callback) callback();
    }
    
    // Fecha a conexão
    close(callback) {
        // PostgreSQL usa pool, não fecha individual
        if (callback) callback();
    }
}

// Cria instância global
const db = new PostgreSQLWrapper();

function getDatabase() {
    return db;
}

module.exports = {
    initDatabase,
    getDatabase,
    DB_PATH: 'PostgreSQL@' + (process.env.PGHOST || 'unknown')
};
