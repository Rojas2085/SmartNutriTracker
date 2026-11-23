# Módulo 04 — Registro de hábitos (Equipo 4)

Historias de usuario: **HU-19** (consumo real de alimentos), **HU-20 / HU-21** (agua, sueño y actividad física), **HU-22** (historial y corrección de registros).

Todo el código del equipo vive en `Modules/04_Habitos/`. No se modificó `Program.cs` ni `Core/`.

## Archivos

| Archivo | Qué hace |
| :--- | :--- |
| `AppDbContext.Habitos.cs` | Declara los `DbSet` del módulo (`RegistrosHabito`, `RegistrosComida`). |
| `Models/RegistroHabito.cs` | Hábitos diarios: agua, sueño, actividad física y nota. |
| `Models/RegistroComida.cs` | Comida consumida: tipo, descripción y kcal opcionales. |
| `Services/HabitosLogica.cs` | Reglas de negocio compartidas por la página y la API (validaciones, historial, comidas). |
| `Controllers/HabitosApiController.cs` | API REST `api/habitos/...` con autenticación JWT Bearer. |
| `Index.cshtml` / `Index.cshtml.cs` | Página `/Modules/04_Habitos` con formularios (cookie). |

## Endpoints de la API

Requieren token JWT (`Authorization: Bearer ...`) y rol `Estudiante` o `Admin`. Se obtiene el token con `POST /api/auth/login`.

| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| GET | `/api/habitos/mis-registros?desde=&hasta=` | Historial de hábitos. Por defecto, últimos 30 días. |
| POST | `/api/habitos` | Crea el registro del día. Responde 409 si ya existe esa fecha. |
| PUT | `/api/habitos/{id}` | Corrige un registro propio. |
| POST | `/api/habitos/comidas` | Registra una comida. |
| GET | `/api/habitos/comidas?fecha=` | Comidas de una fecha. |

Cuerpo de `POST/PUT /api/habitos`:

```json
{ "fecha": "2026-11-17", "aguaMl": 2000, "horasSueno": 7.5, "actividadMin": 30, "nota": "opcional" }
```

Cuerpo de `POST /api/habitos/comidas`:

```json
{ "tipoComida": "Almuerzo", "descripcion": "Arroz integral 1 taza y pollo 150 g", "kcal": 520 }
```

## Validaciones

- Agua: 0 a 10 000 ml.
- Horas de sueño: 0 a 24.
- Actividad física: 0 a 1 440 minutos.
- Tipo de comida: `Desayuno`, `Almuerzo`, `Merienda` o `Cena`.
- Kcal de una comida: 0 a 5 000.
- Solo puede existir un registro de hábitos por estudiante y fecha.

## Decisiones de diseño

- **Usuario:** los registros apuntan a `Usuarios.Id` del núcleo. No hay una tabla `Estudiante` aparte.
- **Comidas como texto libre:** el consumo se describe con texto y kcal opcionales. Así no dependemos del catálogo de **Alimentos** del Equipo 3. Si ese catálogo ya existe, se puede cambiar a una lista desplegable.
- **Sin cambios en Program.cs:** la lógica es una clase estática (`HabitosLogica`), así no hay que registrar servicios en el núcleo.
- **Unicidad por fecha:** se valida en código, porque `Core` no permite agregar un índice único desde el módulo.

## Base de datos (SQLite)

El proyecto usa SQLite (`SmartNutriTracker.db`) y crea las tablas con `EnsureCreated`. Si ya tienes un `SmartNutriTracker.db` de antes, **bórralo** antes de ejecutar: `EnsureCreated` no agrega tablas nuevas a una base que ya existe. Al volver a arrancar se crean las tablas del módulo y se siembran otra vez las cuentas.

## Prueba rápida

Cuenta del equipo: `equipo4.est@innovare.edu.bo` / `Innovare2026*` (rol Estudiante).

1. Inicia sesión en `/Account/Login`.
2. Abre **04. Hábitos Diarios (Equipo 4)** en el menú lateral.
3. Guarda los hábitos del día, registra una comida y edita un registro desde el historial.
