// =================================================================
// IMPORTS E CONFIGURAÇÃO DE PÁGINAS
// =================================================================
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
    // A página gerenciar-usuarios carrega o conteúdo de usuários e backup
    'gerenciar-usuarios': initGerenciarUsuariosPage 
};

let currentUser = null;

// =================================================================
// LÓGICA DE NAVEGAÇÃO E CARREGAMENTO DE PÁGINAS
// =================================================================
async function loadPage(path) {
    const pageContent = document.getElementById('page-content');
    if (!pageContent) return;

    const defaultPage = 'admin';
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
        const userInfoContainer = document.getElementById('user-info-container');
        if (userInfoContainer) {
            let adminButtons = '';
            // Mostra os links de admin se o usuário for admin
            if (currentUser.role === 'admin') {
                document.querySelectorAll('#nav-gerenciar').forEach(el => el?.classList.remove('d-none'));
                 // A página de gerenciar usuários é acessada pelo link "Lojas", mas o botão fica no footer.
                adminButtons = `<a href="/gerenciar-usuarios" class="btn" title="Configurações"><i class="bi bi-gear-fill"></i></a>`;
            }
            // Mostra o link de demandas para todos
            document.querySelectorAll('#nav-demandas').forEach(el => el?.classList.remove('d-none'));

            userInfoContainer.innerHTML = `<div class="user-info"><span>Olá, <strong>${currentUser.username}</strong></span></div><div class="user-actions"><a href="/live" id="live-mode-btn" class="btn" title="Modo Live"><i class="bi bi-broadcast"></i></a>${adminButtons}<a href="/logout" class="btn" title="Sair"><i class="bi bi-box-arrow-right"></i></a></div>`;
            
            document.getElementById('live-mode-btn')?.addEventListener('click', (e) => {
                e.preventDefault();
                window.open(e.currentTarget.href, 'live-window', 'width=550,height=850,scrollbars=yes,resizable=yes');
            });
        }
    } catch (e) { console.error("Falha na sessão:", e); window.location.href = '/login'; }
}

async function main() {
    await setupSessionAndUI();
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