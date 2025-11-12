// =================================================================
// SERVIÇO DVR/NVR - Funções auxiliares para gerenciamento de DVRs
// =================================================================

class DVRService {
    constructor(db) {
        this.db = db;
    }

    // CRUD - Dispositivos DVR/NVR
    async criarDispositivo(dados) {
        const { nome, loja_id, loja_nome, ip_address, porta, usuario, modelo, canais_total, observacoes } = dados;
        
        return new Promise((resolve, reject) => {
            const query = `
                INSERT INTO dvr_dispositivos 
                (nome, loja_id, loja_nome, ip_address, porta, usuario, modelo, canais_total, observacoes, status, updated_at)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, 'offline', CURRENT_TIMESTAMP)
            `;
            
            this.db.run(query, [nome, loja_id, loja_nome, ip_address, porta || 37777, usuario, modelo, canais_total || 0, observacoes], function(err) {
                if (err) return reject(err);
                resolve({ id: this.lastID });
            });
        });
    }

    async listarDispositivos(filtros = {}) {
        return new Promise((resolve, reject) => {
            let whereClauses = [];
            let params = [];

            if (filtros.loja_id) {
                whereClauses.push('loja_id = ?');
                params.push(filtros.loja_id);
            }

            if (filtros.loja_nome) {
                whereClauses.push('loja_nome LIKE ?');
                params.push(`%${filtros.loja_nome}%`);
            }

            if (filtros.status) {
                whereClauses.push('status = ?');
                params.push(filtros.status);
            }

            const whereString = whereClauses.length > 0 ? ' WHERE ' + whereClauses.join(' AND ') : '';
            const query = `SELECT * FROM dvr_dispositivos${whereString} ORDER BY nome ASC`;

            this.db.all(query, params, (err, rows) => {
                if (err) return reject(err);
                resolve(rows || []);
            });
        });
    }

    async obterDispositivo(id) {
        return new Promise((resolve, reject) => {
            this.db.get('SELECT * FROM dvr_dispositivos WHERE id = ?', [id], (err, row) => {
                if (err) return reject(err);
                resolve(row);
            });
        });
    }

    async atualizarDispositivo(id, dados) {
        const { nome, loja_id, loja_nome, ip_address, porta, usuario, modelo, canais_total, status, observacoes } = dados;
        
        return new Promise((resolve, reject) => {
            const query = `
                UPDATE dvr_dispositivos 
                SET nome = ?, loja_id = ?, loja_nome = ?, ip_address = ?, porta = ?, 
                    usuario = ?, modelo = ?, canais_total = ?, status = ?, observacoes = ?,
                    updated_at = CURRENT_TIMESTAMP
                WHERE id = ?
            `;
            
            this.db.run(query, [nome, loja_id, loja_nome, ip_address, porta, usuario, modelo, canais_total, status, observacoes, id], function(err) {
                if (err) return reject(err);
                if (this.changes === 0) return reject(new Error('Dispositivo não encontrado'));
                resolve({ success: true });
            });
        });
    }

    async excluirDispositivo(id) {
        return new Promise((resolve, reject) => {
            this.db.run('DELETE FROM dvr_dispositivos WHERE id = ?', [id], function(err) {
                if (err) return reject(err);
                if (this.changes === 0) return reject(new Error('Dispositivo não encontrado'));
                resolve({ success: true });
            });
        });
    }

    // CRUD - Logs de DVR
    async registrarLog(dados) {
        const { dvr_id, dvr_nome, loja_nome, tipo_evento, descricao, canal, severidade, detalhes_json } = dados;
        
        return new Promise((resolve, reject) => {
            const query = `
                INSERT INTO dvr_logs 
                (dvr_id, dvr_nome, loja_nome, tipo_evento, descricao, canal, severidade, detalhes_json, data_hora)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, CURRENT_TIMESTAMP)
            `;
            
            this.db.run(query, [dvr_id, dvr_nome, loja_nome, tipo_evento, descricao, canal, severidade || 'info', detalhes_json], function(err) {
                if (err) return reject(err);
                resolve({ id: this.lastID });
            });
        });
    }

