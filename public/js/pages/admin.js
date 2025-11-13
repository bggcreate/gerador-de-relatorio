import { showToast } from '../utils.js';

// --- Variáveis de Estado Globais no Módulo ---
let dashboardLineChart = null;
let dashboardBarChart = null;
let dashboardDonutChart = null;
let lastRankingData = [];
let storeSalesChart = null;
let storeTicketChart = null;
let storePaChart = null;
let paymentDistributionChart = null;

// --- Funções Auxiliares ---
const getCssVar = (varName) => getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
const toISODateString = (date) => date.toISOString().split('T')[0];

function setLoadingState(isLoading) {
    const kpiElements = document.querySelectorAll('[id^="geral-"], [id^="loja-"], [id^="overview-"]');
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

async function loadStorePerformance(dataInicio, dataFim) {
    try {
        const params = new URLSearchParams();
        if (dataInicio) params.append('data_inicio', dataInicio);
        if (dataFim) params.append('data_fim', dataFim);
        
        const response = await fetch(`/api/dashboard/store-performance?${params.toString()}`);
        if (!response.ok) {
            console.error('Erro ao carregar desempenho das lojas');
            return;
        }
        const data = await response.json();
        
        renderStoreSalesChart(data);
        renderStoreTicketChart(data);
        renderStorePaChart(data);
        renderPaymentDistributionChart(data);
    } catch (error) {
        console.error('Erro ao carregar desempenho das lojas:', error);
    }
}

function renderStoreSalesChart(data) {
    const ctx = document.getElementById('store-sales-chart')?.getContext('2d');
    if (!ctx) return;
    
    const topStores = data.slice(0, 10);
    const labels = topStores.map(s => s.loja.length > 20 ? s.loja.substring(0, 20) + '...' : s.loja);
    const values = topStores.map(s => s.vendas_media_dia);
    
    if (storeSalesChart) storeSalesChart.destroy();
    storeSalesChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Vendas Médias por Dia',
                data: values,
                backgroundColor: getCssVar('--accent-color'),
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
                            const index = context.dataIndex;
                            const store = topStores[index];
                            return [
                                'Média/dia: ' + store.vendas_media_dia.toLocaleString('pt-BR', { minimumFractionDigits: 2 }),
                                'Total: ' + store.total_vendas.toLocaleString('pt-BR'),
                                'Relatórios: ' + store.dias_registrados
                            ];
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return value.toLocaleString('pt-BR');
                        }
                    }
                }
            }
        }
    });
}

function renderStoreTicketChart(data) {
    const ctx = document.getElementById('store-ticket-chart')?.getContext('2d');
    if (!ctx) return;
    
    const topStores = [...data].sort((a, b) => b.ticket_medio - a.ticket_medio).slice(0, 10);
    const labels = topStores.map(s => s.loja.length > 20 ? s.loja.substring(0, 20) + '...' : s.loja);
    const values = topStores.map(s => s.ticket_medio);
    
    if (storeTicketChart) storeTicketChart.destroy();
    storeTicketChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Ticket Médio (R$)',
                data: values,
                backgroundColor: '#10b981',
                borderRadius: 6,
                borderSkipped: false
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: true, position: 'top' },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const index = context.dataIndex;
                            const store = topStores[index];
                            return [
                                'Ticket Médio: R$ ' + store.ticket_medio.toFixed(2),
                                'Baseado em ' + store.dias_registrados + ' relatório(s)'
                            ];
                        }
                    }
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return 'R$ ' + value.toFixed(2);
                        }
                    }
                }
            }
        }
    });
}

function renderStorePaChart(data) {
    const ctx = document.getElementById('store-pa-chart')?.getContext('2d');
    if (!ctx) return;
    
    const topStores = [...data].sort((a, b) => b.pa - a.pa).slice(0, 10);
    const labels = topStores.map(s => s.loja.length > 20 ? s.loja.substring(0, 20) + '...' : s.loja);
    const values = topStores.map(s => s.pa);
    
    if (storePaChart) storePaChart.destroy();
    storePaChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Peças por Venda (PA)',
                data: values,
                backgroundColor: '#f59e0b',
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
                            const index = context.dataIndex;
                            const store = topStores[index];
                            return [
                                'PA: ' + store.pa.toFixed(2),
                                'Baseado em ' + store.dias_registrados + ' relatório(s)'
                            ];
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return value.toFixed(1);
                        }
                    }
                }
            }
        }
    });
}

