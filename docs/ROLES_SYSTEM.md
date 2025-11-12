# Sistema de Controle de Acesso Baseado em Roles

## Visão Geral
O sistema implementa um controle de acesso granular baseado em 5 roles (cargos) diferentes, cada um com permissões e acessos específicos.

## Roles Disponíveis

### 1. Gerente
- **Descrição**: Gerente de uma única loja
- **Acesso**: Apenas aos dados da loja específica vinculada a ele
- **Permissões**:
  - ✅ Dashboard (dados filtrados pela sua loja)
  - ✅ Consultar relatórios (apenas da sua loja)
  - ✅ Demandas (visualizar todas, mas criar apenas para sua loja)
  - ❌ Novo Relatório
  - ❌ Gerenciar Lojas
  - ❌ Gerenciar Usuários
  - ❌ Logs do Sistema

**Configuração**: No cadastro de usuário, seleciona-se o role "Gerente" e vincula-se UMA loja específica.

### 2. Consultor
- **Descrição**: Consultor que gerencia múltiplas lojas
- **Acesso**: Aos dados de várias lojas vinculadas a ele
- **Permissões**:
  - ✅ Dashboard (dados filtrados pelas lojas vinculadas)
  - ✅ Consultar relatórios (apenas das lojas vinculadas)
  - ✅ Demandas (visualizar todas, criar para suas lojas)
  - ❌ Novo Relatório
  - ❌ Gerenciar Lojas
  - ❌ Gerenciar Usuários
  - ❌ Logs do Sistema

**Configuração**: No cadastro de usuário, seleciona-se o role "Consultor" e vincula-se MÚLTIPLAS lojas (Ctrl+clique).

### 3. Monitoramento
- **Descrição**: Equipe de monitoramento com acesso completo aos dados
- **Acesso**: Todos os dados de todas as lojas
- **Permissões**:
  - ✅ Dashboard (todas as lojas)
  - ✅ Consultar relatórios (todas as lojas)
  - ✅ Novo Relatório
  - ✅ Gerenciar Lojas
  - ✅ Demandas
  - ❌ Gerenciar Usuários
  - ❌ Logs do Sistema

**Configuração**: Apenas seleciona-se o role "Monitoramento", sem vínculo de lojas.

### 4. Administrador (Admin)
- **Descrição**: Administrador do sistema
- **Acesso**: Acesso total ao sistema
- **Permissões**:
  - ✅ Dashboard (todas as lojas)
  - ✅ Consultar relatórios (todas as lojas)
  - ✅ Novo Relatório
  - ✅ Gerenciar Lojas
  - ✅ Demandas
  - ✅ Gerenciar Usuários
  - ✅ Backup e Restauração
  - ❌ Logs do Sistema (exclusivo de Dev)

**Configuração**: Apenas seleciona-se o role "Administrador", sem vínculo de lojas.

### 5. Desenvolvedor (Dev)
- **Descrição**: Desenvolvedor com acesso total incluindo logs
- **Acesso**: Acesso total ao sistema + logs
- **Permissões**:
  - ✅ Dashboard (todas as lojas)
  - ✅ Consultar relatórios (todas as lojas)
  - ✅ Novo Relatório
  - ✅ Gerenciar Lojas
  - ✅ Demandas
  - ✅ Gerenciar Usuários
  - ✅ Backup e Restauração
  - ✅ **Logs do Sistema**

**Configuração**: Apenas seleciona-se o role "Desenvolvedor", sem vínculo de lojas.

## Arquitetura Técnica

### Backend (server.js)
1. **Middleware de Autorização** (`middleware/roleAuth.js`):
   - `requireRole([roles])`: Restringe acesso a rotas baseado no role
   - `requirePage([pages])`: Valida permissões para páginas específicas
   - `getLojaFilter(role, loja_gerente, lojas_consultor)`: Retorna filtros SQL baseados no role
   - `getPermissions(role)`: Retorna objeto com todas as permissões do role

