require('dotenv').config();
const sqlite3 = require('sqlite3').verbose();
const path = require('path');
const fs = require('fs');
const GoogleDriveService = require('../services/googleDriveService');

const SYNC_FILE = 'database-sync.db';
const DATA_DIR = path.join(process.cwd(), 'data');
const LOCAL_DB = path.join(DATA_DIR, 'database.db');
const SYNC_STATE_FILE = path.join(DATA_DIR, 'last-sync.json');

class DatabaseSync {
  constructor() {
    this.driveService = new GoogleDriveService();
    this.db = null;
  }

  async initialize() {
    if (!fs.existsSync(DATA_DIR)) {
      fs.mkdirSync(DATA_DIR, { recursive: true });
      console.log('📁 Diretório data/ criado');
    }

    const authenticated = await this.driveService.autenticar();
    if (!authenticated) {
      throw new Error('Falha na autenticação com Google Drive. Verifique suas credenciais no arquivo .env');
    }

    this.db = new sqlite3.Database(LOCAL_DB, (err) => {
      if (err) {
        console.error('❌ Erro ao conectar no banco local:', err.message);
        throw err;
      }
      console.log('✅ Conectado ao banco de dados local');
    });
  }

  async getLastSyncTime() {
    try {
      if (fs.existsSync(SYNC_STATE_FILE)) {
        const data = JSON.parse(fs.readFileSync(SYNC_STATE_FILE, 'utf8'));
        return new Date(data.lastSync);
      }
    } catch (error) {
      console.log('⚠️  Primeira sincronização');
    }
    return null;
  }

  async updateSyncTime() {
    fs.writeFileSync(SYNC_STATE_FILE, JSON.stringify({
      lastSync: new Date().toISOString(),
      timestamp: Date.now()
    }));
  }

  async downloadFromDrive() {
    console.log('📥 Baixando dados do Google Drive...');

    try {
      const relatorios = await this.driveService.listarRelatorios();
      
      if (!relatorios || relatorios.length === 0) {
        console.log('ℹ️  Nenhum relatório encontrado no Google Drive');
        return { downloaded: 0, skipped: 0 };
      }

      console.log(`📊 Encontrados ${relatorios.length} relatórios no Drive`);

      let downloaded = 0;
      let skipped = 0;

      for (const relatorio of relatorios) {
        try {
          const exists = await this.checkRelatorioExists(relatorio.id);
          
          if (exists) {
            skipped++;
            continue;
          }

          const conteudo = await this.driveService.lerRelatorio(relatorio.fileId);
          await this.saveRelatorioToLocal(conteudo);
          downloaded++;

          if (downloaded % 10 === 0) {
            console.log(`  ✓ ${downloaded} relatórios baixados...`);
          }
        } catch (error) {
          console.error(`  ✗ Erro ao baixar relatório ${relatorio.nome}:`, error.message);
        }
      }

      return { downloaded, skipped };
    } catch (error) {
      console.error('❌ Erro ao baixar do Drive:', error.message);
      throw error;
    }
  }

  async uploadToDrive() {
    console.log('📤 Enviando dados para Google Drive...');

    try {
      const localReports = await this.getLocalReports();
      
      if (!localReports || localReports.length === 0) {
        console.log('ℹ️  Nenhum relatório local para enviar');
        return { uploaded: 0, skipped: 0 };
      }

      console.log(`📊 Encontrados ${localReports.length} relatórios locais`);

      let uploaded = 0;
      let skipped = 0;

      for (const report of localReports) {
        try {
          const existsInDrive = await this.driveService.buscarRelatorioPorId(report.id);
          
          if (existsInDrive) {
            skipped++;
            continue;
          }

          await this.driveService.salvarRelatorio(report);
          uploaded++;

          if (uploaded % 10 === 0) {
            console.log(`  ✓ ${uploaded} relatórios enviados...`);
          }
        } catch (error) {
          console.error(`  ✗ Erro ao enviar relatório #${report.id}:`, error.message);
        }
      }

      return { uploaded, skipped };
    } catch (error) {
      console.error('❌ Erro ao enviar para Drive:', error.message);
      throw error;
    }
  }

