import { initAdminPage } from './pages/admin.js';
import { initConsultaPage } from './pages/consulta.js';
import { initGerenciarLojasPage } from './pages/gerenciar-lojas.js';
import { initDemandasPage } from './pages/demandas.js';
import { initNovoRelatorioPage } from './pages/novo-relatorio.js';
import { initGerenciarUsuariosPage } from './pages/gerenciar-usuarios.js';

const pageInitializers = {
    'admin': initAdminPage,
    'consulta': initConsultaPage,
    'gerenciar-lojas': initGerenciarLojasPage,
    'demandas': initDemandasPage,
    'novo-relatorio': initNovoRelatorioPage,
    'gerenciar-usuarios': initGerenciarUsuariosPage
};

let currentUser = null;

async function loadPage(path) {
    const pageContent = document.getElementById('page-content');
    if (!pageContent) return;

    const defaultPage = 'admin';
    const pageName = (path.startsWith('/') ? path.substring(1) : path).split('?')[0] || defaultPage;
    const activePage = (pageName === '' || pageName === 'index.html') ? defaultPage : pageName;
    
    document.querySelectorAll('.sidebar-nav .nav-item').forEach(item => {
        const link = item.querySelector('.nav-link');
        const linkHref = link.getAttribute('href');
        const linkPage = linkHref.substring(1);
        item.classList.toggle('active', linkPage === activePage);
    });

    pageContent.innerHTML = '<div class="d-flex justify-content-center p-5"><div class="spinner-border" role="status"></div></div>';

    try {
        const response = await fetch(`/content/${activePage}`);
        if (!response.ok) throw new Error(`Conteúdo da página /content/${activePage} não encontrado.`);
        
        pageContent.innerHTML = await response.text();
        
        const initFunc = pageInitializers[activePage];
        if (typeof initFunc === 'function') {
            setTimeout(() => {
                try {
                    initFunc(currentUser);
                } catch (err) {
                    console.error(`Erro ao inicializar a página '${activePage}':`, err);
                    pageContent.innerHTML = `<div class="p-3 text-center text-danger"><h3>Oops!</h3><p>Ocorreu um erro ao carregar os componentes desta página.</p></div>`;
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

async function setupSessionAndUI() {
    try {
        const response = await fetch('/api/session-info');
        if (!response.ok) {
            window.location.href = '/login';
            return;
        }
        currentUser = await response.json();
        
        const userInfoContainer = document.getElementById('user-info-container');
        if (userInfoContainer) {
            let adminButtons = '';
            if (currentUser.role === 'admin') {
                document.getElementById('nav-gerenciar')?.classList.remove('d-none');
                
                adminButtons = `<a href="/gerenciar-usuarios" class="btn" title="Gerenciar Usuários"><i class="bi bi-gear-fill"></i></a>`;
            }

            document.getElementById('nav-demandas')?.classList.remove('d-none');

            userInfoContainer.innerHTML = `
                <div class="user-info"><span>Olá, <strong>${currentUser.username}</strong></span></div>
                <div class="user-actions">
                    <a href="/live" id="live-mode-btn" class="btn" title="Modo Live"><i class="bi bi-broadcast"></i></a>
                    ${adminButtons}
                    <a href="/logout" class="btn" title="Sair"><i class="bi bi-box-arrow-right"></i></a>
                </div>`;
            
            document.getElementById('live-mode-btn')?.addEventListener('click', (e) => {
                e.preventDefault();
                window.open(e.currentTarget.href, 'live-window', 'width=550,height=850,scrollbars=yes,resizable=yes');
            });
        }
    } catch (e) {
        console.error("Falha na sessão:", e);
        window.location.href = '/login';
    }
}

async function main() {
    await setupSessionAndUI();

    // --- INÍCIO: LÓGICA DE RESPONSIVIDADE DA SIDEBAR ---
    const sidebar = document.querySelector('.sidebar');
    const sidebarToggle = document.querySelector('#sidebar-toggle');

    if (sidebarToggle && sidebar) {
        // Abre/Fecha a sidebar ao clicar no botão de menu (hambúrguer)
        sidebarToggle.addEventListener('click', (e) => {
            e.stopPropagation(); // Impede que o clique se propague para outros elementos
            sidebar.classList.toggle('is-open');
        });
    }

    // Fecha a sidebar se o usuário clicar fora dela (apenas em modo mobile)
    document.addEventListener('click', (event) => {
        // Verifica se a sidebar está aberta e se o botão de toggle está visível (indicando tela pequena)
        if (sidebar && sidebar.classList.contains('is-open') && getComputedStyle(sidebarToggle).display !== 'none') {
            // Verifica se o clique não foi na própria sidebar nem no botão que a abre
            if (!sidebar.contains(event.target) && !sidebarToggle.contains(event.target)) {
                sidebar.classList.remove('is-open');
            }
        }
    });
    // --- FIM: LÓGICA DE RESPONSIVIDADE DA SIDEBAR ---


    document.body.addEventListener('click', e => {
        const navLink = e.target.closest('a.nav-link');
        if (navLink && navLink.closest('.sidebar-nav')) {
            e.preventDefault();

            // Adicionado: Fecha a sidebar após clicar em um item de menu no modo mobile
            if (sidebar.classList.contains('is-open')) {
                sidebar.classList.remove('is-open');
            }

            navigateTo(navLink.getAttribute('href'));
        }
    });
    
    window.addEventListener('popstate', () => loadPage(location.pathname + location.search));
    
    loadPage(location.pathname + location.search);
}

document.addEventListener('DOMContentLoaded', main);

export function showToast(title, message, type = 'success') {
    const toastEl = document.getElementById('notificationToast');
    if (!toastEl) return;
    const toastHeader = toastEl.querySelector('.toast-header');
    const toastTitle = document.getElementById('toast-title');
    const toastBody = document.getElementById('toast-body');

    toastTitle.textContent = title;
    toastBody.textContent = message;

    toastHeader.classList.remove('bg-success', 'bg-danger', 'bg-info');
    if (type === 'success') toastHeader.classList.add('bg-success');
    else if (type === 'danger') toastHeader.classList.add('bg-danger');
    else toastHeader.classList.add('bg-info');

    const toast = new bootstrap.Toast(toastEl);
    toast.show();
}

export function showConfirmModal(message) {
    return new Promise((resolve) => {
        const confirmModalEl = document.getElementById('confirmModal');
        if (!confirmModalEl) {
            resolve(window.confirm(message));
            return;
        }

        const confirmModal = new bootstrap.Modal(confirmModalEl);
        document.getElementById('confirmModalBody').textContent = message;

        const btnYes = document.getElementById('confirm-btn-yes');
        const btnNo = document.getElementById('confirm-btn-no');
        const btnClose = confirmModalEl.querySelector('.btn-close');

        const handleResolve = (value) => {
            btnYes.removeEventListener('click', onYesClick);
            btnNo.removeEventListener('click', onNoClick);
            btnClose.removeEventListener('click', onNoClick);
            confirmModalEl.removeEventListener('hidden.bs.modal', onHidden);

            if (confirmModal._isShown) {
                confirmModal.hide();
            }
            resolve(value);
        };

        const onYesClick = () => handleResolve(true);
        const onNoClick = () => handleResolve(false);
        const onHidden = () => handleResolve(false);

        btnYes.addEventListener('click', onYesClick, { once: true });
        btnNo.addEventListener('click', onNoClick, { once: true });
        btnClose.addEventListener('click', onNoClick, { once: true });
        confirmModalEl.addEventListener('hidden.bs.modal', onHidden, { once: true });

        confirmModal.show();
    });
}