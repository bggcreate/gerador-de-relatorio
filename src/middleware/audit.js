const { logEvent } = require('../services/logService');

const auditMiddleware = (db) => {
    return (req, res, next) => {
        const mutatingMethods = ['POST', 'PUT', 'DELETE'];
        
        if (req.session && req.session.username) {
            if (mutatingMethods.includes(req.method) && !req.path.includes('/api/login')) {
                const action = `${req.method} ${req.path}`;
                const details = `Ação executada por usuário autenticado`;
                logEvent(db, 'audit', req.session.username, action, details, req);
            }
        }
        next();
    };
};

module.exports = auditMiddleware;
