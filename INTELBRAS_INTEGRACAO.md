# 🔌 Integração com DVRs Intelbras - API HTTP Nativa (EXPANDIDA)

## 🎯 Como Funciona

Criei um **serviço Node.js** completo que se conecta **diretamente aos DVRs Intelbras** via **API HTTP**, sem precisar das DLLs do Windows.

### ✨ Novas Funcionalidades Implementadas (baseadas no SDK)
✅ **Controle PTZ** - Movimentação de câmeras (Up, Down, Left, Right, diagonais)
✅ **Presets PTZ** - Salvar e ir para posições pré-definidas (até 255 presets)
✅ **Zoom PTZ** - Controle de zoom (aproximar/afastar)
✅ **Snapshots** - Captura de imagens das câmeras
✅ **Busca de Gravações** - Localizar vídeos gravados por período
✅ **URLs RTSP** - Streaming ao vivo e playback de gravações
✅ **Informações de Canais** - Configuração e nomes dos canais
✅ **Controle de Gravação** - Iniciar/parar gravação manual

### Por que não usar as DLLs do SDK?
- As DLLs (.dll) do SDK em `bin/NetSDK 3.050/` são bibliotecas Windows que **não funcionam no Linux** (Replit)
- Mas os DVRs Intelbras expõem uma **API HTTP nativa** que podemos usar de qualquer sistema operacional!
- A integração via HTTP API oferece **TODAS as funcionalidades do SDK** sem dependências de SO

---

## 🚀 CONFIGURAÇÃO RÁPIDA

### 1. Pré-requisitos

Para que a integração funcione, você precisa:

✅ **Acesso à rede local dos DVRs** (IP direto ou DDNS)
✅ **Usuário e senha** de cada DVR
✅ **Porta HTTP aberta** (geralmente porta 80)

⚠️ **NÃO funciona com Intelbras Cloud ID** (limitação da Intelbras)

### 2. Instalar Dependências

```bash
npm install axios
```

### 3. Configurar Senhas dos DVRs

Edite o arquivo `scripts/collect-dvr-logs.js`:

```javascript
const DVR_PASSWORDS = {
    1: 'admin123',        // DVR ID 1
    2: 'senha_segura',    // DVR ID 2
    3: 'outra_senha',     // DVR ID 3
};
```

⚠️ **Importante:** Nunca commit senhas no Git! Este arquivo deve ser local.

---

## 📡 TESTANDO A CONEXÃO

### Teste Manual com curl:

```bash
# Substituir: IP, PORTA, USER, SENHA
curl -u admin:senha http://192.168.1.100:80/cgi-bin/magicBox.cgi?action=getSystemInfo
```

**Resposta Esperada:**
```
deviceType=DVR
hardwareVersion=1.00
serialNumber=ABC123456789
deviceModel=MHDX-1116
```

Se funcionar, significa que o DVR está acessível!

---

## 🎮 NOVAS FUNCIONALIDADES - CONTROLES AVANÇADOS

### 🎥 Controle PTZ (Pan/Tilt/Zoom)

Controle total de câmeras PTZ através da API REST:

**Endpoint:** `POST /api/dvr/ptz/control`

**Body (JSON):**
```json
{
  "dvrId": 1,
  "channel": 0,
  "direction": "Up",
  "action": "start",
  "speed": 5,
  "password": "senha_do_dvr"
}
```

**Direções disponíveis:**
- `Up`, `Down`, `Left`, `Right`
- `LeftUp`, `RightUp`, `LeftDown`, `RightDown`

**Velocidade:** 1-8 (1 = lento, 8 = rápido)

**Para parar o movimento:**
```json
{
  "action": "stop"
}
```

---

### 📌 Presets PTZ

**Ir para um Preset:**
```http
POST /api/dvr/ptz/preset/goto
{
  "dvrId": 1,
  "channel": 0,
  "presetNumber": 1,
  "password": "senha_do_dvr"
}
```

**Salvar Posição Atual como Preset:**
```http
POST /api/dvr/ptz/preset/set
{
  "dvrId": 1,
  "channel": 0,
  "presetNumber": 1,
  "password": "senha_do_dvr"
}
```

Você pode salvar até **255 presets** por câmera!

---

