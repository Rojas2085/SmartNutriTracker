@echo off
chcp 65001 >nul
title SmartNutriTracker - Colegio Innovare (.NET 8 / 10)
color 02

set "PROJECT_DIR=%~dp0NutriProy"
if not exist "%PROJECT_DIR%\SmartNutriTracker.sln" (
    set "PROJECT_DIR=%~dp0"
)
if not exist "%PROJECT_DIR%\SmartNutriTracker.sln" (
    set "PROJECT_DIR=C:\Users\rojoh\Desktop\NutriProy"
)

if not exist "%PROJECT_DIR%\SmartNutriTracker.Web\SmartNutriTracker.Web.csproj" (
    echo [ERROR] No se encontro la carpeta del proyecto SmartNutriTracker.Web.
    pause
    exit /b 1
)

cd /d "%PROJECT_DIR%\SmartNutriTracker.Web"

echo ===============================================================================
echo     🌱 SMARTNUTRITRACKER - COLEGIO "INNOVARE"
echo     Iniciando servidor web (.NET 10 / .NET 8)...
echo ===============================================================================
echo.
echo  Abriendo:
echo   - Web App (Login): http://localhost:5000
echo   - Swagger API:    http://localhost:5000/swagger
echo.
echo  Cuentas semilla disponibles (Contrasena: Innovare2026*):
echo   - Equipo 1 (Admin):         equipo1.admin@innovare.edu.bo
echo   - Equipo 2 (Docente):       equipo2.docente@innovare.edu.bo
echo   - Equipo 3 (Nutricionista): equipo3.nutri@innovare.edu.bo
echo   - Equipo 4 (Estudiante):    equipo4.est@innovare.edu.bo
echo   - Equipo 5 (Nutricionista): equipo5.nutri@innovare.edu.bo
echo   - Equipo 6 (Admin QA):      equipo6.qa@innovare.edu.bo
echo ===============================================================================
echo.

start "" http://localhost:5000
start "" http://localhost:5000/swagger

dotnet run
