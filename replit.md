# Sistema de Monitoramento de Lojas de Varejo

## Visão Geral
Sistema web para monitoramento e geração de relatórios de lojas de varejo, desenvolvido em Node.js com Express e SQLite.

## Estado Atual
- **Status**: Funcionando e pronto para uso
- **Tecnologia**: Node.js 20, Express.js, SQLite
- **Porto**: 5000
- **Banco de Dados**: SQLite (arquivo local em `data/relatorios.db`)

## Credenciais Padrão
- **Usuário**: admin
- **Senha**: admin

## Funcionalidades
- Autenticação de usuários com diferentes níveis de acesso (admin e usuário)
- Cadastro e gerenciamento de lojas
- Criação e consulta de relatórios de vendas
- Upload e processamento de PDFs (Omni e Busca Técnica)
- Exportação de relatórios em TXT, PDF e Excel
- Dashboard com gráficos e estatísticas
- Sistema de demandas internas
- Backup e restauração do banco de dados

## Estrutura do Projeto
```
/
├── server.js              # Servidor Express principal
├── package.json           # Dependências do projeto
├── views/                 # Templates HTML
│   ├── login.html
│   ├── index.html
│   ├── admin.html
│   ├── consulta.html
│   ├── demandas.html
│   ├── gerenciar-lojas.html
│   ├── gerenciar-usuarios.html
│   ├── novo-relatorio.html
│   └── live.html
├── public/                # Arquivos estáticos
│   ├── css/
│   ├── js/
│   └── relatorios_gerados/
└── data/                  # Banco de dados SQLite
    └── relatorios.db
```

## Dependências
- express: Servidor web
- sqlite3: Banco de dados
- express-session: Gerenciamento de sessões
- multer: Upload de arquivos
- pdf-parse: Leitura de PDFs
- pdfkit: Geração de PDFs
- exceljs: Geração de arquivos Excel

## Configuração do Ambiente Replit
- **Workflow**: Servidor rodando em `node server.js` na porta 5000
- **Deploy**: Configurado para autoscale (sem estado em memória)
- **Host**: 0.0.0.0 (necessário para o proxy do Replit)

## Mudanças Recentes

### Redesign Visual Completo - Estilo Apple (2025-10-31)
- 🎨 **Design Premium**: Visual completamente renovado com estética inspirada na Apple
- ✨ **Glassmorphism**: Efeitos de vidro fosco em cards, modais e menus com backdrop blur
- 🌈 **Página de Login Hero**: 
  - Fundo com gradiente sutil (branco → laranja pastel)
  - Animações de float suaves no background
  - Card flutuante com sombras refinadas
  - Fade-in sequenciado de todos os elementos
  - Inputs com transformação ao focus
- 🎯 **Microanimações Globais**:
  - Botões com efeito ripple e scale transform
  - Hover states com translateY(-2px)
  - Active states com feedback tátil (scale 0.98)
  - Transições com cubic-bezier(0.16, 1, 0.3, 1)
- 📊 **Cards e Componentes**:
  - Sombras em múltiplas camadas para profundidade
  - Hover com elevação suave (translateY e box-shadow)
  - Borders mais suaves (12px border-radius)
  - Backdrop filter blur(10px) para efeito premium
- 🧭 **Sidebar Navigation**:
  - Barra de acento deslizante (3px laranja) em hover/active
  - Ícones com animação de scale (1.1x em hover)
  - Translateo X(4px) suave ao passar o mouse
  - Ícone ativo colorido com accent-primary
- 📱 **Experiência Mobile Premium**:
  - Menu mobile com slide-in + fade opacity
  - Touch feedback em todos os botões (scale animations)
  - Backdrop blur no modal e backdrop
  - Espaçamento otimizado para polegar (1rem+ padding)
  - Headers mobile com animações suaves
- 🔄 **Animações de Scroll**:
  - IntersectionObserver para fade-in progressivo
  - Cards aparecem com translateY(30px) → 0
  - Transições de 0.6s com easing personalizado
  - Classe .will-animate para controle preciso
- ⏳ **Loading States**:
  - Spinner customizado com animation keyframes
  - Função setButtonLoading() para estados async
  - Página com fade-in global ao carregar
  - Animações de esqueleto prontas para uso
- 🎨 **CSS Tokens Aprimorados**:
  - Sombras refinadas (shadow-sm, shadow-md, shadow-lg)
  - Border-radius consistente (8px, 12px, 16px, 20px, 24px)
  - Transitions com cubic-bezier para fluidez premium
  - Cores vibrantes mantidas para dashboard
- 💡 **Detalhes de Polish**:
  - Tipografia Inter com letter-spacing ajustado
  - Form inputs com elevação ao focus
  - Theme toggle com microanimação
  - Todas as funcionalidades mantidas 100% intactas
- ✅ **Validação**: Aprovado pelo Architect sem regressões detectadas

### Dashboard de Assistência Técnica e Filtros por Loja do Técnico (2025-10-29)
- 🏪 **Campo loja_tecnico**: Técnicos agora têm uma loja específica atribuída
- 🔒 **Restrição por Loja**: Técnicos só visualizam e criam assistências para sua loja
- 📊 **Seções de Dashboard Expandidas**:
  - **Gráficos Comparativos**: Mantidos e funcionando (barras + donut)
  - **Assistência Técnica - Visão Geral**: 4 cards de estatísticas
  - **Assistência por Loja**: Tabela detalhada com:
    - Total de assistências por loja
    - Concluídas vs Em Andamento
    - Valor total por loja
    - Taxa de conclusão com barra de progresso visual
