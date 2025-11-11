// =================================================================
// SISTEMA DE FLUXO - teste
// =================================================================
require('dotenv').config();
const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const path = require('path');
const session = require('express-session');
const fs = require('fs');
const crypto = require('crypto');
const PDFDocument = require('pdfkit');
const ExcelJS = require('exceljs');
const multer = require('multer');
const pdf = require('pdf-parse');
const bcrypt = require('bcrypt');
const helmet = require('helmet');
const rateLimit = require('express-rate-limit');
const { body, validationResult } = require('express-validator');
const { requireAuth, requireAuthPage, getLojaFilter, getPermissions } = require('./middleware/roleAuth');
const jwt = require('jsonwebtoken');
const DVRService = require('./services/dvrService');
const IntelbrasDvrService = require('./services/intelbrasDvrService');
const googleDriveService = require('./services/googleDriveService');

const app = express();
const PORT = process.env.PORT || 5000;

app.set('trust proxy', true);

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

// --- CONFIGURAÇÃO GERAL ---
const dataDir = path.join(__dirname, 'data');
if (!fs.existsSync(dataDir)) {
    fs.mkdirSync(dataDir, { recursive: true });
}
// Caminho configurável do banco de dados (pode ser sobrescrito por variável de ambiente)
const DB_FILENAME = process.env.DB_PATH || 'database.db';
const DB_PATH = path.join(dataDir, DB_FILENAME);
console.log(`📁 Usando banco de dados: ${DB_PATH}`);

app.use(helmet({
    contentSecurityPolicy: {
        directives: {
            defaultSrc: ["'self'"],
            scriptSrc: ["'self'", "'unsafe-inline'", "https://cdn.jsdelivr.net"],
            styleSrc: ["'self'", "'unsafe-inline'", "https://cdn.jsdelivr.net", "https://fonts.googleapis.com"],
            imgSrc: ["'self'", "data:", "https:"],
            connectSrc: ["'self'"],
            fontSrc: ["'self'", "https://cdn.jsdelivr.net", "https://fonts.gstatic.com"],
            objectSrc: ["'none'"],
            mediaSrc: ["'self'"],
            frameSrc: ["'self'", "blob:"],
        },
    },
    crossOriginEmbedderPolicy: false,
}));

app.use(express.static(path.join(__dirname, 'public')));
app.use(express.urlencoded({ extended: true }));
app.use(express.json());
app.use(session({
    secret: SESSION_SECRET,
    resave: false,
    saveUninitialized: true,
    cookie: { 
        httpOnly: true, 
        secure: false,
        sameSite: 'lax',
        maxAge: 24 * 60 * 60 * 1000
    },
    name: 'sessionId',
    proxy: true
}));

// --- CONFIGURAÇÃO DO MULTER ---
const upload = multer({ storage: multer.memoryStorage() });

// --- MIDDLEWARES ---
const generateCsrfToken = () => crypto.randomBytes(32).toString('hex');

const csrfProtection = (req, res, next) => {
    if (!req.session.csrfToken) {
        req.session.csrfToken = generateCsrfToken();
    }
    next();
};

const validateCsrf = (req, res, next) => {
    // CSRF DESATIVADO - apenas passa adiante
    next();
};

const auditMiddleware = (req, res, next) => {
    const mutatingMethods = ['POST', 'PUT', 'DELETE'];
    
    if (req.session && req.session.username) {
        if (mutatingMethods.includes(req.method) && !req.path.includes('/api/login')) {
            const action = `${req.method} ${req.path}`;
            const details = `Ação executada por usuário autenticado`;
            logEvent('audit', req.session.username, action, details, req);
        }
    }
    next();
};

// CSRF DESATIVADO
// app.use(csrfProtection);
app.use(auditMiddleware);

const requirePageLogin = (req, res, next) => {
    if (req.session && req.session.userId) {
        return next();
    }
    res.redirect('/login');
};

// --- BANCO DE DADOS ---
let db = new sqlite3.Database(DB_PATH, err => {
    if (err) {
        return console.error("Erro fatal ao conectar ao DB:", err.message);
    }
    console.log("Conectado ao banco de dados SQLite.");
    db.serialize(() => {
        db.run(`CREATE TABLE IF NOT EXISTS usuarios (
            id INTEGER PRIMARY KEY AUTOINCREMENT, 
            username TEXT UNIQUE NOT NULL, 
            password TEXT NOT NULL, 
            role TEXT NOT NULL,
            loja_gerente TEXT,
            lojas_consultor TEXT
        )`);
        db.run(`CREATE TABLE IF NOT EXISTS lojas (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT UNIQUE NOT NULL, status TEXT, funcao_especial TEXT, observacoes TEXT)`);
        db.run(`CREATE TABLE IF NOT EXISTS relatorios (
            id INTEGER PRIMARY KEY AUTOINCREMENT, loja TEXT, data TEXT, hora_abertura TEXT, hora_fechamento TEXT,
            gerente_entrada TEXT, gerente_saida TEXT, clientes_monitoramento INTEGER, vendas_monitoramento INTEGER,
            clientes_loja INTEGER, vendas_loja INTEGER, total_vendas_dinheiro REAL, ticket_medio TEXT, pa TEXT,
            quantidade_trocas INTEGER, nome_funcao_especial TEXT, quantidade_funcao_especial INTEGER,
            quantidade_omni INTEGER, vendedores TEXT, nome_arquivo TEXT, enviado_por_usuario TEXT,
            enviado_em DATETIME DEFAULT CURRENT_TIMESTAMP, vendas_cartao INTEGER, vendas_pix INTEGER, vendas_dinheiro INTEGER
        )`);
        db.run(`CREATE TABLE IF NOT EXISTS demandas (id INTEGER PRIMARY KEY AUTOINCREMENT, loja_nome TEXT NOT NULL, descricao TEXT NOT NULL, tag TEXT DEFAULT 'Normal', status TEXT DEFAULT 'pendente', criado_por_usuario TEXT, concluido_por_usuario TEXT, criado_em DATETIME DEFAULT CURRENT_TIMESTAMP, concluido_em DATETIME)`);
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
        
        // Tabelas de Assistência Técnica
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
        
        // Tabela para armazenar PDFs de Ticket Dia
        db.run(`CREATE TABLE IF NOT EXISTS pdf_tickets (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            loja TEXT NOT NULL,
            data TEXT NOT NULL,
            filename TEXT NOT NULL,
            filepath TEXT NOT NULL,
            uploaded_by TEXT NOT NULL,
            uploaded_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )`);
        
        // Tabela para armazenar PDFs de Ranking
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
        
        // Tabelas para Módulo DVR/NVR Intelbras
        db.run(`CREATE TABLE IF NOT EXISTS dvr_dispositivos (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            nome TEXT NOT NULL,
            loja_id INTEGER,
            loja_nome TEXT,
            ip_address TEXT NOT NULL,
            porta INTEGER DEFAULT 37777,
            usuario TEXT,
            modelo TEXT,
            canais_total INTEGER DEFAULT 0,
            status TEXT DEFAULT 'offline',
            ultima_conexao DATETIME,
            observacoes TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (loja_id) REFERENCES lojas(id) ON DELETE SET NULL
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS dvr_logs (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            dvr_id INTEGER NOT NULL,
            dvr_nome TEXT,
            loja_nome TEXT,
            tipo_evento TEXT NOT NULL,
            descricao TEXT,
            canal INTEGER,
            severidade TEXT DEFAULT 'info',
            data_hora DATETIME DEFAULT CURRENT_TIMESTAMP,
            detalhes_json TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (dvr_id) REFERENCES dvr_dispositivos(id) ON DELETE CASCADE
        )`);
        
        db.run(`CREATE TABLE IF NOT EXISTS dvr_arquivos (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            dvr_id INTEGER NOT NULL,
            dvr_nome TEXT,
            loja_nome TEXT,
            tipo_arquivo TEXT NOT NULL,
            nome_arquivo TEXT NOT NULL,
            caminho_arquivo TEXT NOT NULL,
            tamanho_bytes INTEGER,
            data_geracao DATETIME,
            canal INTEGER,
            inicio_gravacao DATETIME,
            fim_gravacao DATETIME,
            descricao TEXT,
            uploaded_by TEXT,
            uploaded_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (dvr_id) REFERENCES dvr_dispositivos(id) ON DELETE CASCADE
        )`);
        
        // Adicionar colunas caso não existam (migração)
        db.run(`ALTER TABLE usuarios ADD COLUMN loja_gerente TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar loja_gerente:', err.message);
        });
        db.run(`ALTER TABLE usuarios ADD COLUMN lojas_consultor TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar lojas_consultor:', err.message);
        });
        db.run(`ALTER TABLE usuarios ADD COLUMN loja_tecnico TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar loja_tecnico:', err.message);
        });
        db.run(`ALTER TABLE lojas ADD COLUMN tecnico_username TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar tecnico_username:', err.message);
        });
        db.run(`ALTER TABLE lojas ADD COLUMN cargo TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar cargo:', err.message);
        });
        db.run(`ALTER TABLE lojas ADD COLUMN cep TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar cep:', err.message);
        });
        db.run(`ALTER TABLE lojas ADD COLUMN numero_contato TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar numero_contato:', err.message);
        });
        db.run(`ALTER TABLE lojas ADD COLUMN gerente TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar gerente:', err.message);
        });
        db.run(`ALTER TABLE estoque_tecnico ADD COLUMN loja TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar loja em estoque_tecnico:', err.message);
        });
        
        db.run(`ALTER TABLE usuarios ADD COLUMN password_hashed INTEGER DEFAULT 0`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar password_hashed:', err.message);
        });
        
        db.run(`ALTER TABLE logs ADD COLUMN ip_address TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar ip_address em logs:', err.message);
        });
        db.run(`ALTER TABLE logs ADD COLUMN user_agent TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar user_agent em logs:', err.message);
        });
        db.run(`ALTER TABLE logs ADD COLUMN event_type TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar event_type em logs:', err.message);
        });
        db.run(`ALTER TABLE logs ADD COLUMN route TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar route em logs:', err.message);
        });
        db.run(`ALTER TABLE logs ADD COLUMN payload_hash TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar payload_hash em logs:', err.message);
        });
        
        // Adicionar coluna de permissões customizadas
        db.run(`ALTER TABLE usuarios ADD COLUMN custom_permissions TEXT`, (err) => {
            if (err && !err.message.includes('duplicate column')) console.error('Erro ao adicionar custom_permissions:', err.message);
        });
        
        const adminUsername = 'admin';
        const correctPassword = 'admin';
        db.get('SELECT * FROM usuarios WHERE username = ?', [adminUsername], (err, row) => {
            if (err) return;
            if (!row) {
                db.run('INSERT INTO usuarios (username, password, role) VALUES (?, ?, ?)', [adminUsername, correctPassword, 'admin']);
            } else if (row.password !== correctPassword) {
                db.run('UPDATE usuarios SET password = ? WHERE username = ?', [correctPassword, adminUsername]);
            }
        });
    });
});

// Inicializar serviço DVR após conexão do banco
let dvrService;
if (db) {
    dvrService = new DVRService(db);
}

// =================================================================
// SISTEMA DE TOKENS JWT TEMPORÁRIOS PARA DESENVOLVIMENTO
// =================================================================

function generateTempToken(expiresInHours = 1, ipRestricted = null) {
    const tokenId = crypto.randomBytes(16).toString('hex');
    const payload = {
        tokenId,
        role: 'dev',
        type: 'temp_access',
        iat: Math.floor(Date.now() / 1000)
    };
    
    const token = jwt.sign(payload, JWT_SECRET, { 
        expiresIn: `${expiresInHours}h`,
        issuer: 'dev-temp-access'
    });
    
    const tokenHash = crypto.createHash('sha256').update(token).digest('hex');
    
    return { token, tokenHash, tokenId, expiresInHours };
}

function verifyTempToken(token) {
    try {
        const decoded = jwt.verify(token, JWT_SECRET, { 
            issuer: 'dev-temp-access' 
        });
        return { valid: true, decoded };
    } catch (error) {
        return { valid: false, error: error.message };
    }
}

async function saveTempTokenToDb(tokenHash, expiresInHours, ipOrigem, ipRestrito, criadoPor) {
    return new Promise((resolve, reject) => {
        const expiraEm = new Date(Date.now() + expiresInHours * 60 * 60 * 1000).toISOString();
        
        db.run(
            `INSERT INTO temp_tokens (token_hash, role, expira_em, ip_origem, ip_restrito, criado_por) 
             VALUES (?, ?, ?, ?, ?, ?)`,
            [tokenHash, 'dev', expiraEm, ipOrigem, ipRestrito, criadoPor],
            function(err) {
                if (err) reject(err);
                else resolve(this.lastID);
            }
        );
    });
}

async function validateTempTokenInDb(token, currentIp) {
    return new Promise((resolve, reject) => {
        const tokenHash = crypto.createHash('sha256').update(token).digest('hex');
        
        db.get(
            `SELECT * FROM temp_tokens WHERE token_hash = ? AND revogado = 0 AND datetime(expira_em) > datetime('now')`,
            [tokenHash],
            (err, row) => {
                if (err) return reject(err);
                if (!row) return resolve({ valid: false, reason: 'Token não encontrado ou expirado' });
                
                if (row.ip_restrito && row.ip_restrito !== currentIp) {
                    return resolve({ valid: false, reason: 'IP não autorizado' });
                }
                
                db.run(
                    `UPDATE temp_tokens SET usado_em = datetime('now') WHERE id = ?`,
                    [row.id],
                    (err) => {
                        if (err) console.error('Erro ao atualizar usado_em:', err);
                    }
                );
                
                resolve({ valid: true, tokenData: row });
            }
        );
    });
}

const tempTokenAuthMiddleware = async (req, res, next) => {
    const authHeader = req.headers.authorization;
    
    if (!authHeader || !authHeader.startsWith('Bearer ')) {
        return next();
    }
    
    const token = authHeader.substring(7);
    
    const jwtVerification = verifyTempToken(token);
    if (!jwtVerification.valid) {
        return res.status(401).json({ error: 'Token inválido ou expirado' });
    }
    
    const currentIp = getClientIp(req);
    const dbValidation = await validateTempTokenInDb(token, currentIp);
    
    if (!dbValidation.valid) {
        logEvent('security', 'temp_token', 'token_rejected', dbValidation.reason, req);
        return res.status(401).json({ error: dbValidation.reason });
    }
    
    req.session.userId = -1;
    req.session.username = 'temp_dev_access';
    req.session.tempToken = true;
    
    logEvent('auth', 'temp_dev_access', 'temp_token_used', `Token temporário utilizado (IP: ${currentIp})`, req);
    
    next();
};

function getClientIp(req) {
    return req.headers['x-forwarded-for']?.split(',')[0].trim() || 
           req.headers['x-real-ip'] || 
           req.connection?.remoteAddress || 
           req.socket?.remoteAddress ||
           'unknown';
}

app.use(tempTokenAuthMiddleware);

// --- ROTAS DE PÁGINAS ---
app.get('/login', (req, res) => res.sendFile(path.join(__dirname, 'views', 'login.html')));
app.get('/403', (req, res) => res.sendFile(path.join(__dirname, 'views', '403.html')));
app.get('/live', requirePageLogin, (req, res) => res.sendFile(path.join(__dirname, 'views', 'live.html')));

