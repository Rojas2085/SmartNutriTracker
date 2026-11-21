# 🌱 SmartNutriTracker — Colegio "Innovare"

Plataforma Escolar de Seguimiento Nutricional y Hábitos Saludables construida con **ASP.NET Core 8 (C#)** bajo una **Arquitectura Modular Aislada** que integra **Vistas Razor (`.cshtml`)**, **Web API REST**, **Entity Framework Core**, **Autenticación Dual (Cookies + JWT Bearer)** y **Swagger UI**, diseñada específicamente para que **6 equipos de desarrollo (25 a 30 estudiantes)** trabajen en paralelo sin conflictos de fusión (*merge conflicts*) en `Program.cs` ni en `AppDbContext.cs`.

---

## 🚀 1. Instrucciones Rápidas de Arranque

### Opción A (1 Clic con Scripts Automáticos .bat):
- **Instalar y compilar todo:** Doble clic en `instalar-dependencias.bat` (o en el acceso directo del Escritorio).
- **Iniciar proyecto y abrir navegador:** Doble clic en `iniciar-proyecto.bat`.

### Opción B (Línea de Comandos):
Gracias a la configuración por defecto con **SQLite (`SmartNutriTracker.db`)** y soporte para **.NET 10 y .NET 8**, cualquier estudiante puede clonar el repositorio y levantar todo el sistema en segundos sin configurar SQL Server local:

```bash
# 1. Ingresar al directorio del proyecto web
cd SmartNutriTracker.Web

# 2. Compilar la solución
dotnet build

# 3. Iniciar la aplicación (crea SmartNutriTracker.db y siembra las 6 cuentas automáticamente)
dotnet run
```

URLs principales al ejecutar:
- **Login Split-Screen (Razor):** `http://localhost:5000` (o `/Account/Login`)
- **Dashboard General:** `http://localhost:5000/Dashboard`
- **Swagger UI (Pruebas API REST + JWT):** `http://localhost:5000/swagger`

---

## 🔐 2. Credenciales Semilla para los 6 Equipos (`Core/Data/DbInitializer.cs`)

Al iniciar la aplicación por primera vez, se insertan automáticamente estas **6 cuentas de prueba** (contraseña única para todas: **`Innovare2026*`**). En la vista de Login puedes hacer clic en los **6 botones tipo *pill* de Acceso Rápido** para autocompletar instantáneamente cada cuenta:

| Equipo | Carpeta Aislada | Correo Electrónico | Contraseña | Rol (`ClaimTypes.Role`) | Historias de Usuario |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Equipo 1 - Autenticación** | `Modules/01_Autenticacion/` | `equipo1.admin@innovare.edu.bo` | `Innovare2026*` | `Admin` | HU-01 a HU-05 |
| **Equipo 2 - Perfil y Reportes** | `Modules/02_PerfilReportes/` | `equipo2.docente@innovare.edu.bo` | `Innovare2026*` | `Docente` | HU-08 a HU-12 |
| **Equipo 3 - Gestión de Alimentos** | `Modules/03_Alimentos/` | `equipo3.nutri@innovare.edu.bo` | `Innovare2026*` | `Nutricionista` | HU-14 a HU-17 |
| **Equipo 4 - Registro de Hábitos** | `Modules/04_Habitos/` | `equipo4.est@innovare.edu.bo` | `Innovare2026*` | `Estudiante` | HU-19 a HU-22 |
| **Equipo 5 - Recomendaciones** | `Modules/05_Recomendaciones/` | `equipo5.nutri@innovare.edu.bo` | `Innovare2026*` | `Nutricionista` | HU-23 a HU-26 |
| **Equipo 6 - Notificaciones y QA** | `Modules/06_Notificaciones/` | `equipo6.qa@innovare.edu.bo` | `Innovare2026*` | `Admin` | HU-27 a HU-32 |

---

## 🏛️ 3. Regla de Oro de Arquitectura (Cero Conflictos entre Equipos)

Para evitar bloqueos o *merge conflicts* entre los 6 grupos:

1. **Aislamiento Estricto por Carpeta:** Cada equipo crea y edita sus `Models`, `DTOs`, `Services`, `Controllers` y vistas Razor (`.cshtml`) **únicamente dentro de su propia carpeta `Modules/0X_.../`**.
2. **Prohibido tocar el Núcleo (`Core/` y `Program.cs`):** Ningún equipo debe modificar `Program.cs`, `Core/Data/AppDbContext.cs` ni las carpetas de los demás equipos.
3. **Extensión de Base de Datos con `partial class AppDbContext`:** El contexto principal en `Core/Data/AppDbContext.cs` está declarado como `public partial class AppDbContext : DbContext`. Cada grupo dispone de su propio archivo `AppDbContext.[Modulo].cs` dentro de su carpeta para declarar sus tablas (`DbSet<T>`) sin tocar el archivo de los demás:

```csharp
// Ejemplo dentro de Modules/03_Alimentos/AppDbContext.Alimentos.cs
using Microsoft.EntityFrameworkCore;

namespace SmartNutriTracker.Web.Core.Data;

public partial class AppDbContext
{
    public DbSet<Alimento> Alimentos => Set<Alimento>();
    public DbSet<MenuDiario> MenusDiarios => Set<MenuDiario>();
}
```

---

## 🌿 4. Flujo de Trabajo Colaborativo en GitHub (Repositorio Público + 6 Líderes)

Para organizar eficientemente a los **25–30 estudiantes** distribuidos en **6 equipos (4 a 5 integrantes por grupo)**:

1. **Repositorio Principal Público:**
   - El repositorio central es **Público**, permitiendo que todos los estudiantes lo clonen de inmediato (`git clone`) sin esperar invitaciones individuales.
2. **Solo 6 Colaboradores Directos (Los 6 Líderes de Equipo):**
   - Únicamente los **6 líderes de grupo** tienen permisos de escritura directa (*Collaborator*) en el repositorio principal.
   - Cada líder gestiona la rama oficial de su equipo en el repositorio principal:
     - `equipo-1-autenticacion`
     - `equipo-2-perfil-reportes`
     - `equipo-3-alimentos`
     - `equipo-4-habitos`
     - `equipo-5-recomendaciones`
     - `equipo-6-notificaciones`
3. **Trabajo de los Integrantes (Fork o Rama hacia su Líder):**
   - Los integrantes de cada grupo hacen **Fork** del repositorio principal (o trabajan sincronizados con su líder de equipo).
   - Todo el equipo consolida y prueba su avance en la rama de su grupo (`equipo-X-...`) verificando que únicamente se hayan modificado archivos dentro de `Modules/0X_.../`.
4. **Entrega mediante un Único Pull Request por Equipo:**
   - Al finalizar el módulo, el **líder del equipo** abre **un único Pull Request (PR)** desde su rama `equipo-X-...` hacia la rama principal `main`.
   - Como cada grupo modificó exclusivamente su carpeta `Modules/0X_.../`, los 6 Pull Requests se fusionan limpiamente sin conflictos.

---

## 📂 5. Árbol de Archivos del Proyecto

```text
SmartNutriTracker/
├── SmartNutriTracker.sln
├── README.md
├── .gitignore
└── SmartNutriTracker.Web/
    ├── SmartNutriTracker.Web.csproj
    ├── Program.cs                                         # SQLite por defecto + Cookie & JWT Auth + Swagger
    ├── appsettings.json
    ├── _ViewImports.cshtml
    ├── Core/                                              # 🔒 NÚCLEO COMPARTIDO PROTEGIDO
    │   ├── Data/
    │   │   ├── AppDbContext.cs                            # public partial class AppDbContext : DbContext
    │   │   └── DbInitializer.cs                           # Seed automático de las 6 cuentas por equipo
    │   ├── Models/
    │   │   └── Usuario.cs                                 # Entidad base Usuario
    │   └── Security/
    │       └── JwtTokenService.cs                         # Servicio emisor de tokens JWT (HS256)
    ├── Pages/                                             # 🌐 VISTAS RAZOR GLOBALES
    │   ├── _ViewImports.cshtml
    │   ├── Shared/
    │   │   └── _Layout.cshtml                             # Sidebar dinámico por Rol hacia los 6 módulos
    │   ├── Account/
    │   │   ├── Login.cshtml                               # Split-Screen + 6 botones pill de Acceso Rápido
    │   │   ├── Login.cshtml.cs
    │   │   ├── Logout.cshtml
    │   │   └── Logout.cshtml.cs
    │   └── Dashboard/
    │       ├── Index.cshtml                               # Panel general con acceso a los 6 módulos y Swagger
    │       └── Index.cshtml.cs
    └── Modules/                                           # 🧩 6 MÓDULOS AISLADOS POR EQUIPO
        ├── 01_Autenticacion/                              # Equipo 1 (HU-01 a HU-05)
        │   ├── AppDbContext.Autenticacion.cs
        │   ├── Index.cshtml
        │   ├── Index.cshtml.cs
        │   ├── Controllers/
        │   │   └── AuthApiController.cs                   # POST /api/auth/login, GET /api/auth/me, GET /api/auth/auditoria
        │   ├── DTOs/
        │   │   └── LoginRequestDto.cs
        │   └── Models/
        │       └── AuditoriaAcceso.cs
        ├── 02_PerfilReportes/                             # Equipo 2 (HU-08 a HU-12)
        │   ├── AppDbContext.PerfilReportes.cs
        │   ├── Index.cshtml
        │   └── Index.cshtml.cs
        ├── 03_Alimentos/                                  # Equipo 3 (HU-14 a HU-17)
        │   ├── AppDbContext.Alimentos.cs
        │   ├── Index.cshtml
        │   └── Index.cshtml.cs
        ├── 04_Habitos/                                    # Equipo 4 (HU-19 a HU-22)
        │   ├── AppDbContext.Habitos.cs
        │   ├── Index.cshtml
        │   └── Index.cshtml.cs
        ├── 05_Recomendaciones/                            # Equipo 5 (HU-23 a HU-26)
        │   ├── AppDbContext.Recomendaciones.cs
        │   ├── Index.cshtml
        │   └── Index.cshtml.cs
        └── 06_Notificaciones/                             # Equipo 6 (HU-27 a HU-32)
            ├── AppDbContext.Notificaciones.cs
            ├── Index.cshtml
            └── Index.cshtml.cs
```
