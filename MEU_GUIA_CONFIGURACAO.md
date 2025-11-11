# 🚀 SEU GUIA DE CONFIGURAÇÃO PERSONALIZADO

---

## 📋 SUAS INFORMAÇÕES

- ✅ **Domínio:** sysmonit.duckdns.org
- ✅ **Token:** 5852a402-7255-4ffc-bc0a-7063d8ececad
- ✅ **IP Local:** 192.168.0.11
- ✅ **Gateway (Roteador):** 192.168.0.1
- ✅ **Porta:** 5000

---

## ✅ PASSO 1: Baixar Script do Replit

1. No Replit, encontre o arquivo: **atualizar-duckdns.bat**
2. Clique com botão direito → **Download**
3. Salve em: `C:\DuckDNS\atualizar-duckdns.bat`

*Se a pasta C:\DuckDNS não existir, crie ela primeiro*

---

## ✅ PASSO 2: Testar o Script

1. Vá até `C:\DuckDNS`
2. Clique duas vezes em **atualizar-duckdns.bat**
3. Deve aparecer uma janela preta dizendo:
   ```
   Atualizando IP do DuckDNS...
   OK
   IP atualizado com sucesso!
   ```

Se aparecer `OK`, está funcionando! ✅

---

## ✅ PASSO 3: Configurar para Rodar Automaticamente

### Opção A: Usando Agendador de Tarefas (Recomendado)

1. Pressione `Win + R`
2. Digite: `taskschd.msc` e aperte Enter
3. Na janela que abrir, clique em **"Criar Tarefa Básica"**

**Configuração:**

| Campo | Valor |
|-------|-------|
| **Nome** | DuckDNS Atualização |
| **Descrição** | Atualiza IP do sysmonit.duckdns.org |
| **Quando começar** | Ao fazer logon |

4. Clique **Avançar**
5. Selecione: **Ao fazer logon**
6. Clique **Avançar**
7. Selecione: **Iniciar um programa**
8. Clique **Avançar**
9. **Programa/script:** `C:\DuckDNS\atualizar-duckdns.bat`
10. Clique **Avançar** → **Concluir**

**Agora configure para repetir:**

1. Na lista de tarefas, encontre **"DuckDNS Atualização"**
2. Clique com botão direito → **Propriedades**
3. Vá na aba **Gatilhos**
4. Clique duas vezes no gatilho existente
5. Marque: ✅ **Repetir a tarefa a cada:** `5 minutos`
6. **Por:** `Indefinidamente`
7. OK → OK

✅ Pronto! Agora seu IP atualiza automaticamente a cada 5 minutos!

---

## ✅ PASSO 4: Configurar Roteador (Port Forwarding)

**ACESSE SEU ROTEADOR:**

1. Abra navegador
2. Digite: `http://192.168.0.1`
3. Login (geralmente):
   - Usuário: `admin`
   - Senha: `admin` ou veja etiqueta do roteador

**PROCURAR "PORT FORWARDING":**

Pode estar em:
- Port Forwarding
- Encaminhamento de Portas
- Virtual Server
- NAT
- Aplicações e Jogos

**CRIAR NOVA REGRA:**

Preencha EXATAMENTE assim:

```
Nome/Descrição: Sistema Relatorios
Porta Externa: 5000
IP Interno/Servidor: 192.168.0.11
Porta Interna: 5000
Protocolo: TCP (ou TCP/UDP)
Status/Ativar: Habilitado/Ligado
```

**SALVAR E REINICIAR ROTEADOR**

⚠️ **SE PORTA 5000 NÃO FUNCIONAR:**

Alguns provedores bloqueiam portas comuns. Tente:
- **8080** (HTTP alternativo)
- **3000** (Node.js padrão)
- **7777** (raramente bloqueada)

Se usar porta diferente, você vai acessar: `http://sysmonit.duckdns.org:8080`

---

## ✅ PASSO 5: Fixar IP Local (Importante!)

Para garantir que o IP 192.168.0.11 não mude:

1. **Painel de Controle** → **Rede e Internet**
2. **Central de Rede e Compartilhamento**
3. Clique na sua conexão (Wi-Fi ou Ethernet)
4. **Propriedades**
5. Selecione **Protocolo IP Versão 4 (TCP/IPv4)**
6. **Propriedades**
7. Marque: **Usar o seguinte endereço IP:**

Preencha:
```
Endereço IP: 192.168.0.11
Máscara de sub-rede: 255.255.255.0
Gateway padrão: 192.168.0.1
Servidor DNS preferencial: 8.8.8.8
Servidor DNS alternativo: 8.8.4.4
```

8. OK → OK

---

## ✅ PASSO 6: TESTAR TUDO!

### Teste 1: Sistema Local

Abra navegador:
```
http://localhost:5000
```
✅ Deve aparecer a tela de login

### Teste 2: Rede Local

