const sqlite3 = require('sqlite3').verbose();
const { query } = require('../src/config/postgresql');
const path = require('path');

const SQLITE_BACKUP_PATH = path.join(__dirname, '..', 'attached_assets', 'backup_reports_2025-11-03_1763039637294.db');

async function importData() {
    console.log('🔄 Iniciando importação do backup SQLite para PostgreSQL...\n');
    
    return new Promise((resolve, reject) => {
        const db = new sqlite3.Database(SQLITE_BACKUP_PATH, sqlite3.OPEN_READONLY, async (err) => {
            if (err) {
                console.error('❌ Erro ao abrir banco SQLite:', err.message);
                reject(err);
                return;
            }
            
            console.log('✅ Banco SQLite aberto com sucesso\n');
            
            try {
                // Importar usuários
                await importTable(db, 'usuarios', [
                    'id', 'username', 'password', 'role', 'loja_gerente', 
                    'lojas_consultor', 'loja_tecnico', 'password_hashed'
                ]);
                
                // Importar lojas
                await importTable(db, 'lojas', [
                    'id', 'nome', 'status', 'funcao_especial', 'observacoes',
                    'tecnico_username', 'cargo', 'cep', 'numero_contato', 'gerente'
                ]);
                
                // Importar relatórios
                await importTable(db, 'relatorios', [
                    'id', 'loja', 'data', 'hora_abertura', 'hora_fechamento',
                    'gerente_entrada', 'gerente_saida', 'clientes_monitoramento',
                    'vendas_monitoramento', 'clientes_loja', 'vendas_loja',
                    'total_vendas_dinheiro', 'ticket_medio', 'pa', 'quantidade_trocas',
                    'nome_funcao_especial', 'quantidade_funcao_especial', 'quantidade_omni',
                    'vendedores', 'nome_arquivo', 'enviado_por_usuario', 'enviado_em',
                    'vendas_cartao', 'vendas_pix', 'vendas_dinheiro'
                ]);
                
                // Importar demandas
                await importTable(db, 'demandas', [
                    'id', 'loja_nome', 'descricao', 'tag', 'status',
                    'criado_por_usuario', 'concluido_por_usuario', 'criado_em', 'concluido_em'
                ]);
                
                // Importar vendedores
                await importTable(db, 'vendedores', [
                    'id', 'loja_id', 'nome', 'telefone', 'data_entrada',
                    'data_demissao', 'previsao_entrada', 'previsao_saida', 'ativo', 'created_at'
                ]);
                
                // Importar logs
                await importTable(db, 'logs', [
                    'id', 'timestamp', 'type', 'username', 'action', 'details', 'ip_address'
                ]);
                
                // Importar estoque técnico
                await importTable(db, 'estoque_tecnico', [
                    'id', 'nome_peca', 'codigo_interno', 'quantidade', 'valor_custo',
                    'loja', 'created_at', 'updated_at'
                ]);
                
                // Importar assistências
                await importTable(db, 'assistencias', [
                    'id', 'cliente_nome', 'cliente_cpf', 'numero_pedido', 'data_entrada',
                    'data_conclusao', 'valor_peca_loja', 'valor_servico_cliente', 'aparelho',
                    'peca_id', 'peca_nome', 'observacoes', 'status', 'tecnico_responsavel',
                    'loja', 'created_at', 'updated_at'
                ]);
                
                console.log('\n✅ Importação concluída com sucesso!');
                db.close();
                resolve();
                
            } catch (error) {
                console.error('❌ Erro durante importação:', error);
                db.close();
                reject(error);
            }
        });
    });
}

function importTable(db, tableName, columns) {
    return new Promise((resolve, reject) => {
        db.all(`SELECT * FROM ${tableName}`, async (err, rows) => {
            if (err) {
                if (err.message.includes('no such table')) {
                    console.log(`⚠️  Tabela ${tableName} não existe no backup - pulando`);
                    resolve();
                    return;
                }
                console.error(`❌ Erro ao ler tabela ${tableName}:`, err.message);
                reject(err);
                return;
            }
            
            if (!rows || rows.length === 0) {
                console.log(`ℹ️  Tabela ${tableName}: 0 registros (vazia)`);
                resolve();
                return;
            }
            
            console.log(`📦 Importando ${rows.length} registros da tabela ${tableName}...`);
            
            try {
                for (const row of rows) {
                    const validColumns = columns.filter(col => row.hasOwnProperty(col));
                    const values = validColumns.map(col => row[col]);
                    const placeholders = validColumns.map((_, i) => `$${i + 1}`).join(', ');
                    const columnList = validColumns.join(', ');
                    
                    const insertSQL = `
                        INSERT INTO ${tableName} (${columnList})
                        VALUES (${placeholders})
                        ON CONFLICT (id) DO UPDATE SET
                        ${validColumns.filter(c => c !== 'id').map((col, i) => `${col} = EXCLUDED.${col}`).join(', ')}
                    `;
                    
                    await query(insertSQL, values);
                }
                
                // Atualizar a sequência do ID
                await query(`
                    SELECT setval(pg_get_serial_sequence('${tableName}', 'id'), 
                    COALESCE((SELECT MAX(id) FROM ${tableName}), 1), true)
                `);
                
                console.log(`   ✅ ${tableName}: ${rows.length} registros importados`);
                resolve();
                
            } catch (error) {
                console.error(`   ❌ Erro ao inserir em ${tableName}:`, error.message);
                reject(error);
            }
        });
    });
}

// Executar importação
importData()
    .then(() => {
        console.log('\n🎉 Todos os dados foram importados!');
        process.exit(0);
    })
    .catch((error) => {
        console.error('\n❌ Falha na importação:', error);
        process.exit(1);
    });
