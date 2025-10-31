// =================================================================
// IMPORTS E CONFIGURAÇÃO DE PÁGINAS
// =================================================================
import { initAdminPage } from './pages/admin.js';
import { initConsultaPage } from './pages/consulta.js';
import { initGerenciarLojasPage } from './pages/gerenciar-lojas.js';
import { initDemandasPage } from './pages/demandas.js';
import { initAssistenciaPage } from './pages/assistencia.js';
import { initAlertasTecnicoPage } from './pages/alertas-tecnico.js';
import { initNovoRelatorioPage } from './pages/novo-relatorio.js';
import { initGerenciarUsuariosPage } from './pages/gerenciar-usuarios.js';
import { initLogsPage } from './pages/logs.js';
import { initPageAnimations, initScrollAnimations } from './utils.js';

const pageInitializers = {
    'admin': initAdminPage,
    'consulta': initConsultaPage,
    'gerenciar-lojas': initGerenciarLojasPage,
    'demandas': initDemandasPage,
    'assistencia': initAssistenciaPage,
    'alertas-tecnico': initAlertasTecnicoPage,
    'novo-relatorio': initNovoRelatorioPage,
    'gerenciar-usuarios': initGerenciarUsuariosPage,
    'logs': initLogsPage
};

let currentUser = null;

// =================================================================
// LÓGICA DE NAVEGAÇÃO E CARREGAMENTO DE PÁGINAS
// =================================================================
async function loadPage(path) {
    const pageContent = document.getElementById('page-content');
    if (!pageContent) return;

    // Definir página padrão baseada no role
    let defaultPage = 'admin';
    if (currentUser && currentUser.role === 'tecnico') {
        defaultPage = 'alertas-tecnico';
    }
    
    const pageName = (path.startsWith('/') ? path.substring(1) : path).split('?')[0] || defaultPage;
    const activePage = (pageName === '' || pageName === 'index.html') ? defaultPage : pageName;

    // Atualiza a classe 'active' nos menus (desktop e mobile)
    document.querySelectorAll('.sidebar-nav .nav-item').forEach(item => {
        const link = item.querySelector('.nav-link');
        const linkHrefPage = link.getAttribute('href').substring(1);
        item.classList.toggle('active', linkHrefPage === activePage);
    });

    pageContent.innerHTML = '<div class="d-flex justify-content-center p-5"><div class="spinner-border" role="status"></div></div>';

    try {
        const response = await fetch(`/content/${activePage}`);
        if (!response.ok) throw new Error(`Página /content/${activePage} não encontrada.`);

        pageContent.innerHTML = await response.text();
        
        // Inicializar animações de scroll para os novos elementos
        setTimeout(() => initScrollAnimations(), 50);
        
        // Garante que a função de inicialização da página seja chamada
        const initFunc = pageInitializers[activePage];
        if (typeof initFunc === 'function') {
           
            setTimeout(() => {
                try {
                    
                    initFunc(currentUser);
                } catch (err) {
                    console.error(`Erro ao inicializar a página '${activePage}':`, err);
                }
            }, 0);
        }
    } catch (error) {
        console.error("Erro ao carregar página:", error);
        pageContent.innerHTML = `<div class="p-3 text-center text-danger"><h3>Oops!</h3><p>Erro ao carregar conteúdo da página.</p></div>`;
    }
}

function navigateTo(path) {
    if (location.pathname + location.search === path) return;
    history.pushState(null, '', path);
    loadPage(path);
}

// =================================================================
// SESSÃO E INICIALIZAÇÃO PRINCIPAL
// =================================================================
async function setupSessionAndUI() {
    try {
        const response = await fetch('/api/session-info');
        if (!response.ok) { window.location.href = '/login'; return; }
        currentUser = await response.json();
        window.currentUser = currentUser; // Expor para outras páginas
        const permissions = currentUser.permissions || {};
        
        // Controlar visibilidade dos itens do menu baseado nas permissões
        const menuVisibility = {
            'nav-alertas': permissions['alertas-tecnico'] || false,
            'nav-dashboard': permissions.dashboard || false,
            'nav-consulta': permissions.consulta || false,
            'nav-novo-relatorio': permissions['novo-relatorio'] || false,
            'nav-lojas': permissions.lojas || false,
            'nav-demandas': permissions.demandas || false,
            'nav-assistencia': permissions.assistencia || false,
            'nav-configuracoes': permissions['gerenciar-usuarios'] || false,
            'nav-logs': permissions.logs || false
        };
        
        // Aplicar visibilidade
        Object.keys(menuVisibility).forEach(menuId => {
            document.querySelectorAll(`#${menuId}`).forEach(el => {
                if (menuVisibility[menuId]) {
                    el?.classList.remove('d-none');
                } else {
                    el?.classList.add('d-none');
                }
            });
        });
        
        const userInfoContainer = document.getElementById('user-info-container');
        if (userInfoContainer) {
            let actionButtons = '';
            
            // Botão Novo Relatório apenas para monitoramento, admin e dev
            if (['monitoramento', 'admin', 'dev'].includes(currentUser.role)) {
                actionButtons += `<a href="/novo-relatorio" id="live-mode-btn" class="btn" title="Novo Relatório"><i class="bi bi-broadcast"></i></a>`;
            }
            
            // Botão de configurações para admin e dev
            if (permissions['gerenciar-usuarios']) {
                actionButtons += `<a href="/gerenciar-usuarios" class="btn" title="Configurações"><i class="bi bi-gear-fill"></i></a>`;
            }
            
            // Botão de logs apenas para dev
            if (permissions.logs) {
                actionButtons += `<a href="/logs" class="btn" title="Logs do Sistema"><i class="bi bi-file-earmark-text"></i></a>`;
            }

            userInfoContainer.innerHTML = `
                <div class="user-info">
                    <span>Olá, <strong>${currentUser.username}</strong></span>
                    <small class="d-block user-role-${currentUser.role}">${getRoleDisplayName(currentUser.role)}</small>
                </div>
                <div class="user-actions">
                    ${actionButtons}
                    <a href="/logout" class="btn" title="Sair"><i class="bi bi-box-arrow-right"></i></a>
                </div>`;
        }
    } catch (e) { console.error("Falha na sessão:", e); window.location.href = '/login'; }
}