  checkRelatorioExists(relatorioId) {
    return new Promise((resolve, reject) => {
      this.db.get(
        'SELECT id FROM relatorios WHERE id = ?',
        [relatorioId],
        (err, row) => {
          if (err) reject(err);
          else resolve(!!row);
        }
      );
    });
  }

  saveRelatorioToLocal(relatorio) {
    return new Promise((resolve, reject) => {
      const sql = `
        INSERT OR REPLACE INTO relatorios 
        (id, loja_id, data_relatorio, clientes_monitoramento, vendas_monitoramento, 
         vendas_omni, tx_conversao, clientes_loja, vendas_loja, tx_conversao_loja, 
         observacoes, created_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
      `;

      this.db.run(
        sql,
        [
          relatorio.id,
          relatorio.loja_id,
          relatorio.data_relatorio,
          relatorio.clientes_monitoramento,
          relatorio.vendas_monitoramento,
          relatorio.vendas_omni,
          relatorio.tx_conversao,
          relatorio.clientes_loja,
          relatorio.vendas_loja,
          relatorio.tx_conversao_loja,
          relatorio.observacoes,
          relatorio.created_at
        ],
        (err) => {
          if (err) reject(err);
          else resolve();
        }
      );
    });
  }

  getLocalReports() {
    return new Promise((resolve, reject) => {
      this.db.all(
        'SELECT * FROM relatorios ORDER BY created_at DESC',
        (err, rows) => {
          if (err) reject(err);
          else resolve(rows || []);
        }
      );
    });
  }

  async syncBidirectional() {
    console.log('\n🔄 Iniciando sincronização bidirecional...\n');

    const lastSync = await this.getLastSyncTime();
    if (lastSync) {
      console.log(`⏱️  Última sincronização: ${lastSync.toLocaleString('pt-BR')}\n`);
    }

    const downloadResult = await this.downloadFromDrive();
    console.log(`\n📥 Download: ${downloadResult.downloaded} novos, ${downloadResult.skipped} já existentes`);

    const uploadResult = await this.uploadToDrive();
    console.log(`📤 Upload: ${uploadResult.uploaded} enviados, ${uploadResult.skipped} já existentes\n`);

    await this.updateSyncTime();
    console.log('✅ Sincronização concluída com sucesso!\n');

    return {
      download: downloadResult,
      upload: uploadResult,
      timestamp: new Date()
    };
  }

  async showQuota() {
    const quota = await this.driveService.verificarQuota();
    if (quota) {
      console.log('\n📊 Quota do Google Drive:');
      console.log(`   Usado: ${quota.usadoGB} GB de ${quota.limiteGB} GB (${quota.percentual}%)`);
      console.log(`   Disponível: ${(quota.limiteGB - quota.usadoGB).toFixed(2)} GB`);
      
      if (quota.precisaBackup) {
        console.log('\n⚠️  ATENÇÃO: Você está perto do limite! Considere fazer backup.');
      }
      console.log('');
    }
  }

  close() {
    if (this.db) {
      this.db.close();
    }
  }
}

async function main() {
  const sync = new DatabaseSync();

  try {
    console.log('═══════════════════════════════════════════════════');
    console.log('  🔄 SINCRONIZAÇÃO DE BANCO DE DADOS - GOOGLE DRIVE');
    console.log('═══════════════════════════════════════════════════\n');

    await sync.initialize();
    await sync.showQuota();
    await sync.syncBidirectional();

    console.log('═══════════════════════════════════════════════════');
    console.log('  ✅ SINCRONIZAÇÃO FINALIZADA');
    console.log('═══════════════════════════════════════════════════\n');

  } catch (error) {
    console.error('\n❌ ERRO NA SINCRONIZAÇÃO:', error.message);
    console.error('\nVerifique:');
    console.error('  1. Arquivo .env está configurado corretamente');
    console.error('  2. Credenciais do Google Drive são válidas');
    console.error('  3. Google Drive API e Gmail API estão ativadas');
    console.error('  4. Conexão com a internet está funcionando\n');
    process.exit(1);
  } finally {
    sync.close();
  }
}

if (require.main === module) {
  main();
}

module.exports = DatabaseSync;
