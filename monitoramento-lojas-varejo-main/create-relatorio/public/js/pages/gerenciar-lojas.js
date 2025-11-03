import { showToast, showConfirmModal } from '../utils.js';

export function initGerenciarLojasPage() {
    const tableBody = document.getElementById('tabela-lojas-corpo');
    const btnAdicionar = document.getElementById('btn-adicionar-loja');
    const modalEl = document.getElementById('modal-loja');
    
    if (!tableBody || !btnAdicionar || !modalEl) return;
    
    const modal = new bootstrap.Modal(modalEl);
    const modalForm = document.getElementById('form-loja');
    const modalTitle = document.getElementById('modalLojaLabel');
    let lojasCache = [];

    async function carregarLojas() {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Carregando...</td></tr>';
        try {
            const response = await fetch('/api/lojas');
            lojasCache = await response.json();
            if (lojasCache.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Nenhuma loja cadastrada.</td></tr>';
                return;
            }
            tableBody.innerHTML = lojasCache.map(loja => {
                const statusBadge = loja.status === 'ativa' ? `<span class="badge bg-success">Ativa</span>` : `<span class="badge bg-secondary">Inativa</span>`;
                return `<tr><td>${loja.nome}</td><td>${statusBadge}</td><td>${loja.funcao_especial || '-'}</td><td>${loja.observacoes || '-'}</td><td class="text-end pe-3"><button class="btn btn-sm btn-outline-secondary" data-action="editar" data-id="${loja.id}"><i class="bi bi-pencil"></i></button> <button class="btn btn-sm btn-outline-danger" data-action="excluir" data-id="${loja.id}"><i class="bi bi-trash"></i></button></td></tr>`;
            }).join('');
        } catch (e) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Erro ao carregar.</td></tr>';
        }
    }

    function abrirModalParaAdicionar() {
        modalForm.reset();
        modalTitle.textContent = 'Adicionar Nova Loja';
        document.getElementById('loja-id').value = '';
        modal.show();
    }

    function abrirModalParaEditar(id) {
        const loja = lojasCache.find(l => l.id === id);
        if (!loja) return;
        modalForm.reset();
        modalTitle.textContent = 'Editar Loja';
        document.getElementById('loja-id').value = loja.id;
        document.getElementById('loja-nome').value = loja.nome;
        document.getElementById('loja-status').value = loja.status;
        document.getElementById('loja-funcao-especial').value = loja.funcao_especial || '';
        document.getElementById('loja-observacoes').value = loja.observacoes || '';
        modal.show();
    }

    async function excluirLoja(id) {
        const confirmed = await showConfirmModal(`Tem certeza que deseja excluir esta loja?`);
        if (!confirmed) return;
        try {
            const response = await fetch(`/api/lojas/${id}`, { method: 'DELETE' });
            if (!response.ok) throw new Error('Falha ao excluir.');
            showToast('Sucesso', 'Loja excluída.', 'success');
            carregarLojas();
        } catch (e) {
            showToast('Erro', 'Não foi possível excluir a loja.', 'danger');
        }
    }

    modalForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const id = document.getElementById('loja-id').value;
        const data = { nome: document.getElementById('loja-nome').value, status: document.getElementById('loja-status').value, funcao_especial: document.getElementById('loja-funcao-especial').value, observacoes: document.getElementById('loja-observacoes').value };
        const method = id ? 'PUT' : 'POST';
        const url = id ? `/api/lojas/${id}` : '/api/lojas';
        try {
            const response = await fetch(url, { method, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
            if (!response.ok) throw new Error('Falha ao salvar. Nome já existe?');
            showToast('Sucesso', `Loja salva.`, 'success');
            modal.hide();
            carregarLojas();
        } catch(e) { showToast('Erro', e.message, 'danger'); }
    });

    btnAdicionar.addEventListener('click', abrirModalParaAdicionar);
    tableBody.addEventListener('click', (e) => {
        const button = e.target.closest('button[data-action]');
        if (!button) return;
        const id = parseInt(button.dataset.id, 10);
        const action = button.dataset.action;
        if (action === 'editar') abrirModalParaEditar(id);
        if (action === 'excluir') excluirLoja(id);
    });
    carregarLojas();
}