const crypto = require('crypto');

const SESSION_SECRET = process.env.SESSION_SECRET || crypto.randomBytes(64).toString('hex');
if (!process.env.SESSION_SECRET) {
    console.warn('⚠️  ATENÇÃO: SESSION_SECRET não configurado. Usando um secret gerado automaticamente.');
    console.warn('⚠️  Para produção, configure a variável de ambiente SESSION_SECRET.');
}

const JWT_SECRET = process.env.JWT_SECRET || crypto.randomBytes(64).toString('hex');
if (!process.env.JWT_SECRET) {
    console.warn('⚠️  ATENÇÃO: JWT_SECRET não configurado. Usando um secret gerado automaticamente.');
    console.warn('⚠️  Para produção, configure a variável de ambiente JWT_SECRET.');
}

const DEV_TEMP_ACCESS_ENABLED = process.env.DEV_TEMP_ACCESS === 'true' && (process.env.NODE_ENV === 'development' || !process.env.NODE_ENV);
if (DEV_TEMP_ACCESS_ENABLED) {
    console.log('🔓 Acesso temporário de desenvolvimento HABILITADO');
    console.warn('⚠️  ATENÇÃO: Desabilite DEV_TEMP_ACCESS antes de fazer deploy em produção!');
} else {
    console.log('🔒 Acesso temporário de desenvolvimento DESABILITADO');
}

module.exports = {
    PORT: process.env.PORT || 5000,
    SESSION_SECRET,
    JWT_SECRET,
    DEV_TEMP_ACCESS_ENABLED,
    NODE_ENV: process.env.NODE_ENV || 'development'
};
