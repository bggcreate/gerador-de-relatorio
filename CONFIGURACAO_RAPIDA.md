# 🚀 CONFIGURAÇÃO RÁPIDA - Sistema na Web

## ✅ O que você já tem:
- ✅ GOOGLE_CLIENT_ID: `598831000105-36cal24gsg9j7ck4pn4fr40olk2j4f5u.apps.googleusercontent.com`
- ✅ GOOGLE_CLIENT_SECRET: `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`
- ✅ NGROK_AUTHTOKEN: `35LOifgN4EHgRI75fIv1RQOgKeH_5QEiUWEQzZTNmocUHAa4s`
- ✅ BACKUP_EMAIL: `alexcoelho.loft@gmail.com`

---

## 📝 PASSO 1: Adicione as Secrets no Replit

No painel lateral do Replit, clique em **"Secrets"** (ícone de cadeado) e adicione:

```
NGROK_AUTHTOKEN = 35LOifgN4EHgRI75fIv1RQOgKeH_5QEiUWEQzZTNmocUHAa4s
GOOGLE_CLIENT_ID = 598831000105-36cal24gsg9j7ck4pn4fr40olk2j4f5u.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET = 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
BACKUP_EMAIL = alexcoelho.loft@gmail.com
```

---

## 🔐 PASSO 2: Obter o GOOGLE_REFRESH_TOKEN

### 2.1 Execute o comando:
```bash
node scripts/google-auth-setup.js
```

### 2.2 O terminal mostrará um link, copie e cole no navegador

### 2.3 Faça login com a conta: **alexcoelho.loft@gmail.com**

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
- ✅ Enviará backup para alexcoelho.loft@gmail.com quando atingir 13GB
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
   - Envia para **alexcoelho.loft@gmail.com**
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
