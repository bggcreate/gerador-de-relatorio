const { app, BrowserWindow, Menu, dialog } = require('electron');
const path = require('path');
const { spawn } = require('child_process');
const http = require('http');

let mainWindow;
let serverProcess = null;

// =================================================================
// CONFIGURAÇÃO DO SERVIDOR
// =================================================================
// Para usar SERVIDOR REMOTO (Replit Deploy):
// 1. Altere USE_REMOTE_SERVER para true
// 2. Coloque a URL do seu deploy no REMOTE_SERVER_URL
// 3. Gere os executáveis: npm run build:electron:win
//
// Para usar SERVIDOR LOCAL (cada PC tem seu próprio banco):
// 1. Mantenha USE_REMOTE_SERVER como false
// =================================================================

const USE_REMOTE_SERVER = false; // Mude para true para conectar ao Replit
const REMOTE_SERVER_URL = 'http://localhost:5000'; // Cole a URL do Replit aqui
const LOCAL_PORT = 5000;

// Verificar se o servidor está respondendo
function checkServerReady(port, maxAttempts = 30) {
    return new Promise((resolve, reject) => {
        let attempts = 0;
        
        const check = () => {
            attempts++;
            
            const req = http.get(`http://localhost:${port}`, (res) => {
                console.log(`✅ Servidor respondendo na porta ${port}!`);
                resolve();
            });
            
            req.on('error', (err) => {
                if (attempts >= maxAttempts) {
                    reject(new Error(`Servidor não respondeu após ${maxAttempts} tentativas`));
                } else {
                    setTimeout(check, 500); // Tenta novamente em 500ms
                }
            });
            
            req.end();
        };
        
        check();
    });
}

// Iniciar o servidor Express
function startServer() {
    return new Promise((resolve, reject) => {
        console.log('Iniciando servidor backend...');
        
        const serverPath = path.join(__dirname, 'server.js');
        
        // Spawn Electron como Node puro usando ELECTRON_RUN_AS_NODE
        serverProcess = spawn(process.execPath, [serverPath], {
            cwd: __dirname,
            env: { 
                ...process.env, 
                ELECTRON_RUN_AS_NODE: '1',  // Roda como Node puro
                PORT: String(LOCAL_PORT)
            },
            stdio: ['ignore', 'pipe', 'pipe'],
            windowsHide: true  // Oculta janela no Windows
        });

        serverProcess.stdout.on('data', (data) => {
            console.log(`[Backend] ${data.toString().trim()}`);
        });

        serverProcess.stderr.on('data', (data) => {
            console.error(`[Backend Error] ${data.toString().trim()}`);
        });

        serverProcess.on('error', (error) => {
            console.error('❌ Erro ao iniciar processo do servidor:', error);
            reject(error);
        });

        serverProcess.on('close', (code) => {
            console.log(`Servidor encerrado com código ${code}`);
            serverProcess = null;
        });

        // Aguarda o servidor estar realmente pronto
        checkServerReady(LOCAL_PORT)
            .then(() => {
                console.log('✅ Servidor backend iniciado com sucesso!');
                resolve();
            })
            .catch((error) => {
                console.error('❌ Servidor não ficou pronto:', error);
                if (serverProcess) {
                    serverProcess.kill();
                    serverProcess = null;
                }
                reject(error);
            });
    });
}

