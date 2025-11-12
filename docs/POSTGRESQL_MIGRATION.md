# Migração para PostgreSQL (Tembo.io)

## 📋 Visão Geral

Este guia detalha como migrar seu sistema de SQLite para PostgreSQL usando o Tembo.io, permitindo que múltiplos computadores acessem o mesmo banco de dados.

## ✨ Recursos Implementados

### 1. **Banco de Dados Centralizado**
- ✅ Migração completa de SQLite para PostgreSQL
- ✅ Suporte para múltiplos computadores acessando o mesmo banco
- ✅ Pool de conexões para alta performance

### 2. **Monitoramento Automático**
- ✅ Verificação periódica do tamanho do banco (a cada 6 horas)
- ✅ Backup automático quando atingir 4GB
- ✅ Envio de backup por email automaticamente

### 3. **Sistema de Unificação**
- ✅ Rastreamento de origem dos dados (source_instance UUID)
- ✅ Script para mesclar múltiplos bancos em um consolidado
- ✅ Análise completa de dados de todas as instâncias

## 🚀 Passo 1: Criar Conta no Tembo.io

### 1.1. Acesse o Tembo.io
```
https://cloud.tembo.io
```

### 1.2. Crie uma Conta
- Clique em "Sign Up" (grátis, sem cartão de crédito)
- Use seu email pessoal ou corporativo
- Confirme o email

### 1.3. Crie uma Nova Instância
1. No painel, clique em **"Create Instance"**
2. Escolha **"Hobby Tier"** (gratuito - 10GB)
3. Configurações recomendadas:
   - **Name**: sistema-lojas
   - **Region**: us-east-1 (mais próximo do Brasil)
   - **Stack**: Standard (padrão)
4. Clique em **"Create"**

### 1.4. Obtenha as Credenciais de Conexão
Após criar a instância, você verá as informações de conexão:

```
Host: <seu-host>.data-1.use1.tembo.io
Port: 5432
Database: postgres
User: postgres
Password: <sua-senha-gerada>
```

**⚠️ IMPORTANTE:** Anote todas essas informações!

## 🔧 Passo 2: Configurar Variáveis de Ambiente

### 2.1. Crie/Edite o arquivo `.env`

Adicione as seguintes variáveis:

```bash
# === POSTGRESQL (TEMBO.IO) ===
PGHOST=seu-host.data-1.use1.tembo.io
PGPORT=5432
PGDATABASE=postgres
PGUSER=postgres
PGPASSWORD=sua-senha-aqui
PGSSLMODE=require

# === IDENTIFICAÇÃO DA INSTÂNCIA ===
INSTANCE_UUID=generated-on-first-run
INSTANCE_NAME=Computador Principal

# === EMAIL PARA BACKUPS ===
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_SECURE=false
SMTP_USER=seu-email@gmail.com
SMTP_PASS=sua-senha-app-gmail
BACKUP_EMAIL_TO=email-destino@exemplo.com
```

### 2.2. Configurar Email do Gmail (Opcional mas Recomendado)

Para receber backups automáticos por email:

1. Acesse https://myaccount.google.com/apppasswords
2. Crie uma senha de app para "Email"
3. Use essa senha no campo `SMTP_PASS`

## 📦 Passo 3: Executar a Migração

### 3.1. Instalar Dependências (se necessário)
```bash
npm install
```

### 3.2. Executar Script de Migração

```bash
# Define o caminho do banco SQLite (se diferente do padrão)
export SQLITE_DB_PATH=./data/database.db

# Executa a migração
node scripts/migrate-to-postgres.js
```

### 3.3. O que o Script Faz

1. ✅ Cria todas as tabelas no PostgreSQL
2. ✅ Migra todos os dados do SQLite
3. ✅ Adiciona coluna `source_instance` para rastreamento
4. ✅ Registra a instância no banco
5. ✅ Exibe relatório de migração

**Saída esperada:**
```
🚀 Iniciando migração SQLite -> PostgreSQL...

📋 Etapa 1: Criando schema no PostgreSQL...
✅ Schema criado

📋 Etapa 2: Registrando instância...
✅ Instância registrada: a1b2c3d4-e5f6-...

📋 Etapa 3: Migrando dados...

   Migrando tabela: usuarios...
   ✅ usuarios: 5 registros migrados
   Migrando tabela: lojas...
   ✅ lojas: 12 registros migrados
   ...

✅ Migração concluída com sucesso!
```

## ⚙️ Passo 4: Atualizar o Servidor

### 4.1. Editar `server.js`

Adicione no início do arquivo (após os requires):

```javascript
const pgConfig = require('./src/config/postgresql');

// Inicializa PostgreSQL se configurado
(async () => {
    if (pgConfig.isPostgresEnabled()) {
        console.log('🐘 PostgreSQL detectado - usando banco na nuvem');
        const connected = await pgConfig.testConnection();
        if (!connected) {
            console.error('❌ Falha ao conectar ao PostgreSQL');
            process.exit(1);
        }
        
        // Inicia monitoramento automático
        const dbMonitor = require('./src/services/dbMonitorService');
        dbMonitor.startMonitoring();
    } else {
        console.log('📁 Usando SQLite local');
    }
})();
```