function getRoleDisplayName(role) {
    const roleNames = {
        'gerente': 'Gerente',
        'consultor': 'Consultor',
        'monitoramento': 'Monitoramento',
        'admin': 'Administrador',
        'dev': 'Desenvolvedor'
    };
    return roleNames[role] || role;
}

// =================================================================
// CONTROLE DE SIDEBAR TOGGLE
// =================================================================
function initSidebarToggle() {
    const sidebar = document.querySelector('.sidebar-desktop');
    const mainContent = document.querySelector('.main-content');
    const toggleBtn = document.getElementById('sidebar-toggle');
    const showBtn = document.getElementById('sidebar-show-btn');
    
    if (!sidebar || !mainContent || !toggleBtn || !showBtn) return;
    
    // Carregar estado salvo do localStorage
    const sidebarHidden = localStorage.getItem('sidebarHidden') === 'true';
    
    if (sidebarHidden) {
        sidebar.classList.add('sidebar-hidden');
        mainContent.classList.add('sidebar-hidden');
        showBtn.classList.add('visible');
    }
    
    // Toggle sidebar ao clicar no botão de ocultar
    toggleBtn.addEventListener('click', () => {
        const isHidden = sidebar.classList.toggle('sidebar-hidden');
        mainContent.classList.toggle('sidebar-hidden');
        
        if (isHidden) {
            showBtn.classList.add('visible');
            localStorage.setItem('sidebarHidden', 'true');
        } else {
            showBtn.classList.remove('visible');
            localStorage.setItem('sidebarHidden', 'false');
        }
    });
    
    // Mostrar sidebar ao clicar no botão flutuante
    showBtn.addEventListener('click', () => {
        sidebar.classList.remove('sidebar-hidden');
        mainContent.classList.remove('sidebar-hidden');
        showBtn.classList.remove('visible');
        localStorage.setItem('sidebarHidden', 'false');
    });
}

async function main() {
    await setupSessionAndUI();
    
    // Inicializar animações globais da página
    initPageAnimations();
    
    // Inicializar controle de sidebar toggle
    initSidebarToggle();
    
    // Redirecionar técnico para alertas se estiver na home
    if (currentUser && currentUser.role === 'tecnico') {
        const currentPath = location.pathname;
        if (currentPath === '/' || currentPath === '/admin' || currentPath === '/dashboard') {
            history.replaceState(null, '', '/alertas-tecnico');
        }
    }
    
    const mobileMenuModalEl = document.getElementById('mobileMenuModal');
    const mobileMenuModal = mobileMenuModalEl ? new bootstrap.Modal(mobileMenuModalEl) : null;

    document.body.addEventListener('click', e => {
        const navLink = e.target.closest('a.nav-link');
        if (navLink && navLink.closest('.sidebar-nav')) {
            e.preventDefault();
            const destination = navLink.getAttribute('href');
            
            if (navLink.closest('#mobileMenuModal')) {
                mobileMenuModalEl.addEventListener('hidden.bs.modal', () => {
                    navigateTo(destination);
                }, { once: true });
                if (mobileMenuModal) mobileMenuModal.hide();
            } else {
                navigateTo(destination);
            }
        }
    });

    window.addEventListener('popstate', () => loadPage(location.pathname + location.search));
    loadPage(location.pathname + location.search);
}

document.addEventListener('DOMContentLoaded', main);

// =================================================================
// FUNÇÕES DE UTILIDADE GLOBAIS
// =================================================================
export function showToast(title, message, type = 'success') {
    const toastEl = document.getElementById('notificationToast');
    if (!toastEl) return;
    const toast = bootstrap.Toast.getOrCreateInstance(toastEl);
    
    toastEl.querySelector('#toast-title').textContent = title;
    toastEl.querySelector('#toast-body').textContent = message;
    const toastHeader = toastEl.querySelector('.toast-header');
    toastHeader.classList.remove('bg-success', 'bg-danger', 'bg-info');
    if (type === 'success') toastHeader.classList.add('bg-success');
    else if (type === 'danger') toastHeader.classList.add('bg-danger');
    else toastHeader.classList.add('bg-info');
    
    toast.show();
}

export function showConfirmModal(message) {
    return new Promise((resolve) => {
        const confirmModalEl = document.getElementById('confirmModal');
        if (!confirmModalEl) { resolve(window.confirm(message)); return; }

        const confirmModal = bootstrap.Modal.getOrCreateInstance(confirmModalEl);
        confirmModalEl.querySelector('#confirmModalBody').textContent = message;

        const btnYes = confirmModalEl.querySelector('#confirm-btn-yes');
        const btnNo = confirmModalEl.querySelector('#confirm-btn-no');

        const onYesClick = () => resolve(true);
        const onNoClick = () => resolve(false);

        btnYes.addEventListener('click', onYesClick, { once: true });
        btnNo.addEventListener('click', onNoClick, { once: true });
        
        // Garante que se o modal for fechado de outra forma, ele resolve como 'false'
        confirmModalEl.addEventListener('hidden.bs.modal', () => resolve(false), { once: true });

        confirmModal.show();
    });
}