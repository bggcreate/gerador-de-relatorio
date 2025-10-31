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
- **Database**: SQLite for local data storage, with automatic creation of `relatorios.db` and necessary tables on first run.
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

## Recent Changes

### UI Redesign - Logo e Navegação (2025-10-31)
- ❤️ **Nova Logo - Coração**: 
  - Substituído ícone anterior (bi-bar-chart-line-fill) por coração (bi-heart-fill)
  - Texto "Reports" removido de todas as interfaces
  - Mudanças aplicadas em: login, navbar desktop, header mobile, menu modal
  - Visual minimalista focado apenas no ícone
- 🌓 **Reposicionamento do Botão de Modo Noturno**: 
  - Botão movido para .sidebar-controls ao lado do toggle da sidebar
  - ID atualizado: #theme-toggle-desktop
  - Removido botão flutuante duplicado (#theme-toggle-floating)
  - Theme.js atualizado para usar apenas botões desktop e mobile
  - Mesmo estilo visual dos outros botões de controle (36px)
- 📋 **Simplificação do Menu**:
  - Removida aba "Usuários" do menu principal
  - Acesso a gestão de usuários mantido via botão de configurações (engrenagem)
  - app.js atualizado para remover referência a nav-gerenciar-usuarios
  - Menu principal agora mais limpo e focado