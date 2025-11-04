# Sistema de Monitoramento de Lojas de Varejo

## Overview
This project is a web-based retail store monitoring and reporting system built with Node.js, Express, and SQLite. Its primary purpose is to provide functionalities for managing retail stores, generating various sales reports, handling PDF uploads, exporting data, and visualizing key performance indicators through a dashboard. The system aims to streamline retail operations by offering authentication, user management, and a comprehensive view of store performance, including future integration with external sales platforms like Bluve/Blu.

## User Preferences
I prefer simple language. I want iterative development. Ask before making major changes. I prefer detailed explanations.

## System Architecture

### UI/UX Decisions
The system features a modern, Apple-inspired design aesthetic with premium visual elements. Key UI/UX decisions include:
- **Glassmorphism**: Utilizes frosted glass effects with `backdrop-filter: blur()` for cards, modals, and menus.
- **Micro-animations**: Implements subtle animations for buttons (ripple, scale), hover states (translateY), and active states for tactile feedback.
- **Responsive Design**: Comprehensive responsiveness across various devices (desktop, tablet, mobile) with optimized layouts for tables, cards, forms, and modals.
- **Theming**: Includes a floating dark mode toggle with smooth transitions.
- **Login Page**: Features a hero-style login page with gradient backgrounds, floating cards, and sequenced fade-in animations.
- **Component Styling**: Standardized buttons with consistent padding, border-radius, and gradient effects. Cards have layered shadows and subtle hover effects. Forms feature bold labels, rounded inputs, and enhanced focus states.
- **Dashboard**: Enhanced with performance indicator cards, comparative bar charts, and a donut chart for conversion distribution. Ranking visuals include medal icons for top performers.
- **Navigation**: Sidebar navigation with sliding accent bars on hover/active states and animated icons. A toggle button is available to hide/show the sidebar, with its state persistently saved.

### Technical Implementations
- **Backend**: Node.js with Express.js framework.
- **Database**: SQLite for local data storage (`database.db`), with automatic creation of necessary tables on first run. Database path is configurable via `DB_PATH` environment variable.
- **Authentication**: User authentication with different access levels (admin, user, technician). Default admin credentials: `admin`/`admin`.
- **File Uploads**: Utilizes `multer` for handling file uploads.
- **PDF Processing**: `pdf-parse` for reading PDFs and `pdfkit` for generating them.
- **Excel Export**: `exceljs` for generating Excel reports.
- **Session Management**: `express-session` for managing user sessions.
- **Dashboard Features**: Dynamic graphs and statistics, including performance indicators, comparative charts, and ranking displays.
- **Demand System**: Internal system for managing demands.
- **Backup/Restore**: Functionality for backing up and restoring the database.
- **Assistência Técnica Module**:
    - **Roles**: Introduced a "Técnico" (Technician) role with restricted access based on assigned `loja_tecnico`.
    - **Functionality**: Management of technical assistance calls, stock control for repair parts, and detailed logging of assistance events. Automatic stock updates upon completion of assistance.
    - **APIs**: CRUD operations for `estoque_tecnico` and `assistencias`.

### Feature Specifications
- User authentication and access control.
- Store management (cadastro e gerenciamento de lojas).
- Sales report generation and querying.
- PDF upload and processing (Omni and Busca Técnica).
- Report export in TXT, PDF, and Excel formats (including exporting all reports to Excel).
- Interactive dashboard with graphs and statistics.
- Internal demand management system.
- Database backup and restoration.
- Technical assistance module with stock management and restricted technician views.

### System Design Choices
- **Project Structure**: Clear separation of concerns with `server.js` as the main entry point, `views/` for HTML templates, `public/` for static assets, and `data/` for the SQLite database.
- **Environment**: Configured for Replit with server running on port 5000 and binding to `0.0.0.0`.
- **Modularity**: New features like the Technical Assistance module are structured with dedicated tables and APIs.
- **Responsiveness**: Extensive CSS additions for adaptive layouts across various screen sizes.

