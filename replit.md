# Sistema de Monitoramento de Lojas de Varejo

## Overview
This project is a web-based retail store monitoring and reporting system built with Node.js, Express, and SQLite. Its primary purpose is to provide functionalities for managing retail stores, generating various sales reports, handling PDF uploads, exporting data, and visualizing key performance indicators through a dashboard. The system aims to streamline retail operations by offering authentication, user management, and a comprehensive view of store performance, with future integration planned for external sales platforms like Bluve/Blu.

## User Preferences
I prefer simple language. I want iterative development. Ask before making major changes. I prefer detailed explanations.

## System Architecture

### UI/UX Decisions
The system features a modern, Apple-inspired design aesthetic with premium visual elements. Key UI/UX decisions include:
- **Glassmorphism**: Utilizes frosted glass effects for cards, modals, and menus.
- **Micro-animations**: Implements subtle animations for buttons, hover states, and active states.
- **Responsive Design**: Comprehensive responsiveness across various devices.
- **Theming**: Includes a floating dark mode toggle with smooth transitions.
- **Login Page**: Features a hero-style login page with gradient backgrounds and sequenced fade-in animations.
- **Component Styling**: Standardized buttons, cards with layered shadows, and forms with bold labels and enhanced focus states.
- **Dashboard**: Enhanced with performance indicator cards, comparative bar charts, and a donut chart. Ranking visuals include medal icons.
- **Navigation**: Sidebar navigation with sliding accent bars and animated icons, with a toggle for persistent state.

### Technical Implementations
- **Backend**: Node.js with Express.js framework.
- **Database**: SQLite (`database.db`) with automatic table creation and configurable path (`DB_PATH`).
- **Authentication**: User authentication with different access levels (admin, user, technician), supporting session management and temporary JWT tokens for development. Default admin credentials: `admin`/`admin`.
- **File Uploads**: Utilizes `multer` for `multipart/form-data`.
- **PDF Processing**: `pdf-parse` for reading and `pdfkit` for generating PDFs.
- **Excel Export**: `exceljs` for generating Excel reports.
- **Dashboard Features**: Dynamic graphs and statistics.
- **Demand System**: Internal management of demands.
- **Backup/Restore**: Database backup and restoration functionality.
- **Assistência Técnica Module**: Manages technical assistance calls, stock control for repair parts, and logging assistance events. Includes "Técnico" role with restricted access.

### Feature Specifications
- User authentication and access control.
- Store management (cadastro e gerenciamento de lojas) with fields for CEP, contact number, and manager.
- Sales report generation and querying.
- PDF upload and processing (Omni and Busca Técnica).
- Report export in TXT, PDF, and Excel formats.
- Interactive dashboard with graphs and statistics.
- Internal demand management system.
- Database backup and restoration.
- Technical assistance module with stock management and restricted technician views.
- Temporary JWT token system for development with configurable validity, IP restriction, and revocation.

### System Design Choices
- **Project Structure**: Clear separation of concerns with `server.js` as the main entry point, dedicated folders for views, static assets, and data.
- **Environment**: Configured for Replit, running on port 5000 and binding to `0.0.0.0`.
- **Modularity**: New features are structured with dedicated tables and APIs.
- **Responsiveness**: Extensive CSS for adaptive layouts.
- **Security**: CSP updates, CSRF token handling, bcrypt hashing for passwords, and rate limiting for login attempts.

## External Dependencies
- **express**: Web server framework.
- **sqlite3**: SQLite database driver.
- **express-session**: Middleware for session management.
- **multer**: Middleware for handling `multipart/form-data`.
- **pdf-parse**: Library for parsing PDF files.
- **pdfkit**: Library for PDF generation.
- **exceljs**: Library for creating and reading Excel XLSX files.
- **jsonwebtoken**: For generating and verifying JWTs.
- **bcrypt**: For password hashing.
- **Bluve/Blu (Planned)**: Future integration for bidirectional store synchronization, automatic report import, and utilizing Bluve's APIs (Movement of Sales, Extract, Reconciliation).