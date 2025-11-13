# Sistema de Monitoramento de Lojas de Varejo

## Visão Geral
Sistema web completo para gestão e monitoramento de lojas de varejo, construído com Node.js, Express e SQLite. O sistema oferece funcionalidades abrangentes para gerenciar lojas, gerar relatórios de vendas, processar uploads de PDFs, exportar dados e visualizar indicadores de desempenho através de um dashboard interativo.

## Preferências de Desenvolvimento
- Linguagem simples e direta
- Desenvolvimento iterativo
- Consultar antes de mudanças significativas
- Explicações detalhadas quando necessário

## Arquitetura do Sistema

### Decisões de UI/UX
O sistema apresenta um design moderno e premium inspirado em interfaces Apple. Principais características visuais:
- **Glassmorfismo**: Efeitos de vidro fosco em cards, modais e menus
- **Micro-animações**: Animações sutis em botões e estados hover
- **Design Responsivo**: Totalmente adaptável a diferentes dispositivos
- **Tema**: Toggle de modo escuro com transições suaves
- **Login**: Página hero com gradientes e animações sequenciais
- **Componentes**: Botões padronizados, cards com sombras em camadas, formulários com labels destacados
- **Dashboard**: Cards de indicadores, gráficos de barras comparativos e gráfico de rosca
- **Navegação**: Sidebar com barras de destaque deslizantes e ícones animados
- **Paleta de Cores Pastel**: Cores suaves para evitar fadiga visual:
  - Botões: #d0ebff (azul detalhes), #e9ecef (cinza edição), #ffe3e3 (vermelho exclusão)
  - Badges: #c3fae8 (verde ativo), #ffe3e3 (vermelho demitido), #f1f3f5 (cinza inativo), #a5d8ff (azul contador)
  - Status de vendedor: Ativo, Demitido (com data), Inativo (sem data de demissão)

### Implementações Técnicas
- **Backend**: Node.js com framework Express.js
- **Banco de Dados**: 
  - **SQLite** (local): Banco padrão com criação automática de tabelas (`DB_PATH`)
  - **PostgreSQL** (nuvem): Suporte completo para Tembo.io e outros provedores com pool de conexões, monitoramento automático e backup
- **Autenticação**: Sistema de login com diferentes níveis de acesso, gerenciamento de sessões e tokens JWT temporários para desenvolvimento. Credenciais padrão: `admin`/`admin`
- **Upload de Arquivos**: Multer para `multipart/form-data`
- **Processamento de PDFs**: pdf-parse para leitura e pdfkit para geração
- **Exportação Excel**: exceljs para relatórios em planilha
- **Dashboard**: Gráficos e estatísticas dinâmicas
- **Sistema de Demandas**: Gerenciamento interno de solicitações
- **Backup/Restore**: Funcionalidade de backup e restauração do banco
- **Módulo de Assistência Técnica**: Gerencia chamados técnicos, controle de estoque de peças e registro de eventos
- **Módulo DVR/NVR**: Gerencia dispositivos Intelbras com coleta de logs e gestão de arquivos (gravações, capturas, relatórios XML/JSON). Armazena arquivos em `data/dvr_files/<dvrId>/`
- **PostgreSQL Multi-Instância** (NOVO):
  - Conexão com PostgreSQL via Tembo.io (10GB gratuito)
  - Pool de conexões otimizado para múltiplos acessos simultâneos
  - Monitoramento automático do tamanho do banco (verificação a cada 6 horas)
  - Backup automático ao atingir 4GB com envio por email
  - Sistema de rastreamento de origem (source_instance UUID)
  - Script de migração SQLite → PostgreSQL com preservação de foreign keys
  - Script de unificação de múltiplos bancos para análise consolidada

### Especificações de Funcionalidades
- Autenticação e controle de acesso
- Gestão de lojas com campos para CEP, telefone de contato e gerente
- Geração e consulta de relatórios de vendas
- **Processamento e Importação de PDFs**: 
  - **Ranking Dia PDF**: Upload e extração automática de métricas PA, Preço Médio e Atendimento Médio. Valida nome da loja e data antes de processar
  - **Ticket Dia PDF**: Upload e armazenamento de PDFs para consulta futura com filtros por loja e data
  - **Tabela do banco**: `pdf_tickets` armazena metadados e caminhos dos arquivos
  - **Endpoints**: POST `/api/pdf/ranking`, POST `/api/pdf/ticket`, GET `/api/pdf/tickets`, GET `/api/pdf/tickets/:id/download`
