// =================================================================
// DVR/NVR MONITOR - Interface de monitoramento de dispositivos
// =================================================================

let dispositivosCache = [];
let logsOffset = 0;
const logsLimit = 100;

export function initDVRMonitorPage() {
    carregarLojas();
    carregarDispositivos();
    
    document.getElementById('btn-add-dispositivo').addEventListener('click', () => {
        limparFormularioDispositivo();
        new bootstrap.Modal(document.getElementById('modal-dispositivo')).show();
    });
    
    document.getElementById('btn-salvar-dispositivo').addEventListener('click', salvarDispositivo);
    
    document.getElementById('btn-filtrar-dispositivos').addEventListener('click', () => carregarDispositivos(true));
    document.getElementById('btn-limpar-filtros-dispositivos').addEventListener('click', () => {
        document.getElementById('filtro-loja-dispositivos').value = '';
        document.getElementById('filtro-status-dispositivos').value = '';
        carregarDispositivos();
    });
    
    document.getElementById('logs-tab').addEventListener('click', () => {
        carregarDispositivos();
        logsOffset = 0;
        carregarLogs();
    });
    
    document.getElementById('arquivos-tab').addEventListener('click', () => {
        carregarDispositivos();
        carregarArquivos();
    });
    
    document.getElementById('btn-filtrar-logs').addEventListener('click', () => {
        logsOffset = 0;
        carregarLogs();
    });
    
    document.getElementById('btn-prev-logs').addEventListener('click', () => {
        if (logsOffset >= logsLimit) {
            logsOffset -= logsLimit;
            carregarLogs();
        }
    });
    
    document.getElementById('btn-next-logs').addEventListener('click', () => {
        logsOffset += logsLimit;
        carregarLogs();
    });
    
    document.getElementById('btn-filtrar-arquivos').addEventListener('click', carregarArquivos);
    
    document.getElementById('btn-upload-arquivo').addEventListener('click', () => {
        carregarDispositivos();
        new bootstrap.Modal(document.getElementById('modal-upload')).show();
    });
    
    document.getElementById('btn-confirmar-upload').addEventListener('click', fazerUpload);
}

async function carregarLojas() {
    try {
        const response = await fetch('/api/lojas');
        if (!response.ok) return;
        
        const lojas = await response.json();
        const selects = [
            document.getElementById('dispositivo-loja'),
            document.getElementById('filtro-dvr-logs'),
            document.getElementById('filtro-dvr-arquivos'),
            document.getElementById('upload-dvr')
        ];
        
        selects.forEach(select => {
            if (select && select.id === 'dispositivo-loja') {
                select.innerHTML = '<option value="">Selecione uma loja</option>';
                lojas.forEach(loja => {
                    const option = document.createElement('option');
                    option.value = loja.id;
                    option.textContent = loja.nome;
                    option.dataset.lojaNome = loja.nome;
                    select.appendChild(option);
                });
            }
        });
    } catch (error) {
        console.error('Erro ao carregar lojas:', error);
    }
}

async function carregarDispositivos(aplicarFiltros = false) {
    try {
        let url = '/api/dvr/dispositivos';
        
        if (aplicarFiltros) {
            const params = new URLSearchParams();
            const loja = document.getElementById('filtro-loja-dispositivos').value.trim();
            const status = document.getElementById('filtro-status-dispositivos').value;
            
            if (loja) params.append('loja_nome', loja);
            if (status) params.append('status', status);
            
            if (params.toString()) {
                url += '?' + params.toString();
            }
        }
        
        const response = await fetch(url);
        if (!response.ok) throw new Error('Erro ao carregar dispositivos');
        
        const data = await response.json();
        dispositivosCache = data.data || [];
        
        renderizarDispositivos(dispositivosCache);
        atualizarSelectsDispositivos(dispositivosCache);
    } catch (error) {
        console.error('Erro ao carregar dispositivos:', error);
        mostrarAlerta('Erro ao carregar dispositivos', 'danger');
    }
}

function renderizarDispositivos(dispositivos) {
    const tbody = document.getElementById('tabela-dispositivos');
    
    if (dispositivos.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="text-center">Nenhum dispositivo cadastrado</td></tr>';
        return;
    }
    
    tbody.innerHTML = dispositivos.map(d => {
        const statusClass = d.status === 'online' ? 'success' : 'secondary';
        const statusIcon = d.status === 'online' ? 'circle-fill' : 'circle';
        const ultimaConexao = d.ultima_conexao ? new Date(d.ultima_conexao).toLocaleString('pt-BR') : 'Nunca';
        
        return `
            <tr>
                <td>${escapeHtml(d.nome)}</td>
                <td>${escapeHtml(d.loja_nome || '-')}</td>
                <td>${escapeHtml(d.ip_address)}:${d.porta}</td>
                <td>${escapeHtml(d.modelo || '-')}</td>
                <td>${d.canais_total}</td>
                <td><span class="badge bg-${statusClass}"><i class="bi bi-${statusIcon}"></i> ${d.status}</span></td>
                <td>${ultimaConexao}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editarDispositivo(${d.id})">
                        <i class="bi bi-pencil"></i> Editar
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="excluirDispositivo(${d.id}, '${escapeHtml(d.nome)}')">
                        <i class="bi bi-trash"></i> Excluir
                    </button>
                </td>
            </tr>
        `;
    }).join('');
}

