export function initMonitorDbPage() {
    carregarDadosMonitor();
    
    setInterval(carregarDadosMonitor, 30000);
}

async function carregarDadosMonitor() {
    try {
        const response = await fetch('/api/monitor/database');
        const data = await response.json();
        
        atualizarCards(data);
        atualizarEstatisticasTabelas(data.tableStats);
        atualizarAtividadesRecentes(data.recentActivities);
        atualizarBackups(data.recentBackups);
        atualizarInformacoesDoSistema(data.systemInfo);
        
    } catch (error) {
        console.error('Erro ao carregar dados do monitor:', error);
        document.getElementById('db-status').innerHTML = '<span class="text-danger"><i class="bi bi-x-circle"></i> Erro</span>';
    }
}

function atualizarCards(data) {
    const statusEl = document.getElementById('db-status');
    if (data.connected) {
        statusEl.innerHTML = '<span class="text-success"><i class="bi bi-check-circle-fill"></i> Conectado</span>';
    } else {
        statusEl.innerHTML = '<span class="text-danger"><i class="bi bi-x-circle-fill"></i> Desconectado</span>';
    }
    
    document.getElementById('db-provider').textContent = data.provider || '-';
    document.getElementById('db-host').textContent = data.host || '-';
    document.getElementById('db-size').textContent = formatBytes(data.databaseSize || 0);
    document.getElementById('total-records').textContent = (data.totalRecords || 0).toLocaleString('pt-BR');
}

function atualizarEstatisticasTabelas(tableStats) {
    const tbody = document.getElementById('table-stats');
    
    if (!tableStats || tableStats.length === 0) {
        tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted">Nenhum dado disponível</td></tr>';
        return;
    }
    
    const totalRecords = tableStats.reduce((sum, t) => sum + t.count, 0);
    
    tbody.innerHTML = tableStats
        .sort((a, b) => b.count - a.count)
        .map(table => {
            const percentage = totalRecords > 0 ? ((table.count / totalRecords) * 100).toFixed(1) : 0;
            return `
                <tr>
                    <td>
                        <i class="bi bi-table"></i> 
                        <strong>${table.name}</strong>
                    </td>
                    <td class="text-end">${table.count.toLocaleString('pt-BR')}</td>
                    <td class="text-end">
                        <span class="badge bg-secondary">${percentage}%</span>
                    </td>
                </tr>
            `;
        })
        .join('');
}

function atualizarAtividadesRecentes(activities) {
    const container = document.getElementById('recent-activities');
    
    if (!activities || activities.length === 0) {
        container.innerHTML = '<p class="text-center text-muted">Nenhuma atividade recente</p>';
        return;
    }
    
    container.innerHTML = activities.map(activity => {
        const iconMap = {
            'login': 'bi-box-arrow-in-right text-primary',
            'logout': 'bi-box-arrow-right text-secondary',
            'create': 'bi-plus-circle text-success',
            'update': 'bi-pencil text-warning',
            'delete': 'bi-trash text-danger',
            'backup': 'bi-archive text-info'
        };
        
        const icon = iconMap[activity.type] || 'bi-circle text-muted';
        const date = new Date(activity.timestamp);
        const timeAgo = getTimeAgo(date);
        
        return `
            <div class="d-flex align-items-start mb-3 pb-2 border-bottom">
                <i class="bi ${icon} me-2 mt-1"></i>
                <div class="flex-grow-1">
                    <div class="d-flex justify-content-between">
                        <strong>${activity.action || 'Ação'}</strong>
                        <small class="text-muted">${timeAgo}</small>
                    </div>
                    <small class="text-muted">
                        ${activity.username || 'Sistema'}
                        ${activity.details ? ` - ${activity.details}` : ''}
                    </small>
                </div>
            </div>
        `;
    }).join('');
}

function atualizarBackups(backups) {
    const tbody = document.getElementById('backup-list');
    
    if (!backups || backups.length === 0) {
        tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Nenhum backup encontrado</td></tr>';
        return;
    }
    
    tbody.innerHTML = backups.map(backup => {
        const date = new Date(backup.created_at);
        const emailIcon = backup.sent_to_email 
            ? '<i class="bi bi-check-circle-fill text-success"></i>' 
            : '<i class="bi bi-x-circle text-muted"></i>';
        
        const typeClass = backup.backup_type === 'automatic' ? 'bg-info' : 'bg-secondary';
        
        return `
            <tr>
                <td>${date.toLocaleDateString('pt-BR')} ${date.toLocaleTimeString('pt-BR', {hour: '2-digit', minute: '2-digit'})}</td>
                <td><span class="badge ${typeClass}">${backup.backup_type}</span></td>
                <td class="text-end">${formatBytes(backup.size_bytes)}</td>
                <td class="text-center">${emailIcon}</td>
            </tr>
        `;
    }).join('');
}

function atualizarInformacoesDoSistema(systemInfo) {
    if (!systemInfo) return;
    
    document.getElementById('info-database').textContent = systemInfo.database || '-';
    document.getElementById('info-server').textContent = systemInfo.server || '-';
    
    const emailStatus = systemInfo.emailConfigured 
        ? '<span class="text-success"><i class="bi bi-check-circle"></i> Configurado</span>' 
        : '<span class="text-warning"><i class="bi bi-exclamation-triangle"></i> Não configurado</span>';
    document.getElementById('info-backup-email').innerHTML = emailStatus;
    
    const driveStatus = systemInfo.googleDriveConfigured 
        ? '<span class="text-success"><i class="bi bi-check-circle"></i> Configurado</span>' 
        : '<span class="text-warning"><i class="bi bi-exclamation-triangle"></i> Não configurado</span>';
    document.getElementById('info-google-drive').innerHTML = driveStatus;
    
    document.getElementById('info-last-update').textContent = new Date().toLocaleTimeString('pt-BR');
}

function formatBytes(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
}

function getTimeAgo(date) {
    const seconds = Math.floor((new Date() - date) / 1000);
    
    if (seconds < 60) return 'Agora mesmo';
    if (seconds < 3600) return Math.floor(seconds / 60) + ' min atrás';
    if (seconds < 86400) return Math.floor(seconds / 3600) + 'h atrás';
    return Math.floor(seconds / 86400) + 'd atrás';
}
