import { showToast, showConfirmModal } from '../utils.js';

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
        const modalBody = document.getElementById('modal-body-content');
        const modalLabel = document.getElementById('modal-visualizar-label');
        modalLabel.textContent = `Carregando Relatório...`;
        modalBody.innerHTML = '<div class="d-flex justify-content-center p-5"><div class="spinner-border" role="status"></div></div>';
        modalView.show();

        try {
            const response = await fetch(`/api/relatorios/${id}/pdf`);
            if (!response.ok) throw new Error("Não foi possível gerar a visualização do PDF.");
            const fileBlob = await response.blob();
            const fileURL = URL.createObjectURL(fileBlob);
            modalLabel.textContent = `Visualizar Relatório #${id}`;
            modalBody.innerHTML = `<iframe src="${fileURL}" style="width: 100%; height: 70vh; border: none;"></iframe>`;
        } catch (e) {
            modalBody.innerHTML = `<div class="p-3 text-center text-danger"><h3>Oops!</h3><p>Não foi possível carregar a visualização.</p></div>`;
            showToast('Erro', e.message, 'danger');
        }
    }

    async function excluirRelatorio(id) {
        const confirmed = await showConfirmModal(`Tem certeza que deseja excluir o relatório #${id}?`);
        if (!confirmed) return;
        try {
            const response = await fetch(`/api/relatorios/${id}`, { method: 'DELETE' });
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
        const btn = e.target.querySelector('button');
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

    carregarLojasNoFiltro().then(() => carregarRelatorios(true));
}