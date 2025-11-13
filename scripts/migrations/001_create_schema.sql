-- =================================================================
-- MIGRAÇÃO POSTGRESQL - Schema Completo do Sistema
-- =================================================================

-- Tabela de usuários
CREATE TABLE IF NOT EXISTS usuarios (
    id SERIAL PRIMARY KEY,
    username TEXT UNIQUE NOT NULL,
    password TEXT NOT NULL,
    role TEXT NOT NULL,
    loja_gerente TEXT,
    lojas_consultor TEXT,
    loja_tecnico TEXT,
    password_hashed INTEGER DEFAULT 0
);

-- Tabela de lojas
CREATE TABLE IF NOT EXISTS lojas (
    id SERIAL PRIMARY KEY,
    nome TEXT UNIQUE NOT NULL,
    status TEXT,
    funcao_especial TEXT,
    observacoes TEXT,
    tecnico_username TEXT,
    cargo TEXT,
    cep TEXT,
    numero_contato TEXT,
    gerente TEXT
);

-- Tabela de relatórios
CREATE TABLE IF NOT EXISTS relatorios (
    id SERIAL PRIMARY KEY,
    loja TEXT,
    data TEXT,
    hora_abertura TEXT,
    hora_fechamento TEXT,
    gerente_entrada TEXT,
    gerente_saida TEXT,
    clientes_monitoramento INTEGER,
    vendas_monitoramento INTEGER,
    clientes_loja INTEGER,
    vendas_loja INTEGER,
    total_vendas_dinheiro REAL,
    ticket_medio TEXT,
    pa TEXT,
    quantidade_trocas INTEGER,
    nome_funcao_especial TEXT,
    quantidade_funcao_especial INTEGER,
    quantidade_omni INTEGER,
    vendedores TEXT,
    nome_arquivo TEXT,
    enviado_por_usuario TEXT,
    enviado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    vendas_cartao INTEGER,
    vendas_pix INTEGER,
    vendas_dinheiro INTEGER,
    source_instance UUID
);

-- Tabela de demandas
CREATE TABLE IF NOT EXISTS demandas (
    id SERIAL PRIMARY KEY,
    loja_nome TEXT NOT NULL,
    descricao TEXT NOT NULL,
    tag TEXT DEFAULT 'Normal',
    status TEXT DEFAULT 'pendente',
    criado_por_usuario TEXT,
    concluido_por_usuario TEXT,
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    concluido_em TIMESTAMP,
    source_instance UUID
);

-- Tabela de vendedores
CREATE TABLE IF NOT EXISTS vendedores (
    id SERIAL PRIMARY KEY,
    loja_id INTEGER NOT NULL,
    nome TEXT NOT NULL,
    telefone TEXT NOT NULL,
    data_entrada TEXT NOT NULL,
    data_demissao TEXT,
    previsao_entrada TEXT,
    previsao_saida TEXT,
    ativo INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    source_instance UUID,
    FOREIGN KEY (loja_id) REFERENCES lojas(id) ON DELETE CASCADE
);

-- Tabela de logs
CREATE TABLE IF NOT EXISTS logs (
    id SERIAL PRIMARY KEY,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    type TEXT NOT NULL,
    username TEXT,
    action TEXT,
    details TEXT,
    source_instance UUID
);

-- Tabela de tokens temporários
CREATE TABLE IF NOT EXISTS temp_tokens (
    id SERIAL PRIMARY KEY,
    token_hash TEXT UNIQUE NOT NULL,
    role TEXT DEFAULT 'dev',
    expira_em TIMESTAMP NOT NULL,
    ip_origem TEXT,
    ip_restrito TEXT,
    revogado INTEGER DEFAULT 0,
    criado_por TEXT,
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    usado_em TIMESTAMP,
    revogado_em TIMESTAMP,
    revogado_por TEXT
);

-- Tabela de estoque técnico
CREATE TABLE IF NOT EXISTS estoque_tecnico (
    id SERIAL PRIMARY KEY,
    nome_peca TEXT NOT NULL,
    codigo_interno TEXT UNIQUE NOT NULL,
    quantidade INTEGER DEFAULT 0,
    valor_custo REAL DEFAULT 0,
    loja TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    source_instance UUID
);

