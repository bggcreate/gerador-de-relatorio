import { showToast, getAuthHeaders } from '../utils.js';

export function initConfiguracoesPage() {
    const secaoConfig = document.getElementById('secao-configuracoes');
    
    if (!secaoConfig) {
        console.error('Elementos da página de configurações não encontrados');
        return;
    }
    
    secaoConfig.style.display = 'block';
    init();
}

function init() {
    initBackupTab();
    initDriveTab();
    initIntegrationsTab();
}

async function initBackupTab() {
    await carregarConfigBackup();
    await carregarHistoricoBackup();

    document.getElementById('btn-criar-backup').addEventListener('click', criarBackupManual);
    document.getElementById('btn-enviar-email').addEventListener('click', enviarBackupEmail);
    document.getElementById('btn-refresh-history').addEventListener('click', carregarHistoricoBackup);
    
    document.getElementById('form-config-backup').addEventListener('submit', async (e) => {
        e.preventDefault();
        await salvarConfigBackup();
    });
}

async function carregarConfigBackup() {
    try {
        const response = await fetch('/api/settings/backup/config', {
            headers: await getAuthHeaders()
        });
        
        if (!response.ok) throw new Error('Erro ao carregar configurações');
        
        const config = await response.json();
        
        document.getElementById('auto-backup-enabled').checked = config.autoBackupEnabled;
        document.getElementById('auto-backup-frequency').value = config.autoBackupFrequency;
        document.getElementById('auto-backup-email').value = config.autoBackupEmail;
    } catch (error) {
        console.error('Erro ao carregar configurações de backup:', error);
    }
}

async function salvarConfigBackup() {
    try {
        const config = {
            autoBackupEnabled: document.getElementById('auto-backup-enabled').checked,
            autoBackupFrequency: document.getElementById('auto-backup-frequency').value,
            autoBackupEmail: document.getElementById('auto-backup-email').value
        };

        const response = await fetch('/api/settings/backup/config', {
            method: 'POST',
            headers: await getAuthHeaders(),
            body: JSON.stringify(config)
        });

        if (!response.ok) throw new Error('Erro ao salvar configurações');

        showToast('Configurações salvas com sucesso', 'success');
    } catch (error) {
        console.error('Erro ao salvar configurações:', error);
        showToast('Erro ao salvar configurações', 'danger');
    }
}