Abra navegador (ou no celular conectado no mesmo Wi-Fi):
```
http://192.168.0.11:5000
```
✅ Deve aparecer a tela de login

### Teste 3: Verificar se DuckDNS está apontando certo

No CMD do Windows:
```cmd
ping sysmonit.duckdns.org
```

O IP retornado deve ser o **SEU IP PÚBLICO** (não o 192.168.0.11)

Para saber seu IP público: https://www.meuip.com.br

### Teste 4: Verificar se porta está aberta

Acesse: https://www.yougetsignal.com/tools/open-ports/

Preencha:
- **Remote Address:** `sysmonit.duckdns.org`
- **Port Number:** `5000`
- Clique em **Check**

✅ Deve mostrar: **"Port 5000 is open on sysmonit.duckdns.org"**

❌ Se mostrar **closed**, reveja o Port Forwarding no roteador

### Teste 5: ACESSO FINAL! 🎉

**MUITO IMPORTANTE:** Teste de FORA da sua rede (dados móveis)

1. Pegue seu celular
2. **DESLIGUE O WI-FI** (use 4G/5G)
3. Abra navegador
4. Acesse:
```
http://sysmonit.duckdns.org:5000
```

✅ Deve aparecer a tela de login do sistema!

---

## 🎯 RESUMO - Tudo Configurado

```
Internet
   ↓
sysmonit.duckdns.org:5000
   ↓
Seu IP Público (atualizado a cada 5min pelo script)
   ↓
Roteador 192.168.0.1 (redireciona porta 5000)
   ↓
Seu PC 192.168.0.11:5000
   ↓
Sistema Rodando!
```

---

## 🔐 SEGURANÇA (FAÇA AGORA!)

1. **Trocar senha padrão:**
   - Acesse o sistema
   - Login: admin/admin
   - Vá em configurações
   - **Troque para senha FORTE!**
   - Exemplo: `Rel@2024$Seg!`

2. **Firewall do Windows:**
   - Mantenha ativo
   - Deve estar permitindo porta 5000

3. **Não compartilhe:**
   - ❌ Não divulgue o token DuckDNS
   - ❌ Não compartilhe a senha do sistema
   - ✅ Compartilhe apenas o link: sysmonit.duckdns.org:5000

---

## 📱 ACESSAR DE QUALQUER LUGAR

### No computador:
```
http://sysmonit.duckdns.org:5000
```

### No celular/tablet:
```
http://sysmonit.duckdns.org:5000
```

### Criar atalho no celular:

1. Abra o site no Chrome/Safari
2. Menu (3 pontos) → **Adicionar à tela inicial**
3. ✅ Agora tem um ícone como app!

---

## ❓ PROBLEMAS COMUNS

### "Não consigo acessar de fora"

**Checklist:**
- [ ] Script DuckDNS está rodando? (Agendador de Tarefas)
- [ ] Port Forwarding configurado? (porta 5000 → 192.168.0.11)
- [ ] Porta testada e aberta? (yougetsignal.com)
- [ ] Sistema está rodando? (localhost:5000 funciona?)
- [ ] Testou de OUTRA rede? (4G do celular, não Wi-Fi)

### "Porta aparece fechada"

**Soluções:**
1. Verifique Port Forwarding no roteador (porta 5000 → 192.168.0.11)
2. Tente porta alternativa (8080, 3000, 7777)
3. Desative firewall temporariamente para testar
4. Alguns provedores bloqueiam portas - ligue e pergunte

### "DuckDNS não atualiza IP"

**Teste:**
```cmd
curl "https://www.duckdns.org/update?domains=sysmonit&token=5852a402-7255-4ffc-bc0a-7063d8ececad&verbose=true"
```

Deve retornar:
```
OK
SEU_IP_PUBLICO
CHANGED (ou NO CHANGE)
```

---

## 📞 LINKS ÚTEIS

- **DuckDNS:** https://www.duckdns.org
- **Testar Porta:** https://www.yougetsignal.com/tools/open-ports/
- **Meu IP Público:** https://www.meuip.com.br
- **Seu Sistema:** http://sysmonit.duckdns.org:5000

---

## ✅ CHECKLIST FINAL

- [ ] Script baixado e testado (aparece "OK")
- [ ] Script agendado (Agendador de Tarefas a cada 5min)
- [ ] IP local fixado (192.168.0.11)
- [ ] Port Forwarding configurado no roteador
- [ ] Porta 5000 testada e aberta
- [ ] Sistema acessível localmente (localhost:5000)
- [ ] Sistema acessível pela rede (192.168.0.11:5000)
- [ ] Sistema acessível pela internet (sysmonit.duckdns.org:5000)
- [ ] Senha admin alterada para senha forte
- [ ] Testado em dados móveis (4G)

---

✅ **PRONTO! Seu sistema está configurado e acessível de qualquer lugar!** 🎉

**Seu link público:** http://sysmonit.duckdns.org:5000

Dados salvos localmente em: `data/database.db` no seu PC
