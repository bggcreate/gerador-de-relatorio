@echo off
REM =========================================
REM  Script de Atualização DuckDNS
REM =========================================
REM 
REM IMPORTANTE: Substitua SEUDOMINIO pelo nome que você criou no DuckDNS
REM Exemplo: Se você criou "minhaloja.duckdns.org", coloque apenas "minhaloja"
REM
REM =========================================

REM Substitua SEUDOMINIO abaixo:
curl "https://www.duckdns.org/update?domains=SEUDOMINIO&token=5852a402-7255-4ffc-bc0a-7063d8ececad&ip="

REM Aguarda 5 segundos antes de fechar
timeout /t 5

REM =========================================
REM EXEMPLO DE USO:
REM Se você criou "minhaloja.duckdns.org"
REM A linha deve ficar:
REM curl "https://www.duckdns.org/update?domains=minhaloja&token=5852a402-7255-4ffc-bc0a-7063d8ececad&ip="
REM =========================================
