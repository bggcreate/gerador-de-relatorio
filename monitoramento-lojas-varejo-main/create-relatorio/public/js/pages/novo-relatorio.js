import { showToast } from '../utils.js';

export function initNovoRelatorioPage() {
    const form = document.getElementById('form-novo-relatorio');
    if (!form) return;

    // --- Seleção de Elementos ---
    const btnImportarPdf = document.getElementById("btn-importar-pdf");
    const pdfFileInput = document.getElementById("pdf-file-input");
    const btnSalvarTudo = document.getElementById("btn-salvar-tudo");
    const btnLimparFormulario = document.getElementById("btn-limpar-formulario");
    const btnAddVendedor = document.getElementById("btn-add-vendedor");
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
    const urlParams = new URLSearchParams(window.location.search);
    const reportId = urlParams.get('edit');
    let monitoramentoDonutChart = null;
    let lojaDonutChart = null;
    const DRAFT_KEY = 'draftRelatorio';

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
        const data = { datasets: [{ data: [percentage, 100 - percentage > 0 ? 100 - percentage : 0], backgroundColor: [color, '#333333'], borderColor: getCssVar('--content-bg'), borderWidth: 3, cutout: '75%' }] };
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
    
    function handleSelecaoDeLoja() {
        const lojaSelecionada = lojasCache.find(loja => loja.nome === lojaSelect.value);
        const containerEspecial = document.getElementById('container-funcao-especial');
        const campoOmni = document.getElementById('campo-omni');
        const campoBuscaAssist = document.getElementById('campo-busca-assist');

        if(containerEspecial) containerEspecial.style.display = "none";
        if(campoOmni) campoOmni.style.display = "none";
        if(campoBuscaAssist) campoBuscaAssist.style.display = "none";

        if (!lojaSelecionada || !lojaSelecionada.funcao_especial) {
            calcularEAtualizarGraficos();
            return;
        }
        containerEspecial.style.display = "block";
        if (lojaSelecionada.funcao_especial === "Omni") campoOmni.style.display = "block";
        else if (lojaSelecionada.funcao_especial === "Busca por Assist. Tec.") campoBuscaAssist.style.display = "block";
        
        calcularEAtualizarGraficos();
    }
    
    function adicionarVendedor(vendedor = { nome: '', atendimentos: 0, vendas: 0 }) {
        const div = document.createElement("div");
        div.className = "input-group input-group-sm mb-2";
        div.innerHTML = `<input type="text" class="form-control" name="vendedor_nome" placeholder="Nome do Vendedor" value="${vendedor.nome||''}" required><input type="number" class="form-control" name="vendedor_atendimentos" value="${vendedor.atendimentos||0}" min="0" title="Atendimentos"><input type="number" class="form-control" name="vendedor_vendas" value="${vendedor.vendas||0}" min="0" title="Vendas"><button type="button" class="btn btn-outline-danger" data-action="remover-vendedor"><i class="bi bi-trash"></i></button>`;
        containerVendedores.appendChild(div);
        updateVendedoresPlaceholder();
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
            const response = await fetch(url, { method, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
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
            const response = await fetch('/api/process-pdf', { method: 'POST', body: formData });
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

    // --- Inicialização dos Event Listeners ---
    btnAddVendedor.addEventListener("click", () => adicionarVendedor());
    btnSalvarTudo.addEventListener("click", handleSalvarTudo);
    btnLimparFormulario.addEventListener("click", limparRascunhoEFormulario);
    lojaSelect.addEventListener("change", handleSelecaoDeLoja);

    form.addEventListener('input', () => {
        calcularEAtualizarGraficos();
        salvarRascunho();
    });

    // ADICIONADO: Event listeners para os novos campos de pagamento
    vendasCartaoInput.addEventListener('input', calcularTotalVendasPagamento);
    vendasPixInput.addEventListener('input', calcularTotalVendasPagamento);
    vendasDinheiroInput.addEventListener('input', calcularTotalVendasPagamento);

    containerVendedores.addEventListener("click", e => {
        if (e.target.closest('button[data-action="remover-vendedor"]')) {
            e.target.closest(".input-group").remove();
            updateVendedoresPlaceholder();
            salvarRascunho();
        }
    });

    // --- Lógica de Inicialização da Página ---
    monitoramentoDonutChart = renderDonutChart(monitoramentoDonutCanvas, 0, getCssVar('--accent-color'));
    lojaDonutChart = renderDonutChart(lojaDonutCanvas, 0, getCssVar('--color-success'));

    if (reportId) {
        document.querySelector('h4.mb-0').textContent = 'Editar Relatório Existente';
        carregarDadosParaEdicao();
    } else {
        carregarLojas();
        updateVendedoresPlaceholder();
        carregarRascunho();
    }
}