### 4.2. Reiniciar o Servidor

```bash
npm start
```

## 🔍 Passo 5: Monitoramento Automático

O sistema agora monitora automaticamente o tamanho do banco:

- **Verificação:** A cada 6 horas
- **Limite:** 4GB
- **Ação:** Backup automático + envio por email

### Logs do Monitoramento

```
🔍 Iniciando monitoramento de banco de dados (limite: 4GB)
📅 Verificação agendada: 0 */6 * * *
📊 Tamanho do banco: 245.67 MB (6.14% do limite de 4GB)
✅ Monitoramento iniciado
```

### Quando Atingir 4GB

```
⚠️  LIMITE ATINGIDO! Criando backup automático...
📦 Criando backup do banco de dados...
✅ Backup criado: backup-2025-11-12.sql (3.8 GB)
✅ Email de backup enviado com sucesso para: seu-email@exemplo.com
✅ Backup registrado no banco
✅ Processo de backup automático concluído
```

## 🔄 Passo 6: Unificar Bancos de Dados (Futuro)

Quando você tiver múltiplas instâncias rodando em computadores diferentes e quiser consolidar todos os dados:

### 6.1. Executar Script de Unificação

```bash
node scripts/merge-databases.js
```

### 6.2. Processo Interativo

O script perguntará:

```
=================================================================
UNIFICAÇÃO DE BANCOS DE DADOS POSTGRESQL
=================================================================

Este script mescla dados de outro banco PostgreSQL para o banco atual.
Certifique-se de ter as credenciais do banco de origem.

Host do banco de origem: outro-host.data-1.use1.tembo.io
Porta (padrão 5432): 5432
Nome do banco: postgres
Usuário: postgres
Senha: ****
Usar SSL? (s/n, padrão n): s
```

### 6.3. Resultado da Unificação

```
✅ Conectado ao banco de origem: postgres@outro-host.data-1.use1.tembo.io
📋 Instância de origem: Computador Loja 2 (b2c3d4e5-...)

📊 Estatísticas da instância de origem:
   - relatorios: 1543 registros
   - vendedores: 89 registros
   - logs: 5621 registros
   TOTAL: 7253 registros

Deseja mesclar 7253 registros? (s/n): s

🚀 Mesclando dados...

   relatorios: 1543/1543 registros inseridos
   vendedores: 89/89 registros inseridos
   logs: 5621/5621 registros inseridos

✅ Mesclagem concluída!
   - Total inserido: 7253 registros
   - Instância: Computador Loja 2 (b2c3d4e5-...)
```

## 📊 Análise de Dados Consolidados

Após unificar, você pode fazer consultas consolidadas:

```sql
-- Total de vendas de todas as instâncias
SELECT 
    source_instance,
    COUNT(*) as total_relatorios,
    SUM(vendas_loja) as total_vendas
FROM relatorios
GROUP BY source_instance;

-- Análise por loja de todas as instâncias
SELECT 
    loja,
    COUNT(*) as total_relatorios,
    AVG(vendas_loja) as media_vendas
FROM relatorios
GROUP BY loja
ORDER BY total_relatorios DESC;
```

## 🔐 Segurança

### Proteja suas Credenciais

1. **Nunca compartilhe** o arquivo `.env`
2. **Adicione ao .gitignore:**
   ```
   .env
   .env.local
   ```

3. **Use senhas fortes** no Tembo.io
4. **Rotacione senhas** periodicamente

## 📈 Limites do Plano Gratuito (Tembo.io)

| Recurso | Limite |
|---------|--------|
| **Armazenamento** | 10 GB |
| **RAM** | 1 GB |
| **CPU** | 0.25 CPU |
| **Uptime** | ~99% (Spot instances) |
| **Backup** | Manual |

**💡 Dica:** Com o sistema de backup automático aos 4GB, você tem margem de segurança!

## ❓ Troubleshooting

### Erro: "FATAL: password authentication failed"
- Verifique as credenciais no `.env`
- Confirme que copiou a senha corretamente do Tembo.io

### Erro: "Connection timeout"
- Verifique sua conexão com a internet
- Confirme que `PGSSLMODE=require` está configurado

### Email não enviado
- Verifique credenciais SMTP no `.env`
- Use senha de app do Gmail (não a senha normal)

### Dados não aparecem após migração
- Verifique os logs de migração
- Execute: `node scripts/migrate-to-postgres.js` novamente

## 📞 Suporte

- **Documentação Tembo.io:** https://tembo.io/docs
- **PostgreSQL Docs:** https://www.postgresql.org/docs/

---

✅ **Migração completa! Agora você pode:**
- Acessar o mesmo banco de múltiplos computadores
- Receber backups automáticos por email
- Consolidar dados de todas as instâncias

🎉 **Parabéns! Seu sistema está pronto para escalar!**