// Dashboard - todos podem acessar
app.get(['/', '/admin'], requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Consulta - todos podem acessar
app.get('/consulta', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Novo Relatório - apenas monitoramento, admin e dev
app.get('/novo-relatorio', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Lojas - todos exceto consultor (mas consultor pode via demandas)
app.get('/gerenciar-lojas', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Demandas - todos podem acessar
app.get('/demandas', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Gerenciar Usuários - apenas admin e dev
app.get('/gerenciar-usuarios', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Logs - apenas dev
app.get('/logs', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// DVR/NVR Monitor - todos podem acessar
app.get('/dvr-monitor', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Backup - apenas admin e dev
app.get('/backup', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

app.get('/dev/system', requirePageLogin, (req, res) => {
    logEvent('dev_access', req.session.username, 'system_access', 'Usuário acessou painel de sistema', req);
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

app.get('/content/:page', requirePageLogin, (req, res) => {
    const allowedPages = ['admin', 'consulta', 'demandas', 'gerenciar-lojas', 'assistencia', 'alertas-tecnico', 'novo-relatorio', 'gerenciar-usuarios', 'logs', 'dvr-monitor'];
    if (allowedPages.includes(req.params.page)) {
        res.sendFile(path.join(__dirname, 'views', `${req.params.page}.html`));
    } else {
        res.status(404).send('Página não encontrada');
    }
});


// --- ROTAS DE API ---

// <<<---------------------------------------------------->>>
// <<<                    leitura PDF                     >>>
// <<<---------------------------------------------------->>>
app.post('/api/process-pdf', requirePageLogin, upload.single('pdfFile'), async (req, res) => {
    if (!req.file) {
        return res.status(400).json({ error: "Nenhum arquivo PDF enviado." });
    }

    try {
        const data = await pdf(req.file.buffer);
        const text = data.text;
        const lines = text.split('\n').map(line => line.trim()).filter(Boolean);

        // Função para converter valores no formato "1.234,56" para número
        const parseBrazilianNumber = (str) => {
            if (!str) return 0;
            return parseFloat(String(str).replace(/\./g, '').replace(',', '.'));
        };

        let extractedData;

        // VERIFICA O TIPO DE PDF
        if (text.includes("Desempenho de vendedores")) {
            // --- LÓGICA PARA PDF TIPO OMNI ---
            console.log("Processando PDF estilo Omni (Desempenho de vendedores)...");

            const linhaTotais = lines.find(l => l.startsWith('Totais:'));
            if (!linhaTotais) {
                throw new Error("Linha 'Totais:' não encontrada no PDF.");
            }

            // Extrai todos os números da linha de totais
            const valores = linhaTotais.replace('Totais:', '').trim().split(/\s+/);
            
            // CORREÇÃO: Procura o cabeçalho para identificar as colunas corretamente
            const headerLine = lines.find(l => l.includes('Peças/Venda') || l.includes('P.A'));
            
            // Mapeia os valores com validação melhorada
            const vendas_loja = Math.round(parseBrazilianNumber(valores[1])); // Total de Vendas
            
            // CORREÇÃO PARA PA = 1: Procura por valores decimais pequenos (PA geralmente é entre 0.5 e 5.0)
            let pa = parseBrazilianNumber(valores[2]); // Posição padrão
            
            // Validação: se o PA não está em um range razoável, procura no array
            if (pa < 0.3 || pa > 10) {
                // Procura um valor decimal entre 0.3 e 10 (range típico de PA)
                for (let i = 0; i < valores.length; i++) {
                    const valor = parseBrazilianNumber(valores[i]);
                    if (valor >= 0.3 && valor <= 10 && valores[i].includes(',')) {
                        pa = valor;
                        console.log(`PA ajustado de ${valores[2]} para ${valores[i]} (valor: ${pa})`);
                        break;
                    }
                }
            }
            
            const total_vendas_dinheiro = parseBrazilianNumber(valores[3]); // Vl. Vendas
            const ticket_medio = parseBrazilianNumber(valores[4]); // Ticket Médio
            const clientes_loja = parseInt(valores[5], 10); // Abordagens

            // Extrai dados 
            const storeNameMatch = text.match(/(\d{1,}-\d{6}-.+)/);
            const storeName = storeNameMatch ? storeNameMatch[1].trim() : "Loja não identificada";
            
            const dateMatch = text.match(/Período: (\d{2}\/\d{2}\/\d{4})/);
            const reportDate = dateMatch ? new Date(dateMatch[1].split('/').reverse().join('-')).toISOString().split('T')[0] : null;

            const vendorLines = lines.filter(line => line.match(/^\d+\s+.+\s+\(\d+\)/));
            const vendedores = vendorLines.map(line => {
                const nameMatch = line.match(/^\d+\s+(.*?)\s+\(\d+\)/);
                const nome = nameMatch ? nameMatch[1].trim() : "Desconhecido";
                
                const statsPart = line.replace(nameMatch[0], '').trim();
                const stats = statsPart.split(/\s+/);
                
                const vendas = Math.round(parseBrazilianNumber(stats[1]));
                const atendimentos = parseInt(stats[5], 10) || 0;
                
                return { nome, vendas, atendimentos };
            });

            extractedData = {
                loja: storeName,
                data: reportDate,
                clientes_loja: clientes_loja,
                vendas_loja: vendas_loja,
                total_vendas_dinheiro: `R$ ${total_vendas_dinheiro.toFixed(2).replace('.', ',')}`,
                ticket_medio: `R$ ${ticket_medio.toFixed(2).replace('.', ',')}`,
                pa: pa.toFixed(2).replace('.', ','),
                vendedores: vendedores
            };

        } else {
            // --- LÓGICA PARA PDF TIPO BUSCA TÉCNICA ---
            console.log("Processando PDF estilo Busca Técnica...");
            
            const linhaTotais = lines.find(l => l.includes('Totais:'));
            const idxTotais = lines.indexOf(linhaTotais);
            const linhaDados = lines[idxTotais + 1] || '';
            const linhaLimpa = linhaDados.replace(/(\d{1,3})\.(\d{3},\d{2})/g, '$1.$2 ').replace(/ +/g, ' ').trim();
            const valoresTotais = linhaLimpa.match(/(\d{1,3}(?:\.\d{3})*,\d{2})|(\d+\.\d{2})|(\d+,\d{2})|(\d+)/g);
            
            if (!valoresTotais || valoresTotais.length < 7) {
                throw new Error("Não foi possível extrair os valores corretamente da linha Totais do PDF.");
            }
            
            const totalVendasValor = parseBrazilianNumber(valoresTotais[0]);
            
            // CORREÇÃO PARA PA = 1: Busca valores decimais típicos de PA
            let pa = parseBrazilianNumber(valoresTotais[valoresTotais.length - 4]); // Posição padrão
            let ticketMedio = parseBrazilianNumber(valoresTotais[valoresTotais.length - 3]);
            
            // Validação inteligente do PA
            // PA geralmente está entre 0.3 e 10, e tem formato decimal (ex: 1,00 ou 2,50)
            const possiveisPAs = valoresTotais
                .map((v, idx) => ({ valor: parseBrazilianNumber(v), original: v, index: idx }))
                .filter(item => {
                    // Filtra valores que parecem PA: decimal entre 0.3 e 10
                    return item.valor >= 0.3 && item.valor <= 10 && 
                           (item.original.includes(',') || item.original.includes('.'));
                });
            
            // Se encontrou candidatos a PA, pega o mais provável
            if (possiveisPAs.length > 0 && (pa < 0.3 || pa > 10)) {
                // Prefere valores próximos ao final do array (geralmente onde está o PA)
                pa = possiveisPAs[possiveisPAs.length - 1].valor;
                console.log(`PA ajustado para: ${pa} (encontrado em possiveisPAs)`);
            }
            
            // Validação do Ticket Médio (geralmente é um valor maior, acima de 50 reais)
            if (ticketMedio < 10) {
                // Procura um valor maior que pareça ticket médio
                const possivelTicket = valoresTotais
                    .map(v => parseBrazilianNumber(v))
                    .filter(v => v >= 10 && v <= 10000)
                    .find(v => v > 50); // Ticket médio geralmente > 50 reais
                
                if (possivelTicket) {
                    ticketMedio = possivelTicket;
                    console.log(`Ticket médio ajustado para: ${ticketMedio}`);
                }
            }
            
            const linhaSplitada = linhaLimpa.split(' ');
            const indexDoValorTotal = linhaSplitada.findIndex(v => v.includes(valoresTotais[0]));
            const totalAtendimentos = parseInt(linhaSplitada[indexDoValorTotal + 2], 10) || 0;
            
            const storeNameMatch = text.match(/^\s*\d{3}\s*-\s*(.+)/m);
            const storeName = storeNameMatch ? storeNameMatch[1].trim().replace(/\s+STORE$/, "") : "Loja não identificada";
            
            const dateMatch = text.match(/Período de (\d{2}\/\d{2}\/\d{4}) a (\d{2}\/\d{2}\/\d{4})/);
            const reportDate = dateMatch ? new Date(dateMatch[1].split('/').reverse().join('-')).toISOString().split('T')[0] : null;

            const vendorLines = lines.filter(line => /^\d+º/.test(line));
            const vendedores = vendorLines.map(line => {
                const vendorParts = line.trim().split(/\s+/);
                const nome = vendorParts.slice(2, -7).join(' ');
                const atendimentos = parseInt(vendorParts[vendorParts.length - 4], 10) || 0;
                return { nome, vendas: atendimentos, atendimentos };
            });

            extractedData = {
                loja: storeName,
                data: reportDate,
                clientes_loja: totalAtendimentos,
                vendas_loja: totalAtendimentos,
                total_vendas_dinheiro: `R$ ${totalVendasValor.toFixed(2).replace('.', ',')}`,
                ticket_medio: `R$ ${ticketMedio.toFixed(2).replace('.', ',')}`,
                pa: pa.toFixed(2).replace('.', ','),
                vendedores: vendedores
            };
        }

        res.json({ success: true, data: extractedData });

    } catch (error) {
        console.error("### ERRO NO PROCESSAMENTO DO PDF ###", error);
        res.status(500).json({ error: error.message || "Erro ao processar o PDF." });
    }
});
// <<<---------------------------------------------------->>>
// <<<          FIM DA API DE PDF                         >>>
// <<<---------------------------------------------------->>>


// APIs DE SESSÃO E USUÁRIOS
app.get('/api/csrf-token', (req, res) => {
    // Garantir que a sessão seja salva antes de retornar o token
    if (!req.session.csrfToken) {
        req.session.csrfToken = generateCsrfToken();
    }
    
    console.log('🔐 Token CSRF gerado:', req.session.csrfToken.substring(0, 10) + '...');
    console.log('🍪 Session ID:', req.sessionID);
    
    req.session.save((err) => {
        if (err) {
            console.error('❌ Erro ao salvar sessão CSRF:', err);
            return res.status(500).json({ error: 'Erro ao gerar token de segurança' });
        }
        console.log('✓ Sessão CSRF salva com sucesso');
        res.json({ csrfToken: req.session.csrfToken });
    });
});

const loginLimiter = rateLimit({
    windowMs: 15 * 60 * 1000,
    max: 50,
    message: { message: 'Muitas tentativas de login. Tente novamente em 15 minutos.' },
    standardHeaders: true,
    legacyHeaders: false,
    skip: () => true,
});

app.post('/api/login', loginLimiter, validateCsrf, async (req, res) => { 
    const { username, password } = req.body; 
    
    if (!username || !password) {
        logEvent('security', username || 'unknown', 'login_failed', 'Tentativa de login sem credenciais', req);
        return res.status(400).json({ message: 'Username e senha são obrigatórios.' });
    }
    
    db.get('SELECT * FROM usuarios WHERE username = ?', [username], async (err, user) => { 
        if (err) {
            logEvent('error', username, 'login_error', `Erro no banco de dados: ${err.message}`, req);
            return res.status(500).json({ message: 'Erro interno do servidor.' }); 
        }
        
        if (!user) {
            logEvent('security', username, 'login_failed', 'Usuário não encontrado', req);
            return res.status(401).json({ message: 'Credenciais inválidas.' }); 
        }
        
        try {
            let passwordMatch = false;
            
            if (user.password_hashed === 1) {
                passwordMatch = await bcrypt.compare(password, user.password);
            } else {
                passwordMatch = (user.password === password);
                
                if (passwordMatch) {
                    const hashedPassword = await bcrypt.hash(password, 10);
                    db.run('UPDATE usuarios SET password = ?, password_hashed = 1 WHERE id = ?', 
                        [hashedPassword, user.id], 
                        (err) => {
                            if (err) console.error('Erro ao migrar senha para hash:', err.message);
                            else console.log(`✓ Senha do usuário ${username} migrada para bcrypt`);
                        }
                    );
                }
            }
            
            if (!passwordMatch) {
                logEvent('security', username, 'login_failed', 'Senha incorreta', req);
                return res.status(401).json({ message: 'Credenciais inválidas.' }); 
            }
            
            req.session.userId = user.id; 
            req.session.username = user.username;
            
            req.session.save((err) => {
                if (err) {
                    console.error('❌ Erro ao salvar sessão:', err);
                    logEvent('error', username, 'login_error', `Erro ao salvar sessão: ${err.message}`, req);
                    return res.status(500).json({ message: 'Erro ao salvar sessão.' });
                }
                console.log(`✓ Login bem-sucedido - Usuário: ${user.username}, Session ID: ${req.sessionID}`);
                logEvent('access', user.username, 'login_success', `Usuário ${user.username} fez login com sucesso`, req);
                res.json({ success: true });
            }); 
        } catch (error) {
            logEvent('error', username, 'login_error', `Erro ao processar login: ${error.message}`, req);
            res.status(500).json({ message: 'Erro ao processar login.' });
        }
    }); 
});
app.get('/logout', (req, res) => { 
    const username = req.session?.username;
    if (username) {
        logEvent('access', username, 'logout', `Usuário ${username} fez logout`, req);
    }
    req.session.destroy(() => res.redirect('/login')); 
});

// =================================================================
// ENDPOINTS DE GERENCIAMENTO DE TOKENS TEMPORÁRIOS
// =================================================================

app.post('/api/dev/generate-temp-token', requirePageLogin, async (req, res) => {
    if (!DEV_TEMP_ACCESS_ENABLED) {
        return res.status(403).json({ 
            error: 'Acesso temporário desabilitado',
            message: 'Configure DEV_TEMP_ACCESS=true em ambiente de desenvolvimento'
        });
    }
    
    try {
        const { expiresInHours = 1, ipRestricted = null } = req.body;
        
        if (expiresInHours < 0.1 || expiresInHours > 24) {
            return res.status(400).json({ 
                error: 'Validade inválida. Use entre 0.1 e 24 horas.' 
            });
        }
        
        const currentIp = getClientIp(req);
        const { token, tokenHash } = generateTempToken(expiresInHours, ipRestricted);
        
        await saveTempTokenToDb(
            tokenHash, 
            expiresInHours, 
            currentIp, 
            ipRestricted, 
            req.session.username
        );
        
        logEvent('security', req.session.username, 'temp_token_generated', 
            `Token temporário gerado (validade: ${expiresInHours}h, IP restrito: ${ipRestricted || 'não'})`, 
            req
        );
        
        res.json({
            success: true,
            token,
            expiresInHours,
            ipRestricted,
            usage: `Authorization: Bearer ${token}`,
            warning: 'Este token tem acesso completo de desenvolvedor. Mantenha-o seguro!'
        });
    } catch (error) {
        console.error('Erro ao gerar token temporário:', error);
        logEvent('error', req.session.username, 'temp_token_error', error.message, req);
        res.status(500).json({ error: 'Erro ao gerar token temporário' });
    }
});

app.delete('/api/dev/revoke-temp-token', requirePageLogin, async (req, res) => {
    if (!DEV_TEMP_ACCESS_ENABLED) {
        return res.status(403).json({ error: 'Acesso temporário desabilitado' });
    }
    
    try {
        const { tokenId } = req.body;
        
        if (!tokenId) {
            return res.status(400).json({ error: 'ID do token não fornecido' });
        }
        
        db.run(
            `UPDATE temp_tokens SET revogado = 1, revogado_em = datetime('now'), revogado_por = ? WHERE id = ?`,
            [req.session.username, tokenId],
            function(err) {
                if (err) {
                    console.error('Erro ao revogar token:', err);
                    return res.status(500).json({ error: 'Erro ao revogar token' });
                }
                
                if (this.changes === 0) {
                    return res.status(404).json({ error: 'Token não encontrado' });
                }
                
                logEvent('security', req.session.username, 'temp_token_revoked', 
                    `Token temporário #${tokenId} revogado manualmente`, 
                    req
                );
                
                res.json({ success: true, message: 'Token revogado com sucesso' });
            }
        );
    } catch (error) {
        console.error('Erro ao revogar token:', error);
        logEvent('error', req.session.username, 'temp_token_revoke_error', error.message, req);
        res.status(500).json({ error: 'Erro ao revogar token' });
    }
});

app.get('/api/dev/temp-tokens', requirePageLogin, (req, res) => {
    if (!DEV_TEMP_ACCESS_ENABLED) {
        return res.status(403).json({ error: 'Acesso temporário desabilitado' });
    }
    
    db.all(
        `SELECT id, role, expira_em, ip_origem, ip_restrito, revogado, criado_por, criado_em, usado_em, revogado_em, revogado_por 
         FROM temp_tokens 
         ORDER BY criado_em DESC 
         LIMIT 100`,
        [],
        (err, tokens) => {
            if (err) {
                console.error('Erro ao listar tokens:', err);
                return res.status(500).json({ error: 'Erro ao listar tokens' });
            }
            
            const tokensWithStatus = tokens.map(t => ({
                ...t,
                status: t.revogado ? 'revogado' : 
                       (new Date(t.expira_em) < new Date() ? 'expirado' : 'ativo')
            }));
            
            res.json(tokensWithStatus);
        }
    );
});

app.get('/api/session-info', requirePageLogin, (req, res) => { 
    const permissions = getPermissions();
    res.json({ 
        id: req.session.userId, 
        username: req.session.username,
        permissions: permissions
    });
});
app.get('/api/usuarios', requirePageLogin, (req, res) => { 
    db.all("SELECT id, username FROM usuarios ORDER BY username", (err, users) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        res.json(users || []); 
    }); 
});
app.post('/api/usuarios', requirePageLogin, validateCsrf, async (req, res) => { 
    const { username, password } = req.body; 
    if (!username || !password) return res.status(400).json({ error: 'Username e senha são obrigatórios.' }); 
    
    try {
        const hashedPassword = await bcrypt.hash(password, 10);
        
        db.run('INSERT INTO usuarios (username, password, password_hashed) VALUES (?, ?, 1)', 
            [username, hashedPassword], 
            function (err) { 
                if (err) {
                    logEvent('error', req.session.username, 'user_creation_failed', `Erro ao criar usuário ${username}: ${err.message}`, req);
                    return res.status(500).json({ error: 'Erro ao criar usuário. O nome de usuário já pode existir.' }); 
                }
                logEvent('admin', req.session.username, 'user_created', `Usuário ${username} criado`, req);
                res.status(201).json({ success: true, id: this.lastID }); 
            }
        );
    } catch (error) {
        logEvent('error', req.session.username, 'user_creation_error', `Erro ao hash senha: ${error.message}`, req);
        res.status(500).json({ error: 'Erro ao processar senha.' });
    }
});
app.put('/api/usuarios/:id', requirePageLogin, validateCsrf, async (req, res) => { 
    const { id } = req.params; 
    const { username, password } = req.body; 
    if (!username) return res.status(400).json({ error: 'Username é obrigatório.' }); 
    
    try {
        let sql, params;
        
        if (password) {
            const hashedPassword = await bcrypt.hash(password, 10);
            sql = 'UPDATE usuarios SET username = ?, password = ?, password_hashed = 1 WHERE id = ?';
            params = [username, hashedPassword, id];
        } else {
            sql = 'UPDATE usuarios SET username = ? WHERE id = ?';
            params = [username, id];
        }
        
        db.run(sql, params, function (err) { 
            if (err) {
                logEvent('error', req.session.username, 'user_update_failed', `Erro ao atualizar usuário ${username}: ${err.message}`, req);
                return res.status(500).json({ error: 'Erro ao atualizar usuário.' }); 
            }
            logEvent('admin', req.session.username, 'user_updated', `Usuário ${username} atualizado`, req);
            res.json({ success: true }); 
        }); 
    } catch (error) {
        logEvent('error', req.session.username, 'user_update_error', `Erro ao hash senha: ${error.message}`, req);
        res.status(500).json({ error: 'Erro ao processar senha.' });
    }
});
app.delete('/api/usuarios/:id', requirePageLogin, validateCsrf, (req, res) => { 
    const { id } = req.params; 
    if (id == req.session.userId) return res.status(403).json({ error: 'Não é permitido excluir o próprio usuário logado.' }); 
    db.run("DELETE FROM usuarios WHERE id = ?", [id], function (err) { 
        if (err) return res.status(500).json({ error: 'Erro ao excluir usuário.' }); 
        if (this.changes === 0) return res.status(404).json({ error: "Usuário não encontrado." }); 
        res.json({ success: true }); 
    }); 
});

// APIs DE LOJAS 
app.get('/api/lojas', requirePageLogin, (req, res) => { 
    let whereClauses = []; 
    const params = []; 
    
    // Aplicar filtro de lojas baseado no role
    
    if (req.query.status) { 
        whereClauses.push("status = ?"); 
        params.push(req.query.status); 
    } 
    
    const whereString = whereClauses.length > 0 ? " WHERE " + whereClauses.join(" AND ") : "";
    const query = "SELECT * FROM lojas" + whereString + " ORDER BY nome"; 
    console.log('SQL executado:', query);
    console.log('Params:', params);
    db.all(query, params, (err, lojas) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        console.log('Lojas retornadas:', lojas.length);
        if (lojas.length === 0 && params.length > 0) {
            // Verificar lojas no banco
            db.all('SELECT nome FROM lojas LIMIT 10', [], (err2, allLojas) => {
                console.log('Primeiras 10 lojas no banco:', allLojas ? allLojas.map(l => l.nome) : []);
            });
        }
        res.json(lojas || []); 
    }); 
});
app.post('/api/lojas', requirePageLogin, (req, res) => { 
    const { nome, status, funcao_especial, tecnico_username, observacoes, cargo, cep, numero_contato, gerente } = req.body; 
    db.run('INSERT INTO lojas (nome, status, funcao_especial, tecnico_username, observacoes, cargo, cep, numero_contato, gerente) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)', 
        [nome, status, funcao_especial, tecnico_username, observacoes, cargo, cep, numero_contato, gerente], 
        function (err) { 
            if (err) return res.status(500).json({ error: 'Erro ao criar loja. O nome já pode existir.' }); 
            res.status(201).json({ success: true, id: this.lastID }); 
        }); 
});
app.put('/api/lojas/:id', requirePageLogin, (req, res) => { 
    const { id } = req.params; 
    const { nome, status, funcao_especial, tecnico_username, observacoes, cargo, cep, numero_contato, gerente } = req.body; 
    db.run('UPDATE lojas SET nome = ?, status = ?, funcao_especial = ?, tecnico_username = ?, observacoes = ?, cargo = ?, cep = ?, numero_contato = ?, gerente = ? WHERE id = ?', 
        [nome, status, funcao_especial, tecnico_username, observacoes, cargo, cep, numero_contato, gerente, id], 
        function (err) { 
            if (err) return res.status(500).json({ error: 'Erro ao atualizar loja.' }); 
            res.json({ success: true }); 
        }); 
});
app.delete('/api/lojas/:id', requirePageLogin, (req, res) => { db.run("DELETE FROM lojas WHERE id = ?", [req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao excluir loja.' }); if (this.changes === 0) return res.status(404).json({ error: "Loja não encontrada." }); res.json({ success: true }); }); });

// APIs DE VENDEDORES
app.get('/api/vendedores', requirePageLogin, (req, res) => {
    const { loja_id } = req.query;
    
    if (!loja_id) {
        return res.status(400).json({ error: 'ID da loja é obrigatório.' });
    }
    
    const query = `
        SELECT v.*, l.nome as loja_nome 
        FROM vendedores v
        INNER JOIN lojas l ON v.loja_id = l.id
        WHERE v.loja_id = ?
        ORDER BY v.ativo DESC, v.nome ASC
    `;
    
    db.all(query, [loja_id], (err, vendedores) => {
        if (err) {
            console.error('Erro ao buscar vendedores:', err);
            return res.status(500).json({ error: 'Erro ao buscar vendedores.' });
        }
        res.json(vendedores || []);
    });
});

app.post('/api/vendedores', requirePageLogin, (req, res) => {
    const { loja_id, nome, telefone, data_entrada, data_demissao, previsao_entrada, previsao_saida } = req.body;
    
    if (!loja_id || !nome || !telefone || !data_entrada) {
        return res.status(400).json({ error: 'Campos obrigatórios: loja_id, nome, telefone, data_entrada' });
    }
    
    const query = `
        INSERT INTO vendedores (loja_id, nome, telefone, data_entrada, data_demissao, previsao_entrada, previsao_saida, ativo)
        VALUES (?, ?, ?, ?, ?, ?, ?, 1)
    `;
    
    db.run(query, [loja_id, nome, telefone, data_entrada, data_demissao || null, previsao_entrada || null, previsao_saida || null], function(err) {
        if (err) {
            console.error('Erro ao adicionar vendedor:', err);
            return res.status(500).json({ error: 'Erro ao adicionar vendedor.' });
        }
        res.status(201).json({ success: true, id: this.lastID });
    });
});

app.put('/api/vendedores/:id', requirePageLogin, (req, res) => {
    const { id } = req.params;
    const { nome, telefone, data_entrada, data_demissao, previsao_entrada, previsao_saida, ativo } = req.body;
    
    if (!nome || !telefone || !data_entrada) {
        return res.status(400).json({ error: 'Campos obrigatórios: nome, telefone, data_entrada' });
    }
    
    const query = `
        UPDATE vendedores 
        SET nome = ?, telefone = ?, data_entrada = ?, data_demissao = ?, 
            previsao_entrada = ?, previsao_saida = ?, ativo = ?
        WHERE id = ?
    `;
    
    db.run(query, [nome, telefone, data_entrada, data_demissao || null, previsao_entrada || null, previsao_saida || null, ativo !== undefined ? ativo : 1, id], function(err) {
        if (err) {
            console.error('Erro ao atualizar vendedor:', err);
            return res.status(500).json({ error: 'Erro ao atualizar vendedor.' });
        }
        if (this.changes === 0) {
            return res.status(404).json({ error: 'Vendedor não encontrado.' });
        }
        res.json({ success: true });
    });
});

app.delete('/api/vendedores/:id', requirePageLogin, (req, res) => {
    const { id } = req.params;
    
    db.run("DELETE FROM vendedores WHERE id = ?", [id], function(err) {
        if (err) {
            console.error('Erro ao excluir vendedor:', err);
            return res.status(500).json({ error: 'Erro ao excluir vendedor.' });
        }
        if (this.changes === 0) {
            return res.status(404).json({ error: 'Vendedor não encontrado.' });
        }
        res.json({ success: true });
    });
});

// API de métricas agregadas para dashboard - suporta filtros independentes
app.get('/api/dashboard/metrics', requirePageLogin, (req, res) => {
    const { loja_monitoramento, loja_bluve } = req.query;
    
    // Query para Monitoramento
    const queryMonit = loja_monitoramento ? `
        SELECT 
            SUM(clientes_monitoramento) as total_clientes_monitoramento,
            SUM(vendas_monitoramento) as total_vendas_monitoramento,
            SUM(quantidade_omni) as total_omni
        FROM relatorios
        WHERE loja = ?
    ` : `
        SELECT 
            SUM(clientes_monitoramento) as total_clientes_monitoramento,
            SUM(vendas_monitoramento) as total_vendas_monitoramento,
            SUM(quantidade_omni) as total_omni
        FROM relatorios
    `;
    const paramsMonit = loja_monitoramento ? [loja_monitoramento] : [];
    
    // Query para Bluve
    const queryBluve = loja_bluve ? `
        SELECT 
            SUM(clientes_loja) as total_clientes_loja,
            SUM(vendas_loja) as total_vendas_loja
        FROM relatorios
        WHERE loja = ?
    ` : `
        SELECT 
            SUM(clientes_loja) as total_clientes_loja,
            SUM(vendas_loja) as total_vendas_loja
        FROM relatorios
    `;
    const paramsBluve = loja_bluve ? [loja_bluve] : [];
    
    // Executar ambas as queries
    db.get(queryMonit, paramsMonit, (err1, rowMonit) => {
        if (err1) {
            console.error('Erro ao buscar métricas de Monitoramento:', err1);
            return res.status(500).json({ error: err1.message });
        }
        
        db.get(queryBluve, paramsBluve, (err2, rowBluve) => {
            if (err2) {
                console.error('Erro ao buscar métricas de Bluve:', err2);
                return res.status(500).json({ error: err2.message });
            }
            
            // Calcular métricas de Monitoramento
            const clientesMonitoramento = parseInt(rowMonit.total_clientes_monitoramento) || 0;
            const vendasMonitoramento = parseInt(rowMonit.total_vendas_monitoramento) || 0;
            const omni = parseInt(rowMonit.total_omni) || 0;
            const vendasMonitoramentoTotal = vendasMonitoramento + omni;
            const txConversaoMonitoramento = clientesMonitoramento > 0 
                ? ((vendasMonitoramentoTotal / clientesMonitoramento) * 100).toFixed(2)
                : '0.00';
            
            // Calcular métricas de Bluve
            const clientesLoja = parseInt(rowBluve.total_clientes_loja) || 0;
            const vendasLoja = parseInt(rowBluve.total_vendas_loja) || 0;
            const txConversaoLoja = clientesLoja > 0 
                ? ((vendasLoja / clientesLoja) * 100).toFixed(2)
                : '0.00';
            
            res.json({
                monitoramento: {
                    clientes: clientesMonitoramento,
                    vendas: vendasMonitoramentoTotal,
                    tx_conversao: txConversaoMonitoramento
                },
                bluve: {
                    clientes: clientesLoja,
                    vendas: vendasLoja,
                    tx_conversao: txConversaoLoja
                }
            });
        });
    });
});

// APIs DE RELATÓRIOS
const processarRelatorio = (r) => { if (!r) return null; const vendas_monitoramento_total = (parseInt(r.vendas_monitoramento, 10) || 0) + (parseInt(r.quantidade_omni, 10) || 0); const tx_conversao_monitoramento = (parseInt(r.clientes_monitoramento, 10) || 0) > 0 ? (vendas_monitoramento_total / r.clientes_monitoramento) * 100 : 0; const tx_conversao_loja = (parseInt(r.clientes_loja, 10) || 0) > 0 ? ((parseInt(r.vendas_loja, 10) || 0) / r.clientes_loja) * 100 : 0; let vendedores_processados = []; try { const vendedores = JSON.parse(r.vendedores || '[]'); vendedores_processados = vendedores.map(v => ({ ...v, tx_conversao: (v.atendimentos > 0 ? ((v.vendas / v.atendimentos) * 100) : 0).toFixed(2) })); } catch (e) {} return { ...r, vendas_monitoramento_total, tx_conversao_monitoramento: tx_conversao_monitoramento.toFixed(2), tx_conversao_loja: tx_conversao_loja.toFixed(2), vendedores_processados }; };
app.get('/api/relatorios', requirePageLogin, (req, res) => { 
    const whereClauses = []; 
    const params = []; 
    
    // Aplicar filtro de lojas baseado no role
    
    if (req.query.loja) { 
        whereClauses.push("loja = ?"); 
        params.push(req.query.loja); 
    } 
    if (req.query.data_inicio) { 
        whereClauses.push("data >= ?"); 
        params.push(req.query.data_inicio); 
    } 
    if (req.query.data_fim) { 
        whereClauses.push("data <= ?"); 
        params.push(req.query.data_fim); 
    } 
    const whereString = whereClauses.length > 0 ? " WHERE " + whereClauses.join(" AND ") : ""; 
    const sortOrder = req.query.sortOrder === 'asc' ? 'ASC' : 'DESC'; 
    db.get(`SELECT COUNT(*) as total FROM relatorios` + whereString, params, (err, row) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        const total = row ? row.total : 0; 
        const limit = parseInt(req.query.limit) || 20; 
        const offset = parseInt(req.query.offset) || 0; 
        const query = `SELECT id, loja, data, total_vendas_dinheiro FROM relatorios` + whereString + ` ORDER BY id ${sortOrder} LIMIT ? OFFSET ?`; 
        db.all(query, [...params, limit, offset], (err, relatorios) => { 
            if (err) return res.status(500).json({ error: err.message }); 
            res.json({ relatorios: relatorios || [], total }); 
        }); 
    }); 
});
app.post('/api/relatorios', requirePageLogin, validateCsrf, (req, res) => { const d = req.body; const sql = `INSERT INTO relatorios (loja, data, hora_abertura, hora_fechamento, gerente_entrada, gerente_saida, clientes_monitoramento, vendas_monitoramento, clientes_loja, vendas_loja, total_vendas_dinheiro, ticket_medio, pa, quantidade_trocas, quantidade_omni, quantidade_funcao_especial, vendedores, enviado_por_usuario, vendas_cartao, vendas_pix, vendas_dinheiro) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)`; const params = [ d.loja, d.data, d.hora_abertura, d.hora_fechamento, d.gerente_entrada, d.gerente_saida, parseInt(d.clientes_monitoramento, 10) || 0, parseInt(d.vendas_monitoramento, 10) || 0, parseInt(d.clientes_loja, 10) || 0, parseInt(d.vendas_loja, 10) || 0, parseFloat(String(d.total_vendas_dinheiro).replace(/[R$\s.]/g, '').replace(',', '.')) || 0, d.ticket_medio || 'R$ 0,00', d.pa || '0.00', parseInt(d.quantidade_trocas, 10) || 0, parseInt(d.quantidade_omni, 10) || 0, parseInt(d.quantidade_funcao_especial, 10) || 0, d.vendedores || '[]', req.session.username, parseInt(d.vendas_cartao, 10) || 0, parseInt(d.vendas_pix, 10) || 0, parseInt(d.vendas_dinheiro, 10) || 0 ]; db.run(sql, params, function (err) { if (err) { console.error("Erro ao inserir relatório:", err.message); return res.status(500).json({ error: 'Falha ao salvar relatório.' }); } res.status(201).json({ success: true, id: this.lastID }); }); });
app.get('/api/relatorios/:id', requirePageLogin, (req, res) => { db.get("SELECT * FROM relatorios WHERE id = ?", [req.params.id], (err, relatorio) => { if (err) return res.status(500).json({ error: err.message }); if (!relatorio) return res.status(404).json({ error: "Relatório não encontrado" }); res.json({ relatorio }); }); });
app.put('/api/relatorios/:id', requirePageLogin, validateCsrf, (req, res) => { const { id } = req.params; const d = req.body; const sql = `UPDATE relatorios SET loja=?, data=?, hora_abertura=?, hora_fechamento=?, gerente_entrada=?, gerente_saida=?, clientes_monitoramento=?, vendas_monitoramento=?, clientes_loja=?, vendas_loja=?, total_vendas_dinheiro=?, ticket_medio=?, pa=?, quantidade_trocas=?, quantidade_omni=?, quantidade_funcao_especial=?, vendedores=?, vendas_cartao=?, vendas_pix=?, vendas_dinheiro=? WHERE id=?`; const params = [ d.loja, d.data, d.hora_abertura, d.hora_fechamento, d.gerente_entrada, d.gerente_saida, parseInt(d.clientes_monitoramento, 10) || 0, parseInt(d.vendas_monitoramento, 10) || 0, parseInt(d.clientes_loja, 10) || 0, parseInt(d.vendas_loja, 10) || 0, parseFloat(String(d.total_vendas_dinheiro).replace(/[R$\s.]/g, '').replace(',', '.')) || 0, d.ticket_medio || 'R$ 0,00', d.pa || '0.00', parseInt(d.quantidade_trocas, 10) || 0, parseInt(d.quantidade_omni, 10) || 0, parseInt(d.quantidade_funcao_especial, 10) || 0, d.vendedores || '[]', parseInt(d.vendas_cartao, 10) || 0, parseInt(d.vendas_pix, 10) || 0, parseInt(d.vendas_dinheiro, 10) || 0, id ]; db.run(sql, params, function (err) { if (err) { console.error("Erro ao atualizar relatório:", err.message); return res.status(500).json({ error: 'Falha ao atualizar o relatório.' }); } if (this.changes === 0) return res.status(404).json({ error: "Relatório não encontrado." }); res.json({ success: true, id: id }); }); });
app.delete('/api/relatorios/:id', requirePageLogin, validateCsrf, (req, res) => { db.run("DELETE FROM relatorios WHERE id = ?", [req.params.id], function (err) { if (err) return res.status(500).json({ error: err.message }); if (this.changes === 0) return res.status(404).json({ error: "Relatório não encontrado" }); res.json({ success: true, message: "Relatório excluído." }); }); });

// =================================================================
// ENDPOINTS GOOGLE DRIVE - Armazenamento em Nuvem Gratuito
// =================================================================

// Verificar quota do Google Drive
app.get('/api/drive/quota', requirePageLogin, async (req, res) => {
    try {
        const quota = await googleDriveService.verificarQuota();
        if (!quota) {
            return res.status(503).json({ error: 'Google Drive não configurado' });
        }
        res.json(quota);
    } catch (error) {
        console.error('Erro ao verificar quota:', error.message);
        res.status(500).json({ error: error.message });
    }
});

// Salvar relatório no Google Drive
app.post('/api/drive/relatorios', requirePageLogin, validateCsrf, async (req, res) => {
    try {
        const relatorio = {
            ...req.body,
            enviado_por_usuario: req.session.username,
            data_criacao: new Date().toISOString()
        };
        
        const resultado = await googleDriveService.salvarRelatorio(relatorio);
        
        // Verificar se precisa de backup automático
        const quota = await googleDriveService.verificarQuota();
        if (quota && quota.precisaBackup) {
            const emailBackup = process.env.EMAIL_BACKUP;
            if (emailBackup) {
                console.log('⚠️  Limite do Drive atingido! Iniciando backup automático...');
                googleDriveService.fazerBackup(emailBackup).catch(err => {
                    console.error('Erro no backup automático:', err.message);
                });
            }
        }
        
        res.status(201).json({ 
            success: true, 
            arquivo: resultado,
            quota: quota 
        });
    } catch (error) {
        console.error('Erro ao salvar relatório no Drive:', error.message);
        res.status(500).json({ error: error.message });
    }
});

// Listar relatórios do Google Drive
app.get('/api/drive/relatorios', requirePageLogin, async (req, res) => {
    try {
        const filtros = {};
        if (req.query.ano) {
            filtros.ano = req.query.ano;
        }
        
        const relatorios = await googleDriveService.listarRelatorios(filtros);
        res.json({ relatorios, total: relatorios.length });
    } catch (error) {
        console.error('Erro ao listar relatórios do Drive:', error.message);
        res.status(500).json({ error: error.message });
    }
});

// Fazer backup manual
app.post('/api/drive/backup', requirePageLogin, validateCsrf, async (req, res) => {
    try {
        const emailDestino = req.body.email || process.env.EMAIL_BACKUP;
        
        if (!emailDestino) {
            return res.status(400).json({ 
                error: 'Email de destino não fornecido. Configure EMAIL_BACKUP no .env ou envie no corpo da requisição.' 
            });
        }
        
        const resultado = await googleDriveService.fazerBackup(emailDestino);
        res.json({ 
            success: true, 
            ...resultado,
            email: emailDestino,
            mensagem: `Backup enviado para ${emailDestino}` 
        });
    } catch (error) {
        console.error('Erro ao fazer backup:', error.message);
        res.status(500).json({ error: error.message });
    }
});

// Limpar relatórios antigos
app.post('/api/drive/limpar', requirePageLogin, validateCsrf, async (req, res) => {
    try {
        const diasManter = parseInt(req.body.dias) || 90;
        const removidos = await googleDriveService.limparRelatoriosAntigos(diasManter);
        res.json({ 
            success: true, 
            removidos,
            mensagem: `${removidos} relatórios antigos removidos` 
        });
    } catch (error) {
        console.error('Erro ao limpar relatórios:', error.message);
        res.status(500).json({ error: error.message });
    }
});

// Obter URL de autorização (para primeira configuração)
app.get('/api/drive/auth-url', requirePageLogin, (req, res) => {
    try {
        const url = googleDriveService.gerarURLAutorizacao();
        res.json({ authUrl: url });
    } catch (error) {
        console.error('Erro ao gerar URL de autorização:', error.message);
        res.status(500).json({ error: error.message });
    }
});

const formatCurrency = (value) => { const numberValue = Number(value) || 0; return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(numberValue); };

// Função para desenhar um gráfico de rosquinha (donut chart)
function desenharGraficoRosquinha(doc, centerX, centerY, radius, dados, cores) {
    const total = dados.reduce((sum, item) => sum + item.valor, 0);
    if (total === 0) return;
    
    let startAngle = -Math.PI / 2;
    const innerRadius = radius * 0.6;
    
    dados.forEach(item => {
        const sliceAngle = (item.valor / total) * 2 * Math.PI;
        const endAngle = startAngle + sliceAngle;
        
        doc.save();
        doc.path('')
           .moveTo(centerX, centerY)
           .arc(centerX, centerY, radius, startAngle, endAngle, false)
           .lineTo(centerX, centerY)
           .fill(item.cor);
        
        doc.path('')
           .moveTo(centerX, centerY)
           .arc(centerX, centerY, innerRadius, startAngle, endAngle, false)
           .lineTo(centerX, centerY)
           .fill('#ffffff');
        doc.restore();
        
        startAngle = endAngle;
    });
    
    doc.fontSize(10).font('Helvetica-Bold').fillColor('#1f2937')
       .text(`${total}`, centerX - 20, centerY - 5, { width: 40, align: 'center' });
}

// Função para gerar PDF do relatório com cores do sistema (laranja e cinza)
function gerarRelatorioPDFProfissional(doc, r) {
    const rp = processarRelatorio(r);
    if (!rp) {
        doc.fontSize(14).text('Erro ao processar relatório.');
        return;
    }
    
    const cores = {
        laranja: '#ff9966',
        cinza: '#6b7280',
        cinzaClaro: '#f3f4f6',
        cinzaEscuro: '#4b5563',
        texto: '#1f2937',
        branco: '#ffffff'
    };
    
    const pageWidth = doc.page.width;
    const pageHeight = doc.page.height;
    const margin = 25;
    const maxY = pageHeight - 25;
    let y = 25;
    
    // === CABEÇALHO COM COR LARANJA ===
    doc.rect(0, 0, pageWidth, 85).fill(cores.laranja);
    doc.fontSize(24).font('Helvetica-Bold').fillColor(cores.branco)
       .text(r.loja.toUpperCase(), margin, 22, { align: 'center', width: pageWidth - margin * 2 });
    const dataFormatada = new Date(rp.data).toLocaleDateString('pt-BR', { timeZone: 'UTC' });
    doc.fontSize(13).font('Helvetica').fillColor(cores.branco)
       .text(`Relatório de ${dataFormatada}`, margin, 56, { align: 'center', width: pageWidth - margin * 2 });
    
    y = 100;
    doc.fillColor(cores.texto);
    
    // === MÉTRICAS PRINCIPAIS (2 colunas maiores) + GRÁFICO ===
    const colWidth = (pageWidth - margin * 2 - 12) / 3;
    const metricHeight = 70;
    
    // TX. CONV. MONIT.
    doc.roundedRect(margin, y, colWidth, metricHeight, 4).fillAndStroke(cores.cinzaClaro, cores.cinza);
    doc.fontSize(11).font('Helvetica').fillColor(cores.cinza).text('TX. CONV. MONIT.', margin + 8, y + 10, { width: colWidth - 16 });
    doc.fontSize(22).font('Helvetica-Bold').fillColor(cores.cinza).text(`${rp.tx_conversao_monitoramento}%`, margin + 8, y + 28);
    doc.fontSize(9).font('Helvetica').fillColor(cores.cinzaEscuro).text(`${rp.clientes_monitoramento || 0} cli | ${rp.vendas_monitoramento_total || 0} vnd`, margin + 8, y + 55);
    
    // TX. CONV. LOJA
    doc.roundedRect(margin + colWidth + 6, y, colWidth, metricHeight, 4).fillAndStroke(cores.cinzaClaro, cores.laranja);
    doc.fontSize(11).font('Helvetica').fillColor(cores.cinza).text('TX. CONV. LOJA', margin + colWidth + 14, y + 10, { width: colWidth - 16 });
    doc.fontSize(22).font('Helvetica-Bold').fillColor(cores.laranja).text(`${rp.tx_conversao_loja}%`, margin + colWidth + 14, y + 28);
    doc.fontSize(9).font('Helvetica').fillColor(cores.cinzaEscuro).text(`${rp.clientes_loja || 0} cli | ${rp.vendas_loja || 0} vnd`, margin + colWidth + 14, y + 55);
    
    // GRÁFICO DE ROSQUINHA (Formas de Pagamento)
    const graficoX = margin + colWidth * 2 + 12;
    const graficoCenterX = graficoX + colWidth / 2;
    const graficoCenterY = y + 28;
    
    const dadosGrafico = [
        { valor: rp.vendas_cartao || 0, cor: cores.laranja, label: 'Cartão' },
        { valor: rp.vendas_pix || 0, cor: '#60a5fa', label: 'Pix' },
        { valor: rp.vendas_dinheiro || 0, cor: '#a78bfa', label: 'Dinheiro' }
    ];
    
    // Título do gráfico
    const totalVendasQtd = (rp.vendas_cartao || 0) + (rp.vendas_pix || 0) + (rp.vendas_dinheiro || 0);
    doc.fontSize(9).font('Helvetica-Bold').fillColor(cores.cinza).text('PAGAMENTOS', graficoX, y + 4);
    
    // Desenhar gráfico maior e mais visível
    desenharGraficoRosquinha(doc, graficoCenterX, graficoCenterY, 24, dadosGrafico, cores);
    
    // Legenda compacta abaixo do gráfico
    let legendaY = y + 52;
    dadosGrafico.forEach((item, idx) => {
        doc.circle(graficoX + 8, legendaY + 3, 3).fill(item.cor);
        doc.fontSize(7).font('Helvetica').fillColor(cores.texto)
           .text(`${item.label}: ${item.valor}`, graficoX + 16, legendaY);
        legendaY += 10;
    });
    
    y += metricHeight + 12;
    
    // === TOTAL VENDAS (card maior e destacado) ===
    doc.roundedRect(margin, y, (pageWidth - margin * 2) / 2 - 6, 65, 4).fillAndStroke(cores.cinzaClaro, cores.laranja);
    doc.fontSize(11).font('Helvetica').fillColor(cores.cinza).text('TOTAL VENDAS', margin + 10, y + 10);
    doc.fontSize(20).font('Helvetica-Bold').fillColor(cores.laranja).text(formatCurrency(rp.total_vendas_dinheiro), margin + 10, y + 30);
    doc.fontSize(9).font('Helvetica').fillColor(cores.cinzaEscuro).text(`TM: ${rp.ticket_medio} | PA: ${rp.pa}`, margin + 10, y + 53);
    
    // === INFORMAÇÕES OPERACIONAIS ===
    const infoX = margin + (pageWidth - margin * 2) / 2 + 6;
    doc.fontSize(11).font('Helvetica-Bold').fillColor(cores.cinza).text('OPERACIONAL', infoX, y + 10);
    y += 26;
    
    doc.fontSize(9).font('Helvetica').fillColor(cores.texto);
    doc.text(`Abertura: ${rp.hora_abertura || '--:--'} - ${rp.hora_fechamento || '--:--'}`, infoX, y);
    y += 13;
    doc.text(`Gerente: ${rp.gerente_entrada || '--:--'} - ${rp.gerente_saida || '--:--'}`, infoX, y);
    y += 13;
    doc.text(`Trocas: ${rp.quantidade_trocas || 0} | Total: ${totalVendasQtd}`, infoX, y);
    if (rp.funcao_especial === "Omni") {
        y += 13;
        doc.text(`Omni: ${rp.quantidade_omni || 0}`, infoX, y);
    } else if (rp.funcao_especial === "Busca por Assist. Tec.") {
        y += 13;
        doc.text(`Assist. Tec.: ${rp.quantidade_funcao_especial || 0}`, infoX, y);
    }
    
    y = 100 + metricHeight + 14 + 65 + 18;
    
    // === DESEMPENHO DA EQUIPE ===
    doc.fontSize(13).font('Helvetica-Bold').fillColor(cores.laranja).text('DESEMPENHO DA EQUIPE', margin, y);
    y += 20;
    
    if (rp.vendedores_processados && rp.vendedores_processados.length > 0) {
        const colX = [margin, margin + 220, margin + 330, margin + 420, margin + 500];
        const headerHeight = 24;
        const rowHeight = 20;
        
        // Cabeçalho
        doc.roundedRect(margin, y, pageWidth - margin * 2, headerHeight, 3).fill(cores.laranja);
        doc.fontSize(11).font('Helvetica-Bold').fillColor(cores.branco);
        doc.text('VENDEDOR', colX[0] + 6, y + 8, { width: 210 });
        doc.text('ATEND.', colX[1] + 6, y + 8);
        doc.text('VENDAS', colX[2] + 6, y + 8);
        doc.text('TX. CONV.', colX[3] + 6, y + 8);
        y += headerHeight;
        
        // Calcular quantos vendedores cabem
        const spaceLeft = maxY - y - 18;
        const maxRows = Math.floor(spaceLeft / rowHeight);
        const numVendedores = Math.min(rp.vendedores_processados.length, maxRows);
        
        // Renderizar vendedores
        for (let i = 0; i < numVendedores; i++) {
            const v = rp.vendedores_processados[i];
            const bgColor = i % 2 === 0 ? cores.branco : cores.cinzaClaro;
            doc.rect(margin, y, pageWidth - margin * 2, rowHeight).fill(bgColor);
            doc.fontSize(9).font('Helvetica').fillColor(cores.texto);
            doc.text(v.nome || 'N/A', colX[0] + 6, y + 6, { width: 210 });
            doc.text(String(v.atendimentos || 0), colX[1] + 6, y + 6);
            doc.text(String(v.vendas || 0), colX[2] + 6, y + 6);
            doc.fontSize(9).font('Helvetica-Bold').fillColor(parseFloat(v.tx_conversao) >= 50 ? '#10b981' : cores.laranja);
            doc.text(`${v.tx_conversao}%`, colX[3] + 6, y + 6);
            y += rowHeight;
        }
        
        if (rp.vendedores_processados.length > numVendedores) {
            doc.fontSize(8).font('Helvetica').fillColor(cores.cinza)
               .text(`... e mais ${rp.vendedores_processados.length - numVendedores} vendedores`, margin, y + 5);
        }
    } else {
        doc.fontSize(10).font('Helvetica').fillColor(cores.cinza)
           .text('Nenhum vendedor registrado.', margin, y);
    }
    
    // === RODAPÉ ===
    doc.fontSize(9).font('Helvetica').fillColor(cores.cinza)
       .text(`Gerado em ${new Date().toLocaleDateString('pt-BR')}`, margin, maxY, { align: 'left' });
}

const formatarRelatorioTexto = (r) => { const rp = processarRelatorio(r); if (!rp) return "Erro ao processar relatório."; let equipeInfo = 'Nenhum vendedor registrado.\n'; if (rp.vendedores_processados && rp.vendedores_processados.length > 0) { equipeInfo = rp.vendedores_processados.map(v => { return `${v.nome}: ${v.atendimentos} Atendimentos / ${v.vendas} Vendas / ${v.tx_conversao}%`; }).join('\n'); } let funcaoEspecialInfo = ''; if (rp.funcao_especial === "Omni") { funcaoEspecialInfo = `Omni: ${rp.quantidade_omni || 0}\n`; } else if (rp.funcao_especial === "Busca por Assist. Tec.") { funcaoEspecialInfo = `Busca por assist tec: ${rp.quantidade_funcao_especial || 0}\n`; } const totalVendasQuantidade = (rp.vendas_cartao || 0) + (rp.vendas_pix || 0) + (rp.vendas_dinheiro || 0); const content = ` DATA: ${new Date(rp.data).toLocaleDateString('pt-BR', { timeZone: 'UTC' })} \n\nClientes: ${rp.clientes_monitoramento || 0}\nBluve: ${rp.clientes_loja || 0}\nVendas / Monitoramento: ${rp.vendas_monitoramento_total || 0}\nVendas / Loja: ${rp.vendas_loja || 0}\nTaxa de conversão da loja: ${rp.tx_conversao_loja || '0.00'}%\nTaxa de conversão do monitoramento: ${rp.tx_conversao_monitoramento || '0.00'}%\n\nAbertura: ${rp.hora_abertura || '--:--'} - ${rp.hora_fechamento || '--:--'}\nGerente: ${rp.gerente_entrada || '--:--'} - ${rp.gerente_saida || '--:--'}\nVendas em Cartão: ${rp.vendas_cartao || 0}\nVendas em Pix: ${rp.vendas_pix || 0}\nVendas em Dinheiro: ${rp.vendas_dinheiro || 0}\n${funcaoEspecialInfo}Total vendas: ${totalVendasQuantidade}\nTroca/Devolução: ${rp.quantidade_trocas || 0}\n\nDesempenho Equipe:\n\n${equipeInfo}\n\nTM: ${rp.ticket_medio || 'R$ 0,00'} / P.A: ${rp.pa || '0.00'} / Total: ${formatCurrency(rp.total_vendas_dinheiro)} / `; return content.trim(); };
app.get('/api/relatorios/:id/txt', requirePageLogin, (req, res) => { const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = ? `; db.get(sql, [req.params.id], (err, r) => { if (err || !r) return res.status(404).send('Relatório não encontrado'); res.setHeader('Content-disposition', `attachment; filename=relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.txt`); res.setHeader('Content-type', 'text/plain; charset=utf-8'); res.send(formatarRelatorioTexto(r)); }); });
app.get('/api/relatorios/:id/pdf', requirePageLogin, (req, res) => { 
    const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = ? `; 
    db.get(sql, [req.params.id], (err, r) => { 
        if (err || !r) return res.status(404).send('Relatório não encontrado'); 
        const doc = new PDFDocument({ margin: 50, size: 'A4' }); 
        res.setHeader('Content-disposition', `inline; filename="relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.pdf"`); 
        res.setHeader('Content-type', 'application/pdf'); 
        doc.pipe(res); 
        gerarRelatorioPDFProfissional(doc, r);
        doc.end(); 
    }); 
});

// =================================================================
// ROTAS DE PROCESSAMENTO DE PDF
// =================================================================

// Configuração do multer para upload de PDFs
const pdfStorage = multer.memoryStorage();
const pdfUpload = multer({
    storage: pdfStorage,
    limits: { fileSize: 5 * 1024 * 1024 }, // 5MB limit
    fileFilter: (req, file, cb) => {
        if (file.mimetype === 'application/pdf') {
            cb(null, true);
        } else {
            cb(new Error('Apenas arquivos PDF são permitidos'));
        }
    }
});

// Função auxiliar para extrair dados do PDF de Ranking
function extractRankingData(pdfText) {
    const lines = pdfText.split('\n').map(l => l.trim()).filter(l => l);
    
    // Extrair nome da loja (formato: "103 - LOFT ITAGUAÇU STORE")
    let lojaMatch = null;
    for (const line of lines) {
        const match = line.match(/^\d+\s*-\s*(.+?)(?:\s+Emissão:|\s+Ranking)/);
        if (match) {
            lojaMatch = match[1].trim();
            break;
        }
    }
    
    // Extrair período/data (formato: "Período de 04/11/2025 a 04/11/2025")
    let dataMatch = null;
    for (const line of lines) {
        const match = line.match(/Período de (\d{2}\/\d{2}\/\d{4})/);
        if (match) {
            const [dia, mes, ano] = match[1].split('/');
            dataMatch = `${ano}-${mes}-${dia}`; // Formato ISO
            break;
        }
    }
    
    // Procurar linha de "Totais:" e extrair valores
    // No PDF real, a linha é: "     Totais:\n1.109,00       0,00         0,00 %       3,33 %      1.109,00      10         4          2,50         110,90   277,25"
    let pa = null, precoMedio = null, atendimentoMedio = null;
    
    for (let i = 0; i < lines.length; i++) {
        // Procurar linha que contém "Totais:"
        if (lines[i].includes('Totais:')) {
            // Verificar se os valores estão na mesma linha ou na linha seguinte
            let dataLine = lines[i];
            
            // Se a linha só tem "Totais:", pegar a próxima linha
            if (dataLine.trim() === 'Totais:') {
                dataLine = lines[i + 1] || '';
            }
            
            // Extrair todos os números no formato brasileiro
            // Formato: \d{1,3}(?:\.\d{3})*(?:,\d+)? captura números como 1.109,00 ou 2,50 ou 10
            // Ex: "1.109,00 0,00 0,00 % 3,33 % 1.109,00 10 4 2,50 110,90 277,25"
            const numberPattern = /\d{1,3}(?:\.\d{3})*(?:,\d+)?/g;
            const numbers = dataLine.match(numberPattern);
            
            if (numbers && numbers.length >= 3) {
                // Os últimos 3 valores são: PA, Preço Médio, Atendimento Médio
                // Converter formato brasileiro (1.000,50) para formato padrão (1000.50)
                const convertBrazilianNumber = (num) => {
                    return num.replace(/\./g, '').replace(',', '.');
                };
                
                pa = convertBrazilianNumber(numbers[numbers.length - 3]);
                precoMedio = convertBrazilianNumber(numbers[numbers.length - 2]);
                atendimentoMedio = convertBrazilianNumber(numbers[numbers.length - 1]);
            }
            break;
        }
    }
    
    return {
        loja: lojaMatch,
        data: dataMatch,
        pa: pa,
        preco_medio: precoMedio,
        atendimento_medio: atendimentoMedio
    };
}

// POST /api/pdf/ranking - Processa e salva PDF de ranking
app.post('/api/pdf/ranking', requirePageLogin, pdfUpload.single('pdf'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({ error: 'Nenhum arquivo PDF foi enviado' });
        }
        
        const { loja, data } = req.body;
        
        if (!loja || !data) {
            return res.status(400).json({ error: 'Loja e data são obrigatórios' });
        }
        
        // Extrair texto do PDF
        const pdfData = await pdf(req.file.buffer);
        const pdfText = pdfData.text;
        
        // Extrair dados do PDF
        const extractedData = extractRankingData(pdfText);
        
        // Validar se a loja corresponde
        if (extractedData.loja && !loja.includes(extractedData.loja) && !extractedData.loja.includes(loja)) {
            return res.status(400).json({
                error: 'O PDF selecionado não corresponde à loja atual.',
                pdfLoja: extractedData.loja,
                lojaAtual: loja
            });
        }
        
        // Validar se a data corresponde
        if (extractedData.data && extractedData.data !== data) {
            return res.status(400).json({
                error: 'O PDF selecionado não corresponde à data atual.',
                pdfData: extractedData.data,
                dataAtual: data
            });
        }
        
        // Verificar se conseguiu extrair os valores
        if (!extractedData.pa || !extractedData.preco_medio || !extractedData.atendimento_medio) {
            return res.status(400).json({
                error: 'Não foi possível extrair os dados do PDF. Verifique se o arquivo está no formato correto.',
                extracted: extractedData
            });
        }
        
        // Criar diretório se não existir
        const rankingsDir = path.join(__dirname, 'data', 'pdfs', 'rankings');
        if (!fs.existsSync(rankingsDir)) {
            fs.mkdirSync(rankingsDir, { recursive: true });
        }
        
        // Gerar nome do arquivo
        const timestamp = Date.now();
        const lojaSafe = loja.replace(/[^a-zA-Z0-9]/g, '_');
        const filename = `ranking_${lojaSafe}_${data}_${timestamp}.pdf`;
        const filepath = path.join(rankingsDir, filename);
        
        // Salvar arquivo
        fs.writeFileSync(filepath, req.file.buffer);
        
        // Registrar no banco de dados
        const sql = `INSERT INTO pdf_rankings (loja, data, filename, filepath, pa, preco_medio, atendimento_medio, uploaded_by, uploaded_at) 
                     VALUES (?, ?, ?, ?, ?, ?, ?, ?, datetime('now'))`;
        
        db.run(sql, [loja, data, filename, filepath, extractedData.pa, extractedData.preco_medio, extractedData.atendimento_medio, req.session.username], function(err) {
            if (err) {
                console.error('Erro ao registrar PDF no banco:', err);
                try {
                    fs.unlinkSync(filepath);
                } catch (e) {}
                return res.status(500).json({ error: 'Erro ao salvar registro do PDF' });
            }
            
            res.json({
                success: true,
                message: 'PDF de ranking salvo com sucesso',
                data: {
                    id: this.lastID,
                    filename: filename,
                    loja: loja,
                    data: data,
                    pa: extractedData.pa,
                    preco_medio: extractedData.preco_medio,
                    atendimento_medio: extractedData.atendimento_medio
                }
            });
        });
        
    } catch (error) {
        console.error('Erro ao processar PDF de ranking:', error);
        res.status(500).json({ error: 'Erro ao processar o PDF: ' + error.message });
    }
});

// POST /api/pdf/ticket - Armazena PDF de ticket
app.post('/api/pdf/ticket', requirePageLogin, pdfUpload.single('pdf'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({ error: 'Nenhum arquivo PDF foi enviado' });
        }
        
        const { loja, data } = req.body;
        
        if (!loja || !data) {
            return res.status(400).json({ error: 'Loja e data são obrigatórios' });
        }
        
        // Criar diretório se não existir
        const ticketsDir = path.join(__dirname, 'data', 'pdfs', 'tickets');
        if (!fs.existsSync(ticketsDir)) {
            fs.mkdirSync(ticketsDir, { recursive: true });
        }
        
        // Gerar nome do arquivo
        const timestamp = Date.now();
        const lojaSafe = loja.replace(/[^a-zA-Z0-9]/g, '_');
        const filename = `ticket_${lojaSafe}_${data}_${timestamp}.pdf`;
        const filepath = path.join(ticketsDir, filename);
        
        // Salvar arquivo
        fs.writeFileSync(filepath, req.file.buffer);
        
        // Registrar no banco de dados
        const sql = `INSERT INTO pdf_tickets (loja, data, filename, filepath, uploaded_by, uploaded_at) 
                     VALUES (?, ?, ?, ?, ?, datetime('now'))`;
        
        db.run(sql, [loja, data, filename, filepath, req.session.username], function(err) {
            if (err) {
                console.error('Erro ao registrar PDF no banco:', err);
                // Tentar remover o arquivo salvo
                try {
                    fs.unlinkSync(filepath);
                } catch (e) {}
                return res.status(500).json({ error: 'Erro ao salvar registro do PDF' });
            }
            
            res.json({
                success: true,
                message: 'PDF salvo com sucesso',
                data: {
                    id: this.lastID,
                    filename: filename,
                    loja: loja,
                    data: data
                }
            });
        });
        
    } catch (error) {
        console.error('Erro ao salvar PDF de ticket:', error);
        res.status(500).json({ error: 'Erro ao salvar o PDF: ' + error.message });
    }
});

// GET /api/pdf/tickets - Lista PDFs de ticket salvos
app.get('/api/pdf/tickets', requirePageLogin, (req, res) => {
    const { loja, data } = req.query;
    
    let sql = 'SELECT id, loja, data, filename, uploaded_by, uploaded_at FROM pdf_tickets';
    const params = [];
    const whereClauses = [];
    
    if (loja) {
        whereClauses.push('loja = ?');
        params.push(loja);
    }
    
    if (data) {
        whereClauses.push('data = ?');
        params.push(data);
    }
    
    if (whereClauses.length > 0) {
        sql += ' WHERE ' + whereClauses.join(' AND ');
    }
    
    sql += ' ORDER BY uploaded_at DESC';
    
    db.all(sql, params, (err, rows) => {
        if (err) {
            console.error('Erro ao listar PDFs:', err);
            return res.status(500).json({ error: 'Erro ao listar PDFs' });
        }
        
        res.json({ success: true, tickets: rows || [] });
    });
});

// GET /api/pdf/tickets/:id/download - Download de PDF de ticket
app.get('/api/pdf/tickets/:id/download', requirePageLogin, (req, res) => {
    const sql = 'SELECT * FROM pdf_tickets WHERE id = ?';
    
    db.get(sql, [req.params.id], (err, row) => {
        if (err || !row) {
            return res.status(404).json({ error: 'PDF não encontrado' });
        }
        
        if (!fs.existsSync(row.filepath)) {
            return res.status(404).json({ error: 'Arquivo PDF não encontrado no servidor' });
        }
        
        res.setHeader('Content-Type', 'application/pdf');
        res.setHeader('Content-Disposition', `attachment; filename="${row.filename}"`);
        fs.createReadStream(row.filepath).pipe(res);
    });
});

// GET /api/pdf/rankings - Lista PDFs de ranking salvos
app.get('/api/pdf/rankings', requirePageLogin, (req, res) => {
    const { loja, data } = req.query;
    
    let sql = 'SELECT id, loja, data, filename, pa, preco_medio, atendimento_medio, uploaded_by, uploaded_at FROM pdf_rankings';
    const params = [];
    const whereClauses = [];
    
    if (loja) {
        whereClauses.push('loja = ?');
        params.push(loja);
    }
    
    if (data) {
        whereClauses.push('data = ?');
        params.push(data);
    }
    
    if (whereClauses.length > 0) {
        sql += ' WHERE ' + whereClauses.join(' AND ');
    }
    
    sql += ' ORDER BY uploaded_at DESC';
    
    db.all(sql, params, (err, rows) => {
        if (err) {
            console.error('Erro ao listar PDFs de ranking:', err);
            return res.status(500).json({ error: 'Erro ao listar PDFs de ranking' });
        }
        
        res.json({ success: true, rankings: rows || [] });
    });
});

// GET /api/pdf/rankings/:id/download - Download de PDF de ranking
app.get('/api/pdf/rankings/:id/download', requirePageLogin, (req, res) => {
    const sql = 'SELECT * FROM pdf_rankings WHERE id = ?';
    
    db.get(sql, [req.params.id], (err, row) => {
        if (err || !row) {
            return res.status(404).json({ error: 'PDF não encontrado' });
        }
        
        if (!fs.existsSync(row.filepath)) {
            return res.status(404).json({ error: 'Arquivo PDF não encontrado no servidor' });
        }
        
        res.setHeader('Content-Type', 'application/pdf');
        res.setHeader('Content-Disposition', `attachment; filename="${row.filename}"`);
        fs.createReadStream(row.filepath).pipe(res);
    });
});

// =================================================================
// FIM DAS ROTAS DE PROCESSAMENTO DE PDF
// =================================================================

// =================================================================
// APIS DE DVR/NVR INTELBRAS
// =================================================================

// Configurar multer para upload de arquivos DVR
const dvrStorage = multer.diskStorage({
    destination: (req, file, cb) => {
        const dvrId = req.body.dvr_id || 'temp';
        const dvrDir = path.join(dataDir, 'dvr_files', dvrId.toString());
        if (!fs.existsSync(dvrDir)) {
            fs.mkdirSync(dvrDir, { recursive: true });
        }
        cb(null, dvrDir);
    },
    filename: (req, file, cb) => {
        const timestamp = Date.now();
        const safeName = file.originalname.replace(/[^a-zA-Z0-9.-]/g, '_');
        cb(null, `${timestamp}_${safeName}`);
    }
});

const dvrUpload = multer({
    storage: dvrStorage,
    limits: {
        fileSize: 500 * 1024 * 1024
    },
    fileFilter: (req, file, cb) => {
        const allowedTypes = ['.mp4', '.avi', '.mkv', '.jpg', '.jpeg', '.png', '.pdf', '.xml', '.json', '.txt'];
        const ext = path.extname(file.originalname).toLowerCase();
        if (allowedTypes.includes(ext)) {
            cb(null, true);
        } else {
            cb(new Error('Tipo de arquivo não permitido. Permitidos: vídeos, imagens, PDF, XML, JSON, TXT'));
        }
    }
});

// GET /api/dvr/dispositivos - Listar dispositivos DVR/NVR
app.get('/api/dvr/dispositivos', requirePageLogin, async (req, res) => {
    try {
        const filtros = {
            loja_id: req.query.loja_id,
            loja_nome: req.query.loja_nome,
            status: req.query.status
        };
        
        const dispositivos = await dvrService.listarDispositivos(filtros);
        res.json({ success: true, data: dispositivos });
    } catch (error) {
        console.error('Erro ao listar dispositivos DVR:', error);
        res.status(500).json({ error: 'Erro ao listar dispositivos DVR' });
    }
});

// GET /api/dvr/dispositivos/:id - Obter dispositivo específico
app.get('/api/dvr/dispositivos/:id', requirePageLogin, async (req, res) => {
    try {
        const dispositivo = await dvrService.obterDispositivo(req.params.id);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }
        res.json({ success: true, data: dispositivo });
    } catch (error) {
        console.error('Erro ao obter dispositivo DVR:', error);
        res.status(500).json({ error: 'Erro ao obter dispositivo DVR' });
    }
});

// POST /api/dvr/dispositivos - Criar novo dispositivo DVR/NVR
app.post('/api/dvr/dispositivos', requirePageLogin, [
    body('nome').notEmpty().withMessage('Nome é obrigatório'),
    body('ip_address').notEmpty().withMessage('IP é obrigatório'),
    body('ip_address').matches(/^(\d{1,3}\.){3}\d{1,3}$/).withMessage('IP inválido')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    
    try {
        const resultado = await dvrService.criarDispositivo(req.body);
        logEvent('admin', req.session.username, 'dvr_created', `Dispositivo DVR ${req.body.nome} criado`, req);
        res.status(201).json({ success: true, data: resultado });
    } catch (error) {
        console.error('Erro ao criar dispositivo DVR:', error);
        res.status(500).json({ error: 'Erro ao criar dispositivo DVR' });
    }
});

// PUT /api/dvr/dispositivos/:id - Atualizar dispositivo DVR/NVR
app.put('/api/dvr/dispositivos/:id', requirePageLogin, async (req, res) => {
    try {
        await dvrService.atualizarDispositivo(req.params.id, req.body);
        logEvent('admin', req.session.username, 'dvr_updated', `Dispositivo DVR ${req.params.id} atualizado`, req);
        res.json({ success: true });
    } catch (error) {
        console.error('Erro ao atualizar dispositivo DVR:', error);
        if (error.message === 'Dispositivo não encontrado') {
            return res.status(404).json({ error: error.message });
        }
        res.status(500).json({ error: 'Erro ao atualizar dispositivo DVR' });
    }
});

// DELETE /api/dvr/dispositivos/:id - Excluir dispositivo DVR/NVR
app.delete('/api/dvr/dispositivos/:id', requirePageLogin, async (req, res) => {
    try {
        await dvrService.excluirDispositivo(req.params.id);
        logEvent('admin', req.session.username, 'dvr_deleted', `Dispositivo DVR ${req.params.id} excluído`, req);
        res.json({ success: true });
    } catch (error) {
        console.error('Erro ao excluir dispositivo DVR:', error);
        if (error.message === 'Dispositivo não encontrado') {
            return res.status(404).json({ error: error.message });
        }
        res.status(500).json({ error: 'Erro ao excluir dispositivo DVR' });
    }
});

// PATCH /api/dvr/dispositivos/:id/status - Atualizar status do dispositivo
app.patch('/api/dvr/dispositivos/:id/status', requirePageLogin, async (req, res) => {
    try {
        const { status } = req.body;
        if (!['online', 'offline'].includes(status)) {
            return res.status(400).json({ error: 'Status inválido. Use: online ou offline' });
        }
        await dvrService.atualizarStatus(req.params.id, status);
        res.json({ success: true });
    } catch (error) {
        console.error('Erro ao atualizar status:', error);
        res.status(500).json({ error: 'Erro ao atualizar status' });
    }
});

// GET /api/dvr/logs - Listar logs de DVR
app.get('/api/dvr/logs', requirePageLogin, async (req, res) => {
    try {
        const filtros = {
            dvr_id: req.query.dvr_id,
            loja_nome: req.query.loja_nome,
            tipo_evento: req.query.tipo_evento,
            severidade: req.query.severidade,
            data_inicio: req.query.data_inicio,
            data_fim: req.query.data_fim
        };
        
        const paginacao = {
            limit: parseInt(req.query.limit) || 100,
            offset: parseInt(req.query.offset) || 0
        };
        
        const logs = await dvrService.listarLogs(filtros, paginacao);
        res.json({ success: true, data: logs });
    } catch (error) {
        console.error('Erro ao listar logs DVR:', error);
        res.status(500).json({ error: 'Erro ao listar logs DVR' });
    }
});

// POST /api/dvr/logs - Registrar novo log de DVR
app.post('/api/dvr/logs', requirePageLogin, [
    body('dvr_id').notEmpty().withMessage('ID do DVR é obrigatório'),
    body('tipo_evento').notEmpty().withMessage('Tipo de evento é obrigatório')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    
    try {
        const resultado = await dvrService.registrarLog(req.body);
        res.status(201).json({ success: true, data: resultado });
    } catch (error) {
        console.error('Erro ao registrar log DVR:', error);
        res.status(500).json({ error: 'Erro ao registrar log DVR' });
    }
});

// DELETE /api/dvr/logs/:id - Excluir log
app.delete('/api/dvr/logs/:id', requirePageLogin, async (req, res) => {
    try {
        await dvrService.excluirLog(req.params.id);
        res.json({ success: true });
    } catch (error) {
        console.error('Erro ao excluir log:', error);
        res.status(500).json({ error: 'Erro ao excluir log' });
    }
});

// GET /api/dvr/arquivos - Listar arquivos de DVR
app.get('/api/dvr/arquivos', requirePageLogin, async (req, res) => {
    try {
        const filtros = {
            dvr_id: req.query.dvr_id,
            loja_nome: req.query.loja_nome,
            tipo_arquivo: req.query.tipo_arquivo,
            data_inicio: req.query.data_inicio,
            data_fim: req.query.data_fim
        };
        
        const paginacao = {
            limit: parseInt(req.query.limit) || 50,
            offset: parseInt(req.query.offset) || 0
        };
        
        const arquivos = await dvrService.listarArquivos(filtros, paginacao);
        res.json({ success: true, data: arquivos });
    } catch (error) {
        console.error('Erro ao listar arquivos DVR:', error);
        res.status(500).json({ error: 'Erro ao listar arquivos DVR' });
    }
});

// POST /api/dvr/arquivos - Upload de arquivo DVR
app.post('/api/dvr/arquivos', requirePageLogin, dvrUpload.single('arquivo'), async (req, res) => {
    try {
        if (!req.file) {
            return res.status(400).json({ error: 'Nenhum arquivo foi enviado' });
        }
        
        const dadosArquivo = {
            dvr_id: req.body.dvr_id,
            dvr_nome: req.body.dvr_nome,
            loja_nome: req.body.loja_nome,
            tipo_arquivo: req.body.tipo_arquivo || path.extname(req.file.originalname).substring(1),
            nome_arquivo: req.file.originalname,
            caminho_arquivo: req.file.path,
            tamanho_bytes: req.file.size,
            data_geracao: req.body.data_geracao || new Date().toISOString(),
            canal: req.body.canal,
            inicio_gravacao: req.body.inicio_gravacao,
            fim_gravacao: req.body.fim_gravacao,
            descricao: req.body.descricao,
            uploaded_by: req.session.username
        };
        
        const resultado = await dvrService.registrarArquivo(dadosArquivo);
        logEvent('admin', req.session.username, 'dvr_file_uploaded', `Arquivo ${req.file.originalname} enviado para DVR ${req.body.dvr_id}`, req);
        res.status(201).json({ 
            success: true, 
            data: resultado,
            file: {
                id: resultado.id,
                nome: req.file.originalname,
                tamanho: req.file.size
            }
        });
    } catch (error) {
        console.error('Erro ao fazer upload de arquivo DVR:', error);
        if (req.file && fs.existsSync(req.file.path)) {
            fs.unlinkSync(req.file.path);
        }
        res.status(500).json({ error: 'Erro ao fazer upload de arquivo DVR' });
    }
});

// GET /api/dvr/arquivos/:id/download - Download de arquivo DVR
app.get('/api/dvr/arquivos/:id/download', requirePageLogin, async (req, res) => {
    try {
        const arquivo = await dvrService.obterArquivo(req.params.id);
        
        if (!arquivo) {
            return res.status(404).json({ error: 'Arquivo não encontrado' });
        }
        
        if (!fs.existsSync(arquivo.caminho_arquivo)) {
            return res.status(404).json({ error: 'Arquivo físico não encontrado no servidor' });
        }
        
        const ext = path.extname(arquivo.nome_arquivo).toLowerCase();
        let contentType = 'application/octet-stream';
        
        if (['.mp4', '.avi', '.mkv'].includes(ext)) {
            contentType = 'video/' + ext.substring(1);
        } else if (['.jpg', '.jpeg'].includes(ext)) {
            contentType = 'image/jpeg';
        } else if (ext === '.png') {
            contentType = 'image/png';
        } else if (ext === '.pdf') {
            contentType = 'application/pdf';
        } else if (ext === '.xml') {
            contentType = 'application/xml';
        } else if (ext === '.json') {
            contentType = 'application/json';
        }
        
        res.setHeader('Content-Type', contentType);
        res.setHeader('Content-Disposition', `attachment; filename="${arquivo.nome_arquivo}"`);
        res.setHeader('Content-Length', arquivo.tamanho_bytes);
        
        fs.createReadStream(arquivo.caminho_arquivo).pipe(res);
    } catch (error) {
        console.error('Erro ao fazer download de arquivo DVR:', error);
        res.status(500).json({ error: 'Erro ao fazer download de arquivo DVR' });
    }
});

// DELETE /api/dvr/arquivos/:id - Excluir arquivo DVR
app.delete('/api/dvr/arquivos/:id', requirePageLogin, async (req, res) => {
    try {
        const arquivo = await dvrService.obterArquivo(req.params.id);
        
        if (!arquivo) {
            return res.status(404).json({ error: 'Arquivo não encontrado' });
        }
        
        await dvrService.excluirArquivo(req.params.id);
        
        if (fs.existsSync(arquivo.caminho_arquivo)) {
            fs.unlinkSync(arquivo.caminho_arquivo);
        }
        
        logEvent('admin', req.session.username, 'dvr_file_deleted', `Arquivo ${arquivo.nome_arquivo} excluído`, req);
        res.json({ success: true });
    } catch (error) {
        console.error('Erro ao excluir arquivo DVR:', error);
        res.status(500).json({ error: 'Erro ao excluir arquivo DVR' });
    }
});

// =================================================================
// NOVAS APIS INTELBRAS - PTZ, GRAVAÇÕES, RTSP
// =================================================================

// Instância do serviço Intelbras (para funcionalidades avançadas)
let intelbrasDvrService = null;
if (db) {
    intelbrasDvrService = new IntelbrasDvrService(DB_PATH);
}

// POST /api/dvr/ptz/control - Controlar PTZ (movimentação)
app.post('/api/dvr/ptz/control', requirePageLogin, [
    body('dvrId').isInt().withMessage('ID do DVR é obrigatório'),
    body('channel').isInt().withMessage('Canal é obrigatório'),
    body('direction').isIn(['Up', 'Down', 'Left', 'Right', 'LeftUp', 'RightUp', 'LeftDown', 'RightDown']).withMessage('Direção inválida'),
    body('action').optional().isIn(['start', 'stop']).withMessage('Ação inválida'),
    body('speed').optional().isInt({ min: 1, max: 8 }).withMessage('Velocidade deve ser entre 1 e 8')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    try {
        const { dvrId, channel, direction, action, speed, password } = req.body;
        
        const dispositivo = await dvrService.obterDispositivo(dvrId);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }

        const result = await intelbrasDvrService.ptzControl(
            dispositivo.ip_address,
            dispositivo.porta || 80,
            dispositivo.usuario || 'admin',
            password,
            channel,
            direction,
            action || 'start',
            speed || 4
        );

        if (result) {
            res.json({ success: true, message: `PTZ ${action || 'start'} ${direction} executado` });
        } else {
            res.status(500).json({ error: 'Falha ao controlar PTZ' });
        }
    } catch (error) {
        console.error('Erro ao controlar PTZ:', error);
        res.status(500).json({ error: 'Erro ao controlar PTZ' });
    }
});

// POST /api/dvr/ptz/preset/goto - Ir para preset PTZ
app.post('/api/dvr/ptz/preset/goto', requirePageLogin, [
    body('dvrId').isInt().withMessage('ID do DVR é obrigatório'),
    body('channel').isInt().withMessage('Canal é obrigatório'),
    body('presetNumber').isInt({ min: 1, max: 255 }).withMessage('Preset deve ser entre 1 e 255')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    try {
        const { dvrId, channel, presetNumber, password } = req.body;
        
        const dispositivo = await dvrService.obterDispositivo(dvrId);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }

        const result = await intelbrasDvrService.gotoPreset(
            dispositivo.ip_address,
            dispositivo.porta || 80,
            dispositivo.usuario || 'admin',
            password,
            channel,
            presetNumber
        );

        if (result) {
            res.json({ success: true, message: `Movido para preset ${presetNumber}` });
        } else {
            res.status(500).json({ error: 'Falha ao ir para preset' });
        }
    } catch (error) {
        console.error('Erro ao ir para preset PTZ:', error);
        res.status(500).json({ error: 'Erro ao ir para preset PTZ' });
    }
});

// POST /api/dvr/ptz/preset/set - Salvar preset PTZ
app.post('/api/dvr/ptz/preset/set', requirePageLogin, [
    body('dvrId').isInt().withMessage('ID do DVR é obrigatório'),
    body('channel').isInt().withMessage('Canal é obrigatório'),
    body('presetNumber').isInt({ min: 1, max: 255 }).withMessage('Preset deve ser entre 1 e 255')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    try {
        const { dvrId, channel, presetNumber, password } = req.body;
        
        const dispositivo = await dvrService.obterDispositivo(dvrId);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }

        const result = await intelbrasDvrService.setPreset(
            dispositivo.ip_address,
            dispositivo.porta || 80,
            dispositivo.usuario || 'admin',
            password,
            channel,
            presetNumber
        );

        if (result) {
            res.json({ success: true, message: `Preset ${presetNumber} salvo` });
        } else {
            res.status(500).json({ error: 'Falha ao salvar preset' });
        }
    } catch (error) {
        console.error('Erro ao salvar preset PTZ:', error);
        res.status(500).json({ error: 'Erro ao salvar preset PTZ' });
    }
});

// POST /api/dvr/snapshot - Capturar snapshot de câmera (senha no body, não em query string)
app.post('/api/dvr/snapshot', requirePageLogin, [
    body('dvrId').isInt().withMessage('ID do DVR é obrigatório'),
    body('channel').isInt().withMessage('Canal é obrigatório'),
    body('password').notEmpty().withMessage('Senha é obrigatória')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    try {
        const { dvrId, channel, password } = req.body;
        
        const dispositivo = await dvrService.obterDispositivo(dvrId);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }

        const imageBuffer = await intelbrasDvrService.getSnapshot(
            dispositivo.ip_address,
            dispositivo.porta || 80,
            dispositivo.usuario || 'admin',
            password,
            parseInt(channel)
        );

        if (imageBuffer) {
            res.setHeader('Content-Type', 'image/jpeg');
            res.send(imageBuffer);
        } else {
            res.status(500).json({ error: 'Falha ao capturar snapshot' });
        }
    } catch (error) {
        console.error('Erro ao capturar snapshot:', error);
        res.status(500).json({ error: 'Erro ao capturar snapshot' });
    }
});

// POST /api/dvr/recordings/find - Buscar gravações
app.post('/api/dvr/recordings/find', requirePageLogin, [
    body('dvrId').isInt().withMessage('ID do DVR é obrigatório'),
    body('channel').isInt().withMessage('Canal é obrigatório'),
    body('startTime').notEmpty().withMessage('Data inicial é obrigatória'),
    body('endTime').notEmpty().withMessage('Data final é obrigatória')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    try {
        const { dvrId, channel, startTime, endTime, password } = req.body;
        
        const dispositivo = await dvrService.obterDispositivo(dvrId);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }

        const recordings = await intelbrasDvrService.findRecordings(
            dispositivo.ip_address,
            dispositivo.porta || 80,
            dispositivo.usuario || 'admin',
            password,
            channel,
            new Date(startTime),
            new Date(endTime)
        );

        res.json({ success: true, recordings });
    } catch (error) {
        console.error('Erro ao buscar gravações:', error);
        res.status(500).json({ error: 'Erro ao buscar gravações' });
    }
});

// POST /api/dvr/rtsp-url - Obter URL RTSP para streaming (senha no body)
app.post('/api/dvr/rtsp-url', requirePageLogin, [
    body('dvrId').isInt().withMessage('ID do DVR é obrigatório'),
    body('channel').isInt().withMessage('Canal é obrigatório'),
    body('password').notEmpty().withMessage('Senha é obrigatória'),
    body('subtype').optional().isInt({ min: 0, max: 1 }).withMessage('Subtype deve ser 0 ou 1')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    try {
        const { dvrId, channel, password, subtype } = req.body;
        
        const dispositivo = await dvrService.obterDispositivo(dvrId);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }

        const rtspUrl = intelbrasDvrService.getRtspUrl(
            dispositivo.ip_address,
            dispositivo.porta_rtsp || 554,
            dispositivo.usuario || 'admin',
            password,
            parseInt(channel),
            parseInt(subtype) || 0
        );

        res.json({ success: true, rtspUrl });
    } catch (error) {
        console.error('Erro ao gerar URL RTSP:', error);
        res.status(500).json({ error: 'Erro ao gerar URL RTSP' });
    }
});

// POST /api/dvr/channels - Obter informações dos canais (senha no body)
app.post('/api/dvr/channels', requirePageLogin, [
    body('dvrId').isInt().withMessage('ID do DVR é obrigatório'),
    body('password').notEmpty().withMessage('Senha é obrigatória')
], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    try {
        const { dvrId, password } = req.body;
        
        const dispositivo = await dvrService.obterDispositivo(dvrId);
        if (!dispositivo) {
            return res.status(404).json({ error: 'Dispositivo não encontrado' });
        }

        const channelInfo = await intelbrasDvrService.getChannelInfo(
            dispositivo.ip_address,
            dispositivo.porta || 80,
            dispositivo.usuario || 'admin',
            password
        );

        if (channelInfo) {
            res.json({ success: true, channels: channelInfo });
        } else {
            res.status(500).json({ error: 'Falha ao obter informações dos canais' });
        }
    } catch (error) {
        console.error('Erro ao obter informações dos canais:', error);
        res.status(500).json({ error: 'Erro ao obter informações dos canais' });
    }
});

