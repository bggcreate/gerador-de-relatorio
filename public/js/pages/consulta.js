import { showToast, showConfirmModal, getAuthHeaders } from '../utils.js';

let eventController;

export function initConsultaPage() {
    const formFiltros = document.getElementById('form-filtros-consulta');
    const tableBody = document.getElementById('tabela-relatorios-corpo');
    const filtroLoja = document.getElementById('filtro-loja');
    const filtroInicio = document.getElementById('filtro-data-inicio');
    const filtroFim = document.getElementById('filtro-data-fim');
    // ADICIONADO: Seleção do novo filtro de ordem
    const filtroOrdem = document.getElementById('filtro-ordem');
    const btnLimpar = document.getElementById('btn-limpar-filtros');
    const btnCarregarMais = document.getElementById('btn-carregar-mais');
    const modalViewEl = document.getElementById('modal-visualizar-relatorio');
    const pageContent = document.getElementById('page-content');

    if (!formFiltros || !modalViewEl) return;
    const modalView = new bootstrap.Modal(modalViewEl);

    let currentOffset = 0;
    const limit = 20;
    let currentReportId = null;
    let totalReportsCount = 0;

    if (eventController) {
        eventController.abort();
    }
    eventController = new AbortController();

    async function carregarRelatorios(isNewSearch = true) {
        if (isNewSearch) {
            currentOffset = 0;
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Carregando...</td></tr>';
        }
        btnCarregarMais.disabled = true;
        btnCarregarMais.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Carregando...';

        const params = new URLSearchParams();
        if (filtroLoja.value) params.append('loja', filtroLoja.value);
        if (filtroInicio.value) params.append('data_inicio', filtroInicio.value);
        if (filtroFim.value) params.append('data_fim', filtroFim.value);
        // ADICIONADO: Envio do parâmetro de ordenação para a API
        params.append('sortOrder', filtroOrdem.value);
        params.append('limit', limit);
        params.append('offset', currentOffset);

        try {
            const response = await fetch(`/api/relatorios?${params.toString()}`);
            const { relatorios, total } = await response.json();

            if (isNewSearch) {
                totalReportsCount = total;
            }

            const formatCurrency = (value) => {
                const numberValue = Number(value) || 0;
                return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(numberValue);
            };

            const newRowsHtml = relatorios.map((r, index) => {
                // ALTERAÇÃO: Lógica para criar um ID sequencial contínuo
                const isDescending = filtroOrdem.value === 'desc';
                const sequentialId = isDescending
                    ? totalReportsCount - currentOffset - index
                    : currentOffset + index + 1;

                return `
                    <tr>
                        <td class="ps-3">${sequentialId}</td>
                        <td>${r.loja}</td>
                        <td>${new Date(r.data).toLocaleDateString('pt-BR', {timeZone: 'UTC'})}</td>
                        <td>${formatCurrency(r.total_vendas_dinheiro)}</td>
                        <td class="text-end pe-3">
                            <div class="btn-group btn-group-sm" role="group">
                                <button type="button" class="btn btn-outline-primary" data-action="visualizar" data-id="${r.id}" title="Visualizar"><i class="bi bi-eye"></i></button>
                                <a href="/novo-relatorio?edit=${r.id}" class="btn btn-outline-secondary" title="Editar"><i class="bi bi-pencil-fill"></i></a>
                                <button type="button" class="btn btn-outline-danger" data-action="excluir" data-id="${r.id}" title="Excluir"><i class="bi bi-trash"></i></button>
                            </div>
                        </td>
                    </tr>
                `;
            }).join('');

            if (isNewSearch) {
                tableBody.innerHTML = relatorios.length > 0 ? newRowsHtml : '<tr><td colspan="5" class="text-center">Nenhum relatório encontrado.</td></tr>';
            } else {
                tableBody.insertAdjacentHTML('beforeend', newRowsHtml);
            }

            currentOffset += relatorios.length;
            btnCarregarMais.classList.toggle('d-none', currentOffset >= totalReportsCount);

        } catch (e) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Erro ao carregar relatórios.</td></tr>';
        } finally {
            btnCarregarMais.disabled = false;
            btnCarregarMais.innerHTML = 'Carregar Mais Relatórios';
        }
    }

    async function visualizarRelatorio(id) {
        currentReportId = id;
        const modalLabel = document.getElementById('modal-visualizar-label');
        const listaAnexos = document.getElementById('lista-anexos');
        const infoLoja = document.getElementById('info-loja');
        const infoData = document.getElementById('info-data');
        const infoVendas = document.getElementById('info-vendas');
        const tabRelatorio = document.getElementById('tab-relatorio');
        
        modalLabel.textContent = `Carregando Relatório...`;
        tabRelatorio.innerHTML = '<div class="d-flex justify-content-center align-items-center" style="height: 70vh;"><div class="spinner-border" role="status"></div></div>';
        listaAnexos.innerHTML = '<div class="text-muted small text-center py-2"><i class="bi bi-file-earmark"></i> Carregando...</div>';
        modalView.show();

        try {
            // Buscar dados do relatório
            const relatorioResponse = await fetch(`/api/relatorios/${id}`);
            if (!relatorioResponse.ok) throw new Error("Não foi possível carregar dados do relatório.");
            const { relatorio } = await relatorioResponse.json();
            
            // Preencher informações do relatório
            infoLoja.textContent = relatorio.loja || '-';
            infoData.textContent = new Date(relatorio.data).toLocaleDateString('pt-BR', {timeZone: 'UTC'});
            infoVendas.textContent = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(relatorio.total_vendas_dinheiro || 0);
            
            // Carregar preview do PDF do relatório na aba correta
            const response = await fetch(`/api/relatorios/${id}/pdf`);
            if (!response.ok) throw new Error("Não foi possível gerar a visualização do PDF.");
            const fileBlob = await response.blob();
            const fileURL = URL.createObjectURL(fileBlob);
            modalLabel.textContent = `Visualizar Relatório #${id}`;
            tabRelatorio.innerHTML = `<iframe src="${fileURL}" style="width: 100%; height: 70vh; border: none;"></iframe>`;
            
            // Carregar anexos (tickets e rankings)
            await carregarAnexos(id, relatorio.loja, relatorio.data);
        } catch (e) {
            tabRelatorio.innerHTML = `<div class="p-3 text-center text-danger"><h3>Oops!</h3><p>Não foi possível carregar a visualização.</p></div>`;
            showToast('Erro', e.message, 'danger');
        }
    }
    
    async function carregarAnexos(relatorioId, loja, data) {
        const listaAnexos = document.getElementById('lista-anexos');
        const tabsList = document.getElementById('tabs-visualizacao');
        const tabsContent = document.getElementById('tabs-visualizacao-content');
        
        try {
            // Buscar PDFs de ticket e ranking associados ao relatório
            const [ticketsResponse, rankingsResponse] = await Promise.all([
                fetch(`/api/pdf/tickets?loja=${encodeURIComponent(loja)}&data=${data}`),
                fetch(`/api/pdf/rankings?loja=${encodeURIComponent(loja)}&data=${data}`)
            ]);
            
            const ticketsData = await ticketsResponse.json();
            const rankingsData = await rankingsResponse.json();
            
            const tickets = ticketsData.tickets || [];
            const rankings = rankingsData.rankings || [];
            
            // Limpar abas antigas (exceto a do relatório)
            const existingTabs = tabsList.querySelectorAll('li:not(:first-child)');
            existingTabs.forEach(tab => tab.remove());
            
            const existingPanes = tabsContent.querySelectorAll('.tab-pane:not(#tab-relatorio)');
            existingPanes.forEach(pane => pane.remove());
            
            if (tickets.length === 0 && rankings.length === 0) {
                listaAnexos.innerHTML = '<div class="text-muted small text-center py-2"><i class="bi bi-inbox"></i> Nenhum anexo encontrado</div>';
                return;
            }
            
            // Renderizar lista de anexos na sidebar
            let anexosHtml = '';
            let tabIndex = 1;
            
            // Adicionar rankings
            rankings.forEach(ranking => {
                const dataUpload = new Date(ranking.uploaded_at).toLocaleDateString('pt-BR');
                const tabId = `tab-ranking-${ranking.id}`;
                
                anexosHtml += `
                    <div class="anexo-item p-2 mb-2 border rounded bg-light" style="cursor: pointer;" data-tab-target="#${tabId}">
                        <div class="d-flex align-items-center">
                            <i class="bi bi-file-earmark-pdf text-warning fs-4 me-2"></i>
                            <div class="flex-grow-1">
                                <div class="fw-bold small">PDF Ranking</div>
                                <div class="text-muted" style="font-size: 0.75rem;">${ranking.filename}</div>
                                <div class="text-muted" style="font-size: 0.7rem;">Enviado em ${dataUpload}</div>
                            </div>
                        </div>
                    </div>
                `;
                
                // Criar aba para o ranking
                tabsList.insertAdjacentHTML('beforeend', `
                    <li class="nav-item" role="presentation">
                        <button class="nav-link" id="${tabId}-btn" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab">
                            <i class="bi bi-file-earmark-pdf text-warning me-1"></i> Ranking
                        </button>
                    </li>
                `);
                
                // Criar conteúdo da aba com iframe
                tabsContent.insertAdjacentHTML('beforeend', `
                    <div class="tab-pane fade" id="${tabId}" role="tabpanel" style="min-height: 70vh;">
                        <div class="d-flex justify-content-center align-items-center" style="height: 70vh;">
                            <div class="spinner-border" role="status"></div>
                        </div>
                    </div>
                `);
                
                // Carregar PDF quando a aba for ativada
                document.getElementById(`${tabId}-btn`).addEventListener('shown.bs.tab', async () => {
                    const pane = document.getElementById(tabId);
                    if (!pane.dataset.loaded) {
                        try {
                            const response = await fetch(`/api/pdf/rankings/${ranking.id}/download`);
                            if (!response.ok) throw new Error("Erro ao carregar PDF");
                            const blob = await response.blob();
                            const url = URL.createObjectURL(blob);
                            pane.innerHTML = `<iframe src="${url}" style="width: 100%; height: 70vh; border: none;"></iframe>`;
                            pane.dataset.loaded = 'true';
                        } catch (e) {
                            pane.innerHTML = `<div class="p-3 text-center text-danger"><h3>Erro</h3><p>Não foi possível carregar o PDF.</p></div>`;
                        }
                    }
                });
                
                tabIndex++;
            });
            
            // Adicionar tickets
            tickets.forEach(ticket => {
                const dataUpload = new Date(ticket.uploaded_at).toLocaleDateString('pt-BR');
                const tabId = `tab-ticket-${ticket.id}`;
                
                anexosHtml += `
                    <div class="anexo-item p-2 mb-2 border rounded bg-light" style="cursor: pointer;" data-tab-target="#${tabId}">
                        <div class="d-flex align-items-center">
                            <i class="bi bi-file-earmark-pdf text-danger fs-4 me-2"></i>
                            <div class="flex-grow-1">
                                <div class="fw-bold small">Ticket Dia</div>
                                <div class="text-muted" style="font-size: 0.75rem;">${ticket.filename}</div>
                                <div class="text-muted" style="font-size: 0.7rem;">Enviado em ${dataUpload}</div>
                            </div>
                        </div>
                    </div>
                `;
                
                // Criar aba para o ticket
                tabsList.insertAdjacentHTML('beforeend', `
                    <li class="nav-item" role="presentation">
                        <button class="nav-link" id="${tabId}-btn" data-bs-toggle="tab" data-bs-target="#${tabId}" type="button" role="tab">
                            <i class="bi bi-file-earmark-pdf text-danger me-1"></i> Ticket
                        </button>
                    </li>
                `);
                
                // Criar conteúdo da aba com iframe
                tabsContent.insertAdjacentHTML('beforeend', `
                    <div class="tab-pane fade" id="${tabId}" role="tabpanel" style="min-height: 70vh;">
                        <div class="d-flex justify-content-center align-items-center" style="height: 70vh;">
                            <div class="spinner-border" role="status"></div>
                        </div>
                    </div>
                `);
                
                // Carregar PDF quando a aba for ativada
                document.getElementById(`${tabId}-btn`).addEventListener('shown.bs.tab', async () => {
                    const pane = document.getElementById(tabId);
                    if (!pane.dataset.loaded) {
                        try {
                            const response = await fetch(`/api/pdf/tickets/${ticket.id}/download`);
                            if (!response.ok) throw new Error("Erro ao carregar PDF");
                            const blob = await response.blob();
                            const url = URL.createObjectURL(blob);
                            pane.innerHTML = `<iframe src="${url}" style="width: 100%; height: 70vh; border: none;"></iframe>`;
                            pane.dataset.loaded = 'true';
                        } catch (e) {
                            pane.innerHTML = `<div class="p-3 text-center text-danger"><h3>Erro</h3><p>Não foi possível carregar o PDF.</p></div>`;
                        }
                    }
                });
                
                tabIndex++;
            });
            
            listaAnexos.innerHTML = anexosHtml;
            
            // Adicionar event listeners para abrir aba ao clicar no anexo
            document.querySelectorAll('.anexo-item').forEach(item => {
                item.addEventListener('click', () => {
                    const tabTarget = item.dataset.tabTarget;
                    const tabButton = document.querySelector(`[data-bs-target="${tabTarget}"]`);
                    if (tabButton) {
                        const tab = new bootstrap.Tab(tabButton);
                        tab.show();
                    }
                });
            });
            
        } catch (e) {
            listaAnexos.innerHTML = '<div class="text-muted small text-center py-2"><i class="bi bi-exclamation-triangle"></i> Erro ao carregar anexos</div>';
            console.error('Erro ao carregar anexos:', e);
        }
    }

    async function excluirRelatorio(id) {
        const confirmed = await showConfirmModal(`Tem certeza que deseja excluir o relatório #${id}?`);
        if (!confirmed) return;
        try {
            const response = await fetch(`/api/relatorios/${id}`, { 
                method: 'DELETE',
                headers: await getAuthHeaders()
            });
            if (!response.ok) throw new Error('Falha ao excluir o relatório.');
            showToast('Sucesso', 'Relatório excluído com sucesso.', 'success');
            carregarRelatorios(true);
        } catch (e) {
            showToast('Erro', 'Não foi possível excluir o relatório.', 'danger');
        }
    }

    async function carregarLojasNoFiltro() {
        try {
            const response = await fetch('/api/lojas');
            const lojas = await response.json();
            filtroLoja.innerHTML = '<option value="">Todas as Lojas</option>';
            lojas.forEach(loja => filtroLoja.add(new Option(loja.nome, loja.nome)));
        } catch (e) {
            console.error("Erro ao carregar lojas:", e);
        }
    }
    
    // --- Event Listeners ---
    document.getElementById('btn-copiar-texto-modal')?.addEventListener('click', async () => {
        if (!currentReportId) return;
        try {
            const response = await fetch(`/api/relatorios/${currentReportId}/txt`);
            if (!response.ok) throw new Error("Falha ao buscar texto para cópia.");
            const textToCopy = await response.text();
            await navigator.clipboard.writeText(textToCopy);
            showToast('Sucesso!', 'Texto copiado.', 'success');
        } catch (err) {
            showToast('Erro', 'Não foi possível copiar o texto.', 'danger');
        }
    }, { signal: eventController.signal });

    document.getElementById('btn-gerar-pdf-modal')?.addEventListener('click', () => {
        if (!currentReportId) return;
        window.open(`/api/relatorios/${currentReportId}/pdf`, '_blank');
    }, { signal: eventController.signal });

    formFiltros.addEventListener('submit', (e) => { e.preventDefault(); carregarRelatorios(true); }, { signal: eventController.signal });
    
    // ADICIONADO: Resetar o filtro de ordem ao limpar
    btnLimpar.addEventListener('click', () => { 
        formFiltros.reset(); 
        filtroOrdem.value = 'desc'; // Garante que o padrão seja selecionado
        carregarRelatorios(true); 
    }, { signal: eventController.signal });
    
    btnCarregarMais.addEventListener('click', () => carregarRelatorios(false), { signal: eventController.signal });

    pageContent.addEventListener('click', (e) => {
        const button = e.target.closest('button[data-action]');
        if (!button || !button.closest('#tabela-relatorios-corpo')) return;
        const id = button.dataset.id;
        const action = button.dataset.action;
        if (action === 'visualizar') visualizarRelatorio(id);
        if (action === 'excluir') excluirRelatorio(id);
    }, { signal: eventController.signal });
    
    // ADICIONADO: Event listener para o novo filtro de ordem
    filtroOrdem.addEventListener('change', () => carregarRelatorios(true), { signal: eventController.signal });

    const formExport = document.getElementById('form-export-excel');
    const exportMonthSelect = document.getElementById('export-month');
    const exportYearSelect = document.getElementById('export-year');
    
    if (exportYearSelect.options.length <= 1) {
        const currentYear = new Date().getFullYear();
        for (let i = 0; i < 5; i++) {
            const year = currentYear - i;
            exportYearSelect.add(new Option(year, year));
        }
    }
    exportMonthSelect.value = new Date().getMonth() + 1;

    formExport.addEventListener('submit', async (e) => {
        e.preventDefault();
        const btn = e.target.querySelector('button[type="submit"]');
        const originalText = btn.innerHTML;
        btn.disabled = true;
        btn.innerHTML = `<span class="spinner-border spinner-border-sm"></span> Gerando...`;
        
        try {
            const response = await fetch(`/api/export/excel?month=${exportMonthSelect.value}&year=${exportYearSelect.value}`);
            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                const monthName = new Date(exportYearSelect.value, exportMonthSelect.value - 1).toLocaleString('pt-BR', { month: 'long' });
                a.download = `Relatorios_${monthName}_${exportYearSelect.value}.xlsx`;
                a.href = url;
                a.click();
                window.URL.revokeObjectURL(url);
            } else {
                const result = await response.json();
                showToast("Erro ao Exportar", result.error || "Não foi possível gerar.", "danger");
            }
        } catch (err) {
            showToast("Erro", "Falha na exportação.", "danger");
        } finally {
            btn.disabled = false;
            btn.innerHTML = originalText;
        }
    }, { signal: eventController.signal });

    // Evento para exportar TODOS os relatórios
    const btnExportarTodos = document.getElementById('btn-exportar-todos-relatorios');
    if (btnExportarTodos) {
        btnExportarTodos.addEventListener('click', async (e) => {
            e.preventDefault();
            const originalText = btnExportarTodos.innerHTML;
            btnExportarTodos.innerHTML = `<span class="spinner-border spinner-border-sm"></span> Gerando...`;
            btnExportarTodos.style.pointerEvents = 'none';
            
            try {
                const response = await fetch(`/api/export/excel/all`);
                if (response.ok) {
                    const blob = await response.blob();
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    const currentDate = new Date().toLocaleDateString('pt-BR').replace(/\//g, '-');
                    a.download = `Todos_Relatorios_${currentDate}.xlsx`;
                    a.href = url;
                    a.click();
                    window.URL.revokeObjectURL(url);
                    showToast("Sucesso!", "Todos os relatórios foram exportados!", "success");
                } else {
                    const result = await response.json();
                    showToast("Erro ao Exportar", result.error || "Não foi possível gerar.", "danger");
                }
            } catch (err) {
                showToast("Erro", "Falha na exportação.", "danger");
            } finally {
                btnExportarTodos.innerHTML = originalText;
                btnExportarTodos.style.pointerEvents = 'auto';
            }
        }, { signal: eventController.signal });
    }

    carregarLojasNoFiltro().then(() => carregarRelatorios(true));
}