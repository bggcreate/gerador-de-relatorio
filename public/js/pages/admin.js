import { showToast } from '../utils.js';

// --- Variáveis de Estado Globais no Módulo ---
let dashboardLineChart = null;
let dashboardBarChart = null;
let dashboardDonutChart = null;
let lastRankingData = [];

// --- Funções Auxiliares ---
const getCssVar = (varName) => getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
const toISODateString = (date) => date.toISOString().split('T')[0];

function setLoadingState(isLoading) {
    const kpiElements = document.querySelectorAll('[id^="geral-"], [id^="loja-"], [id^="overview-"], [id^="assist-"]');
    const chartCanvases = ['dashboard-line-chart', 'dashboard-bar-chart', 'dashboard-donut-chart'];

    if (isLoading) {
        kpiElements.forEach(el => {
            if (el.tagName === 'H2' || el.tagName === 'H3' || el.tagName === 'H5') {
                el.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
            } else { el.innerHTML = ''; }
        });
        
        chartCanvases.forEach(canvasId => {
            const canvas = document.getElementById(canvasId);
            if (canvas) canvas.style.opacity = '0.5';
        });
    } else {
        chartCanvases.forEach(canvasId => {
            const canvas = document.getElementById(canvasId);
            if (canvas) canvas.style.opacity = '1';
        });
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

// --- Funções de Renderização de Gráficos ---

function renderBarChart(rankingData) {
    const ctx = document.getElementById('dashboard-bar-chart').getContext('2d');
    const metricSelect = document.getElementById('bar-chart-metric-select');
    const metric = metricSelect.value;

    let sortedData = [...rankingData];
    if (metric === 'vendas') {
        sortedData.sort((a, b) => b.total_vendas_loja - a.total_vendas_loja);
    } else if (metric === 'clientes') {
        sortedData.sort((a, b) => b.total_clientes_loja - a.total_clientes_loja);
    } else if (metric === 'conversao') {
        sortedData.sort((a, b) => parseFloat(b.tx_loja) - parseFloat(a.tx_loja));
    }
    
    const topLojas = sortedData.slice(0, 10);
    const labels = topLojas.map(l => l.loja.length > 20 ? l.loja.substring(0, 20) + '...' : l.loja);
    
    let data, label, backgroundColor;
    if (metric === 'vendas') {
        data = topLojas.map(l => l.total_vendas_loja);
        label = 'Vendas';
        backgroundColor = getCssVar('--accent-color');
    } else if (metric === 'clientes') {
        data = topLojas.map(l => l.total_clientes_loja);
        label = 'Clientes';
        backgroundColor = getCssVar('--color-success');
    } else {
        data = topLojas.map(l => parseFloat(l.tx_loja));
        label = 'Taxa de Conversão (%)';
        backgroundColor = '#4169E1';
    }

    if (dashboardBarChart) dashboardBarChart.destroy();
    dashboardBarChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: backgroundColor,
                borderRadius: 6,
                borderSkipped: false
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: true, position: 'top' },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) label += ': ';
                            if (metric === 'conversao') {
                                label += context.parsed.y.toFixed(2) + '%';
                            } else {
                                label += context.parsed.y.toLocaleString('pt-BR');
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            if (metric === 'conversao') {
                                return value.toFixed(1) + '%';
                            }
                            return value.toLocaleString('pt-BR');
                        }
                    }
                }
            }
        }
    });
}

function renderDonutChart(rankingData) {
    const ctx = document.getElementById('dashboard-donut-chart').getContext('2d');
    
    const topLojas = [...rankingData]
        .sort((a, b) => parseFloat(b.tx_loja) - parseFloat(a.tx_loja))
        .slice(0, 5);
    
    const labels = topLojas.map(l => l.loja.length > 15 ? l.loja.substring(0, 15) + '...' : l.loja);
    const data = topLojas.map(l => parseFloat(l.tx_loja));
    
    // Cores claras e vibrantes (diferentes do gráfico de linha)
    const colors = ['#f472b6', '#60a5fa', '#fbbf24', '#a78bfa', '#34d399'];

    if (dashboardDonutChart) dashboardDonutChart.destroy();
    dashboardDonutChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderWidth: 2,
                borderColor: getCssVar('--main-bg')
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: { size: 11 }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return context.label + ': ' + context.parsed.toFixed(2) + '%';
                        }
                    }
                }
            }
        }
    });
}

