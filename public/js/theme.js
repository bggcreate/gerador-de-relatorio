// ============================================================
// GERENCIADOR DE TEMA DARK/LIGHT MODE
// Inspirado no estilo Apple com transições suaves
// ============================================================

class ThemeManager {
    constructor() {
        this.THEME_KEY = 'theme-preference';
        this.currentTheme = this.getStoredTheme() || 'light';
        this.init();
    }

    init() {
        // Aplica tema inicial sem transição
        document.documentElement.setAttribute('data-theme', this.currentTheme);
        
        // Aguarda o DOM estar pronto
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.setupToggleButton());
        } else {
            this.setupToggleButton();
        }
    }

    setupToggleButton() {
        const toggleBtnDesktop = document.getElementById('theme-toggle-desktop');
        const toggleBtnMobile = document.getElementById('theme-toggle-mobile');
        
        if (toggleBtnDesktop && !toggleBtnDesktop.dataset.themeInitialized) {
            this.updateButtonIcon(toggleBtnDesktop);
            toggleBtnDesktop.addEventListener('click', () => this.toggle());
            toggleBtnDesktop.dataset.themeInitialized = 'true';
        } else if (toggleBtnDesktop) {
            // Apenas atualiza o ícone se já foi inicializado
            this.updateButtonIcon(toggleBtnDesktop);
        }
        
        if (toggleBtnMobile && !toggleBtnMobile.dataset.themeInitialized) {
            this.updateButtonIcon(toggleBtnMobile);
            toggleBtnMobile.addEventListener('click', () => this.toggle());
            toggleBtnMobile.dataset.themeInitialized = 'true';
        } else if (toggleBtnMobile) {
            // Apenas atualiza o ícone se já foi inicializado
            this.updateButtonIcon(toggleBtnMobile);
        }
    }

    toggle() {
        this.currentTheme = this.currentTheme === 'light' ? 'dark' : 'light';
        document.documentElement.setAttribute('data-theme', this.currentTheme);
        this.saveTheme();
        this.updateAllButtons();
    }

    updateButtonIcon(button) {
        const icon = button.querySelector('i');
        if (icon) {
            if (this.currentTheme === 'dark') {
                icon.className = 'bi bi-sun-fill';
            } else {
                icon.className = 'bi bi-moon-fill';
            }
        }
    }

    updateAllButtons() {
        const toggleBtnDesktop = document.getElementById('theme-toggle-desktop');
        const toggleBtnMobile = document.getElementById('theme-toggle-mobile');
        
        if (toggleBtnDesktop) this.updateButtonIcon(toggleBtnDesktop);
        if (toggleBtnMobile) this.updateButtonIcon(toggleBtnMobile);
    }

    getStoredTheme() {
        return localStorage.getItem(this.THEME_KEY);
    }

    saveTheme() {
        localStorage.setItem(this.THEME_KEY, this.currentTheme);
    }

    getTheme() {
        return this.currentTheme;
    }
}

// Inicializa o gerenciador de tema
const themeManager = new ThemeManager();

// Exporta para uso global
window.themeManager = themeManager;
