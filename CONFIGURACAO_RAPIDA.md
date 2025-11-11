# 🚀 CONFIGURAÇÃO RÁPIDA - Sistema na Web

## ✅ O que você precisa configurar:
- 📝 GOOGLE_CLIENT_ID: (fornecido em attached_assets/client_secret_*.json)
- 📝 GOOGLE_CLIENT_SECRET: (fornecido em attached_assets/client_secret_*.json)
- 📝 NGROK_AUTHTOKEN: (token do ngrok fornecido anteriormente)
- 📝 BACKUP_EMAIL: (email para receber os backups)

---

## 📝 PASSO 1: Adicione as Secrets no Replit

No painel lateral do Replit, clique em **"Secrets"** (ícone de cadeado) e adicione:

```
NGROK_AUTHTOKEN = [seu token do ngrok]
GOOGLE_CLIENT_ID = [copie do arquivo client_secret JSON - campo "client_id"]
GOOGLE_CLIENT_SECRET = [copie do arquivo client_secret JSON - campo "client_secret"]
BACKUP_EMAIL = [seu email para receber backups]
```

---

## 🔐 PASSO 2: Obter o GOOGLE_REFRESH_TOKEN

### 2.1 Execute o comando:
```bash
node scripts/google-auth-setup.js
```

### 2.2 O terminal mostrará um link, copie e cole no navegador

### 2.3 Faça login com sua conta Google

### 2.4 Autorize o aplicativo (clique em "Permitir")

### 2.5 Copie o código fornecido e cole no terminal

### 2.6 O script mostrará seu **GOOGLE_REFRESH_TOKEN**

### 2.7 Adicione como Secret no Replit:
```
GOOGLE_REFRESH_TOKEN = [o token que você copiou]
```

---

## 🌐 PASSO 3: Expor Sistema na Web com Ngrok

Após adicionar todas as secrets, o sistema automaticamente:
- ✅ Conectará ao Google Drive
- ✅ Salvará relatórios automaticamente
- ✅ Enviará backup para o email configurado quando atingir 13GB
- ✅ Estará acessível na web via ngrok

---

## 🎯 COMO FUNCIONA

### Relatórios no Google Drive:
1. Cada relatório criado é salvo automaticamente no Google Drive
2. Organizado por ano/mês: `Sistema_Relatorios/2025/11-Novembro/`
3. Você tem 15GB gratuitos

### Backup Automático:
1. Sistema monitora o espaço usado
2. Quando chegar em **13GB de 15GB**:
   - Cria arquivo ZIP com todos os relatórios
   - Envia para o email configurado em BACKUP_EMAIL
   - Limpa relatórios com mais de 90 dias
   - Libera espaço no Drive

### Backup Manual:
- Você pode fazer backup a qualquer momento
- Basta chamar a API: `POST /api/drive/backup`

---

## 📊 APIs Disponíveis

```bash
# Verificar espaço usado
GET /api/drive/quota

# Listar relatórios do Drive
GET /api/drive/relatorios

# Fazer backup manual (envia para seu email)
POST /api/drive/backup

# Limpar arquivos antigos
POST /api/drive/limpar
```

---

## ✅ Pronto!

Depois de configurar as secrets e obter o refresh token:
1. Reinicie o servidor
2. O sistema estará funcionando!
3. Sua equipe poderá usar pela URL do ngrok
4. Todos os relatórios irão para o Google Drive
5. Backups automáticos para seu email

---

## 🔒 Importante

- ⚠️ **Mantenha as secrets em segredo**
- ⚠️ **Não compartilhe o REFRESH_TOKEN**
- ⚠️ **O token dá acesso à sua conta Google**
