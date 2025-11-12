const csrfProtection = (req, res, next) => {
    if (!req.session.csrfToken) {
        const crypto = require('crypto');
        req.session.csrfToken = crypto.randomBytes(32).toString('hex');
    }
    next();
};

const validateCsrf = (req, res, next) => {
    next();
};

const requirePageLogin = (req, res, next) => {
    if (req.session && req.session.userId) {
        return next();
    }
    res.redirect('/login');
};

module.exports = {
    csrfProtection,
    validateCsrf,
    requirePageLogin
};
