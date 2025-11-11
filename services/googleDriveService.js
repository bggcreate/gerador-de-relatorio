const { google } = require('googleapis');
const fs = require('fs');
const path = require('path');
const archiver = require('archiver');

class GoogleDriveService {
  constructor() {
    this.drive = null;
    this.gmail = null;
    this.auth = null;
    this.pastaRelatorios = null;
    this.limiteGB = 13; // Limite de segurança antes de fazer backup (13GB de 15GB)
  }

  async autenticar() {
    try {
      const clientId = process.env.GOOGLE_CLIENT_ID;
      const clientSecret = process.env.GOOGLE_CLIENT_SECRET;
      const refreshToken = process.env.GOOGLE_REFRESH_TOKEN;

      if (!clientId || !clientSecret || !refreshToken) {
        throw new Error('Credenciais do Google Drive não configuradas. Configure GOOGLE_CLIENT_ID, GOOGLE_CLIENT_SECRET e GOOGLE_REFRESH_TOKEN no arquivo .env');
      }

      const oauth2Client = new google.auth.OAuth2(
        clientId,
        clientSecret,
        'urn:ietf:wg:oauth:2.0:oob'
      );

      oauth2Client.setCredentials({
        refresh_token: refreshToken
      });

      this.auth = oauth2Client;
      this.drive = google.drive({ version: 'v3', auth: oauth2Client });
      this.gmail = google.gmail({ version: 'v1', auth: oauth2Client });

      // Criar/encontrar pasta de relatórios
      await this.criarPastaRelatorios();

      console.log('✅ Autenticado no Google Drive com sucesso!');
      return true;
    } catch (error) {
      console.error('❌ Erro ao autenticar no Google Drive:', error.message);
      return false;
    }
  }

  async criarPastaRelatorios() {
    try {
      // Buscar pasta existente
      const response = await this.drive.files.list({
        q: "name='Sistema_Relatorios' and mimeType='application/vnd.google-apps.folder' and trashed=false",
        fields: 'files(id, name)',
        spaces: 'drive'
      });

      if (response.data.files && response.data.files.length > 0) {
        this.pastaRelatorios = response.data.files[0].id;
        console.log('📁 Pasta Sistema_Relatorios encontrada:', this.pastaRelatorios);
      } else {
        // Criar nova pasta
        const fileMetadata = {
          name: 'Sistema_Relatorios',
          mimeType: 'application/vnd.google-apps.folder'
        };

        const folder = await this.drive.files.create({
          resource: fileMetadata,
          fields: 'id'
        });

        this.pastaRelatorios = folder.data.id;
        console.log('📁 Pasta Sistema_Relatorios criada:', this.pastaRelatorios);
      }
    } catch (error) {
      console.error('❌ Erro ao criar/buscar pasta:', error.message);
      throw error;
    }
  }

  async verificarQuota() {
    try {
      const response = await this.drive.about.get({
        fields: 'storageQuota'
      });

      const quota = response.data.storageQuota;
      const usadoGB = (quota.usage / (1024 ** 3)).toFixed(2);
      const limiteGB = (quota.limit / (1024 ** 3)).toFixed(2);
      const percentual = ((quota.usage / quota.limit) * 100).toFixed(1);

      return {
        usado: parseInt(quota.usage),
        limite: parseInt(quota.limit),
        usadoGB: parseFloat(usadoGB),
        limiteGB: parseFloat(limiteGB),
        percentual: parseFloat(percentual),
        precisaBackup: parseFloat(usadoGB) >= this.limiteGB
      };
    } catch (error) {
      console.error('❌ Erro ao verificar quota:', error.message);
      return null;
    }
  }

  async salvarRelatorio(relatorio) {
    try {
      if (!this.drive) {
        throw new Error('Google Drive não autenticado. Execute autenticar() primeiro.');
      }

      // Criar estrutura de pastas por ano/mês
      const data = new Date(relatorio.data_relatorio || new Date());
      const ano = data.getFullYear();
      const mes = String(data.getMonth() + 1).padStart(2, '0');
      const mesNome = [
        'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
        'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
      ][data.getMonth()];

      // Criar/buscar pasta do ano
      const pastaAno = await this.criarOuBuscarPasta(ano.toString(), this.pastaRelatorios);
      
      // Criar/buscar pasta do mês
      const pastaMes = await this.criarOuBuscarPasta(`${mes}-${mesNome}`, pastaAno);

      // Nome do arquivo
      const nomeArquivo = `relatorio_${relatorio.loja_nome?.replace(/[^a-zA-Z0-9]/g, '_')}_${relatorio.data_relatorio}_${Date.now()}.json`;

      // Upload do arquivo
      const fileMetadata = {
        name: nomeArquivo,
        parents: [pastaMes]
      };

      const media = {
        mimeType: 'application/json',
        body: JSON.stringify(relatorio, null, 2)
      };

      const file = await this.drive.files.create({
        resource: fileMetadata,
        media: media,
        fields: 'id, name, createdTime, size'
      });

      console.log(`✅ Relatório salvo no Drive: ${nomeArquivo}`);
      
      return {
        id: file.data.id,
        nome: file.data.name,
        dataCriacao: file.data.createdTime,
        tamanho: file.data.size
      };
    } catch (error) {
      console.error('❌ Erro ao salvar relatório:', error.message);
      throw error;
    }
  }

