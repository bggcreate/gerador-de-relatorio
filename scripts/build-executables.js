const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

console.log('═══════════════════════════════════════════════════');
console.log('  📦 GERADOR DE EXECUTÁVEIS STANDALONE');
console.log('═══════════════════════════════════════════════════\n');

const DIST_DIR = path.join(process.cwd(), 'dist');

function ensureDir(dir) {
  if (!fs.existsSync(dir)) {
    fs.mkdirSync(dir, { recursive: true });
    console.log(`✅ Diretório criado: ${dir}`);
  }
}

function checkPkgInstalled() {
  try {
    execSync('pkg --version', { stdio: 'pipe' });
    return true;
  } catch (error) {
    return false;
  }
}

function installPkg() {
  console.log('📥 Instalando pkg globalmente...\n');
  try {
    execSync('npm install -g pkg', { stdio: 'inherit' });
    console.log('\n✅ pkg instalado com sucesso!\n');
  } catch (error) {
    console.error('\n❌ Erro ao instalar pkg. Tente manualmente: npm install -g pkg\n');
    process.exit(1);
  }
}

function buildExecutable(target, outputName, platform) {
  console.log(`🔨 Gerando executável para ${platform}...`);
  console.log(`   Target: ${target}`);
  console.log(`   Output: ${outputName}\n`);

  try {
    const cmd = `pkg . --targets ${target} --output ${outputName}`;
    execSync(cmd, { stdio: 'inherit' });
    console.log(`\n✅ ${platform} gerado com sucesso!\n`);
    return true;
  } catch (error) {
    console.error(`\n❌ Erro ao gerar ${platform}\n`);
    return false;
  }
}

function createReadme() {
  const readme = `
═══════════════════════════════════════════════════════════════
  SISTEMA DE RELATÓRIOS - EXECUTÁVEL STANDALONE
═══════════════════════════════════════════════════════════════

📦 CONTEÚDO DESTE PACOTE:

  Windows/
    └── SistemaRelatorios-Windows.exe

  Mac/
    └── SistemaRelatorios-Mac

  Linux/
    └── SistemaRelatorios-Linux

  Arquivos Comuns/
    └── (copie views/, public/, middleware/, services/ do projeto original)

═══════════════════════════════════════════════════════════════
  COMO USAR
═══════════════════════════════════════════════════════════════

1️⃣  CONFIGURAR

   Crie um arquivo .env na mesma pasta do executável:

   SESSION_SECRET=sua_senha_secreta_32_caracteres
   JWT_SECRET=outra_senha_diferente_32_caracteres
   GOOGLE_CLIENT_ID=seu_client_id
   GOOGLE_CLIENT_SECRET=seu_client_secret
   GOOGLE_REFRESH_TOKEN=seu_refresh_token
   EMAIL_REMETENTE=seu_email@gmail.com
   EMAIL_BACKUP=seu_email@gmail.com
   PORT=5000

   📖 Veja GOOGLE_DRIVE_SETUP.md para obter as credenciais

2️⃣  EXECUTAR

   Windows:
     Clique duas vezes em SistemaRelatorios-Windows.exe

   Mac/Linux:
     chmod +x SistemaRelatorios-Mac (ou Linux)
     ./SistemaRelatorios-Mac

3️⃣  ACESSAR

   Abra no navegador: http://localhost:5000
   
   Login padrão:
     Usuário: admin
     Senha: admin

═══════════════════════════════════════════════════════════════
  SINCRONIZAÇÃO COM GOOGLE DRIVE
═══════════════════════════════════════════════════════════════

✅ Automática: Sincroniza a cada 1 hora
✅ Manual: Execute com --sync

   Windows:
     SistemaRelatorios-Windows.exe --sync

   Mac/Linux:
     ./SistemaRelatorios-Mac --sync

═══════════════════════════════════════════════════════════════
  SUPORTE
═══════════════════════════════════════════════════════════════

📖 Documentação completa: EXECUTAVEL_STANDALONE.md
🔧 Configuração Google: GOOGLE_DRIVE_SETUP.md
❓ FAQ: Veja seção "Solução de Problemas" na documentação

═══════════════════════════════════════════════════════════════
`;

  const readmePath = path.join(DIST_DIR, 'LEIA-ME.txt');
  fs.writeFileSync(readmePath, readme);
  console.log(`📄 Arquivo LEIA-ME.txt criado em: ${readmePath}`);
}

function createEnvExample() {
  const envExample = `# ═══════════════════════════════════════════════════════
#  CONFIGURAÇÃO DO SISTEMA
# ═══════════════════════════════════════════════════════
# 
# ⚠️  ATENÇÃO: 
# - Renomeie este arquivo para .env (remova o .example)
# - Preencha com suas credenciais reais
# - NUNCA compartilhe este arquivo com ninguém
# 
# ═══════════════════════════════════════════════════════

# Segurança (escolha senhas fortes com mínimo 32 caracteres)
SESSION_SECRET=minha_senha_super_secreta_minimo_32_caracteres_123456
JWT_SECRET=outro_secret_diferente_tambem_minimo_32_caracteres_789

# Google Drive (obter em console.cloud.google.com)
# Veja o guia: GOOGLE_DRIVE_SETUP.md
GOOGLE_CLIENT_ID=123456789-abc.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=GOCSPX-xyz123456
GOOGLE_REFRESH_TOKEN=1//0abc123xyz...

# Email para receber backups automáticos
EMAIL_REMETENTE=seu_email@gmail.com
EMAIL_BACKUP=seu_email@gmail.com

# Porta do servidor (padrão: 5000)
PORT=5000

# ═══════════════════════════════════════════════════════
#  COMO OBTER AS CREDENCIAIS DO GOOGLE?
# ═══════════════════════════════════════════════════════
# 
# 1. Acesse: https://console.cloud.google.com/
# 2. Crie um projeto
# 3. Ative Google Drive API e Gmail API
# 4. Crie credenciais OAuth 2.0
# 5. Execute: node scripts/google-auth-setup.js
# 6. Copie as credenciais para este arquivo
# 
# ═══════════════════════════════════════════════════════
`;

  const envExamplePath = path.join(DIST_DIR, '.env.example');
  fs.writeFileSync(envExamplePath, envExample);
  console.log(`📄 Arquivo .env.example criado em: ${envExamplePath}`);
}

