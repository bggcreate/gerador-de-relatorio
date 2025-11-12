/**
 * Serviço de Integração com DVRs Intelbras via HTTP API
 * 
 * Este serviço se conecta diretamente aos DVRs Intelbras usando a API HTTP nativa
 * que eles expõem, sem precisar das DLLs do Windows.
 * 
 * Baseado no protocolo Dahua (usado pela Intelbras)
 */

const axios = require('axios');
const sqlite3 = require('sqlite3').verbose();

class IntelbrasDvrService {
    constructor(dbPath = './data/database.db') {
        this.db = new sqlite3.Database(dbPath);
    }

    /**
     * Conecta ao DVR e verifica se está online
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP (geralmente 80)
     * @param {string} username - Usuário do DVR
     * @param {string} password - Senha do DVR
     * @returns {Promise<boolean>}
     */
    async testConnection(ip, port, username, password) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/magicBox.cgi?action=getSystemInfo`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error(`Erro ao conectar ao DVR ${ip}:`, error.message);
            return false;
        }
    }

    /**
     * Obtém informações do sistema do DVR
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @returns {Promise<Object>}
     */
    async getSystemInfo(ip, port, username, password) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/magicBox.cgi?action=getSystemInfo`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });

            // Parse da resposta (formato: chave=valor por linha)
            const info = {};
            const lines = response.data.split('\n');
            lines.forEach(line => {
                const [key, value] = line.split('=');
                if (key && value) {
                    info[key.trim()] = value.trim();
                }
            });

            return info;
        } catch (error) {
            console.error(`Erro ao obter info do DVR ${ip}:`, error.message);
            return null;
        }
    }

    /**
     * Consulta logs/eventos do DVR
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {Date} startTime - Data/hora inicial
     * @param {Date} endTime - Data/hora final
     * @returns {Promise<Array>}
     */
    async getEvents(ip, port, username, password, startTime, endTime) {
        try {
            // Formata datas no formato que o DVR espera: YYYY-MM-DD HH:MM:SS
            const formatDate = (date) => {
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                const hour = String(date.getHours()).padStart(2, '0');
                const min = String(date.getMinutes()).padStart(2, '0');
                const sec = String(date.getSeconds()).padStart(2, '0');
                return `${year}-${month}-${day} ${hour}:${min}:${sec}`;
            };

            const url = `http://${ip}:${port}/cgi-bin/recordFinder.cgi?action=find&name=RecordFinder.factory.create`;
            const params = {
                StartTime: formatDate(startTime),
                EndTime: formatDate(endTime),
                Flags: 'Event',
                Events: JSON.stringify(['VideoMotion', 'VideoLoss', 'VideoBlind', 'AlarmLocal'])
            };

            const response = await axios.post(url, params, {
                auth: { username, password },
                timeout: 10000
            });

            return this.parseEventsResponse(response.data);
        } catch (error) {
            console.error(`Erro ao obter eventos do DVR ${ip}:`, error.message);
            return [];
        }
    }

    /**
     * Parse da resposta de eventos do DVR
     * @param {string} data - Resposta raw do DVR
     * @returns {Array}
     */
    parseEventsResponse(data) {
        const events = [];
        const lines = data.split('\n');
        let currentEvent = {};

        lines.forEach(line => {
            if (line.includes('items.found=')) {
                // Início de novo evento
                if (Object.keys(currentEvent).length > 0) {
                    events.push(currentEvent);
                }
                currentEvent = {};
            } else if (line.includes('=')) {
                const [key, value] = line.split('=');
                if (key && value) {
                    currentEvent[key.trim()] = value.trim();
                }
            }
        });

        if (Object.keys(currentEvent).length > 0) {
            events.push(currentEvent);
        }

        return events;
    }

    /**
     * Captura snapshot de uma câmera
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Número do canal (0-indexed)
     * @returns {Promise<Buffer>}
     */
    async getSnapshot(ip, port, username, password, channel = 0) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/snapshot.cgi?channel=${channel}`;
            const response = await axios.get(url, {
                auth: { username, password },
                responseType: 'arraybuffer',
                timeout: 10000
            });

            return Buffer.from(response.data);
        } catch (error) {
            console.error(`Erro ao capturar snapshot do DVR ${ip}:`, error.message);
            return null;
        }
    }

    /**
     * Monitora todos os DVRs cadastrados e atualiza seus status
     * @returns {Promise<void>}
     */
    async monitorAllDevices() {
        return new Promise((resolve, reject) => {
            this.db.all('SELECT * FROM dvr_dispositivos', async (err, devices) => {
                if (err) {
                    reject(err);
                    return;
                }

                for (const device of devices) {
                    const isOnline = await this.testConnection(
                        device.ip_address,
                        device.porta || 80,
                        device.usuario || 'admin',
                        '' // Senha não está armazenada por segurança
                    );

                    const newStatus = isOnline ? 'online' : 'offline';
                    const now = new Date().toISOString();

                    this.db.run(
                        'UPDATE dvr_dispositivos SET status = ?, ultima_conexao = ? WHERE id = ?',
                        [newStatus, now, device.id],
                        (err) => {
                            if (err) console.error(`Erro ao atualizar status do DVR ${device.id}:`, err);
                        }
                    );

                    // Registra evento de mudança de status
                    if (device.status !== newStatus) {
                        const tipoEvento = isOnline ? 'Conexão' : 'Desconexão';
                        const severidade = isOnline ? 'info' : 'error';
                        
                        this.db.run(
                            `INSERT INTO dvr_logs (dvr_id, dvr_nome, loja_nome, tipo_evento, descricao, severidade, data_hora)
                             VALUES (?, ?, ?, ?, ?, ?, ?)`,
                            [
                                device.id,
                                device.nome,
                                device.loja_nome,
                                tipoEvento,
                                `Dispositivo mudou para ${newStatus}`,
                                severidade,
                                now
                            ]
                        );
                    }
                }

                resolve();
            });
        });
    }

    /**
     * Coleta logs de um DVR específico
     * @param {number} dvrId - ID do DVR no banco
     * @param {string} password - Senha do DVR (não armazenada no banco)
     * @param {number} hoursBack - Quantas horas atrás buscar logs (padrão 24h)
     * @returns {Promise<number>} Quantidade de logs coletados
     */
    async collectLogsFromDevice(dvrId, password, hoursBack = 24) {
        return new Promise((resolve, reject) => {
            this.db.get('SELECT * FROM dvr_dispositivos WHERE id = ?', [dvrId], async (err, device) => {
                if (err) {
                    reject(err);
                    return;
                }

                if (!device) {
                    reject(new Error(`DVR ${dvrId} não encontrado`));
                    return;
                }

                const endTime = new Date();
                const startTime = new Date(endTime.getTime() - (hoursBack * 60 * 60 * 1000));

                const events = await this.getEvents(
                    device.ip_address,
                    device.porta || 80,
                    device.usuario || 'admin',
                    password,
                    startTime,
                    endTime
                );

                let count = 0;
                for (const event of events) {
                    const tipoEvento = this.mapEventType(event.Type || event.type);
                    const canal = parseInt(event.Channel || event.channel || 0);
                    const dataHora = event.StartTime || event.startTime || new Date().toISOString();

                    this.db.run(
                        `INSERT INTO dvr_logs (dvr_id, dvr_nome, loja_nome, tipo_evento, descricao, canal, severidade, data_hora, detalhes_json)
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
                        [
                            device.id,
                            device.nome,
                            device.loja_nome,
                            tipoEvento,
                            `Evento detectado: ${tipoEvento}`,
                            canal,
                            'info',
                            dataHora,
                            JSON.stringify(event)
                        ],
                        (err) => {
                            if (!err) count++;
                        }
                    );
                }

                console.log(`✅ Coletados ${count} logs do DVR ${device.nome}`);
                resolve(count);
            });
        });
    }

    /**
     * Mapeia tipos de evento do DVR para nomes amigáveis
     * @param {string} eventType - Tipo do evento raw
     * @returns {string}
     */
    mapEventType(eventType) {
        const map = {
            'VideoMotion': 'Detecção de Movimento',
            'VideoLoss': 'Perda de Vídeo',
            'VideoBlind': 'Vídeo Cego',
            'AlarmLocal': 'Alarme Local',
            'StorageNotExist': 'HD Não Encontrado',
            'StorageLowSpace': 'HD com Pouco Espaço',
            'NetAbort': 'Perda de Conexão de Rede'
        };
        return map[eventType] || eventType;
    }

    /**
     * CONTROLE PTZ - Movimenta câmeras PTZ
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal da câmera (0-indexed)
     * @param {string} direction - Direção: Up, Down, Left, Right, LeftUp, RightUp, LeftDown, RightDown
     * @param {string} action - Ação: start ou stop
     * @param {number} speed - Velocidade de movimento (1-8, padrão 4)
     * @returns {Promise<boolean>}
     */
    async ptzControl(ip, port, username, password, channel, direction, action = 'start', speed = 4) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/ptz.cgi?action=${action}&channel=${channel}&code=${direction}&arg1=0&arg2=${speed}&arg3=0`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error(`Erro ao controlar PTZ do DVR ${ip}:`, error.message);
            return false;
        }
    }

    /**
     * PRESET PTZ - Vai para uma posição pré-definida
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal da câmera (0-indexed)
     * @param {number} presetNumber - Número do preset (1-255)
     * @returns {Promise<boolean>}
     */
    async gotoPreset(ip, port, username, password, channel, presetNumber) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/ptz.cgi?action=start&channel=${channel}&code=GotoPreset&arg1=0&arg2=${presetNumber}&arg3=0`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error(`Erro ao ir para preset PTZ ${presetNumber}:`, error.message);
            return false;
        }
    }

    /**
     * SALVAR PRESET PTZ - Salva posição atual como preset
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal da câmera (0-indexed)
     * @param {number} presetNumber - Número do preset (1-255)
     * @returns {Promise<boolean>}
     */
    async setPreset(ip, port, username, password, channel, presetNumber) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/ptz.cgi?action=start&channel=${channel}&code=SetPreset&arg1=0&arg2=${presetNumber}&arg3=0`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error(`Erro ao salvar preset PTZ ${presetNumber}:`, error.message);
            return false;
        }
    }

    /**
     * BUSCAR GRAVAÇÕES - Busca arquivos de gravação por período
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal (1-indexed para API)
     * @param {Date} startTime - Data/hora inicial
     * @param {Date} endTime - Data/hora final
     * @returns {Promise<Array>} Lista de arquivos encontrados
     */
    async findRecordings(ip, port, username, password, channel, startTime, endTime) {
        try {
            // Formata datas: YYYY-MM-DD HH:MM:SS
            const formatDate = (date) => {
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                const hour = String(date.getHours()).padStart(2, '0');
                const min = String(date.getMinutes()).padStart(2, '0');
                const sec = String(date.getSeconds()).padStart(2, '0');
                return `${year}-${month}-${day} ${hour}:${min}:${sec}`;
            };

            // Passo 1: Criar objeto de busca
            const createUrl = `http://${ip}:${port}/cgi-bin/mediaFileFind.cgi?action=factory.create`;
            const createResponse = await axios.get(createUrl, {
                auth: { username, password },
                timeout: 5000
            });

            // Parse da resposta (pode ser JSON ou formato key=value)
            let objectId = null;
            try {
                if (typeof createResponse.data === 'object') {
                    objectId = createResponse.data.objectId || createResponse.data.id || createResponse.data.object;
                } else {
                    // Resposta em formato texto: result=true\nobject=12345
                    const lines = String(createResponse.data).split('\n');
                    for (const line of lines) {
                        if (line.includes('object=') || line.includes('id=')) {
                            objectId = line.split('=')[1]?.trim();
                            break;
                        }
                    }
                }
            } catch (e) {
                console.error('Erro ao parsear resposta factory.create:', e);
            }

            if (!objectId) {
                throw new Error('Falha ao criar objeto de busca - ID não encontrado na resposta');
            }

            // Passo 2: Buscar arquivos
            const findUrl = `http://${ip}:${port}/cgi-bin/mediaFileFind.cgi?action=findFile&object=${objectId}&condition.Channel=${channel}&condition.StartTime=${encodeURIComponent(formatDate(startTime))}&condition.EndTime=${encodeURIComponent(formatDate(endTime))}&condition.Dirs[0]=/mnt/sd&condition.Types[0]=dav&condition.Events[0]=All`;
            const findResponse = await axios.get(findUrl, {
                auth: { username, password },
                timeout: 10000
            });

            // Validar resultado da busca
            if (typeof findResponse.data === 'string' && findResponse.data.includes('result=false')) {
                console.warn(`Busca de arquivos retornou result=false para DVR ${ip}`);
            }

            // Passo 3: Obter lista de arquivos
            const nextUrl = `http://${ip}:${port}/cgi-bin/mediaFileFind.cgi?action=findNextFile&object=${objectId}&count=100`;
            const filesResponse = await axios.get(nextUrl, {
                auth: { username, password },
                timeout: 10000
            });

            // Validar resultado
            if (typeof filesResponse.data === 'string' && filesResponse.data.includes('result=false')) {
                console.warn(`findNextFile retornou result=false para DVR ${ip}`);
                return [];
            }

            // Passo 4: Fechar objeto de busca
            const closeUrl = `http://${ip}:${port}/cgi-bin/mediaFileFind.cgi?action=close&object=${objectId}`;
            await axios.get(closeUrl, {
                auth: { username, password },
                timeout: 5000
            });

            // Parse da lista de arquivos
            const files = this.parseMediaFileListResponse(filesResponse.data);
            return files;
        } catch (error) {
            console.error(`Erro ao buscar gravações do DVR ${ip}:`, error.message);
            return [];
        }
    }

    /**
     * OBTER URL RTSP - Gera URL para streaming ao vivo
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta RTSP (padrão 554)
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal (1-indexed)
     * @param {number} subtype - Tipo de stream: 0 = principal, 1 = sub-stream
     * @returns {string} URL RTSP
     */
    getRtspUrl(ip, port = 554, username, password, channel, subtype = 0) {
        return `rtsp://${username}:${password}@${ip}:${port}/cam/realmonitor?channel=${channel}&subtype=${subtype}`;
    }

    /**
     * OBTER URL RTSP PLAYBACK - Gera URL para reprodução de gravação
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta RTSP (padrão 554)
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal (1-indexed)
     * @param {Date} startTime - Horário inicial
     * @param {Date} endTime - Horário final
     * @returns {string} URL RTSP para playback
     */
    getRtspPlaybackUrl(ip, port = 554, username, password, channel, startTime, endTime) {
        // Formato: YYYY_MM_DD_HH_MM_SS
        const formatTime = (date) => {
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            const hour = String(date.getHours()).padStart(2, '0');
            const min = String(date.getMinutes()).padStart(2, '0');
            const sec = String(date.getSeconds()).padStart(2, '0');
            return `${year}_${month}_${day}_${hour}_${min}_${sec}`;
        };

        const start = formatTime(startTime);
        const end = formatTime(endTime);
        return `rtsp://${username}:${password}@${ip}:${port}/cam/playback?channel=${channel}&starttime=${start}&endtime=${end}`;
    }

    /**
     * OBTER INFORMAÇÕES DOS CANAIS
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @returns {Promise<Object>} Informações dos canais
     */
    async getChannelInfo(ip, port, username, password) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/configManager.cgi?action=getConfig&name=ChannelTitle`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });

            // Parse da resposta
            const info = {};
            const lines = response.data.split('\n');
            lines.forEach(line => {
                const [key, value] = line.split('=');
                if (key && value) {
                    info[key.trim()] = value.trim();
                }
            });

            return info;
        } catch (error) {
            console.error(`Erro ao obter info dos canais do DVR ${ip}:`, error.message);
            return null;
        }
    }

    /**
     * CONTROLAR ZOOM
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal da câmera (0-indexed)
     * @param {string} zoomAction - Ação: ZoomWide (afastar) ou ZoomTele (aproximar)
     * @param {string} action - start ou stop
     * @returns {Promise<boolean>}
     */
    async ptzZoom(ip, port, username, password, channel, zoomAction, action = 'start') {
        try {
            const url = `http://${ip}:${port}/cgi-bin/ptz.cgi?action=${action}&channel=${channel}&code=${zoomAction}&arg1=0&arg2=0&arg3=0`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error(`Erro ao controlar zoom PTZ:`, error.message);
            return false;
        }
    }

    /**
     * OBTER STATUS PTZ
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal da câmera (0-indexed)
     * @returns {Promise<Object>} Status PTZ (posição, zoom, etc)
     */
    async getPtzStatus(ip, port, username, password, channel) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/ptz.cgi?action=getStatus&channel=${channel}`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });

            // Parse da resposta
            const status = {};
            const lines = response.data.split('\n');
            lines.forEach(line => {
                const [key, value] = line.split('=');
                if (key && value) {
                    status[key.trim()] = value.trim();
                }
            });

            return status;
        } catch (error) {
            console.error(`Erro ao obter status PTZ:`, error.message);
            return null;
        }
    }

    /**
     * INICIAR GRAVAÇÃO MANUAL
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal (0-indexed)
     * @returns {Promise<boolean>}
     */
    async startRecording(ip, port, username, password, channel) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/recordControl.cgi?action=start&channel=${channel}`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error(`Erro ao iniciar gravação:`, error.message);
            return false;
        }
    }

    /**
     * PARAR GRAVAÇÃO MANUAL
     * @param {string} ip - IP do DVR
     * @param {number} port - Porta HTTP
     * @param {string} username - Usuário
     * @param {string} password - Senha
     * @param {number} channel - Canal (0-indexed)
     * @returns {Promise<boolean>}
     */
    async stopRecording(ip, port, username, password, channel) {
        try {
            const url = `http://${ip}:${port}/cgi-bin/recordControl.cgi?action=stop&channel=${channel}`;
            const response = await axios.get(url, {
                auth: { username, password },
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error(`Erro ao parar gravação:`, error.message);
            return false;
        }
    }

    /**
     * Parse da resposta de lista de arquivos de mídia (formato key=value do DVR)
     * Exemplo: found=2\ninfos[0].FilePath=/mnt/sd/...\ninfos[0].StartTime=...\ninfos[1].FilePath=...
     * @param {string|Object} data - Resposta raw do DVR
     * @returns {Array} Lista de arquivos parseados
     */
    parseMediaFileListResponse(data) {
        try {
            // Se já vier como JSON, retornar diretamente
            if (typeof data === 'object') {
                if (data.infos) return data.infos;
                if (Array.isArray(data)) return data;
                return [];
            }

            // Parse do formato key=value
            const lines = String(data).split('\n');
            const filesMap = new Map();
            let itemCount = 0;

            for (const line of lines) {
                if (!line.includes('=')) continue;

                const [key, value] = line.split('=', 2);
                const trimmedKey = key.trim();
                const trimmedValue = value?.trim();

                // Pegar quantidade de arquivos encontrados (aceita: found, count, items.found)
                if (trimmedKey === 'found' || trimmedKey === 'count' || trimmedKey === 'items.found') {
                    itemCount = parseInt(trimmedValue) || 0;
                    continue;
                }

                // Parse de infos[N].Field=Value
                const match = trimmedKey.match(/infos\[(\d+)\]\.(.+)/);
                if (match) {
                    const index = parseInt(match[1]);
                    const field = match[2];

                    if (!filesMap.has(index)) {
                        filesMap.set(index, {});
                    }

                    const fileObj = filesMap.get(index);
                    fileObj[field] = trimmedValue;
                }
            }

            // Se itemCount ainda for 0, usar tamanho do Map
            if (itemCount === 0 && filesMap.size > 0) {
                itemCount = filesMap.size;
            }

            // Converter Map para Array
            const files = [];
            for (let i = 0; i < itemCount; i++) {
                if (filesMap.has(i)) {
                    files.push(filesMap.get(i));
                }
            }

            return files;
        } catch (error) {
            console.error('Erro ao parsear lista de arquivos:', error);
            return [];
        }
    }

    /**
     * Fecha a conexão com o banco de dados
     */
    close() {
        this.db.close();
    }
}

module.exports = IntelbrasDvrService;
