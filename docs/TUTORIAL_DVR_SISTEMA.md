# 📘 Tutorial: Como Usar o Sistema DVR/NVR

## 🎯 O Que Este Sistema Faz

Este sistema permite que você **gerencie dispositivos DVR/NVR** e **visualize logs de eventos** de forma centralizada. Ele funciona como um painel de controle para acompanhar seus dispositivos de segurança.

---

## 🌐 1. ACESSAR O SISTEMA

**URL:** https://03d5953b-20f2-488a-a843-31bdfd97c251-00-h2qsut0itjvv.riker.replit.dev

**Credenciais:**
- **Usuário:** admin
- **Senha:** admin

⚠️ **Importante:** Troque a senha após o primeiro acesso (menu Configurações > Gerenciar Usuários)

---

## 🏪 2. CADASTRAR SUAS LOJAS

### Passo a Passo:

1. Faça login no sistema
2. Clique em **"Lojas"** no menu lateral
3. Clique no botão **"Adicionar Loja"**
4. Preencha os campos:
   - **Nome:** Nome da loja (ex: "Loja Centro")
   - **Status:** ativa / inativa
   - **CEP:** CEP da loja
   - **Telefone:** Número de contato
   - **Gerente:** Nome do gerente
   - **Observações:** Informações adicionais
5. Clique em **"Salvar"**

### Exemplo Prático:
```
Nome: Loja Centro
Status: ativa
CEP: 88015-100
Telefone: (48) 3333-4444
Gerente: João Silva
Observações: Loja principal - 2 DVRs instalados
```

---

## 📹 3. CADASTRAR DISPOSITIVOS DVR/NVR

### Passo a Passo:

1. Clique em **"DVR/NVR"** no menu lateral
2. Certifique-se que está na aba **"Dispositivos"**
3. Clique em **"Adicionar Dispositivo"**
4. Preencha o formulário:

### Campos do Formulário:

| Campo | Descrição | Exemplo |
|-------|-----------|---------|
| **Nome** | Identificação do DVR | "DVR Entrada Principal" |
| **Loja** | Selecione a loja | Loja Centro |
| **IP** | Endereço IP do DVR | 192.168.1.100 |
| **Porta** | Porta de conexão | 37777 (padrão) |
| **Usuário** | Login do DVR | admin |
| **Modelo** | Modelo Intelbras | MHDX 1116 / NVR 1108 HS |
| **Canais** | Quantidade de câmeras | 8 / 16 |
| **Status** | online / offline | online |
| **Observações** | Detalhes extras | "Serial Cloud: ABC123456" |

### Exemplo Completo:
```
Nome: DVR Entrada Principal
Loja: Loja Centro
IP: 192.168.1.100
Porta: 37777
Usuário: admin
Modelo: Intelbras MHDX 1116
Canais: 16
Status: online
Observações: Serial Cloud: ABC123456789
             Instalado em: 10/11/2025
             Câmeras: entrada, caixa, estoque, corredor
```

---

## 📊 4. COMO FUNCIONAM OS LOGS

### ⚠️ IMPORTANTE - Limitações da Intelbras

**Não é possível receber logs automaticamente do Intelbras Cloud** porque:
- A Intelbras não disponibiliza API pública do Cloud
- O acesso via Serial/Cloud ID não é permitido para desenvolvedores
- Apenas o app iSIC (mobile) tem acesso ao Cloud

### ✅ O Que VOCÊ PODE FAZER:

#### **Opção A: Registrar Logs Manualmente**

1. Acesse a aba **"Logs de Eventos"**
2. Clique em **"Adicionar Log"** (se houver botão)
3. Ou use o menu do dispositivo > "Registrar Evento"

#### **Opção B: Enviar Logs via API (Para Desenvolvedores)**

Se você tiver acesso técnico, pode enviar logs usando curl/Postman:

