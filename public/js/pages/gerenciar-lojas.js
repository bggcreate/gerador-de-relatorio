import { showToast, showConfirmModal, getAuthHeaders } from '../utils.js';

export function initGerenciarLojasPage() {
    const secaoLojas = document.getElementById('secao-gerenciar-lojas');
    
    if (!secaoLojas) {
        console.error('Elementos da página de lojas não encontrados');
        return;
    }
    
    secaoLojas.style.display = 'block';
    initGerenciarLojas();
}

function initGerenciarLojas() {
    const tableBody = document.getElementById('tabela-lojas-corpo');
    const btnAdicionar = document.getElementById('btn-adicionar-loja');
    const modalEl = document.getElementById('modal-loja');
    
    if (!tableBody || !btnAdicionar || !modalEl) {
        console.error('❌ Elementos necessários não encontrados:', {
            tableBody: !!tableBody,
            btnAdicionar: !!btnAdicionar,
            modalEl: !!modalEl
        });
        return;
    }
    
    console.log('✅ Elementos encontrados, inicializando gerenciamento de lojas...');
    
    const modal = new bootstrap.Modal(modalEl);
    const modalForm = document.getElementById('form-loja');
    const modalTitle = document.getElementById('modalLojaLabel');
    let lojasCache = [];
    let tecnicosCache = [];

    async function carregarTecnicos() {
        try {
            const response = await fetch('/api/usuarios');
            const usuarios = await response.json();
            tecnicosCache = usuarios;
            
            const tecnicoSelect = document.getElementById('loja-tecnico');
            tecnicoSelect.innerHTML = '<option value="">Nenhum</option>' + 
                tecnicosCache.map(tec => `<option value="${tec.username}">${tec.username}</option>`).join('');
        } catch (e) {
            console.error('Erro ao carregar técnicos:', e);
        }
    }

    async function carregarLojas() {
        console.log('🔄 carregarLojas() chamada');
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Carregando...</td></tr>';
        try {
            console.log('📡 Fazendo fetch em /api/lojas...');
            const response = await fetch('/api/lojas');
            console.log('📊 Response status:', response.status, response.ok);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const lojas = await response.json();
            console.log('📦 Lojas recebidas:', lojas);
            // Normalizar IDs para números
            lojasCache = lojas.map(loja => ({
                ...loja,
                id: Number(loja.id)
            }));
            
            if (lojasCache.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Nenhuma loja cadastrada.</td></tr>';
                return;
            }
            
            let vendedores = [];
            try {
                const vendedoresResponse = await fetch('/api/vendedores');
                if (vendedoresResponse.ok) {
                    const vendedoresRaw = await vendedoresResponse.json();
                    // Normalizar IDs para números
                    vendedores = vendedoresRaw.map(v => ({
                        ...v,
                        id: Number(v.id),
                        loja_id: Number(v.loja_id)
                    }));
                }
            } catch (e) {
                console.warn('API de vendedores indisponível, continuando sem contagem');
            }
            
            tableBody.innerHTML = lojasCache.map(loja => {
                const statusBadge = loja.status === 'ativa' 
                    ? `<span class="badge" style="background-color: #c3fae8; color: #087f5b;">Ativo</span>` 
                    : `<span class="badge" style="background-color: #f1f3f5; color: #495057;">Inativo</span>`;
                    
                const vendedoresLoja = vendedores.filter(v => v.loja_id === loja.id && v.ativo === 1);
                const totalVendedores = vendedoresLoja.length;
                
                const responsavel = loja.gerente || loja.numero_contato || '-';
                
                const funcaoEspecial = loja.funcao_especial || '-';
                
                return `<tr class="loja-row" data-nome="${loja.nome.toLowerCase()}">
                    <td class="align-middle ps-3"><strong>${loja.nome}</strong></td>
                    <td class="align-middle">${responsavel}</td>
                    <td class="text-center align-middle">${statusBadge}</td>
                    <td class="align-middle">${funcaoEspecial}</td>
                    <td class="text-end align-middle pe-3">
                        <button class="btn btn-sm btn-outline-info" data-action="detalhes" data-id="${loja.id}" title="Ver Detalhes e Vendedores">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-secondary" data-action="editar" data-id="${loja.id}" title="Editar Loja">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger" data-action="excluir" data-id="${loja.id}" title="Excluir Loja">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>`;
            }).join('');
            
            // Atualizar estatísticas
            atualizarEstatisticas();
        } catch (e) {
            console.error('Erro ao carregar lojas:', e);
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Erro ao carregar lojas.</td></tr>';
        }
    }
    
    function atualizarEstatisticas() {
        const total = lojasCache.length;
        const ativas = lojasCache.filter(l => l.status === 'ativa').length;
        const inativas = lojasCache.filter(l => l.status === 'inativa').length;
        const omni = lojasCache.filter(l => l.funcao_especial && l.funcao_especial.toLowerCase() === 'omni').length;
        
        document.getElementById('stats-total-lojas').textContent = total;
        document.getElementById('stats-lojas-ativas').textContent = ativas;
        document.getElementById('stats-lojas-inativas').textContent = inativas;
        document.getElementById('stats-lojas-omni').textContent = omni;
    }
    
    function implementarBusca() {
        const buscaInput = document.getElementById('busca-lojas');
        if (buscaInput) {
            buscaInput.addEventListener('input', (e) => {
                const termo = e.target.value.toLowerCase().trim();
                const rows = tableBody.querySelectorAll('.loja-row');
                
                rows.forEach(row => {
                    const nome = row.dataset.nome;
                    if (nome.includes(termo)) {
                        row.style.display = '';
                    } else {
                        row.style.display = 'none';
                    }
                });
            });
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
        document.getElementById('loja-cep').value = loja.cep || '';
        document.getElementById('loja-numero-contato').value = loja.numero_contato || '';
        document.getElementById('loja-gerente').value = loja.gerente || '';
        document.getElementById('loja-status').value = loja.status;
        document.getElementById('loja-funcao-especial').value = loja.funcao_especial || '';
        document.getElementById('loja-tecnico').value = loja.tecnico_username || '';
        document.getElementById('loja-observacoes').value = loja.observacoes || '';
        modal.show();
    }
    
    async function mostrarDetalhes(id) {
        console.log('=== mostrarDetalhes chamado ===');
        console.log('ID recebido:', id, 'tipo:', typeof id);
        console.log('lojasCache:', lojasCache);
        const loja = lojasCache.find(l => l.id === id);
        console.log('Loja encontrada:', loja);
        if (!loja) {
            console.error('Loja não encontrada com ID:', id);
            return;
        }
        
        const modalDetalhes = new bootstrap.Modal(document.getElementById('modal-detalhes-loja'));
        const tabelaVendedoresDetalhes = document.getElementById('tabela-vendedores-detalhes');
        const semVendedores = document.getElementById('sem-vendedores');
        
        // Atualizar título do modal
        document.getElementById('modalDetalhesLojaLabel').textContent = `Detalhes da Loja - ${loja.nome}`;
        
        // Carregar vendedores da loja
        try {
            const response = await fetch('/api/vendedores');
            const vendedoresRaw = await response.json();
            // Normalizar IDs para números
            const vendedores = vendedoresRaw.map(v => ({
                ...v,
                id: Number(v.id),
                loja_id: Number(v.loja_id)
            }));
            const vendedoresLoja = vendedores.filter(v => v.loja_id === id);
            
            if (vendedoresLoja.length === 0) {
                tabelaVendedoresDetalhes.innerHTML = '';
                semVendedores.classList.remove('d-none');
            } else {
                semVendedores.classList.add('d-none');
                tabelaVendedoresDetalhes.innerHTML = vendedoresLoja.map(v => {
                    let statusBadge;
                    if (v.data_demissao) {
                        statusBadge = `<span class="badge" style="background-color: #ffe3e3; color: #c92a2a;"><i class="bi bi-x-circle-fill me-1"></i>Demitido</span>`;
                    } else if (v.ativo === 1) {
                        statusBadge = `<span class="badge" style="background-color: #c3fae8; color: #087f5b;"><i class="bi bi-check-circle-fill me-1"></i>Ativo</span>`;
                    } else {
                        statusBadge = `<span class="badge" style="background-color: #f1f3f5; color: #495057;"><i class="bi bi-x-circle-fill me-1"></i>Inativo</span>`;
                    }
                    return `<tr>
                        <td>${v.nome}</td>
                        <td>${v.telefone || '-'}</td>
                        <td>${statusBadge}</td>
                        <td class="text-end pe-3">
                            <button class="btn btn-sm" style="background-color: #e9ecef; color: #495057; border: 1px solid #ced4da;" data-action="editar-vendedor" data-id="${v.id}" data-loja-id="${id}">
                                <i class="bi bi-pencil me-1"></i>Editar
                            </button>
                            <button class="btn btn-sm" style="background-color: #ffe3e3; color: #c92a2a; border: 1px solid #ffc9c9;" data-action="excluir-vendedor" data-id="${v.id}" data-loja-id="${id}">
                                <i class="bi bi-trash me-1"></i>Excluir
                            </button>
                        </td>
                    </tr>`;
                }).join('');
            }
            
            // Configurar botão de adicionar vendedor
            const btnAdicionarVendedorDetalhes = document.getElementById('btn-adicionar-vendedor-detalhes');
            btnAdicionarVendedorDetalhes.onclick = () => {
                modalDetalhes.hide();
                abrirModalVendedorParaLoja(id);
            };
            
            modalDetalhes.show();
        } catch (error) {
            showToast('Erro', 'Erro ao carregar vendedores da loja', 'danger');
        }
    }
    
    function abrirModalVendedorParaLoja(lojaId) {
        const loja = lojasCache.find(l => l.id === lojaId);
        if (!loja) return;
        
        const modalVendedor = bootstrap.Modal.getInstance(document.getElementById('modal-vendedor')) || new bootstrap.Modal(document.getElementById('modal-vendedor'));
        const formVendedor = document.getElementById('form-vendedor');
        formVendedor.reset();
        
        document.getElementById('modalVendedorLabel').textContent = `Adicionar Vendedor - ${loja.nome}`;
        document.getElementById('vendedor-id').value = '';
        document.getElementById('vendedor-loja-id').value = lojaId;
        
        modalVendedor.show();
    }

    async function excluirLoja(id) {
        const confirmed = await showConfirmModal(`Tem certeza que deseja excluir esta loja?`);
        if (!confirmed) return;
        try {
            const response = await fetch(`/api/lojas/${id}`, { method: 'DELETE', headers: await getAuthHeaders() });
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
            cep: document.getElementById('loja-cep').value || null,
            numero_contato: document.getElementById('loja-numero-contato').value || null,
            gerente: document.getElementById('loja-gerente').value || null,
            status: document.getElementById('loja-status').value, 
            funcao_especial: document.getElementById('loja-funcao-especial').value || null,
            tecnico_username: document.getElementById('loja-tecnico').value || null,
            observacoes: document.getElementById('loja-observacoes').value || null,
            cargo: null
        };
        const method = id ? 'PUT' : 'POST';
        const url = id ? `/api/lojas/${id}` : '/api/lojas';
        try {
            const response = await fetch(url, { method, headers: await getAuthHeaders(), body: JSON.stringify(data) });
            if (!response.ok) throw new Error('Falha ao salvar. Nome já existe?');
            showToast('Sucesso', `Loja salva com sucesso.`, 'success');
            modal.hide();
            carregarLojas();
        } catch(e) { showToast('Erro', e.message, 'danger'); }
    });

    async function editarVendedor(id, lojaId) {
        try {
            const response = await fetch('/api/vendedores');
            const vendedoresRaw = await response.json();
            // Normalizar IDs para números
            const vendedores = vendedoresRaw.map(v => ({
                ...v,
                id: Number(v.id),
                loja_id: Number(v.loja_id)
            }));
            const vendedor = vendedores.find(v => v.id === id);
            if (!vendedor) return;
            
            const modalVendedor = new bootstrap.Modal(document.getElementById('modal-vendedor'));
            const formVendedor = document.getElementById('form-vendedor');
            
            document.getElementById('modalVendedorLabel').textContent = 'Editar Vendedor';
            document.getElementById('vendedor-id').value = vendedor.id;
            document.getElementById('vendedor-loja-id').value = vendedor.loja_id;
            document.getElementById('vendedor-nome').value = vendedor.nome;
            document.getElementById('vendedor-telefone').value = vendedor.telefone;
            document.getElementById('vendedor-data-entrada').value = vendedor.data_entrada;
            document.getElementById('vendedor-data-demissao').value = vendedor.data_demissao || '';
            document.getElementById('vendedor-previsao-entrada').value = vendedor.previsao_entrada || '';
            document.getElementById('vendedor-previsao-saida').value = vendedor.previsao_saida || '';
            
            // Fechar modal de detalhes
            const modalDetalhes = bootstrap.Modal.getInstance(document.getElementById('modal-detalhes-loja'));
            if (modalDetalhes) modalDetalhes.hide();
            
            modalVendedor.show();
        } catch (error) {
            showToast('Erro', 'Erro ao carregar dados do vendedor', 'danger');
        }
    }
    
    async function excluirVendedor(id, lojaId) {
        const confirmed = await showConfirmModal('Tem certeza que deseja excluir este vendedor?');
        if (!confirmed) return;
        try {
            const response = await fetch(`/api/vendedores/${id}`, { method: 'DELETE', headers: await getAuthHeaders() });
            if (!response.ok) throw new Error('Falha ao excluir.');
            showToast('Sucesso', 'Vendedor excluído.', 'success');
            // Recarregar detalhes
            mostrarDetalhes(lojaId);
        } catch (e) {
            showToast('Erro', 'Não foi possível excluir o vendedor.', 'danger');
        }
    }

    btnAdicionar.addEventListener('click', abrirModalParaAdicionar);
    tableBody.addEventListener('click', (e) => {
        const button = e.target.closest('button[data-action]');
        console.log('Clique detectado:', e.target);
        console.log('Botão encontrado:', button);
        if (!button) {
            console.log('Nenhum botão com data-action encontrado');
            return;
        }
        const id = parseInt(button.dataset.id, 10);
        const action = button.dataset.action;
        console.log('ID:', id, 'Action:', action);
        if (action === 'editar') abrirModalParaEditar(id);
        if (action === 'detalhes') {
            console.log('Chamando mostrarDetalhes com ID:', id);
            mostrarDetalhes(id);
        }
        if (action === 'excluir') excluirLoja(id);
        if (action === 'adicionar-vendedor') abrirModalVendedorParaLoja(id);
    });
    
    // Event listener para botões dentro do modal de detalhes (se existir)
    const modalDetalhesLoja = document.getElementById('modal-detalhes-loja');
    if (modalDetalhesLoja) {
        modalDetalhesLoja.addEventListener('click', (e) => {
            const button = e.target.closest('button[data-action]');
            if (!button) return;
            const id = parseInt(button.getAttribute('data-id'), 10);
            const lojaId = parseInt(button.getAttribute('data-loja-id'), 10);
            const action = button.getAttribute('data-action');
            if (action === 'editar-vendedor') editarVendedor(id, lojaId);
            if (action === 'excluir-vendedor') excluirVendedor(id, lojaId);
        });
    }
    
    // Event listener para o formulário de vendedor
    const formVendedor = document.getElementById('form-vendedor');
    if (formVendedor) {
        formVendedor.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const id = document.getElementById('vendedor-id').value;
            const lojaId = parseInt(document.getElementById('vendedor-loja-id').value, 10);
            const data = {
                loja_id: lojaId,
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
                    headers: await getAuthHeaders(), 
                    body: JSON.stringify(data) 
                });
                
                if (!response.ok) throw new Error('Falha ao salvar vendedor.');
                
                showToast('Sucesso', 'Vendedor salvo com sucesso.', 'success');
                
                // Fechar modal de vendedor
                const modalVendedor = bootstrap.Modal.getInstance(document.getElementById('modal-vendedor'));
                if (modalVendedor) modalVendedor.hide();
                
                // Recarregar tabela de lojas e reabrir modal de detalhes se aplicável
                await carregarLojas();
                if (lojaId) {
                    setTimeout(() => mostrarDetalhes(lojaId), 300);
                }
            } catch (e) {
                showToast('Erro', e.message, 'danger');
            }
        });
    }
    
    carregarTecnicos();
    carregarLojas();
    implementarBusca();
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
            const vendedoresRaw = await response.json();
            // Normalizar IDs para números
            vendedoresCache = vendedoresRaw.map(v => ({
                ...v,
                id: Number(v.id),
                loja_id: Number(v.loja_id)
            }));
            
            if (vendedoresCache.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="8" class="text-center">Nenhum vendedor cadastrado nesta loja.</td></tr>';
                return;
            }
            
            tableBody.innerHTML = vendedoresCache.map(v => {
                let statusBadge;
                if (v.data_demissao) {
                    statusBadge = '<span class="badge" style="background-color: #ffe3e3; color: #c92a2a;">Demitido</span>';
                } else if (v.ativo === 1) {
                    statusBadge = '<span class="badge" style="background-color: #c3fae8; color: #087f5b;">Ativo</span>';
                } else {
                    statusBadge = '<span class="badge" style="background-color: #f1f3f5; color: #495057;">Inativo</span>';
                }
                
                return `<tr>
                    <td>${v.nome}</td>
                    <td>${v.telefone}</td>
                    <td>${formatarData(v.data_entrada)}</td>
                    <td>${v.data_demissao ? formatarData(v.data_demissao) : '-'}</td>
                    <td>${v.previsao_entrada || '-'}</td>
                    <td>${v.previsao_saida || '-'}</td>
                    <td>${statusBadge}</td>
                    <td class="text-end pe-3">
                        <button class="btn btn-sm" style="background-color: #e9ecef; color: #495057; border: 1px solid #ced4da;" data-action="editar" data-id="${v.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-sm" style="background-color: #ffe3e3; color: #c92a2a; border: 1px solid #ffc9c9;" data-action="excluir" data-id="${v.id}"><i class="bi bi-trash"></i></button>
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
            const response = await fetch(`/api/vendedores/${id}`, { method: 'DELETE', headers: await getAuthHeaders() });
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
                headers: await getAuthHeaders(), 
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
