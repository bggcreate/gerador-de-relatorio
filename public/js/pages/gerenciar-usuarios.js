import { showToast, showConfirmModal, getAuthHeaders } from '../utils.js';

export function initGerenciarUsuariosPage(currentUser) {
    initUsuarios(currentUser);
    initBackup();
}

// --- LÓGICA DE GERENCIAMENTO DE USUÁRIOS ---
function initUsuarios(currentUser) {
    const tableBody = document.getElementById('tabela-usuarios-corpo');
    const btnAdicionar = document.getElementById('btn-adicionar-usuario');
    const modalEl = document.getElementById('modal-usuario');
    
    if (!tableBody || !btnAdicionar || !modalEl) return;

    const modal = new bootstrap.Modal(modalEl);
    const modalForm = document.getElementById('form-usuario');
    const modalTitle = document.getElementById('modalUsuarioLabel');
    const roleSelect = document.getElementById('usuario-role');
    const campoLojaGerente = document.getElementById('campo-loja-gerente');
    const campoLojasConsultor = document.getElementById('campo-lojas-consultor');
    const campoLojaTecnico = document.getElementById('campo-loja-tecnico');
    const selectLojaGerente = document.getElementById('usuario-loja-gerente');
    const selectLojasConsultor = document.getElementById('usuario-lojas-consultor');
    const selectLojaTecnico = document.getElementById('usuario-loja-tecnico');
    
    let usuariosCache = [];
    let lojasCache = [];

    const roleNames = {
        'gerente': 'Gerente',
        'consultor': 'Consultor',
        'monitoramento': 'Monitoramento',
        'tecnico': 'Técnico',
        'admin': 'Administrador',
        'dev': 'Desenvolvedor'
    };

    async function carregarLojas() {
        try {
            const response = await fetch('/api/lojas');
            if (!response.ok) throw new Error('Falha ao carregar lojas.');
            lojasCache = await response.json();
            
            // Preencher selects
            selectLojaGerente.innerHTML = '<option value="">Selecione uma loja</option>' + 
                lojasCache.map(loja => `<option value="${loja.nome}">${loja.nome}</option>`).join('');
            
            selectLojasConsultor.innerHTML = 
                lojasCache.map(loja => `<option value="${loja.nome}">${loja.nome}</option>`).join('');
            
            selectLojaTecnico.innerHTML = '<option value="">Selecione uma loja</option>' + 
                lojasCache.map(loja => `<option value="${loja.nome}">${loja.nome}</option>`).join('');
        } catch(e) {
            console.error('Erro ao carregar lojas:', e);
        }
    }

    function mostrarCamposLojas(role) {
        campoLojaGerente.style.display = role === 'gerente' ? 'block' : 'none';
        campoLojasConsultor.style.display = role === 'consultor' ? 'block' : 'none';
        campoLojaTecnico.style.display = role === 'tecnico' ? 'block' : 'none';
        
        // Resetar valores ao mudar de role
        if (role !== 'gerente') selectLojaGerente.value = '';
        if (role !== 'consultor') {
            Array.from(selectLojasConsultor.options).forEach(opt => opt.selected = false);
        }
        if (role !== 'tecnico') selectLojaTecnico.value = '';
    }

    roleSelect.addEventListener('change', (e) => {
        mostrarCamposLojas(e.target.value);
    });

    async function carregarUsuarios() {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Carregando...</td></tr>';
        try {
            const response = await fetch('/api/usuarios');
            if (!response.ok) throw new Error('Falha ao carregar usuários.');
            usuariosCache = await response.json();
            
            if (usuariosCache.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Nenhum usuário cadastrado.</td></tr>';
                return;
            }
            
            tableBody.innerHTML = usuariosCache.map(user => {
                const isCurrentUser = user.id === currentUser.id;
                const deleteButton = isCurrentUser ? 
                    `<button class="btn btn-sm btn-outline-secondary" disabled title="Não é possível excluir o próprio usuário"><i class="bi bi-trash"></i></button>` : 
                    `<button class="btn btn-sm btn-outline-danger" data-action="excluir" data-id="${user.id}" title="Excluir"><i class="bi bi-trash"></i></button>`;
                
                // Determinar lojas vinculadas
                let lojasVinculadas = '-';
                if (user.role === 'gerente' && user.loja_gerente) {
                    lojasVinculadas = `<span class="badge bg-info">${user.loja_gerente}</span>`;
                } else if (user.role === 'consultor' && user.lojas_consultor) {
                    const lojas = user.lojas_consultor.split(',').filter(l => l);
                    lojasVinculadas = lojas.map(l => `<span class="badge bg-info me-1">${l.trim()}</span>`).join('');
                } else if (user.role === 'tecnico' && user.loja_tecnico) {
                    lojasVinculadas = `<span class="badge bg-info">${user.loja_tecnico}</span>`;
                }
                
                const roleBadgeColor = {
                    'dev': 'bg-danger',
                    'admin': 'bg-primary',
                    'monitoramento': 'bg-success',
                    'consultor': 'bg-warning',
                    'gerente': 'bg-secondary',
                    'tecnico': 'bg-secondary'
                };
                
                return `<tr>
                    <td class="ps-3">${user.id}</td>
                    <td>${user.username}</td>
                    <td><span class="badge ${roleBadgeColor[user.role] || 'bg-secondary'}">${roleNames[user.role] || user.role}</span></td>
                    <td>${lojasVinculadas}</td>
                    <td class="text-end pe-3">
                        <button class="btn btn-sm btn-outline-secondary" data-action="editar" data-id="${user.id}" title="Editar"><i class="bi bi-pencil"></i></button>
                        ${deleteButton}
                    </td>
                </tr>`;
            }).join('');
        } catch(e) {
            tableBody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Erro ao carregar usuários.</td></tr>`;
        }
    }

    function abrirModalParaAdicionar() {
        modalForm.reset();
        modalTitle.textContent = 'Adicionar Novo Usuário';
        document.getElementById('usuario-id').value = '';
        document.getElementById('usuario-password').required = true;
        document.getElementById('usuario-password-label').textContent = 'Senha *';
        
        // Admin não pode criar Dev
        const devOption = roleSelect.querySelector('option[value="dev"]');
        if (devOption) {
            devOption.style.display = currentUser.role === 'admin' ? 'none' : 'block';
        }
        
        mostrarCamposLojas('gerente'); // Padrão
        modal.show();
    }

    function abrirModalParaEditar(id) {
        const user = usuariosCache.find(u => u.id === id);
        if (!user) return;
        modalForm.reset();
        modalTitle.textContent = 'Editar Usuário';
        document.getElementById('usuario-id').value = user.id;
        document.getElementById('usuario-username').value = user.username;
        document.getElementById('usuario-role').value = user.role;
        
        // Admin não pode alterar para Dev
        const devOption = roleSelect.querySelector('option[value="dev"]');
        if (devOption) {
            devOption.style.display = currentUser.role === 'admin' ? 'none' : 'block';
        }
        
        // Preencher lojas
        if (user.role === 'gerente' && user.loja_gerente) {
            selectLojaGerente.value = user.loja_gerente;
        }
        if (user.role === 'consultor' && user.lojas_consultor) {
            const lojas = user.lojas_consultor.split(',').map(l => l.trim()).filter(l => l);
            Array.from(selectLojasConsultor.options).forEach(opt => {
                opt.selected = lojas.includes(opt.value);
            });
        }
        if (user.role === 'tecnico' && user.loja_tecnico) {
            selectLojaTecnico.value = user.loja_tecnico;
        }
        
        mostrarCamposLojas(user.role);
        document.getElementById('usuario-password').required = false;
        document.getElementById('usuario-password-label').textContent = 'Nova Senha (deixe em branco para não alterar)';
        modal.show();
    }

    async function excluirUsuario(id) {
        const confirmed = await showConfirmModal(`Tem certeza que deseja excluir o usuário #${id}?`);
        if (!confirmed) return;
        try { 
            const response = await fetch(`/api/usuarios/${id}`, { 
                method: 'DELETE',
                headers: await getAuthHeaders()
            });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error);
            showToast('Sucesso', 'Usuário excluído com sucesso.', 'success');
            carregarUsuarios();
        } catch (e) {
            showToast('Erro', e.message || 'Não foi possível excluir o usuário.', 'danger');
        }
    }
    
    modalForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const id = document.getElementById('usuario-id').value;
        const role = document.getElementById('usuario-role').value;
        
        const data = { 
            username: document.getElementById('usuario-username').value,
            password: document.getElementById('usuario-password').value,
            role: role
        };
        
        // Adicionar lojas baseado no role
        if (role === 'gerente') {
            data.loja_gerente = selectLojaGerente.value || null;
            data.lojas_consultor = '';
            data.loja_tecnico = null;
        } else if (role === 'consultor') {
            data.loja_gerente = null;
            const lojasSelected = Array.from(selectLojasConsultor.selectedOptions).map(opt => opt.value);
            data.lojas_consultor = lojasSelected;
            data.loja_tecnico = null;
        } else if (role === 'tecnico') {
            data.loja_gerente = null;
            data.lojas_consultor = '';
            data.loja_tecnico = selectLojaTecnico.value || null;
        } else {
            data.loja_gerente = null;
            data.lojas_consultor = '';
            data.loja_tecnico = null;
        }
        
        if (!data.password) delete data.password;
        const method = id ? 'PUT' : 'POST';
        const url = id ? `/api/usuarios/${id}` : '/api/usuarios';
        
        try {
            const response = await fetch(url, { 
                method, 
                headers: await getAuthHeaders(), 
                body: JSON.stringify(data) 
            });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error);
            showToast('Sucesso', `Usuário ${id ? 'atualizado' : 'adicionado'} com sucesso.`, 'success');
            modal.hide();
            carregarUsuarios();
        } catch(e) {
            showToast('Erro', e.message || 'Não foi possível salvar o usuário.', 'danger');
        }
    });

    btnAdicionar.addEventListener('click', abrirModalParaAdicionar);
    
    tableBody.addEventListener('click', (e) => {
        const button = e.target.closest('button[data-action]');
        if (!button) return;
        const id = parseInt(button.dataset.id, 10);
        const action = button.dataset.action;
        if (action === 'editar') abrirModalParaEditar(id);
        if (action === 'excluir') excluirUsuario(id);
    });
    
    carregarLojas();
    carregarUsuarios();
}

