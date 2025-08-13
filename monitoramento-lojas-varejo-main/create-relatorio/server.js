// =================================================================
// DEPENDÊNCIAS E CONFIGURAÇÕES INICIAIS
// =================================================================
const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const path = require('path');
const session = require('express-session');
const fs = require('fs');
const PDFDocument = require('pdfkit');
const ExcelJS = require('exceljs');
const multer = require('multer');
const pdf = require('pdf-parse');

const app = express();
const PORT = 3000;

// --- CONFIGURAÇÃO GERAL ---
const dataDir = path.join(__dirname, 'data');
if (!fs.existsSync(dataDir)) {
    fs.mkdirSync(dataDir, { recursive: true });
}
const DB_PATH = path.join(dataDir, 'relatorios.db');
app.use(express.static(path.join(__dirname, 'public')));
app.use(express.urlencoded({ extended: true }));
app.use(express.json());
app.use(session({
    secret: 'chave-definitiva-123',
    resave: false,
    saveUninitialized: false,
    cookie: { httpOnly: true, maxAge: 24 * 60 * 60 * 1000 }
}));

// --- CONFIGURAÇÃO DO MULTER ---
const upload = multer({ storage: multer.memoryStorage() });

// --- MIDDLEWARES ---
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
        db.run(`CREATE TABLE IF NOT EXISTS usuarios (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE NOT NULL, password TEXT NOT NULL, role TEXT NOT NULL)`);
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
app.get('/live', requirePageLogin, (req, res) => res.sendFile(path.join(__dirname, 'views', 'live.html')));
app.get(['/', '/admin', '/consulta', '/demandas', '/gerenciar-lojas', '/novo-relatorio', '/gerenciar-usuarios'], requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});
app.get('/content/:page', requirePageLogin, (req, res) => {
    const allowedPages = ['admin', 'consulta', 'demandas', 'gerenciar-lojas', 'novo-relatorio', 'gerenciar-usuarios'];
    if (allowedPages.includes(req.params.page)) {
        res.sendFile(path.join(__dirname, 'views', `${req.params.page}.html`));
    } else {
        res.status(404).send('Página não encontrada');
    }
});


// --- ROTAS DE API ---