function renderPaymentDistributionChart(data) {
    const ctx = document.getElementById('payment-distribution-chart')?.getContext('2d');
    if (!ctx) return;
    
    const totalCartao = data.reduce((sum, s) => sum + s.formas_pagamento.cartao, 0);
    const totalPix = data.reduce((sum, s) => sum + s.formas_pagamento.pix, 0);
    const totalDinheiro = data.reduce((sum, s) => sum + s.formas_pagamento.dinheiro, 0);
    
    if (paymentDistributionChart) paymentDistributionChart.destroy();
    paymentDistributionChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Cartão', 'PIX', 'Dinheiro'],
            datasets: [{
                data: [totalCartao, totalPix, totalDinheiro],
                backgroundColor: ['#3b82f6', '#10b981', '#f59e0b'],
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
                        font: { size: 12 }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const total = totalCartao + totalPix + totalDinheiro;
                            const percentage = total > 0 ? ((context.parsed / total) * 100).toFixed(1) : 0;
                            return context.label + ': ' + context.parsed.toLocaleString('pt-BR') + ' (' + percentage + '%)';
                        }
                    }
                }
            }
        }
    });
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

// Função para popular os dropdowns de loja nos cards Monitoramento e Bluve
async function populateStoreDropdowns() {
    try {
        const response = await fetch('/api/lojas');
        const lojas = await response.json();
        
        const filtroMonitoramento = document.getElementById('filtro-loja-monitoramento');
        const filtroBluve = document.getElementById('filtro-loja-bluve');
        
        if (filtroMonitoramento && filtroBluve) {
            // Limpar opções existentes (exceto "Geral")
            filtroMonitoramento.innerHTML = '<option value="">Geral</option>';
            filtroBluve.innerHTML = '<option value="">Geral</option>';
            
            // Adicionar lojas aos dropdowns
            lojas.forEach(loja => {
                const optionMonit = document.createElement('option');
                optionMonit.value = loja.nome;
                optionMonit.textContent = loja.nome;
                filtroMonitoramento.appendChild(optionMonit);
                
                const optionBluve = document.createElement('option');
                optionBluve.value = loja.nome;
                optionBluve.textContent = loja.nome;
                filtroBluve.appendChild(optionBluve);
            });
        }
    } catch (error) {
        console.error('Erro ao carregar lojas para dropdowns:', error);
    }
}

// Função para atualizar apenas o card Monitoramento
async function updateMonitoramentoCard(loja = '') {
    try {
        // Obter a loja selecionada no dropdown Bluve para manter seus dados
        const filtroBluve = document.getElementById('filtro-loja-bluve');
        const lojaBluveAtual = filtroBluve ? filtroBluve.value : '';
        
        const params = new URLSearchParams();
        if (loja) params.append('loja_monitoramento', loja);
        if (lojaBluveAtual) params.append('loja_bluve', lojaBluveAtual);
        
        const url = `/api/dashboard/metrics${params.toString() ? '?' + params.toString() : ''}`;
        const response = await fetch(url);
        const data = await response.json();
        
        // Atualizar card Monitoramento
        document.getElementById('monitoramento-clientes').textContent = (data.monitoramento.clientes || 0).toLocaleString('pt-BR');
        document.getElementById('monitoramento-vendas').textContent = (data.monitoramento.vendas || 0).toLocaleString('pt-BR');
        document.getElementById('monitoramento-tx-conversao').textContent = `${data.monitoramento.tx_conversao}%`;
    } catch (error) {
        console.error('Erro ao carregar métricas de Monitoramento:', error);
    }
}

// Função para atualizar apenas o card Bluve
async function updateBluveCard(loja = '') {
    try {
        // Obter a loja selecionada no dropdown Monitoramento para manter seus dados
        const filtroMonitoramento = document.getElementById('filtro-loja-monitoramento');
        const lojaMonitoramentoAtual = filtroMonitoramento ? filtroMonitoramento.value : '';
        
        const params = new URLSearchParams();
        if (lojaMonitoramentoAtual) params.append('loja_monitoramento', lojaMonitoramentoAtual);
        if (loja) params.append('loja_bluve', loja);
        
        const url = `/api/dashboard/metrics${params.toString() ? '?' + params.toString() : ''}`;
        const response = await fetch(url);
        const data = await response.json();
        
        // Atualizar card Bluve
        document.getElementById('bluve-clientes').textContent = (data.bluve.clientes || 0).toLocaleString('pt-BR');
        document.getElementById('bluve-vendas').textContent = (data.bluve.vendas || 0).toLocaleString('pt-BR');
        document.getElementById('bluve-tx-conversao').textContent = `${data.bluve.tx_conversao}%`;
    } catch (error) {
        console.error('Erro ao carregar métricas de Bluve:', error);
    }
}