## External Dependencies
- **express**: Web server framework.
- **sqlite3**: SQLite database driver.
- **express-session**: Middleware for session management.
- **multer**: Middleware for handling `multipart/form-data`, primarily for file uploads.
- **pdf-parse**: Library for parsing PDF files.
- **pdfkit**: Library for PDF generation.
- **exceljs**: Library for creating and reading Excel XLSX files.
- **Bluve/Blu (Planned)**: Future integration for bidirectional store synchronization, automatic report import (daily sales), and utilizing Bluve's APIs (Movement of Sales, Extract, Reconciliation).

## Project Structure
```
/
├── data/                      # Database files
│   └── database.db           # SQLite database (caminho principal configurável)
├── middleware/               # Custom middleware
│   └── roleAuth.js          # Role-based authentication
├── public/                   # Static assets
│   ├── css/
│   ├── js/
│   │   ├── pages/           # Page-specific JavaScript
│   │   ├── app.js           # Main app logic
│   │   ├── theme.js         # Theme switching
│   │   └── utils.js         # Utility functions
│   └── relatorios_gerados/  # Generated reports
├── views/                    # HTML templates
├── docs/                     # Documentation
├── attached_assets/          # Temporary/attached files
│   ├── old_prompts/         # Historical prompts
│   └── screenshots/         # Project screenshots
├── server.js                 # Main server file
├── package.json             # Dependencies
└── replit.md                # Project documentation
```

## Recent Changes

### Store Management System Implementation (2025-11-04)
- 🏪 **Aba "Lojas" - Nova Interface**:
  - Adicionados novos campos ao formulário de cadastro de lojas:
    - **CEP** (opcional)
    - **Número de Contato** (opcional)
    - **Gerente** (opcional)
    - **Nome da Loja** (obrigatório)
  - Tabela simplificada mostrando apenas: Nome, Status, e Ações
  - Status exibido como badges coloridos:
    - Verde para lojas ativas
    - Cinza para lojas inativas
  - Quatro botões de ação por loja:
    - ✏️ **Editar**: Abre formulário para edição
    - 👁️ **Detalhes**: Mostra informações completas da loja em toast
    - 🗑️ **Excluir**: Remove a loja
    - ➕ **Adicionar Vendedor**: Abre modal de vendedor para a loja específica
  - Banco de dados atualizado com novas colunas: `cep`, `numero_contato`, `gerente`
  - APIs POST e PUT atualizadas para manipular os novos campos

- 📊 **Dashboard - Simplificação**:
  - Removido card "Assistência Técnica por Loja" (tabela detalhada)
  - Mantidos os seguintes cards:
    - Card "Ticket de Assistências" (mostra todos os tickets e status)
    - Card "Loja (Bluve)" (mostra Clientes, Vendas, Taxa de Conversão)
    - Card "Métricas Principais / Monitoramento"

- 🔒 **Segurança**:
  - CSP atualizado para permitir Google Fonts (fonts.googleapis.com e fonts.gstatic.com)

### Dashboard Improvements - Assistência Técnica (2025-11-03)
- 🛠️ **Redesign da Seção "Assistência Técnica - Visão Geral"**:
  - Simplificado para mostrar apenas 3 cards com métricas diárias:
    - Assistências Concluídas (Hoje)
    - Faturamento (Hoje)
    - Assistências em Andamento (Hoje)
  - Adicionado dropdown de filtro por loja no cabeçalho da seção
  - Filtro afeta todos os cards e o card de tickets simultaneamente
  - Novo endpoint de API: `/api/assistencias/stats-daily` com suporte a filtro de loja
- 🎫 **Novo Card "Ticket de Assistências"**:
  - Card com altura fixa de 400px e scroll interno suave
  - Exibe tickets em andamento e aguardando peças
  - Design compacto com informações completas (cliente, aparelho, técnico, defeito, valor)
  - Scrollbar personalizada discreta com hover effect laranja
  - Responsivo: altura reduzida para 300px em mobile
  - Novo endpoint de API: `/api/assistencias/tickets` com suporte a filtro de loja
  - Botão de atualização manual dos tickets
- 📊 **Card "Loja (Bluve)"**:
  - Confirmado funcionamento correto com dados de `clientes_loja`, `vendas_loja` e `tx_conversao_loja`
  - Exibe comparações com período anterior
  - Visível para todos os perfis de usuário

