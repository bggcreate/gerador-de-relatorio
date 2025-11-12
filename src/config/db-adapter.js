// =================================================================
// DATABASE ADAPTER - Suporte automático para SQLite e PostgreSQL
// =================================================================
const postgresConfig = require('./postgresql');

const USE_POSTGRES = postgresConfig.isPostgresEnabled();

if (USE_POSTGRES) {
    console.log('🐘 Usando PostgreSQL (Tembo.io)');
} else {
    console.log('📁 Usando SQLite local');
}

// Exporta o banco de dados apropriado baseado na configuração
if (USE_POSTGRES) {
    module.exports = require('./database-postgres');
} else {
    module.exports = require('./database');
}