```bash
curl -X POST https://seu-sistema.replit.dev/api/dvr/logs \
  -H "Content-Type: application/json" \
  -d '{
    "dvr_id": 1,
    "dvr_nome": "DVR Entrada Principal",
    "loja_nome": "Loja Centro",
    "tipo_evento": "Detecção de Movimento",
    "descricao": "Movimento detectado no canal 3",
    "canal": 3,
    "severidade": "info"
  }'
```

**Tipos de Evento:**
- Conexão
- Desconexão
- Detecção de Movimento
- Alarme
- Perda de Vídeo
- Gravação Iniciada
- Erro de Disco

**Severidade:**
- `info` - Informação
- `warning` - Aviso
- `error` - Erro crítico

#### **Opção C: Integração Futura (Requer SDK Intelbras)**

Para receber logs automaticamente, você precisará:

1. Solicitar o SDK da Intelbras:
   - WhatsApp: (48) 2106-0006
   - Email: suporte@intelbras.com.br
   - Assinar termo de confidencialidade

2. Desenvolver um serviço intermediário que:
   - Conecta aos DVRs via IP local (não Cloud)
   - Consulta logs periodicamente
   - Envia para este sistema via API

---

## 📁 5. GERENCIAR ARQUIVOS (Vídeos, Imagens)

### Upload de Arquivos:

1. Vá para a aba **"Arquivos"**
2. Clique em **"Upload de Arquivo"**
3. Selecione:
   - **Dispositivo DVR**
   - **Tipo:** vídeo / imagem / relatório
   - **Canal:** número da câmera
   - **Data/Hora** do arquivo
   - **Descrição**
4. Anexe o arquivo
5. Clique em **"Enviar"**

### Download de Arquivos:

- Na lista de arquivos, clique no ícone de **download** 📥
- O arquivo será baixado para seu computador

---

## 🔍 6. FILTRAR E PESQUISAR

### Filtros Disponíveis:

**Na aba Dispositivos:**
- Filtrar por loja
- Filtrar por status (online/offline)

**Na aba Logs:**
- Filtrar por dispositivo
- Filtrar por tipo de evento
- Filtrar por data
- Filtrar por severidade

**Na aba Arquivos:**
- Filtrar por dispositivo
- Filtrar por tipo de arquivo
- Filtrar por data

---

## 📋 7. USO DO DIA A DIA

### Rotina Diária:

1. **Manhã:**
   - Acesse o sistema
   - Vá em "DVR/NVR" > "Dispositivos"
   - Verifique se todos estão **online**
   - Se algum estiver offline, investigue

2. **Durante o Dia:**
   - Registre eventos importantes manualmente
   - Faça upload de vídeos de incidentes
   - Documente problemas técnicos

3. **Final do Dia:**
   - Revise os logs na aba "Logs de Eventos"
   - Verifique se houve alertas importantes
   - Archive vídeos relevantes

### Cenários Práticos:

**Cenário 1: DVR Ficou Offline**
```
1. Vá em Dispositivos
2. Localize o DVR offline
3. Clique em "Editar"
4. Mude Status para "offline"
5. Adicione nas Observações: "Offline desde [data/hora]"
6. Registre um log: Tipo "Desconexão", Severidade "error"
```

**Cenário 2: Gravação de Incidente**
```
1. Baixe o vídeo do DVR (via app iSIC ou direto do aparelho)
2. Vá em "Arquivos" > "Upload"
3. Selecione o DVR correspondente
4. Tipo: "vídeo"
5. Descrição: "Furto na loja - 10/11/2025 14:30"
6. Faça upload
7. Agora o vídeo está centralizado no sistema
```

**Cenário 3: Manutenção Preventiva**
```
1. Antes da manutenção: Registre log "Manutenção Programada"
2. Durante: Mude status do DVR para "offline"
3. Depois: Registre log "Manutenção Concluída"
4. Mude status de volta para "online"
5. Adicione observações sobre o que foi feito
```

---