// =================================================================
// FIM DAS APIS DE DVR/NVR
// =================================================================

// ROTA DE EXPORTAÇÃO PARA EXCEL 
app.get('/api/export/excel', requirePageLogin, async (req, res) => { const { month, year } = req.query; if (!month || !year) { return res.status(400).json({ error: 'Mês e ano são obrigatórios.' }); } const monthFormatted = month.toString().padStart(2, '0'); const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE strftime('%Y-%m', r.data) = ? ORDER BY r.loja, r.data `; db.all(sql, [`${year}-${monthFormatted}`], async (err, rows) => { if (err) { console.error("Erro ao buscar relatórios para Excel:", err); return res.status(500).json({ error: 'Erro ao buscar relatórios.' }); } if (rows.length === 0) { return res.status(404).json({ error: 'Nenhum relatório encontrado para o período.' }); } const workbook = new ExcelJS.Workbook(); const safeParseFloat = (value) => { if (typeof value === 'number') { return value; } if (typeof value === 'string') { const cleaned = value.replace(/[R$\s]/g, '').replace(/\./g, '').replace(',', '.'); const num = parseFloat(cleaned); return isNaN(num) ? 0 : num; } return 0; }; const relatoriosPorLoja = rows.reduce((acc, row) => { const loja = row.loja; if (!acc[loja]) { acc[loja] = { funcao_especial: row.funcao_especial || 'Não definido', relatorios: [] }; } acc[loja].relatorios.push(processarRelatorio(row)); return acc; }, {}); for (const lojaNome in relatoriosPorLoja) { const lojaData = relatoriosPorLoja[lojaNome]; const worksheet = workbook.addWorksheet(lojaNome.substring(0, 30)); worksheet.mergeCells('A1:M1'); const tituloCell = worksheet.getCell('A1'); tituloCell.value = lojaNome.toUpperCase(); tituloCell.font = { name: 'Arial Black', size: 16, bold: true, color: { argb: 'FF44546A' } }; tituloCell.alignment = { vertical: 'middle', horizontal: 'center' }; worksheet.getRow(1).height = 30; const headers = [ 'DATA', 'BLUVE', 'VENDAS (L)', 'TX DE CONVERSÃO (L)', 'CLIENTES (M)', 'VENDAS (M)', 'TX DE CONVERSÃO (M)', 'P.A', 'TM', 'VALOR TOTAL', 'TROCAS' ]; let funcaoEspecialHeader = 'FUNÇÃO ESPECIAL'; if (lojaData.funcao_especial === 'Omni') { funcaoEspecialHeader = 'OMNI'; } else if (lojaData.funcao_especial === 'Busca por Assist. Tec.') { funcaoEspecialHeader = 'BUSCA P/ ASSIST. TEC.'; } headers.push(funcaoEspecialHeader); headers.push('ENVIADO POR'); const headerRow = worksheet.getRow(3); headerRow.values = headers; headerRow.height = 35; headerRow.eachCell(cell => { cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 10 }; cell.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true }; cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF4472C4' } }; cell.border = { top: { style: 'thin', color: { argb: 'FFBFBFBF' } }, left: { style: 'thin', color: { argb: 'FFBFBFBF' } }, bottom: { style: 'thin', color: { argb: 'FFBFBFBF' } }, right: { style: 'thin', color: { argb: 'FFBFBFBF' } } }; }); lojaData.relatorios.forEach(r => { const rowData = [ new Date(r.data + 'T00:00:00'), parseInt(r.clientes_loja, 10) || 0, parseInt(r.vendas_loja, 10) || 0, parseFloat(r.tx_conversao_loja) / 100, parseInt(r.clientes_monitoramento, 10) || 0, parseInt(r.vendas_monitoramento_total, 10) || 0, parseFloat(r.tx_conversao_monitoramento) / 100, parseFloat(String(r.pa).replace(',', '.')) || 0, safeParseFloat(r.ticket_medio), r.total_vendas_dinheiro, parseInt(r.quantidade_trocas, 10) || 0 ]; if (lojaData.funcao_especial === 'Omni') { rowData.push(parseInt(r.quantidade_omni, 10) || 0); } else if (lojaData.funcao_especial === 'Busca por Assist. Tec.') { rowData.push(parseInt(r.quantidade_funcao_especial, 10) || 0); } else { rowData.push(0); } rowData.push(r.enviado_por_usuario || '-'); const row = worksheet.addRow(rowData); row.getCell(1).numFmt = 'DD/MM/YYYY'; row.getCell(4).numFmt = '0.00%'; row.getCell(7).numFmt = '0.00%'; row.getCell(8).numFmt = '0.00'; row.getCell(9).numFmt = 'R$ #,##0.00'; row.getCell(10).numFmt = 'R$ #,##0.00'; row.eachCell(cell => { cell.alignment = { vertical: 'middle', horizontal: 'center' }; }); }); worksheet.columns.forEach(column => { let maxLength = 0; column.eachCell({ includeEmpty: true }, cell => { const length = cell.value ? cell.value.toString().length : 10; if (length > maxLength) { maxLength = length; } }); column.width = Math.max(12, maxLength + 3); }); worksheet.getColumn(4).width = 20; worksheet.getColumn(7).width = 20; worksheet.getColumn(12).width = 22; } res.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'); res.setHeader('Content-Disposition', `attachment; filename="Relatorios_${year}-${monthFormatted}.xlsx"`); await workbook.xlsx.write(res); res.end(); }); });

// ROTA DE EXPORTAÇÃO PARA EXCEL - TODOS OS RELATÓRIOS
app.get('/api/export/excel/all', requirePageLogin, async (req, res) => { 
    const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome ORDER BY r.loja, r.data `; 
    db.all(sql, [], async (err, rows) => { 
        if (err) { 
            console.error("Erro ao buscar relatórios para Excel:", err); 
            return res.status(500).json({ error: 'Erro ao buscar relatórios.' }); 
        } 
        if (rows.length === 0) { 
            return res.status(404).json({ error: 'Nenhum relatório encontrado no sistema.' }); 
        } 
        const workbook = new ExcelJS.Workbook(); 
        const safeParseFloat = (value) => { 
            if (typeof value === 'number') { return value; } 
            if (typeof value === 'string') { 
                const cleaned = value.replace(/[R$\s]/g, '').replace(/\./g, '').replace(',', '.'); 
                const num = parseFloat(cleaned); 
                return isNaN(num) ? 0 : num; 
            } 
            return 0; 
        }; 
        const relatoriosPorLoja = rows.reduce((acc, row) => { 
            const loja = row.loja; 
            if (!acc[loja]) { 
                acc[loja] = { funcao_especial: row.funcao_especial || 'Não definido', relatorios: [] }; 
            } 
            acc[loja].relatorios.push(processarRelatorio(row)); 
            return acc; 
        }, {}); 
        for (const lojaNome in relatoriosPorLoja) { 
            const lojaData = relatoriosPorLoja[lojaNome]; 
            const worksheet = workbook.addWorksheet(lojaNome.substring(0, 30)); 
            worksheet.mergeCells('A1:M1'); 
            const tituloCell = worksheet.getCell('A1'); 
            tituloCell.value = lojaNome.toUpperCase(); 
            tituloCell.font = { name: 'Arial Black', size: 16, bold: true, color: { argb: 'FF44546A' } }; 
            tituloCell.alignment = { vertical: 'middle', horizontal: 'center' }; 
            worksheet.getRow(1).height = 30; 
            const headers = [ 'DATA', 'BLUVE', 'VENDAS (L)', 'TX DE CONVERSÃO (L)', 'CLIENTES (M)', 'VENDAS (M)', 'TX DE CONVERSÃO (M)', 'P.A', 'TM', 'VALOR TOTAL', 'TROCAS' ]; 
            let funcaoEspecialHeader = 'FUNÇÃO ESPECIAL'; 
            if (lojaData.funcao_especial === 'Omni') { 
                funcaoEspecialHeader = 'OMNI'; 
            } else if (lojaData.funcao_especial === 'Busca por Assist. Tec.') { 
                funcaoEspecialHeader = 'BUSCA P/ ASSIST. TEC.'; 
            } 
            headers.push(funcaoEspecialHeader); 
            headers.push('ENVIADO POR'); 
            const headerRow = worksheet.getRow(3); 
            headerRow.values = headers; 
            headerRow.height = 35; 
            headerRow.eachCell(cell => { 
                cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 10 }; 
                cell.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true }; 
                cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF4472C4' } }; 
                cell.border = { top: { style: 'thin', color: { argb: 'FFBFBFBF' } }, left: { style: 'thin', color: { argb: 'FFBFBFBF' } }, bottom: { style: 'thin', color: { argb: 'FFBFBFBF' } }, right: { style: 'thin', color: { argb: 'FFBFBFBF' } } }; 
            }); 
            lojaData.relatorios.forEach(r => { 
                const rowData = [ 
                    new Date(r.data + 'T00:00:00'), 
                    parseInt(r.clientes_loja, 10) || 0, 
                    parseInt(r.vendas_loja, 10) || 0, 
                    parseFloat(r.tx_conversao_loja) / 100, 
                    parseInt(r.clientes_monitoramento, 10) || 0, 
                    parseInt(r.vendas_monitoramento_total, 10) || 0, 
                    parseFloat(r.tx_conversao_monitoramento) / 100, 
                    parseFloat(String(r.pa).replace(',', '.')) || 0, 
                    safeParseFloat(r.ticket_medio), 
                    r.total_vendas_dinheiro, 
                    parseInt(r.quantidade_trocas, 10) || 0 
                ]; 
                if (lojaData.funcao_especial === 'Omni') { 
                    rowData.push(parseInt(r.quantidade_omni, 10) || 0); 
                } else if (lojaData.funcao_especial === 'Busca por Assist. Tec.') { 
                    rowData.push(parseInt(r.quantidade_funcao_especial, 10) || 0); 
                } else { 
                    rowData.push(0); 
                } 
                rowData.push(r.enviado_por_usuario || '-'); 
                const row = worksheet.addRow(rowData); 
                row.getCell(1).numFmt = 'DD/MM/YYYY'; 
                row.getCell(4).numFmt = '0.00%'; 
                row.getCell(7).numFmt = '0.00%'; 
                row.getCell(8).numFmt = '0.00'; 
                row.getCell(9).numFmt = 'R$ #,##0.00'; 
                row.getCell(10).numFmt = 'R$ #,##0.00'; 
                row.eachCell(cell => { 
                    cell.alignment = { vertical: 'middle', horizontal: 'center' }; 
                }); 
            }); 
            worksheet.columns.forEach(column => { 
                let maxLength = 0; 
                column.eachCell({ includeEmpty: true }, cell => { 
                    const length = cell.value ? cell.value.toString().length : 10; 
                    if (length > maxLength) { maxLength = length; } 
                }); 
                column.width = Math.max(12, maxLength + 3); 
            }); 
            worksheet.getColumn(4).width = 20; 
            worksheet.getColumn(7).width = 20; 
            worksheet.getColumn(12).width = 22; 
        } 
        const currentDate = new Date().toLocaleDateString('pt-BR').replace(/\//g, '-');
        res.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'); 
        res.setHeader('Content-Disposition', `attachment; filename="Todos_Relatorios_${currentDate}.xlsx"`); 
        await workbook.xlsx.write(res); 
        res.end(); 
    }); 
});

