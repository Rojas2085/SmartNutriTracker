@echo off
chcp 65001 >nul
title SmartNutriTracker - Subir a GitHub
color 0B

set "PROJECT_DIR=%~dp0NutriProy"
if not exist "%PROJECT_DIR%\.git" (
    set "PROJECT_DIR=%~dp0"
)
if not exist "%PROJECT_DIR%\.git" (
    set "PROJECT_DIR=C:\Users\rojoh\Desktop\NutriProy"
)

cd /d "%PROJECT_DIR%"

echo ===============================================================================
echo     🌱 SMARTNUTRITRACKER - SUBIR CAMBIOS A GITHUB
echo ===============================================================================
echo.
echo  Estado actual del repositorio:
git status -s
echo.
echo  Ultimo commit listo para subir:
git log -n 1 --oneline
echo.
echo  Ejecutando git push origin main...
echo ===============================================================================
echo.

git push origin main

if %ERRORLEVEL% equ 0 (
    color 0A
    echo.
    echo ===============================================================================
    echo     ✅ ¡CAMBIOS SUBIDOS EXITOSAMENTE A GITHUB!
    echo ===============================================================================
) else (
    color 0C
    echo.
    echo ===============================================================================
    echo     ❌ Hubo un detalle de autenticacion. Verifica tus credenciales de GitHub.
    echo ===============================================================================
)

echo.
pause