-- Tabela de assistências
CREATE TABLE IF NOT EXISTS assistencias (
    id SERIAL PRIMARY KEY,
    cliente_nome TEXT NOT NULL,
    cliente_cpf TEXT NOT NULL,
    numero_pedido TEXT,
    data_entrada TEXT NOT NULL,
    data_conclusao TEXT,
    valor_peca_loja REAL DEFAULT 0,
    valor_servico_cliente REAL DEFAULT 0,
    aparelho TEXT NOT NULL,
    peca_id INTEGER,
    peca_nome TEXT,
    observacoes TEXT,
    status TEXT DEFAULT 'Em andamento',
    tecnico_responsavel TEXT,
    loja TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    source_instance UUID,
    FOREIGN KEY (peca_id) REFERENCES estoque_tecnico(id)
);

-- Tabela de PDFs de tickets
CREATE TABLE IF NOT EXISTS pdf_tickets (
    id SERIAL PRIMARY KEY,
    loja TEXT NOT NULL,
    data TEXT NOT NULL,
    filename TEXT NOT NULL,
    filepath TEXT NOT NULL,
    uploaded_by TEXT NOT NULL,
    uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    source_instance UUID
);

-- Tabela de PDFs de rankings
CREATE TABLE IF NOT EXISTS pdf_rankings (
    id SERIAL PRIMARY KEY,
    loja TEXT NOT NULL,
    data TEXT NOT NULL,
    filename TEXT NOT NULL,
    filepath TEXT NOT NULL,
    pa TEXT,
    preco_medio TEXT,
    atendimento_medio TEXT,
    uploaded_by TEXT,
    uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    source_instance UUID
);

-- Tabela de backups (estendida com novos campos)
CREATE TABLE IF NOT EXISTS db_backups (
    id SERIAL PRIMARY KEY,
    filename TEXT NOT NULL,
    filepath TEXT,
    size_bytes BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    sent_to_email INTEGER DEFAULT 0,
    email_sent_at TIMESTAMP,
    backup_type TEXT DEFAULT 'manual',
    source_instance UUID,
    notes TEXT,
    drive_file_id TEXT,
    created_by TEXT,
    status TEXT DEFAULT 'success',
    error_message TEXT
);

-- Tabela de configurações de instância (para identificação única)
CREATE TABLE IF NOT EXISTS instance_config (
    id SERIAL PRIMARY KEY,
    instance_uuid UUID UNIQUE NOT NULL,
    instance_name TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de configurações do sistema
CREATE TABLE IF NOT EXISTS system_settings (
    id SERIAL PRIMARY KEY,
    setting_key TEXT UNIQUE NOT NULL,
    setting_value TEXT,
    updated_by TEXT,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Criar índices para performance
CREATE INDEX IF NOT EXISTS idx_relatorios_loja ON relatorios(loja);
CREATE INDEX IF NOT EXISTS idx_relatorios_data ON relatorios(data);
CREATE INDEX IF NOT EXISTS idx_relatorios_source ON relatorios(source_instance);
CREATE INDEX IF NOT EXISTS idx_vendedores_loja ON vendedores(loja_id);
CREATE INDEX IF NOT EXISTS idx_logs_timestamp ON logs(timestamp);
CREATE INDEX IF NOT EXISTS idx_logs_type ON logs(type);
CREATE INDEX IF NOT EXISTS idx_demandas_status ON demandas(status);
CREATE INDEX IF NOT EXISTS idx_assistencias_status ON assistencias(status);
CREATE INDEX IF NOT EXISTS idx_backups_created ON db_backups(created_at);
CREATE INDEX IF NOT EXISTS idx_backups_type ON db_backups(backup_type);
CREATE INDEX IF NOT EXISTS idx_backups_status ON db_backups(status);

-- Mensagem de sucesso
DO $$
BEGIN
    RAISE NOTICE 'Schema PostgreSQL criado com sucesso!';
END $$;
