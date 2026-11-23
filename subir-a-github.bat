@echo off
chcp 65001 >nul
title SmartNutriTracker - Subir a GitHub (main + equipo-3-alimentos + equipo-4-habitos + ramas)
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
echo  Rama actual:
git branch --show-current
echo.
echo  Ultimo commit listo para subir:
git log -n 1 --oneline
echo.
echo  Subiendo ramas 'main', 'equipo-3-alimentos' y 'equipo-4-habitos' (todas las ramas)...
echo ===============================================================================
echo.

git push origin main equipo-3-alimentos equipo-4-habitos --all

if %ERRORLEVEL% equ 0 (
    color 0A
    echo.
    echo ===============================================================================
    echo     ✅ ¡CAMBIOS SUBIDOS EXITOSAMENTE A GITHUB! (main, equipo-3, equipo-4)
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
