const crypto = require('crypto');

function getClientIp(req) {
    return req.headers['x-forwarded-for']?.split(',')[0]?.trim() || 
           req.headers['x-real-ip'] || 
           req.connection?.remoteAddress || 
           req.socket?.remoteAddress ||
           'unknown';
}

function hashPayload(data) {
    if (!data || typeof data !== 'object') return null;
    const sanitized = JSON.stringify(data);
    return crypto.createHash('sha256').update(sanitized).digest('hex').substring(0, 16);
}

function logEvent(db, type, username, action, details, req = null) {
    const ip_address = req ? getClientIp(req) : null;
    const user_agent = req ? req.headers['user-agent'] : null;
    const route = req ? req.path : null;
    const event_type = type;
    const payload_hash = req ? hashPayload(req.body) : null;
    
    db.run(
        `INSERT INTO logs (type, username, action, details, ip_address, user_agent, event_type, route, payload_hash) 
         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`, 
        [type, username, action, details, ip_address, user_agent, event_type, route, payload_hash],
        (err) => { 
            if (err && !err.message.includes('no column named')) {
                console.error('Erro ao registrar log:', err.message); 
            }
        }
    );
}

module.exports = {
    logEvent,
    getClientIp,
    hashPayload
};