### Database Standardization (2025-11-03)
- 🗄️ **Padronização do Banco de Dados**:
  - Centralizado o sistema para usar apenas um banco de dados: `data/database.db`
  - Configurado caminho do banco via variável de ambiente `DB_PATH` (padrão: `database.db`)
  - Renomeado `relatorios.db` para `database.db` como banco principal
  - Atualizado sistema de backup/restore para usar caminho configurável
  - Restaurado backup com dados de login e senha funcionando
  - Sistema agora garante que não haverá leitura em múltiplos arquivos de banco diferentes

### Project Organization (2025-11-03)
- 🗂️ **Organização de Arquivos e Pastas**:
  - Removida pasta duplicada `monitoramento-lojas-varejo-main/` que continha código antigo e estava causando conflitos
  - Criada pasta `docs/` para centralizar documentação (README, DASHBOARD_IMPROVEMENTS, EXPORT_ALL_FEATURE, RESUMO_MELHORIAS, ROLES_SYSTEM)
  - Organizada pasta `attached_assets/` com subpastas `old_prompts/` e `screenshots/`
  - Estrutura do projeto agora mais limpa e organizada
- 👤 **Gerenciamento de Usuários**:
  - Removido preenchimento automático "admin" do campo de login
  - Criado usuário `dev` com senha `dev123` e perfil desenvolvedor
  - Sistema agora suporta múltiplos perfis: admin, dev, gerente, consultor, técnico
- 🔐 **Correção de Autenticação e Sessão**:
  - Corrigido problema de CSRF token no ambiente Replit
  - Ajustadas configurações de sessão (`sameSite: 'lax'`, `secure: false`, `saveUninitialized: true`)
  - Corrigida senha do usuário admin (migração completa para bcrypt)
  - Melhorado tratamento de erros na página de login com mensagens mais claras
  - Ambos os usuários (admin e dev) agora com senhas seguras usando bcrypt hash

### Scroll Minimalista e Card Bluve (2025-10-31)
- 📜 **Scroll Minimalista em Cards Expansíveis**:
  - Adicionadas classes CSS `.scrollable-card-content` e `.scrollable-table-wrapper` para containers com scroll
  - Scrollbar minimalista de 4px com hover effect em laranja (accent-primary)
  - Aplicado no card "Demandas Pendentes" (max-height: 400px)
  - Aplicado na tabela "Assistência Técnica por Loja" (max-height: 500px)
  - Cards agora mantêm altura fixa e não ocupam toda a tela quando há muitos dados
  - Responsive: altura reduzida em mobile (300px e 400px respectivamente)
- 🏪 **Card Bluve e Controle de Visualização por Perfil**:
  - **Card "Loja (Bluve)"**: Visível para TODOS os usuários (admin, gerente, técnico)
    - Exibe métricas de CLIENTES, VENDAS e TX. CONVERSÃO
    - Dados carregados automaticamente do novo relatório via `total_clientes_loja`, `total_vendas_loja`, `tx_conversao_loja`
    - Comparações com período anterior funcionando corretamente
  - **Card "Monitoramento"**: Visível APENAS para admin
    - Oculto automaticamente para gerente e técnico
    - Quando oculto, o card Bluve ocupa largura total (col-xl-12)
  - **Gráficos Comparativos**: Visíveis apenas para admin, ocultos para gerente e técnico

### UI Redesign - Logo e Navegação (2025-10-31)
- 🔲 **Nova Logo - Grid Quadriculado**: 
  - Substituído ícone anterior por grid quadriculado (bi-grid-3x3-gap-fill) - estilo Loft Design
  - Texto "Reports" removido de todas as interfaces
  - Mudanças aplicadas em: login, navbar desktop, header mobile, menu modal
  - Visual minimalista focado apenas no ícone
- 🌓 **Reposicionamento do Botão de Modo Noturno**: 
  - Botão movido do topo da sidebar para o rodapé
  - Posicionado junto com botões de ação (Live, Engrenagem, Logout)
  - Injetado dinamicamente via app.js no userInfoContainer
  - Theme.js com proteção contra listeners duplicados (data-theme-initialized)
  - Funciona corretamente em desktop e mobile
- 📋 **Simplificação do Menu**:
  - Removida aba "Usuários" do menu principal
  - Acesso a gestão de usuários mantido via botão de configurações (engrenagem)
  - app.js atualizado para remover referência a nav-gerenciar-usuarios
  - Menu principal agora mais limpo e focado