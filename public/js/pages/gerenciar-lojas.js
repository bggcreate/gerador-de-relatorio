import { showToast, showConfirmModal } from '../utils.js';

export function initGerenciarLojasPage() {
    // Aguardar currentUser estar disponível
    if (!window.currentUser) {
        setTimeout(initGerenciarLojasPage, 100);
        return;
    }
    
    const userRole = window.currentUser.role;
    const isAdminOrDev = ['admin', 'dev'].includes(userRole);
    const isGerenteOrConsultor = ['gerente', 'consultor'].includes(userRole);
    
    const secaoLojas = document.getElementById('secao-gerenciar-lojas');
    const secaoVendedores = document.getElementById('secao-gerenciar-vendedores');
    
    if (!secaoLojas || !secaoVendedores) {
        console.error('Elementos da página de lojas não encontrados');
        return;
    }
    
    if (isAdminOrDev) {
        secaoLojas.style.display = 'block';
        secaoVendedores.style.display = 'none';
        initGerenciarLojas();
    } else if (isGerenteOrConsultor) {
        secaoLojas.style.display = 'none';
        secaoVendedores.style.display = 'block';
        initGerenciarVendedores();
    }
}

function initGerenciarLojas() {
    const tableBody = document.getElementById('tabela-lojas-corpo');
    const btnAdicionar = document.getElementById('btn-adicionar-loja');
    const modalEl = document.getElementById('modal-loja');
    
    if (!tableBody || !btnAdicionar || !modalEl) return;
    
    const modal = new bootstrap.Modal(modalEl);
    const modalForm = document.getElementById('form-loja');
    const modalTitle = document.getElementById('modalLojaLabel');
    let lojasCache = [];
    let tecnicosCache = [];

    async function carregarTecnicos() {
        try {
            const response = await fetch('/api/usuarios');
            const usuarios = await response.json();
            tecnicosCache = usuarios.filter(u => u.role === 'tecnico');
            
            const tecnicoSelect = document.getElementById('loja-tecnico');
            tecnicoSelect.innerHTML = '<option value="">Nenhum</option>' + 
                tecnicosCache.map(tec => `<option value="${tec.username}">${tec.username}</option>`).join('');
        } catch (e) {
            console.error('Erro ao carregar técnicos:', e);
        }
    }

    async function carregarLojas() {
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center">Carregando...</td></tr>';
        try {
            const response = await fetch('/api/lojas');
            lojasCache = await response.json();
            if (lojasCache.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="6" class="text-center">Nenhuma loja cadastrada.</td></tr>';
                return;
            }
            tableBody.innerHTML = lojasCache.map(loja => {
                const statusBadge = loja.status === 'ativa' ? `<span class="badge bg-success">Ativa</span>` : `<span class="badge bg-secondary">Inativa</span>`;
                const tecnicoNome = loja.tecnico_username ? `<span class="badge bg-secondary">${loja.tecnico_username}</span>` : '-';
                return `<tr><td>${loja.nome}</td><td>${statusBadge}</td><td>${loja.funcao_especial || '-'}</td><td>${tecnicoNome}</td><td>${loja.observacoes || '-'}</td><td class="text-end pe-3"><button class="btn btn-sm btn-outline-secondary" data-action="editar" data-id="${loja.id}"><i class="bi bi-pencil"></i></button> <button class="btn btn-sm btn-outline-danger" data-action="excluir" data-id="${loja.id}"><i class="bi bi-trash"></i></button></td></tr>`;
            }).join('');
        } catch (e) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Erro ao carregar.</td></tr>';
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
        document.getElementById('loja-tecnico').value = loja.tecnico_username || '';
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
        const data = { 
            nome: document.getElementById('loja-nome').value, 
            status: document.getElementById('loja-status').value, 
            funcao_especial: document.getElementById('loja-funcao-especial').value,
            tecnico_username: document.getElementById('loja-tecnico').value || null,
            observacoes: document.getElementById('loja-observacoes').value 
        };
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
    carregarTecnicos();
    carregarLojas();
}

function initGerenciarVendedores() {
    const selectLoja = document.getElementById('select-loja-vendedores');
    const containerVendedores = document.getElementById('container-vendedores');
    const tableBody = document.getElementById('tabela-vendedores-corpo');
    const btnAdicionar = document.getElementById('btn-adicionar-vendedor');
    const modalEl = document.getElementById('modal-vendedor');
    
    if (!selectLoja || !containerVendedores || !tableBody || !btnAdicionar || !modalEl) return;
    
    const modal = new bootstrap.Modal(modalEl);
    const modalForm = document.getElementById('form-vendedor');
    const modalTitle = document.getElementById('modalVendedorLabel');
    let vendedoresCache = [];
    let lojaAtualId = null;

    async function carregarLojas() {
        console.log('🔍 Iniciando carregamento de lojas para vendedores...');
        try {
            const response = await fetch('/api/lojas');
            console.log('📡 Response status:', response.status);
            
            const lojas = await response.json();
            console.log('📦 Lojas recebidas:', lojas);
            console.log('📊 Tipo:', typeof lojas, 'É array?', Array.isArray(lojas), 'Quantidade:', lojas.length);
            
            if (!Array.isArray(lojas) || lojas.length === 0) {
                console.warn('⚠️ Nenhuma loja disponível');
                selectLoja.innerHTML = '<option value="">Nenhuma loja disponível</option>';
                return;
            }
            
            selectLoja.innerHTML = '<option value="">Selecione uma loja...</option>' + 
                lojas.map(loja => {
                    console.log('🏪 Adicionando loja:', loja.nome, 'ID:', loja.id);
                    return `<option value="${loja.id}">${loja.nome}</option>`;
                }).join('');
            
            console.log('✅ Select preenchido com', lojas.length, 'lojas');
        } catch (e) {
            console.error('❌ Erro ao carregar lojas:', e);
            showToast('Erro', 'Não foi possível carregar lojas.', 'danger');
        }
    }

    async function carregarVendedores(lojaId) {
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center">Carregando...</td></tr>';
        try {
            const response = await fetch(`/api/vendedores?loja_id=${lojaId}`);
            vendedoresCache = await response.json();
            
            if (vendedoresCache.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="8" class="text-center">Nenhum vendedor cadastrado nesta loja.</td></tr>';
                return;
            }
            
            tableBody.innerHTML = vendedoresCache.map(v => {
                const statusBadge = v.data_demissao ? 
                    '<span class="badge bg-danger">Demitido</span>' : 
                    '<span class="badge bg-success">Ativo</span>';
                
                return `<tr>
                    <td>${v.nome}</td>
                    <td>${v.telefone}</td>
                    <td>${formatarData(v.data_entrada)}</td>
                    <td>${v.data_demissao ? formatarData(v.data_demissao) : '-'}</td>
                    <td>${v.previsao_entrada || '-'}</td>
                    <td>${v.previsao_saida || '-'}</td>
                    <td>${statusBadge}</td>
                    <td class="text-end pe-3">
                        <button class="btn btn-sm btn-outline-secondary" data-action="editar" data-id="${v.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-sm btn-outline-danger" data-action="excluir" data-id="${v.id}"><i class="bi bi-trash"></i></button>
                    </td>
                </tr>`;
            }).join('');
        } catch (e) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Erro ao carregar vendedores.</td></tr>';
        }
    }

    function formatarData(data) {
        if (!data) return '-';
        const partes = data.split('-');
        if (partes.length === 3) {
            return `${partes[2]}/${partes[1]}/${partes[0]}`;
        }
        return data;
    }

    function abrirModalParaAdicionar() {
        if (!lojaAtualId) {
            showToast('Atenção', 'Selecione uma loja primeiro.', 'warning');
            return;
        }
        
        modalForm.reset();
        modalTitle.textContent = 'Adicionar Vendedor';
        document.getElementById('vendedor-id').value = '';
        document.getElementById('vendedor-loja-id').value = lojaAtualId;
        modal.show();
    }

    function abrirModalParaEditar(id) {
        const vendedor = vendedoresCache.find(v => v.id === id);
        if (!vendedor) return;
        
        modalForm.reset();
        modalTitle.textContent = 'Editar Vendedor';
        document.getElementById('vendedor-id').value = vendedor.id;
        document.getElementById('vendedor-loja-id').value = vendedor.loja_id;
        document.getElementById('vendedor-nome').value = vendedor.nome;
        document.getElementById('vendedor-telefone').value = vendedor.telefone;
        document.getElementById('vendedor-data-entrada').value = vendedor.data_entrada;
        document.getElementById('vendedor-data-demissao').value = vendedor.data_demissao || '';
        document.getElementById('vendedor-previsao-entrada').value = vendedor.previsao_entrada || '';
        document.getElementById('vendedor-previsao-saida').value = vendedor.previsao_saida || '';
        modal.show();
    }

    async function excluirVendedor(id) {
        const confirmed = await showConfirmModal('Tem certeza que deseja excluir este vendedor?');
        if (!confirmed) return;
        
        try {
            const response = await fetch(`/api/vendedores/${id}`, { method: 'DELETE' });
            if (!response.ok) throw new Error('Falha ao excluir.');
            showToast('Sucesso', 'Vendedor excluído.', 'success');
            carregarVendedores(lojaAtualId);
        } catch (e) {
            showToast('Erro', 'Não foi possível excluir o vendedor.', 'danger');
        }
    }

    selectLoja.addEventListener('change', (e) => {
        lojaAtualId = e.target.value;
        
        if (lojaAtualId) {
            containerVendedores.style.display = 'block';
            carregarVendedores(lojaAtualId);
        } else {
            containerVendedores.style.display = 'none';
        }
    });

    btnAdicionar.addEventListener('click', abrirModalParaAdicionar);

    tableBody.addEventListener('click', (e) => {
        const button = e.target.closest('button[data-action]');
        if (!button) return;
        const id = parseInt(button.dataset.id, 10);
        const action = button.dataset.action;
        if (action === 'editar') abrirModalParaEditar(id);
        if (action === 'excluir') excluirVendedor(id);
    });

    modalForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const id = document.getElementById('vendedor-id').value;
        const data = {
            loja_id: parseInt(document.getElementById('vendedor-loja-id').value, 10),
            nome: document.getElementById('vendedor-nome').value,
            telefone: document.getElementById('vendedor-telefone').value,
            data_entrada: document.getElementById('vendedor-data-entrada').value,
            data_demissao: document.getElementById('vendedor-data-demissao').value || null,
            previsao_entrada: document.getElementById('vendedor-previsao-entrada').value || null,
            previsao_saida: document.getElementById('vendedor-previsao-saida').value || null,
            ativo: document.getElementById('vendedor-data-demissao').value ? 0 : 1
        };
        
        const method = id ? 'PUT' : 'POST';
        const url = id ? `/api/vendedores/${id}` : '/api/vendedores';
        
        try {
            const response = await fetch(url, { 
                method, 
                headers: { 'Content-Type': 'application/json' }, 
                body: JSON.stringify(data) 
            });
            
            if (!response.ok) throw new Error('Falha ao salvar vendedor.');
            
            showToast('Sucesso', 'Vendedor salvo com sucesso.', 'success');
            modal.hide();
            carregarVendedores(lojaAtualId);
        } catch (e) {
            showToast('Erro', e.message, 'danger');
        }
    });

    carregarLojas();
}