- 📈 **4 Cards de Estatísticas**:
  - Técnico com Mais Assistências
  - Loja com Mais Assistências  
  - Total de Assistências Concluídas e Em Andamento
  - Valor Total em Assistências
- 🎯 **APIs de Estatísticas**: 
  - `/api/assistencias/stats` para dados agregados
  - `/api/assistencias/por-loja` para detalhamento por loja
- 📊 **Gráficos Comparativos Mantidos**: Barras (vendas/clientes/conversão) + Donut (conversão)
- 🔐 **Banco de Dados**: Nova coluna `loja_tecnico` na tabela usuarios

### Módulo de Assistência Técnica (2025-10-29)
- 🔧 **Novo Cargo**: Criado o cargo "Técnico" com permissões específicas
- 📱 **4 Seções**:
  - **Chamados**: Visualização e gestão de assistências em andamento
  - **Nova Assistência**: Cadastro de novas assistências técnicas
  - **Estoque**: Gerenciamento de peças para reparo
  - **Histórico**: Registro de assistências concluídas
- 📊 **Funcionalidades**:
  - Controle de estoque de peças com quantidade e valores
  - Registro completo de assistências (cliente, aparelho, peça, valores)
  - Atualização automática de estoque ao concluir assistência
  - Busca e filtros por status, nome, CPF
  - Visualização detalhada de cada assistência
- 🔐 **Permissões**: Acessível por Técnico, Admin e Dev
- 💾 **Tabelas**: estoque_tecnico e assistencias
- 🎯 **APIs**: CRUD completo para estoque e assistências

### Exportação de Todos os Relatórios (2025-10-29)
- 📥 Adicionado dropdown no botão "Exportar Mês" na aba Consulta
- 📊 Nova opção: "Exportar TODOS os Relatórios" em Excel
- ✅ Mantém o mesmo formato e organização do Excel mensal
- 📁 Organiza por loja em abas separadas (como na exportação mensal)
- 🎯 Útil para backup completo ou análise histórica
- 📅 Nome do arquivo inclui a data de exportação

### Responsividade Completa (2025-10-29)
- 📱 **230+ linhas de CSS** adicionadas para responsividade avançada
- 📐 **3 breakpoints**: Tablet (768-991px), Mobile (até 767px), Mobile Pequeno (até 480px)
- 📊 **Tabelas**: Scroll horizontal automático em telas pequenas
- 🎨 **Cards**: Empilham verticalmente no mobile
- 📝 **Formulários**: Fontes e espaçamentos otimizados
- 🖼️ **Modais**: Quase tela cheia em mobile para melhor usabilidade
- 📉 **Gráficos**: Altura limitada para caber em telas pequenas
- 🔘 **Botões**: Tamanho e fontes adaptadas
- 🏷️ **Abas**: Scroll horizontal quando necessário
- ✅ Site totalmente utilizável em celulares e tablets

### Dashboard Aprimorado (2025-10-29)
- ✨ Adicionados 4 cards de visão geral (Total Lojas, Melhor Loja, Média Conversão, Total Vendas)
- 📊 Novo gráfico de barras comparativo (Vendas/Clientes/Conversão)
- 🍩 Novo gráfico donut mostrando distribuição de conversão
- 📊 Novo gráfico horizontal de ranking com cores dinâmicas
- 🎨 Todos os títulos agora incluem ícones Bootstrap
- 📐 Espaçamento e alinhamento completamente corrigidos
- 🏆 Top 3 do ranking agora mostra medalhas (🥇🥈🥉)
- 📱 Layout totalmente responsivo

### Setup Inicial (2025-10-29)
- Reorganizada estrutura de arquivos do GitHub para raiz do projeto
- Porta alterada de 3000 para 5000 (requisito do Replit)
- Servidor configurado para bind em 0.0.0.0
- Dependência `pg` (PostgreSQL) removida, mantido SQLite
- Adicionada dependência `sqlite3` ao package.json
- Workflow configurado e testado

## Como Usar
1. O servidor inicia automaticamente
2. Acesse a aplicação através do preview do Replit
3. Faça login com as credenciais admin/admin
4. Navegue pelas diferentes funcionalidades através do menu

## Integrações Futuras

### Integração com Bluve/Blu (Em Planejamento)
**Status**: Aguardando API Key do Bluve

**Funcionalidades Planejadas:**
1. **Sincronização Bidirecional de Lojas**
   - Lojas cadastradas aqui → Enviam para Bluve
   - Lojas do Bluve → Importam automaticamente

2. **Importação Automática de Relatórios**
   - Buscar vendas diárias do Bluve via API
   - Transformar em relatórios no formato do sistema
   - Popular dashboard automaticamente

3. **APIs do Bluve a serem utilizadas:**
   - API de Movimento de Vendas
   - API de Extrato
   - API de Conciliação (Débito e Crédito)

**Documentação**: https://integracao.useblu.com.br/varejo-apis

---

## Notas Importantes
- O banco de dados SQLite é criado automaticamente na pasta `data/`
- As tabelas são criadas automaticamente na primeira execução
- O usuário admin é criado automaticamente com senha "admin"
- Os relatórios gerados ficam em `public/relatorios_gerados/`
