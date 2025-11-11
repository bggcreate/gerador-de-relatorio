# 🖥️ Como Rodar no Seu PC (Sair do Replit)

Este guia mostra como baixar o sistema do Replit e rodar no seu próprio computador.

---

## 📋 PASSO 1: Baixar o Projeto do Replit

### Opção A: Download Direto (Mais Fácil)

1. No Replit, clique nos **3 pontos** ao lado do nome do projeto
2. Selecione **"Download as ZIP"**
3. Salve o arquivo (ex: `projeto.zip`)
4. **Descompacte** em uma pasta no seu PC (ex: `C:\SistemaRelatorios`)

### Opção B: Git Clone (Para quem sabe Git)

```bash
# No seu PC, abra terminal/CMD na pasta desejada
git clone URL_DO_SEU_REPLIT
```

---

## 📋 PASSO 2: Instalar Node.js no seu PC

1. Acesse: https://nodejs.org
2. Baixe a versão **LTS** (recomendada)
3. Execute o instalador
4. **Importante:** Marque a opção "Add to PATH"
5. Reinicie o PC após instalação

**Verificar instalação:**
```cmd
node --version
npm --version
```

Deve mostrar as versões (ex: `v20.11.0`)

---

## 📋 PASSO 3: Instalar Dependências

1. Abra **CMD** ou **PowerShell** como Administrador
2. Navegue até a pasta do projeto:
```cmd
cd C:\SistemaRelatorios
```

3. Instale os pacotes:
```cmd
npm install
```

Aguarde 1-2 minutos enquanto baixa tudo.

---

## 📋 PASSO 4: Descobrir seu IP Local

No CMD, digite:
```cmd
ipconfig
```

Procure por **"Endereço IPv4"** em "Adaptador de Rede Sem Fio" ou "Ethernet"

Exemplo: `192.168.1.105`

**ANOTE ESTE IP!** Você vai precisar.

---

## 📋 PASSO 5: Iniciar o Sistema

No CMD, na pasta do projeto:
```cmd
npm start
```

Deve aparecer:
```
Servidor rodando em http://0.0.0.0:5000
```

**✅ TESTE AGORA:**
- Abra navegador: `http://localhost:5000`
- Deve aparecer a tela de login!

**Credenciais padrão:**
- Usuário: `admin`
- Senha: `admin`

---

## 🌐 PASSO 6: Configurar Acesso pela Rede Local

Outros dispositivos na **mesma rede** (Wi-Fi/cabo) já conseguem acessar!

**No celular/outro PC (mesma rede Wi-Fi):**

Abra navegador e acesse (substitua pelo SEU IP):
```
http://192.168.1.105:5000
```

Se funcionar, ótimo! Próximo passo é acesso pela internet.

---

## 🌍 PASSO 7: Configurar DDNS (Acesso pela Internet)

### 7.1 - Criar conta DuckDNS

1. Acesse: https://www.duckdns.org
2. Faça login com **Google, GitHub ou Twitter**
3. Digite um nome (ex: `minhaloja`)
4. Clique em **"add domain"**
5. ✅ Seu domínio: `minhaloja.duckdns.org`
6. **ANOTE O TOKEN** (algo como: `a7c4d0ad-114e-40ef-ba1d`)

### 7.2 - Criar Script de Atualização de IP

**No Windows:**

1. Crie uma pasta: `C:\DuckDNS`
2. Crie arquivo: `C:\DuckDNS\atualizar.bat`
3. Cole este conteúdo (SUBSTITUA pelos seus dados):

```batch
@echo off
curl "https://www.duckdns.org/update?domains=SEUDOMINIO&token=SEUTOKEN&ip="
timeout /t 5
```

**Exemplo real:**
```batch
@echo off
curl "https://www.duckdns.org/update?domains=minhaloja&token=a7c4d0ad-114e-40ef&ip="
timeout /t 5
```

4. **Teste:** Clique duas vezes no arquivo
   - Deve aparecer uma janela com `OK`

5. **Configurar para rodar automaticamente:**

   - Pressione `Win + R`
   - Digite: `taskschd.msc` (Enter)
   - **Ações** → **Criar Tarefa Básica**
   - Nome: `DuckDNS`
   - Disparador: **Ao fazer logon**
   - Ação: **Iniciar programa**
   - Programa: `C:\DuckDNS\atualizar.bat`
   - ✅ Marcar: **Executar com privilégios mais altos**
   
   **Depois, edite a tarefa:**
   - Clique com botão direito → **Propriedades**
   - Aba **Gatilhos** → **Editar**
   - ✅ Marcar: **Repetir a tarefa a cada:** `5 minutos`
   - **Por:** `Indefinidamente`

---

## 🔧 PASSO 8: Fixar IP Local do PC

Para o IP não mudar:

1. **Painel de Controle** → **Rede e Internet** → **Central de Rede**
2. Clique na sua conexão → **Propriedades**
3. Selecione **Protocolo IP Versão 4 (TCP/IPv4)** → **Propriedades**
4. Marque: **Usar o seguinte endereço IP:**