async function loadDailyAssistenciaStats(loja = 'todas') {
    try {
        const url = `/api/assistencias/stats-daily?loja=${encodeURIComponent(loja)}`;
        const response = await fetch(url);
        if (!response.ok) {
            console.error('Erro ao carregar estatísticas diárias de assistência técnica');
            setDefaultDailyValues();
            return;
        }
        const data = await response.json();
        
        const concluidasEl = document.getElementById('assist-concluidas-diarias');
        const faturamentoEl = document.getElementById('assist-faturamento-diario');
        const andamentoEl = document.getElementById('assist-em-andamento-diarias');
        
        if (concluidasEl) concluidasEl.textContent = data.concluidas_hoje || 0;
        if (faturamentoEl) {
            const faturamento = data.faturamento_hoje || 0;
            faturamentoEl.textContent = 'R$ ' + faturamento.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
        if (andamentoEl) andamentoEl.textContent = data.em_andamento || 0;
    } catch (error) {
        console.error('Erro ao carregar estatísticas diárias de assistência técnica:', error);
        setDefaultDailyValues();
    }
}

function setDefaultDailyValues() {
    const concluidasEl = document.getElementById('assist-concluidas-diarias');
    const faturamentoEl = document.getElementById('assist-faturamento-diario');
    const andamentoEl = document.getElementById('assist-em-andamento-diarias');
    
    if (concluidasEl) concluidasEl.textContent = '0';
    if (faturamentoEl) faturamentoEl.textContent = 'R$ 0,00';
    if (andamentoEl) andamentoEl.textContent = '0';
}

async function loadAssistenciaTickets(loja = 'todas') {
    const container = document.getElementById('assist-tickets-container');
    if (!container) return;
    
    try {
        const url = `/api/assistencias/tickets?loja=${encodeURIComponent(loja)}`;
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error('Erro ao carregar tickets');
        }
        const tickets = await response.json();
        
        if (tickets.length === 0) {
            container.innerHTML = `
                <div class="text-center text-muted py-5">
                    <i class="bi bi-check-circle fs-1"></i>
                    <p class="mb-0 mt-2">Nenhum ticket em andamento</p>
                </div>
            `;
            return;
        }
        
        container.innerHTML = tickets.map(ticket => {
            const statusColors = {
                'Em andamento': 'warning',
                'Aguardando peças': 'info',
                'Concluído': 'success'
            };
            const statusColor = statusColors[ticket.status] || 'secondary';
            const dataEntrada = new Date(ticket.data_entrada).toLocaleDateString('pt-BR');
            const valorTotal = (ticket.valor_peca_loja || 0) + (ticket.valor_servico_cliente || 0);
            
            return `
                <div class="card mb-3 border-start border-${statusColor} border-3">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start mb-2">
                            <div>
                                <h6 class="mb-1">
                                    <i class="bi bi-person me-1"></i>${ticket.cliente_nome}
                                </h6>
                                <small class="text-muted">CPF: ${ticket.cliente_cpf || '-'}</small>
                            </div>
                            <span class="badge bg-${statusColor}">${ticket.status}</span>
                        </div>
                        <div class="row g-2 small">
                            <div class="col-6">
                                <i class="bi bi-phone me-1"></i><strong>Aparelho:</strong> ${ticket.aparelho || '-'}
                            </div>
                            <div class="col-6">
                                <i class="bi bi-shop me-1"></i><strong>Loja:</strong> ${ticket.loja || '-'}
                            </div>
                            <div class="col-6">
                                <i class="bi bi-calendar me-1"></i><strong>Entrada:</strong> ${dataEntrada}
                            </div>
                            <div class="col-6">
                                <i class="bi bi-person-gear me-1"></i><strong>Técnico:</strong> ${ticket.tecnico_responsavel || '-'}
                            </div>
                            ${ticket.numero_pedido ? `
                            <div class="col-6">
                                <i class="bi bi-hash me-1"></i><strong>Pedido:</strong> ${ticket.numero_pedido}
                            </div>` : ''}
                            ${ticket.defeito_reclamado ? `
                            <div class="col-12">
                                <i class="bi bi-exclamation-circle me-1"></i><strong>Defeito:</strong> ${ticket.defeito_reclamado}
                            </div>` : ''}
                            <div class="col-12 mt-2">
                                <strong class="text-success">Valor Total: R$ ${valorTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</strong>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        }).join('');
    } catch (error) {
        console.error('Erro ao carregar tickets de assistência:', error);
        container.innerHTML = `
            <div class="text-center text-danger py-5">
                <i class="bi bi-exclamation-triangle"></i>
                <p class="mb-0 mt-2 small">Erro ao carregar tickets</p>
            </div>
        `;
    }
}

async function populateAssistenciaLojaFilter() {
    try {
        const response = await fetch('/api/lojas/ativas');
        if (!response.ok) return;
        const lojas = await response.json();
        
        const select = document.getElementById('filtro-loja-assistencia');
        if (!select) return;
        
        select.innerHTML = '<option value="todas" selected>Todas as Lojas</option>';
        lojas.forEach(loja => {
            const option = document.createElement('option');
            option.value = loja.nome;
            option.textContent = loja.nome;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Erro ao carregar lojas para filtro de assistência:', error);
    }
}

async function loadDemandas() {
    const container = document.getElementById('dashboard-demandas-container');
    if (!container) return;
    
    try {
        const response = await fetch('/api/demandas/pendentes');
        if (!response.ok) {
            throw new Error('Erro ao carregar demandas');
        }
        const demandas = await response.json();
        
        if (demandas.length === 0) {
            container.innerHTML = `
                <div class="text-center text-muted py-3">
                    <i class="bi bi-check-circle fs-1"></i>
                    <p class="mb-0 mt-2">Nenhuma demanda pendente</p>
                </div>
            `;
            return;
        }
        
        // Mostrar apenas as 5 primeiras demandas
        const demandasToShow = demandas.slice(0, 5);
        
        container.innerHTML = `
            <div class="list-group list-group-flush">
                ${demandasToShow.map(demanda => {
                    const tagColors = {
                        'urgente': 'danger',
                        'importante': 'warning',
                        'normal': 'info'
                    };
                    const tagColor = tagColors[demanda.tag] || 'secondary';
                    const dataFormatada = new Date(demanda.criado_em).toLocaleDateString('pt-BR');
                    
                    return `
                        <div class="list-group-item list-group-item-action p-3">
                            <div class="d-flex w-100 justify-content-between align-items-start">
                                <div class="flex-grow-1">
                                    <h6 class="mb-1">
                                        <i class="bi bi-shop me-1"></i>${demanda.loja_nome}
                                    </h6>
                                    <p class="mb-1 small">${demanda.descricao}</p>
                                    <small class="text-muted">
                                        <i class="bi bi-person me-1"></i>${demanda.criado_por_usuario} • ${dataFormatada}
                                    </small>
                                </div>
                                <span class="badge bg-${tagColor} ms-2">${demanda.tag}</span>
                            </div>
                        </div>
                    `;
                }).join('')}
            </div>
            ${demandas.length > 5 ? `
                <div class="text-center mt-2">
                    <small class="text-muted">+ ${demandas.length - 5} demandas pendentes</small>
                </div>
            ` : ''}
        `;
    } catch (error) {
        console.error('Erro ao carregar demandas:', error);
        container.innerHTML = `
            <div class="text-center text-danger py-3">
                <i class="bi bi-exclamation-triangle"></i>
                <p class="mb-0 mt-2 small">Erro ao carregar demandas</p>
            </div>
        `;
    }
}

async function loadAssistenciasPorLoja() {
    try {
        const response = await fetch('/api/assistencias/por-loja');
        if (!response.ok) {
            console.error('Erro ao carregar assistências por loja');
            return;
        }
        const data = await response.json();
        
        const tbody = document.getElementById('assist-por-loja-tbody');
        if (!tbody) return;
        
        if (data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center p-4">Nenhuma assistência técnica registrada</td></tr>';
            return;
        }
        
        tbody.innerHTML = data.map(loja => {
            const taxaConclusao = loja.total > 0 ? ((loja.concluidas / loja.total) * 100).toFixed(1) : '0.0';
            const valorFormatado = (loja.valor_total || 0).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            
            return `
                <tr>
                    <td class="ps-3"><strong>${loja.loja}</strong></td>
                    <td>${loja.total}</td>
                    <td class="text-success"><strong>${loja.concluidas || 0}</strong></td>
                    <td class="text-warning"><strong>${loja.em_andamento || 0}</strong></td>
                    <td>R$ ${valorFormatado}</td>
                    <td>
                        <div class="d-flex align-items-center gap-2">
                            <div class="progress" style="width: 100px; height: 20px;">
                                <div class="progress-bar bg-success" role="progressbar" style="width: ${taxaConclusao}%" 
                                     aria-valuenow="${taxaConclusao}" aria-valuemin="0" aria-valuemax="100">
                                </div>
                            </div>
                            <span class="small">${taxaConclusao}%</span>
                        </div>
                    </td>
                </tr>
            `;
        }).join('');
    } catch (error) {
        console.error('Erro ao carregar assistências por loja:', error);
    }
}

function renderLineChart(currentChartData, comparisonChartData, hideMonitData = false) {
    const ctx = document.getElementById('dashboard-line-chart').getContext('2d');
    
    const datasets = [
        {
            label: 'Tx. Conv. Loja (Atual)',
            data: currentChartData.txConversaoLoja,
            borderColor: '#4ade80',
            backgroundColor: 'rgba(74, 222, 128, 0.2)',
            fill: true, tension: 0.4, borderWidth: 3
        },
        {
            label: 'Tx. Conv. Loja (Comparado)',
            data: comparisonChartData.txConversaoLoja,
            borderColor: '#4ade80',
            borderDash: [5, 5], fill: false, tension: 0.4, borderWidth: 2
        }
    ];
    
    // Adicionar dados de monitoramento apenas se não for gerente
    if (!hideMonitData) {
        datasets.push(
            {
                label: 'Tx. Conv. Monit. (Atual)',
                data: currentChartData.txConversaoMonitoramento,
                borderColor: '#60a5fa',
                backgroundColor: 'rgba(96, 165, 250, 0.2)',
                fill: true, tension: 0.4, borderWidth: 3
            },
            {
                label: 'Tx. Conv. Monit. (Comparado)',
                data: comparisonChartData.txConversaoMonitoramento,
                borderColor: '#60a5fa',
                borderDash: [5, 5], fill: false, tension: 0.4, borderWidth: 2
            }
        );
    }
    
    if (dashboardLineChart) dashboardLineChart.destroy();
    dashboardLineChart = new Chart(ctx, {
        type: 'line',
        data: { labels: currentChartData.labels, datasets: datasets },
        options: {
            responsive: true, maintainAspectRatio: false,
            interaction: { mode: 'index', intersect: false },
            plugins: { 
                legend: { position: 'top' }, 
                tooltip: { position: 'nearest' } 
            },
            scales: { 
                y: { 
                    beginAtZero: true, 
                    ticks: { callback: (value) => value + '%' } 
                } 
            }
        }
    });
}


function updateOverviewCards(rankingData, currentData) {
    // Total de lojas ativas
    document.getElementById('overview-total-lojas').textContent = rankingData.length;
    
    // Melhor loja (por taxa de conversão)
    if (rankingData.length > 0) {
        const melhorLoja = [...rankingData].sort((a, b) => parseFloat(b.tx_loja) - parseFloat(a.tx_loja))[0];
        const nomeLoja = melhorLoja.loja.length > 25 ? melhorLoja.loja.substring(0, 25) + '...' : melhorLoja.loja;
        document.getElementById('overview-melhor-loja').textContent = `${nomeLoja} (${parseFloat(melhorLoja.tx_loja).toFixed(2)}%)`;
    } else {
        document.getElementById('overview-melhor-loja').textContent = '-';
    }
    
    // Média de conversão
    if (rankingData.length > 0) {
        const somaConversao = rankingData.reduce((acc, loja) => acc + parseFloat(loja.tx_loja), 0);
        const media = somaConversao / rankingData.length;
        document.getElementById('overview-media-conversao').textContent = `${media.toFixed(2)}%`;
    } else {
        document.getElementById('overview-media-conversao').textContent = '0.00%';
    }
    
    // Total de vendas
    const totalVendas = currentData.total_vendas_loja || 0;
    document.getElementById('overview-total-vendas').textContent = totalVendas.toLocaleString('pt-BR');
}

function updateUI(results, hideMonitData = false) {
    const [currentData, rankingData, currentChartData, comparisonData, comparisonChartData] = results;

    // Armazenar rankingData globalmente
    lastRankingData = rankingData;

    // Atualizar cards de visão geral
    updateOverviewCards(rankingData, currentData);

    // Atualizar métricas principais (apenas se não for gerente e dados existirem)
    if (!hideMonitData && currentData.total_clientes_monitoramento !== undefined) {
        document.getElementById('geral-clientes').textContent = (currentData.total_clientes_monitoramento || 0).toLocaleString('pt-BR');
        document.getElementById('geral-vendas').textContent = ((currentData.total_vendas_monitoramento || 0) + (currentData.total_omni || 0)).toLocaleString('pt-BR');
        document.getElementById('geral-tx-conversao').textContent = `${parseFloat(currentData.tx_conversao_monitoramento || 0).toFixed(2)}%`;
        document.getElementById('geral-clientes-comp').innerHTML = getComparisonHtml(currentData.total_clientes_monitoramento, comparisonData.total_clientes_monitoramento || 0);
        document.getElementById('geral-vendas-comp').innerHTML = getComparisonHtml((currentData.total_vendas_monitoramento || 0) + (currentData.total_omni || 0), (comparisonData.total_vendas_monitoramento || 0) + (comparisonData.total_omni || 0));
        document.getElementById('geral-tx-conversao-comp').innerHTML = getComparisonHtml(currentData.tx_conversao_monitoramento, comparisonData.tx_conversao_monitoramento || 0, '%');
    }
    
    // Atualizar métricas da loja
    const lojaClientes = document.getElementById('loja-clientes');
    const lojaVendas = document.getElementById('loja-vendas');
    const lojaTxConversao = document.getElementById('loja-tx-conversao');
    const lojaClientesComp = document.getElementById('loja-clientes-comp');
    const lojaVendasComp = document.getElementById('loja-vendas-comp');
    const lojaTxConversaoComp = document.getElementById('loja-tx-conversao-comp');
    
    if (lojaClientes) lojaClientes.textContent = (currentData.total_clientes_loja || 0).toLocaleString('pt-BR');
    if (lojaVendas) lojaVendas.textContent = (currentData.total_vendas_loja || 0).toLocaleString('pt-BR');
    if (lojaTxConversao) lojaTxConversao.textContent = `${parseFloat(currentData.tx_conversao_loja || 0).toFixed(2)}%`;
    if (lojaClientesComp) lojaClientesComp.innerHTML = getComparisonHtml(currentData.total_clientes_loja, comparisonData.total_clientes_loja);
    if (lojaVendasComp) lojaVendasComp.innerHTML = getComparisonHtml(currentData.total_vendas_loja, comparisonData.total_vendas_loja);
    if (lojaTxConversaoComp) lojaTxConversaoComp.innerHTML = getComparisonHtml(currentData.tx_conversao_loja, comparisonData.tx_conversao_loja, '%');

    // Renderizar gráficos
    renderLineChart(currentChartData, comparisonChartData, hideMonitData);
    renderBarChart(rankingData);
    renderDonutChart(rankingData);
    
    // Carregar estatísticas de assistência técnica (diárias)
    loadAssistenciasPorLoja();

    // Ativar tooltips
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
}

// --- Lógica Principal e Eventos ---
export function initAdminPage(currentUser) {
    const form = document.getElementById('form-filtros-dashboard');
    if (!form) return;

    const lojaSelect = document.getElementById('filtro-loja-dashboard');
    const dataInicioInput = document.getElementById('filtro-data-inicio-dashboard');
    const dataFimInput = document.getElementById('filtro-data-fim-dashboard');
    const quickPeriodButtons = document.querySelectorAll('[data-period]');
    const barChartMetricSelect = document.getElementById('bar-chart-metric-select');
    
    const allowedMonitoramentoRoles = ['admin', 'monitoramento', 'dev', 'consultor'];
    const canViewMonitoramento = currentUser && allowedMonitoramentoRoles.includes(currentUser.role);
    const isGerente = currentUser && currentUser.role === 'gerente';
    const isTecnico = currentUser && currentUser.role === 'tecnico';
    const isNotAdmin = isGerente || isTecnico;
    
    if (!canViewMonitoramento) {
        const comparativeSection = document.getElementById('comparative-charts-section');
        if (comparativeSection) comparativeSection.style.display = 'none';
        
        const monitoramentoCard = document.getElementById('monitoramento-card');
        if (monitoramentoCard) monitoramentoCard.style.display = 'none';
        
        const lojaCardCol = document.getElementById('loja-card-col');
        if (lojaCardCol) {
            lojaCardCol.classList.remove('col-xl-6');
            lojaCardCol.classList.add('col-xl-12');
        }
        
        console.log(`Dashboard adaptado para ${currentUser.role} - card Monitoramento oculto (visível apenas para admin, monitoramento, dev, consultor)`);
    }
    
    // Garantir que o card Bluve esteja sempre visível
    const lojaCardCol = document.getElementById('loja-card-col');
    if (lojaCardCol) {
        lojaCardCol.style.display = '';
        console.log('Card Bluve garantido como visível');
    }
    
    if (isGerente && lojaSelect) {
        const todasOption = lojaSelect.querySelector('option[value="todas"]');
        if (todasOption) todasOption.remove();
    }

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
        console.log('analisarDados() chamado');
        setLoadingState(true);

        const loja = lojaSelect.value;
        const dataInicio = dataInicioInput.value;
        const dataFim = dataFimInput.value;

        console.log('Dados do filtro:', { loja, dataInicio, dataFim });

        if (!dataInicio || !dataFim) {
            console.error('Datas não definidas!', { dataInicio, dataFim });
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
        
        // Sempre comparar com período anterior
        const diff = endDate.getTime() - startDate.getTime();
        const compEndDate = new Date(startDate.getTime() - 86400000);
        const compStartDate = new Date(compEndDate.getTime() - diff);
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
            
            // Verificar cada resposta individualmente
            for (let i = 0; i < responses.length; i++) {
                if (!responses[i].ok) {
                    console.error(`API ${i} falhou:`, responses[i].status, responses[i].statusText);
                    throw new Error(`Falha na API ${i}: ${responses[i].status} ${responses[i].statusText}`);
                }
            }
            
            const results = await Promise.all(responses.map(res => res.json()));

            updateUI(results, isNotAdmin);
        } catch (error) {
            console.error("Erro ao analisar dados:", error.message, error.stack);
            showToast("Erro", "Não foi possível carregar os dados do dashboard.", "danger");
        } finally {
            setLoadingState(false);
        }
    }
    
    // Event Listeners
    form.addEventListener('submit', analisarDados);
    
    quickPeriodButtons.forEach(button => {
        button.addEventListener('click', (e) => {
            quickPeriodButtons.forEach(btn => btn.classList.remove('active'));
            e.currentTarget.classList.add('active');
            setDateRange(e.currentTarget.dataset.period);
        });
    });

    if (barChartMetricSelect) {
        barChartMetricSelect.addEventListener('change', () => {
            if (lastRankingData.length > 0) {
                renderBarChart(lastRankingData);
            }
        });
    }

    // Event listener para botão de atualizar assistências por loja
    const btnRefreshAssistLoja = document.getElementById('btn-refresh-assist-loja');
    if (btnRefreshAssistLoja) {
        btnRefreshAssistLoja.addEventListener('click', () => {
            loadAssistenciasPorLoja();
            showToast('Atualizado', 'Dados de assistência por loja atualizados', 'success');
        });
    }

    // Event listeners para seção de assistência técnica diária
    const filtroLojaAssistencia = document.getElementById('filtro-loja-assistencia');
    if (filtroLojaAssistencia) {
        filtroLojaAssistencia.addEventListener('change', (e) => {
            const lojaSelecionada = e.target.value;
            loadDailyAssistenciaStats(lojaSelecionada);
            loadAssistenciaTickets(lojaSelecionada);
        });
    }

    const btnRefreshAssistTickets = document.getElementById('btn-refresh-assist-tickets');
    if (btnRefreshAssistTickets) {
        btnRefreshAssistTickets.addEventListener('click', () => {
            const lojaSelecionada = filtroLojaAssistencia ? filtroLojaAssistencia.value : 'todas';
            loadAssistenciaTickets(lojaSelecionada);
            showToast('Atualizado', 'Tickets de assistência atualizados', 'success');
        });
    }

    // Inicialização
    async function inicializar() {
        console.log('Inicializando dashboard...');
        await carregarLojas();
        console.log('Lojas carregadas');
        
        // Inicializar seção de assistência técnica
        await populateAssistenciaLojaFilter();
        loadDailyAssistenciaStats('todas');
        loadAssistenciaTickets('todas');
        
        loadDemandas(); // Carregar demandas pendentes
        console.log('Configurando período de 7 dias');
        setDateRange('7d');
        const initialActiveButton = document.querySelector('[data-period="7d"]');
        if(initialActiveButton) initialActiveButton.classList.add('active');
        console.log('Chamando analisarDados...');
        analisarDados();
    }
    
    console.log('Iniciando admin page...');
    inicializar();
}
