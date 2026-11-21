@echo off
chcp 65001 >nul
title SmartNutriTracker - Subir a GitHub (main + equipo-3-alimentos + ramas)
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
echo  Ultimo commit listo para subir:
git log -n 1 --oneline
echo.
echo  Subiendo rama 'main' y rama 'equipo-3-alimentos' (junto con las demas ramas)...
echo ===============================================================================
echo.

git push origin main equipo-3-alimentos --all

if %ERRORLEVEL% equ 0 (
    color 0A
    echo.
    echo ===============================================================================
    echo     ✅ ¡CAMBIOS SUBIDOS EXITOSAMENTE A MAIN Y EQUIPO-3-ALIMENTOS EN GITHUB!
    echo ===============================================================================
) else (
    color 0C
    echo.
    echo ===============================================================================
    echo     ❌ Hubo un detalle al subir. Revisa el mensaje superior.
    echo ===============================================================================
)

echo.
pause
