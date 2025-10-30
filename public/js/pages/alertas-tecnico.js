import { showToast } from '../utils.js';

export function initAlertasTecnicoPage() {
    if (!window.currentUser) {
        setTimeout(initAlertasTecnicoPage, 100);
        return;
    }

    const userRole = window.currentUser.role;
    
    if (userRole !== 'tecnico') {
        window.location.href = '/admin';
        return;
    }

    // Aguardar DOM estar pronto
    setTimeout(() => {
        carregarEstatisticas();
        carregarAssistenciasEmAndamento();
        carregarAssistenciasConcluidas();
        
        // Atualizar a cada 30 segundos
        setInterval(() => {
            carregarEstatisticas();
            carregarAssistenciasEmAndamento();
            carregarAssistenciasConcluidas();
        }, 30000);
    }, 100);
}

async function carregarEstatisticas() {
    try {
        const response = await fetch('/api/assistencias/stats-tecnico');
        const stats = await response.json();
        
        document.getElementById('stat-em-andamento').textContent = stats.emAndamento || 0;
        document.getElementById('stat-concluidas-hoje').textContent = stats.concluidasHoje || 0;
        document.getElementById('stat-total-mes').textContent = stats.totalMes || 0;
        document.getElementById('stat-aguardando-pecas').textContent = stats.aguardandoPecas || 0;
    } catch (e) {
        console.error('Erro ao carregar estatísticas:', e);
    }
}

async function carregarAssistenciasEmAndamento() {
    const tbody = document.getElementById('tabela-em-andamento');
    
    try {
        const response = await fetch('/api/assistencias?status=Em andamento,Aguardando peças');
        const assistencias = await response.json();
        
        if (assistencias.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center py-4 text-muted">Nenhuma assistência em andamento</td></tr>';
            return;
        }
        
        tbody.innerHTML = assistencias.map(a => {
            const statusClass = a.status === 'Em andamento' ? 'warning' : 'danger';
            const statusTexto = a.status === 'Em andamento' ? 'Em Andamento' : 'Aguardando Peças';
            const dataEntrada = new Date(a.data_entrada).toLocaleDateString('pt-BR');
            
            return `
                <tr>
                    <td><strong>#${a.os}</strong></td>
                    <td>${a.cliente_nome}</td>
                    <td>${a.aparelho}</td>
                    <td><span class="badge bg-${statusClass}">${statusTexto}</span></td>
                    <td>${dataEntrada}</td>
                    <td class="text-end">
                        <button class="btn btn-sm btn-outline-primary" onclick="window.location.href='/assistencia?os=${a.os}'">
                            Ver Detalhes
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    } catch (e) {
        console.error('Erro ao carregar assistências em andamento:', e);
        tbody.innerHTML = '<tr><td colspan="6" class="text-center py-4 text-danger">Erro ao carregar dados</td></tr>';
    }
}

async function carregarAssistenciasConcluidas() {
    const tbody = document.getElementById('tabela-concluidas');
    
    try {
        const response = await fetch('/api/assistencias?status=Concluído&limit=10');
        const assistencias = await response.json();
        
        if (assistencias.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center py-4 text-muted">Nenhuma assistência concluída recentemente</td></tr>';
            return;
        }
        
        tbody.innerHTML = assistencias.map(a => {
            const dataConclusao = a.data_saida ? new Date(a.data_saida).toLocaleDateString('pt-BR') : '-';
            
            return `
                <tr>
                    <td><strong>#${a.os}</strong></td>
                    <td>${a.cliente_nome}</td>
                    <td>${a.aparelho}</td>
                    <td>${dataConclusao}</td>
                    <td class="text-end">
                        <button class="btn btn-sm btn-outline-secondary" onclick="window.location.href='/assistencia?os=${a.os}'">
                            Ver Detalhes
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    } catch (e) {
        console.error('Erro ao carregar assistências concluídas:', e);
        tbody.innerHTML = '<tr><td colspan="5" class="text-center py-4 text-danger">Erro ao carregar dados</td></tr>';
    }
}
