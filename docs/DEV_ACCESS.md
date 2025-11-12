# Sistema de Acesso Temporário para Desenvolvimento

## ⚠️ ATENÇÃO - APENAS PARA DESENVOLVIMENTO

Este sistema fornece acesso temporário com permissões completas de desenvolvedor (role: `dev`) através de tokens JWT, **sem necessidade de login tradicional**. 

**NUNCA HABILITE ESTE RECURSO EM PRODUÇÃO!**

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Como Funciona](#como-funciona)
3. [Configuração](#configuração)
4. [Como Usar](#como-usar)
5. [Endpoints Disponíveis](#endpoints-disponíveis)
6. [Segurança](#segurança)
7. [Auditoria e Logs](#auditoria-e-logs)
8. [Testes](#testes)
9. [Desabilitar Antes de Produção](#desabilitar-antes-de-produção)

---

## 🎯 Visão Geral

O sistema de acesso temporário permite que desenvolvedores e administradores gerem tokens JWT com:

- **Validade configurável**: 0.1 a 24 horas
- **Permissões completas**: Role `dev` com acesso total ao sistema
- **Restrição opcional por IP**: Tokens podem ser limitados a um IP específico
- **Revogação manual**: Tokens podem ser revogados a qualquer momento
- **Auditoria completa**: Todas as ações são registradas em logs

---

## 🔧 Como Funciona

### Fluxo de Autenticação

1. **Geração do Token**
   - Admin/Dev faz login normalmente no sistema
   - Chama endpoint `/api/dev/generate-temp-token`
   - Sistema gera token JWT assinado e armazena hash no banco
   - Token é retornado para uso

2. **Uso do Token**
   - Cliente envia requisições com header `Authorization: Bearer <token>`
   - Middleware `tempTokenAuthMiddleware` intercepta a requisição
   - Verifica assinatura JWT e validade
   - Consulta banco de dados para validar token (não revogado, não expirado)
   - Verifica restrição de IP se configurada
   - Cria sessão temporária com role `dev`

3. **Revogação**
   - Admin/Dev chama endpoint `/api/dev/revoke-temp-token`
   - Token é marcado como revogado no banco
   - Próximas tentativas de uso são bloqueadas

---

## ⚙️ Configuração

### 1. Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto (use `.env.example` como referência):

```bash
# Ambiente de desenvolvimento
NODE_ENV=development

# Secret para tokens JWT (gere um secret forte)
JWT_SECRET=sua_chave_jwt_secreta_aqui_minimo_64_caracteres

# Habilitar acesso temporário (apenas em desenvolvimento)
DEV_TEMP_ACCESS=true
```

### 2. Gerar Secrets Seguros

Execute o comando abaixo para gerar um secret forte:

```bash
node -e "console.log(require('crypto').randomBytes(64).toString('hex'))"
```

### 3. Verificar Configuração

Ao iniciar o servidor, você verá uma das mensagens:

```
🔓 Acesso temporário de desenvolvimento HABILITADO
⚠️  ATENÇÃO: Desabilite DEV_TEMP_ACCESS antes de fazer deploy em produção!
```

ou

```
🔒 Acesso temporário de desenvolvimento DESABILITADO
```

---

## 🚀 Como Usar

### Passo 1: Login Normal

Primeiro, faça login no sistema normalmente:

```bash
POST /api/login
Content-Type: application/json

{
  "username": "admin",
  "password": "sua_senha"
}
```

### Passo 2: Gerar Token Temporário

```bash
POST /api/dev/generate-temp-token
Content-Type: application/json

{
  "expiresInHours": 1,        # Opcional, padrão: 1 hora
  "ipRestricted": "192.168.1.100"  # Opcional, restringe a um IP
}
```

**Resposta:**

```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresInHours": 1,
  "ipRestricted": null,
  "usage": "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "warning": "Este token tem acesso completo de desenvolvedor. Mantenha-o seguro!"
}
```

### Passo 3: Usar o Token

Use o token em todas as requisições:

```bash
GET /api/usuarios
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

O token substitui completamente o login tradicional e fornece acesso como desenvolvedor.

---

## 📡 Endpoints Disponíveis

### 1. Gerar Token Temporário

**POST** `/api/dev/generate-temp-token`

**Autenticação:** Requer login com role `admin` ou `dev`

**Body:**
```json
{
  "expiresInHours": 1,        // Opcional, padrão: 1, mín: 0.1, máx: 24
  "ipRestricted": "IP"        // Opcional, restringe token a um IP
}
```

**Resposta de Sucesso:**
```json
{
  "success": true,
  "token": "eyJ...",
  "expiresInHours": 1,
  "ipRestricted": null,
  "usage": "Authorization: Bearer eyJ...",
  "warning": "..."
}
```

---

### 2. Listar Tokens Ativos

**GET** `/api/dev/temp-tokens`

**Autenticação:** Requer login com role `admin` ou `dev`

**Resposta:**
```json
[
  {
    "id": 1,
    "role": "dev",
    "expira_em": "2025-11-04T17:00:00.000Z",
    "ip_origem": "192.168.1.1",
    "ip_restrito": null,
    "revogado": 0,
    "criado_por": "admin",
    "criado_em": "2025-11-04T16:00:00.000Z",
    "usado_em": "2025-11-04T16:05:00.000Z",
    "revogado_em": null,
    "revogado_por": null,
    "status": "ativo"
  }
]
```

**Status possíveis:**
- `ativo`: Token válido e não revogado
- `expirado`: Token passou da validade
- `revogado`: Token foi revogado manualmente

---

### 3. Revogar Token

**DELETE** `/api/dev/revoke-temp-token`

**Autenticação:** Requer login com role `admin` ou `dev`

**Body:**
```json
{
  "tokenId": 1
}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Token revogado com sucesso"
}
```

---

## 🔒 Segurança

### Proteções Implementadas

1. **Assinatura JWT**: Tokens são assinados com `JWT_SECRET` forte
2. **Hash no Banco**: Apenas hash SHA-256 do token é armazenado
3. **Validade Limitada**: Máximo de 24 horas
4. **Revogação**: Tokens podem ser revogados imediatamente
5. **Restrição de IP**: Opcional, limita uso a um IP específico
6. **Audit Log**: Todas as ações são registradas
7. **Feature Flag**: Só funciona com `DEV_TEMP_ACCESS=true`
8. **Ambiente**: Só funciona em `NODE_ENV=development` (ou não definido)

### Boas Práticas

✅ **FAÇA:**
- Use tokens apenas em desenvolvimento/teste
- Configure restrição de IP quando possível
- Revogue tokens quando não forem mais necessários
- Use validade curta (1-2 horas)
- Monitore logs de uso de tokens
- Mantenha `JWT_SECRET` seguro e forte

❌ **NÃO FAÇA:**
- Commitar tokens no git
- Compartilhar tokens publicamente
- Usar tokens em produção
- Deixar `DEV_TEMP_ACCESS=true` em produção
- Gerar tokens com validade muito longa
- Ignorar alertas de segurança

---

## 📊 Auditoria e Logs

Todos os eventos relacionados a tokens temporários são registrados na tabela `logs`:

### Eventos Registrados

1. **Geração de Token** (`temp_token_generated`)
   - Quem gerou
   - IP de origem
   - Validade configurada
   - Restrição de IP

2. **Uso de Token** (`temp_token_used`)
   - Timestamp de uso
   - IP do usuário
   - Endpoint acessado

3. **Revogação de Token** (`temp_token_revoked`)
   - Quem revogou
   - ID do token
   - Motivo

4. **Rejeição de Token** (`token_rejected`)
   - Motivo da rejeição
   - IP tentado
   - Detalhes do erro

### Consultar Logs

```sql
SELECT * FROM logs 
WHERE action LIKE '%token%' 
ORDER BY timestamp DESC 
LIMIT 50;
```

---

## 🧪 Testes

### Teste Manual

1. **Gerar Token:**
```bash
curl -X POST http://localhost:5000/api/dev/generate-temp-token \
  -H "Content-Type: application/json" \
  -H "Cookie: sessionId=..." \
  -d '{"expiresInHours": 0.1}'
```

2. **Usar Token:**
```bash
curl http://localhost:5000/api/usuarios \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

3. **Listar Tokens:**
```bash
curl http://localhost:5000/api/dev/temp-tokens \
  -H "Cookie: sessionId=..."
```

4. **Revogar Token:**
```bash
curl -X DELETE http://localhost:5000/api/dev/revoke-temp-token \
  -H "Content-Type: application/json" \
  -H "Cookie: sessionId=..." \
  -d '{"tokenId": 1}'
```

### Verificar Expiração

Gere um token com validade de 0.1 hora (6 minutos) e aguarde. Após expirar, tente usar:

```bash
# Deve retornar erro 401: Token expirado
curl http://localhost:5000/api/usuarios \
  -H "Authorization: Bearer TOKEN_EXPIRADO"
```

---

## 🚫 Desabilitar Antes de Produção

### Checklist Pré-Deploy

- [ ] Definir `NODE_ENV=production` no servidor
- [ ] Remover ou definir `DEV_TEMP_ACCESS=false` (ou não definir)
- [ ] Verificar que nenhum token está ativo
- [ ] Confirmar que secrets de produção são diferentes de desenvolvimento
- [ ] Testar que endpoint `/api/dev/generate-temp-token` retorna 403
- [ ] Revisar logs para garantir que não há uso suspeito de tokens

### Como Desabilitar

**Opção 1: Remover variável**
```bash
# No arquivo .env de produção, simplesmente não defina DEV_TEMP_ACCESS
# ou remova a linha completamente
```

**Opção 2: Definir como false**
```bash
DEV_TEMP_ACCESS=false
```

**Opção 3: NODE_ENV production**
```bash
NODE_ENV=production
```

Qualquer uma das opções acima desabilita o sistema de tokens temporários.

### Verificação

Ao iniciar em produção, você deve ver:

```
🔒 Acesso temporário de desenvolvimento DESABILITADO
```

Tentativas de gerar tokens retornarão:

```json
{
  "error": "Acesso temporário desabilitado",
  "message": "Configure DEV_TEMP_ACCESS=true em ambiente de desenvolvimento"
}
```

---

## 📞 Suporte

Para dúvidas ou problemas:

1. Verifique os logs: `SELECT * FROM logs WHERE action LIKE '%token%'`
2. Confirme configuração: variáveis de ambiente corretas
3. Revogue todos os tokens antes de deploy
4. Consulte a documentação do sistema

---

## 📝 Notas Técnicas

### Estrutura da Tabela `temp_tokens`

```sql
CREATE TABLE temp_tokens (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    token_hash TEXT UNIQUE NOT NULL,      -- SHA-256 hash do token
    role TEXT DEFAULT 'dev',              -- Sempre 'dev'
    expira_em DATETIME NOT NULL,          -- Data/hora de expiração
    ip_origem TEXT,                       -- IP de quem gerou
    ip_restrito TEXT,                     -- IP autorizado (opcional)
    revogado INTEGER DEFAULT 0,           -- 0=ativo, 1=revogado
    criado_por TEXT,                      -- Username de quem criou
    criado_em DATETIME DEFAULT CURRENT_TIMESTAMP,
    usado_em DATETIME,                    -- Última vez usado
    revogado_em DATETIME,                 -- Quando foi revogado
    revogado_por TEXT                     -- Quem revogou
);
```

### Payload do Token JWT

```json
{
  "tokenId": "hex_string_16_bytes",
  "role": "dev",
  "type": "temp_access",
  "iat": 1234567890,
  "exp": 1234571490,
  "iss": "dev-temp-access"
}
```

---

**Versão:** 1.0  
**Última Atualização:** 04/11/2025  
**Ambiente:** Desenvolvimento apenas