// Criar a janela principal
function createWindow() {
    mainWindow = new BrowserWindow({
        width: 1400,
        height: 900,
        minWidth: 1200,
        minHeight: 700,
        icon: path.join(__dirname, 'build', 'icon.png'),
        webPreferences: {
            nodeIntegration: false,
            contextIsolation: true,
            preload: path.join(__dirname, 'electron-preload.js')
        },
        backgroundColor: '#f5f5f5',
        show: false, // Não mostrar até estar pronto
        autoHideMenuBar: false
    });

    // Menu personalizado
    const template = [
        {
            label: 'Arquivo',
            submenu: [
                {
                    label: 'Recarregar',
                    accelerator: 'F5',
                    click: () => mainWindow.reload()
                },
                { type: 'separator' },
                {
                    label: 'Sair',
                    accelerator: 'Alt+F4',
                    click: () => app.quit()
                }
            ]
        },
        {
            label: 'Editar',
            submenu: [
                { role: 'undo', label: 'Desfazer' },
                { role: 'redo', label: 'Refazer' },
                { type: 'separator' },
                { role: 'cut', label: 'Recortar' },
                { role: 'copy', label: 'Copiar' },
                { role: 'paste', label: 'Colar' },
                { role: 'selectAll', label: 'Selecionar Tudo' }
            ]
        },
        {
            label: 'Visualizar',
            submenu: [
                { role: 'reload', label: 'Recarregar' },
                { role: 'forceReload', label: 'Forçar Recarga' },
                { role: 'toggleDevTools', label: 'Ferramentas do Desenvolvedor' },
                { type: 'separator' },
                { role: 'resetZoom', label: 'Zoom Padrão' },
                { role: 'zoomIn', label: 'Aumentar Zoom' },
                { role: 'zoomOut', label: 'Diminuir Zoom' },
                { type: 'separator' },
                { role: 'togglefullscreen', label: 'Tela Cheia' }
            ]
        },
        {
            label: 'Ajuda',
            submenu: [
                {
                    label: 'Sobre',
                    click: () => {
                        dialog.showMessageBox(mainWindow, {
                            type: 'info',
                            title: 'Sistema de Relatórios',
                            message: 'Sistema de Relatórios - Versão 1.0.0',
                            detail: 'Sistema profissional para gestão de lojas e relatórios.\n\nDesenvolvido com Node.js + Electron',
                            buttons: ['OK']
                        });
                    }
                },
                {
                    label: 'Documentação',
                    click: () => {
                        dialog.showMessageBox(mainWindow, {
                            type: 'info',
                            title: 'Documentação',
                            message: 'Documentação do Sistema',
                            detail: 'Usuário padrão: admin\nSenha padrão: admin\n\nPara mais informações, consulte o README.md',
                            buttons: ['OK']
                        });
                    }
                }
            ]
        }
    ];

    const menu = Menu.buildFromTemplate(template);
    Menu.setApplicationMenu(menu);

    // Carregar a aplicação (local ou remota)
    const serverUrl = USE_REMOTE_SERVER ? REMOTE_SERVER_URL : `http://localhost:${LOCAL_PORT}`;
    console.log(`📡 Conectando ao servidor: ${serverUrl}`);
    mainWindow.loadURL(serverUrl);

    // Mostrar janela quando estiver pronta
    mainWindow.once('ready-to-show', () => {
        mainWindow.show();
        console.log('✅ Aplicação iniciada!');
    });

    // Tratar erro de carregamento
    mainWindow.webContents.on('did-fail-load', () => {
        console.error('❌ Falha ao carregar a aplicação');
        const serverUrl = USE_REMOTE_SERVER ? REMOTE_SERVER_URL : `http://localhost:${LOCAL_PORT}`;
        setTimeout(() => {
            mainWindow.loadURL(serverUrl);
        }, 1000);
    });

    mainWindow.on('closed', () => {
        mainWindow = null;
    });
}

// Quando o Electron estiver pronto
app.whenReady().then(async () => {
    try {
        // Iniciar servidor local apenas se não estiver usando servidor remoto
        if (!USE_REMOTE_SERVER) {
            console.log('🚀 Iniciando servidor local...');
            await startServer();
            await new Promise(resolve => setTimeout(resolve, 1500));
        } else {
            console.log('🌐 Modo servidor remoto ativado');
        }
        
        // Criar janela
        createWindow();
    } catch (error) {
        console.error('Erro ao iniciar aplicação:', error);
        app.quit();
    }

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

// Fechar o app quando todas as janelas forem fechadas
app.on('window-all-closed', () => {
    // Encerrar o servidor se estiver rodando
    if (serverProcess) {
        console.log('Encerrando servidor...');
        serverProcess.kill();
        serverProcess = null;
    }
    
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

// Garantir que o servidor seja encerrado antes de fechar
app.on('before-quit', () => {
    if (serverProcess) {
        serverProcess.kill();
        serverProcess = null;
    }
});

// Tratar erros não capturados
process.on('uncaughtException', (error) => {
    console.error('Erro não capturado:', error);
});
