#!/usr/bin/env node

const http = require('http');

const BASE_URL = 'http://localhost:5000';
let sessionCookie = null;
let tempToken = null;

function makeRequest(method, path, data = null, headers = {}) {
    return new Promise((resolve, reject) => {
        const url = new URL(path, BASE_URL);
        const options = {
            method,
            headers: {
                'Content-Type': 'application/json',
                ...headers
            }
        };
        
        if (sessionCookie && !headers['Authorization']) {
            options.headers['Cookie'] = sessionCookie;
        }
        
        const req = http.request(url, options, (res) => {
            let body = '';
            res.on('data', chunk => body += chunk);
            res.on('end', () => {
                try {
                    const parsedBody = body ? JSON.parse(body) : {};
                    
                    if (res.headers['set-cookie']) {
                        sessionCookie = res.headers['set-cookie'][0].split(';')[0];
                    }
                    
                    resolve({
                        status: res.statusCode,
                        headers: res.headers,
                        body: parsedBody
                    });
                } catch (e) {
                    resolve({
                        status: res.statusCode,
                        headers: res.headers,
                        body: body
                    });
                }
            });
        });
        
        req.on('error', reject);
        
        if (data) {
            req.write(JSON.stringify(data));
        }
        
        req.end();
    });
}

async function test(name, fn) {
    try {
        console.log(`\n🧪 Teste: ${name}`);
        await fn();
        console.log(`✅ PASSOU: ${name}`);
        return true;
    } catch (error) {
        console.error(`❌ FALHOU: ${name}`);
        console.error(`   Erro: ${error.message}`);
        return false;
    }
}

function assert(condition, message) {
    if (!condition) {
        throw new Error(message);
    }
}

async function runTests() {
    console.log('='.repeat(60));
    console.log('TESTES DO SISTEMA DE TOKENS TEMPORÁRIOS');
    console.log('='.repeat(60));
    
    let passed = 0;
    let failed = 0;
    
    if (await test('1. Login com credenciais válidas', async () => {
        const res = await makeRequest('POST', '/api/login', {
            username: 'admin',
            password: 'admin'
        });
        assert(res.status === 200, `Status esperado 200, recebido ${res.status}`);
        assert(res.body.success === true, 'Login deve retornar success: true');
        assert(sessionCookie !== null, 'Cookie de sessão deve ser definido');
    })) {
        passed++;
    } else {
        failed++;
    }
    
    if (await test('2. Gerar token temporário', async () => {
        const res = await makeRequest('POST', '/api/dev/generate-temp-token', {
            expiresInHours: 1
        });
        
        assert(res.status === 200, `Status esperado 200, recebido ${res.status}`);
        assert(res.body.success === true, 'Deve retornar success: true');
        assert(res.body.token, 'Deve retornar um token');
        assert(res.body.expiresInHours === 1, 'Validade deve ser 1 hora');
        
        tempToken = res.body.token;
    })) {
        passed++;
    } else {
        failed++;
    }
    
    if (await test('3. Usar token temporário para acessar API', async () => {
        assert(tempToken !== null, 'Token deve estar disponível do teste anterior');
        
        const res = await makeRequest('GET', '/api/session-info', null, {
            'Authorization': `Bearer ${tempToken}`
        });
        
        assert(res.status === 200, `Status esperado 200, recebido ${res.status}`);
        assert(res.body.role === 'dev', `Role deve ser 'dev', recebido '${res.body.role}'`);
        assert(res.body.username === 'temp_dev_access', 'Username deve ser temp_dev_access');
    })) {
        passed++;
    } else {
        failed++;
    }
    
    if (await test('4. Listar tokens ativos', async () => {
        const res = await makeRequest('GET', '/api/dev/temp-tokens');
        
        assert(res.status === 200, `Status esperado 200, recebido ${res.status}`);
        assert(Array.isArray(res.body), 'Deve retornar um array');
        assert(res.body.length > 0, 'Deve haver pelo menos 1 token');
        assert(res.body[0].status === 'ativo', 'Token deve estar ativo');
    })) {
        passed++;
    } else {
        failed++;
    }
    
    if (await test('5. Tentar gerar token com validade inválida', async () => {
        const res = await makeRequest('POST', '/api/dev/generate-temp-token', {
            expiresInHours: 100
        });
        
        assert(res.status === 400, `Status esperado 400, recebido ${res.status}`);
        assert(res.body.error, 'Deve retornar mensagem de erro');
    })) {
        passed++;
    } else {
        failed++;
    }
    
    if (await test('6. Revogar token temporário', async () => {
        const listRes = await makeRequest('GET', '/api/dev/temp-tokens');
        const tokenId = listRes.body[0].id;
        
        const res = await makeRequest('DELETE', '/api/dev/revoke-temp-token', {
            tokenId
        });
        
        assert(res.status === 200, `Status esperado 200, recebido ${res.status}`);
        assert(res.body.success === true, 'Deve retornar success: true');
    })) {
        passed++;
    } else {
        failed++;
    }
    
    if (await test('7. Verificar que token revogado não funciona', async () => {
        const res = await makeRequest('GET', '/api/session-info', null, {
            'Authorization': `Bearer ${tempToken}`
        });
        
        assert(res.status === 401, `Status esperado 401, recebido ${res.status}`);
        assert(res.body.error, 'Deve retornar mensagem de erro');
    })) {
        passed++;
    } else {
        failed++;
    }
    
    if (await test('8. Verificar token com assinatura inválida', async () => {
        const fakeToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.invalidsignature';
        
        const res = await makeRequest('GET', '/api/session-info', null, {
            'Authorization': `Bearer ${fakeToken}`
        });
        
        assert(res.status === 401, `Status esperado 401, recebido ${res.status}`);
    })) {
        passed++;
    } else {
        failed++;
    }
    
    console.log('\n' + '='.repeat(60));
    console.log(`RESULTADO: ${passed} passaram, ${failed} falharam de ${passed + failed} testes`);
    console.log('='.repeat(60));
    
    process.exit(failed > 0 ? 1 : 0);
}

console.log('⏳ Aguardando 2 segundos para garantir que o servidor está pronto...\n');
setTimeout(runTests, 2000);