function atualizarSelectsDispositivos(dispositivos) {
    const selectLogs = document.getElementById('filtro-dvr-logs');
    const selectArquivos = document.getElementById('filtro-dvr-arquivos');
    const selectUpload = document.getElementById('upload-dvr');
    
    [selectLogs, selectArquivos, selectUpload].forEach(select => {
        if (!select) return;
        
        const valorAtual = select.value;
        const isUpload = select.id === 'upload-dvr';
        
        select.innerHTML = isUpload ? '<option value="">Selecione um dispositivo</option>' : '<option value="">Todos os dispositivos</option>';
        
        dispositivos.forEach(d => {
            const option = document.createElement('option');
            option.value = d.id;
            option.textContent = `${d.nome} - ${d.loja_nome || 'Sem loja'}`;
            option.dataset.dvrNome = d.nome;
            option.dataset.lojaNome = d.loja_nome || '';
            select.appendChild(option);
        });
        
        if (valorAtual) select.value = valorAtual;
    });
}

function limparFormularioDispositivo() {
    document.getElementById('dispositivo-id').value = '';
    document.getElementById('dispositivo-nome').value = '';
    document.getElementById('dispositivo-loja').value = '';
    document.getElementById('dispositivo-ip').value = '';
    document.getElementById('dispositivo-porta').value = '37777';
    document.getElementById('dispositivo-usuario').value = '';
    document.getElementById('dispositivo-modelo').value = '';
    document.getElementById('dispositivo-canais').value = '0';
    document.getElementById('dispositivo-status').value = 'offline';
    document.getElementById('dispositivo-observacoes').value = '';
}

window.editarDispositivo = async function(id) {
    try {
        const response = await fetch(`/api/dvr/dispositivos/${id}`);
        if (!response.ok) throw new Error('Dispositivo não encontrado');
        
        const data = await response.json();
        const d = data.data;
        
        document.getElementById('dispositivo-id').value = d.id;
        document.getElementById('dispositivo-nome').value = d.nome;
        document.getElementById('dispositivo-loja').value = d.loja_id || '';
        document.getElementById('dispositivo-ip').value = d.ip_address;
        document.getElementById('dispositivo-porta').value = d.porta;
        document.getElementById('dispositivo-usuario').value = d.usuario || '';
        document.getElementById('dispositivo-modelo').value = d.modelo || '';
        document.getElementById('dispositivo-canais').value = d.canais_total;
        document.getElementById('dispositivo-status').value = d.status;
        document.getElementById('dispositivo-observacoes').value = d.observacoes || '';
        
        new bootstrap.Modal(document.getElementById('modal-dispositivo')).show();
    } catch (error) {
        console.error('Erro ao carregar dispositivo:', error);
        mostrarAlerta('Erro ao carregar dispositivo', 'danger');
    }
};

async function salvarDispositivo() {
    const id = document.getElementById('dispositivo-id').value;
    const lojaSelect = document.getElementById('dispositivo-loja');
    const lojaOption = lojaSelect.options[lojaSelect.selectedIndex];
    
    const dados = {
        nome: document.getElementById('dispositivo-nome').value.trim(),
        loja_id: lojaSelect.value || null,
        loja_nome: lojaOption ? lojaOption.dataset.lojaNome : null,
        ip_address: document.getElementById('dispositivo-ip').value.trim(),
        porta: parseInt(document.getElementById('dispositivo-porta').value) || 37777,
        usuario: document.getElementById('dispositivo-usuario').value.trim(),
        modelo: document.getElementById('dispositivo-modelo').value.trim(),
        canais_total: parseInt(document.getElementById('dispositivo-canais').value) || 0,
        status: document.getElementById('dispositivo-status').value,
        observacoes: document.getElementById('dispositivo-observacoes').value.trim()
    };
    
    if (!dados.nome || !dados.ip_address) {
        mostrarAlerta('Nome e IP são obrigatórios', 'warning');
        return;
    }
    
    try {
        const url = id ? `/api/dvr/dispositivos/${id}` : '/api/dvr/dispositivos';
        const method = id ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dados)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Erro ao salvar');
        }
        
        bootstrap.Modal.getInstance(document.getElementById('modal-dispositivo')).hide();
        mostrarAlerta(id ? 'Dispositivo atualizado com sucesso' : 'Dispositivo criado com sucesso', 'success');
        carregarDispositivos();
    } catch (error) {
        console.error('Erro ao salvar dispositivo:', error);
        mostrarAlerta(error.message || 'Erro ao salvar dispositivo', 'danger');
    }
}

