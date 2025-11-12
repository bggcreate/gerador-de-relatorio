# 🖥️ Sistema de Relatórios - Versão Desktop (Electron)

## 📋 Visão Geral

Este sistema agora pode ser executado como uma **aplicação desktop profissional** usando Electron, permitindo que sua equipe trabalhe com uma interface nativa do Windows.

## 🏗️ Arquitetura para Equipe (4+ Pessoas)

### **Modelo Recomendado: Cliente-Servidor**

```
┌─────────────────────────────────────────┐
│   SERVIDOR CENTRAL (Replit Deploy)      │
│   - Backend Node.js/Express             │
│   - Banco de dados SQLite único         │
│   - API REST                            │
│   - Sempre online (24/7)                │
└──────────────┬──────────────────────────┘
               │ Internet
     ┌─────────┼─────────┬────────────┐
     │         │         │            │
┌────▼───┐ ┌──▼────┐ ┌──▼────┐  ┌───▼────┐
│ PC 1   │ │ PC 2  │ │ PC 3  │  │ PC 4   │
│Electron│ │Electron│ │Electron│  │Electron│
│Desktop │ │Desktop│ │Desktop│  │Desktop │
└────────┘ └───────┘ └───────┘  └────────┘
```

### **Vantagens desta arquitetura:**
✅ Dados centralizados e sincronizados em tempo real  
✅ Múltiplos usuários trabalhando simultaneamente  
✅ Backup automático (um único banco de dados)  
✅ Análise de dados unificada  
✅ Gratuito com Replit  

---

## 🚀 Como Usar

### **Opção 1: Desenvolvimento/Teste Local**

Teste a aplicação Electron no seu computador:

```bash
# Instalar dependências (se ainda não instalou)
npm install

# Executar em modo Electron
npm run electron
```

Isso abrirá uma janela desktop com o sistema rodando.

---

### **Opção 2: Gerar Executável para Distribuir**

Crie arquivos `.exe` para distribuir para sua equipe:

```bash
# Gerar instalador + versão portátil para Windows
npm run build:electron:win

# Gerar para Mac (se estiver em Mac)
npm run build:electron:mac

# Gerar para Linux
npm run build:electron:linux
```

**Arquivos gerados em `dist-electron/`:**
- `Sistema de Relatórios-Setup-1.0.0.exe` → Instalador completo
- `Sistema de Relatórios-Portable-1.0.0.exe` → Versão portátil (não precisa instalar)

---

## 🌐 Configuração para Equipe (Servidor + Apps)

### **Passo 1: Deploy do Servidor no Replit**

1. **Fazer Deploy deste projeto no Replit:**
   - Clique no botão "Deploy" no Replit
   - Configure para rodar 24/7
   - Anote a URL do deploy (ex: `https://seu-projeto.replit.app`)

2. **Configure as variáveis de ambiente no Replit:**
   ```
   NODE_ENV=production
   JWT_SECRET=seu_secret_seguro_aqui_123456789
   SESSION_SECRET=outro_secret_seguro_aqui_987654321
   ```

### **Passo 2: Configurar Apps Electron para Conectar ao Servidor**

Antes de gerar os executáveis, você precisa configurar a URL do servidor:

**Edite o arquivo `electron-main.js` (linhas 19-20):**

```javascript
// ALTERE ESTAS LINHAS:
const USE_REMOTE_SERVER = false;
const REMOTE_SERVER_URL = 'http://localhost:5000';

// PARA (use a URL do seu Replit Deploy):
const USE_REMOTE_SERVER = true;
const REMOTE_SERVER_URL = 'https://seu-projeto.replit.app';
```

**Depois gere os executáveis:**
```bash
npm run build:electron:win
```

### **Passo 3: Distribuir para a Equipe**

1. Compartilhe o arquivo `.exe` com cada membro da equipe
2. Cada pessoa instala ou executa o arquivo portátil
3. Todos se conectam ao mesmo servidor central
4. Dados sincronizados automaticamente!

---

## 📁 Modo de Operação

### **Modo Atual (Desktop Standalone)**
O Electron inicia um servidor local e abre uma interface desktop. Cada PC tem seu próprio banco de dados.

**Uso:** Ideal para trabalho individual ou testes.

### **Modo Servidor Central (Recomendado para Equipe)**
Modifique `electron-main.js` para conectar a um servidor remoto ao invés de iniciar um local:

```javascript
// Configure estas constantes (linhas 19-20):
const USE_REMOTE_SERVER = true; // Mude para true
const REMOTE_SERVER_URL = 'https://seu-servidor.replit.app'; // Cole a URL do Replit
```

O sistema automaticamente:
- **Não iniciará** um servidor local
- **Conectará** diretamente ao servidor remoto
- **Permitirá** múltiplos usuários simultâneos

**Uso:** Ideal para equipes de 4+ pessoas com dados unificados.

---

## 🛠️ Personalização

### **Alterar Ícone do Aplicativo**

Substitua o arquivo `build/icon.png` por seu próprio ícone (512x512px recomendado).

### **Alterar Nome do Aplicativo**

Edite o `package.json`:
```json
{
  "name": "seu-nome-aqui",
  "productName": "Seu Nome Profissional",
  "description": "Sua descrição"
}
```

### **Configurar Auto-Update**

Para versões futuras incluírem atualização automática, você pode usar:
- **electron-updater** (grátis com GitHub Releases)
- Documentação: https://www.electron.build/auto-update

---

## 📊 Benefícios do Electron

✅ **Interface Nativa:** Aplicativo se comporta como software Windows  
✅ **Offline:** Pode funcionar sem internet (modo standalone)  
✅ **Instalável:** Cria ícone no menu iniciar e desktop  
✅ **Profissional:** Parece software comercial  
✅ **Multiplataforma:** Windows, Mac, Linux  
✅ **Auto-contido:** Não precisa instalar Node.js separadamente  

---

## 🔍 Troubleshooting

### **Aplicativo não abre:**
- Verifique se o arquivo `server.js` existe
- Execute `npm install` novamente
- Tente executar: `npm run electron:dev` para ver logs de erro

### **Erro ao gerar executável:**
- Certifique-se que todas as dependências estão instaladas
- Limpe a pasta `dist-electron/` e tente novamente
- Verifique se tem espaço em disco suficiente (~500MB necessários)

### **Servidor não conecta:**
- Verifique se a URL no `electron-main.js` está correta
- Teste a URL no navegador primeiro
- Certifique-se que o servidor Replit está online

---

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique os logs do Electron: `npm run electron:dev`
2. Verifique os logs do servidor no Replit
3. Consulte a documentação do Electron: https://www.electronjs.org/docs

---

## 🎯 Próximos Passos

1. ✅ Estrutura Electron criada
2. ⏳ Deploy do servidor no Replit
3. ⏳ Configurar URL do servidor nos apps
4. ⏳ Gerar executáveis finais
5. ⏳ Distribuir para a equipe

---

**Desenvolvido com ❤️ usando Node.js + Electron**
