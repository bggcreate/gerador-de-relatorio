import { showToast } from '../utils.js';

// --- Variáveis de Estado Globais no Módulo ---
let dashboardLineChart = null;
let lastRankingData = [];

// --- Funções Auxiliares ---
const getCssVar = (varName) => getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
const toISODateString = (date) => date.toISOString().split('T')[0];

function setLoadingState(isLoading) {
    const kpiElements = document.querySelectorAll('[id^="geral-"], [id^="loja-"]');
    const chartCanvas = document.getElementById('dashboard-line-chart');
    const rankingBody = document.getElementById('ranking-corpo-tabela');

    if (isLoading) {
        kpiElements.forEach(el => {
            if (el.tagName === 'H2' || el.tagName === 'H3') {
                el.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
            } else { el.innerHTML = ''; }
        });
        if (rankingBody) rankingBody.innerHTML = '<tr><td colspan="6" class="text-center p-5"><div class="spinner-border" role="status"></div></td></tr>';
        if (dashboardLineChart) dashboardLineChart.destroy();
        if (chartCanvas) chartCanvas.style.opacity = '0.5';
    } else {
        if (chartCanvas) chartCanvas.style.opacity = '1';
    }
}

function getComparisonHtml(current, previous, unit = '') {
    const currentNum = parseFloat(current);
    const previousNum = parseFloat(previous);
    if (isNaN(currentNum) || isNaN(previousNum)) return '';

    if (previousNum === 0) return currentNum > 0 ? '<span class="text-success">▲ Novo</span>' : '';
    
    const diff = ((currentNum - previousNum) / previousNum) * 100;
    if (Math.abs(diff) < 0.1) return '';

    const icon = diff > 0 ? '▲' : '▼';
    const colorClass = diff > 0 ? 'text-success' : 'text-danger';
    
    const previousFormatted = unit === '%' ? previousNum.toFixed(2) : previousNum.toLocaleString('pt-BR');
    const tooltipTitle = `Valor anterior: ${previousFormatted}${unit}`;

    return `<span class="${colorClass}" data-bs-toggle="tooltip" data-bs-title="${tooltipTitle}">${icon} ${diff.toFixed(1)}%</span>`;
}

// --- Funções de Renderização ---
function renderRankingTable(rankingData) {
    const rankingBody = document.getElementById('ranking-corpo-tabela');
    const sortBy = document.getElementById('ranking-sort-select').value;
    
    const sortedData = [...rankingData].sort((a, b) => parseFloat(b[sortBy]) - parseFloat(a[sortBy]));

    if (sortedData.length > 0) {
        rankingBody.innerHTML = sortedData.map((loja, index) => `
            <tr>
                <td class="ps-3"><b>#${index + 1}</b></td>
                <td>${loja.loja}</td>
                <td><b>${parseFloat(loja.tx_loja).toFixed(2)}%</b></td>
                <td>${parseFloat(loja.tx_monitoramento).toFixed(2)}%</td>
                <td>${loja.total_vendas_loja.toLocaleString('pt-BR')}</td>
                <td>${loja.total_clientes_loja.toLocaleString('pt-BR')}</td>
            </tr>
        `).join('');
    } else {
        rankingBody.innerHTML = '<tr><td colspan="6" class="text-center p-4">Nenhuma loja ativa encontrada.</td></tr>';
    }
}

