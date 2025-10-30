/**
 * Middleware de Autorização baseado em Roles
 * Sistema de controle de acesso para diferentes tipos de usuários
 */

// Definição de roles disponíveis no sistema
const ROLES = {
    DEV: 'dev',
    ADMIN: 'admin',
    MONITORAMENTO: 'monitoramento',
    CONSULTOR: 'consultor',
    GERENTE: 'gerente',
    TECNICO: 'tecnico'
};

// Definição de permissões por role
const PERMISSIONS = {
    [ROLES.DEV]: {
        canAccessAllData: true,
        canManageUsers: true,
        canManageLojas: true,
        canCreateReports: true,
        canViewLogs: true,
        canAccessDemandas: true,
        canExportData: true,
        canManageAssistencia: true,
        canManageEstoque: true,
        menuItems: ['dashboard', 'consulta', 'novo-relatorio', 'lojas', 'demandas', 'assistencia', 'gerenciar-usuarios', 'logs', 'backup']
    },
    [ROLES.ADMIN]: {
        canAccessAllData: true,
        canManageUsers: true,
        canManageLojas: true,
        canCreateReports: true,
        canViewLogs: false,
        canAccessDemandas: true,
        canExportData: true,
        canManageAssistencia: true,
        canManageEstoque: true,
        menuItems: ['dashboard', 'consulta', 'novo-relatorio', 'lojas', 'demandas', 'assistencia', 'gerenciar-usuarios']
    },
    [ROLES.MONITORAMENTO]: {
        canAccessAllData: true,
        canManageUsers: false,
        canManageLojas: false,
        canCreateReports: true,
        canViewLogs: false,
        canAccessDemandas: true,
        canExportData: true,
        canViewAssistencia: true,
        menuItems: ['dashboard', 'consulta', 'novo-relatorio', 'lojas', 'demandas', 'assistencia']
    },
    [ROLES.CONSULTOR]: {
        canAccessAllData: false,
        canManageUsers: false,
        canManageLojas: false,
        canCreateReports: false,
        canViewLogs: false,
        canAccessDemandas: true,
        canExportData: true,
        canManageVendedores: true,
        canViewAssistencia: true,
        canDeleteConcluidas: true,
        menuItems: ['dashboard', 'consulta', 'lojas', 'demandas', 'assistencia']
    },
    [ROLES.GERENTE]: {
        canAccessAllData: false,
        canManageUsers: false,
        canManageLojas: false,
        canCreateReports: false,
        canViewLogs: false,
        canAccessDemandas: true,
        canExportData: false,
        canManageVendedores: true,
        canViewAssistencia: true,
        canDeleteConcluidas: true,
        menuItems: ['dashboard', 'lojas', 'demandas', 'assistencia']  // Removido 'consulta' - gerente não acessa relatórios de monitoramento
    },
    [ROLES.TECNICO]: {
        canAccessAllData: false,
        canManageUsers: false,
        canManageLojas: false,
        canCreateReports: false,
        canViewLogs: false,
        canAccessDemandas: false,
        canExportData: false,
        canManageAssistencia: true,
        canManageEstoque: true,
        menuItems: ['alertas-tecnico', 'assistencia']
    }
};

/**
 * Middleware para verificar se o usuário tem um dos roles permitidos
 * @param {Array<string>} allowedRoles - Array de roles permitidos
 */
function requireRole(allowedRoles) {
    return (req, res, next) => {
        if (!req.session || !req.session.role) {
            return res.status(401).json({ error: 'Não autenticado.' });
        }

        if (allowedRoles.includes(req.session.role)) {
            return next();
        }

        return res.status(403).json({ error: 'Acesso negado. Permissões insuficientes.' });
    };
}

/**
 * Middleware para verificar se usuário pode acessar dados de uma loja específica
 */
function canAccessLoja(db) {
    return (req, res, next) => {
        const userRole = req.session.role;
        const userId = req.session.userId;
        
        // Dev e Admin têm acesso total
        if (userRole === ROLES.DEV || userRole === ROLES.ADMIN || userRole === ROLES.MONITORAMENTO) {
            return next();
        }

        // Gerente e Consultor precisam verificar vínculos
        const lojaNome = req.query.loja || req.body.loja || req.params.loja;
        
        if (!lojaNome) {
            // Se não especificou loja, deixa passar - será filtrado depois
            return next();
        }

        // Buscar vínculos do usuário
        db.get('SELECT loja_gerente, lojas_consultor FROM usuarios WHERE id = ?', [userId], (err, user) => {
            if (err || !user) {
                return res.status(500).json({ error: 'Erro ao verificar permissões.' });
            }

            let hasAccess = false;

            if (userRole === ROLES.GERENTE && user.loja_gerente === lojaNome) {
                hasAccess = true;
            } else if (userRole === ROLES.CONSULTOR && user.lojas_consultor) {
                const lojas = user.lojas_consultor.split(',').map(l => l.trim());
                hasAccess = lojas.includes(lojaNome);
            }

            if (hasAccess) {
                return next();
            } else {
                return res.status(403).json({ error: 'Acesso negado a esta loja.' });
            }
        });
    };
}

/**
 * Retorna filtro SQL baseado no role do usuário
 */
function getLojaFilter(role, loja_gerente, lojas_consultor, loja_tecnico) {
    // Se não tem role, trata como admin (compatibilidade com sessões antigas)
    if (!role) {
        return null;
    }

    // Dev, Admin e Monitoramento veem tudo
    if (role === ROLES.DEV || role === ROLES.ADMIN || role === ROLES.MONITORAMENTO) {
        return null;
    }

    // Gerente - filtrar por uma loja
    if (role === ROLES.GERENTE && loja_gerente) {
        return {
            clause: 'TRIM(loja) = ?',
            params: [loja_gerente.trim()]
        };
    }

    // Consultor - filtrar por múltiplas lojas
    if (role === ROLES.CONSULTOR && lojas_consultor) {
        const lojas = lojas_consultor.split(',').map(l => l.trim()).filter(l => l);
        if (lojas.length > 0) {
            const placeholders = lojas.map(() => '?').join(',');
            return {
                clause: `TRIM(loja) IN (${placeholders})`,
                params: lojas
            };
        }
    }

    // Técnico - filtrar por uma loja
    if (role === ROLES.TECNICO && loja_tecnico) {
        return {
            clause: 'TRIM(loja) = ?',
            params: [loja_tecnico.trim()]
        };
    }

    // Sem acesso
    return null;
}

/**
 * Retorna permissões do role no formato esperado pelo frontend
 */
function getPermissions(role) {
    const rolePermissions = PERMISSIONS[role] || PERMISSIONS[ROLES.GERENTE];
    
    // Converter menuItems array para objeto com propriedades individuais
    const menuPermissions = {};
    rolePermissions.menuItems.forEach(item => {
        menuPermissions[item] = true;
    });
    
    return {
        ...rolePermissions,
        ...menuPermissions
    };
}

/**
 * Middleware para verificar se usuário pode acessar uma página
 */
function requirePage(allowedMenuItems) {
    return (req, res, next) => {
        if (!req.session || !req.session.role) {
            return res.redirect('/login');
        }

        const permissions = getPermissions(req.session.role);
        const hasAccess = allowedMenuItems.some(item => permissions.menuItems.includes(item));

        if (hasAccess) {
            return next();
        }

        return res.redirect('/403');
    };
}

module.exports = {
    ROLES,
    PERMISSIONS,
    requireRole,
    canAccessLoja,
    getLojaFilter,
    getPermissions,
    requirePage
};
