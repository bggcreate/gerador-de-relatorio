// =================================================================
// CONFIGURAÇÕES INICIAIS
// =================================================================
require('dotenv').config(); // Carrega as variáveis do arquivo .env
const express = require('express');
const { Pool } = require('pg');
const path = require('path');
const session = require('express-session');
const fs = require('fs');
const PDFDocument = require('pdfkit');
const ExcelJS = require('exceljs');
const multer = require('multer');
const pdf = require('pdf-parse');

const app = express();
const PORT = process.env.PORT || 3000;

// --- CONFIGURAÇÃO DO BANCO DE DADOS POSTGRESQL ---
const isProduction = process.env.NODE_ENV === 'production';
const connectionConfig = {
    connectionString: process.env.DATABASE_URL,
    ssl: isProduction ? { rejectUnauthorized: false } : false
};
const pool = new Pool(connectionConfig);

// --- CONFIGURAÇÃO GERAL ---
app.use(express.static(path.join(__dirname, 'public')));
app.use(express.urlencoded({ extended: true }));
app.use(express.json());
app.use(session({ secret: 'chave-definitiva-123', resave: false, saveUninitialized: false, cookie: { httpOnly: true, maxAge: 24 * 60 * 60 * 1000 } }));

// --- CONFIGURAÇÃO DO MULTER ---
const upload = multer({ storage: multer.memoryStorage() });

// --- MIDDLEWARES ---
const requirePageLogin = (req, res, next) => {
    if (req.session && req.session.userId) return next();
    res.redirect('/login');
};
const requireAdmin = (req, res, next) => {
    if (req.session && req.session.role === 'admin') return next();
    res.status(403).json({ error: 'Acesso negado.' });
};

// --- FUNÇÃO DE INICIALIZAÇÃO DO BANCO DE DADOS ---
const initializeDatabase = async () => {
    try {
        await pool.query(`
            CREATE TABLE IF NOT EXISTS usuarios (id SERIAL PRIMARY KEY, username TEXT UNIQUE NOT NULL, password TEXT NOT NULL, role TEXT NOT NULL);
        `);
        await pool.query(`
            CREATE TABLE IF NOT EXISTS lojas (id SERIAL PRIMARY KEY, nome TEXT UNIQUE NOT NULL, status TEXT, funcao_especial TEXT, observacoes TEXT);
        `);
        await pool.query(`
            CREATE TABLE IF NOT EXISTS relatorios (
                id SERIAL PRIMARY KEY, loja TEXT, data DATE, hora_abertura TEXT, hora_fechamento TEXT,
                gerente_entrada TEXT, gerente_saida TEXT, clientes_monitoramento INTEGER, vendas_monitoramento INTEGER,
                clientes_loja INTEGER, vendas_loja INTEGER, total_vendas_dinheiro REAL, ticket_medio TEXT, pa TEXT,
                quantidade_trocas INTEGER, nome_funcao_especial TEXT, quantidade_funcao_especial INTEGER,
                quantidade_omni INTEGER, vendedores JSONB, nome_arquivo TEXT, enviado_por_usuario TEXT,
                enviado_em TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP, vendas_cartao INTEGER, vendas_pix INTEGER, vendas_dinheiro INTEGER
            );
        `);
        await pool.query(`
            CREATE TABLE IF NOT EXISTS demandas (
                id SERIAL PRIMARY KEY, loja_nome TEXT NOT NULL, descricao TEXT NOT NULL, tag TEXT DEFAULT 'Normal',
                status TEXT DEFAULT 'pendente', criado_por_usuario TEXT, concluido_por_usuario TEXT,
                criado_em TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP, concluido_em TIMESTAMPTZ
            );
        `);
        const adminUsername = 'admin';
        const correctPassword = 'admin';
        const adminResult = await pool.query('SELECT * FROM usuarios WHERE username = $1', [adminUsername]);
        if (adminResult.rowCount === 0) {
            await pool.query('INSERT INTO usuarios (username, password, role) VALUES ($1, $2, $3)', [adminUsername, correctPassword, 'admin']);
            console.log("Usuário admin criado.");
        }
        console.log("Banco de dados pronto.");
    } catch (err) {
        console.error("Erro fatal ao inicializar o banco de dados:", err.message);
        process.exit(1);
    }
};

