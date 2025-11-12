// Arquivo de preload para segurança do Electron
// Este arquivo atua como ponte entre o processo principal e o renderer

const { contextBridge } = require('electron');

// Expor APIs seguras para o renderer process
contextBridge.exposeInMainWorld('electronAPI', {
    platform: process.platform,
    versions: {
        node: process.versions.node,
        chrome: process.versions.chrome,
        electron: process.versions.electron
    },
    isElectron: true
});

console.log('🔐 Preload script carregado com segurança');
