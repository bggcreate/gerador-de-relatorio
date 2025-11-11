# 🌐 Guia Completo: Acesso Remoto via DDNS

Este guia mostra como acessar seu sistema de qualquer lugar da internet usando **DDNS gratuito**.

---

## 📋 O que você vai conseguir fazer

✅ Acessar o sistema de qualquer lugar (casa, trabalho, celular)  
✅ URL fixa e fácil de lembrar (ex: `minhaloja.duckdns.org`)  
✅ Dados salvos no seu PC (banco local SQLite)  
✅ **100% GRATUITO** (sem mensalidades)

---

## 🎯 Como Funciona

```
Você (Trabalho/Casa/Celular)
         ↓
    Internet
         ↓
meuservidor.duckdns.org  (nome fixo)
         ↓
    Seu IP público (muda toda hora, mas DDNS atualiza automaticamente)
         ↓
    Seu Roteador (porta 5000 redirecionada)
         ↓
    Seu PC (sistema rodando)
```

---

## 🏆 Melhores Opções de DDNS Gratuito

### 🥇 **DuckDNS** (RECOMENDADO)
- ✅ **100% gratuito** para sempre
- ✅ **Sem confirmação mensal** (No-IP exige)
- ✅ Até **5 domínios**
- ✅ Super simples de configurar
- ✅ Servidores na AWS (rápido)

**Site:** https://www.duckdns.org

### 🥈 **No-IP**
- ✅ Mais conhecido
- ✅ Até **3 hostnames**
- ✅ Integração nativa em muitos roteadores
- ⚠️ Requer confirmação **a cada 30 dias** (email)

**Site:** https://www.noip.com

### 🥉 **Dynu**
- ✅ Hostnames **ilimitados**
- ✅ Muito completo
- ✅ Sem confirmação mensal

**Site:** https://www.dynu.com

---

## 🚀 PASSO A PASSO: Configurar DuckDNS (Recomendado)

### ✅ ETAPA 1: Criar seu domínio DuckDNS

1. Acesse: **https://www.duckdns.org**
2. Faça login com **Google, GitHub ou Twitter**
3. No campo, digite um nome (ex: `minhaloja`)
4. Clique em **"add domain"**
5. ✅ Pronto! Seu domínio: **minhaloja.duckdns.org**
6. **ANOTE O TOKEN** exibido (algo como: `a7c4d0ad-114e-40ef-ba1d-...`)

---

### ✅ ETAPA 2: Configurar atualização automática do IP

Seu IP público muda frequentemente. O DuckDNS precisa saber quando isso acontece.

#### **No Windows:**

**Opção A: Script Simples (Recomendado)**

1. Crie uma pasta: `C:\DuckDNS`
2. Dentro dela, crie um arquivo: `atualizar-ip.bat`
3. Abra com Bloco de Notas e cole:

```batch
@echo off
REM Substitua SEUDOMINIO e SEUTOKEN pelos seus dados
curl "https://www.duckdns.org/update?domains=SEUDOMINIO&token=SEUTOKEN&ip="
timeout /t 5
```

**Exemplo real:**
```batch
@echo off
curl "https://www.duckdns.org/update?domains=minhaloja&token=a7c4d0ad-114e-40ef-ba1d&ip="
timeout /t 5
```

4. **Configurar para rodar automaticamente** (a cada 5 minutos):
   - Pressione `Win + R`
   - Digite: `taskschd.msc` e Enter
   - Clique em **"Criar Tarefa Básica"**
   - Nome: `DuckDNS Atualização`
   - Disparador: **Ao iniciar o computador**
   - Ação: **Iniciar um programa**
   - Programa: `C:\DuckDNS\atualizar-ip.bat`
   - Marque: ✅ **Executar com privilégios mais altos**
   - Em "Gatilhos", adicione também: **Repetir a cada 5 minutos**

**Opção B: PowerShell (Avançado)**

Crie `C:\DuckDNS\update.ps1`:
```powershell
$domain = "SEUDOMINIO"
$token = "SEUTOKEN"
Invoke-WebRequest "https://www.duckdns.org/update?domains=$domain&token=$token&ip="
```

