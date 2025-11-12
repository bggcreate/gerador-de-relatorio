# 🚀 Como Integrar com o Banco Gratuito Tembo.io

## ⏱️ Integração em 10 Minutos

Seu sistema **JÁ TEM** todo o suporte para PostgreSQL implementado. Você só precisa:

### 1️⃣ Criar Conta Grátis (3 min)
```
👉 https://cloud.tembo.io
   - Clique em "Sign Up"
   - Crie instância "Hobby Tier" (10GB grátis)
```

### 2️⃣ Configurar .env (2 min)
Adicione no arquivo `.env`:
```bash
PGHOST=seu-host.data-1.use1.tembo.io
PGPORT=5432
PGDATABASE=postgres
PGUSER=postgres
PGPASSWORD=sua-senha-do-tembo
PGSSLMODE=require
INSTANCE_NAME=Meu Computador
```

### 3️⃣ Migrar Dados (5 min)
```bash
npm install
node scripts/migrate-to-postgres.js
```

### 4️⃣ Iniciar Sistema
```bash
npm start
```

## ✅ Pronto!

Seu sistema agora está:
- ✅ Na nuvem (acesse de qualquer lugar)
- ✅ Com 10GB gratuito
- ✅ Sincronização automática
- ✅ Backup automático aos 4GB

## 📖 Guia Completo
Para instruções detalhadas, consulte:
- **Guia Completo:** [docs/INTEGRACAO_TEMBO_FREE.md](docs/INTEGRACAO_TEMBO_FREE.md)
- **Guia Rápido:** [docs/GUIA_RAPIDO_POSTGRESQL.md](docs/GUIA_RAPIDO_POSTGRESQL.md)

## 🆘 Problemas?

### Erro de Conexão?
```bash
# Verifique suas credenciais
cat .env | grep PG

# Teste a conexão
node -e "require('./src/config/postgresql').testConnection()"
```

### Dúvidas?
- Consulte a [documentação completa](docs/INTEGRACAO_TEMBO_FREE.md)
- Veja as [perguntas frequentes](docs/INTEGRACAO_TEMBO_FREE.md#perguntas-frequentes)

## 🎯 Benefícios

| Antes (SQLite) | Depois (Tembo PostgreSQL) |
|----------------|---------------------------|
| Local apenas | ☁️ Acesso de qualquer lugar |
| 1 computador | 👥 Múltiplos dispositivos |
| Backup manual | 🤖 Backup automático |
| Sem sincronização | ⚡ Sync em tempo real |

---

**Criado em:** 2025-11-12