## ⚙️ 8. CONFIGURAÇÕES IMPORTANTES

### Trocar Senha do Admin:

1. Menu lateral > **"Configurações"**
2. Clique em **"Gerenciar Usuários"**
3. Edite o usuário "admin"
4. Digite nova senha
5. Salve

### Adicionar Novos Usuários:

1. Menu lateral > **"Configurações"**
2. **"Gerenciar Usuários"** > **"Adicionar Usuário"**
3. Preencha:
   - Username (único)
   - Senha
4. Salve

---

## 🚨 9. SOLUÇÃO DE PROBLEMAS

### Problema: Não consigo fazer login
**Solução:** Use `admin` / `admin` (tudo minúsculo)

### Problema: Não vejo a aba DVR/NVR
**Solução:** Faça logout e login novamente

### Problema: Erro ao adicionar dispositivo
**Solução:** Verifique se preencheu todos os campos obrigatórios (Nome, IP, Loja)

### Problema: Upload de arquivo falha
**Solução:** 
- Verifique o tamanho (máximo 500MB)
- Verifique o formato do arquivo
- Tente um arquivo menor primeiro

---

## 📱 10. INTELBRAS CLOUD - O QUE VOCÊ PODE FAZER

### O Que NÃO Funciona (Limitação da Intelbras):
❌ Receber logs automaticamente do Cloud
❌ Conectar via Serial/Cloud ID
❌ Integração direta com API Cloud

### O Que FUNCIONA:
✅ Usar o app **iSIC** no celular para monitoramento ao vivo
✅ Registrar manualmente no sistema os eventos importantes que você vê no iSIC
✅ Fazer download de vídeos pelo iSIC e depois fazer upload aqui
✅ Manter registro centralizado de todos os seus DVRs
✅ Documentar incidentes com vídeos e logs
✅ Gerar relatórios internos

### Fluxo de Trabalho Recomendado:
```
1. Use o iSIC (mobile) para monitorar ao vivo
2. Quando algo importante acontecer:
   a. Anote data/hora/câmera
   b. Faça download do vídeo pelo iSIC (se necessário)
3. Acesse este sistema web
4. Registre o log do evento
5. Faça upload do vídeo (se baixou)
6. Agora você tem tudo documentado em um só lugar
```

---

## 🎯 11. RESUMO RÁPIDO

**O sistema serve para:**
✅ Centralizar informação de todos os seus DVRs
✅ Manter histórico de eventos e incidentes
✅ Armazenar vídeos importantes
✅ Facilitar gestão de múltiplas lojas

**O sistema NÃO faz:**
❌ Receber logs automaticamente do Intelbras Cloud (limitação da Intelbras)
❌ Substituir o app iSIC para monitoramento ao vivo
❌ Conectar diretamente aos DVRs via Cloud ID

**É útil porque:**
- Você tem um painel único para ver todos os DVRs
- Pode documentar eventos importantes
- Pode armazenar vídeos de incidentes
- Pode acompanhar status de todos os dispositivos
- Pode gerar relatórios internos

---

## 📞 PRECISA DE AJUDA?

**Para problemas técnicos do sistema:**
- Verifique se o servidor está rodando
- Tente fazer logout e login novamente
- Limpe cache do navegador (Ctrl+Shift+Del)

**Para dúvidas sobre DVRs Intelbras:**
- Suporte Intelbras: (48) 2106-0006
- Email: suporte@intelbras.com.br
- Fórum: forum.intelbras.com.br

---

## 🚀 PRÓXIMOS PASSOS

1. ✅ Acesse o sistema e faça login
2. ✅ Cadastre suas lojas
3. ✅ Cadastre seus DVRs
4. ✅ Registre alguns logs de teste
5. ✅ Faça upload de um arquivo de teste
6. ✅ Explore os filtros e busca
7. ✅ Comece a usar no dia a dia!

---

**Versão do Tutorial:** 1.0
**Última Atualização:** 10/11/2025
