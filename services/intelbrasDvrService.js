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
     * Fecha a conexão com o banco de dados
     */
    close() {
        this.db.close();
    }
}

module.exports = IntelbrasDvrService;