2. **Estrutura do Banco de Dados**:
   - Tabela `usuarios`:
     - `role`: gerente, consultor, monitoramento, admin, dev
     - `loja_gerente`: nome da loja (para gerente)
     - `lojas_consultor`: lista de lojas separadas por vírgula (para consultor)
   - Tabela `logs`:
     - Registra ações do sistema (login, erros, acessos)
     - Visível apenas para role "dev"

3. **Filtros de Dados**:
   - APIs de dashboard, ranking, relatórios e lojas aplicam filtros automáticos
   - Gerente vê apenas dados da sua loja
   - Consultor vê apenas dados das lojas vinculadas
   - Outros roles veem todos os dados

### Frontend (app.js e páginas)
1. **Controle de Menu**:
   - Menu lateral mostra apenas itens permitidos para o role do usuário
   - Baseado no objeto `permissions` retornado pela API `/api/session-info`

2. **Interface de Gerenciamento de Usuários**:
   - Campo de role com dropdown
   - Campos condicionais para lojas:
     - Gerente: select simples (uma loja)
     - Consultor: select múltiplo (várias lojas)
   - Exibição de lojas vinculadas na tabela de usuários

3. **Página de Logs** (`views/logs.html`):
   - Exclusiva para role "dev"
   - Filtragem por tipo (error, warning, info, access)
   - Filtragem por data
   - Estatísticas de sistema (erros, warnings, usuários ativos, uptime)
   - Função de limpar logs

## Segurança

1. **Validação de Permissões**:
   - Todas as rotas validam permissões no backend
   - Frontend esconde elementos, mas backend sempre valida

2. **Isolamento de Dados**:
   - Filtros SQL aplicados automaticamente
   - Impossível acessar dados de outras lojas (para gerente/consultor)

3. **Registro de Ações**:
   - Sistema de logs registra acessos e ações importantes
   - Logs incluem: timestamp, tipo, usuário, ação, detalhes

## Fluxo de Uso

### Criando um Usuário Gerente:
1. Admin/Dev acessa "Usuários" → "Adicionar Usuário"
2. Preenche username e senha
3. Seleciona role "Gerente"
4. Campo "Loja do Gerente" aparece
5. Seleciona a loja específica
6. Salva

### Criando um Usuário Consultor:
1. Admin/Dev acessa "Usuários" → "Adicionar Usuário"
2. Preenche username e senha
3. Seleciona role "Consultor"
4. Campo "Lojas do Consultor" aparece
5. Seleciona múltiplas lojas (Ctrl+clique)
6. Salva

### Acessando Logs (apenas Dev):
1. Dev faz login
2. Vê item "Logs" no menu lateral
3. Acessa página de logs
4. Pode filtrar por tipo, data
5. Pode limpar logs se necessário

## Arquivos Modificados/Criados

### Novos Arquivos:
- `middleware/roleAuth.js` - Middleware de autorização
- `views/403.html` - Página de acesso negado
- `views/logs.html` - Página de logs do sistema
- `public/js/pages/logs.js` - JavaScript da página de logs
- `ROLES_SYSTEM.md` - Esta documentação

### Arquivos Modificados:
- `server.js` - Integração do middleware, rotas de logs, filtros de dados
- `views/index.html` - Menu com IDs e item de Logs
- `views/gerenciar-usuarios.html` - Campos de roles e lojas
- `public/js/app.js` - Controle de menu baseado em permissões
- `public/js/pages/gerenciar-usuarios.js` - Lógica de gerenciamento de roles e lojas

## Manutenção

### Adicionando um Novo Role:
1. Adicionar role em `middleware/roleAuth.js` no objeto `ROLES`
2. Definir permissões no `getPermissions()`
3. Atualizar dropdown em `views/gerenciar-usuarios.html`
4. Atualizar mapeamento de nomes em `public/js/app.js` e `gerenciar-usuarios.js`

### Modificando Permissões de um Role:
1. Editar função `getPermissions()` em `middleware/roleAuth.js`
2. Permissões são aplicadas automaticamente em todas as rotas

## Credenciais Padrão
- **Username**: admin
- **Password**: admin
- **Role**: admin
