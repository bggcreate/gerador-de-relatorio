import { showToast, getCsrfToken, getAuthHeaders } from '../utils.js';

export function initNovoRelatorioPage() {
    const form = document.getElementById('form-novo-relatorio');
    if (!form) return;

    // --- Seleção de Elementos ---
    const btnImportarPdf = document.getElementById("btn-importar-pdf");
    const pdfFileInput = document.getElementById("pdf-file-input");
    const btnSalvarTudo = document.getElementById("btn-salvar-tudo");
    const btnLimparFormulario = document.getElementById("btn-limpar-formulario");
    const btnAddVendedor = document.getElementById("btn-add-vendedor");
    const btnAddVendedorManual = document.getElementById("btn-add-vendedor-manual");
    const containerVendedores = document.getElementById("container-vendedores");
    const placeholderVendedores = document.getElementById("vendedores-placeholder");
    const lojaSelect = document.getElementById("loja");
    const dataInput = document.getElementById("data");
    const resultadosPdfContainer = document.getElementById('resultados-pdf-container');
    const totalVendasDinheiroInput = form.querySelector('[name="total_vendas_dinheiro"]');
    const ticketMedioInput = form.querySelector('[name="ticket_medio"]');
    const paInput = form.querySelector('[name="pa"]');
    const clientesMonitoramentoInput = document.getElementById('clientes_monitoramento_input');
    const vendasMonitoramentoInput = document.getElementById('vendas_monitoramento_input');
    const clientesLojaInput = document.getElementById('clientes_loja_input');
    const vendasLojaInput = document.getElementById('vendas_loja_input');
    const omniInput = document.getElementById('quantidade_omni_input');
    const monitoramentoDonutCanvas = document.getElementById('monitoramento-donut-chart');
    const lojaDonutCanvas = document.getElementById('loja-donut-chart');
    
    // ADICIONADO: Selecionar os novos inputs de pagamento
    const vendasCartaoInput = document.getElementById('vendas_cartao_input');
    const vendasPixInput = document.getElementById('vendas_pix_input');
    const vendasDinheiroInput = document.getElementById('vendas_dinheiro_input');
    const vendasTotalPagamentoInput = document.getElementById('vendas_total_pagamento_input');

    // --- Variáveis de Estado ---
    let lojasCache = [];
    let vendedoresCache = [];
    const urlParams = new URLSearchParams(window.location.search);
    const reportId = urlParams.get('edit');
    let monitoramentoDonutChart = null;
    let lojaDonutChart = null;
    const DRAFT_KEY = 'draftRelatorio';
    let vendedorCounter = 0;
    let previousLojaValue = null; // Track previous store to detect changes

    // --- Funções de Gerenciamento de Rascunho ---
    function salvarRascunho() {
        if (reportId) return;
        const formData = new FormData(form);
        const data = Object.fromEntries(formData.entries());
        const nomes = formData.getAll('vendedor_nome');
        const atendimentos = formData.getAll('vendedor_atendimentos');
        const vendas = formData.getAll('vendedor_vendas');
        data.vendedores = nomes.map((nome, index) => ({ nome: nome.trim(), atendimentos: atendimentos[index] || 0, vendas: vendas[index] || 0 }));
        data.pdfSectionVisible = resultadosPdfContainer.style.display === 'block';
        sessionStorage.setItem(DRAFT_KEY, JSON.stringify(data));
    }

    function carregarRascunho() {
        if (reportId) return;
        const draft = sessionStorage.getItem(DRAFT_KEY);
        if (!draft) return;
        const data = JSON.parse(draft);
        for (const key in data) {
            const input = form.querySelector(`[name="${key}"]`);
            if (input && key !== 'vendedores') input.value = data[key];
        }
        if (data.vendedores && Array.isArray(data.vendedores)) {
            containerVendedores.innerHTML = '';
            data.vendedores.forEach(vendedor => adicionarVendedor(vendedor));
        }
        if (data.pdfSectionVisible && resultadosPdfContainer) {
            resultadosPdfContainer.style.display = 'block';
        }
        updateVendedoresPlaceholder();
        handleSelecaoDeLoja();
        calcularEAtualizarGraficos();
        calcularTotalVendasPagamento(); // Adicionado para atualizar o total ao carregar rascunho
        showToast("Rascunho Carregado", "Seu relatório não salvo foi restaurado.", "info");
    }

    function limparRascunhoEFormulario() {
        form.reset();
        sessionStorage.removeItem(DRAFT_KEY);
        const hoje = new Date();
        const offset = hoje.getTimezoneOffset();
        dataInput.value = new Date(hoje.getTime() - (offset * 60 * 1000)).toISOString().split('T')[0];
        containerVendedores.innerHTML = '';
        if(resultadosPdfContainer) resultadosPdfContainer.style.display = 'none';
        updateVendedoresPlaceholder();
        handleSelecaoDeLoja();
        calcularEAtualizarGraficos();
        calcularTotalVendasPagamento(); // Adicionado para resetar o total ao limpar
        showToast("Formulário Limpo", "Todos os campos foram resetados.", "success");
    }

    // --- Funções de UI e Lógica ---
    const getCssVar = (varName) => getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
    
    function renderDonutChart(canvas, percentage, color) {
        if (!canvas) return null;
        const textElement = document.getElementById(canvas.id.replace('chart', 'text'));
        if (textElement) textElement.textContent = `${parseFloat(percentage).toFixed(1)}%`;
        const data = { datasets: [{ data: [percentage, 100 - percentage > 0 ? 100 - percentage : 0], backgroundColor: [color, '#e5e5e5'], borderColor: getCssVar('--content-bg'), borderWidth: 3, cutout: '75%' }] };
        const options = { responsive: true, maintainAspectRatio: true, plugins: { legend: { display: false }, tooltip: { enabled: false } } };
        const ctx = canvas.getContext('2d');
        if (canvas.chart) canvas.chart.destroy();
        canvas.chart = new Chart(ctx, { type: 'doughnut', data, options });
        return canvas.chart;
    }

    function updateDonutChart(chartInstance, percentage) {
        if (!chartInstance) return;
        const textElement = document.getElementById(chartInstance.canvas.id.replace('chart', 'text'));
        if (textElement) textElement.textContent = `${parseFloat(percentage).toFixed(1)}%`;
        chartInstance.data.datasets[0].data[0] = percentage;
        chartInstance.data.datasets[0].data[1] = 100 - percentage > 0 ? 100 - percentage : 0;
        chartInstance.update();
    }
    
    // ADICIONADO: Nova função para somar os totais de pagamento
    function calcularTotalVendasPagamento() {
        const cartao = Number(vendasCartaoInput.value) || 0;
        const pix = Number(vendasPixInput.value) || 0;
        const dinheiro = Number(vendasDinheiroInput.value) || 0;
        vendasTotalPagamentoInput.value = cartao + pix + dinheiro;
    }

    function calcularEAtualizarGraficos() {
        const clientesM = Number(clientesMonitoramentoInput.value) || 0;
        const vendasM = Number(vendasMonitoramentoInput.value) || 0;
        const omni = omniInput ? (Number(omniInput.value) || 0) : 0;
        const totalVendasM = vendasM + omni;
        const txMonitoramento = clientesM > 0 ? (totalVendasM / clientesM) * 100 : 0;
        updateDonutChart(monitoramentoDonutChart, txMonitoramento);
        
        const clientesL = Number(clientesLojaInput.value) || 0;
        const vendasL = Number(vendasLojaInput.value) || 0;
        const txLoja = clientesL > 0 ? (vendasL / clientesL) * 100 : 0;
        updateDonutChart(lojaDonutChart, txLoja);
    }

    function updateVendedoresPlaceholder() { if(placeholderVendedores) placeholderVendedores.style.display = containerVendedores.children.length === 0 ? "block" : "none"; };
    
    async function carregarLojas() {
        try {
            const response = await fetch("/api/lojas");
            if (!response.ok) throw new Error('Falha ao carregar lojas.');
            lojasCache = await response.json();
            lojaSelect.innerHTML = '<option value="" disabled selected>Selecione uma loja</option>';
            const lojasFiltradas = reportId ? lojasCache : lojasCache.filter(l => l.status === 'ativa');
            lojasFiltradas.forEach(l => lojaSelect.add(new Option(l.nome, l.nome)));
        } catch (e) { console.error("Erro ao carregar lojas", e); }
    }
    
    async function carregarVendedoresDaLoja() {
        const lojaSelecionada = lojasCache.find(loja => loja.nome === lojaSelect.value);
        if (!lojaSelecionada) {
            console.log('Nenhuma loja selecionada');
            vendedoresCache = [];
            return;
        }
        
        console.log(`Carregando vendedores para loja: ${lojaSelecionada.nome} (ID: ${lojaSelecionada.id})`);
        
        try {
            const response = await fetch(`/api/vendedores?loja_id=${lojaSelecionada.id}`);
            if (!response.ok) {
                console.warn('API de vendedores não disponível, usando modo manual');
                vendedoresCache = [];
                return;
            }
            const todosVendedores = await response.json();
            vendedoresCache = todosVendedores.filter(v => v.ativo === 1 || v.ativo === true);
            console.log(`Vendedores carregados: ${vendedoresCache.length} vendedores ativos`);
            console.log('Vendedores:', vendedoresCache);
            
            atualizarDropdownsVendedores();
        } catch (e) {
            console.error("Erro ao carregar vendedores", e);
            vendedoresCache = [];
        }
    }
    
    function atualizarDropdownsVendedores() {
        // Atualizar todos os selects de vendedores existentes com o novo cache
        const selectsExistentes = containerVendedores.querySelectorAll('.vendedor-select');
        console.log(`Atualizando ${selectsExistentes.length} dropdowns de vendedores com novo cache`);
        
        selectsExistentes.forEach(select => {
            const valorAtual = select.value; // Preservar seleção atual se possível
            
            // Reconstruir opções
            select.innerHTML = '<option value="">Selecione</option>';
            vendedoresCache.forEach(v => {
                const option = document.createElement('option');
                option.value = v.id;
                option.textContent = v.nome;
                if (v.id == valorAtual) {
                    option.selected = true;
                }
                select.appendChild(option);
            });
            
            console.log(`Dropdown atualizado. ${vendedoresCache.length} vendedores disponíveis`);
        });
    }
    
    async function handleSelecaoDeLoja() {
        const currentLojaValue = lojaSelect.value;
        console.log(`Loja selecionada: ${currentLojaValue}`);
        const lojaSelecionada = lojasCache.find(loja => loja.nome === currentLojaValue);
        const containerEspecial = document.getElementById('container-funcao-especial');
        const campoOmni = document.getElementById('campo-omni');
        const campoBuscaAssist = document.getElementById('campo-busca-assist');

        if(containerEspecial) containerEspecial.style.display = "none";
        if(campoOmni) campoOmni.style.display = "none";
        if(campoBuscaAssist) campoBuscaAssist.style.display = "none";

        if (!lojaSelecionada || !lojaSelecionada.funcao_especial) {
            calcularEAtualizarGraficos();
        } else {
            containerEspecial.style.display = "block";
            if (lojaSelecionada.funcao_especial === "Omni") campoOmni.style.display = "block";
            else if (lojaSelecionada.funcao_especial === "Busca por Assist. Tec.") campoBuscaAssist.style.display = "block";
            calcularEAtualizarGraficos();
        }
        
        // Detectar se a loja realmente mudou (usuário trocou manualmente)
        const lojaChanged = previousLojaValue !== null && previousLojaValue !== currentLojaValue;
        
        if (lojaChanged) {
            // IMPORTANTE: Limpar vendedores existentes APENAS quando o usuário trocar de loja
            // Isso previne limpar vendedores ao carregar rascunhos/edições
            containerVendedores.innerHTML = '';
            console.log('Loja alterada: vendedores limpos. Adicione novos vendedores para esta loja.');
        }
        
        // Atualizar loja anterior
        previousLojaValue = currentLojaValue;
        
        // Carregar vendedores da loja selecionada
        await carregarVendedoresDaLoja();
        
        // Atualizar placeholder de vendedores
        updateVendedoresPlaceholder();
    }
    
    function adicionarVendedor(vendedor = { nome: '', atendimentos: 0, vendas: 0, id_vendedor: null }, forcarManual = false) {
        if (!lojaSelect.value) {
            showToast("Atenção", "Selecione uma loja antes de adicionar vendedores.", "warning");
            return;
        }
        
        vendedorCounter++;
        const uniqueId = `vendedor-${vendedorCounter}`;
        
        const card = document.createElement("div");
        card.className = "vendedor-card";
        card.dataset.uniqueId = uniqueId;
        
        // Se forçar manual OU não tiver vendedores cadastrados, mostrar INPUT. Caso contrário, mostrar SELECT
        const temVendedoresCadastrados = vendedoresCache.length > 0;
        const mostrarInput = forcarManual || !temVendedoresCadastrados;
        
        let selectHtml = '';
        if (!mostrarInput && temVendedoresCadastrados) {
            selectHtml = '<select class="form-select vendedor-select" data-id="' + uniqueId + '">';
            selectHtml += '<option value="">Selecione</option>';
            vendedoresCache.forEach(v => {
                const selected = vendedor.id_vendedor === v.id ? 'selected' : '';
                selectHtml += `<option value="${v.id}" ${selected}>${v.nome}</option>`;
            });
            selectHtml += '</select>';
        }
        
        card.innerHTML = `
            <span class="vendedor-numero">#${vendedorCounter}</span>
            <div class="vendedor-field vendedor-nome">
                <label>Vendedor</label>
                ${mostrarInput ? 
                `<input type="text" class="form-control vendedor-nome-input" 
                    placeholder="Nome do vendedor" value="${vendedor.nome || ''}" required>` : selectHtml}
                <input type="hidden" class="vendedor-nome-hidden" name="vendedor_nome" value="${vendedor.nome || ''}">
                <input type="hidden" class="vendedor-id-input" name="vendedor_id" value="${vendedor.id_vendedor || ''}">
            </div>
            <div class="vendedor-field vendedor-atend">
                <label>Atend.</label>
                <input type="number" class="form-control vendedor-atendimentos-input" 
                    name="vendedor_atendimentos" value="${vendedor.atendimentos || 0}" min="0" required>
            </div>
            <div class="vendedor-field vendedor-vendas">
                <label>Vendas</label>
                <input type="number" class="form-control vendedor-vendas-input" 
                    name="vendedor_vendas" value="${vendedor.vendas || 0}" min="0" required>
            </div>
            <div class="vendedor-taxa-conversao">
                <span class="taxa-valor">0%</span>
            </div>
            <button type="button" class="btn-remove-vendedor">×</button>
        `;
        
        containerVendedores.appendChild(card);
        updateVendedoresPlaceholder();
        
        // Calcular taxa de conversão inicial
        calcularTaxaConversao(card);
        
        // Event listeners para este card
        const selectVendedor = card.querySelector('.vendedor-select');
        const nomeInput = card.querySelector('.vendedor-nome-input');
        const nomeHidden = card.querySelector('.vendedor-nome-hidden');
        const idInput = card.querySelector('.vendedor-id-input');
        const atendimentosInput = card.querySelector('.vendedor-atendimentos-input');
        const vendasInput = card.querySelector('.vendedor-vendas-input');
        
        // Se tem SELECT (vendedores cadastrados), adicionar listener
        if (selectVendedor) {
            selectVendedor.addEventListener('change', (e) => {
                const vendedorId = e.target.value;
                if (vendedorId) {
                    const vendedorSelecionado = vendedoresCache.find(v => v.id == vendedorId);
                    if (vendedorSelecionado) {
                        nomeHidden.value = vendedorSelecionado.nome;
                        idInput.value = vendedorSelecionado.id;
                    }
                } else {
                    nomeHidden.value = '';
                    idInput.value = '';
                }
                salvarRascunho();
            });
        }
        
        // Se tem INPUT manual, adicionar listener
        if (nomeInput) {
            nomeInput.addEventListener('input', (e) => {
                nomeHidden.value = e.target.value;
                salvarRascunho();
            });
        }
        
        atendimentosInput.addEventListener('input', () => {
            calcularTaxaConversao(card);
            salvarRascunho();
        });
        
        vendasInput.addEventListener('input', () => {
            calcularTaxaConversao(card);
            salvarRascunho();
        });
        
        // Botão de remover vendedor
        const btnRemover = card.querySelector('.btn-remove-vendedor');
        btnRemover.addEventListener('click', () => {
            card.remove();
            updateVendedoresPlaceholder();
            salvarRascunho();
        });
    }
    
    function calcularTaxaConversao(card) {
        const atendimentos = Number(card.querySelector('.vendedor-atendimentos-input').value) || 0;
        const vendas = Number(card.querySelector('.vendedor-vendas-input').value) || 0;
        const taxa = atendimentos > 0 ? (vendas / atendimentos) * 100 : 0;
        card.querySelector('.taxa-valor').textContent = `${taxa.toFixed(1)}%`;
    }
    
    async function carregarDadosParaEdicao() {
        showToast("Modo de Edição", "Carregando dados do relatório...", "info");
        await carregarLojas();
        try {
            const response = await fetch(`/api/relatorios/${reportId}`);
            if (!response.ok) throw new Error('Relatório não encontrado.');
            const { relatorio } = await response.json();

            if(resultadosPdfContainer && relatorio.total_vendas_dinheiro && parseFloat(String(relatorio.total_vendas_dinheiro).replace(/[R$\s.]/g, '').replace(',', '.')) > 0) {
                resultadosPdfContainer.style.display = 'block';
            }
            
            const vendedores = JSON.parse(relatorio.vendedores||'[]');
            for (const key in relatorio) { 
                const input = form.querySelector(`[name="${key}"]`); 
                if (input) input.value = relatorio[key]; 
            }
            containerVendedores.innerHTML = '';
            vendedores.forEach(vend => adicionarVendedor(vend));
            
            updateVendedoresPlaceholder();
            handleSelecaoDeLoja();
            btnSalvarTudo.textContent = 'SALVAR ALTERAÇÕES';
            calcularEAtualizarGraficos();
            calcularTotalVendasPagamento(); // Adicionado para calcular o total ao carregar para edição
        } catch(e) { showToast("Erro", "Não foi possível carregar os dados para edição.", "danger"); }
    }
    
    async function handleSalvarTudo() {
        if (!form.checkValidity()) { 
            form.reportValidity(); 
            showToast("Campos Inválidos", "Por favor, preencha todos os campos obrigatórios.", "danger");
            return;
        }
        
        const fd = new FormData(form);
        const data = Object.fromEntries(fd.entries());
        const n = fd.getAll('vendedor_nome'), a = fd.getAll('vendedor_atendimentos'), v = fd.getAll('vendedor_vendas');
        data.vendedores = JSON.stringify(n.map((nome, i) => ({ nome: nome.trim(), atendimentos: parseInt(a[i],10)||0, vendas: parseInt(v[i],10)||0 })).filter(vend => vend.nome));
        
        const method = reportId ? 'PUT' : 'POST';
        const url = reportId ? `/api/relatorios/${reportId}` : '/api/relatorios';
        
        btnSalvarTudo.disabled = true;
        btnSalvarTudo.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Salvando...';
        try {
            const response = await fetch(url, { method, headers: await getAuthHeaders(), body: JSON.stringify(data) });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error || 'Erro desconhecido ao salvar.');
            showToast('Sucesso!', reportId ? 'Relatório atualizado com sucesso!' : 'Relatório salvo com sucesso!', 'success');
            
            if (reportId) {
                setTimeout(() => window.location.href = '/consulta', 1000);
            } else {
                limparRascunhoEFormulario();
            }
        } catch (e) {
            showToast('Falha ao Salvar', e.message, 'danger');
        } finally {
            btnSalvarTudo.disabled = false;
            btnSalvarTudo.textContent = reportId ? 'SALVAR ALTERAÇÕES' : 'SALVAR RELATÓRIO COMPLETO';
        }
    }
    
    // --- Lógica de Importação de PDF ---
    btnImportarPdf.addEventListener('click', () => pdfFileInput.click());
    pdfFileInput.addEventListener('change', async (event) => {
        const file = event.target.files[0];
        if (!file) return;

        btnImportarPdf.disabled = true;
        btnImportarPdf.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processando...';

        try {
            const formData = new FormData();
            formData.append('pdfFile', file);
            const csrfToken = await getCsrfToken();
            const response = await fetch('/api/process-pdf', { 
                method: 'POST', 
                headers: { 'x-csrf-token': csrfToken },
                body: formData 
            });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error || 'Erro ao processar PDF.');
            
            const { data: extractedData } = result;
            
            // PREENCHIMENTO EXPLÍCITO CORRIGIDO
            if (extractedData.total_vendas_dinheiro) totalVendasDinheiroInput.value = extractedData.total_vendas_dinheiro;
            if (extractedData.ticket_medio) ticketMedioInput.value = extractedData.ticket_medio;
            if (extractedData.pa) paInput.value = extractedData.pa;
            if (extractedData.data) dataInput.value = extractedData.data;
            if (extractedData.loja) {
                const storeExists = Array.from(lojaSelect.options).some(option => option.text.trim().toUpperCase() === extractedData.loja.trim().toUpperCase());
                if (storeExists) {
                    lojaSelect.value = Array.from(lojaSelect.options).find(option => option.text.trim().toUpperCase() === extractedData.loja.trim().toUpperCase()).value;
                } else {
                    showToast("Atenção", `A loja "${extractedData.loja}" do PDF não foi encontrada no sistema.`, "danger");
                }
            }
            containerVendedores.innerHTML = '';
            if (extractedData.vendedores && extractedData.vendedores.length > 0) {
                extractedData.vendedores.forEach(vendedor => adicionarVendedor(vendedor));
            }
            
            if(resultadosPdfContainer) resultadosPdfContainer.style.display = 'block';
            
            // ATUALIZAÇÃO DA UI
            updateVendedoresPlaceholder();
            handleSelecaoDeLoja();
            calcularEAtualizarGraficos();
            salvarRascunho();
            showToast("Sucesso!", "Dados do PDF importados com sucesso.", "success");

        } catch (error) {
            showToast("Erro na Importação", error.message, "danger");
        } finally {
            btnImportarPdf.disabled = false;
            btnImportarPdf.innerHTML = '<i class="bi bi-file-earmark-arrow-up-fill me-2"></i>Importar de PDF';
            pdfFileInput.value = '';
        }
    });

    // --- Lógica dos Novos Botões de PDF (Ranking e Ticket Dia) ---
    const btnRankingDia = document.getElementById('btn-ranking-dia');
    const btnTicketDia = document.getElementById('btn-ticket-dia');
    const btnVerPdfsSalvos = document.getElementById('btn-ver-pdfs-salvos');
    const rankingPdfInput = document.getElementById('ranking-pdf-input');
    const ticketPdfInput = document.getElementById('ticket-pdf-input');
    
    // Botão Ranking Dia - Funciona igual ao botão PDF principal
    if (btnRankingDia) {
        btnRankingDia.addEventListener('click', () => rankingPdfInput.click());
    }
    
    if (rankingPdfInput) {
        rankingPdfInput.addEventListener('change', async (event) => {
            const file = event.target.files[0];
            if (!file) return;
            
            btnRankingDia.disabled = true;
            btnRankingDia.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processando...';
            
            try {
                const formData = new FormData();
                formData.append('pdf', file);
                
                const loja = lojaSelect.value;
                const data = dataInput.value;
                if (loja) formData.append('loja', loja);
                if (data) formData.append('data', data);
                
                const csrfToken = await getCsrfToken();
                const response = await fetch('/api/pdf/ranking', {
                    method: 'POST',
                    headers: { 'x-csrf-token': csrfToken },
                    body: formData
                });
                
                const result = await response.json();
                
                if (!response.ok) {
                    throw new Error(result.error || 'Erro ao processar PDF de Ranking');
                }
                
                // Aplicar dados automaticamente, igual ao botão PDF principal
                const extractedData = result.data;
                if (extractedData.pa) paInput.value = extractedData.pa;
                if (extractedData.loja) {
                    const storeExists = Array.from(lojaSelect.options).some(option => 
                        option.text.trim().toUpperCase() === extractedData.loja.trim().toUpperCase()
                    );
                    if (storeExists) {
                        lojaSelect.value = Array.from(lojaSelect.options).find(option => 
                            option.text.trim().toUpperCase() === extractedData.loja.trim().toUpperCase()
                        ).value;
                        await handleSelecaoDeLoja();
                    }
                }
                if (extractedData.data) dataInput.value = extractedData.data;
                
                salvarRascunho();
                showToast('Sucesso!', 'Dados do PDF de Ranking importados com sucesso.', 'success');
                
            } catch (error) {
                showToast('Erro', error.message, 'danger');
            } finally {
                btnRankingDia.disabled = false;
                btnRankingDia.innerHTML = '<i class="bi bi-trophy-fill me-2"></i>Ranking';
                rankingPdfInput.value = '';
            }
        });
    }
    
    // Botão Ticket Dia
    if (btnTicketDia) {
        btnTicketDia.addEventListener('click', () => {
            const loja = lojaSelect.value;
            const data = dataInput.value;
            
            if (!loja || !data) {
                showToast('Atenção', 'Por favor, selecione a loja e a data antes de salvar o PDF de Ticket.', 'warning');
                return;
            }
            
            ticketPdfInput.click();
        });
    }
    
    if (ticketPdfInput) {
        ticketPdfInput.addEventListener('change', async (event) => {
            const file = event.target.files[0];
            if (!file) return;
            
            const loja = lojaSelect.value;
            const data = dataInput.value;
            
            if (!loja || !data) {
                showToast('Atenção', 'Por favor, selecione a loja e a data antes de salvar o PDF.', 'warning');
                ticketPdfInput.value = '';
                return;
            }
            
            btnTicketDia.disabled = true;
            btnTicketDia.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Salvando...';
            
            try {
                const formData = new FormData();
                formData.append('pdf', file);
                formData.append('loja', loja);
                formData.append('data', data);
                
                const csrfToken = await getCsrfToken();
                const response = await fetch('/api/pdf/ticket', {
                    method: 'POST',
                    headers: { 'x-csrf-token': csrfToken },
                    body: formData
                });
                
                const result = await response.json();
                
                if (!response.ok) {
                    throw new Error(result.error || 'Erro ao salvar PDF de Ticket');
                }
                
                showToast('Sucesso!', `PDF de Ticket salvo: ${result.data.filename}`, 'success');
                
            } catch (error) {
                showToast('Erro', error.message, 'danger');
            } finally {
                btnTicketDia.disabled = false;
                btnTicketDia.innerHTML = '<i class="bi bi-receipt me-2"></i>Ticket';
                ticketPdfInput.value = '';
            }
        });
    }
    
    // Botão Ver PDFs Salvos
    if (btnVerPdfsSalvos) {
        btnVerPdfsSalvos.addEventListener('click', async () => {
            const loja = lojaSelect.value;
            const data = dataInput.value;
            
            btnVerPdfsSalvos.disabled = true;
            btnVerPdfsSalvos.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Carregando...';
            
            try {
                let url = '/api/pdf/tickets';
                const params = new URLSearchParams();
                if (loja) params.append('loja', loja);
                if (data) params.append('data', data);
                if (params.toString()) url += `?${params.toString()}`;
                
                const response = await fetch(url, {
                    headers: await getAuthHeaders()
                });
                
                const result = await response.json();
                
                if (!response.ok) {
                    throw new Error(result.error || 'Erro ao listar PDFs');
                }
                
                if (result.tickets.length === 0) {
                    showPdfMessage('Nenhum PDF de Ticket foi encontrado para os filtros selecionados.', 'info');
                } else {
                    let message = `<strong>PDFs de Ticket Salvos (${result.tickets.length}):</strong><ul class="mt-2 mb-0">`;
                    result.tickets.forEach(ticket => {
                        const dataFormatada = new Date(ticket.data).toLocaleDateString('pt-BR');
                        message += `<li><a href="/api/pdf/tickets/${ticket.id}/download" target="_blank">${ticket.filename}</a> - ${ticket.loja} - ${dataFormatada} (por ${ticket.uploaded_by})</li>`;
                    });
                    message += '</ul>';
                    showPdfMessage(message, 'info');
                }
                
            } catch (error) {
                showPdfMessage(error.message, 'danger');
                showToast('Erro', error.message, 'danger');
            } finally {
                btnVerPdfsSalvos.disabled = false;
                btnVerPdfsSalvos.innerHTML = '<i class="bi bi-folder2-open me-2"></i>PDFs Salvos';
            }
        });
    }

    // --- Inicialização dos Event Listeners ---
    btnAddVendedor.addEventListener("click", () => adicionarVendedor());
    btnAddVendedorManual.addEventListener("click", (e) => {
        e.preventDefault();
        adicionarVendedor({}, true); // true = forçar modo manual
    });
    btnSalvarTudo.addEventListener("click", handleSalvarTudo);
    btnLimparFormulario.addEventListener("click", limparRascunhoEFormulario);
    lojaSelect.addEventListener("change", handleSelecaoDeLoja);

    form.addEventListener('input', () => {
        calcularEAtualizarGraficos();
        salvarRascunho();
    });

    // ADICIONADO: Event listeners para os novos campos de pagamento
    if(vendasCartaoInput) vendasCartaoInput.addEventListener('input', calcularTotalVendasPagamento);
    if(vendasPixInput) vendasPixInput.addEventListener('input', calcularTotalVendasPagamento);
    if(vendasDinheiroInput) vendasDinheiroInput.addEventListener('input', calcularTotalVendasPagamento);

    // --- Lógica de Inicialização da Página ---
    monitoramentoDonutChart = renderDonutChart(monitoramentoDonutCanvas, 0, '#60a5fa');
    lojaDonutChart = renderDonutChart(lojaDonutCanvas, 0, '#4ade80');

    if (reportId) {
        document.querySelector('h4.mb-0').textContent = 'Editar Relatório Existente';
        carregarDadosParaEdicao();
    } else {
        carregarLojas();
        updateVendedoresPlaceholder();
        carregarRascunho();
    }
}