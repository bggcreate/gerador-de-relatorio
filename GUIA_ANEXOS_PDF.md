# Guia de Uso: Sistema de Anexos PDF

## Como Funciona

O sistema permite anexar PDFs (Ranking e Ticket do Dia) aos relatórios. Esses anexos ficam associados à **loja** e **data** do relatório, e podem ser visualizados na aba **Consulta**.

## Como Adicionar Anexos a um Relatório

### 1. Acesse a página "Novo Relatório"

Na página de novo relatório, você verá dois botões no topo:
- 📊 **Importar PDF** - Para fazer upload do PDF de Ranking
- 🧾 **Ticket do Dia** - Para fazer upload do PDF de Ticket

### 2. Selecione Loja e Data PRIMEIRO

**IMPORTANTE:** Antes de fazer upload de qualquer PDF, você precisa:
1. Selecionar a loja no dropdown
2. Selecionar a data no campo de data

Os PDFs serão associados a essa loja e data.

### 3. Faça Upload dos PDFs

#### Para PDF de Ranking:
1. Clique no botão **"Importar PDF"** (ícone 📊)
2. Selecione o arquivo PDF de ranking
3. O sistema irá:
   - Extrair automaticamente os dados (PA, Preço Médio, Atendimento Médio)
   - Validar se a loja e data do PDF correspondem
   - Salvar o arquivo no servidor
   - Registrar no banco de dados

#### Para PDF de Ticket:
1. Clique no botão do ticket (ícone 🧾)
2. Selecione o arquivo PDF do ticket do dia
3. O sistema irá salvar e associar à loja e data selecionadas

### 4. Visualizar Anexos na Aba Consulta

1. Vá para a aba **Consulta**
2. Busque o relatório que deseja visualizar
3. Clique no botão **"Visualizar"** (ícone de olho)
4. O modal será aberto com:
   - **Aba "Relatório"**: PDF do relatório principal
   - **Sidebar "ANEXOS"**: Lista todos os PDFs anexados (Rankings e Tickets)
   - **Abas dinâmicas**: Cada anexo cria uma aba para navegação

### 5. Navegar entre PDFs

- Clique em qualquer anexo na sidebar para abrir sua aba
- As abas aparecem no topo do modal:
  - 📄 Relatório (sempre visível)
  - ⚠️ Ranking (se houver PDF de ranking)
  - 🧾 Ticket (se houver PDF de ticket)
- Navegue entre as abas clicando nelas

## Exemplos Práticos

Foram criados PDFs de exemplo para você testar:

### Exemplo 1 - Relatório #212
**Seu relatório mais recente:**
- Loja: "119 QSQ AERO RJ"
- Data: 04/11/2025
- Anexos: 1 PDF de Ranking + 1 PDF de Ticket

**Como visualizar:**
1. Faça login (admin/admin)
2. Vá para "Consulta"
3. Busque pelo relatório #212 ou filtre por novembro/2025
4. Clique em "Visualizar" no relatório da loja "119 QSQ AERO RJ" de 04/11/2025
5. Na sidebar "ANEXOS", você verá os 2 PDFs
6. Clique neles para abrir em abas separadas

### Exemplo 2 - Relatório Antigo
- Loja: "020 QSQ ESTAÇÃO"
- Data: 06/08/2025  
- Anexos: 1 PDF de Ranking + 1 PDF de Ticket

**Como visualizar:** (mesmo processo do exemplo 1)

## Observações Importantes

- **Os anexos são associados por loja e data**: Todos os PDFs com a mesma loja e data aparecem juntos
- **Múltiplos PDFs permitidos**: Você pode ter vários PDFs de ranking ou ticket para a mesma loja/data
- **Validação automática**: O sistema valida se o PDF de ranking corresponde à loja e data selecionadas
- **Carregamento sob demanda**: Os PDFs só são carregados quando você clica na aba (mais rápido)

## Estrutura de Arquivos

Os PDFs são salvos em:
- Tickets: `data/pdfs/tickets/`
- Rankings: `data/pdfs/rankings/`

Os registros são salvos nas tabelas:
- `pdf_tickets` (loja, data, filename, filepath, uploaded_by, uploaded_at)
- `pdf_rankings` (loja, data, filename, filepath, pa, preco_medio, atendimento_medio, uploaded_by, uploaded_at)
