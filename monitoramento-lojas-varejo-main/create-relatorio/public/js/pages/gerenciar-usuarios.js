import { showToast, showConfirmModal } from '../utils.js';

export function initGerenciarUsuariosPage(currentUser) {
    // A função agora inicializa tanto usuários quanto backup
    initUsuarios(currentUser);
    initBackup();
}

// --- LÓGICA DE GERENCIAMENTO DE USUÁRIOS (Sem alterações) ---
function initUsuarios(currentUser) {
    const tableBody = document.getElementById('tabela-usuarios-corpo');
    const btnAdicionar = document.getElementById('btn-adicionar-usuario');
    const modalEl = document.getElementById('modal-usuario');
    
    if (!tableBody || !btnAdicionar || !modalEl) return;

    const modal = new bootstrap.Modal(modalEl);
    const modalForm = document.getElementById('form-usuario');
    const modalTitle = document.getElementById('modalUsuarioLabel');
    let usuariosCache = [];

    async function carregarUsuarios() {
        tableBody.innerHTML = '<tr><td colspan="4" class="text-center">Carregando...</td></tr>';
        try {
            const response = await fetch('/api/usuarios');
            if (!response.ok) throw new Error('Falha ao carregar usuários.');
            usuariosCache = await response.json();
            
            if (usuariosCache.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="4" class="text-center">Nenhum usuário cadastrado.</td></tr>';
                return;
            }
            tableBody.innerHTML = usuariosCache.map(user => {
                const isCurrentUser = user.id === currentUser.id;
                const deleteButton = isCurrentUser ? `<button class="btn btn-sm btn-outline-secondary" disabled title="Não é possível excluir o próprio usuário"><i class="bi bi-trash"></i></button>` : `<button class="btn btn-sm btn-outline-danger" data-action="excluir" data-id="${user.id}" title="Excluir"><i class="bi bi-trash"></i></button>`;
                return `<tr><td class="ps-3">${user.id}</td><td>${user.username}</td><td><span class="badge ${user.role === 'admin' ? 'bg-primary' : 'bg-secondary'}">${user.role}</span></td><td class="text-end pe-3"><button class="btn btn-sm btn-outline-secondary" data-action="editar" data-id="${user.id}" title="Editar"><i class="bi bi-pencil"></i></button> ${deleteButton}</td></tr>`;
            }).join('');
        } catch(e) {
            tableBody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">Erro ao carregar usuários.</td></tr>`;
        }
    }

    function abrirModalParaAdicionar() {
        modalForm.reset();
        modalTitle.textContent = 'Adicionar Novo Usuário';
        document.getElementById('usuario-id').value = '';
        document.getElementById('usuario-password').required = true;
        document.getElementById('usuario-password-label').textContent = 'Senha *';
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
        document.getElementById('usuario-password').required = false;
        document.getElementById('usuario-password-label').textContent = 'Nova Senha (deixe em branco para não alterar)';
        modal.show();
    }

    async function excluirUsuario(id) {
        const confirmed = await showConfirmModal(`Tem certeza que deseja excluir o usuário #${id}?`);
        if (!confirmed) return;
        try { 
            const response = await fetch(`/api/usuarios/${id}`, { method: 'DELETE' });
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
        const data = { username: document.getElementById('usuario-username').value, password: document.getElementById('usuario-password').value, role: document.getElementById('usuario-role').value };
        if (!data.password) delete data.password;
        const method = id ? 'PUT' : 'POST';
        const url = id ? `/api/usuarios/${id}` : '/api/usuarios';
        try {
            const response = await fetch(url, { method, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
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
    
    carregarUsuarios();
}

// --- LÓGICA DE BACKUP E RESTAURAÇÃO (NOVO) ---
function initBackup() {
    const dbSizeSpan = document.getElementById('db-size');
    const btnLimparDb = document.getElementById('btn-limpar-db');
    const btnRestaurarBackup = document.getElementById('btn-restaurar-backup');
    const backupFileInput = document.getElementById('backup-file-input');

    // Carrega o tamanho do DB ao entrar na aba
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

    // Limpar DB
    btnLimparDb.addEventListener('click', async () => {
        const confirmed = await showConfirmModal('Você tem CERTEZA ABSOLUTA que deseja limpar todos os relatórios e demandas? Esta ação é IRREVERSÍVEL.');
        if (!confirmed) return;

        btnLimparDb.disabled = true;
        btnLimparDb.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Limpando...';
        try {
            const response = await fetch('/api/backup/clear', { method: 'DELETE' });
            const result = await response.json();
            if (!response.ok) throw new Error(result.error);
            showToast('Sucesso', 'Banco de dados limpo com sucesso.', 'success');
            carregarInfoBackup(); // Atualiza o tamanho
        } catch (error) {
            showToast('Erro', error.message, 'danger');
        } finally {
            btnLimparDb.disabled = false;
            btnLimparDb.innerHTML = '<i class="bi bi-trash3-fill me-2"></i>Limpar Relatórios e Demandas';
        }
    });

    // Habilita/Desabilita o botão de restaurar conforme um arquivo é selecionado
    backupFileInput.addEventListener('change', () => {
        btnRestaurarBackup.disabled = !backupFileInput.files.length;
    });

    // Restaurar Backup
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
            // Recarrega a página inteira para garantir que o sistema use o novo DB
            setTimeout(() => window.location.reload(), 2000);
        } catch (error) {
            showToast('Erro na Restauração', error.message, 'danger');
            btnRestaurarBackup.disabled = false;
            btnRestaurarBackup.innerHTML = '<i class="bi bi-upload me-2"></i>Restaurar';
        }
    });
    
    // Carrega as informações iniciais ao carregar a página
    carregarInfoBackup();
}