  async criarOuBuscarPasta(nomePasta, pastaParent) {
    try {
      // Buscar pasta existente
      const response = await this.drive.files.list({
        q: `name='${nomePasta}' and '${pastaParent}' in parents and mimeType='application/vnd.google-apps.folder' and trashed=false`,
        fields: 'files(id, name)',
        spaces: 'drive'
      });

      if (response.data.files && response.data.files.length > 0) {
        return response.data.files[0].id;
      }

      // Criar nova pasta
      const fileMetadata = {
        name: nomePasta,
        mimeType: 'application/vnd.google-apps.folder',
        parents: [pastaParent]
      };

      const folder = await this.drive.files.create({
        resource: fileMetadata,
        fields: 'id'
      });

      return folder.data.id;
    } catch (error) {
      console.error(`❌ Erro ao criar/buscar pasta ${nomePasta}:`, error.message);
      throw error;
    }
  }

  async listarRelatorios(filtros = {}) {
    try {
      // Buscar todos os arquivos JSON recursivamente dentro da pasta Sistema_Relatorios
      // Estratégia: buscar todas as pastas de ano, depois todas as pastas de mês, depois todos os JSONs
      
      const arquivos = [];
      
      // Se filtro por ano específico
      if (filtros.ano) {
        const pastaAno = await this.buscarPasta(filtros.ano.toString(), this.pastaRelatorios);
        if (pastaAno) {
          const arquivosDoAno = await this.listarArquivosRecursivo(pastaAno);
          arquivos.push(...arquivosDoAno);
        }
      } else {
        // Buscar em todos os anos
        arquivos.push(...await this.listarArquivosRecursivo(this.pastaRelatorios));
      }

      // Ordenar por data de criação (mais recentes primeiro)
      arquivos.sort((a, b) => new Date(b.dataCriacao) - new Date(a.dataCriacao));

      return arquivos;
    } catch (error) {
      console.error('❌ Erro ao listar relatórios:', error.message);
      return [];
    }
  }

  async listarArquivosRecursivo(pastaId) {
    try {
      const arquivos = [];
      
      // Listar tudo nesta pasta (arquivos e subpastas)
      const response = await this.drive.files.list({
        q: `'${pastaId}' in parents and trashed=false`,
        fields: 'files(id, name, mimeType, createdTime, size)',
        pageSize: 1000
      });

      for (const item of response.data.files || []) {
        if (item.mimeType === 'application/vnd.google-apps.folder') {
          // É uma pasta, buscar recursivamente
          const subArquivos = await this.listarArquivosRecursivo(item.id);
          arquivos.push(...subArquivos);
        } else if (item.mimeType === 'application/json' || item.name.endsWith('.json')) {
          // É um arquivo JSON, ler conteúdo
          const conteudo = await this.lerRelatorio(item.id);
          if (conteudo) {
            arquivos.push({
              ...conteudo,
              driveId: item.id,
              nomeArquivo: item.name,
              dataCriacao: item.createdTime,
              tamanho: item.size
            });
          }
        }
      }

      return arquivos;
    } catch (error) {
      console.error('❌ Erro ao listar arquivos recursivo:', error.message);
      return [];
    }
  }

  async buscarPasta(nomePasta, pastaParent) {
    try {
      const response = await this.drive.files.list({
        q: `name='${nomePasta}' and '${pastaParent}' in parents and mimeType='application/vnd.google-apps.folder' and trashed=false`,
        fields: 'files(id)',
        spaces: 'drive'
      });

      return response.data.files && response.data.files.length > 0 
        ? response.data.files[0].id 
        : null;
    } catch (error) {
      return null;
    }
  }

  async lerRelatorio(fileId) {
    try {
      const response = await this.drive.files.get({
        fileId: fileId,
        alt: 'media'
      });

      return response.data;
    } catch (error) {
      console.error(`❌ Erro ao ler relatório ${fileId}:`, error.message);
      return null;
    }
  }

