import { getAuthHeaders } from '../utils.js';

export function initAssistenciaPage() {
    if (!window.currentUser) {
        setTimeout(initAssistenciaPage, 100);
        return;
    }
    
    init();
}

let estoquePecas = [];
let assistencias = [];
let assistenciaEdit = null;
let lojaAtiva = null;

let modalPeca, modalDetalhes;

async function init() {
    modalPeca = new bootstrap.Modal(document.getElementById('modal-peca'));
    modalDetalhes = new bootstrap.Modal(document.getElementById('modal-detalhes'));
    
    await carregarLojas();
    await renderCardsLojas();
    await carregarFiltrosLoja();
    
    // Controlar visibilidade das abas baseado no cargo
    controlarVisibilidadeAbas();
    
    // Carregar dados iniciais (todas as lojas para quem tem acesso múltiplo)
    await carregarChamados();
    await carregarHistorico();
    
    // Se já tiver loja ativa (Gerente/Técnico), carregar estoque também
    if (lojaAtiva) {
        await carregarEstoque();
    }
    
    document.getElementById('filter-status').addEventListener('change', carregarChamados);
    document.getElementById('filter-loja-chamados').addEventListener('change', carregarChamados);
    document.getElementById('search-chamados').addEventListener('input', debounce(carregarChamados, 300));
    document.getElementById('search-estoque').addEventListener('input', debounce(carregarEstoque, 300));
    document.getElementById('filter-disponivel').addEventListener('change', carregarEstoque);
    document.getElementById('filter-loja-historico').addEventListener('change', carregarHistorico);
    document.getElementById('search-historico').addEventListener('input', debounce(carregarHistorico, 300));
    
    document.getElementById('btn-adicionar-peca').addEventListener('click', () => {
        document.getElementById('modalPecaLabel').textContent = 'Adicionar Peça';
        document.getElementById('form-peca').reset();
        document.getElementById('peca-id').value = '';
        modalPeca.show();
    });
    
    document.getElementById('form-peca').addEventListener('submit', salvarPeca);
    document.getElementById('form-assistencia').addEventListener('submit', salvarAssistencia);
    
    document.getElementById('btn-cancelar').addEventListener('click', () => {
        assistenciaEdit = null;
        document.getElementById('form-assistencia').reset();
        document.getElementById('assistencia-id').value = '';
        document.getElementById('data-entrada').value = new Date().toISOString().split('T')[0];
    });
    
    document.getElementById('data-entrada').value = new Date().toISOString().split('T')[0];
    
    // Ocultar botão de adicionar peça apenas para técnicos
    if (window.currentUser && window.currentUser.role === 'tecnico') {
        const btnAdicionarPeca = document.getElementById('btn-adicionar-peca');
        if (btnAdicionarPeca) {
            btnAdicionarPeca.style.display = 'none';
        }
    }
    
    document.querySelectorAll('button[data-bs-toggle="tab"]').forEach(tab => {
        tab.addEventListener('shown.bs.tab', (e) => {
            if (e.target.id === 'historico-tab') {
                carregarHistorico();
            }
        });
    });
}

async function carregarLojas() {
    try {
        const response = await fetch('/api/lojas');
        const lojas = await response.json();
        
        const lojaSelect = document.getElementById('loja-assist');
        
        // Se for técnico, mostrar apenas a loja dele
        if (window.currentUser && window.currentUser.role === 'tecnico' && window.currentUser.loja_tecnico) {
            lojaSelect.innerHTML = `<option value="${window.currentUser.loja_tecnico}" selected>${window.currentUser.loja_tecnico}</option>`;
            lojaSelect.disabled = true;
            lojaSelect.title = 'Técnicos só podem criar assistências para sua loja cadastrada';
        } else if (window.currentUser && window.currentUser.role === 'gerente' && window.currentUser.loja_gerente) {
            // Gerente vê apenas sua loja
            lojaSelect.innerHTML = `<option value="${window.currentUser.loja_gerente}" selected>${window.currentUser.loja_gerente}</option>`;
            lojaSelect.disabled = true;
            lojaSelect.title = 'Gerentes só podem criar assistências para sua loja';
        } else if (lojaAtiva) {
            // Se houver loja ativa (selecionada via card), pré-selecionar e desabilitar
            lojaSelect.innerHTML = `<option value="${lojaAtiva}" selected>${lojaAtiva}</option>`;
            lojaSelect.disabled = true;
            lojaSelect.title = 'Loja pré-selecionada. Clique em Voltar para trocar de loja';
        } else {
            // Admin/Dev/Consultor/Monitoramento sem loja selecionada veem todas as lojas
            lojaSelect.innerHTML = '<option value="">Selecione uma loja...</option>' + 
                lojas.map(loja => `<option value="${loja.nome}">${loja.nome}</option>`).join('');
            lojaSelect.disabled = false;
        }
    } catch (error) {
        console.error('Erro ao carregar lojas:', error);
    }
}

