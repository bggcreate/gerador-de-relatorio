# 🚀 Como Criar e Usar o Executável Standalone

Este guia mostra como transformar o sistema em um **executável standalone** que pode rodar em qualquer computador **sem precisar instalar Node.js**, e sincroniza automaticamente com o Google Drive.

---

## 📦 O que é um Executável Standalone?

É uma versão do sistema que:
- ✅ **Não precisa Node.js instalado** na máquina
- ✅ **Funciona em Windows, Mac e Linux**
- ✅ **Sincroniza automaticamente** com Google Drive
- ✅ **Banco de dados híbrido** (local + nuvem)
- ✅ **Um único arquivo executável** (.exe no Windows)

---

## 🛠️ Parte 1: Criar os Executáveis

### Pré-requisitos

Você precisa ter **Node.js instalado APENAS na máquina de desenvolvimento** (onde vai criar o executável).

### Passo 1: Instalar Dependências

```bash
npm install
```

### Passo 2: Gerar Executáveis

Você pode gerar para todas as plataformas de uma vez:

```bash
npm run build
```

Ou gerar para plataformas específicas:

```bash
# Apenas Windows
npm run build:win

# Apenas Mac
npm run build:mac

# Apenas Linux
npm run build:linux
```

### Passo 3: Localizar os Executáveis

Os executáveis serão criados na pasta `dist/`:

```
dist/
├── SistemaRelatorios-Windows.exe  (Windows)
├── SistemaRelatorios-Mac          (Mac)
└── SistemaRelatorios-Linux        (Linux)
```

**Tamanho aproximado:** 60-80 MB cada (inclui Node.js embutido)

---

## 💻 Parte 2: Usar o Executável

### Passo 1: Preparar Pacote de Distribuição

Crie uma pasta com estes arquivos:

```
SistemaRelatorios/
├── SistemaRelatorios-Windows.exe  (ou Mac/Linux)
├── .env                           (criar este arquivo)
├── views/                         (copiar do projeto)
├── public/                        (copiar do projeto)
├── middleware/                    (copiar do projeto)
├── services/                      (copiar do projeto)
└── README-USUARIO.txt             (opcional: instruções)
```

### Passo 2: Configurar o arquivo .env

⚠️ **IMPORTANTE**: Cada usuário deve criar seu próprio arquivo `.env` com suas credenciais do Google.

Crie um arquivo chamado `.env` na mesma pasta do executável:

```env
# ═══════════════════════════════════════════════════════
#  CONFIGURAÇÃO DO SISTEMA - NÃO COMPARTILHE ESTE ARQUIVO
# ═══════════════════════════════════════════════════════

# Segurança (escolha senhas fortes!)
SESSION_SECRET=minha_senha_super_secreta_minimo_32_caracteres_123456
JWT_SECRET=outro_secret_diferente_tambem_minimo_32_caracteres_789

# Google Drive (obter em console.cloud.google.com)
GOOGLE_CLIENT_ID=123456789-abc.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=GOCSPX-xyz123456
GOOGLE_REFRESH_TOKEN=1//0abc123xyz...

# Email para receber backups
EMAIL_REMETENTE=seu_email@gmail.com
EMAIL_BACKUP=seu_email@gmail.com

# Porta do servidor (padrão: 5000)
PORT=5000
```

**Como obter as credenciais do Google?**  
Siga o guia completo em: `GOOGLE_DRIVE_SETUP.md`

### Passo 3: Executar o Sistema

#### No Windows:
```
Clique duas vezes em: SistemaRelatorios-Windows.exe
```

Ou via CMD:
```cmd
SistemaRelatorios-Windows.exe
```

#### No Mac:
```bash
chmod +x SistemaRelatorios-Mac
./SistemaRelatorios-Mac
```

#### No Linux:
```bash
chmod +x SistemaRelatorios-Linux
./SistemaRelatorios-Linux
```

### Passo 4: Acessar no Navegador

Abra: **http://localhost:5000**

Login padrão:
- **Usuário:** admin
- **Senha:** admin