Configure no Agendador de Tarefas para executar:
```
powershell.exe -ExecutionPolicy Bypass -File "C:\DuckDNS\update.ps1"
```

#### **No Linux/Mac:**

1. Crie o script:
```bash
mkdir ~/duckdns
nano ~/duckdns/duck.sh
```

2. Cole este conteúdo (substitua valores):
```bash
#!/bin/bash
echo url="https://www.duckdns.org/update?domains=SEUDOMINIO&token=SEUTOKEN&ip=" | curl -k -o ~/duckdns/duck.log -K -
```

3. Dê permissão:
```bash
chmod 700 ~/duckdns/duck.sh
```

4. Adicione ao crontab (atualiza a cada 5 minutos):
```bash
crontab -e
```

Adicione esta linha:
```
*/5 * * * * ~/duckdns/duck.sh >/dev/null 2>&1
```

5. Teste manualmente:
```bash
~/duckdns/duck.sh
cat ~/duckdns/duck.log
```

Deve retornar: `OK`

---

### ✅ ETAPA 3: Descobrir seu IP local

Você precisa saber o IP do seu PC na rede local.

#### **No Windows:**
```cmd
ipconfig
```

Procure por: `Endereço IPv4`  
Exemplo: **192.168.1.100**

#### **No Linux/Mac:**
```bash
ifconfig
# ou
ip addr show
```

---

### ✅ ETAPA 4: Fixar IP local (Importante!)

Para evitar que o IP mude na rede interna:

#### **No Windows:**

1. Painel de Controle → Rede e Internet → Central de Rede
2. Clique em sua conexão → **Propriedades**
3. Selecione **Protocolo IP Versão 4 (TCP/IPv4)** → **Propriedades**
4. Marque: **Usar o seguinte endereço IP**
5. Preencha:
   - **Endereço IP:** 192.168.1.100 (escolha um IP livre)
   - **Máscara:** 255.255.255.0
   - **Gateway:** 192.168.1.1 (IP do seu roteador)
   - **DNS:** 8.8.8.8 (Google) ou 1.1.1.1 (Cloudflare)
6. OK → OK

#### **No Linux:**

Edite `/etc/network/interfaces`:
```bash
auto eth0
iface eth0 inet static
  address 192.168.1.100
  netmask 255.255.255.0
  gateway 192.168.1.1
  dns-nameservers 8.8.8.8
```

---

### ✅ ETAPA 5: Configurar Port Forwarding no Roteador

**O que é Port Forwarding?**  
Diz ao roteador: "Quando alguém acessar porta 5000 de fora, redirecione para o PC 192.168.1.100"

#### **Passo a Passo:**

1. **Descubra o IP do roteador:**
   - Windows: `ipconfig` → **Gateway Padrão**
   - Geralmente: `192.168.0.1` ou `192.168.1.1` ou `10.0.0.1`

2. **Acesse o roteador:**
   - Abra navegador: `http://192.168.1.1`
   - Login (comum): **admin/admin** ou **admin/senha** (veja etiqueta do roteador)

3. **Localize Port Forwarding:**
   - Procure por:
     - **Port Forwarding**
     - **Redirecionamento de Portas**
     - **Virtual Server**
     - **NAT**
     - **Aplicativos e Jogos** (alguns roteadores)

4. **Criar nova regra:**

   | Campo | Valor |
   |-------|-------|
   | **Nome/Descrição** | Sistema-Relatorios |
   | **Porta Externa** | 5000 (ou outra, ex: 8080) |
   | **IP Interno** | 192.168.1.100 (seu PC) |
   | **Porta Interna** | 5000 |
   | **Protocolo** | TCP ou TCP/UDP |

   **⚠️ IMPORTANTE:** Alguns provedores bloqueiam portas comuns (80, 443, 22). Se não funcionar, use porta alternativa como **8080** ou **3000**.

5. **Salvar e Reiniciar o Roteador**

---

### ✅ ETAPA 6: Testar se está funcionando

#### **1. Testar internamente (mesma rede):**

Abra navegador: `http://192.168.1.100:5000`  
✅ Deve aparecer a tela de login