async function carregarFiltrosLoja() {
    // Mostrar filtro de loja apenas para cargos que veem múltiplas lojas
    const role = window.currentUser.role;
    const showFilter = ['consultor', 'admin', 'dev', 'monitoramento'].includes(role);
    
    if (showFilter) {
        try {
            const response = await fetch('/api/lojas');
            const lojas = await response.json();
            
            // Preencher filtro de chamados
            const filterChamados = document.getElementById('filter-loja-chamados');
            filterChamados.innerHTML = '<option value="">Todas as Lojas</option>' + 
                lojas.map(loja => `<option value="${loja.nome}">${loja.nome}</option>`).join('');
            filterChamados.style.display = 'block';
            
            // Preencher filtro de histórico
            const filterHistorico = document.getElementById('filter-loja-historico');
            filterHistorico.innerHTML = '<option value="">Todas as Lojas</option>' + 
                lojas.map(loja => `<option value="${loja.nome}">${loja.nome}</option>`).join('');
            filterHistorico.style.display = 'block';
        } catch (error) {
            console.error('Erro ao carregar filtros de loja:', error);
        }
    }
}

async function renderCardsLojas() {
    const role = window.currentUser.role;
    
    // Gerente e Técnico: auto-selecionar sua loja
    if (role === 'gerente' && window.currentUser.loja_gerente) {
        lojaAtiva = window.currentUser.loja_gerente;
        return;
    }
    if (role === 'tecnico' && window.currentUser.loja_tecnico) {
        lojaAtiva = window.currentUser.loja_tecnico;
        return;
    }
    
    // Consultor, Admin, Dev, Monitoramento: mostrar cards
    if (['consultor', 'admin', 'dev', 'monitoramento'].includes(role)) {
        try {
            const response = await fetch('/api/lojas');
            const lojas = await response.json();
            
            console.log('Total de lojas retornadas:', lojas.length);
            console.log('Lojas:', lojas);
            
            // Filtrar apenas lojas com funcao_especial "Busca por Assist. Tec." OU todas as lojas ativas
            // Mudança: Agora mostra TODAS as lojas ativas para facilitar acesso à assistência técnica
            const lojasFiltradas = lojas.filter(loja => loja.status === 'ativa' || loja.status === 'Ativa');
            
            console.log('Lojas filtradas (ativas):', lojasFiltradas.length);
            console.log('Lojas filtradas:', lojasFiltradas);
            
            const seletorLojas = document.getElementById('seletor-lojas');
            const cardsContainer = document.getElementById('cards-lojas');
            
            if (!seletorLojas || !cardsContainer) {
                console.error('Elementos seletor-lojas ou cards-lojas não encontrados!');
                return;
            }
            
            seletorLojas.style.display = 'block';
            
            if (lojasFiltradas.length === 0) {
                cardsContainer.innerHTML = '<div class="col-12"><p class="text-muted text-center">Nenhuma loja ativa encontrada.</p></div>';
            } else {
                cardsContainer.innerHTML = lojasFiltradas.map(loja => `
                    <div class="col-md-3">
                        <div class="card loja-card h-100" data-loja="${loja.nome}" onclick="window.selecionarLoja('${loja.nome}')" style="cursor: pointer; transition: all 0.2s;">
                            <div class="card-body text-center">
                                <h5 class="card-title">${loja.nome}</h5>
                                <p class="text-muted small mb-0">${loja.status}</p>
                            </div>
                        </div>
                    </div>
                `).join('');
            }
        } catch (error) {
            console.error('Erro ao renderizar cards de lojas:', error);
        }
    }
}

