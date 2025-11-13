import { showToast, showConfirmModal, getAuthHeaders } from '../utils.js';

export function initDemandasPage() {
    const containerPendentes = document.getElementById('demandas-pendentes-container');
    const containerHistorico = document.getElementById('demandas-historico-container');
    const abas = document.querySelectorAll('#demandasTab .nav-link');
    const modalDemandaEl = document.getElementById('modal-add-demanda');
    
    // ===== A MUDANÇA ESTÁ AQUI =====
    // Trocamos 'page-content' pelo nosso novo wrapper específico da página.
    const demandasWrapper = document.getElementById('demandas-page-wrapper');
    // ===============================

    if (!modalDemandaEl || !demandasWrapper) return; // Adicionada verificação para o wrapper

    const modalDemanda = new bootstrap.Modal(modalDemandaEl);
    const formDemanda = document.getElementById('form-add-demanda');
    const selectLojaModal = document.getElementById('demanda-loja');

    async function carregarLojasNoModal() {
        try {
            const response = await fetch('/api/lojas?status=ativa');
            const lojas = await response.json();
            selectLojaModal.innerHTML = '<option value="" selected disabled>Selecione uma loja</option>';
            lojas.forEach(loja => {
                selectLojaModal.add(new Option(loja.nome, loja.nome));
            });
        } catch (error) {
            console.error("Falha ao carregar lojas:", error);
        }
    }

    async function carregarDemandas(tipo) {
        const container = tipo === 'pendentes' ? containerPendentes : containerHistorico;
        const url = `/api/demandas/${tipo}`;
        if (!container) return;

        container.innerHTML = '<p class="text-center">Carregando...</p>';
        try {
            const response = await fetch(url);
            const demandas = await response.json();
            if (demandas.length === 0) {
                container.innerHTML = '<p class="text-center text-muted">Nenhuma demanda encontrada.</p>';
                return;
            }

            container.innerHTML = demandas.map(d => {
                const tagCores = { 'Urgente': 'bg-danger', 'Alta': 'bg-warning text-dark', 'Normal': 'bg-info text-dark', 'Baixa': 'bg-secondary' };
                let acoesHtml = '';
                
                if (tipo === 'pendentes') {
                    acoesHtml = `<div class="d-flex justify-content-end mt-2"><button class="btn btn-sm btn-success me-2" data-action="concluir" data-id="${d.id}" title="Concluir"><i class="bi bi-check-lg"></i></button><button class="btn btn-sm btn-danger" data-action="excluir" data-id="${d.id}" title="Excluir"><i class="bi bi-trash"></i></button></div>`;
                } else if (tipo === 'historico') {
                    acoesHtml = `<div class="d-flex justify-content-end mt-2"><button class="btn btn-sm btn-danger" data-action="excluir" data-id="${d.id}" title="Excluir do Histórico"><i class="bi bi-trash"></i></button></div>`;
                }
                const footerHtml = tipo === 'pendentes' ? `Criado por <strong>${d.criado_por_usuario}</strong> em ${new Date(d.criado_em).toLocaleDateString('pt-BR')}` : `Concluído por <strong>${d.concluido_por_usuario || 'N/A'}</strong> em ${new Date(d.concluido_em).toLocaleDateString('pt-BR')}`;

                return `<div class="card mb-3"><div class="card-body"><div class="d-flex justify-content-between align-items-start"><div><h5 class="card-title mb-1">${d.loja_nome}</h5><p class="card-text mb-0">${d.descricao}</p></div><span class="badge ${tagCores[d.tag] || 'bg-light text-dark'}">${d.tag}</span></div>${acoesHtml}</div><div class="card-footer text-muted small">${footerHtml}</div></div>`;
            }).join('');
        } catch (e) {
            container.innerHTML = '<p class="text-center text-danger">Erro ao carregar demandas.</p>';
        }
    }

    formDemanda.addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = Object.fromEntries(new FormData(e.target).entries());
        if (!data.loja_nome) {
            showToast('Atenção', 'Selecione uma loja.', 'danger');
            return;
        }
        try {
            const response = await fetch('/api/demandas', { method: 'POST', headers: await getAuthHeaders(), body: JSON.stringify(data) });
            if (!response.ok) throw new Error('Falha ao adicionar demanda.');
            modalDemanda.hide();
            e.target.reset();
            showToast('Sucesso', 'Demanda adicionada.', 'success');
            carregarDemandas('pendentes');
        } catch (error) {
            showToast('Erro', error.message, 'danger');
        }
    });

    abas.forEach(aba => {
        aba.addEventListener('shown.bs.tab', (event) => {
            const targetId = event.target.getAttribute('data-bs-target');
            // A lógica para carregar o histórico estava errada, corrigido para 'historico'
            if (targetId === '#pendentes') carregarDemandas('pendentes');
            else if (targetId === '#historico') carregarDemandas('historico'); // CORREÇÃO LÓGICA
        });
    });

    // ===== A MUDANÇA ESTÁ AQUI =====
    // O ouvinte agora está no nosso wrapper, isolado do resto do app.
    demandasWrapper.addEventListener('click', async (e) => {
        const button = e.target.closest('button[data-action]');
        // A verificação de containeres ainda é uma boa prática
        if (!button || !button.closest('#demandas-pendentes-container, #demandas-historico-container')) return;

        const id = button.dataset.id;
        const action = button.dataset.action;
        const isPendente = !!button.closest('#demandas-pendentes-container');

        if (action === 'concluir') {
            const confirmed = await showConfirmModal('Marcar esta demanda como concluída?');
            if (!confirmed) return;
            try {
                const response = await fetch(`/api/demandas/${id}/concluir`, { method: 'PUT', headers: await getAuthHeaders() });
                if (!response.ok) throw new Error('Falha ao concluir demanda.');
                showToast('Sucesso', 'Demanda movida para o histórico.', 'info');
                carregarDemandas('pendentes');
            } catch (e) { showToast('Erro', 'Não foi possível concluir.', 'danger'); }
        }

        if (action === 'excluir') {
            const confirmed = await showConfirmModal('EXCLUIR PERMANENTEMENTE esta demanda?');
            if (!confirmed) return;
            try {
                const response = await fetch(`/api/demandas/${id}`, { method: 'DELETE', headers: await getAuthHeaders() });
                if (!response.ok) throw new Error('Falha ao excluir demanda.');
                showToast('Sucesso', 'Demanda excluída.', 'success');
                // A lógica para recarregar o histórico estava errada, corrigido.
                if (isPendente) carregarDemandas('pendentes');
                else carregarDemandas('historico'); // CORREÇÃO LÓGICA
            } catch (e) { showToast('Erro', 'Não foi possível excluir.', 'danger'); }
        }
    });

    carregarLojasNoModal();
    carregarDemandas('pendentes');
}