#### **2. Descobrir seu IP público:**

Acesse: https://www.meuip.com.br  
Anote o IP exibido (ex: `201.34.123.45`)

#### **3. Verificar se DuckDNS está apontando certo:**

```bash
ping minhaloja.duckdns.org
```

O IP retornado deve ser **igual** ao seu IP público.

#### **4. Testar se porta está aberta:**

Acesse: https://www.yougetsignal.com/tools/open-ports/

- **Remote Address:** `minhaloja.duckdns.org`
- **Port Number:** `5000`
- Clique em **Check**

✅ Deve mostrar: **Port 5000 is open**

#### **5. Testar acesso externo:**

**IMPORTANTE:** Teste usando dados móveis (4G/5G) ou de outra rede (não a mesma Wi-Fi)

Abra navegador: `http://minhaloja.duckdns.org:5000`  
✅ Deve aparecer a tela de login do sistema!

---

## 🔒 SEGURANÇA (MUITO IMPORTANTE!)

Seu sistema estará acessível pela internet. **PROTEJA-O!**

### ✅ Configurações Obrigatórias:

1. **Senha forte:**
   - Troque a senha padrão `admin/admin`
   - Use no mínimo 12 caracteres
   - Misture letras, números e símbolos

2. **Use HTTPS (SSL):**
   - Obtenha certificado SSL gratuito com Let's Encrypt
   - Ou use Cloudflare (proxy gratuito com SSL)

3. **Evite porta padrão:**
   - Em vez de 5000, use 8080, 3000, 7777
   - Portas não-padrão dificultam ataques automatizados

4. **Firewall do Windows ativo:**
   - Mantenha firewall ligado
   - Libere apenas porta 5000 (ou a escolhida)

5. **Mantenha sistema atualizado:**
   - Atualize Node.js regularmente
   - Atualize dependências npm

6. **Monitore logs de acesso:**
   - Verifique acessos suspeitos
   - Use `fail2ban` (Linux) para bloquear IPs maliciosos

---

## 📱 Acessando de Diferentes Dispositivos

### **Do PC/Notebook (fora de casa):**
```
http://minhaloja.duckdns.org:5000
```

### **Do Celular/Tablet:**
```
http://minhaloja.duckdns.org:5000
```

### **Criar atalho no celular (Android/iPhone):**

1. Abra o site no navegador
2. Menu (3 pontos) → **Adicionar à tela inicial**
3. ✅ Agora você tem um ícone como se fosse um app!

---

## 💾 Confirmação: Dados Salvos Localmente

**Seu sistema JÁ ESTÁ configurado para salvar tudo no seu PC!**

📂 **Localização dos dados:**
```
Seu-Projeto/
└── data/
    ├── database.db          ← Banco de dados SQLite (TODOS os relatórios)
    ├── pdfs/                ← PDFs de tickets e rankings
    │   ├── tickets/
    │   └── rankings/
    └── dvr_files/           ← Arquivos do DVR/NVR
```

**Banco de dados:** SQLite (arquivo local `database.db`)  
**Localização:** Pasta `data/` no projeto  
**Tamanho:** Cresce conforme você adiciona relatórios

### ✅ Vantagens:

- ✅ **Controle total** dos dados
- ✅ **Rápido** (não depende de internet)
- ✅ **Gratuito** (sem custo de servidor)
- ✅ **Fácil backup** (copie a pasta `data/`)

---

## 🔄 Preparação para Banco de Dados em Servidor (Futuro)

Quando você quiser migrar para servidor externo, já está preparado:

### **Opções futuras:**

1. **PostgreSQL em servidor VPS**
2. **MySQL/MariaDB**
3. **MongoDB**
4. **Supabase** (PostgreSQL gratuito na nuvem)

### **Migração será simples:**

O sistema já tem estrutura preparada. Bastará:
1. Instalar driver do novo banco
2. Exportar dados do SQLite
3. Importar no novo banco
4. Alterar string de conexão

**Você não perderá nada!** Todos os dados serão migrados.

---

## 🆘 Solução de Problemas

### ❌ "Não consigo acessar de fora"

