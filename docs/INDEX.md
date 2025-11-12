# 📚 Índice da Documentação

Bem-vindo à documentação do Sistema de Monitoramento e Gestão de Lojas.

## 🚀 Início Rápido

- **[README Principal](../README.md)** - Visão geral do projeto e como começar
- **[Credenciais de Login](CREDENCIAIS_LOGIN.md)** - Usuário e senha padrão
- **[Configuração Rápida](CONFIGURACAO_RAPIDA.md)** - Setup básico para começar a usar

## 📖 Guias de Instalação

- **[Como Rodar em Qualquer Máquina](COMO_RODAR_EM_QUALQUER_MAQUINA.md)** - Guia completo de instalação
- **[Instalação em PC Local](GUIA_INSTALACAO_PC_LOCAL.md)** - Instalação em ambiente local
- **[Setup Web](SETUP_WEB.md)** - Configuração para ambiente web
- **[Executável Standalone](EXECUTAVEL_STANDALONE.md)** - Como criar versão executável

## 🔧 Configuração

- **[Meu Guia de Configuração](MEU_GUIA_CONFIGURACAO.md)** - Configuração personalizada
- **[Google Drive Setup](GOOGLE_DRIVE_SETUP.md)** - Integração com Google Drive para backups
- **[Acesso Remoto DDNS](GUIA_ACESSO_REMOTO_DDNS.md)** - Configuração de acesso remoto
- **[DEV Access](DEV_ACCESS.md)** - Acesso temporário de desenvolvimento

## 🔐 Segurança e Permissões

- **[Sistema de Roles](ROLES_SYSTEM.md)** - Gerenciamento de permissões e funções

## 📊 Funcionalidades

- **[Dashboard Improvements](DASHBOARD_IMPROVEMENTS.md)** - Melhorias no painel administrativo
- **[Export All Feature](EXPORT_ALL_FEATURE.md)** - Funcionalidade de exportação em massa
- **[Anexos PDF](GUIA_ANEXOS_PDF.md)** - Como trabalhar com anexos em PDF
- **[Solução Anexos Relatório](SOLUCAO_ANEXOS_RELATORIO_212.md)** - Correções e soluções

## 🔌 Integrações

- **[Integração Intelbras](INTELBRAS_INTEGRACAO.md)** - Conexão com DVR/NVR Intelbras

## 📝 Histórico

- **[Resumo de Melhorias](RESUMO_MELHORIAS.md)** - Histórico de atualizações e melhorias

## 📁 Estrutura do Projeto

```
├── src/                    # Código fonte organizado
│   ├── config/             # Arquivos de configuração
│   ├── database/           # Módulo de banco de dados
│   ├── middleware/         # Middlewares customizados
│   ├── services/           # Serviços (logs, auth, PDF, etc.)
│   └── utils/              # Utilitários
├── public/                 # Arquivos públicos (CSS, JS, imagens)
│   ├── css/                # Estilos
│   └── js/                 # JavaScript do frontend
├── views/                  # Templates HTML
├── data/                   # Dados e arquivos
│   ├── database.db         # Banco de dados SQLite
│   ├── reports/            # Relatórios gerados
│   ├── pdfs/               # PDFs anexados
│   ├── backups/            # Backups do banco
│   └── dvr_files/          # Arquivos DVR
├── docs/                   # Documentação
├── scripts/                # Scripts auxiliares
└── server.js               # Servidor principal
```

## 💡 Dicas

- Sempre consulte o `.env.example` para ver todas as variáveis de ambiente disponíveis
- Mantenha backups regulares do banco de dados
- Configure o Google Drive para backups automáticos
- Use o sistema de logs para auditoria e troubleshooting

## 🆘 Suporte

Em caso de dúvidas ou problemas:
1. Consulte a documentação relevante acima
2. Verifique os logs do sistema em `/logs`
3. Entre em contato com o suporte técnico