---

## 🔄 Sincronização com Google Drive

### Sincronização Automática

O sistema sincroniza automaticamente quando:
- ✅ Um novo relatório é criado
- ✅ O sistema é iniciado
- ✅ A cada 1 hora (sincronização em background)

### Sincronização Manual

Para forçar uma sincronização imediata:

```bash
# No Windows (CMD)
SistemaRelatorios-Windows.exe --sync

# No Mac/Linux
./SistemaRelatorios-Mac --sync
```

Ou use o script incluído:

```bash
node scripts/sync-database.js
```

### Como Funciona a Sincronização?

```
┌─────────────────┐        ┌──────────────────┐
│  Banco Local    │  ←───→ │  Google Drive    │
│  (SQLite)       │  Sync  │  (15GB grátis)   │
└─────────────────┘        └──────────────────┘

Fluxo bidirecional:
1. Upload: Relatórios locais novos → Google Drive
2. Download: Relatórios do Drive → Banco local
3. Merge: Sem duplicatas, mantém versão mais recente
```

**Vantagens:**
- 📱 Acesse de qualquer máquina (dados na nuvem)
- 💾 Backup automático
- 🔄 Sempre sincronizado
- ⚡ Rápido (usa cache local)

---

## 📊 Banco de Dados Híbrido

### Estrutura:

```
data/
├── database.db          # Banco local SQLite
├── last-sync.json       # Estado da última sincronização
└── dvr_files/           # Arquivos do DVR/NVR
```

### Quando usar Local vs Drive:

| Operação               | Usa        | Motivo                    |
|------------------------|------------|---------------------------|
| Criar relatório        | Ambos      | Salva local + envia Drive |
| Consultar relatórios   | Local      | Mais rápido               |
| Backup completo        | Drive      | Segurança                 |
| Sincronização          | Ambos      | Mantém atualizado         |

---

## 🎯 Casos de Uso

### Caso 1: Executável para cada Loja

```
Loja Centro/
└── SistemaRelatorios-Windows.exe + .env
    ↓
    Sincroniza com: Google Drive → conta centralizada

Loja Shopping/
└── SistemaRelatorios-Windows.exe + .env
    ↓
    Sincroniza com: Google Drive → mesma conta

Resultado: Todos os dados centralizados no Google Drive!
```

### Caso 2: Executável Portátil em Pen Drive

```
Pen Drive/
├── SistemaRelatorios-Windows.exe
├── .env
├── views/
├── public/
└── data/

Vantagens:
✅ Leva para qualquer máquina
✅ Não precisa instalar nada
✅ Dados sincronizados
```

### Caso 3: Múltiplas Máquinas, Mesma Base

```
Máquina 1 (Windows) ─┐
Máquina 2 (Mac)      ├──→ Google Drive (Banco Central)
Máquina 3 (Linux)    ─┘

Todas veem os mesmos relatórios!
```

---

## 🔒 Segurança

### ⚠️ NUNCA FAÇA ISSO:

❌ Compartilhar o arquivo `.env`  
❌ Incluir `.env` no executável  
❌ Enviar credenciais por email  
❌ Versionar `.env` no git  

### ✅ FAÇA ASSIM:

✅ Cada usuário cria seu próprio `.env`  
✅ Use `.env.example` como modelo  
✅ Guarde credenciais em local seguro  
✅ Use senhas fortes (mínimo 32 caracteres)  

---

## 🆘 Solução de Problemas

### "Cannot find module"

**Problema:** Executável não encontra os arquivos.

**Solução:** 
1. Certifique-se que `views/`, `public/`, `middleware/` e `services/` estão na mesma pasta do executável
2. Execute a partir da pasta onde o executável está

### "Google Drive não configurado"

**Problema:** Arquivo `.env` ausente ou incompleto.

**Solução:**
1. Crie o arquivo `.env` na mesma pasta do executável
2. Siga o guia `GOOGLE_DRIVE_SETUP.md`
3. Verifique que todas as variáveis estão preenchidas

