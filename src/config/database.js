const sqlite3 = require('sqlite3').verbose();
const path = require('path');
const fs = require('fs');

const dataDir = path.join(__dirname, '../../data');
if (!fs.existsSync(dataDir)) {
    fs.mkdirSync(dataDir, { recursive: true });
}

const DB_FILENAME = process.env.DB_PATH || 'database.db';
const DB_PATH = path.join(dataDir, DB_FILENAME);

let db = null;

function initDatabase(callback) {
    console.log(`📁 Usando banco de dados: ${DB_PATH}`);
    
    db = new sqlite3.Database(DB_PATH, (err) => {
        if (err) {
            console.error("Erro fatal ao conectar ao DB:", err.message);
            return callback(err);
        }
        console.log("Conectado ao banco de dados SQLite.");
        createTables(callback);
    });
}

function createTables(callback) {
    db.serialize(() => {
        db.run(`CREATE TABLE IF NOT EXISTS usuarios (
            id INTEGER PRIMARY KEY AUTOINCREMENT, 
            username TEXT UNIQUE NOT NULL, 
            password TEXT NOT NULL, 
            role TEXT NOT NULL,
            loja_gerente TEXT,
            lojas_consultor TEXT
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS lojas (
            id INTEGER PRIMARY KEY AUTOINCREMENT, 
            nome TEXT UNIQUE NOT NULL, 
            status TEXT, 
            funcao_especial TEXT, 
            observacoes TEXT
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS relatorios (
            id INTEGER PRIMARY KEY AUTOINCREMENT, 
            loja TEXT, 
            data TEXT, 
            hora_abertura TEXT, 
            hora_fechamento TEXT,
            gerente_entrada TEXT, 
            gerente_saida TEXT, 
            clientes_monitoramento INTEGER, 
            vendas_monitoramento INTEGER,
            clientes_loja INTEGER, 
            vendas_loja INTEGER, 
            total_vendas_dinheiro REAL, 
            ticket_medio TEXT, 
            pa TEXT,
            quantidade_trocas INTEGER, 
            nome_funcao_especial TEXT, 
            quantidade_funcao_especial INTEGER,
            quantidade_omni INTEGER, 
            vendedores TEXT, 
            nome_arquivo TEXT, 
            enviado_por_usuario TEXT,
            enviado_em DATETIME DEFAULT CURRENT_TIMESTAMP, 
            vendas_cartao INTEGER, 
            vendas_pix INTEGER, 
            vendas_dinheiro INTEGER
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS demandas (
            id INTEGER PRIMARY KEY AUTOINCREMENT, 
            loja_nome TEXT NOT NULL, 
            descricao TEXT NOT NULL, 
            tag TEXT DEFAULT 'Normal', 
            status TEXT DEFAULT 'pendente', 
            criado_por_usuario TEXT, 
            concluido_por_usuario TEXT, 
            criado_em DATETIME DEFAULT CURRENT_TIMESTAMP, 
            concluido_em DATETIME
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS vendedores (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            loja_id INTEGER NOT NULL,
            nome TEXT NOT NULL,
            telefone TEXT NOT NULL,
            data_entrada TEXT NOT NULL,
            data_demissao TEXT,
            previsao_entrada TEXT,
            previsao_saida TEXT,
            ativo INTEGER DEFAULT 1,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (loja_id) REFERENCES lojas(id) ON DELETE CASCADE
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS logs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
            type TEXT NOT NULL,
            username TEXT,
            action TEXT,
            details TEXT
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS temp_tokens (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            token_hash TEXT UNIQUE NOT NULL,
            role TEXT DEFAULT 'dev',
            expira_em DATETIME NOT NULL,
            ip_origem TEXT,
            ip_restrito TEXT,
            revogado INTEGER DEFAULT 0,
            criado_por TEXT,
            criado_em DATETIME DEFAULT CURRENT_TIMESTAMP,
            usado_em DATETIME,
            revogado_em DATETIME,
            revogado_por TEXT
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS estoque_tecnico (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            nome_peca TEXT NOT NULL,
            codigo_interno TEXT UNIQUE NOT NULL,
            quantidade INTEGER DEFAULT 0,
            valor_custo REAL DEFAULT 0,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS assistencias (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            cliente_nome TEXT NOT NULL,
            cliente_cpf TEXT NOT NULL,
            numero_pedido TEXT,
            data_entrada TEXT NOT NULL,
            data_conclusao TEXT,
            valor_peca_loja REAL DEFAULT 0,
            valor_servico_cliente REAL DEFAULT 0,
            aparelho TEXT NOT NULL,
            peca_id INTEGER,
            peca_nome TEXT,
            observacoes TEXT,
            status TEXT DEFAULT 'Em andamento',
            tecnico_responsavel TEXT,
            loja TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (peca_id) REFERENCES estoque_tecnico(id)
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS pdf_tickets (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            loja TEXT NOT NULL,
            data TEXT NOT NULL,
            filename TEXT NOT NULL,
            filepath TEXT NOT NULL,
            uploaded_by TEXT NOT NULL,
            uploaded_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS pdf_rankings (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            loja TEXT NOT NULL,
            data TEXT NOT NULL,
            filename TEXT NOT NULL,
            filepath TEXT NOT NULL,
            pa TEXT,
            preco_medio TEXT,
            atendimento_medio TEXT,
            uploaded_by TEXT,
            uploaded_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS db_backups (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            filename TEXT NOT NULL,
            filepath TEXT,
            size_bytes INTEGER NOT NULL,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            sent_to_email INTEGER DEFAULT 0,
            email_sent_at DATETIME,
            backup_type TEXT DEFAULT 'manual',
            notes TEXT,
            drive_file_id TEXT,
            created_by TEXT,
            status TEXT DEFAULT 'success',
            error_message TEXT
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS system_settings (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            setting_key TEXT UNIQUE NOT NULL,
            setting_value TEXT,
            updated_by TEXT,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )`, (err) => {
            if (callback) callback(err);
        });
    });
}

function getDatabase() {
    return db;
}

module.exports = {
    initDatabase,
    getDatabase,
    DB_PATH
};
