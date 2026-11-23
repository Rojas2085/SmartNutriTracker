# Entrega: Módulo 04 · Registro de hábitos (Equipo 4)

**Proyecto:** SmartNutriTracker, Colegio Innovare
**Historias de usuario:** HU-19, HU-20, HU-21 y HU-22
**Rama sugerida:** `equipo-4-habitos`

Esta carpeta contiene todo lo que hace falta para integrar el Módulo 04 al repositorio del equipo. Solo se agregan archivos dentro de `SmartNutriTracker.Web/Modules/04_Habitos/`. No se modifica `Program.cs`, `Core/` ni ningún otro módulo.

## Contenido de la entrega

| Ruta | Qué es |
|---|---|
| `SmartNutriTracker.Web/Modules/04_Habitos/` | Código completo del módulo (9 archivos, incluido su README). |
| `equipo4-habitos.patch` | Parche con los dos commits del módulo, para aplicar con `git am`. |
| `Modulo04_Habitos_Equipo4.docx` | Documentación completa del módulo: funcionamiento, modelo de datos, API, seguridad, pruebas y anexo con el código. |
| `LEEME.md` | Este archivo. |

### Archivos del módulo

| Archivo | Responsabilidad |
|---|---|
| `AppDbContext.Habitos.cs` | Agrega las tablas `RegistrosHabito` y `RegistrosComida` al contexto (clase parcial). |
| `Models/RegistroHabito.cs` | Entidad de los hábitos diarios: agua, sueño, actividad y nota. |
| `Models/RegistroComida.cs` | Entidad de las comidas: tipo, descripción y kcal opcionales. |
| `Services/HabitosLogica.cs` | Validaciones y reglas de negocio compartidas por la web y la API. |
| `Controllers/HabitosApiController.cs` | API REST en `/api/habitos`, protegida con JWT. |
| `Index.cshtml.cs` | Modelo de la página web: carga datos y procesa los formularios. |
| `Index.cshtml` | Vista de la página en `/Modules/04_Habitos`. |
| `README.md` | Guía técnica breve del módulo (endpoints, validaciones, decisiones). |

## Qué hace el módulo

- **HU-19:** el estudiante registra lo que comió (tipo de comida, descripción y kcal opcionales) y ve sus comidas del día.
- **HU-20:** registra el consumo de agua del día (0 a 10 000 ml).
- **HU-21:** registra horas de sueño (0 a 24) y minutos de actividad física (0 a 1 440).
- **HU-22:** consulta el historial de los últimos 30 días y edita sus propios registros. Solo puede ver y editar lo que le pertenece.

Solo se puede guardar un registro de hábitos por estudiante y por día. Si se intenta repetir una fecha, el sistema muestra un error.

## Cómo integrarlo (administrador)

**Opción A: con el parche (conserva los commits del equipo)**

```
git checkout -b equipo-4-habitos
git am equipo4-habitos.patch
git push -u origin equipo-4-habitos
```

**Opción B: copiando la carpeta**

1. Copia `SmartNutriTracker.Web/Modules/04_Habitos/` dentro del repositorio, en la misma ruta.
2. Haz el commit:
   ```
   git add SmartNutriTracker.Web/Modules/04_Habitos
   git commit -m "feat(equipo-4): módulo Registro de Hábitos (HU-19 a HU-22)"
   git push -u origin equipo-4-habitos
   ```

Después abre un Pull Request de `equipo-4-habitos` hacia `main`.

## Cómo probarlo

1. Borra `SmartNutriTracker.db` dentro de `SmartNutriTracker.Web` si existe. Si no se borra, las tablas nuevas no se crean.
2. En la carpeta `SmartNutriTracker.Web` ejecuta:
   ```
   dotnet build
   dotnet run
   ```
3. Entra a la URL que muestre la consola (normalmente `http://localhost:5000`) con la cuenta de prueba:
   - Correo: `equipo4.est@innovare.edu.bo`
   - Contraseña: `Innovare2026*`
4. Abre `/Modules/04_Habitos` desde el dashboard o directamente.

La lista completa de pruebas (web, validaciones, aislamiento entre usuarios y API en Swagger) está en la sección 11 del documento `Modulo04_Habitos_Equipo4.docx`.

## Estado de verificación

- El código **no se ha compilado ni ejecutado en el entorno donde se preparó esta entrega**, porque no había SDK de .NET disponible. Hay que correr `dotnet build` antes de aprobar el Pull Request.
- Antes de entregar el proyecto final conviene cambiar la contraseña de las cuentas de prueba y la clave JWT.

## Limitaciones conocidas

- Las comidas son texto libre con kcal opcionales; no se conectan con el catálogo de alimentos del Equipo 3.
- La regla de una fecha por estudiante se valida en código, no con un índice único en la base de datos.
- El proyecto usa `EnsureCreated`, así que una base de datos ya creada no recibe las tablas nuevas: hay que borrar el `.db`.
- No hay pruebas automatizadas; las pruebas son manuales.