function renderLineChart(currentChartData, comparisonChartData) {
    const ctx = document.getElementById('dashboard-line-chart').getContext('2d');
    
    const datasets = [
        {
            label: 'Tx. Conv. Loja (Atual)',
            data: currentChartData.txConversaoLoja,
            borderColor: getCssVar('--color-success'),
            backgroundColor: getCssVar('--color-success') + '20',
            fill: true, tension: 0.4, borderWidth: 2.5
        },
        {
            label: 'Tx. Conv. Monit. (Atual)',
            data: currentChartData.txConversaoMonitoramento,
            borderColor: getCssVar('--accent-color'),
            backgroundColor: getCssVar('--accent-color') + '20',
            fill: true, tension: 0.4, borderWidth: 2.5
        },
        {
            label: 'Tx. Conv. Loja (Comparado)',
            data: comparisonChartData.txConversaoLoja,
            borderColor: getCssVar('--color-success'),
            borderDash: [5, 5], fill: false, tension: 0.4, borderWidth: 1.5
        },
        {
            label: 'Tx. Conv. Monit. (Comparado)',
            data: comparisonChartData.txConversaoMonitoramento,
            borderColor: getCssVar('--accent-color'),
            borderDash: [5, 5], fill: false, tension: 0.4, borderWidth: 1.5
        }
    ];
    
    if (dashboardLineChart) dashboardLineChart.destroy();
    dashboardLineChart = new Chart(ctx, {
        type: 'line',
        data: { labels: currentChartData.labels, datasets: datasets },
        options: {
            responsive: true, maintainAspectRatio: false,
            interaction: { mode: 'index', intersect: false },
            plugins: { legend: { position: 'top' }, tooltip: { position: 'nearest' } },
            scales: { y: { beginAtZero: true, ticks: { callback: (value) => value + '%' } } }
        }
    });
}

function updateUI(results) {
    const [currentData, rankingData, currentChartData, comparisonData, comparisonChartData] = results;

    document.getElementById('geral-clientes').textContent = currentData.total_clientes_monitoramento.toLocaleString('pt-BR');
    document.getElementById('geral-vendas').textContent = (currentData.total_vendas_monitoramento + currentData.total_omni).toLocaleString('pt-BR');
    document.getElementById('geral-tx-conversao').textContent = `${parseFloat(currentData.tx_conversao_monitoramento).toFixed(2)}%`;
    document.getElementById('loja-clientes').textContent = currentData.total_clientes_loja.toLocaleString('pt-BR');
    document.getElementById('loja-vendas').textContent = currentData.total_vendas_loja.toLocaleString('pt-BR');
    document.getElementById('loja-tx-conversao').textContent = `${parseFloat(currentData.tx_conversao_loja).toFixed(2)}%`;

    document.getElementById('geral-clientes-comp').innerHTML = getComparisonHtml(currentData.total_clientes_monitoramento, comparisonData.total_clientes_monitoramento);
    document.getElementById('geral-vendas-comp').innerHTML = getComparisonHtml(currentData.total_vendas_monitoramento + currentData.total_omni, comparisonData.total_vendas_monitoramento + comparisonData.total_omni);
    document.getElementById('geral-tx-conversao-comp').innerHTML = getComparisonHtml(currentData.tx_conversao_monitoramento, comparisonData.tx_conversao_monitoramento, '%');
    document.getElementById('loja-clientes-comp').innerHTML = getComparisonHtml(currentData.total_clientes_loja, comparisonData.total_clientes_loja);
    document.getElementById('loja-vendas-comp').innerHTML = getComparisonHtml(currentData.total_vendas_loja, comparisonData.total_vendas_loja);
    document.getElementById('loja-tx-conversao-comp').innerHTML = getComparisonHtml(currentData.tx_conversao_loja, comparisonData.tx_conversao_loja, '%');

    renderLineChart(currentChartData, comparisonChartData);
    
    lastRankingData = rankingData;
    renderRankingTable(lastRankingData);

    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
}

