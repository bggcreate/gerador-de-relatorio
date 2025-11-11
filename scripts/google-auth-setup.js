require('dotenv').config();
const { google } = require('googleapis');
const readline = require('readline');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

async function configurarAutenticacao() {
  console.log('\n🔐 CONFIGURAÇÃO DO GOOGLE DRIVE - OBTER REFRESH TOKEN\n');
  console.log('═'.repeat(60));

  const clientId = process.env.GOOGLE_CLIENT_ID;
  const clientSecret = process.env.GOOGLE_CLIENT_SECRET;

  if (!clientId || !clientSecret) {
    console.error('\n❌ ERRO: Credenciais não encontradas!');
    console.log('\n📝 Configure primeiro o arquivo .env com:');
    console.log('   GOOGLE_CLIENT_ID=seu_client_id_aqui');
    console.log('   GOOGLE_CLIENT_SECRET=sua_secret_aqui\n');
    console.log('📖 Veja o arquivo GOOGLE_DRIVE_SETUP.md para instruções completas\n');
    process.exit(1);
  }

  const oauth2Client = new google.auth.OAuth2(
    clientId,
    clientSecret,
    'urn:ietf:wg:oauth:2.0:oob'
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

  console.log('\n📋 PASSO 1: Acesse esta URL no seu navegador:\n');
  console.log('🔗 ' + authUrl + '\n');
  console.log('═'.repeat(60));
  console.log('\n📋 PASSO 2: Faça login com sua conta Google');
  console.log('📋 PASSO 3: Autorize o aplicativo');
  console.log('📋 PASSO 4: Copie o código fornecido\n');
  console.log('═'.repeat(60));

  rl.question('\n✏️  Cole o código aqui: ', async (code) => {
    try {
      const { tokens } = await oauth2Client.getToken(code);
      
      console.log('\n✅ SUCESSO! Token obtido com sucesso!\n');
      console.log('═'.repeat(60));
      console.log('\n📝 Adicione esta linha no seu arquivo .env:\n');
      console.log(`GOOGLE_REFRESH_TOKEN=${tokens.refresh_token}\n`);
      console.log('═'.repeat(60));
      console.log('\n💡 SEU ARQUIVO .env DEVE FICAR ASSIM:\n');
      console.log(`GOOGLE_CLIENT_ID=${clientId}`);
      console.log(`GOOGLE_CLIENT_SECRET=${clientSecret}`);
      console.log(`GOOGLE_REFRESH_TOKEN=${tokens.refresh_token}`);
      console.log('EMAIL_REMETENTE=seu_email@gmail.com');
      console.log('EMAIL_BACKUP=seu_email@gmail.com\n');
      console.log('═'.repeat(60));
      console.log('\n✅ Após adicionar o token, execute: npm start\n');
      
    } catch (error) {
      console.error('\n❌ ERRO ao obter token:', error.message);
      console.log('\n💡 Verifique se:');
      console.log('   1. O código foi copiado corretamente');
      console.log('   2. As credenciais no .env estão corretas');
      console.log('   3. As APIs estão ativadas no Google Cloud Console\n');
    } finally {
      rl.close();
    }
  });
}

configurarAutenticacao();