### "Port already in use"

**Problema:** Porta 5000 já está sendo usada.

**Solução:**
1. Altere no `.env`: `PORT=3000`
2. Ou feche o programa que está usando a porta 5000

### "Invalid refresh token"

**Problema:** Token do Google expirou.

**Solução:**
1. Execute: `node scripts/google-auth-setup.js`
2. Siga as instruções para obter novo token
3. Atualize o `.env` com o novo `GOOGLE_REFRESH_TOKEN`

### Executável não inicia no Mac

**Problema:** Mac bloqueia executáveis não assinados.

**Solução:**
```bash
# Dar permissão de execução
chmod +x SistemaRelatorios-Mac

# Se ainda assim bloquear:
# Vá em: Preferências do Sistema > Segurança e Privacidade
# Clique em "Abrir Assim Mesmo"
```

---

## 📈 Performance

### Tamanhos:

| Item                     | Tamanho  |
|--------------------------|----------|
| Executável Windows       | ~70 MB   |
| Executável Mac           | ~65 MB   |
| Executável Linux         | ~65 MB   |
| Banco de dados (vazio)   | 20 KB    |
| Banco (1000 relatórios)  | ~2 MB    |

### Requisitos Mínimos:

- **RAM:** 256 MB disponível
- **Disco:** 200 MB livres
- **Internet:** Para sincronização com Drive
- **Sistema:** Windows 10+, macOS 10.13+, Linux moderno

---

## 🚀 Distribuição Profissional

### Passo 1: Criar Pacote Completo

```bash
# Estrutura profissional
SistemaRelatorios-v1.0/
├── Windows/
│   ├── SistemaRelatorios.exe
│   ├── LEIA-ME.txt
│   └── .env.example
├── Mac/
│   ├── SistemaRelatorios
│   ├── LEIA-ME.txt
│   └── .env.example
├── Linux/
│   ├── SistemaRelatorios
│   ├── LEIA-ME.txt
│   └── .env.example
├── Arquivos Comuns/
│   ├── views/
│   ├── public/
│   ├── middleware/
│   └── services/
└── Documentacao/
    ├── GOOGLE_DRIVE_SETUP.md
    ├── MANUAL-USUARIO.pdf
    └── FAQ.md
```

### Passo 2: Criar Instalador (Opcional)

Para Windows, use **Inno Setup** ou **NSIS**  
Para Mac, crie um `.dmg`  
Para Linux, crie um `.deb` ou `.rpm`

---

## 💰 Custos

**TOTALMENTE GRATUITO!**

- ✅ Google Drive: 15GB grátis
- ✅ Gmail API: grátis
- ✅ pkg (ferramenta): grátis e open source
- ✅ Node.js: grátis e open source
- ✅ Sem hospedagem paga
- ✅ Sem mensalidades

**Estimativa de capacidade:**  
Com 15GB grátis do Google Drive, você pode armazenar aproximadamente **7.500.000 relatórios** (assumindo ~2KB por relatório)!

---

## 📞 Checklist Rápido

Antes de distribuir o executável:

- [ ] Executável foi gerado com `npm run build`?
- [ ] Arquivo `.env.example` está incluído (SEM credenciais)?
- [ ] Pastas `views/`, `public/`, `middleware/`, `services/` estão no pacote?
- [ ] Documentação `GOOGLE_DRIVE_SETUP.md` está incluída?
- [ ] Testou em uma máquina limpa (sem Node.js)?
- [ ] Verificou que a sincronização funciona?
- [ ] Criou manual do usuário?

---

## 🎓 Resumo

1. **Desenvolver:** Crie o sistema normalmente com Node.js
2. **Buildar:** `npm run build` gera executáveis para todas as plataformas
3. **Distribuir:** Envie executável + arquivos necessários + `.env.example`
4. **Usuário:** Cria `.env` com suas credenciais, executa, pronto!
5. **Sincronizar:** Dados ficam no Google Drive, acessíveis de qualquer máquina

**Simples, gratuito e profissional!** 🚀