### 📸 Capturar Snapshots

Capture imagens das câmeras em tempo real:

```http
POST /api/dvr/snapshot
{
  "dvrId": 1,
  "channel": 0,
  "password": "senha_do_dvr"
}
```

Retorna uma imagem JPEG diretamente. Use em tags `<img>` ou salve como arquivo.

**Exemplo JavaScript:**
```javascript
const response = await fetch('/api/dvr/snapshot', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ dvrId, channel, password: senha })
});
const blob = await response.blob();
const url = URL.createObjectURL(blob);
document.getElementById('imagem').src = url;
```

⚠️ **SEGURANÇA:** Senha enviada no corpo da requisição (POST), nunca em query strings!

---

### 🎬 Buscar Gravações

Localize vídeos gravados por período:

```http
POST /api/dvr/recordings/find
{
  "dvrId": 1,
  "channel": 1,
  "startTime": "2025-01-10 08:00:00",
  "endTime": "2025-01-10 20:00:00",
  "password": "senha_do_dvr"
}
```

**Resposta:**
```json
{
  "success": true,
  "recordings": [
    {
      "Channel": 0,
      "StartTime": "2025-01-10 08:00:00",
      "EndTime": "2025-01-10 08:59:59",
      "Type": "dav",
      "FilePath": "/mnt/sd/2025-01-10/001/dav/08/08.00.00-08.59.59[M][0@0][0].dav",
      "Length": 524288000,
      "Events": ["Motion"]
    }
  ]
}
```

---

### 📡 URLs RTSP para Streaming

**Streaming ao Vivo:**
```http
POST /api/dvr/rtsp-url
{
  "dvrId": 1,
  "channel": 1,
  "password": "senha_do_dvr",
  "subtype": 0
}
```

**Resposta:**
```json
{
  "success": true,
  "rtspUrl": "rtsp://admin:senha@192.168.1.100:554/cam/realmonitor?channel=1&subtype=0"
}
```

Use essa URL em players de vídeo compatíveis com RTSP (VLC, ffmpeg, etc).

**Tipos de Stream:**
- `subtype: 0` - Stream principal (alta qualidade)
- `subtype: 1` - Sub-stream (baixa qualidade, economiza banda)

⚠️ **SEGURANÇA:** Senha enviada no corpo POST, protegida por HTTPS!

---

### 📋 Informações dos Canais

Obtenha configurações dos canais do DVR:

```http
POST /api/dvr/channels
{
  "dvrId": 1,
  "password": "senha_do_dvr"
}
```

**Resposta:**
```json
{
  "success": true,
  "channels": {
    "table.ChannelTitle[0].Name": "Entrada Principal",
    "table.ChannelTitle[1].Name": "Estoque",
    "table.ChannelTitle[2].Name": "Caixa 1",
    ...
  }
}
```

⚠️ **SEGURANÇA:** Todas as senhas são enviadas via POST body, nunca em URLs!

---

## 🔄 COLETANDO LOGS AUTOMATICAMENTE

### Opção A: Execução Manual

Execute o script quando quiser coletar logs:

```bash
node scripts/collect-dvr-logs.js
```

**O que ele faz:**
1. Verifica status de todos os DVRs (online/offline)
2. Coleta logs das últimas 24h de cada DVR
3. Salva tudo no banco de dados
4. Você visualiza na aba "Logs de Eventos"

### Opção B: Automação com Cron (Agendamento)

Para coletar logs **automaticamente** a cada hora:

1. Crie um workflow no Replit:
```bash
# .replit ou adicione um novo workflow
[deployment]
run = ["bash", "-c", "while true; do node scripts/collect-dvr-logs.js && sleep 3600; done"]
```

2. Ou use cron no Linux:
```bash
# Editar crontab
crontab -e

# Adicionar linha (executa todo dia às 8h):
0 8 * * * cd /caminho/do/projeto && node scripts/collect-dvr-logs.js
```

---

## 🌐 API HTTP DOS DVRs INTELBRAS

### Endpoints Disponíveis:

| Endpoint | Descrição | Exemplo |
|----------|-----------|---------|
| `/cgi-bin/magicBox.cgi?action=getSystemInfo` | Informações do sistema | Model, Serial, Versão |
| `/cgi-bin/snapshot.cgi?channel=0` | Snapshot de câmera | Imagem JPEG |
| `/cgi-bin/recordFinder.cgi` | Buscar gravações/eventos | Logs, motion, alarmes |
| `/cgi-bin/configManager.cgi?action=getConfig` | Obter configuração | Canais, IPs, etc |
| `/cgi-bin/ptz.cgi?action=start&code=Up` | Controle PTZ | Mover câmera |

### Autenticação:

Todos os endpoints usam **HTTP Basic Auth**:

```javascript
axios.get('http://192.168.1.100/cgi-bin/snapshot.cgi?channel=0', {
    auth: {
        username: 'admin',
        password: 'senha123'
    }
});
```

---

## 📊 FUNCIONALIDADES DO SERVIÇO

### 1. Verificar Conexão

```javascript
const service = new IntelbrasDvrService();

const online = await service.testConnection(
    '192.168.1.100',  // IP
    80,               // Porta
    'admin',          // Usuário
    'senha'           // Senha
);

console.log(`DVR está ${online ? 'online' : 'offline'}`);
```

### 2. Obter Informações do DVR

```javascript
const info = await service.getSystemInfo(
    '192.168.1.100',
    80,
    'admin',
    'senha'
);

console.log(`Modelo: ${info.deviceModel}`);
console.log(`Serial: ${info.serialNumber}`);
console.log(`Versão: ${info.hardwareVersion}`);
```

### 3. Capturar Snapshot

```javascript
const imagemBuffer = await service.getSnapshot(
    '192.168.1.100',
    80,
    'admin',
    'senha',
    0  // Canal (0 = primeira câmera)
);

// Salvar imagem
fs.writeFileSync('snapshot.jpg', imagemBuffer);
```

### 4. Coletar Eventos/Logs

```javascript
const endTime = new Date();
const startTime = new Date(endTime.getTime() - (24 * 60 * 60 * 1000)); // 24h atrás

const eventos = await service.getEvents(
    '192.168.1.100',
    80,
    'admin',
    'senha',
    startTime,
    endTime
);

console.log(`${eventos.length} eventos encontrados`);
```

---

## 🔧 PROBLEMAS COMUNS E SOLUÇÕES

### Problema 1: "Connection timeout" ou "ECONNREFUSED"

**Causa:** DVR não está acessível na rede

**Soluções:**
1. Verifique se o DVR está ligado
2. Teste com `ping 192.168.1.100`
3. Verifique se está na mesma rede
4. Confirme a porta HTTP no menu do DVR
5. Teste com curl primeiro (comando acima)

### Problema 2: "401 Unauthorized"

**Causa:** Usuário ou senha incorretos

**Soluções:**
1. Confirme usuário e senha no DVR
2. Tente resetar senha do DVR (via botão físico)
3. Use o app iSIC para confirmar credenciais

### Problema 3: "Empty response" ou "No data"

**Causa:** DVR não tem eventos no período

**Soluções:**
1. Verifique se há movimento/eventos no DVR
2. Aumente o período de busca (ex: 7 dias)
3. Verifique configurações de detecção de movimento

### Problema 4: Não funciona via Intelbras Cloud

**Causa:** Intelbras não permite acesso via Cloud ID/Serial

**Soluções:**
1. Configure DDNS no DVR (ex: No-IP, DynDNS)
2. Abra porta no roteador (port forward)
3. Use VPN para acessar rede local remotamente
4. Ou use apenas via IP local mesmo

---

## 🌍 ACESSO REMOTO (Internet)

Para coletar logs de DVRs em outras lojas via internet:

### Opção A: DDNS (Recomendado)

1. Configure DDNS no DVR:
   - Menu > Rede > DDNS
   - Escolha No-IP ou DynDNS
   - Cadastre um domínio (ex: loja-centro.ddns.net)

2. Abra porta no roteador:
   - Acesse roteador (192.168.0.1 ou 192.168.1.1)
   - Port Forwarding
   - Porta Externa: 8080 → IP DVR:80

3. Use no código:
   ```javascript
   const eventos = await service.getEvents(
       'loja-centro.ddns.net',
       8080,  // Porta externa
       'admin',
       'senha',
       startTime,
       endTime
   );
   ```