window.selecionarLoja = async function(nomeLoja) {
    lojaAtiva = nomeLoja;
    
    // Esconder todos os cards
    document.getElementById('seletor-lojas').style.display = 'none';
    
    // Mostrar título da loja selecionada
    document.getElementById('titulo-loja-selecionada').style.display = 'block';
    document.getElementById('texto-loja-selecionada').textContent = `Assistência ${nomeLoja}`;
    
    // Mostrar todas as abas
    controlarVisibilidadeAbas();
    
    // Atualizar campo Loja no formulário
    await carregarLojas();
    
    // Recarregar dados filtrados pela loja
    await carregarEstoque();
    await carregarChamados();
    await carregarHistorico();
    
    showToast(`Loja selecionada: ${nomeLoja}`, 'success');
};

window.voltarParaTodasLojas = async function() {
    lojaAtiva = null;
    
    // Esconder título da loja selecionada
    document.getElementById('titulo-loja-selecionada').style.display = 'none';
    
    // Mostrar cards novamente
    document.getElementById('seletor-lojas').style.display = 'block';
    
    // Limpar seleção visual dos cards
    document.querySelectorAll('.loja-card').forEach(card => {
        card.classList.remove('border-primary', 'bg-primary', 'bg-opacity-10');
    });
    
    // Esconder abas de Nova Assistência e Estoque
    controlarVisibilidadeAbas();
    
    // Atualizar campo Loja no formulário
    await carregarLojas();
    
    // Voltar para aba de Chamados
    document.getElementById('chamados-tab').click();
    
    // Recarregar dados de todas as lojas
    await carregarChamados();
    await carregarHistorico();
    
    showToast('Visualizando todas as lojas', 'info');
};

function controlarVisibilidadeAbas() {
    const role = window.currentUser.role;
    
    // Para cargos com acesso a múltiplas lojas
    if (['consultor', 'admin', 'dev', 'monitoramento'].includes(role)) {
        const cadastroTab = document.querySelector('#cadastro-tab').parentElement;
        const estoqueTab = document.querySelector('#estoque-tab').parentElement;
        
        if (lojaAtiva) {
            // Com loja selecionada: mostrar todas as abas
            cadastroTab.style.display = 'block';
            estoqueTab.style.display = 'block';
        } else {
            // Sem loja selecionada: esconder Nova Assistência e Estoque
            cadastroTab.style.display = 'none';
            estoqueTab.style.display = 'none';
        }
    }
    // Gerente e Técnico sempre veem todas as abas (loja já está definida)
};

async function carregarEstoque() {
    if (!lojaAtiva) {
        document.getElementById('tabela-estoque').innerHTML = '<tr><td colspan="6" class="text-center text-muted">Selecione uma loja para ver o estoque</td></tr>';
        return;
    }
    
    const search = document.getElementById('search-estoque').value;
    const disponivel = document.getElementById('filter-disponivel').checked;
    
    try {
        const params = new URLSearchParams({ search, disponivel, loja: lojaAtiva });
        const response = await fetch(`/api/estoque-tecnico?${params.toString()}`);
        estoquePecas = await response.json();
        
        atualizarSelectPecas();
        renderEstoque();
    } catch (error) {
        showToast('Erro ao carregar estoque', 'error');
    }
}

