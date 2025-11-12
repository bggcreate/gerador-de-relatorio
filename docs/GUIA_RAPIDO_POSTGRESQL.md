# 🚀 Guia Rápido: PostgreSQL + Monitoramento

## ⏱️ Setup em 5 Minutos

### 1️⃣ Criar Conta no Tembo.io (2 min)

```
1. Acesse: https://cloud.tembo.io
2. Clique em "Sign Up" (grátis, 10GB)
3. Crie instância "Hobby Tier"
4. Anote as credenciais:
   - Host: xxxxxxx.data-1.use1.tembo.io
   - Port: 5432
   - Database: postgres
   - User: postgres
   - Password: xxxxxxxxx
```

### 2️⃣ Configurar .env (1 min)

Adicione no arquivo `.env`:

```bash
# PostgreSQL Tembo.io
PGHOST=seu-host.data-1.use1.tembo.io
PGPORT=5432
PGDATABASE=postgres
PGUSER=postgres
PGPASSWORD=sua-senha-aqui
PGSSLMODE=require

# Identificação
INSTANCE_NAME=Meu Computador

# Email (opcional - para backups automáticos)
SMTP_USER=seu-email@gmail.com
SMTP_PASS=senha-app-gmail
BACKUP_EMAIL_TO=destino@exemplo.com
```

### 3️⃣ Migrar Dados (2 min)

```bash
node scripts/migrate-to-postgres.js
```

Aguarde a mensagem:
```
✅ Migração concluída com sucesso!
```

### 4️⃣ Pronto! 🎉

Seu sistema agora:
- ✅ Está na nuvem (acesse de qualquer lugar)
- ✅ Monitora tamanho automaticamente
- ✅ Faz backup ao atingir 4GB
- ✅ Envia backup por email

## 📊 Como Verificar

### Tamanho do Banco
```bash
npm start
```

Procure por:
```
📊 Tamanho do banco: 245.67 MB (6.14% do limite de 4GB)
```

### Backups Criados
Verifique a pasta:
```
data/backups/
```

## 🔄 Múltiplos Computadores

### No Computador 2, 3, etc:

1. Copie as mesmas credenciais do `.env`
2. **NÃO** execute `migrate-to-postgres.js`
3. Apenas rode `npm start`
4. Pronto! Todos conectados ao mesmo banco

### Unificar Dados Futuramente

```bash
node scripts/merge-databases.js
```

Siga as instruções interativas.

## 💡 Dicas

1. **Primeira vez:** Use `migrate-to-postgres.js`
2. **Outros computadores:** Apenas configure `.env` e rode
3. **Backup manual:** Está na interface administrativa
4. **Email de backup:** Configure Gmail com senha de app

## ⚠️ Importante

- **Nunca compartilhe** o arquivo `.env`
- **Faça backup** das credenciais do Tembo.io
- **Monitore** o email para alertas de 4GB

---

📖 **Documentação Completa:** `docs/POSTGRESQL_MIGRATION.md`
