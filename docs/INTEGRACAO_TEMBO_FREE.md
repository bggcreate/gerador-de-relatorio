# 🎯 Guia Completo: Integração com Banco de Dados Gratuito Tembo.io

## 📖 Índice
1. [Visão Geral](#visão-geral)
2. [Por que usar o Tembo.io?](#por-que-usar-o-temboio)
3. [Passo a Passo Completo](#passo-a-passo-completo)
4. [Configuração Detalhada](#configuração-detalhada)
5. [Migração de Dados](#migração-de-dados)
6. [Verificação e Testes](#verificação-e-testes)
7. [Múltiplos Computadores](#múltiplos-computadores)
8. [Solução de Problemas](#solução-de-problemas)
9. [Perguntas Frequentes](#perguntas-frequentes)

---

## 🌟 Visão Geral

Este sistema já possui **suporte completo e integrado** para PostgreSQL via Tembo.io. Toda a infraestrutura necessária já está implementada, incluindo:

- ✅ Configuração de conexão PostgreSQL com pool de conexões
- ✅ Scripts de migração automática do SQLite para PostgreSQL
- ✅ Suporte SSL/TLS para conexão segura
- ✅ Monitoramento automático do tamanho do banco
- ✅ Sistema de backup automático ao atingir 4GB
- ✅ Suporte multi-instância (vários computadores)
- ✅ Registro de auditoria completo

**Tempo estimado de integração:** 10-15 minutos

---

## 🚀 Por que usar o Tembo.io?

### Vantagens do PostgreSQL na Nuvem

| Recurso | SQLite (Atual) | Tembo PostgreSQL |
|---------|----------------|------------------|
| **Armazenamento** | Limitado ao disco local | 10GB gratuito na nuvem |
| **Acesso Multi-dispositivo** | ❌ Não | ✅ Sim |
| **Backup Automático** | Manual | Automático + Email |
| **Sincronização** | ❌ Não | ✅ Tempo real |
| **Escalabilidade** | Limitada | Alta |
| **Performance** | Boa para 1 usuário | Excelente para múltiplos |
| **Custos** | Grátis (local) | Grátis até 10GB |

### Plano Gratuito do Tembo.io

- **Armazenamento:** 10GB (limite gratuito)
- **Conexões:** Até 20 simultâneas
- **Backups:** Automáticos diários
- **SSL/TLS:** Incluído
- **Disponibilidade:** 99.9%
- **Suporte:** Comunidade + Documentação

---

## 📋 Passo a Passo Completo

### **Passo 1: Criar Conta no Tembo.io** (3-5 minutos)

#### 1.1. Acesse o Site
```
https://cloud.tembo.io
```

#### 1.2. Crie uma Conta
- Clique em **"Sign Up"** ou **"Get Started"**
- Opções disponíveis:
  - Email + Senha
  - Login com GitHub
  - Login com Google

#### 1.3. Verifique seu Email
- Você receberá um email de confirmação
- Clique no link de verificação

#### 1.4. Faça Login
- Acesse o dashboard: https://cloud.tembo.io/dashboard

---

### **Passo 2: Criar Instância PostgreSQL** (2-3 minutos)

#### 2.1. No Dashboard do Tembo
1. Clique em **"Create New Instance"** ou **"+ New Stack"**
2. Selecione o tipo: **"PostgreSQL Standard"**
3. Escolha o plano: **"Hobby Tier"** (Gratuito - 10GB)

#### 2.2. Configure a Instância
```
Nome: meu-sistema-monitoramento
Região: US East (use1) - Recomendado
Versão: PostgreSQL 16.x (mais recente)
```

#### 2.3. Clique em "Create"
- Aguarde 30-60 segundos enquanto a instância é provisionada
- Você verá uma tela com status "Provisioning..." → "Running"

---

### **Passo 3: Obter Credenciais** (1 minuto)

#### 3.1. Na Página da Instância
1. Clique na instância recém-criada
2. Procure por **"Connection Info"** ou **"Connection String"**
3. Você verá algo assim:

```
Host: seu-projeto-abc123.data-1.use1.tembo.io
Port: 5432
Database: postgres
User: postgres
Password: ********* (clique no ícone do olho para revelar)
```

#### 3.2. Copie as Credenciais
- **IMPORTANTE:** Salve essas informações em local seguro
- Você precisará delas para o próximo passo

**Exemplo de credenciais:**
```
Host: myapp-prod-x7k2m.data-1.use1.tembo.io
Port: 5432
Database: postgres
User: postgres
Password: tEmB0_s3cUR3_pA55w0rD_2024
```

---

### **Passo 4: Configurar o Sistema** (2 minutos)

#### 4.1. Criar Arquivo .env
Na raiz do projeto, crie (ou edite) o arquivo `.env`:

```bash
# Copie do .env.example se ainda não existir
cp .env.example .env
```

#### 4.2. Adicionar Credenciais do Tembo
Abra o arquivo `.env` e adicione/edite as seguintes linhas:

```bash
# =================================================================
# POSTGRESQL - TEMBO.IO (BANCO DE DADOS GRATUITO)
# =================================================================

# Host da sua instância (copie do dashboard Tembo)
PGHOST=seu-projeto-abc123.data-1.use1.tembo.io

# Porta (sempre 5432 para PostgreSQL)
PGPORT=5432

# Nome do banco de dados (padrão: postgres)
PGDATABASE=postgres

# Usuário (padrão: postgres)
PGUSER=postgres

# Senha (copie do dashboard Tembo - clique no ícone do olho)
PGPASSWORD=sua_senha_real_aqui

# SSL obrigatório para Tembo
PGSSLMODE=require

# Nome deste computador (para identificação)
INSTANCE_NAME=Computador Principal
```

#### 4.3. Exemplo Completo
```bash
# Exemplo real (substitua pelos seus valores)
PGHOST=myapp-prod-x7k2m.data-1.use1.tembo.io
PGPORT=5432
PGDATABASE=postgres
PGUSER=postgres
PGPASSWORD=tEmB0_s3cUR3_pA55w0rD_2024
PGSSLMODE=require
INSTANCE_NAME=Notebook Dell - João
```

#### 4.4. Mantenha Outras Variáveis
Certifique-se de manter as variáveis existentes:
```bash
NODE_ENV=development
PORT=5000
SESSION_SECRET=sua_chave_secreta_aqui
JWT_SECRET=sua_chave_jwt_aqui
```

---

### **Passo 5: Testar Conexão** (1 minuto)

#### 5.1. Instale Dependências (se necessário)
```bash
npm install
```

#### 5.2. Teste a Conexão
```bash
node -e "
const pg = require('./src/config/postgresql');
pg.testConnection().then(ok => {
  if (ok) {
    console.log('✅ SUCESSO! Conectado ao Tembo.io');
    process.exit(0);
  } else {
    console.log('❌ ERRO: Não foi possível conectar');
    process.exit(1);
  }
});
"
```

**Resultado esperado:**
```
✅ Pool de conexões PostgreSQL criado
✅ Conexão PostgreSQL bem-sucedida: 2025-11-12T20:15:30.123Z
✅ SUCESSO! Conectado ao Tembo.io
```

**Se houver erro:**
- Verifique se as credenciais estão corretas
- Confirme que `PGSSLMODE=require` está configurado
- Teste a conexão no dashboard do Tembo.io

---

### **Passo 6: Migrar Dados Existentes** (2-5 minutos)

#### 6.1. Verificar Banco SQLite Atual
```bash
# Ver tamanho do banco atual
ls -lh data/database.db
```

#### 6.2. Executar Script de Migração
```bash
node scripts/migrate-to-postgres.js
```

#### 6.3. Acompanhe o Progresso
Você verá mensagens como:
```
🔍 Verificando configuração...
✅ Configuração PostgreSQL encontrada
✅ Conexão PostgreSQL bem-sucedida

📊 Migrando dados...
✅ Tabela 'usuarios' criada (15 registros migrados)
✅ Tabela 'lojas' criada (8 registros migrados)
✅ Tabela 'relatorios' criada (347 registros migrados)
✅ Tabela 'demandas' criada (123 registros migrados)
✅ Tabela 'vendedores' criada (42 registros migrados)
✅ Tabela 'estoque_tecnico' criada (89 registros migrados)
✅ Tabela 'assistencias' criada (201 registros migrados)
...

🎉 Migração concluída com sucesso!
📊 Total: 1.245 registros migrados em 13 tabelas
🔧 Instância registrada: Computador Principal (uuid-xyz-123)
```

#### 6.4. Backup de Segurança
O script **automaticamente cria um backup** do SQLite antes de migrar:
```
data/backups/database-backup-YYYY-MM-DD-HHMMSS.db
```

---

### **Passo 7: Iniciar Sistema com PostgreSQL** (1 minuto)

#### 7.1. Parar o Servidor (se estiver rodando)
```bash
# Ctrl+C no terminal onde está rodando
```

#### 7.2. Iniciar com PostgreSQL
```bash
npm start
```

#### 7.3. Verifique os Logs
Procure por estas mensagens:
```
✅ Pool de conexões PostgreSQL criado
✅ Conexão PostgreSQL bem-sucedida
🚀 Servidor rodando em modo POSTGRESQL
📊 Tamanho do banco: 15.47 MB (0.38% do limite de 4GB)
🌐 Servidor rodando na porta 5000
```

#### 7.4. Teste o Sistema
1. Abra o navegador: `http://localhost:5000`
2. Faça login com suas credenciais
3. Verifique se os dados foram migrados corretamente
4. Teste criar um novo registro

---

## 🔧 Configuração Detalhada

### Variáveis de Ambiente

| Variável | Obrigatória | Padrão | Descrição |
|----------|-------------|--------|-----------|
| `PGHOST` | ✅ Sim | - | Host da instância Tembo (ex: `abc.data-1.use1.tembo.io`) |
| `PGPORT` | ⚠️ Recomendado | 5432 | Porta PostgreSQL |
| `PGDATABASE` | ✅ Sim | - | Nome do banco (geralmente `postgres`) |
| `PGUSER` | ✅ Sim | - | Usuário PostgreSQL (geralmente `postgres`) |
| `PGPASSWORD` | ✅ Sim | - | Senha fornecida pelo Tembo |
| `PGSSLMODE` | ✅ Sim | - | Modo SSL (sempre `require` para Tembo) |
| `INSTANCE_NAME` | ⚠️ Recomendado | - | Nome do computador (para multi-instância) |

### Configuração do Pool de Conexões

O sistema usa um pool de conexões otimizado:

```javascript
// Configuração padrão (src/config/postgresql.js:14)
max: 20,                      // Máximo de conexões simultâneas
idleTimeoutMillis: 30000,     // 30s - Tempo antes de fechar conexão ociosa
connectionTimeoutMillis: 10000 // 10s - Timeout para estabelecer conexão
```

**Ajustes recomendados por uso:**
- **1-5 usuários:** Padrão (20 conexões)
- **5-10 usuários:** Aumentar para 30
- **10+ usuários:** Considerar plano pago do Tembo

---

## 📦 Migração de Dados

### O que o Script Faz?

1. **Verifica** conexão com PostgreSQL
2. **Cria backup** do SQLite existente
3. **Cria schema** PostgreSQL completo
4. **Migra dados** tabela por tabela
5. **Registra instância** com UUID único
6. **Valida** integridade dos dados

### Estrutura do Banco Migrado

Todas as 13 tabelas são migradas:
- `usuarios` - Usuários do sistema
- `lojas` - Lojas cadastradas
- `relatorios` - Relatórios diários
- `demandas` - Tickets de suporte
- `vendedores` - Vendedores
- `logs` - Logs de auditoria
- `temp_tokens` - Tokens temporários
- `estoque_tecnico` - Estoque técnico
- `assistencias` - Assistências técnicas
- `pdf_tickets` - PDFs de tickets
- `pdf_rankings` - PDFs de rankings
- `google_drive_backups` - Controle de backups
- `instances` - Instâncias registradas

### Dados Adicionados na Migração

O script adiciona automaticamente:
- Coluna `source_instance` (UUID) em todas as tabelas
- Registro na tabela `instances` com:
  - UUID da instância
  - Nome do computador
  - Data de criação
  - Última sincronização

---

## ✅ Verificação e Testes

### 1. Verificar Conexão

```bash
# Teste rápido de conexão
node -e "require('./src/config/postgresql').testConnection()"
```

### 2. Verificar Tamanho do Banco

```bash
# Ver tamanho atual
node -e "
const pg = require('./src/config/postgresql');
pg.getDatabaseSize().then(size => {
  console.log('Tamanho:', pg.formatBytes(size));
  process.exit(0);
});
"
```

### 3. Verificar Dados Migrados

```bash
# Contar registros por tabela
node -e "
const pg = require('./src/config/postgresql');
const tables = ['usuarios', 'lojas', 'relatorios', 'demandas'];
(async () => {
  for (const table of tables) {
    const result = await pg.query(\`SELECT COUNT(*) FROM \${table}\`);
    console.log(\`\${table}: \${result.rows[0].count} registros\`);
  }
  process.exit(0);
})();
"
```

### 4. Verificar Instância Registrada

```bash
# Ver instâncias registradas
node -e "
const pg = require('./src/config/postgresql');
pg.query('SELECT * FROM instances ORDER BY created_at DESC').then(r => {
  console.log('Instâncias:', JSON.stringify(r.rows, null, 2));
  process.exit(0);
});
"
```

---

## 🖥️ Múltiplos Computadores

### Cenário: Acessar o Mesmo Banco em Vários Computadores

#### Computador 1 (Principal) - JÁ CONFIGURADO ✅
- Migração já foi executada
- Dados já estão no Tembo

#### Computador 2, 3, 4... (Novos)

##### Passo 1: Clonar Repositório
```bash
git clone seu-repositorio.git
cd seu-repositorio
npm install
```

##### Passo 2: Configurar .env
Copie as **MESMAS credenciais** do Computador 1:

```bash
# .env no Computador 2
PGHOST=myapp-prod-x7k2m.data-1.use1.tembo.io  # MESMO host
PGPORT=5432
PGDATABASE=postgres
PGUSER=postgres
PGPASSWORD=tEmB0_s3cUR3_pA55w0rD_2024         # MESMA senha
PGSSLMODE=require
INSTANCE_NAME=Computador 2 - Maria             # DIFERENTE (identifica)
```

##### Passo 3: NÃO Executar Migração
**⚠️ IMPORTANTE:** NO Computador 2, **NÃO** execute:
```bash
# ❌ NÃO EXECUTE ISTO NO COMPUTADOR 2
# node scripts/migrate-to-postgres.js
```

##### Passo 4: Apenas Iniciar
```bash
npm start
```

##### Passo 5: Pronto!
- O sistema conecta automaticamente ao banco na nuvem
- Todos os dados já estão lá
- Mudanças em qualquer computador são sincronizadas em tempo real

---

### Unificar Dados de Múltiplas Instâncias SQLite

Se você tinha bancos SQLite separados em diferentes computadores e quer unificá-los:

#### Passo 1: Migrar Primeiro Computador
```bash
# No Computador 1
node scripts/migrate-to-postgres.js
```

#### Passo 2: Unificar Outros Computadores
```bash
# No Computador 2 (com SQLite local)
node scripts/merge-databases.js
```

#### O Script Faz:
1. Conecta ao PostgreSQL existente
2. Lê dados do SQLite local
3. Mescla evitando duplicatas
4. Mantém rastreabilidade por instância

---

## 🔧 Solução de Problemas

### Erro: "Cannot connect to PostgreSQL"

**Possíveis causas:**
1. Credenciais incorretas
2. SSL não configurado
3. Firewall bloqueando porta 5432
4. Instância Tembo desligada

**Solução:**
```bash
# 1. Verificar credenciais no .env
cat .env | grep PG

# 2. Testar conexão básica
node -e "require('./src/config/postgresql').testConnection()"

# 3. Verificar SSL
# Deve estar: PGSSLMODE=require

# 4. Verificar status no dashboard Tembo
# https://cloud.tembo.io/dashboard
```

---

### Erro: "relation does not exist"

**Causa:** Schema PostgreSQL não foi criado

**Solução:**
```bash
# Execute novamente a migração
node scripts/migrate-to-postgres.js
```

---

### Erro: "Pool has already been terminated"

**Causa:** Pool de conexões foi fechado prematuramente

**Solução:**
```bash
# Reinicie o servidor
npm start
```

---

### Banco Ficou Lento

**Possíveis causas:**
1. Muitas conexões abertas
2. Queries não otimizadas
3. Índices faltando

**Solução:**
```bash
# Verificar número de conexões
node -e "
const pg = require('./src/config/postgresql');
pg.query('SELECT count(*) FROM pg_stat_activity WHERE datname = \$1',
  [process.env.PGDATABASE]).then(r => {
  console.log('Conexões ativas:', r.rows[0].count);
  process.exit(0);
});
"
```

---

### Backup Automático Não Funciona

**Causa:** Email não configurado ou banco < 4GB

**Verificar:**
```bash
# Ver tamanho atual
node -e "
const pg = require('./src/config/postgresql');
pg.getDatabaseSize().then(size => {
  const gb = size / (1024 ** 3);
  console.log(\`Tamanho: \${gb.toFixed(2)} GB\`);
  if (gb >= 4) {
    console.log('✅ Backup será acionado');
  } else {
    console.log(\`⏳ Faltam \${(4 - gb).toFixed(2)} GB para backup automático\`);
  }
  process.exit(0);
});
"
```

---

## ❓ Perguntas Frequentes

### 1. Posso usar SQLite e PostgreSQL ao mesmo tempo?
**Não.** O sistema usa UM banco por vez. Escolha:
- SQLite: Desenvolvimento local
- PostgreSQL: Produção/Multi-usuário

### 2. Como voltar para SQLite?
```bash
# Remova as variáveis PG* do .env
# Reinicie o servidor
npm start
```

### 3. Perdi as credenciais do Tembo. E agora?
1. Acesse: https://cloud.tembo.io/dashboard
2. Clique na instância
3. Em "Connection Info", clique no ícone do olho para ver a senha
4. Ou redefina a senha no painel

### 4. Quanto custa o Tembo?
- **Hobby Tier:** Gratuito até 10GB
- **Pro Tier:** A partir de $10/mês (50GB)
- Ver planos: https://tembo.io/pricing

### 5. Posso migrar novamente?
**Sim**, mas cuidado:
```bash
# Migração SUBSTITUI os dados no PostgreSQL
# Faça backup antes:
node scripts/backup-postgresql.js  # Se existir
```

### 6. Como fazer backup manual?
No dashboard do Tembo:
1. Clique na instância
2. Vá em "Backups"
3. Clique em "Create Backup"

### 7. Posso usar outro provedor PostgreSQL?
**Sim!** O sistema funciona com qualquer PostgreSQL:
- **Supabase:** https://supabase.com (2GB grátis)
- **ElephantSQL:** https://elephantsql.com (20MB grátis)
- **Neon:** https://neon.tech (0.5GB grátis)
- **Heroku Postgres:** $5/mês (1GB)

Basta ajustar as credenciais no `.env`.

### 8. Como atualizar a senha do PostgreSQL?
1. No dashboard do Tembo, vá em "Settings"
2. Clique em "Reset Password"
3. Copie a nova senha
4. Atualize `PGPASSWORD` no `.env`
5. Reinicie o servidor

### 9. Posso compartilhar o banco com outros?
**Sim**, mas com cuidado:
- Compartilhe apenas com pessoas de confiança
- **NUNCA** compartilhe o arquivo `.env` completo (tem secrets)
- Considere criar usuários separados no PostgreSQL

### 10. O que acontece se eu atingir 10GB?
1. Sistema envia email de alerta aos 8GB
2. Aos 10GB, banco fica read-only (Tembo policy)
3. Opções:
   - Limpar dados antigos
   - Fazer upgrade para plano pago
   - Migrar para outro provedor

---

## 📚 Recursos Adicionais

### Documentação do Projeto
- [Guia Rápido PostgreSQL](./GUIA_RAPIDO_POSTGRESQL.md)
- [Migração Detalhada](./POSTGRESQL_MIGRATION.md)
- [README Principal](../README.md)

### Documentação Tembo.io
- [Documentação Oficial](https://tembo.io/docs)
- [Guia de Início](https://tembo.io/docs/getting-started)
- [Connection Strings](https://tembo.io/docs/connections)
- [Backup & Restore](https://tembo.io/docs/backup-restore)

### Documentação PostgreSQL
- [PostgreSQL Oficial](https://www.postgresql.org/docs/)
- [pg (Node.js driver)](https://node-postgres.com/)

### Comunidade & Suporte
- [Tembo Discord](https://discord.gg/tembo) - Suporte da comunidade
- [Issues do Projeto](../issues) - Reportar bugs

---

## 🎉 Conclusão

Parabéns! Você agora tem:

- ✅ Banco de dados na nuvem (10GB grátis)
- ✅ Acesso de qualquer lugar
- ✅ Sincronização em tempo real
- ✅ Backups automáticos
- ✅ Monitoramento de uso
- ✅ Suporte multi-computador

**Próximos passos:**
1. Configure outros computadores (se necessário)
2. Configure email para alertas de backup
3. Monitore o uso no dashboard do Tembo
4. Compartilhe o sistema com sua equipe

**Dúvidas?**
- Consulte a seção [Solução de Problemas](#solução-de-problemas)
- Veja as [Perguntas Frequentes](#perguntas-frequentes)
- Abra uma issue no repositório

---

**Documento criado em:** 2025-11-12
**Última atualização:** 2025-11-12
**Versão:** 1.0.0