// Inicializar dropdowns e métricas
function initMetricsCards() {
    // Popular dropdowns
    populateStoreDropdowns();
    populateOmniStoresDropdown();
    populateAssistenciasStoresDropdown();
    
    // Carregar métricas gerais inicialmente para ambos os cards
    updateMonitoramentoCard();
    updateBluveCard();
    
    // Adicionar event listeners aos dropdowns (independentes)
    const filtroMonitoramento = document.getElementById('filtro-loja-monitoramento');
    const filtroBluve = document.getElementById('filtro-loja-bluve');
    const filtroOmni = document.getElementById('filtro-loja-omni');
    
    if (filtroMonitoramento) {
        filtroMonitoramento.addEventListener('change', (e) => {
            updateMonitoramentoCard(e.target.value);
        });
    }
    
    if (filtroBluve) {
        filtroBluve.addEventListener('change', (e) => {
            updateBluveCard(e.target.value);
        });
    }
    
    if (filtroOmni) {
        filtroOmni.addEventListener('change', () => {
            updateOmniCard();
        });
    }
    
    const filtroAssistencias = document.getElementById('filtro-loja-assistencias');
    if (filtroAssistencias) {
        filtroAssistencias.addEventListener('change', () => {
            updateAssistenciasCard();
        });
    }
}

// Popular dropdown de lojas com função Omni
async function populateOmniStoresDropdown() {
    try {
        const response = await fetch('/api/lojas');
        const lojas = await response.json();
        
        const filtroOmni = document.getElementById('filtro-loja-omni');
        
        if (filtroOmni) {
            filtroOmni.innerHTML = '<option value="">Geral</option>';
            
            lojas.filter(loja => loja.funcao_especial && loja.funcao_especial.toLowerCase() === 'omni')
                .forEach(loja => {
                    const option = document.createElement('option');
                    option.value = loja.nome;
                    option.textContent = loja.nome;
                    filtroOmni.appendChild(option);
                });
        }
    } catch (error) {
        console.error('Erro ao carregar lojas Omni:', error);
    }
}

// Popular dropdown de lojas com função Assistência Técnica
async function populateAssistenciasStoresDropdown() {
    try {
        const response = await fetch('/api/lojas');
        const lojas = await response.json();
        
        const filtroAssistencias = document.getElementById('filtro-loja-assistencias');
        
        if (filtroAssistencias) {
            filtroAssistencias.innerHTML = '<option value="">Geral</option>';
            
            lojas.filter(loja => loja.funcao_especial && loja.funcao_especial.toLowerCase().includes('assist'))
                .forEach(loja => {
                    const option = document.createElement('option');
                    option.value = loja.nome;
                    option.textContent = loja.nome;
                    filtroAssistencias.appendChild(option);
                });
        }
    } catch (error) {
        console.error('Erro ao carregar lojas de Assistências:', error);
    }
}

// Atualizar card de Assistências de forma independente (sempre mostra dados de HOJE)
async function updateAssistenciasCard() {
    try {
        const hoje = toISODateString(new Date());
        const params = new URLSearchParams();
        params.append('data_inicio', hoje);
        params.append('data_fim', hoje);
        
        const lojaAssistencias = document.getElementById('filtro-loja-assistencias')?.value;
        if (lojaAssistencias) params.append('loja', lojaAssistencias);
        
        const response = await fetch(`/api/assistencias?${params.toString()}`);
        
        let assistenciasTotal = 0;
        if (response.ok) {
            const assistencias = await response.json();
            assistenciasTotal = Array.isArray(assistencias) ? assistencias.length : 0;
        }
        
        document.getElementById('card-assistencias-total').textContent = assistenciasTotal.toLocaleString('pt-BR');
    } catch (error) {
        console.error('Erro ao carregar card de Assistências:', error);
        document.getElementById('card-assistencias-total').textContent = '0';
    }
}

