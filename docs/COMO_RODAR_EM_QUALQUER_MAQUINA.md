# 🖥️ Como Rodar o Sistema em Qualquer Máquina

Este guia mostra como executar o sistema em qualquer computador (Windows, Mac, Linux) sem precisar pagar hospedagem.

---

## 📋 Requisitos

- **Node.js** versão 18 ou superior
- **Conta Google** (gratuita, para Drive e Gmail)
- **5 minutos** para configuração inicial

---

## 🚀 Passo a Passo - Primeira Instalação

### 1. Baixar o Sistema

Copie estes arquivos/pastas para a nova máquina:

```
sistema-monitoramento/
├── server.js
├── package.json
├── .env (crie com suas credenciais)
├── middleware/
├── public/
├── scripts/
├── services/
├── views/
└── data/ (será criado automaticamente)
```

### 2. Instalar Dependências

Abra o terminal/prompt de comando na pasta do sistema e execute:

```bash
npm install
```

### 3. Configurar Google Drive

Siga EXATAMENTE os passos do arquivo `GOOGLE_DRIVE_SETUP.md`:

1. Criar projeto no Google Cloud Console
2. Ativar Google Drive API e Gmail API
3. Criar credenciais OAuth 2.0
4. Obter Refresh Token

### 4. Criar arquivo .env

Crie um arquivo chamado `.env` na raiz do projeto com este conteúdo:

```env
# Segurança (use senhas fortes!)
SESSION_SECRET=minha_senha_super_secreta_minimo_32_caracteres_123456
JWT_SECRET=outro_secret_diferente_tambem_com_minimo_32_caracteres_789

# Google Drive (obter em console.cloud.google.com)
GOOGLE_CLIENT_ID=123456789-abc.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=GOCSPX-xyz123456
GOOGLE_REFRESH_TOKEN=1//0abc123xyz...

# Email para receber backups
EMAIL_REMETENTE=seu_email@gmail.com
EMAIL_BACKUP=seu_email@gmail.com

# Porta do servidor
PORT=5000
```

### 5. Iniciar o Sistema

```bash
npm start
```

Você verá:

```
✅ Autenticado no Google Drive com sucesso!
📊 Google Drive: 0.05GB de 15GB usados (0.3%)
Servidor rodando em http://0.0.0.0:5000
```

### 6. Acessar no Navegador

Abra: `http://localhost:5000`

Faça login com:
- **Usuário:** admin
- **Senha:** admin

---

## 💡 Como Funciona

### Armazenamento

- **Relatórios:** Salvos automaticamente no Google Drive
- **Estrutura:** `Sistema_Relatorios/2025/11-Novembro/relatorio_....json`
- **Grátis:** 15GB do Google Drive (muitos anos de relatórios!)

### Backup Automático

Quando o Drive atingir **13GB usados**:

1. ✅ Sistema cria backup ZIP com todos os relatórios
2. ✅ Envia por email para você
3. ✅ Remove relatórios com mais de 90 dias
4. ✅ Libera espaço no Drive

### Backup Manual

No sistema, vá em **Configurações** e clique em **"Fazer Backup Agora"**.

---

## 🔄 Rodar em Outra Máquina

Já configurou tudo? Para rodar em um segundo computador:

### Opção 1: Copiar Tudo

1. Copie a pasta completa (com `.env`)
2. Execute `npm install`
3. Execute `npm start`

### Opção 2: Apenas Código

1. Copie os arquivos (SEM `.env`)
2. Execute `npm install`
3. Crie novo `.env` com as MESMAS credenciais
4. Execute `npm start`

**✅ Os relatórios aparecerão automaticamente!** (estão no Drive)

---

## 📱 Acesso Remoto

### Opção 1: Rede Local (Mesma Casa/Empresa)

Descubra o IP da máquina que roda o sistema:

```bash
# Windows
ipconfig

# Mac/Linux
ifconfig
```

Outros computadores na mesma rede acessam:  
`http://192.168.1.X:5000` (substitua X pelo IP correto)

### Opção 2: Internet (De Qualquer Lugar)

Instale um túnel gratuito:

**LocalTunnel (fácil):**
```bash
npm install -g localtunnel
lt --port 5000
```

Vai mostrar um link tipo: `https://abc-123.loca.lt`  
Compartilhe esse link!

**Ngrok (recomendado):**
```bash
# Baixe em: https://ngrok.com/download
ngrok http 5000
```

---

## 🛠️ Comandos Úteis

```bash
# Iniciar sistema
npm start

# Ver logs em tempo real
# (os logs aparecem automaticamente ao iniciar)

# Parar sistema
# Pressione Ctrl+C no terminal

# Obter novo refresh token (se expirar)
node scripts/google-auth-setup.js
```

---

## ❓ Solução de Problemas

### "Google Drive não configurado"

→ Verifique se o `.env` existe e está preenchido corretamente

### "Cannot find module"

→ Execute: `npm install` novamente

### "Porta 5000 já está em uso"

→ Altere a porta no `.env`: `PORT=3000`

### "Invalid refresh token"

→ Execute novamente: `node scripts/google-auth-setup.js`

---

## 💰 Custos

**TUDO GRATUITO!**

- ✅ Google Drive: 15GB grátis
- ✅ Gmail API: grátis para uso pessoal
- ✅ Node.js: grátis e open source
- ✅ Hospedagem: ZERO (roda no seu PC)

---

## 🔒 Dicas de Segurança

1. **Nunca compartilhe seu arquivo `.env`**
2. **Troque as senhas padrão** (admin/admin)
3. **Mantenha o Node.js atualizado**
4. **Faça backup do `.env`** em local seguro
5. **Use senhas fortes** para SESSION_SECRET e JWT_SECRET

---

## 📞 Checklist Rápido

Antes de usar em outra máquina:

- [ ] Node.js instalado?
- [ ] Pasta do sistema copiada?
- [ ] Executou `npm install`?
- [ ] Arquivo `.env` criado e preenchido?
- [ ] Google Drive API ativada?
- [ ] Refresh token configurado?
- [ ] Executou `npm start`?
- [ ] Acessou `http://localhost:5000`?

**✅ Tudo OK?** Sistema pronto para usar!

---

## 🎯 Resumo

1. **Instalar Node.js**
2. **Copiar sistema**
3. **Executar** `npm install`
4. **Configurar** `.env` com credenciais do Google
5. **Iniciar** com `npm start`
6. **Acessar** `http://localhost:5000`

**Simples assim!** 🚀