// <<<---------------------------------------------------->>>
// <<<        INÍCIO DA API DE PDF ATUALIZADA             >>>
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

        // VERIFICA O TIPO DE PDF PELA PRESENÇA DO TÍTULO "Desempenho de vendedores"
        if (text.includes("Desempenho de vendedores")) {
            // --- NOVA LÓGICA PARA PDF TIPO OMNI ---
            console.log("Processando PDF estilo Omni (Desempenho de vendedores)...");

            const linhaTotais = lines.find(l => l.startsWith('Totais:'));
            if (!linhaTotais) {
                throw new Error("Linha 'Totais:' não encontrada no PDF.");
            }

            // Extrai todos os "números" da linha de totais
            const valores = linhaTotais.replace('Totais:', '').trim().split(/\s+/);
            
            // Mapeia os valores com base na imagem de exemplo
            const vendas_loja = Math.round(parseBrazilianNumber(valores[1])); // Total de Vendas
            const pa = parseBrazilianNumber(valores[2]); // Peças/Venda
            const total_vendas_dinheiro = parseBrazilianNumber(valores[3]); // Vl. Vendas
            const ticket_medio = parseBrazilianNumber(valores[4]); // Ticket Médio
            const clientes_loja = parseInt(valores[5], 10); // Abordagens

            // Extrai dados comuns (nome da loja, data, vendedores)
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
            // --- LÓGICA ANTIGA PARA PDF TIPO BUSCA TÉCNICA ---
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
// <<<          FIM DA API DE PDF ATUALIZADA              >>>
// <<<---------------------------------------------------->>>


// APIs DE SESSÃO E USUÁRIOS (Sem alterações)
app.post('/api/login', (req, res) => { const { username, password } = req.body; db.get('SELECT * FROM usuarios WHERE username = ? AND password = ?', [username, password], (err, user) => { if (err || !user) return res.status(401).json({ message: 'Credenciais inválidas.' }); req.session.userId = user.id; req.session.username = user.username; req.session.role = user.role; res.json({ success: true }); }); });
app.get('/logout', (req, res) => { req.session.destroy(() => res.redirect('/login')); });
app.get('/api/session-info', requirePageLogin, (req, res) => { res.json({ id: req.session.userId, username: req.session.username, role: req.session.role }); });
app.get('/api/usuarios', requirePageLogin, requireAdmin, (req, res) => { db.all("SELECT id, username, role FROM usuarios ORDER BY username", (err, users) => { if (err) return res.status(500).json({ error: err.message }); res.json(users || []); }); });
app.post('/api/usuarios', requirePageLogin, requireAdmin, (req, res) => { const { username, password, role } = req.body; if (!username || !password || !role) return res.status(400).json({ error: 'Todos os campos são obrigatórios.' }); db.run('INSERT INTO usuarios (username, password, role) VALUES (?, ?, ?)', [username, password, role], function (err) { if (err) return res.status(500).json({ error: 'Erro ao criar usuário. O nome de usuário já pode existir.' }); res.status(201).json({ success: true, id: this.lastID }); }); });
app.put('/api/usuarios/:id', requirePageLogin, requireAdmin, (req, res) => { const { id } = req.params; const { username, password, role } = req.body; if (!username || !role) return res.status(400).json({ error: 'Username e Cargo são obrigatórios.' }); const sql = password ? 'UPDATE usuarios SET username = ?, password = ?, role = ? WHERE id = ?' : 'UPDATE usuarios SET username = ?, role = ? WHERE id = ?'; const params = password ? [username, password, role, id] : [username, role, id]; db.run(sql, params, function (err) { if (err) return res.status(500).json({ error: 'Erro ao atualizar usuário.' }); res.json({ success: true }); }); });
app.delete('/api/usuarios/:id', requirePageLogin, requireAdmin, (req, res) => { const { id } = req.params; if (id == req.session.userId) return res.status(403).json({ error: 'Não é permitido excluir o próprio usuário logado.' }); db.run("DELETE FROM usuarios WHERE id = ?", [id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao excluir usuário.' }); if (this.changes === 0) return res.status(404).json({ error: "Usuário não encontrado." }); res.json({ success: true }); }); });

// APIs DE LOJAS (Sem alterações)
app.get('/api/lojas', requirePageLogin, (req, res) => { let query = "SELECT * FROM lojas"; const params = []; if (req.query.status) { query += " WHERE status = ?"; params.push(req.query.status); } query += " ORDER BY nome"; db.all(query, params, (err, lojas) => { if (err) return res.status(500).json({ error: err.message }); res.json(lojas || []); }); });
app.post('/api/lojas', requirePageLogin, (req, res) => { const { nome, status, funcao_especial, observacoes } = req.body; db.run('INSERT INTO lojas (nome, status, funcao_especial, observacoes) VALUES (?, ?, ?, ?)', [nome, status, funcao_especial, observacoes], function (err) { if (err) return res.status(500).json({ error: 'Erro ao criar loja. O nome já pode existir.' }); res.status(201).json({ success: true, id: this.lastID }); }); });
app.put('/api/lojas/:id', requirePageLogin, (req, res) => { const { id } = req.params; const { nome, status, funcao_especial, observacoes } = req.body; db.run('UPDATE lojas SET nome = ?, status = ?, funcao_especial = ?, observacoes = ? WHERE id = ?', [nome, status, funcao_especial, observacoes, id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao atualizar loja.' }); res.json({ success: true }); }); });
app.delete('/api/lojas/:id', requirePageLogin, (req, res) => { db.run("DELETE FROM lojas WHERE id = ?", [req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao excluir loja.' }); if (this.changes === 0) return res.status(404).json({ error: "Loja não encontrada." }); res.json({ success: true }); }); });

// APIs DE RELATÓRIOS (Sem alterações)
const processarRelatorio = (r) => { if (!r) return null; const vendas_monitoramento_total = (parseInt(r.vendas_monitoramento, 10) || 0) + (parseInt(r.quantidade_omni, 10) || 0); const tx_conversao_monitoramento = (parseInt(r.clientes_monitoramento, 10) || 0) > 0 ? (vendas_monitoramento_total / r.clientes_monitoramento) * 100 : 0; const tx_conversao_loja = (parseInt(r.clientes_loja, 10) || 0) > 0 ? ((parseInt(r.vendas_loja, 10) || 0) / r.clientes_loja) * 100 : 0; let vendedores_processados = []; try { const vendedores = JSON.parse(r.vendedores || '[]'); vendedores_processados = vendedores.map(v => ({ ...v, tx_conversao: (v.atendimentos > 0 ? ((v.vendas / v.atendimentos) * 100) : 0).toFixed(2) })); } catch (e) {} return { ...r, vendas_monitoramento_total, tx_conversao_monitoramento: tx_conversao_monitoramento.toFixed(2), tx_conversao_loja: tx_conversao_loja.toFixed(2), vendedores_processados }; };
app.get('/api/relatorios', requirePageLogin, (req, res) => { const whereClauses = []; const params = []; if (req.query.loja) { whereClauses.push("loja = ?"); params.push(req.query.loja); } if (req.query.data_inicio) { whereClauses.push("data >= ?"); params.push(req.query.data_inicio); } if (req.query.data_fim) { whereClauses.push("data <= ?"); params.push(req.query.data_fim); } const whereString = whereClauses.length > 0 ? " WHERE " + whereClauses.join(" AND ") : ""; const sortOrder = req.query.sortOrder === 'asc' ? 'ASC' : 'DESC'; db.get(`SELECT COUNT(*) as total FROM relatorios` + whereString, params, (err, row) => { if (err) return res.status(500).json({ error: err.message }); const total = row ? row.total : 0; const limit = parseInt(req.query.limit) || 20; const offset = parseInt(req.query.offset) || 0; const query = `SELECT id, loja, data, total_vendas_dinheiro FROM relatorios` + whereString + ` ORDER BY id ${sortOrder} LIMIT ? OFFSET ?`; db.all(query, [...params, limit, offset], (err, relatorios) => { if (err) return res.status(500).json({ error: err.message }); res.json({ relatorios: relatorios || [], total }); }); }); });
app.post('/api/relatorios', requirePageLogin, (req, res) => { const d = req.body; const sql = `INSERT INTO relatorios (loja, data, hora_abertura, hora_fechamento, gerente_entrada, gerente_saida, clientes_monitoramento, vendas_monitoramento, clientes_loja, vendas_loja, total_vendas_dinheiro, ticket_medio, pa, quantidade_trocas, quantidade_omni, quantidade_funcao_especial, vendedores, enviado_por_usuario, vendas_cartao, vendas_pix, vendas_dinheiro) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)`; const params = [ d.loja, d.data, d.hora_abertura, d.hora_fechamento, d.gerente_entrada, d.gerente_saida, parseInt(d.clientes_monitoramento, 10) || 0, parseInt(d.vendas_monitoramento, 10) || 0, parseInt(d.clientes_loja, 10) || 0, parseInt(d.vendas_loja, 10) || 0, parseFloat(String(d.total_vendas_dinheiro).replace(/[R$\s.]/g, '').replace(',', '.')) || 0, d.ticket_medio || 'R$ 0,00', d.pa || '0.00', parseInt(d.quantidade_trocas, 10) || 0, parseInt(d.quantidade_omni, 10) || 0, parseInt(d.quantidade_funcao_especial, 10) || 0, d.vendedores || '[]', req.session.username, parseInt(d.vendas_cartao, 10) || 0, parseInt(d.vendas_pix, 10) || 0, parseInt(d.vendas_dinheiro, 10) || 0 ]; db.run(sql, params, function (err) { if (err) { console.error("Erro ao inserir relatório:", err.message); return res.status(500).json({ error: 'Falha ao salvar relatório.' }); } res.status(201).json({ success: true, id: this.lastID }); }); });
app.get('/api/relatorios/:id', requirePageLogin, (req, res) => { db.get("SELECT * FROM relatorios WHERE id = ?", [req.params.id], (err, relatorio) => { if (err) return res.status(500).json({ error: err.message }); if (!relatorio) return res.status(404).json({ error: "Relatório não encontrado" }); res.json({ relatorio }); }); });
app.put('/api/relatorios/:id', requirePageLogin, (req, res) => { const { id } = req.params; const d = req.body; const sql = `UPDATE relatorios SET loja=?, data=?, hora_abertura=?, hora_fechamento=?, gerente_entrada=?, gerente_saida=?, clientes_monitoramento=?, vendas_monitoramento=?, clientes_loja=?, vendas_loja=?, total_vendas_dinheiro=?, ticket_medio=?, pa=?, quantidade_trocas=?, quantidade_omni=?, quantidade_funcao_especial=?, vendedores=?, vendas_cartao=?, vendas_pix=?, vendas_dinheiro=? WHERE id=?`; const params = [ d.loja, d.data, d.hora_abertura, d.hora_fechamento, d.gerente_entrada, d.gerente_saida, parseInt(d.clientes_monitoramento, 10) || 0, parseInt(d.vendas_monitoramento, 10) || 0, parseInt(d.clientes_loja, 10) || 0, parseInt(d.vendas_loja, 10) || 0, parseFloat(String(d.total_vendas_dinheiro).replace(/[R$\s.]/g, '').replace(',', '.')) || 0, d.ticket_medio || 'R$ 0,00', d.pa || '0.00', parseInt(d.quantidade_trocas, 10) || 0, parseInt(d.quantidade_omni, 10) || 0, parseInt(d.quantidade_funcao_especial, 10) || 0, d.vendedores || '[]', parseInt(d.vendas_cartao, 10) || 0, parseInt(d.vendas_pix, 10) || 0, parseInt(d.vendas_dinheiro, 10) || 0, id ]; db.run(sql, params, function (err) { if (err) { console.error("Erro ao atualizar relatório:", err.message); return res.status(500).json({ error: 'Falha ao atualizar o relatório.' }); } if (this.changes === 0) return res.status(404).json({ error: "Relatório não encontrado." }); res.json({ success: true, id: id }); }); });
app.delete('/api/relatorios/:id', requirePageLogin, (req, res) => { db.run("DELETE FROM relatorios WHERE id = ?", [req.params.id], function (err) { if (err) return res.status(500).json({ error: err.message }); if (this.changes === 0) return res.status(404).json({ error: "Relatório não encontrado" }); res.json({ success: true, message: "Relatório excluído." }); }); });
const formatCurrency = (value) => { const numberValue = Number(value) || 0; return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(numberValue); };
const formatarRelatorioTexto = (r) => { const rp = processarRelatorio(r); if (!rp) return "Erro ao processar relatório."; let equipeInfo = 'Nenhum vendedor registrado.\n'; if (rp.vendedores_processados && rp.vendedores_processados.length > 0) { equipeInfo = rp.vendedores_processados.map(v => { return `${v.nome}: ${v.atendimentos} Atendimentos / ${v.vendas} Vendas / ${v.tx_conversao}%`; }).join('\n'); } let funcaoEspecialInfo = ''; if (rp.funcao_especial === "Omni") { funcaoEspecialInfo = `Omni: ${rp.quantidade_omni || 0}\n`; } else if (rp.funcao_especial === "Busca por Assist. Tec.") { funcaoEspecialInfo = `Busca por assist tec: ${rp.quantidade_funcao_especial || 0}\n`; } const totalVendasQuantidade = (rp.vendas_cartao || 0) + (rp.vendas_pix || 0) + (rp.vendas_dinheiro || 0); const content = ` DATA: ${new Date(rp.data).toLocaleDateString('pt-BR', { timeZone: 'UTC' })} \n\nClientes: ${rp.clientes_monitoramento || 0}\nBluve: ${rp.clientes_loja || 0}\nVendas / Monitoramento: ${rp.vendas_monitoramento_total || 0}\nVendas / Loja: ${rp.vendas_loja || 0}\nTaxa de conversão da loja: ${rp.tx_conversao_loja || '0.00'}%\nTaxa de conversão do monitoramento: ${rp.tx_conversao_monitoramento || '0.00'}%\n\nAbertura: ${rp.hora_abertura || '--:--'} - ${rp.hora_fechamento || '--:--'}\nGerente: ${rp.gerente_entrada || '--:--'} - ${rp.gerente_saida || '--:--'}\nVendas em Cartão: ${rp.vendas_cartao || 0}\nVendas em Pix: ${rp.vendas_pix || 0}\nVendas em Dinheiro: ${rp.vendas_dinheiro || 0}\n${funcaoEspecialInfo}Total vendas: ${totalVendasQuantidade}\nTroca/Devolução: ${rp.quantidade_trocas || 0}\n\nDesempenho Equipe:\n\n${equipeInfo}\n\nTM: ${rp.ticket_medio || 'R$ 0,00'} / P.A: ${rp.pa || '0.00'} / Total: ${formatCurrency(rp.total_vendas_dinheiro)} / `; return content.trim(); };
app.get('/api/relatorios/:id/txt', requirePageLogin, (req, res) => { const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = ? `; db.get(sql, [req.params.id], (err, r) => { if (err || !r) return res.status(404).send('Relatório não encontrado'); res.setHeader('Content-disposition', `attachment; filename=relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.txt`); res.setHeader('Content-type', 'text/plain; charset=utf-8'); res.send(formatarRelatorioTexto(r)); }); });
app.get('/api/relatorios/:id/pdf', requirePageLogin, (req, res) => { const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = ? `; db.get(sql, [req.params.id], (err, r) => { if (err || !r) return res.status(404).send('Relatório não encontrado'); const doc = new PDFDocument({ margin: 50, size: 'A4' }); res.setHeader('Content-disposition', `inline; filename="relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.pdf"`); res.setHeader('Content-type', 'application/pdf'); doc.pipe(res); doc.fontSize(18).font('Helvetica-Bold').text(r.loja.toUpperCase(), { align: 'center' }).moveDown(1); doc.fontSize(11).font('Helvetica').text(formatarRelatorioTexto(r), { align: 'left' }); doc.end(); }); });


// ROTA DE EXPORTAÇÃO PARA EXCEL (Sem alterações, já corrigida)
app.get('/api/export/excel', requirePageLogin, async (req, res) => { const { month, year } = req.query; if (!month || !year) { return res.status(400).json({ error: 'Mês e ano são obrigatórios.' }); } const monthFormatted = month.toString().padStart(2, '0'); const sql = ` SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE strftime('%Y-%m', r.data) = ? ORDER BY r.loja, r.data `; db.all(sql, [`${year}-${monthFormatted}`], async (err, rows) => { if (err) { console.error("Erro ao buscar relatórios para Excel:", err); return res.status(500).json({ error: 'Erro ao buscar relatórios.' }); } if (rows.length === 0) { return res.status(404).json({ error: 'Nenhum relatório encontrado para o período.' }); } const workbook = new ExcelJS.Workbook(); const safeParseFloat = (value) => { if (typeof value === 'number') { return value; } if (typeof value === 'string') { const cleaned = value.replace(/[R$\s]/g, '').replace(/\./g, '').replace(',', '.'); const num = parseFloat(cleaned); return isNaN(num) ? 0 : num; } return 0; }; const relatoriosPorLoja = rows.reduce((acc, row) => { const loja = row.loja; if (!acc[loja]) { acc[loja] = { funcao_especial: row.funcao_especial || 'Não definido', relatorios: [] }; } acc[loja].relatorios.push(processarRelatorio(row)); return acc; }, {}); for (const lojaNome in relatoriosPorLoja) { const lojaData = relatoriosPorLoja[lojaNome]; const worksheet = workbook.addWorksheet(lojaNome.substring(0, 30)); worksheet.mergeCells('A1:M1'); const tituloCell = worksheet.getCell('A1'); tituloCell.value = lojaNome.toUpperCase(); tituloCell.font = { name: 'Arial Black', size: 16, bold: true, color: { argb: 'FF44546A' } }; tituloCell.alignment = { vertical: 'middle', horizontal: 'center' }; worksheet.getRow(1).height = 30; const headers = [ 'DATA', 'BLUVE', 'VENDAS (L)', 'TX DE CONVERSÃO (L)', 'CLIENTES (M)', 'VENDAS (M)', 'TX DE CONVERSÃO (M)', 'P.A', 'TM', 'VALOR TOTAL', 'TROCAS' ]; let funcaoEspecialHeader = 'FUNÇÃO ESPECIAL'; if (lojaData.funcao_especial === 'Omni') { funcaoEspecialHeader = 'OMNI'; } else if (lojaData.funcao_especial === 'Busca por Assist. Tec.') { funcaoEspecialHeader = 'BUSCA P/ ASSIST. TEC.'; } headers.push(funcaoEspecialHeader); headers.push('ENVIADO POR'); const headerRow = worksheet.getRow(3); headerRow.values = headers; headerRow.height = 35; headerRow.eachCell(cell => { cell.font = { bold: true, color: { argb: 'FFFFFFFF' }, size: 10 }; cell.alignment = { vertical: 'middle', horizontal: 'center', wrapText: true }; cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF4472C4' } }; cell.border = { top: { style: 'thin', color: { argb: 'FFBFBFBF' } }, left: { style: 'thin', color: { argb: 'FFBFBFBF' } }, bottom: { style: 'thin', color: { argb: 'FFBFBFBF' } }, right: { style: 'thin', color: { argb: 'FFBFBFBF' } } }; }); lojaData.relatorios.forEach(r => { const rowData = [ new Date(r.data + 'T00:00:00'), parseInt(r.clientes_loja, 10) || 0, parseInt(r.vendas_loja, 10) || 0, parseFloat(r.tx_conversao_loja) / 100, parseInt(r.clientes_monitoramento, 10) || 0, parseInt(r.vendas_monitoramento_total, 10) || 0, parseFloat(r.tx_conversao_monitoramento) / 100, parseFloat(String(r.pa).replace(',', '.')) || 0, safeParseFloat(r.ticket_medio), r.total_vendas_dinheiro, parseInt(r.quantidade_trocas, 10) || 0 ]; if (lojaData.funcao_especial === 'Omni') { rowData.push(parseInt(r.quantidade_omni, 10) || 0); } else if (lojaData.funcao_especial === 'Busca por Assist. Tec.') { rowData.push(parseInt(r.quantidade_funcao_especial, 10) || 0); } else { rowData.push(0); } rowData.push(r.enviado_por_usuario || '-'); const row = worksheet.addRow(rowData); row.getCell(1).numFmt = 'DD/MM/YYYY'; row.getCell(4).numFmt = '0.00%'; row.getCell(7).numFmt = '0.00%'; row.getCell(8).numFmt = '0.00'; row.getCell(9).numFmt = 'R$ #,##0.00'; row.getCell(10).numFmt = 'R$ #,##0.00'; row.eachCell(cell => { cell.alignment = { vertical: 'middle', horizontal: 'center' }; }); }); worksheet.columns.forEach(column => { let maxLength = 0; column.eachCell({ includeEmpty: true }, cell => { const length = cell.value ? cell.value.toString().length : 10; if (length > maxLength) { maxLength = length; } }); column.width = Math.max(12, maxLength + 3); }); worksheet.getColumn(4).width = 20; worksheet.getColumn(7).width = 20; worksheet.getColumn(12).width = 22; } res.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'); res.setHeader('Content-Disposition', `attachment; filename="Relatorios_${year}-${monthFormatted}.xlsx"`); await workbook.xlsx.write(res); res.end(); }); });

// APIs DE DASHBOARD, DEMANDAS, BACKUP E RESTORE (Sem alterações)
app.get('/api/dashboard-data', requirePageLogin, (req, res) => { let whereClauses = []; let params = []; if (req.query.loja && req.query.loja !== 'todas') { whereClauses.push('loja = ?'); params.push(req.query.loja); } if (req.query.data_inicio) { whereClauses.push('data >= ?'); params.push(req.query.data_inicio); } if (req.query.data_fim) { whereClauses.push('data <= ?'); params.push(req.query.data_fim); } const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : ''; const sql = `SELECT COALESCE(SUM(clientes_monitoramento),0) as total_clientes_monitoramento, COALESCE(SUM(vendas_monitoramento),0) as total_vendas_monitoramento, COALESCE(SUM(clientes_loja),0) as total_clientes_loja, COALESCE(SUM(vendas_loja),0) as total_vendas_loja, COALESCE(SUM(quantidade_omni),0) as total_omni FROM relatorios ${whereString}`; db.get(sql, params, (err, row) => { if (err) return res.status(500).json({ error: err.message }); const vendas_m_total = (row.total_vendas_monitoramento || 0) + (row.total_omni || 0); res.json({ ...row, tx_conversao_monitoramento: (row.total_clientes_monitoramento > 0 ? (vendas_m_total / row.total_clientes_monitoramento) * 100 : 0), tx_conversao_loja: (row.total_clientes_loja > 0 ? (row.total_vendas_loja / row.total_clientes_loja) * 100 : 0) }); }); });
app.get('/api/ranking', requirePageLogin, (req, res) => { let whereClauses = []; let params = []; if (req.query.data_inicio) { whereClauses.push('r.data >= ?'); params.push(req.query.data_inicio); } if (req.query.data_fim) { whereClauses.push('r.data <= ?'); params.push(req.query.data_fim); } const joinCondition = whereClauses.length > 0 ? `AND ${whereClauses.join(' AND ')}` : ''; const sql = `SELECT l.nome as loja, COALESCE(SUM(r.clientes_loja), 0) as total_clientes_loja, COALESCE(SUM(r.vendas_loja), 0) as total_vendas_loja, COALESCE(SUM(r.clientes_monitoramento), 0) as total_clientes_monitoramento, COALESCE(SUM(r.vendas_monitoramento), 0) as total_vendas_monitoramento, COALESCE(SUM(r.quantidade_omni), 0) as total_omni FROM lojas l LEFT JOIN relatorios r ON l.nome = r.loja ${joinCondition} WHERE l.status = 'ativa' GROUP BY l.nome`; db.all(sql, params, (err, rows) => { if (err) return res.status(500).json({ error: err.message }); const ranking = rows.map(r => { const vendas_m_total = (r.total_vendas_monitoramento || 0) + (r.total_omni || 0); return { ...r, tx_loja: (r.total_clientes_loja > 0 ? (r.total_vendas_loja / r.total_clientes_loja) * 100 : 0), tx_monitoramento: (r.total_clientes_monitoramento > 0 ? (vendas_m_total / r.total_clientes_monitoramento) * 100 : 0) } }); res.json(ranking); }); });
app.get('/api/dashboard/chart-data', requirePageLogin, (req, res) => { const { loja, data_inicio, data_fim } = req.query; let whereClauses = []; let params = []; if (loja && loja !== 'todas') { whereClauses.push('loja = ?'); params.push(loja); } if (data_inicio) { whereClauses.push('data >= ?'); params.push(data_inicio); } if (data_fim) { whereClauses.push('data <= ?'); params.push(data_fim); } if (whereClauses.length === 0) { const date = new Date(); date.setDate(date.getDate() - 30); const startDate = date.toISOString().slice(0, 10); whereClauses.push('data >= ?'); params.push(startDate); } const whereString = `WHERE ${whereClauses.join(' AND ')}`; const sql = `SELECT data, SUM(clientes_loja) as total_clientes_loja, SUM(vendas_loja) as total_vendas_loja, SUM(clientes_monitoramento) as total_clientes_monitoramento, SUM(vendas_monitoramento) as total_vendas_monitoramento, SUM(quantidade_omni) as total_omni FROM relatorios ${whereString} GROUP BY data ORDER BY data ASC`; db.all(sql, params, (err, rows) => { if (err) return res.status(500).json({ error: 'Erro ao buscar dados para o gráfico.' }); const labels = []; const txConversaoLoja = []; const txConversaoMonitoramento = []; rows.forEach(row => { labels.push(new Date(row.data).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', timeZone: 'UTC' })); const vendas_m_total = (row.total_vendas_monitoramento || 0) + (row.total_omni || 0); const tx_m = row.total_clientes_monitoramento > 0 ? (vendas_m_total / row.total_clientes_monitoramento) * 100 : 0; const tx_l = row.total_clientes_loja > 0 ? (row.total_vendas_loja / row.total_clientes_loja) * 100 : 0; txConversaoLoja.push(tx_l.toFixed(2)); txConversaoMonitoramento.push(tx_m.toFixed(2)); }); res.json({ labels, txConversaoLoja, txConversaoMonitoramento }); }); });
app.post('/api/demandas', requirePageLogin, (req, res) => { const { loja_nome, descricao, tag } = req.body; db.run('INSERT INTO demandas (loja_nome, descricao, tag, criado_por_usuario) VALUES (?, ?, ?, ?)', [loja_nome, descricao, tag, req.session.username], function (err) { if (err) return res.status(500).json({ error: 'Falha ao salvar demanda.' }); res.status(201).json({ success: true, id: this.lastID }); }); });
app.get('/api/demandas/:status', requirePageLogin, (req, res) => { const status = req.params.status === 'pendentes' ? 'pendente' : 'concluido'; db.all(`SELECT * FROM demandas WHERE status = ? ORDER BY criado_em DESC`, [status], (err, demandas) => { if (err) return res.status(500).json({ error: err.message }); res.json(demandas || []); }); });
app.put('/api/demandas/:id/concluir', requirePageLogin, (req, res) => { db.run("UPDATE demandas SET status = 'concluido', concluido_por_usuario = ?, concluido_em = CURRENT_TIMESTAMP WHERE id = ?", [req.session.username, req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao concluir demanda.' }); if (this.changes === 0) return res.status(404).json({ error: 'Demanda não encontrada.' }); res.json({ success: true }); }); });
app.delete('/api/demandas/:id', requirePageLogin, (req, res) => { db.run("DELETE FROM demandas WHERE id = ?", [req.params.id], function (err) { if (err) return res.status(500).json({ error: 'Erro ao excluir demanda.' }); if (this.changes === 0) return res.status(404).json({ error: "Demanda não encontrada." }); res.json({ success: true }); }); });
app.get('/api/backup/download', requirePageLogin, requireAdmin, (req, res) => { const date = new Date().toISOString().slice(0, 10); const fileName = `backup_reports_${date}.db`; res.download(DB_PATH, fileName, (err) => { if (err && !res.headersSent) { res.status(500).send("Não foi possível baixar o arquivo de backup."); } }); });
app.post('/api/backup/restore', requirePageLogin, requireAdmin, upload.single('backupFile'), (req, res) => { if (!req.file) { return res.status(400).json({ error: "Nenhum arquivo de backup foi enviado." }); } const backupBuffer = req.file.buffer; db.close((err) => { if (err) { console.error("Erro ao fechar o DB antes de restaurar:", err.message); return res.status(500).json({ error: "Não foi possível fechar a conexão com o banco de dados atual." }); } fs.writeFile(DB_PATH, backupBuffer, (err) => { if (err) { console.error("Falha ao escrever o arquivo de backup:", err.message); db = new sqlite3.Database(DB_PATH); return res.status(500).json({ error: "Falha ao substituir o arquivo de banco de dados." }); } db = new sqlite3.Database(DB_PATH, (err) => { if (err) { console.error("DB restaurado, mas falha ao reconectar:", err.message); return res.status(500).json({ error: "Banco de dados restaurado, mas falha ao reconectar. Reinicie o servidor." }); } console.log("Banco de dados restaurado e reconectado com sucesso."); res.json({ success: true, message: "Banco de dados restaurado com sucesso. A página será recarregada." }); }); }); }); });

// =================================================================
// INICIALIZAÇÃO DO SERVIDOR
// =================================================================
app.listen(PORT, () => console.log(`Servidor rodando em http://localhost:${PORT}`));