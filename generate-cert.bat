@echo off
REM Generate self-signed certificate for development
REM This script creates a certificate for local HTTPS testing

setlocal enabledelayedexpansion

set CERT_DIR=.\certs
set CERT_NAME=certificate
set CERT_PASSWORD=%CERTIFICATE_PASSWORD%
if "%CERT_PASSWORD%"=="" set CERT_PASSWORD=pikpro6

echo Creating certificate directory...
if not exist "%CERT_DIR%" mkdir "%CERT_DIR%"

echo.
echo Generating self-signed certificate...
echo.

REM Check if OpenSSL is installed
where openssl >nul 2>&1
if errorlevel 1 (
    echo [ERROR] OpenSSL is not installed or not in PATH
    echo.
    echo Please install OpenSSL:
    echo   - Download from: https://slproweb.com/products/Win32OpenSSL.html
    echo   - Or install via Chocolatey: choco install openssl
    echo   - Or use Git Bash which includes OpenSSL
    echo.
    pause
    exit /b 1
)

REM Generate certificate
openssl req -x509 -newkey rsa:4096 -sha256 -days 365 ^
  -nodes -keyout "%CERT_DIR%\%CERT_NAME%.key" ^
  -out "%CERT_DIR%\%CERT_NAME%.crt" ^
  -subj "/CN=localhost" ^
  -addext "subjectAltName=DNS:localhost,DNS:nnews-api,IP:127.0.0.1"

if errorlevel 1 (
    echo [ERROR] Failed to generate certificate
    pause
    exit /b 1
)

echo.
echo Converting to PFX format...
openssl pkcs12 -export ^
  -out "%CERT_DIR%\%CERT_NAME%.pfx" ^
  -inkey "%CERT_DIR%\%CERT_NAME%.key" ^
  -in "%CERT_DIR%\%CERT_NAME%.crt" ^
  -password pass:%CERT_PASSWORD%

if errorlevel 1 (
    echo [ERROR] Failed to convert certificate
    pause
    exit /b 1
)

echo.
echo ========================================
echo   Certificate Generated Successfully!
echo ========================================
echo.
echo Location: %CERT_DIR%\%CERT_NAME%.pfx
echo Password: %CERT_PASSWORD%
echo.
echo [WARNING] This is a self-signed certificate for development only.
echo           Do not use in production!
echo.
echo To trust this certificate on Windows:
echo   1. Double-click on %CERT_DIR%\%CERT_NAME%.crt
echo   2. Click "Install Certificate"
echo   3. Select "Local Machine"
echo   4. Select "Place all certificates in the following store"
echo   5. Browse and select "Trusted Root Certification Authorities"
echo   6. Click Finish
echo.
pause