- Exportação de relatórios em TXT, PDF e Excel
- Dashboard interativo com gráficos e estatísticas
- Sistema interno de gerenciamento de demandas
- Backup e restauração do banco de dados
- Módulo de assistência técnica com gestão de estoque e visualizações restritas
- Sistema de tokens JWT temporários para desenvolvimento com validade configurável
- **Módulo de Monitoramento DVR/NVR** (Totalmente Funcional):
  - **Gestão de Dispositivos**: Registro e gerenciamento de DVRs/NVRs Intelbras com IP, porta, credenciais e localização
  - **Sistema de Logs**: Coleta e filtragem de logs com timestamps, tipos de evento e descrições
  - **Gestão de Arquivos**: Upload e download de gravações, capturas e relatórios com rastreamento de metadados
  - **Tabelas do banco**: `dvr_dispositivos` (dispositivos), `dvr_logs` (eventos), `dvr_arquivos` (arquivos com metadados)
  - **Armazenamento**: Instância dedicada do multer salvando arquivos em `data/dvr_files/`
  - **Segurança**: Limite de 500MB por arquivo, filtragem de MIME types, autenticação obrigatória
  - **Endpoints**: POST/GET/PUT/DELETE `/api/dvr/dispositivos`, POST/GET `/api/dvr/logs`, POST/GET/DELETE `/api/dvr/arquivos`
  - **Frontend**: Interface com três abas (Dispositivos, Logs, Arquivos) com filtros, paginação e upload/download
  - **Rota**: `/dvr-monitor` - acessível via menu "DVR/NVR" após login
  - **Dados de Exemplo**: 3 dispositivos (2 online, 1 offline), 6 logs de eventos, 4 registros de arquivos
  - **Nota**: Este módulo NÃO inclui streaming de vídeo ao vivo, apenas gerenciamento de logs e arquivos
  - **Integração Intelbras** (Implementada):
    - Serviço Node.js que conecta diretamente aos DVRs via API HTTP nativa
    - Não requer DLLs do Windows (funciona em Linux/Replit)
    - Coleta logs e eventos automaticamente dos dispositivos
    - Script: `scripts/collect-dvr-logs.js`
    - Serviço: `services/intelbrasDvrService.js`
    - Documentação completa disponível em `docs/`

### Decisões de Design do Sistema
- **Estrutura do Projeto**: Separação clara de responsabilidades com `server.js` como ponto de entrada principal
- **Ambiente**: Configurado para rodar na porta 5000, binding em `0.0.0.0`
- **Modularidade**: Novas funcionalidades estruturadas com tabelas e APIs dedicadas
- **Responsividade**: CSS extensivo para layouts adaptativos
- **Segurança**: CSP, tokens CSRF, hash bcrypt para senhas e rate limiting para login

## Dependências Externas
- **express**: Framework do servidor web
- **sqlite3**: Driver do banco SQLite (local)
- **pg**: Driver PostgreSQL para bancos em nuvem (Tembo.io)
- **node-cron**: Agendamento de tarefas (monitoramento de banco)
- **uuid**: Geração de UUIDs para rastreamento de instâncias
- **express-session**: Middleware para gerenciamento de sessões
- **multer**: Middleware para `multipart/form-data`
- **pdf-parse**: Biblioteca para análise de PDFs
- **pdfkit**: Biblioteca para geração de PDFs
- **exceljs**: Biblioteca para criação e leitura de arquivos Excel XLSX
- **jsonwebtoken**: Geração e verificação de JWTs
- **bcrypt**: Hash de senhas
- **axios**: Cliente HTTP para integrações
- **googleapis**: Integração com Google Drive e Gmail
- **nodemailer**: Envio de emails (backups automáticos)
- **ngrok**: Acesso remoto durante desenvolvimento

## Integrações Implementadas

