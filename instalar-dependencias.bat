@echo off
chcp 65001 >nul
title SmartNutriTracker - Instalador de Dependencias (.NET 10 / .NET 8)
color 0A

echo ===============================================================================
echo     🌱 SMARTNUTRITRACKER - COLEGIO "INNOVARE"
echo     Instalador Automatico de Dependencias (.NET 10.0 y .NET 8.0)
echo ===============================================================================
echo.

:: 1. Verificar si .NET CLI esta instalado
where dotnet >nul 2>&1
if %ERRORLEVEL% neq 0 (
    color 0C
    echo [ERROR CRITICO] No se encontro .NET instalado en este equipo.
    echo.
    echo Por favor descarga e instala .NET SDK (version 8 o 10) desde:
    echo https://dotnet.microsoft.com/download/dotnet
    echo.
    pause
    start https://dotnet.microsoft.com/download/dotnet
    exit /b 1
)

:: 2. Detectar versiones instaladas
for /f "tokens=*" %%v in ('dotnet --version 2^>nul') do set DOTNET_VER=%%v
echo [1/5] Verificando entorno .NET...
echo       - .NET SDK detectado: %DOTNET_VER%
echo.

echo [Runtimes instalados en el sistema]:
dotnet --list-runtimes
echo.

:: 3. Ubicar el directorio del proyecto
set "PROJECT_DIR=%~dp0NutriProy"
if not exist "%PROJECT_DIR%\SmartNutriTracker.sln" (
    set "PROJECT_DIR=%~dp0"
)
if not exist "%PROJECT_DIR%\SmartNutriTracker.sln" (
    set "PROJECT_DIR=C:\Users\rojoh\Desktop\NutriProy"
)

if not exist "%PROJECT_DIR%\SmartNutriTracker.sln" (
    color 0C
    echo [ERROR] No se encontro la solucion SmartNutriTracker.sln.
    echo Asegurate de que la carpeta 'NutriProy' este en el Escritorio.
    echo.
    pause
    exit /b 1
)

echo [2/5] Carpeta del proyecto localizada:
echo       %PROJECT_DIR%
echo.

cd /d "%PROJECT_DIR%"

:: 4. Restaurar paquetes NuGet
echo [3/5] Restaurando dependencias NuGet para .NET 10 / .NET 8...
echo       - Microsoft.AspNetCore.Authentication.JwtBearer
echo       - Microsoft.EntityFrameworkCore.Sqlite
echo       - Microsoft.EntityFrameworkCore.SqlServer
echo       - Swashbuckle.AspNetCore (Swagger UI)
echo.
dotnet restore SmartNutriTracker.sln
if %ERRORLEVEL% neq 0 (
    color 0E
    echo.
    echo [AVISO] Fallo restauracion online. Intentando restaurar desde cache local...
    dotnet restore SmartNutriTracker.sln --source "%USERPROFILE%\.nuget\packages"
)

:: 5. Compilar la solucion completa
echo.
echo [4/5] Compilando la solucion modular SmartNutriTracker...
dotnet build SmartNutriTracker.sln --no-restore
if %ERRORLEVEL% neq 0 (
    color 0C
    echo.
    echo [ERROR] Hubo un error al compilar el proyecto.
    pause
    exit /b 1
)

echo.
echo [5/5] Compilacion completada con exito (0 Errores).
echo.
echo ===============================================================================
echo     ✅ INSTALACION Y CONFIGURACION COMPLETADA EXITOSAMENTE
echo ===============================================================================
echo.
echo  Detalles del entorno:
echo   - Soporte: .NET 10.0 y .NET 8.0 (con RollForward automatico)
echo   - Base de Datos: SQLite local (SmartNutriTracker.db en SmartNutriTracker.Web/)
echo   - Cuentas Semilla: 6 equipos listos con contrasena 'Innovare2026*'
echo.
echo  Presiona cualquier tecla para INICIAR el servidor y abrir el navegador...
echo ===============================================================================
pause >nul

cd /d "%PROJECT_DIR%\SmartNutriTracker.Web"

echo.
echo Levantando SmartNutriTracker en http://localhost:5000...
echo (Presiona Ctrl + C en esta ventana cuando quieras detener el servidor)
echo.

start "" http://localhost:5000
start "" http://localhost:5000/swagger

dotnet run