function renderEstoque() {
    const tbody = document.getElementById('tabela-estoque');
    
    if (estoquePecas.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="text-center">Nenhuma peça encontrada</td></tr>';
        return;
    }
    
    const isReadOnly = window.currentUser && window.currentUser.role === 'tecnico';
    
    tbody.innerHTML = estoquePecas.map(peca => `
        <tr>
            <td>${peca.nome_peca}</td>
            <td><code>${peca.codigo_interno}</code></td>
            <td>
                <span class="badge bg-${peca.quantidade > 5 ? 'success' : peca.quantidade > 0 ? 'warning' : 'danger'}">
                    ${peca.quantidade}
                </span>
            </td>
            <td>R$ ${parseFloat(peca.valor_custo).toFixed(2)}</td>
            <td>
                ${peca.quantidade > 0 
                    ? '<span class="badge bg-success">Disponível</span>' 
                    : '<span class="badge bg-danger">Esgotado</span>'}
            </td>
            <td class="text-end pe-3">
                ${!isReadOnly ? `
                    <button class="btn btn-sm btn-outline-primary" onclick="window.editarPeca(${peca.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="window.deletarPeca(${peca.id})">
                        <i class="bi bi-trash"></i>
                    </button>
                ` : '<span class="text-muted small">Somente leitura</span>'}
            </td>
        </tr>
    `).join('');
}

function atualizarSelectPecas() {
    const select = document.getElementById('peca-select');
    select.innerHTML = '<option value="">Selecione uma peça...</option>' + 
        estoquePecas
            .filter(p => p.quantidade > 0)
            .map(p => `<option value="${p.id}" data-nome="${p.nome_peca}">${p.nome_peca} (${p.quantidade} disponíveis)</option>`)
            .join('');
}

async function salvarPeca(e) {
    e.preventDefault();
    
    if (!lojaAtiva) {
        showToast('Selecione uma loja antes de adicionar peças', 'error');
        return;
    }
    
    const id = document.getElementById('peca-id').value;
    const data = {
        nome_peca: document.getElementById('peca-nome').value,
        codigo_interno: document.getElementById('peca-codigo').value,
        quantidade: parseInt(document.getElementById('peca-quantidade').value),
        valor_custo: parseFloat(document.getElementById('peca-valor').value),
        loja: lojaAtiva
    };
    
    try {
        const url = id ? `/api/estoque-tecnico/${id}` : '/api/estoque-tecnico';
        const method = id ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method,
            headers: await getAuthHeaders(),
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (response.ok) {
            showToast(id ? 'Peça atualizada!' : 'Peça adicionada!', 'success');
            modalPeca.hide();
            await carregarEstoque();
        } else {
            showToast(result.error || 'Erro ao salvar peça', 'error');
        }
    } catch (error) {
        showToast('Erro ao salvar peça', 'error');
    }
}

window.editarPeca = async function(id) {
    const peca = estoquePecas.find(p => p.id === id);
    if (!peca) return;
    
    document.getElementById('modalPecaLabel').textContent = 'Editar Peça';
    document.getElementById('peca-id').value = peca.id;
    document.getElementById('peca-nome').value = peca.nome_peca;
    document.getElementById('peca-codigo').value = peca.codigo_interno;
    document.getElementById('peca-quantidade').value = peca.quantidade;
    document.getElementById('peca-valor').value = peca.valor_custo;
    
    modalPeca.show();
};

window.deletarPeca = async function(id) {
    if (!confirm('Tem certeza que deseja remover esta peça do estoque?')) return;
    
    try {
        const response = await fetch(`/api/estoque-tecnico/${id}`, { method: 'DELETE', headers: await getAuthHeaders() });
        
        if (response.ok) {
            showToast('Peça removida do estoque', 'success');
            await carregarEstoque();
        } else {
            showToast('Erro ao remover peça', 'error');
        }
    } catch (error) {
        showToast('Erro ao remover peça', 'error');
    }
};

async function carregarChamados() {
    const status = document.getElementById('filter-status').value;
    const search = document.getElementById('search-chamados').value;
    const loja = document.getElementById('filter-loja-chamados').value;
    
    try {
        const params = new URLSearchParams({ status, search });
        
        // Se houver loja ativa, filtrar por ela (ignora filtro manual)
        if (lojaAtiva) {
            params.append('loja', lojaAtiva);
        } else if (loja) {
            // Se não houver loja ativa, usar filtro manual
            params.append('loja', loja);
        }
        // Se não houver loja ativa nem filtro, carrega de todas as lojas
        
        const response = await fetch(`/api/assistencias?${params.toString()}`);
        assistencias = await response.json();
        
        renderChamados();
    } catch (error) {
        showToast('Erro ao carregar chamados', 'error');
    }
}

