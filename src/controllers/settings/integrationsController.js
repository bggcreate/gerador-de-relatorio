const googleDriveService = require('../../../services/googleDriveService');

async function getIntegrationsStatus(req, res) {
    try {
        const integrations = [];

        const googleDriveEnabled = !!(
            process.env.GOOGLE_CLIENT_ID &&
            process.env.GOOGLE_CLIENT_SECRET &&
            process.env.GOOGLE_REFRESH_TOKEN
        );

        let driveStatus = 'disconnected';
        let driveLastCheck = null;
        
        if (googleDriveEnabled) {
            try {
                const quota = await googleDriveService.verificarQuota();
                if (quota) {
                    driveStatus = 'connected';
                    driveLastCheck = new Date().toISOString();
                }
            } catch (error) {
                driveStatus = 'error';
            }
        }

        integrations.push({
            name: 'Google Drive',
            type: 'storage',
            status: driveStatus,
            enabled: googleDriveEnabled,
            lastCheck: driveLastCheck,
            details: googleDriveEnabled ? {
                hasClientId: !!process.env.GOOGLE_CLIENT_ID,
                hasClientSecret: !!process.env.GOOGLE_CLIENT_SECRET,
                hasRefreshToken: !!process.env.GOOGLE_REFRESH_TOKEN
            } : null
        });

        const emailEnabled = !!(
            process.env.EMAIL_REMETENTE &&
            process.env.GOOGLE_REFRESH_TOKEN
        );

        integrations.push({
            name: 'Gmail (Email)',
            type: 'email',
            status: emailEnabled ? 'connected' : 'disconnected',
            enabled: emailEnabled,
            lastCheck: emailEnabled ? new Date().toISOString() : null,
            details: emailEnabled ? {
                emailFrom: process.env.EMAIL_REMETENTE
            } : null
        });

        const postgresEnabled = !!(
            process.env.PGHOST &&
            process.env.PGDATABASE &&
            process.env.PGUSER &&
            process.env.PGPASSWORD
        );

        integrations.push({
            name: 'PostgreSQL',
            type: 'database',
            status: postgresEnabled ? 'connected' : 'disconnected',
            enabled: postgresEnabled,
            lastCheck: postgresEnabled ? new Date().toISOString() : null,
            details: postgresEnabled ? {
                host: process.env.PGHOST,
                database: process.env.PGDATABASE
            } : null
        });

        res.json({ integrations });

    } catch (error) {
        console.error('Erro ao verificar integrações:', error);
        res.status(500).json({ error: 'Erro ao verificar status das integrações' });
    }
}

module.exports = {
    getIntegrationsStatus
};