// Atualizar card de Omni de forma independente (sempre mostra dados de HOJE)
async function updateOmniCard() {
    try {
        const hoje = toISODateString(new Date());
        const params = new URLSearchParams();
        params.append('data_inicio', hoje);
        params.append('data_fim', hoje);
        
        const lojaOmni = document.getElementById('filtro-loja-omni')?.value;
        if (lojaOmni) params.append('loja', lojaOmni);
        
        const response = await fetch(`/api/dashboard-data?${params.toString()}`);
        
        let omniTotal = 0;
        if (response.ok) {
            const omniData = await response.json();
            omniTotal = omniData.total_omni || 0;
        }
        
        document.getElementById('card-omni-total').textContent = omniTotal.toLocaleString('pt-BR');
    } catch (error) {
        console.error('Erro ao carregar card de Omni:', error);
        document.getElementById('card-omni-total').textContent = '0';
    }
}

// Atualizar cards estáticos (Relatórios Hoje e Última Loja)
async function updateStaticCards() {
    try {
        const hoje = toISODateString(new Date());
        
        const [ultimaLojaResp, relatoriosHojeResp] = await Promise.all([
            fetch('/api/relatorios/ultima-loja'),
            fetch(`/api/relatorios?data_inicio=${hoje}&data_fim=${hoje}`)
        ]);
        
        if (ultimaLojaResp.ok) {
            const ultimaLoja = await ultimaLojaResp.json();
            if (ultimaLoja && ultimaLoja.loja) {
                const nomeLoja = ultimaLoja.loja.length > 20 ? ultimaLoja.loja.substring(0, 20) + '...' : ultimaLoja.loja;
                document.getElementById('card-ultima-loja').textContent = nomeLoja;
                const dataFormatada = new Date(ultimaLoja.enviado_em).toLocaleDateString('pt-BR');
                document.getElementById('card-ultima-loja-data').textContent = `Enviado em ${dataFormatada}`;
            } else {
                document.getElementById('card-ultima-loja').textContent = '-';
                document.getElementById('card-ultima-loja-data').textContent = 'Nenhum relatório';
            }
        } else {
            document.getElementById('card-ultima-loja').textContent = '-';
            document.getElementById('card-ultima-loja-data').textContent = 'Erro ao carregar';
        }
        
        let relatoriosHojeTotal = 0;
        if (relatoriosHojeResp.ok) {
            const relatoriosHoje = await relatoriosHojeResp.json();
            relatoriosHojeTotal = relatoriosHoje.total || 0;
        }
        
        document.getElementById('card-relatorios-hoje').textContent = relatoriosHojeTotal.toLocaleString('pt-BR');
    } catch (error) {
        console.error('Erro ao carregar cards estáticos:', error);
        document.getElementById('card-ultima-loja').textContent = 'Erro';
        document.getElementById('card-ultima-loja-data').textContent = 'Erro ao carregar';
        document.getElementById('card-relatorios-hoje').textContent = '0';
    }
}

// Atualizar todos os cards de indicadores de performance
async function updatePerformanceCards(dataInicio = null, dataFim = null) {
    await Promise.all([
        updateAssistenciasCard(),
        updateOmniCard(),
        updateStaticCards()
    ]);
}