### Google Drive (✅ Configurado - 13/11/2025)
- **Status**: Autenticado e funcionando no ambiente local do usuário
- **Configuração**: OAuth 2.0 manual usando credenciais do Google Cloud Console
- **Credenciais**: Armazenadas como secrets (GOOGLE_CLIENT_ID, GOOGLE_CLIENT_SECRET, GOOGLE_REFRESH_TOKEN)
- **Nota**: Usuário optou por não usar a integração nativa do Replit
- Armazenamento de relatórios (15GB gratuitos)
- Backup automático ao atingir limite
- Envio de backups por email
- Organização automática por ano/mês
- Limpeza de arquivos antigos (>90 dias)
- **Script de Configuração**: `scripts/google-auth-setup-new.js` (versão atualizada com servidor local)

### DVR Intelbras
- Conexão via API HTTP nativa
- Coleta automática de logs
- Captura de snapshots
- Monitoramento de status
- Download de gravações

## Estrutura de Arquivos
```
├── server.js              # Servidor principal
├── package.json           # Dependências do projeto
├── data/                  # Dados e banco de dados
│   ├── database.db        # Banco SQLite
│   ├── pdfs/             # PDFs anexados
│   └── dvr_files/        # Arquivos DVR
├── public/                # Arquivos públicos
│   ├── js/               # JavaScript frontend
│   │   ├── app.js        # Aplicação principal
│   │   ├── pages/        # Scripts por página
│   │   ├── utils.js      # Utilitários
│   │   └── theme.js      # Gerenciamento de tema
│   └── css/              # Estilos
├── views/                 # Templates HTML
├── services/              # Serviços de integração
├── scripts/               # Scripts auxiliares
├── middleware/            # Middlewares personalizados
└── docs/                 # Documentação técnica
```

## Credenciais Padrão
- **Usuário**: admin
- **Senha**: admin

> Altere após o primeiro acesso para garantir segurança.

## ✅ Melhorias Recentes

### Dashboard Cards com Filtros Independentes (13/11/2025)
- ✅ **Cards de Indicadores**: Implementação de filtros independentes para Assistências e Omni
- ✅ **Dados de Hoje**: Cards sempre mostram métricas do dia atual, independente do filtro de período
- ✅ **Filtro Assistências**: Dropdown exclusivo para lojas com função "Busca por Assist. Tec."
- ✅ **Filtro Omni**: Dropdown exclusivo para lojas com função "Omni"
- ✅ **Independência Total**: Mudanças em um dropdown não afetam o outro card
- ✅ **Botões da Página Lojas**: Cores padronizadas Bootstrap (btn-outline-*) e formato compacto (ícones apenas)
- ✅ **Sistema de Demandas**: Botões de excluir presentes em ambas as abas (principal e histórico)

### PostgreSQL Multi-Instância (12/11/2025)

### Sistema de PostgreSQL Multi-Instância
- ✅ **Migração para a Nuvem**: Suporte completo para PostgreSQL (Tembo.io - 10GB gratuito)
- ✅ **Múltiplos Computadores**: Vários computadores podem acessar o mesmo banco simultaneamente
- ✅ **Monitoramento Automático**: Verificação periódica do tamanho do banco a cada 6 horas
- ✅ **Backup Inteligente**: Ao atingir 4GB, backup automático com envio por email
- ✅ **Rastreamento de Origem**: Todas as tabelas incluem `source_instance` UUID
- ✅ **Unificação de Dados**: Script para consolidar dados de múltiplas instâncias
- ✅ **Documentação Completa**: Guias passo-a-passo em português

### Como Usar PostgreSQL
1. **Criar conta**: https://cloud.tembo.io (grátis, 10GB)
2. **Configurar .env**: Adicionar credenciais PostgreSQL
3. **Migrar dados**: `node scripts/migrate-to-postgres.js`
4. **Pronto!**: Sistema monitora e faz backup automaticamente

📚 **Documentação Completa**: `docs/POSTGRESQL_MIGRATION.md` e `docs/GUIA_RAPIDO_POSTGRESQL.md`

## Próximas Melhorias Planejadas
- Relatórios com gráficos avançados
- App mobile para consulta
- Dashboard em tempo real com WebSockets
- Analytics de vendas com BI