// --- ROTAS DE PÁGINAS ---
app.get('/login', (req, res) => res.sendFile(path.join(__dirname, 'views', 'login.html')));
app.get('/live', requirePageLogin, (req, res) => res.sendFile(path.join(__dirname, 'views', 'live.html')));
app.get(['/', '/admin', '/consulta', '/demandas', '/gerenciar-lojas', '/novo-relatorio', '/gerenciar-usuarios'], requirePageLogin, (req, res) => {
    res.sendFile(path.join(__dirname, 'views', 'index.html'));
});
app.get('/content/:page', requirePageLogin, (req, res) => {
    const allowedPages = ['admin', 'consulta', 'demandas', 'gerenciar-lojas', 'novo-relatorio', 'gerenciar-usuarios'];
    if (allowedPages.includes(req.params.page)) res.sendFile(path.join(__dirname, 'views', `${req.params.page}.html`));
    else res.status(404).send('Página não encontrada');
});


// --- ROTAS DE API --- :D

// API DE PROCESSAMENTO DE PDF - MUITA DOR DE CABEÇA ISSO SLC
app.post('/api/process-pdf', requirePageLogin, upload.single('pdfFile'), async (req, res) => {
    if (!req.file) return res.status(400).json({ error: "Nenhum arquivo PDF enviado." });
    try {
        const data = await pdf(req.file.buffer);
        const text = data.text;
        const lines = text.split('\n').map(line => line.trim()).filter(Boolean);
        const parse = str => parseFloat(str.replace(/\./g, '').replace(',', '.'));
        const linhaTotais = lines.find(l => l.includes('Totais:'));
        const idxTotais = lines.indexOf(linhaTotais);
        const linhaDados = lines[idxTotais + 1] || '';
        const linhaLimpa = linhaDados.replace(/(\d{1,3})\.(\d{3},\d{2})/g, '$1.$2 ').replace(/ +/g, ' ').trim();
        const valoresTotais = linhaLimpa.match(/\d{1,3}(?:\.\d{3})*,\d{2}/g);
        if (!valoresTotais || valoresTotais.length < 7) {
            throw new Error("Não foi possível extrair os valores corretamente da linha Totais.");
        }
        const totalVendasValor = parse(valoresTotais[0]);
        const pa = parse(valoresTotais[valoresTotais.length - 4]);
        const ticketMedio = parse(valoresTotais[valoresTotais.length - 3]);
        const linhaSplitada = linhaLimpa.split(' ');
        const indexDoValorTotal = linhaSplitada.findIndex(v => v.includes(valoresTotais[0]));
        const totalAtendimentos = parseInt(linhaSplitada[indexDoValorTotal + 2], 10) || 0;
        const vendasLoja = totalAtendimentos;
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
        res.json({
            success: true,
            data: {
                loja: storeName, data: reportDate, clientes_loja: totalAtendimentos,
                vendas_loja: vendasLoja, total_vendas_dinheiro: `R$ ${totalVendasValor.toFixed(2).replace('.', ',')}`,
                ticket_medio: `R$ ${ticketMedio.toFixed(2).replace('.', ',')}`, pa: pa.toFixed(2).replace('.', ','),
                vendedores
            }
        });
    } catch (error) {
        console.error("### ERRO NO PROCESSAMENTO DO PDF ###", error);
        res.status(500).json({ error: error.message || "Erro ao processar o PDF." });
    }
});