    async listarLogs(filtros = {}, paginacao = {}) {
        return new Promise((resolve, reject) => {
            let whereClauses = [];
            let params = [];

            if (filtros.dvr_id) {
                whereClauses.push('dvr_id = ?');
                params.push(filtros.dvr_id);
            }

            if (filtros.loja_nome) {
                whereClauses.push('loja_nome LIKE ?');
                params.push(`%${filtros.loja_nome}%`);
            }

            if (filtros.tipo_evento) {
                whereClauses.push('tipo_evento = ?');
                params.push(filtros.tipo_evento);
            }

            if (filtros.severidade) {
                whereClauses.push('severidade = ?');
                params.push(filtros.severidade);
            }

            if (filtros.data_inicio) {
                whereClauses.push('data_hora >= ?');
                params.push(filtros.data_inicio);
            }

            if (filtros.data_fim) {
                whereClauses.push('data_hora <= ?');
                params.push(filtros.data_fim);
            }

            const whereString = whereClauses.length > 0 ? ' WHERE ' + whereClauses.join(' AND ') : '';
            const limit = paginacao.limit || 100;
            const offset = paginacao.offset || 0;
            
            const query = `SELECT * FROM dvr_logs${whereString} ORDER BY data_hora DESC LIMIT ? OFFSET ?`;
            params.push(limit, offset);

            this.db.all(query, params, (err, rows) => {
                if (err) return reject(err);
                resolve(rows || []);
            });
        });
    }

    async excluirLog(id) {
        return new Promise((resolve, reject) => {
            this.db.run('DELETE FROM dvr_logs WHERE id = ?', [id], function(err) {
                if (err) return reject(err);
                if (this.changes === 0) return reject(new Error('Log não encontrado'));
                resolve({ success: true });
            });
        });
    }

    // CRUD - Arquivos de DVR
    async registrarArquivo(dados) {
        const { dvr_id, dvr_nome, loja_nome, tipo_arquivo, nome_arquivo, caminho_arquivo, 
                tamanho_bytes, data_geracao, canal, inicio_gravacao, fim_gravacao, 
                descricao, uploaded_by } = dados;
        
        return new Promise((resolve, reject) => {
            const query = `
                INSERT INTO dvr_arquivos 
                (dvr_id, dvr_nome, loja_nome, tipo_arquivo, nome_arquivo, caminho_arquivo, 
                 tamanho_bytes, data_geracao, canal, inicio_gravacao, fim_gravacao, 
                 descricao, uploaded_by, uploaded_at)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, CURRENT_TIMESTAMP)
            `;
            
            this.db.run(query, [dvr_id, dvr_nome, loja_nome, tipo_arquivo, nome_arquivo, caminho_arquivo,
                                tamanho_bytes, data_geracao, canal, inicio_gravacao, fim_gravacao,
                                descricao, uploaded_by], function(err) {
                if (err) return reject(err);
                resolve({ id: this.lastID });
            });
        });
    }

    async listarArquivos(filtros = {}, paginacao = {}) {
        return new Promise((resolve, reject) => {
            let whereClauses = [];
            let params = [];

            if (filtros.dvr_id) {
                whereClauses.push('dvr_id = ?');
                params.push(filtros.dvr_id);
            }

            if (filtros.loja_nome) {
                whereClauses.push('loja_nome LIKE ?');
                params.push(`%${filtros.loja_nome}%`);
            }

            if (filtros.tipo_arquivo) {
                whereClauses.push('tipo_arquivo = ?');
                params.push(filtros.tipo_arquivo);
            }

            if (filtros.data_inicio) {
                whereClauses.push('data_geracao >= ?');
                params.push(filtros.data_inicio);
            }

            if (filtros.data_fim) {
                whereClauses.push('data_geracao <= ?');
                params.push(filtros.data_fim);
            }

            const whereString = whereClauses.length > 0 ? ' WHERE ' + whereClauses.join(' AND ') : '';
            const limit = paginacao.limit || 50;
            const offset = paginacao.offset || 0;
            
            const query = `SELECT * FROM dvr_arquivos${whereString} ORDER BY data_geracao DESC, uploaded_at DESC LIMIT ? OFFSET ?`;
            params.push(limit, offset);

            this.db.all(query, params, (err, rows) => {
                if (err) return reject(err);
                resolve(rows || []);
            });
        });
    }

    async obterArquivo(id) {
        return new Promise((resolve, reject) => {
            this.db.get('SELECT * FROM dvr_arquivos WHERE id = ?', [id], (err, row) => {
                if (err) return reject(err);
                resolve(row);
            });
        });
    }

    async excluirArquivo(id) {
        return new Promise((resolve, reject) => {
            this.db.run('DELETE FROM dvr_arquivos WHERE id = ?', [id], function(err) {
                if (err) return reject(err);
                if (this.changes === 0) return reject(new Error('Arquivo não encontrado'));
                resolve({ success: true });
            });
        });
    }

    // Atualizar status do dispositivo
    async atualizarStatus(id, status) {
        return new Promise((resolve, reject) => {
            const query = `
                UPDATE dvr_dispositivos 
                SET status = ?, ultima_conexao = CURRENT_TIMESTAMP, updated_at = CURRENT_TIMESTAMP
                WHERE id = ?
            `;
            
            this.db.run(query, [status, id], function(err) {
                if (err) return reject(err);
                if (this.changes === 0) return reject(new Error('Dispositivo não encontrado'));
                resolve({ success: true });
            });
        });
    }
}

module.exports = DVRService;