// APIs DE DASHBOARD, DEMANDAS, BACKUP E RESTORE 
app.get('/api/dashboard-data', requirePageLogin, (req, res) => { 
    let whereClauses = []; 
    let params = []; 
    
    // Aplicar filtro de lojas baseado no role
    
    if (req.query.loja && req.query.loja !== 'todas') { 
        whereClauses.push('loja = ?'); 
        params.push(req.query.loja); 
    } 
    if (req.query.data_inicio) { 
        whereClauses.push('data >= ?'); 
        params.push(req.query.data_inicio); 
    } 
    if (req.query.data_fim) { 
        whereClauses.push('data <= ?'); 
        params.push(req.query.data_fim); 
    } 
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : ''; 
    const sql = `SELECT COALESCE(SUM(clientes_monitoramento),0) as total_clientes_monitoramento, COALESCE(SUM(vendas_monitoramento),0) as total_vendas_monitoramento, COALESCE(SUM(clientes_loja),0) as total_clientes_loja, COALESCE(SUM(vendas_loja),0) as total_vendas_loja, COALESCE(SUM(quantidade_omni),0) as total_omni FROM relatorios ${whereString}`; 
    db.get(sql, params, (err, row) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        
        const allowedMonitoramentoRoles = ['admin', 'monitoramento', 'dev', 'consultor'];
        const canViewMonitoramento = allowedMonitoramentoRoles.includes(req.session.role);
        
        const vendas_m_total = (row.total_vendas_monitoramento || 0) + (row.total_omni || 0); 
        const response = { 
            ...row, 
            tx_conversao_monitoramento: (row.total_clientes_monitoramento > 0 ? (vendas_m_total / row.total_clientes_monitoramento) * 100 : 0), 
            tx_conversao_loja: (row.total_clientes_loja > 0 ? (row.total_vendas_loja / row.total_clientes_loja) * 100 : 0),
            canViewMonitoramento 
        };
        
        if (!canViewMonitoramento) {
            delete response.total_clientes_monitoramento;
            delete response.total_vendas_monitoramento;
            delete response.total_omni;
            delete response.tx_conversao_monitoramento;
        }
        
        res.json(response); 
    }); 
});

