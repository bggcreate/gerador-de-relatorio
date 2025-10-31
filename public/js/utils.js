/**
 * Exibe uma notificação toast no canto da tela.
 * @param {string} title Título do toast.
 * @param {string} message Mensagem do corpo do toast.
 * @param {'success'|'danger'|'info'} type O tipo de notificação (muda a cor).
 */
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

/**
 * Exibe um modal de confirmação e retorna uma Promise que resolve como true ou false.
 * @param {string} message A pergunta a ser exibida no modal.
 * @returns {Promise<boolean>} Retorna true se o usuário clicar "Sim", senão false.
 */
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

/**
 * Inicializa animações de scroll para elementos
 * Adiciona a classe 'fade-in-up' aos elementos que entram na viewport
 */
export function initScrollAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    const animateElements = document.querySelectorAll('.card, .animate-on-scroll');
    animateElements.forEach(el => {
        el.classList.add('will-animate');
        observer.observe(el);
    });
}

/**
 * Adiciona efeito de loading suave em botões
 * @param {HTMLElement} button O botão a ser modificado
 * @param {boolean} isLoading Se está carregando ou não
 */
export function setButtonLoading(button, isLoading) {
    if (isLoading) {
        button.dataset.originalText = button.innerHTML;
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Carregando...';
    } else {
        button.disabled = false;
        button.innerHTML = button.dataset.originalText || button.innerHTML;
    }
}

/**
 * Inicializa animações globais da página
 */
export function initPageAnimations() {
    document.body.classList.add('page-loaded');
    
    setTimeout(() => {
        initScrollAnimations();
    }, 100);
}

let cachedCsrfToken = null;

/**
 * Obtém o token CSRF do servidor e armazena em cache
 * @returns {Promise<string>} O token CSRF
 */
export async function getCsrfToken() {
    if (cachedCsrfToken) {
        return cachedCsrfToken;
    }
    
    try {
        const response = await fetch('/api/csrf-token');
        const data = await response.json();
        cachedCsrfToken = data.csrfToken;
        return cachedCsrfToken;
    } catch (error) {
        console.error('Failed to fetch CSRF token:', error);
        return null;
    }
}

/**
 * Cria headers padrão para requisições fetch incluindo CSRF token
 * @param {Object} additionalHeaders Headers adicionais
 * @returns {Promise<Object>} Headers com CSRF token
 */
export async function getAuthHeaders(additionalHeaders = {}) {
    const csrfToken = await getCsrfToken();
    return {
        'Content-Type': 'application/json',
        'x-csrf-token': csrfToken,
        ...additionalHeaders
    };
}