Preencha:
```
Endereço IP: 192.168.1.105  (o IP que você anotou)
Máscara: 255.255.255.0
Gateway: 192.168.1.1  (geralmente é esse, ou 192.168.0.1)
DNS: 8.8.8.8
DNS alternativo: 8.8.4.4
```

5. OK → OK

---

## 🔀 PASSO 9: Configurar Roteador (Port Forwarding)

**ESTE É O PASSO MAIS IMPORTANTE!**

### 9.1 - Acessar o Roteador

No navegador, digite um destes IPs:
- `http://192.168.1.1`
- `http://192.168.0.1`
- `http://10.0.0.1`

**Login:** Geralmente `admin/admin` ou `admin/senha`
(Veja na etiqueta do roteador)

### 9.2 - Encontrar Port Forwarding

Procure por um destes nomes:
- **Port Forwarding**
- **Encaminhamento de Portas**
- **Virtual Server**
- **NAT**
- **Aplicativos e Jogos**

### 9.3 - Criar Nova Regra

Preencha assim:

| Campo | Valor |
|-------|-------|
| **Nome** | Sistema Relatorios |
| **Porta Externa** | 5000 |
| **IP Interno** | 192.168.1.105 (seu IP fixo) |
| **Porta Interna** | 5000 |
| **Protocolo** | TCP |

**⚠️ IMPORTANTE:** Se a porta 5000 não funcionar (provedor pode bloquear), use outra:
- **8080**, **3000**, **7777**, **8888**

### 9.4 - Salvar e Reiniciar Roteador

Salve as configurações e reinicie o roteador.

---

## ✅ PASSO 10: TESTAR TUDO!

### Teste 1: Local
```
http://localhost:5000
```
✅ Deve funcionar

### Teste 2: Rede Local
```
http://192.168.1.105:5000
```
✅ Deve funcionar

### Teste 3: Verificar IP Público

Acesse: https://www.meuip.com.br

Anote o IP (ex: `189.45.123.200`)

### Teste 4: Testar DuckDNS

No CMD:
```cmd
ping minhaloja.duckdns.org
```

O IP retornado deve ser **igual** ao seu IP público.

### Teste 5: Verificar Porta Aberta

Acesse: https://www.yougetsignal.com/tools/open-ports/

- **Remote Address:** `minhaloja.duckdns.org`
- **Port:** `5000`
- Clique em **Check**

✅ Deve mostrar: **"Port 5000 is open"**

### Teste 6: ACESSO FINAL! 🎉

**IMPORTANTE:** Teste de **OUTRA REDE** (dados móveis do celular)

No navegador do celular (4G/5G ligado, Wi-Fi desligado):
```
http://minhaloja.duckdns.org:5000
```

✅ Deve aparecer a tela de login!

---

## 🎯 RESUMO VISUAL

```
┌──────────────────────────────────────────┐
│  SEU SISTEMA FUNCIONANDO                 │
└──────────────────────────────────────────┘

Internet
   ↓
minhaloja.duckdns.org:5000
   ↓
Seu IP Público (atualizado a cada 5min)
   ↓
Roteador (redireciona porta 5000)
   ↓
Seu PC (192.168.1.105:5000)
   ↓
Sistema rodando!
```

---

## 🔒 SEGURANÇA

**FAÇA ISSO AGORA:**

1. **Trocar senha padrão:**
   - Login com admin/admin
   - Vá em configurações
   - Troque para senha forte (12+ caracteres)

2. **Firewall do Windows:**
   - Deixe ativo
   - Libere apenas porta 5000

3. **Mantenha sistema rodando:**
   - Configure para iniciar com Windows (opcional)

---

## 🚀 Fazer Sistema Iniciar com Windows (Opcional)

### Opção 1: Criar arquivo .bat

1. Crie `C:\SistemaRelatorios\iniciar.bat`:
```batch
@echo off
cd C:\SistemaRelatorios
npm start
```

2. Pressione `Win + R`
3. Digite: `shell:startup`
4. Copie `iniciar.bat` para essa pasta

✅ Agora inicia automaticamente com Windows!

### Opção 2: Usar PM2 (Profissional)

```cmd
npm install -g pm2
pm2 start server.js --name "sistema-relatorios"
pm2 startup
pm2 save
```

---

## 📞 CHECKLIST FINAL

- [ ] Node.js instalado
- [ ] Projeto baixado do Replit
- [ ] `npm install` executado com sucesso
- [ ] Sistema roda localmente (localhost:5000)
- [ ] IP local anotado e fixado
- [ ] DuckDNS criado e script rodando
- [ ] Port Forwarding configurado no roteador
- [ ] Porta 5000 testada e aberta
- [ ] Acesso externo testado (celular 4G)
- [ ] Senha admin alterada

---

## ❓ Problemas Comuns

**"Porta fechada"**
→ Use porta alternativa (8080, 3000)

**"Não consigo acessar de fora"**
→ Teste cada passo: local → rede → internet

**"DuckDNS não atualiza"**
→ Verifique se script está rodando no Agendador de Tarefas

**"Sistema não inicia"**
→ Verifique se rodou `npm install` completo

---

✅ **PRONTO! Seu sistema está rodando no SEU PC com acesso pela internet!** 🎉

Dados salvos localmente em: `C:\SistemaRelatorios\data\database.db`