function updateUI(results, hideMonitData = false, dataInicio = null, dataFim = null) {
    const [currentData, rankingData, currentChartData, comparisonData, comparisonChartData] = results;

    // Armazenar rankingData globalmente
    lastRankingData = rankingData;

    // Atualizar cards de visão geral
    updateOverviewCards(rankingData, currentData);
    
    // Atualizar cards de indicadores de performance
    updatePerformanceCards(dataInicio, dataFim);

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
    
    // Controlar visibilidade do card de Monitoramento
    const monitoramentoCard = document.getElementById('monitoramento-card');
    const lojaCardCol = document.getElementById('loja-card-col');
    
    if (!canViewMonitoramento) {
        const comparativeSection = document.getElementById('comparative-charts-section');
        if (comparativeSection) comparativeSection.style.display = 'none';
        
        if (monitoramentoCard) {
            monitoramentoCard.style.display = 'none';
            monitoramentoCard.classList.remove('visible');
        }
        
        if (lojaCardCol) {
            lojaCardCol.classList.add('full-width');
        }
        
        console.log(`Dashboard adaptado para ${currentUser.role} - card Monitoramento oculto (visível apenas para admin, monitoramento, dev, consultor)`);
    }
    
    // Garantir que o card Bluve esteja SEMPRE visível (FORÇADO)
    if (lojaCardCol) {
        lojaCardCol.style.display = 'block';
        lojaCardCol.style.visibility = 'visible';
        lojaCardCol.style.opacity = '1';
        lojaCardCol.classList.remove('d-none', 'hidden', 'invisible');
        console.log('✅ Card Bluve garantido como visível - display:', lojaCardCol.style.display);
    } else {
        console.error('❌ ERRO CRÍTICO: Elemento loja-card-col não encontrado no DOM!');
    }
    
    // Verificação adicional após delay para garantir que o card permanece visível
    setTimeout(() => {
        const lojaCardCheck = document.getElementById('loja-card-col');
        if (lojaCardCheck) {
            lojaCardCheck.style.display = 'block';
            lojaCardCheck.style.visibility = 'visible';
            lojaCardCheck.style.opacity = '1';
            const computedDisplay = window.getComputedStyle(lojaCardCheck).display;
            console.log('✅ Card Bluve RE-verificado - computedDisplay:', computedDisplay);
            
            if (computedDisplay === 'none') {
                console.error('❌ ALERTA: Card Bluve ainda está com display:none. Forçando novamente...');
                lojaCardCheck.style.setProperty('display', 'block', 'important');
            }
        }
    }, 100);
    
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

        const baseParams = {};
        if (dataInicio) baseParams.data_inicio = dataInicio;
        if (dataFim) baseParams.data_fim = dataFim;
        if (loja !== 'todas') {
            baseParams.loja = loja;
        }

        const currentParams = new URLSearchParams(baseParams);
        
        let comparisonParams;
        if (dataInicio && dataFim) {
            const startDate = new Date(dataInicio + 'T00:00:00');
            const endDate = new Date(dataFim + 'T00:00:00');
            
            // Comparar com período anterior
            const diff = endDate.getTime() - startDate.getTime();
            const compEndDate = new Date(startDate.getTime() - 86400000);
            const compStartDate = new Date(compEndDate.getTime() - diff);
            const compBaseParams = {...baseParams, data_inicio: toISODateString(compStartDate), data_fim: toISODateString(compEndDate) };
            comparisonParams = new URLSearchParams(compBaseParams);
        } else {
            comparisonParams = new URLSearchParams({});
        }
        
        const rankingParams = new URLSearchParams();
        if (dataInicio) rankingParams.append('data_inicio', dataInicio);
        if (dataFim) rankingParams.append('data_fim', dataFim);
        
        const apiCalls = [
            fetch(`/api/dashboard-data?${currentParams.toString()}`),
            fetch(`/api/ranking?${rankingParams.toString()}`),
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

            updateUI(results, isNotAdmin, dataInicio, dataFim);
            
            // Atualizar gráficos de desempenho das lojas
            loadStorePerformance(dataInicio, dataFim);
        } catch (error) {
            console.error("Erro ao analisar dados:", error.message, error.stack);
            showToast("Erro", "Não foi possível carregar os dados do dashboard.", "danger");
        } finally {
            setLoadingState(false);
        }
    }
    
    // Carregar dados do Google Drive
    async function loadDriveStats() {
        try {
            const response = await fetch('/api/settings/drive/usage');
            if (!response.ok) throw new Error('Erro ao carregar Drive');
            
            const data = await response.json();
            
            document.getElementById('dash-drive-usado').textContent = data.usadoGB + ' GB';
            document.getElementById('dash-drive-disponivel').textContent = data.disponivelGB.toFixed(2) + ' GB';
            document.getElementById('dash-drive-percentual').textContent = data.percentual + '%';
            
            const statusBadge = document.getElementById('dash-drive-status');
            if (data.precisaBackup) {
                statusBadge.textContent = 'Atenção';
                statusBadge.className = 'badge bg-warning';
            } else {
                statusBadge.textContent = 'Online';
                statusBadge.className = 'badge bg-success';
            }
        } catch (error) {
            console.error('Erro ao carregar stats do Drive:', error);
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
    
    // Carregar dados do Drive ao iniciar
    loadDriveStats();

    if (barChartMetricSelect) {
        barChartMetricSelect.addEventListener('change', () => {
            if (lastRankingData.length > 0) {
                renderBarChart(lastRankingData);
            }
        });
    }

    // Inicialização
    async function inicializar() {
        console.log('Inicializando dashboard...');
        await carregarLojas();
        console.log('Lojas carregadas');
        
        // Inicializar cards de Monitoramento e Bluve
        initMetricsCards();
        
        loadDemandas(); // Carregar demandas pendentes
        
        // Carregar todos os dados sem filtro de data
        console.log('Carregando todos os relatórios...');
        loadStorePerformance();
        analisarDados();
    }
    
    console.log('Iniciando admin page...');
    inicializar();
}