function showSummary(results) {
  console.log('\n═══════════════════════════════════════════════════');
  console.log('  📊 RESUMO DA GERAÇÃO');
  console.log('═══════════════════════════════════════════════════\n');

  console.log('Executáveis gerados:\n');
  results.forEach(result => {
    const status = result.success ? '✅' : '❌';
    console.log(`  ${status} ${result.platform}`);
    if (result.success) {
      console.log(`      ${result.path}`);
    }
  });

  console.log('\n═══════════════════════════════════════════════════');
  console.log('  📦 PRÓXIMOS PASSOS');
  console.log('═══════════════════════════════════════════════════\n');

  console.log('1. Copie estas pastas para dist/:');
  console.log('   - views/');
  console.log('   - public/');
  console.log('   - middleware/');
  console.log('   - services/');
  console.log('');
  console.log('2. Arquivos de documentação criados:');
  console.log('   - dist/LEIA-ME.txt');
  console.log('   - dist/.env.example');
  console.log('');
  console.log('3. Para distribuir, inclua:');
  console.log('   - Executável da plataforma desejada');
  console.log('   - Pastas necessárias (views, public, etc)');
  console.log('   - .env.example (modelo de configuração)');
  console.log('   - EXECUTAVEL_STANDALONE.md (documentação)');
  console.log('   - GOOGLE_DRIVE_SETUP.md (configuração)');
  console.log('');
  console.log('4. O usuário final deve:');
  console.log('   - Criar arquivo .env com suas credenciais');
  console.log('   - Executar o programa');
  console.log('   - Acessar http://localhost:5000');
  console.log('');
  console.log('═══════════════════════════════════════════════════\n');
}

function copyRequiredFolders() {
  console.log('📁 Copiando arquivos necessários para dist/...\n');

  const folders = ['views', 'public', 'middleware', 'services'];
  const foldersPath = path.join(DIST_DIR, 'Arquivos-Comuns');

  ensureDir(foldersPath);

  folders.forEach(folder => {
    const sourcePath = path.join(process.cwd(), folder);
    const destPath = path.join(foldersPath, folder);

    if (fs.existsSync(sourcePath)) {
      try {
        fs.cpSync(sourcePath, destPath, { recursive: true });
        console.log(`  ✅ ${folder}/ copiado`);
      } catch (error) {
        console.log(`  ⚠️  Erro ao copiar ${folder}/:`, error.message);
      }
    } else {
      console.log(`  ⚠️  ${folder}/ não encontrado`);
    }
  });

  const docs = ['EXECUTAVEL_STANDALONE.md', 'GOOGLE_DRIVE_SETUP.md', 'COMO_RODAR_EM_QUALQUER_MAQUINA.md'];
  docs.forEach(doc => {
    const sourcePath = path.join(process.cwd(), doc);
    const destPath = path.join(DIST_DIR, doc);

    if (fs.existsSync(sourcePath)) {
      try {
        fs.copyFileSync(sourcePath, destPath);
        console.log(`  ✅ ${doc} copiado`);
      } catch (error) {
        console.log(`  ⚠️  Erro ao copiar ${doc}:`, error.message);
      }
    }
  });

  console.log('');
}

async function main() {
  ensureDir(DIST_DIR);

  if (!checkPkgInstalled()) {
    console.log('⚠️  pkg não está instalado globalmente\n');
    installPkg();
  }

  const builds = [
    {
      target: 'node20-win-x64',
      output: path.join(DIST_DIR, 'Windows', 'SistemaRelatorios-Windows.exe'),
      platform: 'Windows (64-bit)'
    },
    {
      target: 'node20-macos-x64',
      output: path.join(DIST_DIR, 'Mac', 'SistemaRelatorios-Mac'),
      platform: 'macOS (64-bit)'
    },
    {
      target: 'node20-linux-x64',
      output: path.join(DIST_DIR, 'Linux', 'SistemaRelatorios-Linux'),
      platform: 'Linux (64-bit)'
    }
  ];

  ensureDir(path.join(DIST_DIR, 'Windows'));
  ensureDir(path.join(DIST_DIR, 'Mac'));
  ensureDir(path.join(DIST_DIR, 'Linux'));

  const results = [];

  for (const build of builds) {
    const success = buildExecutable(build.target, build.output, build.platform);
    results.push({
      platform: build.platform,
      path: build.output,
      success
    });
  }

  console.log('\n📝 Criando arquivos de documentação...\n');
  createReadme();
  createEnvExample();
  copyRequiredFolders();

  showSummary(results);

  const allSuccess = results.every(r => r.success);
  process.exit(allSuccess ? 0 : 1);
}

main().catch(error => {
  console.error('\n❌ ERRO CRÍTICO:', error.message);
  process.exit(1);
});