### Opção B: VPN

1. Configure VPN no servidor onde o sistema roda
2. Conecte à VPN da loja
3. Use IPs locais normalmente (192.168.x.x)

---

## 📅 FLUXO DE TRABALHO RECOMENDADO

### Para Lojas Locais (Mesma Rede):

```
1. DVRs conectados na rede local (192.168.1.x)
2. Script roda no servidor a cada hora
3. Logs aparecem automaticamente no sistema
4. Você só visualiza na interface web
```

### Para Lojas Remotas (Internet):

```
1. Configure DDNS em cada DVR
2. Cadastre DVRs com domínio DDNS
3. Script roda no servidor a cada hora
4. Logs aparecem automaticamente
```

### Híbrido (Local + Remoto):

```
1. Use VPN para acessar redes remotas
2. Todos os DVRs acessíveis por IP local
3. Mais seguro que expor portas na internet
```

---

## 🎯 EXEMPLO PRÁTICO COMPLETO

### 1. Cadastre o DVR no Sistema

Acesse: https://seu-sistema.replit.dev → DVR/NVR → Adicionar Dispositivo

```
Nome: DVR Loja Centro
Loja: Loja Centro
IP: 192.168.1.100  (ou loja-centro.ddns.net)
Porta: 80
Usuário: admin
Modelo: Intelbras MHDX 1116
Canais: 16
Status: online
```

### 2. Configure Senha no Script

Edite `scripts/collect-dvr-logs.js`:

```javascript
const DVR_PASSWORDS = {
    1: 'senha_dvr_centro',  // ID 1 = DVR Loja Centro
};
```

### 3. Execute Coleta Manual

```bash
node scripts/collect-dvr-logs.js
```

**Saída esperada:**
```
🔄 Iniciando coleta de logs dos DVRs Intelbras...

📡 Verificando status de todos os DVRs...
✅ Status atualizado

📥 Coletando logs do DVR 1...
   ✅ 15 logs coletados

🎉 Coleta concluída! Total de 15 logs coletados
```

### 4. Visualize os Logs

Acesse: DVR/NVR → Aba "Logs de Eventos"

Você verá:
- Detecções de movimento
- Perdas de vídeo
- Alarmes
- Conexões/desconexões

---

## 🔐 SEGURANÇA

### Boas Práticas:

✅ **NÃO** commite senhas no Git
✅ Use arquivo `.env` para senhas (se necessário)
✅ Use VPN ao invés de expor portas
✅ Troque senhas padrão dos DVRs
✅ Use HTTPS se possível (porta 443)
✅ Limite IPs que podem acessar DVRs

### Exemplo com .env:

```bash
# .env
DVR_1_PASSWORD=senha_secreta
DVR_2_PASSWORD=outra_senha
```

```javascript
// No script
require('dotenv').config();

const DVR_PASSWORDS = {
    1: process.env.DVR_1_PASSWORD,
    2: process.env.DVR_2_PASSWORD,
};
```

---

## 📞 SUPORTE

**Problemas com o sistema:**
- Verifique logs: console do servidor
- Teste conexão manual com curl
- Verifique senhas e IPs

**Problemas com DVRs:**
- Suporte Intelbras: (48) 2106-0006
- Fórum: forum.intelbras.com.br

---

## ✅ CHECKLIST DE IMPLEMENTAÇÃO

- [ ] DVR acessível via IP/DDNS
- [ ] Porta HTTP aberta (80 ou custom)
- [ ] Usuário e senha conhecidos
- [ ] DVR cadastrado no sistema
- [ ] Senha configurada no script
- [ ] Teste manual executado com sucesso
- [ ] Logs aparecendo na interface
- [ ] (Opcional) Automação configurada

---

## 🎉 PRONTO!

Agora você tem **integração real** com os DVRs Intelbras usando a API HTTP nativa, sem precisar das DLLs do Windows!

**Vantagens:**
✅ Funciona no Linux/Replit
✅ Não precisa de serviços intermediários
✅ Coleta logs automaticamente
✅ Centraliza tudo em um sistema web
✅ Pode escalar para dezenas de DVRs

**Versão:** 1.0
**Data:** 10/11/2025