app.get('/api/dashboard-bluve', requirePageLogin, (req, res) => { 
    let whereClauses = []; 
    let params = []; 
    
    
    if (req.query.loja && req.query.loja !== 'todas') { 
        whereClauses.push('loja = ?'); 
        params.push(req.query.loja); 
    } 
    if (req.query.data_inicio) { 
        whereClauses.push('data >= ?'); 
        params.push(req.query.data_inicio); 
    } 
    if (req.query.data_fim) { 
        whereClauses.push('data <= ?'); 
        params.push(req.query.data_fim); 
    } 
    
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : ''; 
    const sql = `SELECT 
        COALESCE(SUM(clientes_loja), 0) as clientes_atendidos, 
        COALESCE(SUM(vendas_loja), 0) as vendas,
        CASE 
            WHEN SUM(clientes_loja) > 0 THEN (CAST(SUM(vendas_loja) AS REAL) / SUM(clientes_loja)) * 100
            ELSE 0
        END as taxa_conversao
    FROM relatorios ${whereString}`; 
    
    db.get(sql, params, (err, row) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        res.json({
            clientes_atendidos: row.clientes_atendidos || 0,
            vendas: row.vendas || 0,
            taxa_conversao: row.taxa_conversao ? parseFloat(row.taxa_conversao.toFixed(2)) : 0
        }); 
    }); 
});
app.get('/api/ranking', requirePageLogin, (req, res) => { 
    let whereClauses = ['l.status = \'ativa\'']; 
    let params = []; 
    
    // Aplicar filtro de lojas baseado no role (no LEFT JOIN)
    
    let joinConditions = [];
    if (req.query.data_inicio) { 
        joinConditions.push('r.data >= ?'); 
        params.push(req.query.data_inicio); 
    } 
    if (req.query.data_fim) { 
        joinConditions.push('r.data <= ?'); 
        params.push(req.query.data_fim); 
    } 
    const joinCondition = joinConditions.length > 0 ? `AND ${joinConditions.join(' AND ')}` : ''; 
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : '';
    const sql = `SELECT l.nome as loja, COALESCE(SUM(r.clientes_loja), 0) as total_clientes_loja, COALESCE(SUM(r.vendas_loja), 0) as total_vendas_loja, COALESCE(SUM(r.clientes_monitoramento), 0) as total_clientes_monitoramento, COALESCE(SUM(r.vendas_monitoramento), 0) as total_vendas_monitoramento, COALESCE(SUM(r.quantidade_omni), 0) as total_omni FROM lojas l LEFT JOIN relatorios r ON l.nome = r.loja ${joinCondition} ${whereString} GROUP BY l.nome`; 
    db.all(sql, params, (err, rows) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        
        const ranking = rows.map(r => { 
            const vendas_m_total = (r.total_vendas_monitoramento || 0) + (r.total_omni || 0); 
            return { 
                ...r, 
                tx_loja: (r.total_clientes_loja > 0 ? (r.total_vendas_loja / r.total_clientes_loja) * 100 : 0), 
                tx_monitoramento: (r.total_clientes_monitoramento > 0 ? (vendas_m_total / r.total_clientes_monitoramento) * 100 : 0) 
            };
        }); 
        res.json(ranking); 
    }); 
});
app.get('/api/dashboard/chart-data', requirePageLogin, (req, res) => {
    const { loja, data_inicio, data_fim } = req.query;
    let whereClauses = [];
    let params = [];
    
    // Aplicar filtro de lojas baseado no role
    
    // Filtro específico por loja (query param)
    if (loja && loja !== 'todas' && !lojaFilter) {
        whereClauses.push('TRIM(loja) = ?');
        params.push(loja);
    }
    
    // Filtros de data
    if (data_inicio) {
        whereClauses.push('data >= ?');
        params.push(data_inicio);
    }
    if (data_fim) {
        whereClauses.push('data <= ?');
        params.push(data_fim);
    }
    
    // Se não tem nenhum filtro de data, pegar últimos 30 dias
    if (!data_inicio && !data_fim) {
        const date = new Date();
        date.setDate(date.getDate() - 30);
        const startDate = date.toISOString().slice(0, 10);
        whereClauses.push('data >= ?');
        params.push(startDate);
    }
    
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : '';
    const sql = `SELECT data, SUM(clientes_loja) as total_clientes_loja, SUM(vendas_loja) as total_vendas_loja, SUM(clientes_monitoramento) as total_clientes_monitoramento, SUM(vendas_monitoramento) as total_vendas_monitoramento, SUM(quantidade_omni) as total_omni FROM relatorios ${whereString} GROUP BY data ORDER BY data ASC`;
    
    db.all(sql, params, (err, rows) => {
        if (err) return res.status(500).json({ error: 'Erro ao buscar dados para o gráfico.' });
        
        const labels = [];
        const txConversaoLoja = [];
        const txConversaoMonitoramento = [];
        
        rows.forEach(row => {
            labels.push(new Date(row.data).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', timeZone: 'UTC' }));
            const tx_l = row.total_clientes_loja > 0 ? (row.total_vendas_loja / row.total_clientes_loja) * 100 : 0;
            txConversaoLoja.push(tx_l.toFixed(2));
            
            const vendas_m_total = (row.total_vendas_monitoramento || 0) + (row.total_omni || 0);
            const tx_m = row.total_clientes_monitoramento > 0 ? (vendas_m_total / row.total_clientes_monitoramento) * 100 : 0;
            txConversaoMonitoramento.push(tx_m.toFixed(2));
        });
        
        res.json({ labels, txConversaoLoja, txConversaoMonitoramento });
    });
});

app.get('/api/dashboard/store-performance', requirePageLogin, (req, res) => {
    const { data_inicio, data_fim } = req.query;
    let whereClauses = [];
    let params = [];
    
    if (data_inicio) {
        whereClauses.push('data >= ?');
        params.push(data_inicio);
    }
    if (data_fim) {
        whereClauses.push('data <= ?');
        params.push(data_fim);
    }
    
    if (!data_inicio && !data_fim) {
        const date = new Date();
        date.setDate(date.getDate() - 30);
        const startDate = date.toISOString().slice(0, 10);
        whereClauses.push('data >= ?');
        params.push(startDate);
    }
    
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : '';
    
    const sql = `
        SELECT 
            loja,
            SUM(vendas_loja) as total_vendas,
            AVG(ticket_medio) as ticket_medio_avg,
            AVG(pa) as pa_avg,
            SUM(vendas_cartao) as total_vendas_cartao,
            SUM(vendas_pix) as total_vendas_pix,
            SUM(vendas_dinheiro) as total_vendas_dinheiro,
            SUM(clientes_loja) as total_clientes,
            COUNT(*) as dias_registrados
        FROM relatorios 
        ${whereString}
        GROUP BY loja
        ORDER BY total_vendas DESC
    `;
    
    db.all(sql, params, (err, rows) => {
        if (err) {
            console.error('Erro ao buscar métricas de desempenho:', err);
            return res.status(500).json({ error: 'Erro ao buscar métricas de desempenho.' });
        }
        
        const metrics = rows.map(row => {
            const diasRegistrados = row.dias_registrados || 1;
            const totalVendas = row.total_vendas || 0;
            const vendasMediaDia = totalVendas / diasRegistrados;
            
            return {
                loja: row.loja,
                vendas_media_dia: parseFloat(vendasMediaDia.toFixed(2)),
                total_vendas: totalVendas,
                ticket_medio: parseFloat((row.ticket_medio_avg || 0).toFixed(2)),
                pa: parseFloat((row.pa_avg || 0).toFixed(2)),
                formas_pagamento: {
                    cartao: row.total_vendas_cartao || 0,
                    pix: row.total_vendas_pix || 0,
                    dinheiro: row.total_vendas_dinheiro || 0
                },
                total_clientes: row.total_clientes || 0,
                dias_registrados: diasRegistrados
            };
        }).sort((a, b) => b.vendas_media_dia - a.vendas_media_dia);
        
        res.json(metrics);
    });
});

app.post('/api/demandas', requirePageLogin, (req, res) => { const { loja_nome, descricao, tag } = req.body; db.run('INSERT INTO demandas (loja_nome, descricao, tag, criado_por_usuario) VALUES (?, ?, ?, ?)', [loja_nome, descricao, tag, req.session.username], function (err) { if (err) return res.status(500).json({ error: 'Falha ao salvar demanda.' }); res.status(201).json({ success: true, id: this.lastID }); }); });
app.get('/api/demandas/:status', requirePageLogin, (req, res) => { 
    const status = req.params.status === 'pendentes' ? 'pendente' : 'concluido'; 
    const whereClauses = ['status = ?'];
    const params = [status];
    
    // Aplicar filtro de lojas baseado no role
    
    const whereString = whereClauses.join(' AND ');
    const query = `SELECT * FROM demandas WHERE ${whereString} ORDER BY criado_em DESC`;
    
    db.all(query, params, (err, demandas) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        res.json(demandas || []); 
    }); 
});
app.put('/api/demandas/:id/concluir', requirePageLogin, (req, res) => { db.run("UPDATE demandas SET status = 'concluido', concluido_por_usuario = ?, concluido_em = CURRENT_TIMESTAMP WHERE id = ?", [req.session.username, req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao concluir demanda.' }); if (this.changes === 0) return res.status(404).json({ error: 'Demanda não encontrada.' }); res.json({ success: true }); }); });
app.delete('/api/demandas/:id', requirePageLogin, (req, res) => { db.run("DELETE FROM demandas WHERE id = ?", [req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao excluir demanda.' }); if (this.changes === 0) return res.status(404).json({ error: "Demanda não encontrada." }); res.json({ success: true }); }); });
app.get('/api/backup/info', requirePageLogin, (req, res) => {
    try {
        const stats = fs.statSync(DB_PATH);
        const sizeMB = (stats.size / (1024 * 1024)).toFixed(2);
        res.json({ sizeMB });
    } catch (error) {
        console.error("Erro ao obter informações do backup:", error);
        res.status(500).json({ error: 'Não foi possível obter informações do banco de dados.' });
    }
});

// API para limpar tabelas específicas do banco de dados
app.delete('/api/backup/clear', requirePageLogin, (req, res) => {
    db.serialize(() => {
        db.run("DELETE FROM relatorios", (err) => {
            if (err) return res.status(500).json({ error: 'Erro ao limpar relatórios.' });
        });
        db.run("DELETE FROM demandas", (err) => {
            if (err) return res.status(500).json({ error: 'Erro ao limpar demandas.' });
        });
        res.json({ success: true, message: 'Relatórios e demandas foram limpos.' });
    });
});
app.get('/api/backup/download', requirePageLogin, (req, res) => { const date = new Date().toISOString().slice(0, 10); const fileName = `backup_reports_${date}.db`; res.download(DB_PATH, fileName, (err) => { if (err && !res.headersSent) { res.status(500).send("Não foi possível baixar o arquivo de backup."); } }); });
app.post('/api/backup/restore', requirePageLogin, upload.single('backupFile'), (req, res) => { if (!req.file) { return res.status(400).json({ error: "Nenhum arquivo de backup foi enviado." }); } const backupBuffer = req.file.buffer; db.close((err) => { if (err) { console.error("Erro ao fechar o DB antes de restaurar:", err.message); return res.status(500).json({ error: "Não foi possível fechar a conexão com o banco de dados atual." }); } fs.writeFile(DB_PATH, backupBuffer, (err) => { if (err) { console.error("Falha ao escrever o arquivo de backup:", err.message); db = new sqlite3.Database(DB_PATH); return res.status(500).json({ error: "Falha ao substituir o arquivo de banco de dados." }); } db = new sqlite3.Database(DB_PATH, (err) => { if (err) { console.error("DB restaurado, mas falha ao reconectar:", err.message); return res.status(500).json({ error: "Banco de dados restaurado, mas falha ao reconectar. Reinicie o servidor." }); } console.log("Banco de dados restaurado e reconectado com sucesso."); res.json({ success: true, message: "Banco de dados restaurado com sucesso. A página será recarregada." }); }); }); }); });

// APIs DE LOGS (apenas para Dev)
app.get('/api/logs', requirePageLogin, (req, res) => {
    const { type, start, end, limit, offset } = req.query;
    let whereClauses = [];
    let params = [];
    
    if (type && type !== 'all') {
        whereClauses.push('type = ?');
        params.push(type);
    }
    if (start) {
        whereClauses.push('timestamp >= ?');
        params.push(start);
    }
    if (end) {
        whereClauses.push('timestamp <= ?');
        params.push(end + ' 23:59:59');
    }
    
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : '';
    const limitNum = parseInt(limit) || 50;
    const offsetNum = parseInt(offset) || 0;
    
    // Buscar total
    db.get(`SELECT COUNT(*) as total FROM logs ${whereString}`, params, (err, countRow) => {
        if (err) return res.status(500).json({ error: err.message });
        
        // Buscar logs
        db.all(`SELECT * FROM logs ${whereString} ORDER BY timestamp DESC LIMIT ? OFFSET ?`, 
            [...params, limitNum, offsetNum], 
            (err, logs) => {
                if (err) return res.status(500).json({ error: err.message });
                
                // Buscar estatísticas (últimas 24h)
                const last24h = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString();
                db.all(`SELECT type, COUNT(*) as count FROM logs WHERE timestamp >= ? GROUP BY type`, [last24h], (err, statsRows) => {
                    const stats = {
                        errors: 0,
                        warnings: 0,
                        activeUsers: 0,
                        uptime: Math.floor(process.uptime() / 60) + ' min'
                    };
                    
                    if (!err && statsRows) {
                        statsRows.forEach(row => {
                            if (row.type === 'error') stats.errors = row.count;
                            if (row.type === 'warning') stats.warnings = row.count;
                        });
                    }
                    
                    // Contar usuários ativos (última hora)
                    const lastHour = new Date(Date.now() - 60 * 60 * 1000).toISOString();
                    db.get(`SELECT COUNT(DISTINCT username) as count FROM logs WHERE timestamp >= ? AND type = 'access'`, [lastHour], (err, userRow) => {
                        if (!err && userRow) stats.activeUsers = userRow.count;
                        res.json({ logs, total: countRow.total, stats });
                    });
                });
            }
        );
    });
});

app.delete('/api/logs', requirePageLogin, (req, res) => {
    db.run('DELETE FROM logs', (err) => {
        if (err) return res.status(500).json({ error: 'Erro ao limpar logs.' });
        res.json({ success: true, message: 'Logs limpos com sucesso.' });
    });
});

// Função auxiliar para obter IP real do cliente
function getClientIp(req) {
    return req.headers['x-forwarded-for']?.split(',')[0]?.trim() || 
           req.headers['x-real-ip'] || 
           req.connection?.remoteAddress || 
           req.socket?.remoteAddress ||
           'unknown';
}

// Função auxiliar para criar hash de payload (sem expor dados sensíveis)
function hashPayload(data) {
    if (!data || typeof data !== 'object') return null;
    const sanitized = JSON.stringify(data);
    return crypto.createHash('sha256').update(sanitized).digest('hex').substring(0, 16);
}

// Função auxiliar para registrar logs com auditoria completa
function logEvent(type, username, action, details, req = null) {
    const ip_address = req ? getClientIp(req) : null;
    const user_agent = req ? req.headers['user-agent'] : null;
    const route = req ? req.path : null;
    const event_type = type;
    const payload_hash = req ? hashPayload(req.body) : null;
    
    db.run(
        `INSERT INTO logs (type, username, action, details, ip_address, user_agent, event_type, route, payload_hash) 
         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`, 
        [type, username, action, details, ip_address, user_agent, event_type, route, payload_hash],
        (err) => { 
            if (err && !err.message.includes('no column named')) {
                console.error('Erro ao registrar log:', err.message); 
            }
        }
    );
}

// Exportar função de log para usar em outras rotas
global.logEvent = logEvent;

// =================================================================
// ROTAS DE ASSISTÊNCIA TÉCNICA
// =================================================================

// Página principal de Assistência Técnica
app.get('/assistencia', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'assistencia.html'));
});

// Página de Alertas para Técnicos (via SPA)
app.get('/alertas-tecnico', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// API - Listar Estoque Técnico
app.get('/api/estoque-tecnico', requirePageLogin, (req, res) => {
    const search = req.query.search || '';
    const disponivel = req.query.disponivel;
    const loja = req.query.loja;
    
    let whereClauses = [];
    let params = [];
    
    if (search) {
        whereClauses.push('(nome_peca LIKE ? OR codigo_interno LIKE ?)');
        params.push(`%${search}%`, `%${search}%`);
    }
    
    if (disponivel === 'true') {
        whereClauses.push('quantidade > 0');
    }
    
    if (loja) {
        whereClauses.push('loja = ?');
        params.push(loja);
    }
    
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : '';
    
    db.all(`SELECT * FROM estoque_tecnico ${whereString} ORDER BY nome_peca ASC`, params, (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows || []);
    });
});

// API - Adicionar Peça ao Estoque
app.post('/api/estoque-tecnico', requirePageLogin, (req, res) => {
    const { nome_peca, codigo_interno, quantidade, valor_custo, loja } = req.body;
    
    if (!nome_peca || !codigo_interno || !loja) {
        return res.status(400).json({ error: 'Nome da peça, código interno e loja são obrigatórios.' });
    }
    
    db.run(
        'INSERT INTO estoque_tecnico (nome_peca, codigo_interno, quantidade, valor_custo, loja) VALUES (?, ?, ?, ?, ?)',
        [nome_peca, codigo_interno, quantidade || 0, valor_custo || 0, loja],
        function(err) {
            if (err) {
                if (err.message.includes('UNIQUE')) {
                    return res.status(400).json({ error: 'Código interno já existe.' });
                }
                return res.status(500).json({ error: err.message });
            }
            logEvent('info', req.session.username, 'estoque_add', `Peça adicionada: ${nome_peca} (Loja: ${loja})`);
            res.status(201).json({ success: true, id: this.lastID });
        }
    );
});

// API - Atualizar Estoque de Peça
app.put('/api/estoque-tecnico/:id', requirePageLogin, (req, res) => {
    const { id } = req.params;
    const { nome_peca, codigo_interno, quantidade, valor_custo, loja } = req.body;
    
    db.run(
        `UPDATE estoque_tecnico SET 
            nome_peca = ?, 
            codigo_interno = ?, 
            quantidade = ?, 
            valor_custo = ?,
            loja = ?,
            updated_at = CURRENT_TIMESTAMP
        WHERE id = ?`,
        [nome_peca, codigo_interno, quantidade, valor_custo, loja, id],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            if (this.changes === 0) return res.status(404).json({ error: 'Peça não encontrada.' });
            logEvent('info', req.session.username, 'estoque_update', `Estoque atualizado: ID ${id} (Loja: ${loja})`);
            res.json({ success: true });
        }
    );
});

// API - Deletar Peça do Estoque
app.delete('/api/estoque-tecnico/:id', requirePageLogin, (req, res) => {
    const { id } = req.params;
    
    db.run('DELETE FROM estoque_tecnico WHERE id = ?', [id], function(err) {
        if (err) return res.status(500).json({ error: err.message });
        if (this.changes === 0) return res.status(404).json({ error: 'Peça não encontrada.' });
        logEvent('info', req.session.username, 'estoque_delete', `Peça removida: ID ${id}`);
        res.json({ success: true });
    });
});

// API - Listar Assistências
app.get('/api/assistencias', requirePageLogin, (req, res) => {
    const status = req.query.status;
    const search = req.query.search || '';
    const limit = req.query.limit ? parseInt(req.query.limit) : null;
    const lojaFilter = req.query.loja || '';
    
    let whereClauses = [];
    let params = [];
    
    // Filtro opcional por loja específica
    if (lojaFilter) {
        whereClauses.push('TRIM(loja) = ?');
        params.push(lojaFilter.trim());
    }
    
    if (status && status !== 'todos') {
        // Suportar múltiplos status separados por vírgula
        const statusList = status.split(',').map(s => s.trim());
        if (statusList.length === 1) {
            whereClauses.push('status = ?');
            params.push(statusList[0]);
        } else {
            const placeholders = statusList.map(() => '?').join(',');
            whereClauses.push(`status IN (${placeholders})`);
            params.push(...statusList);
        }
    }
    
    if (search) {
        whereClauses.push('(cliente_nome LIKE ? OR cliente_cpf LIKE ? OR numero_pedido LIKE ?)');
        params.push(`%${search}%`, `%${search}%`, `%${search}%`);
    }
    
    const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : '';
    const limitString = limit ? ` LIMIT ${limit}` : '';
    
    db.all(`SELECT * FROM assistencias ${whereString} ORDER BY created_at DESC${limitString}`, params, (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows || []);
    });
});

// API - Criar Assistência
app.post('/api/assistencias', requirePageLogin, (req, res) => {
    const {
        cliente_nome,
        cliente_cpf,
        numero_pedido,
        data_entrada,
        valor_peca_loja,
        valor_servico_cliente,
        aparelho,
        peca_id,
        peca_nome,
        observacoes,
        loja
    } = req.body;
    
    if (!cliente_nome || !cliente_cpf || !data_entrada || !aparelho) {
        return res.status(400).json({ error: 'Campos obrigatórios: cliente, CPF, data de entrada e aparelho.' });
    }
    
    db.run(
        `INSERT INTO assistencias (
            cliente_nome, cliente_cpf, numero_pedido, data_entrada,
            valor_peca_loja, valor_servico_cliente, aparelho,
            peca_id, peca_nome, observacoes, tecnico_responsavel, loja
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
        [
            cliente_nome, cliente_cpf, numero_pedido, data_entrada,
            valor_peca_loja || 0, valor_servico_cliente || 0, aparelho,
            peca_id, peca_nome, observacoes, req.session.username, loja
        ],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            logEvent('info', req.session.username, 'assistencia_create', `Assistência criada: ${cliente_nome}`);
            res.status(201).json({ success: true, id: this.lastID });
        }
    );
});

// API - Atualizar Assistência
app.put('/api/assistencias/:id', requirePageLogin, (req, res) => {
    const { id } = req.params;
    const {
        cliente_nome,
        cliente_cpf,
        numero_pedido,
        data_entrada,
        data_conclusao,
        valor_peca_loja,
        valor_servico_cliente,
        aparelho,
        peca_id,
        peca_nome,
        observacoes,
        status,
        loja
    } = req.body;
    
    db.run(
        `UPDATE assistencias SET 
            cliente_nome = ?, cliente_cpf = ?, numero_pedido = ?,
            data_entrada = ?, data_conclusao = ?,
            valor_peca_loja = ?, valor_servico_cliente = ?,
            aparelho = ?, peca_id = ?, peca_nome = ?,
            observacoes = ?, status = ?, loja = ?,
            updated_at = CURRENT_TIMESTAMP
        WHERE id = ?`,
        [
            cliente_nome, cliente_cpf, numero_pedido, data_entrada, data_conclusao,
            valor_peca_loja, valor_servico_cliente, aparelho, peca_id, peca_nome,
            observacoes, status, loja, id
        ],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            if (this.changes === 0) return res.status(404).json({ error: 'Assistência não encontrada.' });
            logEvent('info', req.session.username, 'assistencia_update', `Assistência atualizada: ID ${id}`);
            res.json({ success: true });
        }
    );
});

// API - Concluir Assistência (atualiza status e estoque)
app.post('/api/assistencias/:id/concluir', requirePageLogin, (req, res) => {
    const { id } = req.params;
    
    // Buscar assistência
    db.get('SELECT * FROM assistencias WHERE id = ?', [id], (err, assistencia) => {
        if (err) return res.status(500).json({ error: err.message });
        if (!assistencia) return res.status(404).json({ error: 'Assistência não encontrada.' });
        
        // Atualizar assistência para concluída
        db.run(
            `UPDATE assistencias SET 
                status = 'Concluído',
                data_conclusao = ?,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = ?`,
            [new Date().toISOString().split('T')[0], id],
            function(err) {
                if (err) return res.status(500).json({ error: err.message });
                
                // Se tem peça associada, atualizar estoque
                if (assistencia.peca_id) {
                    db.run(
                        'UPDATE estoque_tecnico SET quantidade = quantidade - 1 WHERE id = ? AND quantidade > 0',
                        [assistencia.peca_id],
                        (err) => {
                            if (err) console.error('Erro ao atualizar estoque:', err.message);
                        }
                    );
                }
                
                logEvent('info', req.session.username, 'assistencia_complete', `Assistência concluída: ${assistencia.cliente_nome}`);
                res.json({ success: true, message: 'Assistência concluída com sucesso!' });
            }
        );
    });
});

// API - Histórico de Assistências (apenas concluídas)
app.get('/api/assistencias/historico', requirePageLogin, (req, res) => {
    const search = req.query.search || '';
    const lojaFilter = req.query.loja || '';
    
    let whereClauses = ["status = 'Concluído'"];
    let params = [];
    
    // Filtro opcional por loja específica
    if (lojaFilter) {
        whereClauses.push('TRIM(loja) = ?');
        params.push(lojaFilter.trim());
    }
    
    if (search) {
        whereClauses.push('(cliente_nome LIKE ? OR cliente_cpf LIKE ? OR numero_pedido LIKE ?)');
        params.push(`%${search}%`, `%${search}%`, `%${search}%`);
    }
    
    const whereString = `WHERE ${whereClauses.join(' AND ')}`;
    
    db.all(`SELECT * FROM assistencias ${whereString} ORDER BY data_conclusao DESC, created_at DESC`, params, (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows || []);
    });
});

// API - Deletar Assistência (apenas concluídas)
app.delete('/api/assistencias/:id', requirePageLogin, (req, res) => {
    const { id } = req.params;
    
    // Primeiro, verificar se a assistência existe e se está concluída
    db.get('SELECT id, status FROM assistencias WHERE id = ?', [id], (err, row) => {
        if (err) return res.status(500).json({ error: err.message });
        if (!row) return res.status(404).json({ error: 'Assistência não encontrada.' });
        
        // Verificar se está concluída
        if (row.status !== 'Concluído') {
            return res.status(403).json({ 
                error: 'Apenas assistências concluídas podem ser removidas.' 
            });
        }
        
        // Se estiver concluída, deletar
        db.run('DELETE FROM assistencias WHERE id = ?', [id], function(err) {
            if (err) return res.status(500).json({ error: err.message });
            logEvent('info', req.session.username, 'assistencia_delete', `Assistência concluída removida: ID ${id}`);
            res.json({ success: true });
        });
    });
});

// API - Estatísticas de Assistência Técnica para Dashboard
app.get('/api/assistencias/stats-tecnico', requirePageLogin, (req, res) => {
    const hoje = new Date().toISOString().split('T')[0];
    const mesAtual = hoje.substring(0, 7);
    
    // Em andamento
    db.get(`
        SELECT COUNT(*) as total 
        FROM assistencias 
        WHERE status = 'Em andamento'
    `, [], (err, emAndamento) => {
        if (err) return res.status(500).json({ error: err.message });
        
        // Concluídas hoje
        db.get(`
            SELECT COUNT(*) as total 
            FROM assistencias 
            WHERE status = 'Concluído' AND DATE(data_saida) = ?
        `, [hoje], (err2, concluidasHoje) => {
            if (err2) return res.status(500).json({ error: err2.message });
            
            // Total do mês
            db.get(`
                SELECT COUNT(*) as total 
                FROM assistencias 
                WHERE strftime('%Y-%m', data_entrada) = ?
            `, [mesAtual], (err3, totalMes) => {
                if (err3) return res.status(500).json({ error: err3.message });
                
                // Aguardando peças
                db.get(`
                    SELECT COUNT(*) as total 
                    FROM assistencias 
                    WHERE status = 'Aguardando peças'
                `, [], (err4, aguardandoPecas) => {
                    if (err4) return res.status(500).json({ error: err4.message });
                    
                    res.json({
                        emAndamento: emAndamento?.total || 0,
                        concluidasHoje: concluidasHoje?.total || 0,
                        totalMes: totalMes?.total || 0,
                        aguardandoPecas: aguardandoPecas?.total || 0
                    });
                });
            });
        });
    });
});

app.get('/api/assistencias/stats', requirePageLogin, (req, res) => {
    // Técnico com mais assistências
    db.get(`
        SELECT tecnico_responsavel, COUNT(*) as total 
        FROM assistencias 
        WHERE tecnico_responsavel IS NOT NULL 
        GROUP BY tecnico_responsavel 
        ORDER BY total DESC 
        LIMIT 1
    `, (err, topTecnico) => {
        if (err) return res.status(500).json({ error: err.message });
        
        // Loja com mais assistências
        db.get(`
            SELECT loja, COUNT(*) as total 
            FROM assistencias 
            WHERE loja IS NOT NULL 
            GROUP BY loja 
            ORDER BY total DESC 
            LIMIT 1
        `, (err2, topLoja) => {
            if (err2) return res.status(500).json({ error: err2.message });
            
            // Total de assistências e valores
            db.get(`
                SELECT 
                    COUNT(*) as total_assistencias,
                    SUM(CASE WHEN status = 'Concluído' THEN 1 ELSE 0 END) as concluidas,
                    SUM(CASE WHEN status = 'Em andamento' THEN 1 ELSE 0 END) as em_andamento,
                    SUM(valor_peca_loja + valor_servico_cliente) as valor_total
                FROM assistencias
            `, (err3, totais) => {
                if (err3) return res.status(500).json({ error: err3.message });
                
                res.json({
                    topTecnico: topTecnico || { tecnico_responsavel: '-', total: 0 },
                    topLoja: topLoja || { loja: '-', total: 0 },
                    totais: totais || { total_assistencias: 0, concluidas: 0, em_andamento: 0, valor_total: 0 }
                });
            });
        });
    });
});

// API - Assistências por Loja (detalhamento)
app.get('/api/assistencias/por-loja', requirePageLogin, (req, res) => {
    db.all(`
        SELECT 
            loja,
            COUNT(*) as total,
            SUM(CASE WHEN status = 'Concluído' THEN 1 ELSE 0 END) as concluidas,
            SUM(CASE WHEN status = 'Em andamento' THEN 1 ELSE 0 END) as em_andamento,
            SUM(valor_peca_loja + valor_servico_cliente) as valor_total
        FROM assistencias
        WHERE loja IS NOT NULL
        GROUP BY loja
        ORDER BY total DESC
    `, (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows || []);
    });
});

// API - Estatísticas Diárias de Assistência com Filtro de Loja
app.get('/api/assistencias/stats-daily', requirePageLogin, (req, res) => {
    const { loja } = req.query;
    const hoje = new Date().toISOString().split('T')[0];
    
    let whereClauses = [`DATE(data_saida) = ?`];
    let params = [hoje];
    
    if (loja && loja !== 'todas') {
        whereClauses.push('TRIM(loja) = ?');
        params.push(loja.trim());
    }
    
    const whereString = whereClauses.join(' AND ');
    
    db.get(`
        SELECT 
            SUM(CASE WHEN status = 'Concluído' THEN 1 ELSE 0 END) as concluidas_hoje,
            SUM(CASE WHEN status = 'Concluído' THEN (valor_peca_loja + valor_servico_cliente) ELSE 0 END) as faturamento_hoje
        FROM assistencias
        WHERE ${whereString}
    `, params, (err, totais) => {
        if (err) return res.status(500).json({ error: err.message });
        
        let whereClausesAndamento = ['status = ?'];
        let paramsAndamento = ['Em andamento'];
        
        if (loja && loja !== 'todas') {
            whereClausesAndamento.push('TRIM(loja) = ?');
            paramsAndamento.push(loja.trim());
        }
        
        const whereStringAndamento = whereClausesAndamento.join(' AND ');
        
        db.get(`
            SELECT COUNT(*) as em_andamento
            FROM assistencias
            WHERE ${whereStringAndamento}
        `, paramsAndamento, (err2, andamento) => {
            if (err2) return res.status(500).json({ error: err2.message });
            
            res.json({
                concluidas_hoje: totais?.concluidas_hoje || 0,
                faturamento_hoje: totais?.faturamento_hoje || 0,
                em_andamento: andamento?.em_andamento || 0
            });
        });
    });
});

// API - Lista de Tickets de Assistência
app.get('/api/assistencias/tickets', requirePageLogin, (req, res) => {
    const { loja, limit } = req.query;
    
    let whereClauses = ["status IN ('Em andamento', 'Aguardando peças')"];
    let params = [];
    
    if (loja && loja !== 'todas') {
        whereClauses.push('TRIM(loja) = ?');
        params.push(loja.trim());
    }
    
    const whereString = whereClauses.join(' AND ');
    const limitClause = limit ? `LIMIT ${parseInt(limit)}` : 'LIMIT 50';
    
    db.all(`
        SELECT 
            id,
            cliente_nome,
            cliente_cpf,
            numero_pedido,
            aparelho,
            status,
            loja,
            data_entrada,
            data_saida,
            tecnico_responsavel,
            defeito_reclamado,
            valor_peca_loja,
            valor_servico_cliente
        FROM assistencias
        WHERE ${whereString}
        ORDER BY data_entrada DESC
        ${limitClause}
    `, params, (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows || []);
    });
});

// =================================================================
// INICIALIZAÇÃO DO SERVIDOR
// =================================================================
const startTime = new Date();
console.log(`Iniciando servidor em ${startTime.toLocaleString('pt-BR')}...`);

app.listen(PORT, '0.0.0.0', async () => {
    console.log(`Servidor rodando em http://0.0.0.0:${PORT}`);
    logEvent('info', 'system', 'server_start', `Servidor iniciado em http://0.0.0.0:${PORT}`);
    
    // Inicializar Google Drive se configurado
    try {
        const driveAutenticado = await googleDriveService.autenticar();
        if (driveAutenticado) {
            const quota = await googleDriveService.verificarQuota();
            if (quota) {
                console.log(`📊 Google Drive: ${quota.usadoGB}GB de ${quota.limiteGB}GB usados (${quota.percentual}%)`);
                
                // Verificar se precisa de backup automático
                if (quota.precisaBackup) {
                    console.log('⚠️  ATENÇÃO: Drive próximo do limite! Configure EMAIL_BACKUP no .env para backup automático.');
                }
            }
        }
    } catch (error) {
        console.log('ℹ️  Google Drive não configurado. Sistema funcionará com SQLite local.');
        console.log('📖 Para usar Google Drive, veja: GOOGLE_DRIVE_SETUP.md');
    }
});