  async fazerBackup(emailDestino) {
    try {
      console.log('📦 Iniciando backup do Google Drive...');

      // Baixar todos os arquivos
      const arquivos = await this.listarRelatorios();
      
      if (arquivos.length === 0) {
        throw new Error('Nenhum relatório encontrado para fazer backup');
      }

      // Criar arquivo ZIP temporário
      const backupDir = path.join(__dirname, '..', 'data', 'backups');
      if (!fs.existsSync(backupDir)) {
        fs.mkdirSync(backupDir, { recursive: true });
      }

      const dataAtual = new Date().toISOString().split('T')[0];
      const zipPath = path.join(backupDir, `backup_relatorios_${dataAtual}.zip`);
      
      const output = fs.createWriteStream(zipPath);
      const archive = archiver('zip', { zlib: { level: 9 } });

      output.on('close', async () => {
        console.log(`✅ Backup criado: ${(archive.pointer() / 1024 / 1024).toFixed(2)} MB`);
        
        // Enviar por email
        await this.enviarBackupPorEmail(zipPath, emailDestino);

        // Remover arquivo temporário
        fs.unlinkSync(zipPath);
      });

      archive.on('error', (err) => {
        throw err;
      });

      archive.pipe(output);

      // Adicionar cada relatório ao ZIP
      for (const relatorio of arquivos) {
        const fileName = relatorio.nomeArquivo || `relatorio_${relatorio.id}.json`;
        archive.append(JSON.stringify(relatorio, null, 2), { name: fileName });
      }

      await archive.finalize();

      return {
        sucesso: true,
        totalArquivos: arquivos.length,
        tamanhoMB: (archive.pointer() / 1024 / 1024).toFixed(2)
      };
    } catch (error) {
      console.error('❌ Erro ao fazer backup:', error.message);
      throw error;
    }
  }

  async enviarBackupPorEmail(arquivoZip, emailDestino) {
    try {
      console.log('📧 Enviando backup por email...');

      const emailRemetente = process.env.EMAIL_REMETENTE || 'noreply@sistema.com';
      const dataAtual = new Date().toLocaleDateString('pt-BR');

      // Ler arquivo ZIP em base64
      const attachment = fs.readFileSync(arquivoZip).toString('base64');

      const message = [
        'Content-Type: multipart/mixed; boundary="boundary123"',
        'MIME-Version: 1.0',
        `To: ${emailDestino}`,
        `From: ${emailRemetente}`,
        `Subject: Backup Automático de Relatórios - ${dataAtual}`,
        '',
        '--boundary123',
        'Content-Type: text/html; charset="UTF-8"',
        '',
        '<h2>Backup Automático de Relatórios</h2>',
        `<p>Segue em anexo o backup dos relatórios gerado em ${dataAtual}.</p>`,
        '<p>Este backup foi gerado automaticamente pelo sistema pois o limite de armazenamento do Google Drive está próximo do máximo.</p>',
        '<p><strong>Importante:</strong> Salve este arquivo em um local seguro!</p>',
        '<hr>',
        '<p><small>Sistema de Monitoramento de Lojas</small></p>',
        '',
        '--boundary123',
        'Content-Type: application/zip',
        'Content-Transfer-Encoding: base64',
        `Content-Disposition: attachment; filename="${path.basename(arquivoZip)}"`,
        '',
        attachment,
        '--boundary123--'
      ].join('\n');

      const encodedMessage = Buffer.from(message)
        .toString('base64')
        .replace(/\+/g, '-')
        .replace(/\//g, '_')
        .replace(/=+$/, '');

      await this.gmail.users.messages.send({
        userId: 'me',
        requestBody: {
          raw: encodedMessage
        }
      });

      console.log(`✅ Backup enviado para ${emailDestino}`);
      return true;
    } catch (error) {
      console.error('❌ Erro ao enviar email:', error.message);
      throw error;
    }
  }

  async limparRelatoriosAntigos(diasManter = 90) {
    try {
      console.log(`🗑️ Limpando relatórios com mais de ${diasManter} dias...`);

      const dataLimite = new Date();
      dataLimite.setDate(dataLimite.getDate() - diasManter);

      // Listar todos os relatórios recursivamente
      const todosRelatorios = await this.listarRelatorios();
      
      let removidos = 0;
      for (const relatorio of todosRelatorios) {
        const dataCriacao = new Date(relatorio.dataCriacao);
        
        if (dataCriacao < dataLimite) {
          // Remover arquivo antigo
          await this.drive.files.delete({ fileId: relatorio.driveId });
          removidos++;
          console.log(`🗑️ Removido: ${relatorio.nomeArquivo} (${dataCriacao.toLocaleDateString('pt-BR')})`);
        }
      }

      console.log(`✅ ${removidos} relatórios antigos removidos`);
      return removidos;
    } catch (error) {
      console.error('❌ Erro ao limpar relatórios antigos:', error.message);
      throw error;
    }
  }

  async gerarURLAutorizacao() {
    const oauth2Client = new google.auth.OAuth2(
      process.env.GOOGLE_CLIENT_ID,
      process.env.GOOGLE_CLIENT_SECRET,
      'urn:ietf:wg:oauth:2.0:oob'
    );

    const scopes = [
      'https://www.googleapis.com/auth/drive.file',
      'https://www.googleapis.com/auth/gmail.send'
    ];

    const url = oauth2Client.generateAuthUrl({
      access_type: 'offline',
      scope: scopes
    });

    return url;
  }

  async trocarCodigoPorToken(codigo) {
    const oauth2Client = new google.auth.OAuth2(
      process.env.GOOGLE_CLIENT_ID,
      process.env.GOOGLE_CLIENT_SECRET,
      'urn:ietf:wg:oauth:2.0:oob'
    );

    const { tokens } = await oauth2Client.getToken(codigo);
    return tokens.refresh_token;
  }
}

module.exports = new GoogleDriveService();
