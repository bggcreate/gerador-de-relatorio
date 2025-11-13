const googleDriveService = require('../../../services/googleDriveService');

async function getDriveUsage(req, res) {
    try {
        const quota = await googleDriveService.verificarQuota();
        
        if (!quota) {
            return res.status(500).json({ error: 'Erro ao verificar quota do Drive' });
        }

        res.json({
            usado: quota.usado,
            limite: quota.limite,
            usadoGB: quota.usadoGB,
            limiteGB: quota.limiteGB,
            percentual: quota.percentual,
            precisaBackup: quota.precisaBackup,
            disponivelGB: quota.limiteGB - quota.usadoGB
        });
    } catch (error) {
        console.error('Erro ao buscar uso do Drive:', error);
        res.status(500).json({ error: 'Erro ao buscar informações do Drive' });
    }
}

async function getDriveTimeline(req, res) {
    try {
        const { periodo = '30' } = req.query;
        const dias = parseInt(periodo, 10);
        
        const relatorios = await googleDriveService.listarRelatorios();
        
        const hoje = new Date();
        const dataInicio = new Date();
        dataInicio.setDate(dataInicio.getDate() - dias);
        
        const timeline = {};
        
        for (let d = new Date(dataInicio); d <= hoje; d.setDate(d.getDate() + 1)) {
            const dataStr = d.toISOString().split('T')[0];
            timeline[dataStr] = 0;
        }
        
        relatorios.forEach(rel => {
            if (rel.dataCriacao) {
                const dataStr = new Date(rel.dataCriacao).toISOString().split('T')[0];
                if (timeline[dataStr] !== undefined) {
                    timeline[dataStr]++;
                }
            }
        });
        
        const labels = Object.keys(timeline).sort();
        const data = labels.map(label => timeline[label]);
        
        res.json({
            labels,
            data,
            totalArquivos: relatorios.length
        });
    } catch (error) {
        console.error('Erro ao buscar timeline do Drive:', error);
        res.status(500).json({ error: 'Erro ao buscar timeline' });
    }
}

async function getLastSync(req, res) {
    try {
        const db = require('../../../src/config/db-adapter').getDatabase();
        
        db.get(
            `SELECT created_at FROM relatorios ORDER BY created_at DESC LIMIT 1`,
            [],
            (err, row) => {
                if (err) {
                    return res.status(500).json({ error: 'Erro ao verificar última sincronização' });
                }
                
                res.json({
                    lastSync: row ? row.created_at : null,
                    status: row ? 'ok' : 'sem_dados'
                });
            }
        );
    } catch (error) {
        console.error('Erro ao verificar última sincronização:', error);
        res.status(500).json({ error: 'Erro ao verificar sincronização' });
    }
}

module.exports = {
    getDriveUsage,
    getDriveTimeline,
    getLastSync
};
