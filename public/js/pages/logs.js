import { showToast, getAuthHeaders } from '../utils.js';

export function initLogsPage() {
    const tableBody = document.getElementById('logs-table-body');
    const btnFilter = document.getElementById('btn-filter-logs');
    const btnClear = document.getElementById('btn-clear-logs');
    const btnLoadMore = document.getElementById('btn-load-more-logs');
    const logTypeFilter = document.getElementById('log-type-filter');
    const dateStart = document.getElementById('log-date-start');
    const dateEnd = document.getElementById('log-date-end');

    let currentOffset = 0;
    const limit = 50;

    async function loadLogs(reset = false) {
        if (reset) {
            currentOffset = 0;
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Carregando...</td></tr>';
        }

        const params = new URLSearchParams({
            type: logTypeFilter.value,
            start: dateStart.value || '',
            end: dateEnd.value || '',
            limit,
            offset: currentOffset
        });

        try {
            const response = await fetch(`/api/logs?${params}`);
            const { logs, total, stats } = await response.json();

            if (reset) {
                tableBody.innerHTML = '';
            }

            if (logs.length === 0 && reset) {
                tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Nenhum log encontrado.</td></tr>';
                btnLoadMore.classList.add('d-none');
                return;
            }

            logs.forEach(log => {
                const row = createLogRow(log);
                tableBody.insertAdjacentHTML('beforeend', row);
            });

            currentOffset += logs.length;
            btnLoadMore.classList.toggle('d-none', currentOffset >= total);

            updateStats(stats);

        } catch (err) {
            showToast('Erro', 'Não foi possível carregar os logs.', 'danger');
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Erro ao carregar logs.</td></tr>';
        }
    }

    function createLogRow(log) {
        const typeColors = {
            error: 'danger',
            warning: 'warning',
            info: 'info',
            access: 'success'
        };
        const color = typeColors[log.type] || 'secondary';

        return `
            <tr>
                <td><small>${new Date(log.timestamp).toLocaleString('pt-BR')}</small></td>
                <td><span class="badge bg-${color}">${log.type.toUpperCase()}</span></td>
                <td>${log.username || '-'}</td>
                <td>${log.action || '-'}</td>
                <td><small>${log.details || '-'}</small></td>
            </tr>
        `;
    }

    function updateStats(stats) {
        if (stats) {
            document.getElementById('stat-errors').textContent = stats.errors || 0;
            document.getElementById('stat-warnings').textContent = stats.warnings || 0;
            document.getElementById('stat-users').textContent = stats.activeUsers || 0;
            document.getElementById('stat-uptime').textContent = stats.uptime || '--';
        }
    }

    async function clearLogs() {
        if (!confirm('Tem certeza que deseja limpar todos os logs? Esta ação não pode ser desfeita.')) {
            return;
        }

        try {
            const response = await fetch('/api/logs', { method: 'DELETE', headers: await getAuthHeaders() });
            if (response.ok) {
                showToast('Sucesso', 'Logs limpos com sucesso.', 'success');
                loadLogs(true);
            } else {
                throw new Error('Falha ao limpar logs');
            }
        } catch (err) {
            showToast('Erro', 'Não foi possível limpar os logs.', 'danger');
        }
    }

    btnFilter.addEventListener('click', () => loadLogs(true));
    btnClear.addEventListener('click', clearLogs);
    btnLoadMore.addEventListener('click', () => loadLogs(false));

    loadLogs(true);
}