// APIs DE SESSÃO E USUÁRIOS - CADASTRO 
app.post('/api/login', async (req, res) => {
    const { username, password } = req.body;
    try {
        const result = await pool.query('SELECT * FROM usuarios WHERE username = $1 AND password = $2', [username, password]);
        if (result.rowCount === 0) return res.status(401).json({ message: 'Credenciais inválidas.' });
        const user = result.rows[0];
        req.session.userId = user.id; req.session.username = user.username; req.session.role = user.role;
        res.json({ success: true });
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/logout', (req, res) => { req.session.destroy(() => res.redirect('/login')); });
app.get('/api/session-info', requirePageLogin, (req, res) => res.json({ id: req.session.userId, username: req.session.username, role: req.session.role }));
app.get('/api/usuarios', requirePageLogin, requireAdmin, async (req, res) => {
    try {
        const result = await pool.query("SELECT id, username, role FROM usuarios ORDER BY username");
        res.json(result.rows);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/usuarios', requirePageLogin, requireAdmin, async (req, res) => {
    const { username, password, role } = req.body;
    if (!username || !password || !role) return res.status(400).json({ error: 'Todos os campos são obrigatórios.' });
    try {
        const result = await pool.query('INSERT INTO usuarios (username, password, role) VALUES ($1, $2, $3) RETURNING id', [username, password, role]);
        res.status(201).json({ success: true, id: result.rows[0].id });
    } catch (err) { res.status(500).json({ error: 'Erro ao criar usuário. O nome de usuário já pode existir.' }); }
});

app.put('/api/usuarios/:id', requirePageLogin, requireAdmin, async (req, res) => {
    const { id } = req.params;
    const { username, password, role } = req.body;
    if (!username || !role) return res.status(400).json({ error: 'Username e Cargo são obrigatórios.' });
    try {
        const sql = password ? 'UPDATE usuarios SET username = $1, password = $2, role = $3 WHERE id = $4' : 'UPDATE usuarios SET username = $1, role = $2 WHERE id = $3';
        const params = password ? [username, password, role, id] : [username, role, id];
        await pool.query(sql, params);
        res.json({ success: true });
    } catch (err) { res.status(500).json({ error: 'Erro ao atualizar usuário.' }); }
});

app.delete('/api/usuarios/:id', requirePageLogin, requireAdmin, async (req, res) => {
    const { id } = req.params;
    if (id == req.session.userId) return res.status(403).json({ error: 'Não é permitido excluir o próprio usuário logado.' });
    try {
        const result = await pool.query("DELETE FROM usuarios WHERE id = $1", [id]);
        if (result.rowCount === 0) return res.status(404).json({ error: "Usuário não encontrado." });
        res.json({ success: true });
    } catch (err) { res.status(500).json({ error: 'Erro ao excluir usuário.' }); }
});

// APIs DE LOJAS
app.get('/api/lojas', requirePageLogin, async (req, res) => {
    try {
        let query = "SELECT * FROM lojas";
        const params = [];
        if (req.query.status) {
            query += " WHERE status = $1";
            params.push(req.query.status);
        }
        query += " ORDER BY nome";
        const result = await pool.query(query, params);
        res.json(result.rows);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/lojas', requirePageLogin, async (req, res) => {
    const { nome, status, funcao_especial, observacoes } = req.body;
    try {
        const result = await pool.query('INSERT INTO lojas (nome, status, funcao_especial, observacoes) VALUES ($1, $2, $3, $4) RETURNING id', [nome, status, funcao_especial, observacoes]);
        res.status(201).json({ success: true, id: result.rows[0].id });
    } catch (err) { res.status(500).json({ error: 'Erro ao criar loja. O nome já pode existir.' }); }
});

app.put('/api/lojas/:id', requirePageLogin, async (req, res) => {
    const { id } = req.params;
    const { nome, status, funcao_especial, observacoes } = req.body;
    try {
        await pool.query('UPDATE lojas SET nome = $1, status = $2, funcao_especial = $3, observacoes = $4 WHERE id = $5', [nome, status, funcao_especial, observacoes, id]);
        res.json({ success: true });
    } catch (err) { res.status(500).json({ error: 'Erro ao atualizar loja.' }); }
});

app.delete('/api/lojas/:id', requirePageLogin, async (req, res) => {
    try {
        const result = await pool.query("DELETE FROM lojas WHERE id = $1", [req.params.id]);
        if (result.rowCount === 0) return res.status(404).json({ error: "Loja não encontrada." });
        res.json({ success: true });
    } catch (err) { res.status(500).json({ error: 'Erro ao excluir loja.' }); }
});

// APIs DE RELATÓRIOS
const processarRelatorio = (r) => {
    if (!r) return null;
    const vendas_monitoramento_total = (parseInt(r.vendas_monitoramento, 10) || 0) + (parseInt(r.quantidade_omni, 10) || 0);
    const tx_conversao_monitoramento = (parseInt(r.clientes_monitoramento, 10) || 0) > 0 ? (vendas_monitoramento_total / r.clientes_monitoramento) * 100 : 0;
    const tx_conversao_loja = (parseInt(r.clientes_loja, 10) || 0) > 0 ? ((parseInt(r.vendas_loja, 10) || 0) / r.clientes_loja) * 100 : 0;
    let vendedores_processados = [];
    try {
        vendedores_processados = (r.vendedores || []).map(v => ({ ...v, tx_conversao: (v.atendimentos > 0 ? ((v.vendas / v.atendimentos) * 100) : 0).toFixed(2) }));
    } catch (e) { }
    return { ...r, vendas_monitoramento_total, tx_conversao_monitoramento: tx_conversao_monitoramento.toFixed(2), tx_conversao_loja: tx_conversao_loja.toFixed(2), vendedores_processados };
};

app.get('/api/relatorios', requirePageLogin, async (req, res) => {
    try {
        let whereClauses = []; let params = []; let paramIndex = 1;
        if (req.query.loja) { whereClauses.push(`loja = $${paramIndex++}`); params.push(req.query.loja); }
        if (req.query.data_inicio) { whereClauses.push(`data >= $${paramIndex++}`); params.push(req.query.data_inicio); }
        if (req.query.data_fim) { whereClauses.push(`data <= $${paramIndex++}`); params.push(req.query.data_fim); }
        
        const whereString = whereClauses.length > 0 ? " WHERE " + whereClauses.join(" AND ") : "";
        const totalResult = await pool.query(`SELECT COUNT(*) as total FROM relatorios` + whereString, params);
        const total = totalResult.rows[0] ? parseInt(totalResult.rows[0].total, 10) : 0;

        const sortOrder = req.query.sortOrder === 'asc' ? 'ASC' : 'DESC';
        const limit = parseInt(req.query.limit) || 20;
        const offset = parseInt(req.query.offset) || 0;
        
        const finalParams = [...params, limit, offset];
        const query = `SELECT id, loja, data, total_vendas_dinheiro FROM relatorios` + whereString + ` ORDER BY id ${sortOrder} LIMIT $${paramIndex++} OFFSET $${paramIndex++}`;
        
        const relatoriosResult = await pool.query(query, finalParams);
        res.json({ relatorios: relatoriosResult.rows, total });
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/relatorios', requirePageLogin, async (req, res) => {
    const d = req.body;
    const sql = `INSERT INTO relatorios (loja, data, hora_abertura, hora_fechamento, gerente_entrada, gerente_saida, clientes_monitoramento, vendas_monitoramento, clientes_loja, vendas_loja, total_vendas_dinheiro, ticket_medio, pa, quantidade_trocas, quantidade_omni, quantidade_funcao_especial, vendedores, enviado_por_usuario, vendas_cartao, vendas_pix, vendas_dinheiro) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, $16, $17, $18, $19, $20, $21) RETURNING id`;
    const params = [
        d.loja, d.data, d.hora_abertura, d.hora_fechamento, d.gerente_entrada, d.gerente_saida,
        parseInt(d.clientes_monitoramento, 10) || 0, parseInt(d.vendas_monitoramento, 10) || 0,
        parseInt(d.clientes_loja, 10) || 0, parseInt(d.vendas_loja, 10) || 0,
        parseFloat(String(d.total_vendas_dinheiro).replace(/[R$\s.]/g, '').replace(',', '.')) || 0,
        d.ticket_medio || 'R$ 0,00', d.pa || '0.00',
        parseInt(d.quantidade_trocas, 10) || 0, parseInt(d.quantidade_omni, 10) || 0,
        parseInt(d.quantidade_funcao_especial, 10) || 0, d.vendedores || '[]',
        req.session.username, parseInt(d.vendas_cartao, 10) || 0,
        parseInt(d.vendas_pix, 10) || 0, parseInt(d.vendas_dinheiro, 10) || 0
    ];
    try {
        const result = await pool.query(sql, params);
        res.status(201).json({ success: true, id: result.rows[0].id });
    } catch (err) {
        console.error("Erro ao inserir relatório:", err.message);
        res.status(500).json({ error: 'Falha ao salvar relatório.' });
    }
});

app.get('/api/relatorios/:id', requirePageLogin, async (req, res) => {
    try {
        const result = await pool.query("SELECT * FROM relatorios WHERE id = $1", [req.params.id]);
        if (result.rowCount === 0) return res.status(404).json({ error: "Relatório não encontrado" });
        res.json({ relatorio: result.rows[0] });
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.put('/api/relatorios/:id', requirePageLogin, async (req, res) => {
    const { id } = req.params;
    const d = req.body;
    const sql = `UPDATE relatorios SET loja=$1, data=$2, hora_abertura=$3, hora_fechamento=$4, gerente_entrada=$5, gerente_saida=$6, clientes_monitoramento=$7, vendas_monitoramento=$8, clientes_loja=$9, vendas_loja=$10, total_vendas_dinheiro=$11, ticket_medio=$12, pa=$13, quantidade_trocas=$14, quantidade_omni=$15, quantidade_funcao_especial=$16, vendedores=$17, vendas_cartao=$18, vendas_pix=$19, vendas_dinheiro=$20 WHERE id=$21`;
    const params = [
        d.loja, d.data, d.hora_abertura, d.hora_fechamento, d.gerente_entrada, d.gerente_saida,
        parseInt(d.clientes_monitoramento, 10) || 0, parseInt(d.vendas_monitoramento, 10) || 0,
        parseInt(d.clientes_loja, 10) || 0, parseInt(d.vendas_loja, 10) || 0,
        parseFloat(String(d.total_vendas_dinheiro).replace(/[R$\s.]/g, '').replace(',', '.')) || 0,
        d.ticket_medio || 'R$ 0,00', d.pa || '0.00',
        parseInt(d.quantidade_trocas, 10) || 0, parseInt(d.quantidade_omni, 10) || 0,
        parseInt(d.quantidade_funcao_especial, 10) || 0, d.vendedores || '[]',
        parseInt(d.vendas_cartao, 10) || 0, parseInt(d.vendas_pix, 10) || 0,
        parseInt(d.vendas_dinheiro, 10) || 0, id
    ];
    try {
        const result = await pool.query(sql, params);
        if (result.rowCount === 0) return res.status(404).json({ error: "Relatório não encontrado." });
        res.json({ success: true, id: id });
    } catch (err) {
        console.error("Erro ao atualizar relatório:", err.message);
        res.status(500).json({ error: 'Falha ao atualizar o relatório.' });
    }
});

app.delete('/api/relatorios/:id', requirePageLogin, async (req, res) => {
    try {
        const result = await pool.query("DELETE FROM relatorios WHERE id = $1", [req.params.id]);
        if (result.rowCount === 0) return res.status(404).json({ error: "Relatório não encontrado" });
        res.json({ success: true, message: "Relatório excluído." });
    } catch (err) { res.status(500).json({ error: err.message }); }
});

// APIs DE EXPORTAÇÃO E DASHBOARD
const formatCurrency = (value) => {
    const numberValue = Number(value) || 0;
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(numberValue);
};

const formatarRelatorioTexto = (r) => {
    const rp = processarRelatorio(r);
    if (!rp) return "Erro ao processar relatório.";
    let equipeInfo = 'Nenhum vendedor registrado.\n';
    if (rp.vendedores_processados && rp.vendedores_processados.length > 0) {
        equipeInfo = rp.vendedores_processados.map(v => `${v.nome}: ${v.atendimentos} Atendimentos / ${v.vendas} Vendas / ${v.tx_conversao}%`).join('\n');
    }
    let funcaoEspecialInfo = '';
    if (rp.funcao_especial === "Omni") {
        funcaoEspecialInfo = `Omni: ${rp.quantidade_omni || 0}\n`;
    } else if (rp.funcao_especial === "Busca por Assist. Tec.") {
        funcaoEspecialInfo = `Busca por assist tec: ${rp.quantidade_funcao_especial || 0}\n`;
    }
    const totalVendasQuantidade = (rp.vendas_cartao || 0) + (rp.vendas_pix || 0) + (rp.vendas_dinheiro || 0);
    const content = `DATA: ${new Date(rp.data).toLocaleDateString('pt-BR', { timeZone: 'UTC' })}\n\nClientes: ${rp.clientes_monitoramento || 0}\nBluve: ${rp.clientes_loja || 0}\nVendas / Monitoramento: ${rp.vendas_monitoramento_total || 0}\nVendas / Loja: ${rp.vendas_loja || 0}\nTaxa de conversão da loja: ${rp.tx_conversao_loja || '0.00'}%\nTaxa de conversão do monitoramento: ${rp.tx_conversao_monitoramento || '0.00'}%\n\nAbertura: ${rp.hora_abertura || '--:--'} - ${rp.hora_fechamento || '--:--'}\nGerente: ${rp.gerente_entrada || '--:--'} - ${rp.gerente_saida || '--:--'}\nVendas em Cartão: ${rp.vendas_cartao || 0}\nVendas em Pix: ${rp.vendas_pix || 0}\nVendas em Dinheiro: ${rp.vendas_dinheiro || 0}\n${funcaoEspecialInfo}Total vendas: ${totalVendasQuantidade}\nTroca/Devolução: ${rp.quantidade_trocas || 0}\n\nDesempenho Equipe:\n\n${equipeInfo}\n\nTM: ${rp.ticket_medio || 'R$ 0,00'} / P.A: ${rp.pa || '0.00'} / Total: ${formatCurrency(rp.total_vendas_dinheiro)} /`;
    return content.trim();
};

app.get('/api/relatorios/:id/txt', requirePageLogin, async (req, res) => {
    const sql = `SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = $1`;
    try {
        const result = await pool.query(sql, [req.params.id]);
        if (result.rowCount === 0) return res.status(404).send('Relatório não encontrado');
        const r = result.rows[0];
        res.setHeader('Content-disposition', `attachment; filename=relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.txt`);
        res.setHeader('Content-type', 'text/plain; charset=utf-8');
        res.send(formatarRelatorioTexto(r));
    } catch (err) { res.status(500).send('Erro ao gerar relatório'); }
});

app.get('/api/relatorios/:id/pdf', requirePageLogin, async (req, res) => {
    const sql = `SELECT r.*, l.funcao_especial FROM relatorios r LEFT JOIN lojas l ON r.loja = l.nome WHERE r.id = $1`;
    try {
        const result = await pool.query(sql, [req.params.id]);
        if (result.rowCount === 0) return res.status(404).send('Relatório não encontrado');
        const r = result.rows[0];
        const doc = new PDFDocument({ margin: 50, size: 'A4' });
        res.setHeader('Content-disposition', `inline; filename="relatorio_${r.loja.replace(/ /g, '_')}_${r.data}.pdf"`);
        res.setHeader('Content-type', 'application/pdf');
        doc.pipe(res);
        doc.fontSize(18).font('Helvetica-Bold').text(r.loja.toUpperCase(), { align: 'center' }).moveDown(1);
        doc.fontSize(11).font('Helvetica').text(formatarRelatorioTexto(r), { align: 'left' });
        doc.end();
    } catch (err) { res.status(500).send('Erro ao gerar PDF'); }
});

// ADICIONADO: Rotas de Dashboard, Ranking e Demandas convertidas
app.get('/api/export/excel', requirePageLogin, async (req, res) => {
    const { month, year } = req.query;
    if (!month || !year) return res.status(400).json({ error: 'Mês e ano são obrigatórios.' });
    const monthFormatted = month.toString().padStart(2, '0');
    const sql = `SELECT * FROM relatorios WHERE TO_CHAR(data, 'YYYY-MM') = $1 ORDER BY loja, data`;
    try {
        const result = await pool.query(sql, [`${year}-${monthFormatted}`]);
        const rows = result.rows;
        if (rows.length === 0) return res.status(404).json({ error: 'Nenhum relatório encontrado para o período.' });
        const workbook = new ExcelJS.Workbook();
        const relatoriosPorLoja = rows.reduce((acc, row) => ({ ...acc, [row.loja]: [...(acc[row.loja] || []), processarRelatorio(row)] }), {});
        for (const lojaNome in relatoriosPorLoja) {
            const worksheet = workbook.addWorksheet(lojaNome.substring(0, 30));
            worksheet.mergeCells('A1:G1');
            worksheet.getCell('A1').value = lojaNome;
            worksheet.getRow(3).values = ['Data', 'Total Vendas R$', 'Ticket Médio', 'P.A', 'Nº Atendimentos', 'Tx Conv. (M)', 'Tx Conv. (L)'];
            relatoriosPorLoja[lojaNome].forEach(r => {
                worksheet.addRow([new Date(r.data), r.total_vendas_dinheiro, r.ticket_medio, r.pa, r.vendas_loja, parseFloat(r.tx_conversao_monitoramento) / 100, parseFloat(r.tx_conversao_loja) / 100]);
            });
        }
        res.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        res.setHeader('Content-Disposition', `attachment; filename="Relatorios_${new Date(year, month - 1).toLocaleString('pt-BR', { month: 'long' })}_${year}.xlsx"`);
        await workbook.xlsx.write(res);
        res.end();
    } catch (err) { res.status(500).json({ error: 'Erro ao buscar relatórios.' }); }
});

app.get('/api/dashboard-data', requirePageLogin, async (req, res) => {
    try {
        let whereClauses = []; let params = []; let paramIndex = 1;
        if (req.query.loja && req.query.loja !== 'todas') { whereClauses.push(`loja = $${paramIndex++}`); params.push(req.query.loja); }
        if (req.query.data_inicio) { whereClauses.push(`data >= $${paramIndex++}`); params.push(req.query.data_inicio); }
        if (req.query.data_fim) { whereClauses.push(`data <= $${paramIndex++}`); params.push(req.query.data_fim); }
        
        const whereString = whereClauses.length > 0 ? `WHERE ${whereClauses.join(' AND ')}` : '';
        const sql = `SELECT COALESCE(SUM(clientes_monitoramento),0) as total_clientes_monitoramento, COALESCE(SUM(vendas_monitoramento),0) as total_vendas_monitoramento, COALESCE(SUM(clientes_loja),0) as total_clientes_loja, COALESCE(SUM(vendas_loja),0) as total_vendas_loja, COALESCE(SUM(quantidade_omni),0) as total_omni FROM relatorios ${whereString}`;
        
        const result = await pool.query(sql, params);
        const row = result.rows[0];
        const vendas_m_total = (Number(row.total_vendas_monitoramento) || 0) + (Number(row.total_omni) || 0);
        res.json({ ...row, tx_conversao_monitoramento: (row.total_clientes_monitoramento > 0 ? (vendas_m_total / row.total_clientes_monitoramento) * 100 : 0), tx_conversao_loja: (row.total_clientes_loja > 0 ? (row.total_vendas_loja / row.total_clientes_loja) * 100 : 0) });
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/ranking', requirePageLogin, async (req, res) => {
    try {
        let whereClauses = []; let params = []; let paramIndex = 1;
        if (req.query.data_inicio) { whereClauses.push(`r.data >= $${paramIndex++}`); params.push(req.query.data_inicio); }
        if (req.query.data_fim) { whereClauses.push(`r.data <= $${paramIndex++}`); params.push(req.query.data_fim); }
        
        const joinCondition = whereClauses.length > 0 ? `AND ${whereClauses.join(' AND ')}` : '';
        const sql = `SELECT l.nome as loja, COALESCE(SUM(r.clientes_loja), 0) as total_clientes_loja, COALESCE(SUM(r.vendas_loja), 0) as total_vendas_loja, COALESCE(SUM(r.clientes_monitoramento), 0) as total_clientes_monitoramento, COALESCE(SUM(r.vendas_monitoramento), 0) as total_vendas_monitoramento, COALESCE(SUM(r.quantidade_omni), 0) as total_omni FROM lojas l LEFT JOIN relatorios r ON l.nome = r.loja ${joinCondition} WHERE l.status = 'ativa' GROUP BY l.nome`;
        
        const result = await pool.query(sql, params);
        const ranking = result.rows.map(r => {
            const vendas_m_total = (Number(r.total_vendas_monitoramento) || 0) + (Number(r.total_omni) || 0);
            return { ...r, tx_loja: (r.total_clientes_loja > 0 ? (r.total_vendas_loja / r.total_clientes_loja) * 100 : 0), tx_monitoramento: (r.total_clientes_monitoramento > 0 ? (vendas_m_total / r.total_clientes_monitoramento) * 100 : 0) }
        });
        res.json(ranking);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/dashboard/chart-data', requirePageLogin, async (req, res) => {
    try {
        let whereClauses = []; let params = []; let paramIndex = 1;
        const { loja, data_inicio, data_fim } = req.query;

        if (loja && loja !== 'todas') { whereClauses.push(`loja = $${paramIndex++}`); params.push(loja); }
        if (data_inicio) { whereClauses.push(`data >= $${paramIndex++}`); params.push(data_inicio); }
        if (data_fim) { whereClauses.push(`data <= $${paramIndex++}`); params.push(data_fim); }
        
        const whereString = `WHERE ${whereClauses.join(' AND ')}`;
        const sql = `SELECT data, SUM(clientes_loja) as total_clientes_loja, SUM(vendas_loja) as total_vendas_loja, SUM(clientes_monitoramento) as total_clientes_monitoramento, SUM(vendas_monitoramento) as total_vendas_monitoramento, SUM(quantidade_omni) as total_omni FROM relatorios ${whereString} GROUP BY data ORDER BY data ASC`;
        
        const result = await pool.query(sql, params);
        const labels = []; const txConversaoLoja = []; const txConversaoMonitoramento = [];
        result.rows.forEach(row => {
            labels.push(new Date(row.data).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', timeZone: 'UTC' }));
            const vendas_m_total = (Number(row.total_vendas_monitoramento) || 0) + (Number(row.total_omni) || 0);
            const tx_m = row.total_clientes_monitoramento > 0 ? (vendas_m_total / row.total_clientes_monitoramento) * 100 : 0;
            const tx_l = row.total_clientes_loja > 0 ? (row.total_vendas_loja / row.total_clientes_loja) * 100 : 0;
            txConversaoLoja.push(tx_l.toFixed(2));
            txConversaoMonitoramento.push(tx_m.toFixed(2));
        });
        res.json({ labels, txConversaoLoja, txConversaoMonitoramento });
    } catch (err) { res.status(500).json({ error: 'Erro ao buscar dados para o gráfico.' }); }
});

// APIs DE DEMANDAS
app.post('/api/demandas', requirePageLogin, async (req, res) => {
    const { loja_nome, descricao, tag } = req.body;
    try {
        const result = await pool.query('INSERT INTO demandas (loja_nome, descricao, tag, criado_por_usuario) VALUES ($1, $2, $3, $4) RETURNING id', [loja_nome, descricao, tag, req.session.username]);
        res.status(201).json({ success: true, id: result.rows[0].id });
    } catch (err) { res.status(500).json({ error: 'Falha ao salvar demanda.' }); }
});

app.get('/api/demandas/:status', requirePageLogin, async (req, res) => {
    const status = req.params.status === 'pendentes' ? 'pendente' : 'concluido';
    try {
        const result = await pool.query(`SELECT * FROM demandas WHERE status = $1 ORDER BY criado_em DESC`, [status]);
        res.json(result.rows);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

app.put('/api/demandas/:id/concluir', requirePageLogin, async (req, res) => {
    try {
        const result = await pool.query("UPDATE demandas SET status = 'concluido', concluido_por_usuario = $1, concluido_em = CURRENT_TIMESTAMP WHERE id = $2", [req.session.username, req.params.id]);
        if (result.rowCount === 0) return res.status(404).json({ error: 'Demanda não encontrada.' });
        res.json({ success: true });
    } catch (err) { res.status(500).json({ error: 'Erro ao concluir demanda.' }); }
});

app.delete('/api/demandas/:id', requirePageLogin, async (req, res) => {
    try {
        const result = await pool.query("DELETE FROM demandas WHERE id = $1", [req.params.id]);
        if (result.rowCount === 0) return res.status(404).json({ error: "Demanda não encontrada." });
        res.json({ success: true });
    } catch (err) { res.status(500).json({ error: 'Erro ao excluir demanda.' }); }
});

// =================================================================
// INICIALIZAÇÃO DO SERVIDOR
// =================================================================
app.listen(PORT, '0.0.0.0', () => {
    console.log(`Servidor rodando na porta ${PORT}`);
    initializeDatabase();
});