async function criarBackupManual() {
    const btn = document.getElementById('btn-criar-backup');
    const originalText = btn.innerHTML;
    
    try {
        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Criando...';
        
        const response = await fetch('/api/settings/backup/manual', {
            method: 'POST',
            headers: await getAuthHeaders()
        });

        const result = await response.json();

        if (!response.ok) throw new Error(result.error || 'Erro ao criar backup');

        showToast(`Backup criado com sucesso (${result.sizeMB} MB)`, 'success');
        await carregarHistoricoBackup();
    } catch (error) {
        console.error('Erro ao criar backup:', error);
        showToast(error.message, 'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalText;
    }
}

async function enviarBackupEmail() {
    const emailInput = document.getElementById('email-backup');
    const email = emailInput.value.trim();
    
    if (!email) {
        showToast('Digite um email', 'warning');
        return;
    }

    const btn = document.getElementById('btn-enviar-email');
    const originalText = btn.innerHTML;
    
    try {
        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
        
        const response = await fetch('/api/settings/backup/email', {
            method: 'POST',
            headers: await getAuthHeaders(),
            body: JSON.stringify({ emailTo: email })
        });

        const result = await response.json();

        if (!response.ok) throw new Error(result.error || 'Erro ao enviar backup');

        showToast('Backup enviado com sucesso!', 'success');
        emailInput.value = '';
        await carregarHistoricoBackup();
    } catch (error) {
        console.error('Erro ao enviar backup:', error);
        showToast(error.message, 'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalText;
    }
}

async function carregarHistoricoBackup() {
    const tbody = document.getElementById('backup-history-table');
    
    try {
        const response = await fetch('/api/settings/backup/history?limit=20', {
            headers: await getAuthHeaders()
        });

        if (!response.ok) throw new Error('Erro ao carregar histórico');

        const data = await response.json();
        const backups = data.backups || [];

        if (backups.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">Nenhum backup encontrado</td></tr>';
            return;
        }

        tbody.innerHTML = backups.map(backup => {
            const date = new Date(backup.created_at).toLocaleString('pt-BR');
            const statusBadge = backup.status === 'success' 
                ? '<span class="badge bg-success">Sucesso</span>' 
                : '<span class="badge bg-danger">Erro</span>';
            
            const emailSent = backup.sent_to_email 
                ? '<i class="bi bi-envelope-check text-success" title="Email enviado"></i>'
                : '<i class="bi bi-envelope text-muted" title="Email não enviado"></i>';

            return `
                <tr>
                    <td>${date}</td>
                    <td>
                        ${emailSent}
                        <span class="badge bg-secondary ms-1">${backup.backup_type}</span>
                    </td>
                    <td>${backup.sizeMB} MB</td>
                    <td>${statusBadge}</td>
                    <td>${backup.created_by || '-'}</td>
                    <td class="text-center">
                        ${backup.filepath ? `<a href="${backup.downloadUrl}" class="btn btn-sm btn-outline-primary" title="Download">
                            <i class="bi bi-download"></i>
                        </a>` : '-'}
                    </td>
                </tr>
            `;
        }).join('');
    } catch (error) {
        console.error('Erro ao carregar histórico:', error);
        tbody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Erro ao carregar histórico</td></tr>';
    }
}

let driveTimelineChart = null;

async function initDriveTab() {
    await carregarUsoDrive();
    await carregarTimelineDrive();
    await carregarUltimaSinc();

    document.getElementById('timeline-period').addEventListener('change', async (e) => {
        await carregarTimelineDrive(e.target.value);
    });

    setInterval(carregarUsoDrive, 60000);
}

async function carregarUsoDrive() {
    try {
        const response = await fetch('/api/settings/drive/usage', {
            headers: await getAuthHeaders()
        });

        if (!response.ok) throw new Error('Erro ao carregar uso do Drive');

        const data = await response.json();

        document.getElementById('drive-usado-gb').textContent = data.usadoGB + ' GB';
        document.getElementById('drive-limite-gb').textContent = data.limiteGB + ' GB';
        document.getElementById('drive-percentual').textContent = data.percentual + '%';
        document.getElementById('drive-disponivel-gb').textContent = data.disponivelGB.toFixed(2) + ' GB';
    } catch (error) {
        console.error('Erro ao carregar uso do Drive:', error);
        document.getElementById('drive-usado-gb').textContent = 'Erro';
    }
}

async function carregarTimelineDrive(periodo = 30) {
    try {
        const response = await fetch(`/api/settings/drive/timeline?periodo=${periodo}`, {
            headers: await getAuthHeaders()
        });

        if (!response.ok) throw new Error('Erro ao carregar timeline');

        const data = await response.json();

        const ctx = document.getElementById('drive-timeline-chart').getContext('2d');

        if (driveTimelineChart) {
            driveTimelineChart.destroy();
        }

        driveTimelineChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.labels.map(d => new Date(d).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' })),
                datasets: [{
                    label: 'Arquivos Enviados',
                    data: data.data,
                    borderColor: '#4285f4',
                    backgroundColor: 'rgba(66, 133, 244, 0.1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: true
                    },
                    tooltip: {
                        callbacks: {
                            label: (context) => {
                                return `${context.parsed.y} arquivo(s)`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Erro ao carregar timeline:', error);
    }
}

async function carregarUltimaSinc() {
    const container = document.getElementById('last-sync-info');
    
    try {
        const response = await fetch('/api/settings/drive/last-sync', {
            headers: await getAuthHeaders()
        });

        if (!response.ok) throw new Error('Erro ao verificar sincronização');

        const data = await response.json();

        if (data.lastSync) {
            const date = new Date(data.lastSync).toLocaleString('pt-BR');
            container.innerHTML = `
                <div class="d-flex align-items-center">
                    <i class="bi bi-check-circle text-success me-2" style="font-size: 1.5rem;"></i>
                    <div>
                        <strong>Última sincronização:</strong> ${date}<br>
                        <small class="text-muted">Status: Online</small>
                    </div>
                </div>
            `;
        } else {
            container.innerHTML = `
                <div class="d-flex align-items-center">
                    <i class="bi bi-exclamation-circle text-warning me-2" style="font-size: 1.5rem;"></i>
                    <div>
                        <strong>Nenhuma sincronização encontrada</strong><br>
                        <small class="text-muted">Ainda não há dados</small>
                    </div>
                </div>
            `;
        }
    } catch (error) {
        console.error('Erro ao verificar sincronização:', error);
        container.innerHTML = '<span class="text-danger">Erro ao verificar</span>';
    }
}

async function initIntegrationsTab() {
    await carregarIntegracoes();
}

async function carregarIntegracoes() {
    const container = document.getElementById('integrations-container');
    
    try {
        const response = await fetch('/api/settings/integrations', {
            headers: await getAuthHeaders()
        });

        if (!response.ok) throw new Error('Erro ao carregar integrações');

        const data = await response.json();
        const integrations = data.integrations || [];

        container.innerHTML = integrations.map(integration => {
            const statusColor = integration.status === 'connected' ? 'success' : 
                                integration.status === 'error' ? 'danger' : 'secondary';
            const statusText = integration.status === 'connected' ? 'Conectado' : 
                              integration.status === 'error' ? 'Erro' : 'Desconectado';
            const statusIcon = integration.status === 'connected' ? 'check-circle-fill' : 
                              integration.status === 'error' ? 'x-circle-fill' : 'circle';

            return `
                <div class="col-md-4 mb-4">
                    <div class="card h-100">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-start mb-3">
                                <h5 class="card-title mb-0">${integration.name}</h5>
                                <span class="badge bg-${statusColor}">
                                    <i class="bi bi-${statusIcon} me-1"></i>${statusText}
                                </span>
                            </div>
                            <p class="text-muted small">Tipo: ${integration.type}</p>
                            ${integration.lastCheck ? `
                                <p class="small text-muted mb-0">
                                    <i class="bi bi-clock me-1"></i>
                                    Última verificação: ${new Date(integration.lastCheck).toLocaleString('pt-BR')}
                                </p>
                            ` : ''}
                            ${integration.details ? `
                                <hr>
                                <small class="text-muted">
                                    ${Object.entries(integration.details).map(([key, value]) => 
                                        `<div>${key}: ${typeof value === 'boolean' ? (value ? '✓' : '✗') : value}</div>`
                                    ).join('')}
                                </small>
                            ` : ''}
                        </div>
                    </div>
                </div>
            `;
        }).join('');
    } catch (error) {
        console.error('Erro ao carregar integrações:', error);
        container.innerHTML = '<div class="col-12"><div class="alert alert-danger">Erro ao carregar integrações</div></div>';
    }
}
