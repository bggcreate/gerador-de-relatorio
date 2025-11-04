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
const { requireRole, requirePage, getLojaFilter, getPermissions, ROLES } = require('./middleware/roleAuth');

const app = express();
const PORT = process.env.PORT || 5000;

app.set('trust proxy', true);

const SESSION_SECRET = process.env.SESSION_SECRET || crypto.randomBytes(64).toString('hex');
if (!process.env.SESSION_SECRET) {
    console.warn('⚠️  ATENÇÃO: SESSION_SECRET não configurado. Usando um secret gerado automaticamente.');
    console.warn('⚠️  Para produção, configure a variável de ambiente SESSION_SECRET.');
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
            styleSrc: ["'self'", "'unsafe-inline'", "https://cdn.jsdelivr.net"],
            imgSrc: ["'self'", "data:", "https:"],
            connectSrc: ["'self'"],
            fontSrc: ["'self'", "https://cdn.jsdelivr.net"],
            objectSrc: ["'none'"],
            mediaSrc: ["'self'"],
            frameSrc: ["'none'"],
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
    const privilegedRoles = ['admin', 'dev'];
    const mutatingMethods = ['POST', 'PUT', 'DELETE'];
    
    if (req.session && req.session.username && privilegedRoles.includes(req.session.role)) {
        if (mutatingMethods.includes(req.method) && !req.path.includes('/api/login')) {
            const action = `${req.method} ${req.path}`;
            const details = `Ação privilegiada executada por ${req.session.role}`;
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
const requireAdmin = (req, res, next) => {
    if (req.session && req.session.role === 'admin') {
        return next();
    }
    res.status(403).json({ error: 'Acesso negado.' });
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

// --- ROTAS DE PÁGINAS ---
app.get('/login', (req, res) => res.sendFile(path.join(__dirname, 'views', 'login.html')));
app.get('/403', (req, res) => res.sendFile(path.join(__dirname, 'views', '403.html')));
app.get('/live', requirePageLogin, (req, res) => res.sendFile(path.join(__dirname, 'views', 'live.html')));

// Dashboard - todos podem acessar (mas dados filtrados), exceto técnico
app.get(['/', '/admin'], requirePageLogin, (req, res) => {
    // Técnicos devem ir para a página de alertas
    if (req.session.role === 'tecnico') {
        return res.redirect('/alertas-tecnico');
    }
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Consulta - todos podem acessar
app.get('/consulta', requirePageLogin, requirePage(['consulta']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Novo Relatório - apenas monitoramento, admin e dev
app.get('/novo-relatorio', requirePageLogin, requirePage(['novo-relatorio']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Lojas - todos exceto consultor (mas consultor pode via demandas)
app.get('/gerenciar-lojas', requirePageLogin, requirePage(['lojas']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Demandas - todos podem acessar
app.get('/demandas', requirePageLogin, requirePage(['demandas']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Gerenciar Usuários - apenas admin e dev
app.get('/gerenciar-usuarios', requirePageLogin, requirePage(['gerenciar-usuarios']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Logs - apenas dev
app.get('/logs', requirePageLogin, requirePage(['logs']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// Backup - apenas admin e dev
app.get('/backup', requirePageLogin, requirePage(['backup']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

app.get('/dev/system', requirePageLogin, requireRole([ROLES.DEV]), (req, res) => {
    logEvent('dev_access', req.session.username, 'system_access', 'Desenvolvedor acessou painel de sistema', req);
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

app.get('/content/:page', requirePageLogin, (req, res) => {
    const allowedPages = ['admin', 'consulta', 'demandas', 'gerenciar-lojas', 'assistencia', 'alertas-tecnico', 'novo-relatorio', 'gerenciar-usuarios', 'logs'];
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
            
            // Mapeia os valores
            const vendas_loja = Math.round(parseBrazilianNumber(valores[1])); // Total de Vendas
            const pa = parseBrazilianNumber(valores[2]); // Peças/Venda
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
            const valoresTotais = linhaLimpa.match(/(\d{1,3}(?:\.\d{3})*,\d{2})|(\d+\.\d{2})|(\d+)/g);
            
            if (!valoresTotais || valoresTotais.length < 7) {
                throw new Error("Não foi possível extrair os valores corretamente da linha Totais do PDF.");
            }
            
            const totalVendasValor = parseBrazilianNumber(valoresTotais[0]);
            const pa = parseBrazilianNumber(valoresTotais[valoresTotais.length - 4]);
            const ticketMedio = parseBrazilianNumber(valoresTotais[valoresTotais.length - 3]);
            
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
            req.session.role = user.role;
            req.session.loja_gerente = user.loja_gerente;
            req.session.lojas_consultor = user.lojas_consultor;
            req.session.loja_tecnico = user.loja_tecnico;
            req.session.custom_permissions = user.custom_permissions;
            
            req.session.save((err) => {
                if (err) {
                    console.error('❌ Erro ao salvar sessão:', err);
                    logEvent('error', username, 'login_error', `Erro ao salvar sessão: ${err.message}`, req);
                    return res.status(500).json({ message: 'Erro ao salvar sessão.' });
                }
                console.log(`✓ Login bem-sucedido - Usuário: ${user.username}, Role: ${user.role}, Session ID: ${req.sessionID}`);
                logEvent('access', user.username, 'login_success', `Usuário ${user.username} (${user.role}) fez login com sucesso`, req);
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
    const role = req.session?.role;
    if (username) {
        logEvent('access', username, 'logout', `Usuário ${username} (${role}) fez logout`, req);
    }
    req.session.destroy(() => res.redirect('/login')); 
});
app.get('/api/session-info', requirePageLogin, (req, res) => { 
    // Buscar permissões customizadas do usuário
    db.get('SELECT custom_permissions FROM usuarios WHERE id = ?', [req.session.userId], (err, user) => {
        const customPerms = user?.custom_permissions || req.session.custom_permissions;
        const permissions = getPermissions(req.session.role, customPerms);
        res.json({ 
            id: req.session.userId, 
            username: req.session.username, 
            role: req.session.role,
            permissions: permissions
        });
    });
});
app.get('/api/usuarios', requirePageLogin, requireRole(['admin', 'dev']), (req, res) => { 
    db.all("SELECT id, username, role, loja_gerente, lojas_consultor, loja_tecnico, custom_permissions FROM usuarios ORDER BY username", (err, users) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        res.json(users || []); 
    }); 
});
app.post('/api/usuarios', requirePageLogin, requireRole(['admin', 'dev']), validateCsrf, async (req, res) => { 
    const { username, password, role, loja_gerente, lojas_consultor, loja_tecnico, custom_permissions } = req.body; 
    if (!username || !password || !role) return res.status(400).json({ error: 'Username, senha e cargo são obrigatórios.' }); 
    
    if (req.session.role === 'admin' && role === 'dev') {
        return res.status(403).json({ error: 'Apenas desenvolvedores podem criar usuários com cargo Dev.' });
    }
    
    try {
        const hashedPassword = await bcrypt.hash(password, 10);
        const lojas_consultor_str = Array.isArray(lojas_consultor) ? lojas_consultor.join(',') : (lojas_consultor || '');
        const custom_permissions_str = custom_permissions ? (typeof custom_permissions === 'string' ? custom_permissions : JSON.stringify(custom_permissions)) : null;
        
        db.run('INSERT INTO usuarios (username, password, role, loja_gerente, lojas_consultor, loja_tecnico, custom_permissions, password_hashed) VALUES (?, ?, ?, ?, ?, ?, ?, 1)', 
            [username, hashedPassword, role, loja_gerente || null, lojas_consultor_str, loja_tecnico || null, custom_permissions_str], 
            function (err) { 
                if (err) {
                    logEvent('error', req.session.username, 'user_creation_failed', `Erro ao criar usuário ${username}: ${err.message}`, req);
                    return res.status(500).json({ error: 'Erro ao criar usuário. O nome de usuário já pode existir.' }); 
                }
                logEvent('admin', req.session.username, 'user_created', `Usuário ${username} criado com cargo ${role}`, req);
                res.status(201).json({ success: true, id: this.lastID }); 
            }
        );
    } catch (error) {
        logEvent('error', req.session.username, 'user_creation_error', `Erro ao hash senha: ${error.message}`, req);
        res.status(500).json({ error: 'Erro ao processar senha.' });
    }
});
app.put('/api/usuarios/:id', requirePageLogin, requireRole(['admin', 'dev']), validateCsrf, async (req, res) => { 
    const { id } = req.params; 
    const { username, password, role, loja_gerente, lojas_consultor, loja_tecnico, custom_permissions } = req.body; 
    if (!username || !role) return res.status(400).json({ error: 'Username e Cargo são obrigatórios.' }); 
    
    if (req.session.role === 'admin' && role === 'dev') {
        return res.status(403).json({ error: 'Apenas desenvolvedores podem criar/alterar usuários com cargo Dev.' });
    }
    
    try {
        const lojas_consultor_str = Array.isArray(lojas_consultor) ? lojas_consultor.join(',') : (lojas_consultor || '');
        const custom_permissions_str = custom_permissions ? (typeof custom_permissions === 'string' ? custom_permissions : JSON.stringify(custom_permissions)) : null;
        let sql, params;
        
        if (password) {
            const hashedPassword = await bcrypt.hash(password, 10);
            sql = 'UPDATE usuarios SET username = ?, password = ?, password_hashed = 1, role = ?, loja_gerente = ?, lojas_consultor = ?, loja_tecnico = ?, custom_permissions = ? WHERE id = ?';
            params = [username, hashedPassword, role, loja_gerente || null, lojas_consultor_str, loja_tecnico || null, custom_permissions_str, id];
        } else {
            sql = 'UPDATE usuarios SET username = ?, role = ?, loja_gerente = ?, lojas_consultor = ?, loja_tecnico = ?, custom_permissions = ? WHERE id = ?';
            params = [username, role, loja_gerente || null, lojas_consultor_str, loja_tecnico || null, custom_permissions_str, id];
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
app.delete('/api/usuarios/:id', requirePageLogin, requireRole(['admin', 'dev']), validateCsrf, (req, res) => { 
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
    const lojaFilter = getLojaFilter(req.session.role, req.session.loja_gerente, req.session.lojas_consultor, req.session.loja_tecnico);
    console.log('API /api/lojas - User:', req.session.username, 'Role:', req.session.role, 'Filter:', lojaFilter);
    console.log('Lojas consultor:', req.session.lojas_consultor);
    if (lojaFilter) {
        whereClauses.push(lojaFilter.clause.replace('TRIM(loja)', 'TRIM(nome)'));
        params.push(...lojaFilter.params);
    }
    
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

app.post('/api/vendedores', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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

app.put('/api/vendedores/:id', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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

app.delete('/api/vendedores/:id', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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

// APIs DE RELATÓRIOS
const processarRelatorio = (r) => { if (!r) return null; const vendas_monitoramento_total = (parseInt(r.vendas_monitoramento, 10) || 0) + (parseInt(r.quantidade_omni, 10) || 0); const tx_conversao_monitoramento = (parseInt(r.clientes_monitoramento, 10) || 0) > 0 ? (vendas_monitoramento_total / r.clientes_monitoramento) * 100 : 0; const tx_conversao_loja = (parseInt(r.clientes_loja, 10) || 0) > 0 ? ((parseInt(r.vendas_loja, 10) || 0) / r.clientes_loja) * 100 : 0; let vendedores_processados = []; try { const vendedores = JSON.parse(r.vendedores || '[]'); vendedores_processados = vendedores.map(v => ({ ...v, tx_conversao: (v.atendimentos > 0 ? ((v.vendas / v.atendimentos) * 100) : 0).toFixed(2) })); } catch (e) {} return { ...r, vendas_monitoramento_total, tx_conversao_monitoramento: tx_conversao_monitoramento.toFixed(2), tx_conversao_loja: tx_conversao_loja.toFixed(2), vendedores_processados }; };
app.get('/api/relatorios', requirePageLogin, (req, res) => { 
    const whereClauses = []; 
    const params = []; 
    
    // Aplicar filtro de lojas baseado no role
    const lojaFilter = getLojaFilter(req.session.role, req.session.loja_gerente, req.session.lojas_consultor, req.session.loja_tecnico);
    console.log('API /api/relatorios - User:', req.session.username, 'Role:', req.session.role, 'Filter:', lojaFilter);
    console.log('Lojas consultor:', req.session.lojas_consultor);
    if (lojaFilter) {
        whereClauses.push(lojaFilter.clause);
        params.push(...lojaFilter.params);
    }
    
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
const formatCurrency = (value) => { const numberValue = Number(value) || 0; return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(numberValue); };
const formatarRelatorioTexto = (r) => { const rp = processarRelatorio(r); if (!rp) return "Erro ao processar relatório."; let equipeInfo = 'Nenhum vendedor registrado.\n'; if (rp.vendedores_processados && rp.vendedores_processados.length > 0) { equipeInfo = rp.vendedores_processados.map(v => { return `${v.nome}: ${v.atendimentos} Atendimentos / ${v.vendas} Vendas / ${v.tx_conversao}%`; }).join('\n'); } let funcaoEspecialInfo = ''; if (rp.funcao_especial === "Omni") { funcaoEspecialInfo = `Omni: ${rp.quantidade_omni || 0}\n`; } else if (rp.funcao_especial === "Busca por Assist. Tec.") { funcaoEspecialInfo = `Busca por assist tec: ${rp.quantidade_funcao_especial || 0}\n`; } const totalVendasQuantidade = (rp.vendas_cartao || 0) + (rp.vendas_pix || 0) + (rp.vendas_dinheiro || 0); const content = ` DATA: ${new Date(rp.data).toLocaleDateString('pt-BR', { timeZone: 'UTC' })} \n\nClientes: ${rp.clientes_monitoramento || 0}\nBluve: ${rp.clientes_loja || 0}\nVendas / Monitoramento: ${rp.vendas_monitoramento_total || 0}\nVendas / Loja: ${rp.vendas_loja || 0}\nTaxa de conversão da loja: ${rp.tx_conversao_loja || '0.00'}%\nTaxa de conversão do monitoramento: ${rp.tx_conversao_monitoramento || '0.00'}%\n\nAbertura: ${rp.hora_abertura || '--:--'} - ${rp.hora_fechamento || '--:--'}\nGerente: ${rp.gerente_entrada || '--:--'} - ${rp.gerente_saida || '--:--'}\nVendas em Cartão: ${rp.vendas_cartao || 0}\nVendas em Pix: ${rp.vendas_pix || 0}\nVendas em Dinheiro: ${rp.vendas_dinheiro || 0}\n${funcaoEspecialInfo}Total vendas: ${totalVendasQuantidade}\nTroca/Devolução: ${rp.quantidade_trocas || 0}\n\nDesempenho Equipe:\n\n${equipeInfo}\n\nTM: ${rp.ticket_medio || 'R$ 0,00'} / P.A: ${rp.pa || '0.00'} / Total: ${formatCurrency(rp.total_vendas_dinheiro)} / `; return content.trim(); };
app.get('/api/relatorios/:id/txt', requirePageLogin, (req, res) => { const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = ? `; db.get(sql, [req.params.id], (err, r) => { if (err || !r) return res.status(404).send('Relatório não encontrado'); res.setHeader('Content-disposition', `attachment; filename=relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.txt`); res.setHeader('Content-type', 'text/plain; charset=utf-8'); res.send(formatarRelatorioTexto(r)); }); });
app.get('/api/relatorios/:id/pdf', requirePageLogin, (req, res) => { const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = ? `; db.get(sql, [req.params.id], (err, r) => { if (err || !r) return res.status(404).send('Relatório não encontrado'); const doc = new PDFDocument({ margin: 50, size: 'A4' }); res.setHeader('Content-disposition', `inline; filename="relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.pdf"`); res.setHeader('Content-type', 'application/pdf'); doc.pipe(res); doc.fontSize(18).font('Helvetica-Bold').text(r.loja.toUpperCase(), { align: 'center' }).moveDown(1); doc.fontSize(11).font('Helvetica').text(formatarRelatorioTexto(r), { align: 'left' }); doc.end(); }); });


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
    const lojaFilter = getLojaFilter(req.session.role, req.session.loja_gerente, req.session.lojas_consultor, req.session.loja_tecnico);
    if (lojaFilter) {
        whereClauses.push(lojaFilter.clause);
        params.push(...lojaFilter.params);
    }
    
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
    
    const lojaFilter = getLojaFilter(req.session.role, req.session.loja_gerente, req.session.lojas_consultor, req.session.loja_tecnico);
    if (lojaFilter) {
        whereClauses.push(lojaFilter.clause);
        params.push(...lojaFilter.params);
    }
    
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
    const lojaFilter = getLojaFilter(req.session.role, req.session.loja_gerente, req.session.lojas_consultor, req.session.loja_tecnico);
    if (lojaFilter) {
        whereClauses.push(lojaFilter.clause.replace('TRIM(loja)', 'TRIM(l.nome)'));
        params.push(...lojaFilter.params);
    }
    
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
        
        // Verificar se é gerente
        const isGerente = req.session.role === 'gerente';
        
        const ranking = rows.map(r => { 
            const vendas_m_total = (r.total_vendas_monitoramento || 0) + (r.total_omni || 0); 
            const item = { 
                ...r, 
                tx_loja: (r.total_clientes_loja > 0 ? (r.total_vendas_loja / r.total_clientes_loja) * 100 : 0), 
                tx_monitoramento: (r.total_clientes_monitoramento > 0 ? (vendas_m_total / r.total_clientes_monitoramento) * 100 : 0) 
            };
            
            // Remover campos de monitoramento se for gerente
            if (isGerente) {
                delete item.total_clientes_monitoramento;
                delete item.total_vendas_monitoramento;
                delete item.total_omni;
                delete item.tx_monitoramento;
            }
            
            return item;
        }); 
        res.json(ranking); 
    }); 
});
app.get('/api/dashboard/chart-data', requirePageLogin, (req, res) => {
    const { loja, data_inicio, data_fim } = req.query;
    let whereClauses = [];
    let params = [];
    
    // Aplicar filtro de lojas baseado no role
    const lojaFilter = getLojaFilter(req.session.role, req.session.loja_gerente, req.session.lojas_consultor, req.session.loja_tecnico);
    if (lojaFilter) {
        whereClauses.push(lojaFilter.clause);
        params.push(...lojaFilter.params);
    }
    
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
        
        // Verificar se é gerente
        const isGerente = req.session.role === 'gerente';
        
        const labels = [];
        const txConversaoLoja = [];
        const txConversaoMonitoramento = [];
        
        rows.forEach(row => {
            labels.push(new Date(row.data).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', timeZone: 'UTC' }));
            const tx_l = row.total_clientes_loja > 0 ? (row.total_vendas_loja / row.total_clientes_loja) * 100 : 0;
            txConversaoLoja.push(tx_l.toFixed(2));
            
            // Só calcular monitoramento se não for gerente
            if (!isGerente) {
                const vendas_m_total = (row.total_vendas_monitoramento || 0) + (row.total_omni || 0);
                const tx_m = row.total_clientes_monitoramento > 0 ? (vendas_m_total / row.total_clientes_monitoramento) * 100 : 0;
                txConversaoMonitoramento.push(tx_m.toFixed(2));
            }
        });
        
        // Remover txConversaoMonitoramento da resposta se for gerente
        const response = { labels, txConversaoLoja };
        if (!isGerente) {
            response.txConversaoMonitoramento = txConversaoMonitoramento;
        }
        
        res.json(response);
    });
});
app.post('/api/demandas', requirePageLogin, (req, res) => { const { loja_nome, descricao, tag } = req.body; db.run('INSERT INTO demandas (loja_nome, descricao, tag, criado_por_usuario) VALUES (?, ?, ?, ?)', [loja_nome, descricao, tag, req.session.username], function (err) { if (err) return res.status(500).json({ error: 'Falha ao salvar demanda.' }); res.status(201).json({ success: true, id: this.lastID }); }); });
app.get('/api/demandas/:status', requirePageLogin, (req, res) => { 
    const status = req.params.status === 'pendentes' ? 'pendente' : 'concluido'; 
    const whereClauses = ['status = ?'];
    const params = [status];
    
    // Aplicar filtro de lojas baseado no role
    const lojaFilter = getLojaFilter(req.session.role, req.session.loja_gerente, req.session.lojas_consultor, req.session.loja_tecnico);
    if (lojaFilter) {
        whereClauses.push(lojaFilter.clause.replace('TRIM(loja)', 'TRIM(loja_nome)'));
        params.push(...lojaFilter.params);
    }
    
    const whereString = whereClauses.join(' AND ');
    const query = `SELECT * FROM demandas WHERE ${whereString} ORDER BY criado_em DESC`;
    
    db.all(query, params, (err, demandas) => { 
        if (err) return res.status(500).json({ error: err.message }); 
        res.json(demandas || []); 
    }); 
});
app.put('/api/demandas/:id/concluir', requirePageLogin, (req, res) => { db.run("UPDATE demandas SET status = 'concluido', concluido_por_usuario = ?, concluido_em = CURRENT_TIMESTAMP WHERE id = ?", [req.session.username, req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao concluir demanda.' }); if (this.changes === 0) return res.status(404).json({ error: 'Demanda não encontrada.' }); res.json({ success: true }); }); });
app.delete('/api/demandas/:id', requirePageLogin, requireRole(['admin', 'dev', 'monitoramento', 'consultor']), (req, res) => { db.run("DELETE FROM demandas WHERE id = ?", [req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao excluir demanda.' }); if (this.changes === 0) return res.status(404).json({ error: "Demanda não encontrada." }); res.json({ success: true }); }); });
app.get('/api/backup/info', requirePageLogin, requireAdmin, (req, res) => {
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
app.delete('/api/backup/clear', requirePageLogin, requireAdmin, (req, res) => {
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
app.get('/api/backup/download', requirePageLogin, requireRole(['admin', 'dev']), (req, res) => { const date = new Date().toISOString().slice(0, 10); const fileName = `backup_reports_${date}.db`; res.download(DB_PATH, fileName, (err) => { if (err && !res.headersSent) { res.status(500).send("Não foi possível baixar o arquivo de backup."); } }); });
app.post('/api/backup/restore', requirePageLogin, requireRole(['admin', 'dev']), upload.single('backupFile'), (req, res) => { if (!req.file) { return res.status(400).json({ error: "Nenhum arquivo de backup foi enviado." }); } const backupBuffer = req.file.buffer; db.close((err) => { if (err) { console.error("Erro ao fechar o DB antes de restaurar:", err.message); return res.status(500).json({ error: "Não foi possível fechar a conexão com o banco de dados atual." }); } fs.writeFile(DB_PATH, backupBuffer, (err) => { if (err) { console.error("Falha ao escrever o arquivo de backup:", err.message); db = new sqlite3.Database(DB_PATH); return res.status(500).json({ error: "Falha ao substituir o arquivo de banco de dados." }); } db = new sqlite3.Database(DB_PATH, (err) => { if (err) { console.error("DB restaurado, mas falha ao reconectar:", err.message); return res.status(500).json({ error: "Banco de dados restaurado, mas falha ao reconectar. Reinicie o servidor." }); } console.log("Banco de dados restaurado e reconectado com sucesso."); res.json({ success: true, message: "Banco de dados restaurado com sucesso. A página será recarregada." }); }); }); }); });

// APIs DE LOGS (apenas para Dev)
app.get('/api/logs', requirePageLogin, requireRole(['dev']), (req, res) => {
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

app.delete('/api/logs', requirePageLogin, requireRole(['dev']), (req, res) => {
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
app.get('/assistencia', requirePageLogin, requirePage(['assistencia']), (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'assistencia.html'));
});

// Página de Alertas para Técnicos (via SPA)
app.get('/alertas-tecnico', requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});

// API - Listar Estoque Técnico
app.get('/api/estoque-tecnico', requirePageLogin, requireRole(['tecnico', 'gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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
app.post('/api/estoque-tecnico', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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
app.put('/api/estoque-tecnico/:id', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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
app.delete('/api/estoque-tecnico/:id', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
    const { id } = req.params;
    
    db.run('DELETE FROM estoque_tecnico WHERE id = ?', [id], function(err) {
        if (err) return res.status(500).json({ error: err.message });
        if (this.changes === 0) return res.status(404).json({ error: 'Peça não encontrada.' });
        logEvent('info', req.session.username, 'estoque_delete', `Peça removida: ID ${id}`);
        res.json({ success: true });
    });
});

// API - Listar Assistências
app.get('/api/assistencias', requirePageLogin, requireRole(['tecnico', 'gerente', 'consultor', 'monitoramento', 'admin', 'dev']), (req, res) => {
    const status = req.query.status;
    const search = req.query.search || '';
    const limit = req.query.limit ? parseInt(req.query.limit) : null;
    const lojaFilter = req.query.loja || '';
    
    let whereClauses = [];
    let params = [];
    
    // Filtrar por loja baseado no cargo
    if (req.session.role === 'tecnico' && req.session.loja_tecnico) {
        // Técnico vê apenas sua loja
        whereClauses.push('TRIM(loja) = ?');
        params.push(req.session.loja_tecnico.trim());
    } else if (req.session.role === 'gerente' && req.session.loja_gerente) {
        // Gerente vê apenas sua loja
        whereClauses.push('TRIM(loja) = ?');
        params.push(req.session.loja_gerente.trim());
    } else if (req.session.role === 'consultor' && req.session.lojas_consultor) {
        // Consultor vê lojas que administra
        try {
            const lojasArray = JSON.parse(req.session.lojas_consultor);
            if (lojasArray && lojasArray.length > 0) {
                const placeholders = lojasArray.map(() => 'TRIM(loja) = ?').join(' OR ');
                whereClauses.push(`(${placeholders})`);
                params.push(...lojasArray.map(l => l.trim()));
            }
        } catch (e) {
            // Se lojas_consultor não for um JSON válido, não adiciona filtro
        }
    }
    
    // Filtro opcional por loja específica (para cargos que veem múltiplas lojas)
    if (lojaFilter && ['consultor', 'admin', 'dev', 'monitoramento'].includes(req.session.role)) {
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
app.post('/api/assistencias', requirePageLogin, requireRole(['consultor', 'gerente', 'admin', 'dev']), (req, res) => {
    let {
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
    
    // Se for gerente, forçar loja do gerente
    if (req.session.role === 'gerente' && req.session.loja_gerente) {
        loja = req.session.loja_gerente;
    }
    
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
app.put('/api/assistencias/:id', requirePageLogin, requireRole(['consultor', 'gerente', 'admin', 'dev']), (req, res) => {
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
app.post('/api/assistencias/:id/concluir', requirePageLogin, requireRole(['consultor', 'gerente', 'admin', 'dev']), (req, res) => {
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
app.get('/api/assistencias/historico', requirePageLogin, requireRole(['tecnico', 'gerente', 'consultor', 'monitoramento', 'admin', 'dev']), (req, res) => {
    const search = req.query.search || '';
    const lojaFilter = req.query.loja || '';
    
    let whereClauses = ["status = 'Concluído'"];
    let params = [];
    
    // Filtrar por loja baseado no cargo
    if (req.session.role === 'tecnico' && req.session.loja_tecnico) {
        // Técnico vê apenas sua loja
        whereClauses.push('TRIM(loja) = ?');
        params.push(req.session.loja_tecnico.trim());
    } else if (req.session.role === 'gerente' && req.session.loja_gerente) {
        // Gerente vê apenas sua loja
        whereClauses.push('TRIM(loja) = ?');
        params.push(req.session.loja_gerente.trim());
    } else if (req.session.role === 'consultor' && req.session.lojas_consultor) {
        // Consultor vê lojas que administra
        try {
            const lojasArray = JSON.parse(req.session.lojas_consultor);
            if (lojasArray && lojasArray.length > 0) {
                const placeholders = lojasArray.map(() => 'TRIM(loja) = ?').join(' OR ');
                whereClauses.push(`(${placeholders})`);
                params.push(...lojasArray.map(l => l.trim()));
            }
        } catch (e) {
            // Se lojas_consultor não for um JSON válido, não adiciona filtro
        }
    }
    
    // Filtro opcional por loja específica (para cargos que veem múltiplas lojas)
    if (lojaFilter && ['consultor', 'admin', 'dev', 'monitoramento'].includes(req.session.role)) {
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
app.delete('/api/assistencias/:id', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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
app.get('/api/assistencias/stats-tecnico', requirePageLogin, requireRole(['tecnico']), (req, res) => {
    const lojaTecnico = req.session.loja_tecnico;
    
    if (!lojaTecnico) {
        return res.status(400).json({ error: 'Técnico sem loja atribuída' });
    }
    
    const hoje = new Date().toISOString().split('T')[0];
    const mesAtual = hoje.substring(0, 7);
    
    // Em andamento
    db.get(`
        SELECT COUNT(*) as total 
        FROM assistencias 
        WHERE TRIM(loja) = ? AND status = 'Em andamento'
    `, [lojaTecnico.trim()], (err, emAndamento) => {
        if (err) return res.status(500).json({ error: err.message });
        
        // Concluídas hoje
        db.get(`
            SELECT COUNT(*) as total 
            FROM assistencias 
            WHERE TRIM(loja) = ? AND status = 'Concluído' AND DATE(data_saida) = ?
        `, [lojaTecnico.trim(), hoje], (err2, concluidasHoje) => {
            if (err2) return res.status(500).json({ error: err2.message });
            
            // Total do mês
            db.get(`
                SELECT COUNT(*) as total 
                FROM assistencias 
                WHERE TRIM(loja) = ? AND strftime('%Y-%m', data_entrada) = ?
            `, [lojaTecnico.trim(), mesAtual], (err3, totalMes) => {
                if (err3) return res.status(500).json({ error: err3.message });
                
                // Aguardando peças
                db.get(`
                    SELECT COUNT(*) as total 
                    FROM assistencias 
                    WHERE TRIM(loja) = ? AND status = 'Aguardando peças'
                `, [lojaTecnico.trim()], (err4, aguardandoPecas) => {
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

app.get('/api/assistencias/stats', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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
app.get('/api/assistencias/por-loja', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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
app.get('/api/assistencias/stats-daily', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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
app.get('/api/assistencias/tickets', requirePageLogin, requireRole(['gerente', 'consultor', 'admin', 'dev']), (req, res) => {
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

app.listen(PORT, '0.0.0.0', () => {
    console.log(`Servidor rodando em http://0.0.0.0:${PORT}`);
    logEvent('info', 'system', 'server_start', `Servidor iniciado em http://0.0.0.0:${PORT}`);
});

