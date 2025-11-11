# 🚀 Configuração do Google Drive como Banco de Dados

Este sistema usa o **Google Drive gratuito (15GB)** para armazenar relatórios. Veja como configurar:

---

## 📋 Passo 1: Criar Projeto no Google Cloud Console

1. Acesse: https://console.cloud.google.com/
2. Clique em **"Criar Projeto"**
3. Nome do projeto: `Sistema Relatorios`
4. Clique em **"Criar"**

---

## 🔑 Passo 2: Ativar APIs Necessárias

1. No menu lateral, vá em **"APIs e Serviços" → "Biblioteca"**
2. Ative estas 2 APIs:
   - **Google Drive API** (para salvar/ler relatórios)
   - **Gmail API** (para enviar backups por email)

---

## 🎫 Passo 3: Criar Credenciais OAuth 2.0

1. Vá em **"APIs e Serviços" → "Credenciais"**
2. Clique em **"+ Criar Credenciais" → "ID do cliente OAuth"**
3. Tipo de aplicativo: **"Aplicativo para computador"**
4. Nome: `Sistema Relatorios Desktop`
5. Clique em **"Criar"**
6. **ANOTE:**
   - `ID do cliente` (exemplo: 123456-abc.apps.googleusercontent.com)
   - `Chave secreta do cliente` (exemplo: GOCSPX-xyz123)

---

## 🔐 Passo 4: Obter Refresh Token

### 4.1 Configure as credenciais temporariamente

Crie/edite o arquivo `.env` na raiz do projeto:

```env
GOOGLE_CLIENT_ID=seu_client_id_aqui
GOOGLE_CLIENT_SECRET=sua_client_secret_aqui
```

### 4.2 Execute o script de autorização

```bash
node scripts/google-auth-setup.js
```

### 4.3 Siga as instruções

1. O script mostrará um link
2. Abra o link no navegador
3. Faça login com sua conta Google
4. Autorize o aplicativo
5. Copie o código fornecido
6. Cole no terminal
7. O script mostrará seu `REFRESH_TOKEN`

### 4.4 Complete o arquivo .env

Adicione o refresh token:

```env
GOOGLE_CLIENT_ID=seu_client_id_aqui
GOOGLE_CLIENT_SECRET=sua_client_secret_aqui
GOOGLE_REFRESH_TOKEN=seu_refresh_token_aqui
EMAIL_REMETENTE=seu_email@gmail.com
EMAIL_BACKUP=seu_email@gmail.com
```

---

## ✅ Passo 5: Testar Conexão

Execute o servidor:

```bash
npm start
```

Você deve ver:
```
✅ Autenticado no Google Drive com sucesso!
📁 Pasta Sistema_Relatorios criada
```

---

## 📊 Como Funciona

### Estrutura no Google Drive:

```
📁 Sistema_Relatorios/
  📁 2025/
    📁 11-Novembro/
      📄 relatorio_loja_001_2025-11-11.json
      📄 relatorio_loja_002_2025-11-11.json
    📁 12-Dezembro/
      📄 relatorio_loja_003_2025-12-01.json
```

### Quando você cria um relatório:
1. Sistema salva automaticamente no Drive
2. Organiza por ano e mês
3. Formato JSON (fácil de ler e exportar)

### Monitoramento automático:
- Sistema verifica espaço usado
- Quando atingir 13GB (de 15GB):
  - ✅ Cria backup ZIP com todos os relatórios
  - ✅ Envia por email
  - ✅ Remove relatórios com mais de 90 dias
  - ✅ Libera espaço no Drive

### Backup manual:
- Acesse a aba **"Configurações"** no sistema
- Clique em **"Fazer Backup Agora"**
- Backup enviado para seu email

---

## 🖥️ Rodar em Qualquer Máquina

### 1. Copie estes arquivos para a nova máquina:
```
- server.js
- package.json
- .env (com suas credenciais)
- /services/
- /public/
- /views/
- /middleware/
```

### 2. Instale dependências:
```bash
npm install
```

### 3. Inicie o sistema:
```bash
npm start
```

### 4. Acesse no navegador:
```
http://localhost:5000
```

**✅ Pronto!** O sistema lerá os relatórios do Google Drive automaticamente.

---

## 🔒 Segurança

- ⚠️ **NUNCA compartilhe seu arquivo `.env`**
- ⚠️ **Mantenha Client ID e Secret em segredo**
- ⚠️ **Refresh Token dá acesso à sua conta Google**

---

## 🆘 Problemas Comuns

### "Credenciais não configuradas"
→ Verifique se o arquivo `.env` existe e está preenchido corretamente

### "Access denied"
→ Certifique-se de ter ativado Google Drive API e Gmail API

### "Invalid refresh token"
→ Execute `node scripts/google-auth-setup.js` novamente

---

## 💰 Custos

**TOTALMENTE GRATUITO!**
- ✅ Google Drive: 15GB grátis
- ✅ Gmail API: gratuito para uso pessoal
- ✅ Sistema roda localmente (sem hospedagem)

---

## 📞 Suporte

Em caso de dúvidas, verifique:
1. Arquivo `.env` está completo?
2. APIs estão ativadas no Google Cloud?
3. Credenciais estão corretas?
