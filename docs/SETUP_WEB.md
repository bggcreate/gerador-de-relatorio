# 🌐 CONFIGURAÇÃO DO SISTEMA NA WEB

## Guia completo para deixar seu sistema acessível pela internet

---

## ✅ PASSO 1: Adicionar as Secrets no Replit

Clique no ícone de **🔒 Secrets** no painel lateral esquerdo do Replit e adicione cada uma dessas variáveis:

### 1.1 Token do Ngrok (para expor o sistema na web)
```
Nome: NGROK_AUTHTOKEN
Valor: [seu token do ngrok fornecido]
```

### 1.2 Credenciais do Google
```
Nome: GOOGLE_CLIENT_ID
Valor: [copie do arquivo client_secret JSON - campo "client_id"]
```

```
Nome: GOOGLE_CLIENT_SECRET
Valor: [copie do arquivo client_secret JSON - campo "client_secret"]
```

### 1.3 Email para receber backups
```
Nome: BACKUP_EMAIL
Valor: [seu email para receber os backups]
```

---

## 🔐 PASSO 2: Obter o Google Refresh Token

### 2.1 Execute o script de autenticação do Google:

No terminal do Replit (Shell), execute:

```bash
node scripts/google-auth-setup.js
```

### 2.2 O script mostrará um link, **copie e cole no navegador**

### 2.3 Faça login com sua conta Google

### 2.4 Clique em **"Permitir"** para autorizar o app

### 2.5 **Copie o código** que aparecer na tela

### 2.6 **Cole o código no terminal** do Replit e pressione Enter

### 2.7 O script mostrará seu **GOOGLE_REFRESH_TOKEN**

### 2.8 Adicione como Secret no Replit:
```
Nome: GOOGLE_REFRESH_TOKEN
Valor: [cole o token que você copiou]
```

---

## 🌐 PASSO 3: Expor o Sistema na Web com Ngrok

### Opção A: Usar Workflow (Recomendado)

1. No Replit, vá em **Workflows** (painel lateral)
2. Clique em **"Add Workflow"**
3. Configure:
   - **Nome**: `Ngrok`
   - **Comando**: `npm run ngrok`
   - **Output Type**: `console`