// --- Lógica Principal e Eventos ---
export function initAdminPage() {
    const form = document.getElementById('form-filtros-dashboard');
    if (!form) return;

    const lojaSelect = document.getElementById('filtro-loja-dashboard');
    const dataInicioInput = document.getElementById('filtro-data-inicio-dashboard');
    const dataFimInput = document.getElementById('filtro-data-fim-dashboard');
    const quickPeriodButtons = document.querySelectorAll('[data-period]');
    const comparisonTypeSelect = document.getElementById('comparison-type-select');
    const rankingSortSelect = document.getElementById('ranking-sort-select');

    async function carregarLojas() {
        try {
            const response = await fetch('/api/lojas?status=ativa');
            const lojas = await response.json();
            lojas.forEach(loja => lojaSelect.add(new Option(loja.nome, loja.nome)));
        } catch (error) { console.error('Falha ao carregar lojas', error); }
    }
    
    function setDateRange(period) {
        const hoje = new Date();
        let inicio, fim = new Date(hoje);
        switch (period) {
            case 'hoje': inicio = hoje; break;
            case '7d': inicio = new Date(); inicio.setDate(hoje.getDate() - 6); break;
            case 'mes-atual': inicio = new Date(hoje.getFullYear(), hoje.getMonth(), 1); break;
            case 'mes-passado':
                inicio = new Date(hoje.getFullYear(), hoje.getMonth() - 1, 1);
                fim = new Date(hoje.getFullYear(), hoje.getMonth(), 0);
                break;
        }
        dataInicioInput.value = toISODateString(inicio);
        dataFimInput.value = toISODateString(fim);
    }

    async function analisarDados(e) {
        if(e) e.preventDefault();
        setLoadingState(true);

        const loja = lojaSelect.value;
        const dataInicio = dataInicioInput.value;
        const dataFim = dataFimInput.value;

        if (!dataInicio || !dataFim) {
            showToast("Atenção", "Por favor, selecione um período de datas.", "danger");
            setLoadingState(false);
            return;
        }
        
        const baseParams = { data_inicio: dataInicio, data_fim: dataFim };
        if (loja !== 'todas') {
            baseParams.loja = loja;
        }

        const currentParams = new URLSearchParams(baseParams);

        const startDate = new Date(dataInicio + 'T00:00:00');
        const endDate = new Date(dataFim + 'T00:00:00');
        let compStartDate, compEndDate;
        if (comparisonTypeSelect.value === 'ano-anterior') {
            compStartDate = new Date(startDate); compStartDate.setFullYear(startDate.getFullYear() - 1);
            compEndDate = new Date(endDate); compEndDate.setFullYear(endDate.getFullYear() - 1);
        } else {
            const diff = endDate.getTime() - startDate.getTime();
            compEndDate = new Date(startDate.getTime() - 86400000);
            compStartDate = new Date(compEndDate.getTime() - diff);
        }
        const compBaseParams = {...baseParams, data_inicio: toISODateString(compStartDate), data_fim: toISODateString(compEndDate) };
        const comparisonParams = new URLSearchParams(compBaseParams);
        
        const apiCalls = [
            fetch(`/api/dashboard-data?${currentParams.toString()}`),
            fetch(`/api/ranking?${new URLSearchParams({ data_inicio: dataInicio, data_fim: dataFim })}`),
            fetch(`/api/dashboard/chart-data?${currentParams.toString()}`),
            fetch(`/api/dashboard-data?${comparisonParams.toString()}`),
            fetch(`/api/dashboard/chart-data?${comparisonParams.toString()}`)
        ];
        
        try {
            const responses = await Promise.all(apiCalls);
            const results = await Promise.all(responses.map(res => {
                if(!res.ok) throw new Error(`Falha na API: ${res.status} ${res.statusText}`);
                return res.json();
            }));

            updateUI(results);
        } catch (error) {
            console.error("Erro ao analisar dados:", error);
            showToast("Erro", "Não foi possível carregar os dados do dashboard.", "danger");
        } finally {
            setLoadingState(false);
        }
    }
    
    form.addEventListener('submit', analisarDados);
    
    quickPeriodButtons.forEach(button => {
        button.addEventListener('click', (e) => {
            quickPeriodButtons.forEach(btn => btn.classList.remove('active'));
            e.currentTarget.classList.add('active');
            setDateRange(e.currentTarget.dataset.period);
        });
    });

    if (rankingSortSelect) {
        rankingSortSelect.addEventListener('change', () => {
            if (lastRankingData.length > 0) {
                renderRankingTable(lastRankingData);
            }
        });
    }

    carregarLojas();
    setDateRange('7d');
    const initialActiveButton = document.querySelector('[data-period="7d"]');
    if(initialActiveButton) initialActiveButton.classList.add('active');
    analisarDados();
}