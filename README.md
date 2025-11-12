# Sistema de Gestão para Lojas de Varejo

Um sistema completo e profissional para gerenciamento de lojas de varejo, desenvolvido para facilitar o dia a dia de gestores e equipes comerciais.

## 📋 Sobre o Sistema

Este é um sistema web robusto que oferece controle total sobre operações de varejo, incluindo:

- **Gestão de Lojas**: Cadastro completo com informações detalhadas de cada unidade
- **Controle de Vendedores**: Gerenciamento de equipe com histórico e status
- **Relatórios Gerenciais**: Geração automática de relatórios em PDF e Excel
- **Dashboard Executivo**: Visualização de métricas e indicadores em tempo real
- **Monitoramento de Segurança**: Integração com sistemas DVR/NVR Intelbras
- **Assistência Técnica**: Controle de chamados e estoque de peças
- **Backup Automático**: Armazenamento seguro no Google Drive

## 🚀 Começando

### Pré-requisitos

- Node.js versão 18 ou superior
- NPM (gerenciador de pacotes do Node)

### Instalação Rápida

1. **Clone o repositório**
```bash
git clone https://github.com/seu-usuario/sistema-gestao-lojas.git
cd sistema-gestao-lojas
```

2. **Instale as dependências**
```bash
npm install
```

3. **Inicie o servidor**
```bash
npm start
```

4. **Acesse o sistema**
```
http://localhost:5000
```

### Primeiro Acesso

Use as credenciais padrão:
- **Usuário**: `admin`
- **Senha**: `admin`

> ⚠️ **Importante**: Altere a senha após o primeiro acesso para garantir a segurança.

## 💼 Funcionalidades Principais

### Gerenciamento de Lojas
- Cadastro completo com CEP, telefone e responsável
- Vinculação de vendedores a cada loja
- Controle de status (ativo/inativo)
- Histórico de alterações

### Relatórios Inteligentes
- Geração automática em PDF profissional
- Exportação para Excel
- Anexação de documentos complementares
- Filtros avançados por período e loja

### Dashboard Executivo
- Métricas de conversão em tempo real
- Indicadores de vendas Monitoramento e Bluve
- Gráficos comparativos de desempenho
- Filtros personalizados por loja

### Segurança Integrada
- Monitoramento de câmeras DVR/NVR Intelbras
- Registro de eventos e alarmes
- Download de gravações
- Captura de imagens

### Assistência Técnica
- Abertura e acompanhamento de chamados
- Controle de estoque de peças
- Histórico de atendimentos
- Relatórios de manutenção

## 🛠️ Tecnologias

O sistema foi construído com tecnologias modernas e confiáveis:

- **Backend**: Node.js + Express
- **Banco de Dados**: SQLite (leve e eficiente)
- **Frontend**: HTML5, CSS3, JavaScript + Bootstrap 5
- **Segurança**: bcrypt, JWT, helmet
- **PDFs**: PDFKit
- **Excel**: ExcelJS
- **Upload**: Multer

## 📚 Documentação Completa

Acesse a pasta `docs/` para guias detalhados:

- **Instalação Local**: Guia completo para instalar em qualquer máquina
- **Acesso Remoto**: Configuração de DDNS para acesso externo
- **Google Drive**: Integração com armazenamento na nuvem
- **DVR Intelbras**: Integração com sistemas de segurança
- **Desenvolvimento**: Tokens temporários e configurações avançadas

## 🔧 Configuração Avançada

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
PORT=5000
DB_PATH=./data/database.db
JWT_SECRET=sua-chave-secreta-aqui
```

### Backup Automático

O sistema pode ser configurado para fazer backup automático no Google Drive. Consulte `docs/GOOGLE_DRIVE_SETUP.md` para instruções detalhadas.

### Integração DVR

Para conectar câmeras Intelbras, veja o guia em `docs/INTELBRAS_INTEGRACAO.md`.

## 📊 Estrutura do Projeto

```
├── server.js               # Servidor principal
├── package.json            # Dependências do projeto
├── .env.example            # Exemplo de variáveis de ambiente
├── src/                    # Código fonte organizado
│   ├── config/             # Configurações (database, security, etc.)
│   ├── middleware/         # Middlewares customizados (auth, audit, roles)
│   ├── services/           # Serviços (logs, Google Drive, etc.)
│   └── utils/              # Funções utilitárias
├── data/                   # Dados e arquivos gerados
│   ├── database.db         # Banco de dados SQLite
│   ├── reports/            # Relatórios gerados
│   ├── pdfs/               # PDFs anexados
│   ├── backups/            # Backups do banco
│   └── dvr_files/          # Arquivos do DVR
├── public/                 # Arquivos públicos (frontend)
│   ├── css/                # Estilos
│   └── js/                 # JavaScript do frontend
├── views/                  # Templates HTML
├── scripts/                # Scripts auxiliares (build, sync, etc.)
└── docs/                   # Documentação técnica completa
```

## 🤝 Contribuindo

Sugestões e melhorias são sempre bem-vindas! Sinta-se à vontade para:

1. Fazer um fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abrir um Pull Request

## 📄 Licença

Este projeto é de uso interno e privado.

## 💡 Suporte

Para dúvidas ou problemas:
- Consulte a documentação em `docs/`
- Verifique os logs do sistema
- Entre em contato com o administrador

---

**Desenvolvido com dedicação para otimizar a gestão do seu varejo** 🏪