4. Clique em **Start**
5. Copie a URL pública que aparecer no console (ex: https://abc123.ngrok.io)

### Opção B: Rodar Manualmente no Terminal

```bash
npm run ngrok
```

A URL pública aparecerá no terminal. Copie e compartilhe com sua equipe!

---

## 📋 VERIFICAÇÃO FINAL

Depois de configurar tudo, verifique se está funcionando:

### 1. Servidor rodando:
✅ Workflow "Server" deve estar **RUNNING** (verde)

### 2. Ngrok rodando:
✅ Workflow "Ngrok" deve estar **RUNNING** (verde)
✅ Console do ngrok mostra a URL pública

### 3. Google Drive conectado:
✅ Console do servidor mostra: `✅ Autenticado no Google Drive com sucesso!`

---

## 🎯 COMO USAR O SISTEMA

### 1. Acesse a URL do ngrok no navegador
Exemplo: `https://abc123.ngrok-free.app`

### 2. Faça login:
```
Usuário: admin
Senha: admin
```

### 3. Crie um relatório novo:
- Clique em **"Novo Relatório"**
- Preencha os dados
- Ao salvar, o relatório vai automaticamente para o **Google Drive**!

### 4. Verifique o Google Drive:
- Abra o Google Drive da sua conta
- Procure pela pasta **"Sistema_Relatorios"**
- Você verá os relatórios organizados por ano/mês

### 5. Monitorar espaço usado:
- Na aba **Admin** ou **Configurações**, veja o espaço usado
- Quando chegar em **13GB de 15GB**:
  - ✅ Sistema cria backup ZIP automaticamente
  - ✅ Envia para o email configurado em BACKUP_EMAIL
  - ✅ Remove relatórios com mais de 90 dias
  - ✅ Libera espaço no Drive

---

## 🔄 BACKUP MANUAL

Se quiser fazer backup a qualquer momento, use a API:

### Via cURL (Terminal):
```bash
curl -X POST https://sua-url-ngrok.app/api/drive/backup \
  -H "Content-Type: application/json" \
  -d '{"email": "seu-email@exemplo.com"}'
```

### Via Interface (se implementado):
- Vá em **Configurações**
- Clique em **"Fazer Backup Agora"**

---

## 📊 ESTRUTURA NO GOOGLE DRIVE

```
📁 Sistema_Relatorios/
  📁 2025/
    📁 01-Janeiro/
      📄 relatorio_loja_001_2025-01-15.json
      📄 relatorio_loja_002_2025-01-20.json
    📁 02-Fevereiro/
      📄 relatorio_loja_003_2025-02-05.json
    📁 11-Novembro/
      📄 relatorio_loja_120_2025-11-11.json
```

---

## 🛡️ SEGURANÇA

### ⚠️ NUNCA compartilhe:
- ❌ NGROK_AUTHTOKEN
- ❌ GOOGLE_CLIENT_SECRET
- ❌ GOOGLE_REFRESH_TOKEN

### ✅ Pode compartilhar:
- ✅ URL do ngrok (https://abc123.ngrok.io)
- ✅ Credenciais de login (admin/admin)

### 🔒 Recomendações:
1. **Mude a senha padrão** do admin após primeiro acesso
2. **Crie usuários** para cada membro da equipe
3. **Não compartilhe** o arquivo .env ou as Secrets do Replit
4. **Mantenha** backup dos dados importantes

---

## 💰 CUSTOS

**100% GRATUITO!**
- ✅ Google Drive: 15GB grátis
- ✅ Gmail API: gratuito para uso pessoal
- ✅ Ngrok: plano gratuito com URL aleatória
- ✅ Replit: plano gratuito (ou seu plano atual)

### Upgrades opcionais (se precisar):
- 💎 Ngrok Pro: URL fixa personalizada ($8/mês)
- 💎 Google Workspace: mais espaço no Drive (R$30/mês = 100GB)

---

## 🆘 PROBLEMAS COMUNS

### "Cannot find module 'ngrok'"
```bash
npm install
```

### "NGROK_AUTHTOKEN não configurado"
- Verifique se adicionou a Secret no Replit
- Nome EXATO: `NGROK_AUTHTOKEN`

### "Credenciais do Google Drive não configuradas"
- Certifique-se de ter executado `node scripts/google-auth-setup.js`
- Adicione o GOOGLE_REFRESH_TOKEN nas Secrets

### "Access denied" ao autorizar Google
- Certifique-se de ter ativado:
  - ✅ Google Drive API
  - ✅ Gmail API
- No Google Cloud Console do projeto

### Ngrok mostra erro de limite
- O plano gratuito do ngrok tem limite de requisições
- Considere upgrade para plano pago se necessário

---

## 📞 SUPORTE

Em caso de dúvidas:
1. ✅ Verifique se todas as Secrets estão configuradas
2. ✅ Certifique-se de ter executado o script de autenticação do Google
3. ✅ Confirme que as APIs estão ativas no Google Cloud Console
4. ✅ Reinicie os workflows se necessário

---

## ✅ CHECKLIST FINAL

Antes de compartilhar com a equipe, confirme:

- [ ] Secret `NGROK_AUTHTOKEN` adicionada
- [ ] Secret `GOOGLE_CLIENT_ID` adicionada
- [ ] Secret `GOOGLE_CLIENT_SECRET` adicionada
- [ ] Secret `BACKUP_EMAIL` adicionada
- [ ] Executou `node scripts/google-auth-setup.js`
- [ ] Secret `GOOGLE_REFRESH_TOKEN` adicionada
- [ ] Workflow "Server" está RUNNING
- [ ] Workflow "Ngrok" está RUNNING (ou `npm run ngrok` rodando)
- [ ] Console mostra: "✅ Autenticado no Google Drive"
- [ ] Consegue acessar a URL do ngrok no navegador
- [ ] Consegue fazer login (admin/admin)
- [ ] Criou um relatório de teste
- [ ] Verificou que o relatório apareceu no Google Drive

**🎉 PRONTO! Seu sistema está acessível na web!**
