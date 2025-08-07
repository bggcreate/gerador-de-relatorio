/**
 * Exibe uma notificação toast no canto da tela.
 * @param {string} title Título do toast.
 * @param {string} message Mensagem do corpo do toast.
 * @param {'success'|'danger'|'info'} type O tipo de notificação .
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