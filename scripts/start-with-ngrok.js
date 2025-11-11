const ngrok = require('ngrok');
require('dotenv').config();

async function iniciarNgrok() {
  try {
    const authtoken = process.env.NGROK_AUTHTOKEN;
    
    if (!authtoken) {
      console.error('\n❌ ERRO: NGROK_AUTHTOKEN não configurado!');
      console.log('\n📝 Configure a secret NGROK_AUTHTOKEN no Replit com seu token do ngrok\n');
      process.exit(1);
    }

    console.log('🚀 Iniciando ngrok...');
    
    await ngrok.authtoken(authtoken);
    
    const url = await ngrok.connect({
      addr: 5000,
      region: 'sa'
    });

    console.log('\n✅ SISTEMA DISPONÍVEL NA WEB!');
    console.log('═'.repeat(80));
    console.log('\n🌐 URL Pública do Sistema:');
    console.log(`   ${url}`);
    console.log('\n📋 Compartilhe esta URL com sua equipe para acessar o sistema!');
    console.log('\n💡 IMPORTANTE:');
    console.log('   - Esta URL é temporária e muda a cada reinicialização');
    console.log('   - Para URL fixa, considere usar domínio personalizado no ngrok');
    console.log('   - O sistema estará acessível enquanto o servidor estiver rodando');
    console.log('\n═'.repeat(80));
    console.log('\n🔒 Credenciais de Login:');
    console.log('   Usuário: admin');
    console.log('   Senha: admin');
    console.log('\n═'.repeat(80));
    console.log('\n✅ Sistema funcionando! Mantenha este processo rodando.\n');

  } catch (error) {
    console.error('\n❌ Erro ao iniciar ngrok:', error.message);
    console.log('\n💡 Possíveis soluções:');
    console.log('   1. Verifique se o NGROK_AUTHTOKEN está correto nas Secrets');
    console.log('   2. Certifique-se de que o servidor está rodando na porta 5000');
    console.log('   3. Verifique sua conexão com a internet\n');
    process.exit(1);
  }
}

process.on('SIGINT', async () => {
  console.log('\n\n🛑 Encerrando ngrok...');
  await ngrok.kill();
  process.exit(0);
});

iniciarNgrok();
