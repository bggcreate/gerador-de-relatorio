# 🚀 Guia de Deploy no Replit (Servidor Central)

## 📋 Objetivo

Hospedar o backend Node.js no Replit para que **4 pessoas** possam usar os apps Electron conectados ao mesmo servidor, com **dados unificados** para análise.

---

## 🎯 Passo a Passo

### **1. Preparar o Projeto**

O projeto já está quase pronto! Você só precisa fazer algumas configurações.

#### **1.1. Criar arquivo `.env` para produção**

O Replit vai usar variáveis de ambiente. Configure no painel do Replit:

```env
NODE_ENV=production
PORT=5000
JWT_SECRET=coloque_um_secret_seguro_aqui_minimo_32_caracteres_aleatorios
SESSION_SECRET=outro_secret_diferente_minimo_32_caracteres_aleatorios
```

**Como gerar secrets seguros:**
```bash
# No terminal local ou Replit Shell:
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
```

---

### **2. Fazer Deploy no Replit**

#### **Opção A: Usando o botão Deploy do Replit** ⭐ RECOMENDADO

1. No seu Replit, clique no botão **"Deploy"** no topo
2. Escolha **"Autoscale"** (gratuito para começar)
3. Configure:
   - **Nome:** sistema-relatorios (ou outro de sua preferência)
   - **Comando de build:** (deixe vazio)
   - **Comando de run:** `node server.js`
4. Clique em **"Deploy"**
5. Aguarde o deploy (1-3 minutos)
6. Anote a URL gerada (ex: `https://sistema-relatorios.username.repl.co`)

#### **Configurações Importantes:**

No painel de Deploy, configure:
- **Environment Variables:** Adicione JWT_SECRET e SESSION_SECRET
- **Always On:** Ative se quiser que fique online 24/7 (pode ter custo)
- **Custom Domain:** (Opcional) Configure um domínio personalizado

---

### **3. Testar o Deploy**

Acesse a URL do deploy no navegador:
```
https://sua-url.repl.co
```

Você deve ver a tela de login do sistema!

**Credenciais padrão:**
- Usuário: `admin`
- Senha: `admin`

---

### **4. Configurar Apps Electron para Conectar**

Agora que o servidor está online, configure os apps:

#### **4.1. Editar `electron-main.js`**

Abra o arquivo `electron-main.js` e altere apenas **duas linhas** (19 e 20):

```javascript
// Encontre estas linhas (linhas 19-20):
const USE_REMOTE_SERVER = false;
const REMOTE_SERVER_URL = 'http://localhost:5000';

// E altere para:
const USE_REMOTE_SERVER = true;
const REMOTE_SERVER_URL = 'https://sua-url-do-replit.repl.co'; // Cole a URL do seu deploy
```

**Pronto!** O resto do código já está configurado para funcionar automaticamente.

#### **4.2. Gerar Executáveis**

```bash
npm run build:electron:win
```

Os executáveis gerados em `dist-electron/` estarão configurados para se conectar ao servidor Replit!

---

### **5. Distribuir para a Equipe**

1. Compartilhe os arquivos da pasta `dist-electron/`:
   - **Instalador:** `Sistema de Relatórios-Setup-1.0.0.exe`
   - **Portátil:** `Sistema de Relatórios-Portable-1.0.0.exe`

2. Cada pessoa:
   - Instala ou executa o arquivo portátil
   - Faz login no sistema
   - Trabalha com os mesmos dados!

---

## 🔒 Segurança

### **Usuários e Permissões**

Por padrão, existe apenas o usuário `admin`. Para a equipe:

1. Acesse o sistema como admin
2. Vá em **"Gerenciar Usuários"**
3. Crie usuários para cada membro da equipe
4. Configure permissões conforme necessário

### **Boas Práticas:**

✅ Mude a senha do admin imediatamente  
✅ Crie usuários individuais para cada pessoa  
✅ Use senhas fortes  
✅ Configure JWT_SECRET e SESSION_SECRET seguros  
✅ Faça backups regulares do banco de dados  

---

## 💾 Backup do Banco de Dados

### **Backup Manual:**

1. No Replit, vá para a pasta `data/`
2. Baixe o arquivo `database.db`
3. Guarde em local seguro

### **Backup Automático (Opcional):**

O sistema já tem integração com Google Drive. Configure:

1. Obtenha credenciais da API do Google Drive
2. Configure no `.env`:
   ```env
   GOOGLE_CLIENT_ID=seu_client_id
   GOOGLE_CLIENT_SECRET=seu_client_secret
   GOOGLE_REFRESH_TOKEN=seu_refresh_token
   ```
3. O sistema fará backups automáticos

---

## 📊 Monitoramento

### **Ver Logs do Servidor:**

No Replit, vá em **"Logs"** para ver:
- Acessos ao sistema
- Erros
- Ações dos usuários

### **Métricas de Uso:**

No painel de Deploy do Replit, você pode ver:
- Número de requisições
- Uso de CPU/Memória
- Tempo de resposta

---

## 💰 Custos (Replit)

### **Plano Gratuito:**
✅ Suficiente para 4 pessoas  
✅ Deploy básico incluído  
⚠️ Pode dormir após inatividade  

### **Plano Hacker ($7/mês):**
✅ Always On (24/7)  
✅ Mais recursos  
✅ Melhor performance  

### **Plano Pro ($20/mês):**
✅ Múltiplos deploys  
✅ Recursos dedicados  
✅ Suporte prioritário  

**Recomendação:** Comece com o gratuito, atualize se necessário.

---

## 🔧 Troubleshooting

### **Deploy falha:**
- Verifique se todas as dependências estão no `package.json`
- Certifique-se que o comando `node server.js` funciona localmente
- Verifique os logs de erro no Replit

### **Apps não conectam ao servidor:**
- Teste a URL no navegador primeiro
- Verifique se a URL no `electron-main.js` está correta (sem barra no final)
- Verifique se o servidor está online no Replit

### **Banco de dados não persiste:**
- Certifique-se que a pasta `data/` existe
- Configure o caminho correto do banco no `.env`
- Use Replit Storage se necessário

---

## ✅ Checklist Final

Antes de liberar para a equipe:

- [ ] Deploy funcionando no Replit
- [ ] Variáveis de ambiente configuradas
- [ ] Senha do admin alterada
- [ ] Usuários da equipe criados
- [ ] Apps Electron configurados com URL do servidor
- [ ] Executáveis gerados e testados
- [ ] Backup inicial do banco criado
- [ ] Equipe treinada no uso básico

---

## 🎯 Resultado Final

Após seguir este guia:

✅ Servidor online 24/7 no Replit (gratuito)  
✅ 4 pessoas trabalhando simultaneamente  
✅ Dados unificados e sincronizados  
✅ Apps desktop profissionais  
✅ Análise de dados centralizada  
✅ Zero custo de infraestrutura  

---

**Pronto para começar! 🚀**
