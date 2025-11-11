@echo off
REM =========================================
REM  Script de Atualização DuckDNS
REM  Dominio: sysmonit.duckdns.org
REM =========================================

echo Atualizando IP do DuckDNS...
curl "https://www.duckdns.org/update?domains=sysmonit&token=5852a402-7255-4ffc-bc0a-7063d8ececad&ip="
echo.
echo.
echo IP atualizado com sucesso!
timeout /t 3
