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
        const toggleBtn = document.getElementById('theme-toggle');
        const toggleBtnMobile = document.getElementById('theme-toggle-mobile');
        const toggleBtnFloating = document.getElementById('theme-toggle-floating');
        
        if (toggleBtn) {
            this.updateButtonIcon(toggleBtn);
            toggleBtn.addEventListener('click', () => this.toggle());
        }
        
        if (toggleBtnMobile) {
            this.updateButtonIcon(toggleBtnMobile);
            toggleBtnMobile.addEventListener('click', () => this.toggle());
        }
        
        if (toggleBtnFloating) {
            this.updateButtonIcon(toggleBtnFloating);
            toggleBtnFloating.addEventListener('click', () => this.toggle());
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
        const toggleBtn = document.getElementById('theme-toggle');
        const toggleBtnMobile = document.getElementById('theme-toggle-mobile');
        const toggleBtnFloating = document.getElementById('theme-toggle-floating');
        
        if (toggleBtn) this.updateButtonIcon(toggleBtn);
        if (toggleBtnMobile) this.updateButtonIcon(toggleBtnMobile);
        if (toggleBtnFloating) this.updateButtonIcon(toggleBtnFloating);
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