window.excluirDispositivo = async function(id, nome) {
    if (!confirm(`Deseja realmente excluir o dispositivo "${nome}"?\n\nIsso também excluirá todos os logs e arquivos associados.`)) {
        return;
    }
    
    try {
        const response = await fetch(`/api/dvr/dispositivos/${id}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) throw new Error('Erro ao excluir');
        
        mostrarAlerta('Dispositivo excluído com sucesso', 'success');
        carregarDispositivos();
    } catch (error) {
        console.error('Erro ao excluir dispositivo:', error);
        mostrarAlerta('Erro ao excluir dispositivo', 'danger');
    }
};

async function carregarLogs() {
    try {
        const params = new URLSearchParams();
        
        const dvrId = document.getElementById('filtro-dvr-logs').value;
        const tipoEvento = document.getElementById('filtro-tipo-evento').value;
        const severidade = document.getElementById('filtro-severidade').value;
        const dataInicio = document.getElementById('filtro-data-inicio-logs').value;
        const dataFim = document.getElementById('filtro-data-fim-logs').value;
        
        if (dvrId) params.append('dvr_id', dvrId);
        if (tipoEvento) params.append('tipo_evento', tipoEvento);
        if (severidade) params.append('severidade', severidade);
        if (dataInicio) params.append('data_inicio', dataInicio);
        if (dataFim) params.append('data_fim', dataFim);
        params.append('limit', logsLimit);
        params.append('offset', logsOffset);
        
        const response = await fetch(`/api/dvr/logs?${params.toString()}`);
        if (!response.ok) throw new Error('Erro ao carregar logs');
        
        const data = await response.json();
        renderizarLogs(data.data || []);
        
        document.getElementById('btn-prev-logs').disabled = logsOffset === 0;
        document.getElementById('btn-next-logs').disabled = (data.data || []).length < logsLimit;
        
        const inicio = logsOffset + 1;
        const fim = logsOffset + (data.data || []).length;
        document.getElementById('info-paginacao-logs').textContent = `Mostrando ${inicio}-${fim} logs`;
    } catch (error) {
        console.error('Erro ao carregar logs:', error);
        mostrarAlerta('Erro ao carregar logs', 'danger');
    }
}

function renderizarLogs(logs) {
    const tbody = document.getElementById('tabela-logs');
    
    if (logs.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="text-center">Nenhum log encontrado</td></tr>';
        return;
    }
    
    tbody.innerHTML = logs.map(log => {
        const dataHora = new Date(log.data_hora).toLocaleString('pt-BR');
        const severidadeClass = {
            'info': 'info',
            'warning': 'warning',
            'error': 'danger',
            'critical': 'danger'
        }[log.severidade] || 'secondary';
        
        return `
            <tr>
                <td>${dataHora}</td>
                <td>${escapeHtml(log.dvr_nome || '-')}</td>
                <td>${escapeHtml(log.loja_nome || '-')}</td>
                <td><span class="badge bg-secondary">${escapeHtml(log.tipo_evento)}</span></td>
                <td>${escapeHtml(log.descricao || '-')}</td>
                <td>${log.canal || '-'}</td>
                <td><span class="badge bg-${severidadeClass}">${log.severidade}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-danger" onclick="excluirLog(${log.id})">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    }).join('');
}

window.excluirLog = async function(id) {
    if (!confirm('Deseja realmente excluir este log?')) return;
    
    try {
        const response = await fetch(`/api/dvr/logs/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Erro ao excluir');
        
        mostrarAlerta('Log excluído com sucesso', 'success');
        carregarLogs();
    } catch (error) {
        console.error('Erro ao excluir log:', error);
        mostrarAlerta('Erro ao excluir log', 'danger');
    }
};

async function carregarArquivos() {
    try {
        const params = new URLSearchParams();
        
        const dvrId = document.getElementById('filtro-dvr-arquivos').value;
        const tipoArquivo = document.getElementById('filtro-tipo-arquivo').value;
        const dataInicio = document.getElementById('filtro-data-inicio-arquivos').value;
        const dataFim = document.getElementById('filtro-data-fim-arquivos').value;
        
        if (dvrId) params.append('dvr_id', dvrId);
        if (tipoArquivo) params.append('tipo_arquivo', tipoArquivo);
        if (dataInicio) params.append('data_inicio', dataInicio);
        if (dataFim) params.append('data_fim', dataFim);
        
        const response = await fetch(`/api/dvr/arquivos?${params.toString()}`);
        if (!response.ok) throw new Error('Erro ao carregar arquivos');
        
        const data = await response.json();
        renderizarArquivos(data.data || []);
    } catch (error) {
        console.error('Erro ao carregar arquivos:', error);
        mostrarAlerta('Erro ao carregar arquivos', 'danger');
    }
}

function renderizarArquivos(arquivos) {
    const tbody = document.getElementById('tabela-arquivos');
    
    if (arquivos.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="text-center">Nenhum arquivo encontrado</td></tr>';
        return;
    }
    
    tbody.innerHTML = arquivos.map(arq => {
        const dataGeracao = arq.data_geracao ? new Date(arq.data_geracao).toLocaleString('pt-BR') : '-';
        const tamanho = formatarTamanho(arq.tamanho_bytes);
        
        return `
            <tr>
                <td>${escapeHtml(arq.nome_arquivo)}</td>
                <td>${escapeHtml(arq.dvr_nome || '-')}</td>
                <td>${escapeHtml(arq.loja_nome || '-')}</td>
                <td><span class="badge bg-info">${escapeHtml(arq.tipo_arquivo)}</span></td>
                <td>${tamanho}</td>
                <td>${dataGeracao}</td>
                <td>${escapeHtml(arq.uploaded_by || '-')}</td>
                <td>
                    <button class="btn btn-sm btn-success" onclick="downloadArquivo(${arq.id}, '${escapeHtml(arq.nome_arquivo)}')">
                        <i class="bi bi-download"></i> Download
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="excluirArquivo(${arq.id}, '${escapeHtml(arq.nome_arquivo)}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    }).join('');
}

window.downloadArquivo = function(id, nome) {
    window.location.href = `/api/dvr/arquivos/${id}/download`;
    mostrarAlerta(`Iniciando download de "${nome}"...`, 'info');
};

window.excluirArquivo = async function(id, nome) {
    if (!confirm(`Deseja realmente excluir o arquivo "${nome}"?`)) return;
    
    try {
        const response = await fetch(`/api/dvr/arquivos/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Erro ao excluir');
        
        mostrarAlerta('Arquivo excluído com sucesso', 'success');
        carregarArquivos();
    } catch (error) {
        console.error('Erro ao excluir arquivo:', error);
        mostrarAlerta('Erro ao excluir arquivo', 'danger');
    }
};

async function fazerUpload() {
    const dvrSelect = document.getElementById('upload-dvr');
    const dvrOption = dvrSelect.options[dvrSelect.selectedIndex];
    const arquivoInput = document.getElementById('upload-arquivo');
    
    if (!dvrSelect.value || !arquivoInput.files[0]) {
        mostrarAlerta('Dispositivo e arquivo são obrigatórios', 'warning');
        return;
    }
    
    const formData = new FormData();
    formData.append('arquivo', arquivoInput.files[0]);
    formData.append('dvr_id', dvrSelect.value);
    formData.append('dvr_nome', dvrOption.dataset.dvrNome);
    formData.append('loja_nome', dvrOption.dataset.lojaNome);
    
    const canal = document.getElementById('upload-canal').value;
    const descricao = document.getElementById('upload-descricao').value;
    
    if (canal) formData.append('canal', canal);
    if (descricao) formData.append('descricao', descricao);
    
    try {
        const btnUpload = document.getElementById('btn-confirmar-upload');
        btnUpload.disabled = true;
        btnUpload.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Enviando...';
        
        const response = await fetch('/api/dvr/arquivos', {
            method: 'POST',
            body: formData
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Erro ao fazer upload');
        }
        
        bootstrap.Modal.getInstance(document.getElementById('modal-upload')).hide();
        mostrarAlerta('Arquivo enviado com sucesso', 'success');
        carregarArquivos();
        
        document.getElementById('form-upload').reset();
    } catch (error) {
        console.error('Erro ao fazer upload:', error);
        mostrarAlerta(error.message || 'Erro ao fazer upload', 'danger');
    } finally {
        const btnUpload = document.getElementById('btn-confirmar-upload');
        btnUpload.disabled = false;
        btnUpload.innerHTML = '<i class="bi bi-cloud-upload"></i> Fazer Upload';
    }
}

function formatarTamanho(bytes) {
    if (!bytes) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function mostrarAlerta(mensagem, tipo = 'info') {
    const alertaDiv = document.createElement('div');
    alertaDiv.className = `alert alert-${tipo} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3`;
    alertaDiv.style.zIndex = '9999';
    alertaDiv.innerHTML = `
        ${mensagem}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.body.appendChild(alertaDiv);
    
    setTimeout(() => {
        alertaDiv.remove();
    }, 5000);
}
