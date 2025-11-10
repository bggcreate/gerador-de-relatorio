/**
 * Script para Coletar Logs dos DVRs Intelbras
 * 
 * Execute: node scripts/collect-dvr-logs.js
 * 
 * Este script:
 * 1. Verifica status de todos os DVRs
 * 2. Coleta logs das últimas 24h de cada DVR online
 * 3. Salva os logs no banco de dados
 */

const IntelbrasDvrService = require('../services/intelbrasDvrService');

// CONFIGURAÇÃO: Adicione as senhas dos DVRs aqui
// (não armazenamos senhas no banco por segurança)
const DVR_PASSWORDS = {
    // ID do DVR: senha
    1: 'senha_dvr_1',  // Substitua pela senha real
    2: 'senha_dvr_2',  // Substitua pela senha real
    3: 'senha_dvr_3',  // Substitua pela senha real
};

async function main() {
    console.log('🔄 Iniciando coleta de logs dos DVRs Intelbras...\n');

    const service = new IntelbrasDvrService();

    try {
        // 1. Monitorar todos os dispositivos e atualizar status
        console.log('📡 Verificando status de todos os DVRs...');
        await service.monitorAllDevices();
        console.log('✅ Status atualizado\n');

        // 2. Coletar logs de cada DVR
        const dvrIds = Object.keys(DVR_PASSWORDS);
        let totalLogs = 0;

        for (const dvrId of dvrIds) {
            const password = DVR_PASSWORDS[dvrId];
            
            console.log(`📥 Coletando logs do DVR ${dvrId}...`);
            
            try {
                const count = await service.collectLogsFromDevice(
                    parseInt(dvrId),
                    password,
                    24 // Últimas 24 horas
                );
                
                totalLogs += count;
                console.log(`   ✅ ${count} logs coletados\n`);
            } catch (error) {
                console.error(`   ❌ Erro ao coletar logs do DVR ${dvrId}:`, error.message, '\n');
            }
        }

        console.log(`\n🎉 Coleta concluída! Total de ${totalLogs} logs coletados`);

    } catch (error) {
        console.error('❌ Erro durante a coleta:', error);
    } finally {
        service.close();
    }
}

// Executar
main().catch(console.error);