function renderChamados() {
    const tbody = document.getElementById('tabela-chamados');
    
    if (assistencias.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" class="text-center">Nenhum chamado encontrado</td></tr>';
        return;
    }
    
    const canDelete = window.currentUser && ['gerente', 'consultor', 'admin', 'dev'].includes(window.currentUser.role);
    const canEdit = window.currentUser && ['tecnico', 'gerente', 'consultor', 'admin', 'dev'].includes(window.currentUser.role);
    
    tbody.innerHTML = assistencias.map(assist => `
        <tr>
            <td>${assist.cliente_nome}</td>
            <td>${formatCPF(assist.cliente_cpf)}</td>
            <td>${assist.numero_pedido || '-'}</td>
            <td>${assist.aparelho}</td>
            <td>
                <span class="badge bg-${getStatusColor(assist.status)}">
                    ${assist.status}
                </span>
            </td>
            <td>${formatDate(assist.data_entrada)}</td>
            <td class="text-end pe-3">
                <button class="btn btn-sm btn-outline-info" onclick="window.verDetalhes(${assist.id})">
                    <i class="bi bi-eye"></i>
                </button>
                ${canEdit ? `
                    <button class="btn btn-sm btn-outline-primary" onclick="window.editarAssistencia(${assist.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    ${assist.status !== 'Concluído' ? `
                        <button class="btn btn-sm btn-outline-success" onclick="window.concluirAssistencia(${assist.id})">
                            <i class="bi bi-check-circle"></i>
                        </button>
                    ` : ''}
                ` : ''}
                ${canDelete && assist.status === 'Concluído' ? `
                    <button class="btn btn-sm btn-outline-danger" onclick="window.deletarAssistencia(${assist.id})">
                        <i class="bi bi-trash"></i>
                    </button>
                ` : ''}
            </td>
        </tr>
    `).join('');
}

async function salvarAssistencia(e) {
    e.preventDefault();
    
    const pecaSelect = document.getElementById('peca-select');
    const pecaOption = pecaSelect.selectedOptions[0];
    
    const id = document.getElementById('assistencia-id').value;
    const data = {
        cliente_nome: document.getElementById('cliente-nome').value,
        cliente_cpf: document.getElementById('cliente-cpf').value.replace(/\D/g, ''),
        numero_pedido: document.getElementById('numero-pedido').value,
        data_entrada: document.getElementById('data-entrada').value,
        valor_peca_loja: parseFloat(document.getElementById('valor-peca').value) || 0,
        valor_servico_cliente: parseFloat(document.getElementById('valor-servico').value) || 0,
        aparelho: document.getElementById('aparelho').value,
        peca_id: pecaSelect.value || null,
        peca_nome: pecaOption ? pecaOption.dataset.nome : null,
        observacoes: document.getElementById('observacoes').value,
        status: document.getElementById('status-select').value,
        loja: document.getElementById('loja-assist').value
    };
    
    if (id) {
        data.data_conclusao = assistenciaEdit?.data_conclusao || null;
    }
    
    try {
        const url = id ? `/api/assistencias/${id}` : '/api/assistencias';
        const method = id ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method,
            headers: await getAuthHeaders(),
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (response.ok) {
            showToast(id ? 'Assistência atualizada!' : 'Assistência cadastrada!', 'success');
            document.getElementById('form-assistencia').reset();
            document.getElementById('assistencia-id').value = '';
            document.getElementById('data-entrada').value = new Date().toISOString().split('T')[0];
            assistenciaEdit = null;
            
            await carregarChamados();
            
            const tab = new bootstrap.Tab(document.getElementById('chamados-tab'));
            tab.show();
        } else {
            showToast(result.error || 'Erro ao salvar assistência', 'error');
        }
    } catch (error) {
        showToast('Erro ao salvar assistência', 'error');
    }
}

window.editarAssistencia = async function(id) {
    const assist = assistencias.find(a => a.id === id);
    if (!assist) return;
    
    assistenciaEdit = assist;
    
    document.getElementById('assistencia-id').value = assist.id;
    document.getElementById('cliente-nome').value = assist.cliente_nome;
    document.getElementById('cliente-cpf').value = formatCPF(assist.cliente_cpf);
    document.getElementById('numero-pedido').value = assist.numero_pedido || '';
    document.getElementById('data-entrada').value = assist.data_entrada;
    document.getElementById('valor-peca').value = assist.valor_peca_loja;
    document.getElementById('valor-servico').value = assist.valor_servico_cliente;
    document.getElementById('aparelho').value = assist.aparelho;
    document.getElementById('peca-select').value = assist.peca_id || '';
    document.getElementById('observacoes').value = assist.observacoes || '';
    document.getElementById('status-select').value = assist.status;
    document.getElementById('loja-assist').value = assist.loja || '';
    
    const tab = new bootstrap.Tab(document.getElementById('cadastro-tab'));
    tab.show();
};

window.concluirAssistencia = async function(id) {
    if (!confirm('Deseja marcar esta assistência como concluída?')) return;
    
    try {
        const response = await fetch(`/api/assistencias/${id}/concluir`, { method: 'POST', headers: await getAuthHeaders() });
        const result = await response.json();
        
        if (response.ok) {
            showToast(result.message, 'success');
            await carregarChamados();
        } else {
            showToast(result.error || 'Erro ao concluir assistência', 'error');
        }
    } catch (error) {
        showToast('Erro ao concluir assistência', 'error');
    }
};

window.deletarAssistencia = async function(id) {
    if (!confirm('Tem certeza que deseja excluir esta assistência?')) return;
    
    try {
        const response = await fetch(`/api/assistencias/${id}`, { method: 'DELETE', headers: await getAuthHeaders() });
        const result = await response.json();
        
        if (response.ok) {
            showToast('Assistência removida', 'success');
            await carregarChamados();
        } else {
            showToast(result.error || 'Erro ao remover assistência', 'error');
        }
    } catch (error) {
        showToast('Erro ao remover assistência', 'error');
    }
};

window.verDetalhes = function(id) {
    const assist = assistencias.find(a => a.id === id);
    if (!assist) return;
    
    const valorTotal = (parseFloat(assist.valor_peca_loja) || 0) + (parseFloat(assist.valor_servico_cliente) || 0);
    
    document.getElementById('detalhes-content').innerHTML = `
        <div class="row">
            <div class="col-md-6">
                <h6>Informações do Cliente</h6>
                <p><strong>Nome:</strong> ${assist.cliente_nome}</p>
                <p><strong>CPF:</strong> ${formatCPF(assist.cliente_cpf)}</p>
                <p><strong>Nº Pedido:</strong> ${assist.numero_pedido || '-'}</p>
            </div>
            <div class="col-md-6">
                <h6>Informações da Assistência</h6>
                <p><strong>Aparelho:</strong> ${assist.aparelho}</p>
                <p><strong>Peça:</strong> ${assist.peca_nome || 'Nenhuma'}</p>
                <p><strong>Loja:</strong> ${assist.loja || '-'}</p>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-6">
                <h6>Datas</h6>
                <p><strong>Entrada:</strong> ${formatDate(assist.data_entrada)}</p>
                <p><strong>Conclusão:</strong> ${assist.data_conclusao ? formatDate(assist.data_conclusao) : '-'}</p>
            </div>
            <div class="col-md-6">
                <h6>Valores</h6>
                <p><strong>Peça (Loja):</strong> R$ ${parseFloat(assist.valor_peca_loja).toFixed(2)}</p>
                <p><strong>Serviço (Cliente):</strong> R$ ${parseFloat(assist.valor_servico_cliente).toFixed(2)}</p>
                <p><strong>Total:</strong> <strong>R$ ${valorTotal.toFixed(2)}</strong></p>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-12">
                <h6>Status</h6>
                <p><span class="badge bg-${getStatusColor(assist.status)}">${assist.status}</span></p>
            </div>
        </div>
        ${assist.observacoes ? `
            <div class="row mt-3">
                <div class="col-12">
                    <h6>Observações</h6>
                    <p>${assist.observacoes}</p>
                </div>
            </div>
        ` : ''}
        <div class="row mt-3">
            <div class="col-12">
                <p class="text-muted small mb-0">
                    <strong>Técnico:</strong> ${assist.tecnico_responsavel || '-'}<br>
                    <strong>Criado em:</strong> ${new Date(assist.created_at).toLocaleString('pt-BR')}
                </p>
            </div>
        </div>
    `;
    
    modalDetalhes.show();
};

async function carregarHistorico() {
    const search = document.getElementById('search-historico').value;
    const loja = document.getElementById('filter-loja-historico').value;
    
    try {
        const params = new URLSearchParams({ search });
        
        // Se houver loja ativa, filtrar por ela (ignora filtro manual)
        if (lojaAtiva) {
            params.append('loja', lojaAtiva);
        } else if (loja) {
            // Se não houver loja ativa, usar filtro manual
            params.append('loja', loja);
        }
        // Se não houver loja ativa nem filtro, carrega de todas as lojas
        
        const response = await fetch(`/api/assistencias/historico?${params.toString()}`);
        const historico = await response.json();
        
        renderHistorico(historico);
    } catch (error) {
        showToast('Erro ao carregar histórico', 'error');
    }
}

function renderHistorico(historico) {
    const tbody = document.getElementById('tabela-historico');
    
    if (historico.length === 0) {
        tbody.innerHTML = '<tr><td colspan="9" class="text-center">Nenhuma assistência encontrada</td></tr>';
        return;
    }
    
    const canDelete = window.currentUser && ['gerente', 'consultor', 'admin', 'dev'].includes(window.currentUser.role);
    
    tbody.innerHTML = historico.map(assist => {
        const dataAtualizacao = assist.data_saida || assist.data_conclusao || assist.created_at;
        return `
            <tr>
                <td>${assist.cliente_nome}</td>
                <td>${formatCPF(assist.cliente_cpf)}</td>
                <td>${assist.numero_pedido || '-'}</td>
                <td>${assist.loja || '-'}</td>
                <td>${assist.tecnico_responsavel || '-'}</td>
                <td>
                    <span class="badge bg-${getStatusColor(assist.status)}">
                        ${assist.status}
                    </span>
                </td>
                <td>${formatDate(assist.data_entrada)}</td>
                <td>${formatDateTime(dataAtualizacao)}</td>
                <td class="text-end pe-3">
                    <button class="btn btn-sm btn-outline-info" onclick="window.verDetalhesHistorico(${assist.id})" title="Ver Detalhes">
                        <i class="bi bi-eye"></i>
                    </button>
                    ${canDelete && assist.status === 'Concluído' ? `
                        <button class="btn btn-sm btn-outline-danger" onclick="window.deletarAssistencia(${assist.id})" title="Excluir">
                            <i class="bi bi-trash"></i>
                        </button>
                    ` : ''}
                </td>
            </tr>
        `;
    }).join('');
}

window.verDetalhesHistorico = async function(id) {
    try {
        const response = await fetch(`/api/assistencias?status=Concluído`);
        const historico = await response.json();
        const assist = historico.find(a => a.id === id);
        
        if (assist) {
            assistencias.push(assist);
            window.verDetalhes(id);
        }
    } catch (error) {
        showToast('Erro ao carregar detalhes', 'error');
    }
};

function getStatusColor(status) {
    const colors = {
        'Em andamento': 'primary',
        'Falta de peças': 'warning',
        'Concluído': 'success'
    };
    return colors[status] || 'secondary';
}

function formatCPF(cpf) {
    if (!cpf) return '';
    const cleaned = cpf.replace(/\D/g, '');
    return cleaned.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
}

function formatDate(dateStr) {
    if (!dateStr) return '-';
    const [year, month, day] = dateStr.split('-');
    return `${day}/${month}/${year}`;
}

function formatDateTime(dateTimeStr) {
    if (!dateTimeStr) return '-';
    try {
        const date = new Date(dateTimeStr);
        return date.toLocaleString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    } catch (e) {
        return '-';
    }
}

function debounce(func, wait) {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
}

function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type === 'error' ? 'danger' : type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        document.body.appendChild(container);
    }
    
    container.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    toast.addEventListener('hidden.bs.toast', () => toast.remove());
}
