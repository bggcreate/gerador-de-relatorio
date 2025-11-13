require('dotenv').config();
const { google } = require('googleapis');
const http = require('http');
const url = require('url');

async function configurarAutenticacao() {
  console.log('\n🔐 CONFIGURAÇÃO DO GOOGLE DRIVE - OBTER REFRESH TOKEN\n');
  console.log('═'.repeat(60));

  const clientId = process.env.GOOGLE_CLIENT_ID;
  const clientSecret = process.env.GOOGLE_CLIENT_SECRET;

  if (!clientId || !clientSecret) {
    console.error('\n❌ ERRO: Credenciais não encontradas!');
    console.log('\n📝 Configure primeiro as variáveis de ambiente com:');
    console.log('   GOOGLE_CLIENT_ID=seu_client_id_aqui');
    console.log('   GOOGLE_CLIENT_SECRET=sua_secret_aqui\n');
    console.log('📖 Veja o arquivo GOOGLE_DRIVE_SETUP.md para instruções completas\n');
    process.exit(1);
  }

  const REDIRECT_URI = 'http://localhost:3000/oauth2callback';
  
  const oauth2Client = new google.auth.OAuth2(
    clientId,
    clientSecret,
    REDIRECT_URI
  );

  const scopes = [
    'https://www.googleapis.com/auth/drive.file',
    'https://www.googleapis.com/auth/gmail.send'
  ];

  const authUrl = oauth2Client.generateAuthUrl({
    access_type: 'offline',
    scope: scopes,
    prompt: 'consent'
  });

  console.log('\n📋 SIGA ESTES PASSOS:\n');
  console.log('═'.repeat(60));
  console.log('\n1️⃣  Um servidor local está rodando na porta 3000');
  console.log('2️⃣  Acesse esta URL no seu navegador:\n');
  console.log('🔗 ' + authUrl + '\n');
  console.log('═'.repeat(60));
  console.log('\n3️⃣  Faça login com sua conta Google');
  console.log('4️⃣  Autorize o aplicativo');
  console.log('5️⃣  Aguarde o redirecionamento automático...\n');
  console.log('═'.repeat(60));

  const server = http.createServer(async (req, res) => {
    try {
      const queryData = url.parse(req.url, true).query;
      
      if (queryData.code) {
        const code = queryData.code;
        
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`
          <!DOCTYPE html>
          <html>
          <head>
            <meta charset="utf-8">
            <title>Autorização Concluída</title>
            <style>
              body {
                font-family: Arial, sans-serif;
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
              }
              .container {
                background: white;
                padding: 40px;
                border-radius: 10px;
                box-shadow: 0 10px 40px rgba(0,0,0,0.3);
                text-align: center;
                max-width: 500px;
              }
              h1 { color: #4CAF50; margin-bottom: 20px; }
              p { color: #555; line-height: 1.6; }
              .success { font-size: 60px; margin-bottom: 20px; }
            </style>
          </head>
          <body>
            <div class="container">
              <div class="success">✅</div>
              <h1>Autorização Concluída!</h1>
              <p>Você já pode fechar esta janela.</p>
              <p>Volte ao terminal para ver o REFRESH_TOKEN.</p>
            </div>
          </body>
          </html>
        `);

        try {
          const { tokens } = await oauth2Client.getToken(code);
          
          console.log('\n\n✅ SUCESSO! Token obtido com sucesso!\n');
          console.log('═'.repeat(60));
          console.log('\n📝 Configure estas variáveis de ambiente (Secrets):\n');
          console.log(`GOOGLE_CLIENT_ID=${clientId}`);
          console.log(`GOOGLE_CLIENT_SECRET=${clientSecret}`);
          console.log(`GOOGLE_REFRESH_TOKEN=${tokens.refresh_token}\n`);
          console.log('═'.repeat(60));
          console.log('\n✅ Adicione essas credenciais nas Secrets do Replit');
          console.log('✅ Depois disso, o sistema estará pronto para usar o Google Drive!\n');
          
          server.close();
          process.exit(0);
        } catch (error) {
          console.error('\n❌ ERRO ao obter token:', error.message);
          server.close();
          process.exit(1);
        }
      }
    } catch (error) {
      console.error('Erro ao processar callback:', error);
      res.writeHead(500, { 'Content-Type': 'text/plain' });
      res.end('Erro ao processar autorização');
    }
  });

  server.listen(3000, () => {
    console.log('\n🌐 Servidor local iniciado na porta 3000');
    console.log('⏳ Aguardando autorização...\n');
  });
}

configurarAutenticacao().catch(console.error);