**Checklist:**
1. ✅ DuckDNS está atualizando? (ping deve retornar seu IP público)
2. ✅ Port Forwarding configurado no roteador?
3. ✅ Porta está aberta? (teste em yougetsignal.com)
4. ✅ Firewall do Windows não está bloqueando?
5. ✅ Sistema está rodando? (verifique `http://localhost:5000`)

**Teste em etapas:**
```bash
# 1. Teste local
http://localhost:5000

# 2. Teste na rede local
http://192.168.1.100:5000

# 3. Teste com IP público (dados móveis)
http://SEU-IP-PUBLICO:5000

# 4. Teste com domínio (dados móveis)
http://minhaloja.duckdns.org:5000
```

### ❌ "Porta aparece fechada"

**Possíveis causas:**
1. **Provedor bloqueando:** Alguns provedores bloqueiam portas
   - **Solução:** Use porta alternativa (8080, 3000, 7777)
2. **Firewall do roteador:** Desative temporariamente para testar
3. **Duplo NAT:** Se você tem 2 roteadores, configure ambos

### ❌ "DuckDNS não atualiza"

**Verificar:**
```bash
# Windows
curl "https://www.duckdns.org/update?domains=SEUDOMINIO&token=SEUTOKEN&verbose=true"

# Deve retornar:
# OK
# IP ATUAL
# CHANGED/NO CHANGE
```

### ❌ "Site carrega mas não consigo fazer login"

**Verificar:**
1. Cookies habilitados no navegador
2. Se usou IP público direto, use domínio DuckDNS
3. Limpe cache do navegador

---

## 📊 Resumo Visual

```
┌─────────────────────────────────────────────────────────┐
│  CONFIGURAÇÃO COMPLETA                                  │
└─────────────────────────────────────────────────────────┘

1. ✅ DuckDNS criado: minhaloja.duckdns.org
2. ✅ Script rodando: Atualiza IP a cada 5 minutos
3. ✅ IP fixo no PC: 192.168.1.100
4. ✅ Port Forwarding: Porta 5000 → PC
5. ✅ Sistema rodando: localhost:5000
6. ✅ Acesso externo: minhaloja.duckdns.org:5000

┌─────────────────────────────────────────────────────────┐
│  DADOS                                                  │
└─────────────────────────────────────────────────────────┘

📂 Local: data/database.db (SQLite)
💾 Backup: Copie pasta data/ regularmente
🔄 Futura migração: PostgreSQL/MySQL (simples)
```

---

## 🎯 Checklist Final

Antes de considerar tudo configurado:

- [ ] DuckDNS criado e token anotado
- [ ] Script de atualização rodando automaticamente
- [ ] IP fixo configurado no PC
- [ ] Port Forwarding configurado no roteador
- [ ] Sistema acessível internamente (192.168.1.100:5000)
- [ ] Porta testada e aberta (yougetsignal.com)
- [ ] Domínio testado de rede externa (dados móveis)
- [ ] Senha admin alterada para senha forte
- [ ] Backup da pasta `data/` criado

---

## 💰 Custos

**TOTALMENTE GRATUITO!**

- ✅ DuckDNS: Grátis
- ✅ SQLite: Grátis
- ✅ Node.js: Grátis
- ✅ Sistema: Grátis
- ✅ Seu PC: Você já tem

**Custo mensal:** R$ 0,00

---

## 📞 Suporte Rápido

**Links úteis:**
- DuckDNS: https://www.duckdns.org
- Testar portas: https://www.yougetsignal.com/tools/open-ports/
- Ver meu IP: https://www.meuip.com.br
- Qual meu IP: https://whatismyipaddress.com

**Comandos úteis:**

```bash
# Ver IP local
ipconfig (Windows)
ifconfig (Linux/Mac)

# Testar conexão local
ping 192.168.1.100

# Ver IP público
curl https://api.ipify.org

# Testar DuckDNS
ping minhaloja.duckdns.org
```

---

✅ **Pronto! Seu sistema está acessível de qualquer lugar da internet!** 🚀

Com dados salvos localmente no seu PC e preparado para migração futura quando precisar!