// --- LÓGICA DE BACKUP E RESTAURAÇÃO ---
function initBackup() {
    const dbSizeSpan = document.getElementById('db-size');
    const btnLimparDb = document.getElementById('btn-limpar-db');
    const btnRestaurarBackup = document.getElementById('btn-restaurar-backup');
    const backupFileInput = document.getElementById('backup-file-input');

    async function carregarInfoBackup() {
        try {
            const response = await fetch('/api/backup/info');
            const data = await response.json();
            dbSizeSpan.textContent = `${data.sizeMB} MB`;
        } catch (error) {
            dbSizeSpan.textContent = 'Erro ao carregar';
            dbSizeSpan.classList.add('text-danger');
        }
    }

    btnLimparDb.addEventListener('click', async () => {
        const confirmed = await showConfirmModal('Você tem CERTEZA ABSOLUTA que deseja limpar todos os relatórios e demandas? Esta ação é IRREVERSÍVEL.');
        if (!confirmed) return;

        btnLimparDb.disabled = true;
        btnLimparDb.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Limpando...';
        try {
            const response = await fetch('/api/backup/clear', { 
                method: 'DELETE',
                headers: await getAuthHeaders()
            });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error);
            showToast('Sucesso', 'Banco de dados limpo com sucesso.', 'success');
            carregarInfoBackup();
        } catch (error) {
            showToast('Erro', error.message, 'danger');
        } finally {
            btnLimparDb.disabled = false;
            btnLimparDb.innerHTML = '<i class="bi bi-trash3-fill me-2"></i>Limpar Relatórios e Demandas';
        }
    });

    backupFileInput.addEventListener('change', () => {
        btnRestaurarBackup.disabled = !backupFileInput.files.length;
    });

    btnRestaurarBackup.addEventListener('click', async () => {
        const file = backupFileInput.files[0];
        if (!file) {
            showToast('Atenção', 'Selecione um arquivo de backup (.db) primeiro.', 'info');
            return;
        }

        const confirmed = await showConfirmModal('Você está prestes a SUBSTITUIR TODO o banco de dados atual. O sistema atual será perdido para sempre. Deseja continuar?');
        if (!confirmed) return;

        const formData = new FormData();
        formData.append('backupFile', file);

        btnRestaurarBackup.disabled = true;
        btnRestaurarBackup.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Restaurando...';

        try {
            const response = await fetch('/api/backup/restore', {
                method: 'POST',
                body: formData
            });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error);
            showToast('Sucesso!', result.message, 'success');
            setTimeout(() => window.location.reload(), 2000);
        } catch (error) {
            showToast('Erro na Restauração', error.message, 'danger');
            btnRestaurarBackup.disabled = false;
            btnRestaurarBackup.innerHTML = '<i class="bi bi-upload me-2"></i>Restaurar';
        }
    });
    
    carregarInfoBackup();
}
