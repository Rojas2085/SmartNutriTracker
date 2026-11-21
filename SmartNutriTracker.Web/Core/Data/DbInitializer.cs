using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Models;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;
using SmartNutriTracker.Web.Modules.PerfilReportes.Models;
using SmartNutriTracker.Web.Modules.Alimentos.Models;
using SmartNutriTracker.Web.Modules.Habitos.Models;
using SmartNutriTracker.Web.Modules.Recomendaciones.Models;
using SmartNutriTracker.Web.Modules.Notificaciones.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Inicializador de base de datos y siembra automática para todos los módulos de SmartNutriTracker.
/// </summary>
public static class DbInitializer
{
    public const string DefaultSeedPassword = "Innovare2026*";
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        try
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Usuarios.AnyAsync())
            {
                logger.LogInformation("[SEED] Base de datos verificada. Las tablas y usuarios ya existen.");
                return;
            }

            logger.LogInformation("[SEED] Inicializando tablas y datos semilla de todos los módulos del Colegio Innovare...");

            var passwordHash = HashPassword(DefaultSeedPassword);

            // ==========================================
            // 1. CUENTAS DE USUARIO (Equipo 1)
            // ==========================================
            var usuarioAdmin = new Usuario
            {
                NombreCompleto = "Ing. Carlos Ramos (Líder Equipo 1)",
                Email = "equipo1.admin@innovare.edu.bo",
                PasswordHash = passwordHash,
                Rol = "Admin",
                EquipoAsignado = "Equipo 1 - Autenticación",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            var usuarioDocente = new Usuario
            {
                NombreCompleto = "Prof. Roberto Mendoza (Líder Equipo 2)",
                Email = "equipo2.docente@innovare.edu.bo",
                PasswordHash = passwordHash,
                Rol = "Docente",
                EquipoAsignado = "Equipo 2 - Perfil y Reportes",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            var usuarioNutriAlimentos = new Usuario
            {
                NombreCompleto = "Lic. Sofía Valverde (Líder Equipo 3)",
                Email = "equipo3.nutri@innovare.edu.bo",
                PasswordHash = passwordHash,
                Rol = "Nutricionista",
                EquipoAsignado = "Equipo 3 - Gestión de Alimentos",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            var usuarioEstudiante = new Usuario
            {
                NombreCompleto = "Mateo Quispe Flores (Líder Equipo 4)",
                Email = "equipo4.est@innovare.edu.bo",
                PasswordHash = passwordHash,
                Rol = "Estudiante",
                EquipoAsignado = "Equipo 4 - Registro de Hábitos",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            var usuarioNutriRecom = new Usuario
            {
                NombreCompleto = "Lic. Mariana Morales (Líder Equipo 5)",
                Email = "equipo5.nutri@innovare.edu.bo",
                PasswordHash = passwordHash,
                Rol = "Nutricionista",
                EquipoAsignado = "Equipo 5 - Recomendaciones",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            var usuarioAdminQa = new Usuario
            {
                NombreCompleto = "Ing. Elena Ríos (Líder Equipo 6)",
                Email = "equipo6.qa@innovare.edu.bo",
                PasswordHash = passwordHash,
                Rol = "Admin",
                EquipoAsignado = "Equipo 6 - Notificaciones y QA",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            await context.Usuarios.AddRangeAsync(
                usuarioAdmin, usuarioDocente, usuarioNutriAlimentos,
                usuarioEstudiante, usuarioNutriRecom, usuarioAdminQa);
            await context.SaveChangesAsync();

            // Auditoría inicial (Equipo 1)
            await context.AuditoriasAcceso.AddAsync(new AuditoriaAcceso
            {
                Email = "sistema@innovare.edu.bo",
                Rol = "Admin",
                EquipoAsignado = "Arquitectura Core",
                Exitoso = true,
                Origen = "DbInitializer Seed",
                Detalle = "Inicialización exitosa de base de datos modular para los 6 equipos.",
                FechaHora = DateTime.Now
            });

            // ==========================================
            // 2. PERFIL DE ESTUDIANTE (Equipo 2)
            // ==========================================
            var perfilMateo = new PerfilEstudiante
            {
                UsuarioId = usuarioEstudiante.Id,
                CodigoEstudiante = "EST-2026-004",
                GradoSeccion = "5to Secundaria A",
                FechaNacimiento = new DateTime(2009, 5, 14),
                Sexo = "M",
                PesoKg = 58.5m,
                TallaCm = 168.0m,
                Imc = 20.73m,
                ClasificacionImc = "Normal",
                ObjetivoNutricional = "Mantener peso y ganar energía",
                FechaRegistro = DateTime.UtcNow
            };
            await context.PerfilesEstudiantes.AddAsync(perfilMateo);
            await context.SaveChangesAsync();

            await context.HistorialesAntropometricos.AddAsync(new HistorialAntropometrico
            {
                PerfilEstudianteId = perfilMateo.Id,
                FechaMedicion = DateTime.UtcNow.AddDays(-30),
                PesoKg = 57.8m,
                TallaCm = 167.5m,
                ImcCalculado = 20.60m,
                ClasificacionImc = "Normal",
                ObservacionesDocente = "Control antropométrico inicial saludable."
            });

            // ==========================================
            // 3. CATÁLOGO INICIAL DE ALIMENTOS (Equipo 3)
            // ==========================================
            var alimentosSemilla = new List<Alimento>
            {
                new() { Nombre = "Manzana Roja", Categoria = "Frutas", PorcionReferencia = "1 unidad mediana (150g)", GramosPorcion = 150m, Calorias = 78m, ProteinasGramos = 0.4m, CarbohidratosGramos = 21m, GrasasGramos = 0.2m, SemaforoNutricional = "Verde", EsAptoQuioscoEscolar = true, Descripcion = "Rica en fibra y antioxidantes." },
                new() { Nombre = "Plátano de Seda", Categoria = "Frutas", PorcionReferencia = "1 unidad (120g)", GramosPorcion = 120m, Calorias = 105m, ProteinasGramos = 1.3m, CarbohidratosGramos = 27m, GrasasGramos = 0.3m, SemaforoNutricional = "Verde", EsAptoQuioscoEscolar = true, Descripcion = "Excelente fuente de potasio y energía." },
                new() { Nombre = "Sándwich de Pollo con Lechuga", Categoria = "Proteínas/Cereales", PorcionReferencia = "1 unidad (180g)", GramosPorcion = 180m, Calorias = 265m, ProteinasGramos = 18.5m, CarbohidratosGramos = 28m, GrasasGramos = 6.2m, SemaforoNutricional = "Verde", EsAptoQuioscoEscolar = true, Descripcion = "Pan integral con pechuga desmenuzada y vegetales frescos." },
                new() { Nombre = "Yogurt Natural con Semillas de Chía", Categoria = "Lácteos", PorcionReferencia = "1 vaso (200ml)", GramosPorcion = 200m, Calorias = 140m, ProteinasGramos = 7.0m, CarbohidratosGramos = 16m, GrasasGramos = 3.5m, SemaforoNutricional = "Amarillo", EsAptoQuioscoEscolar = true, Descripcion = "Aporte proteico y probiótico para el recreo." },
                new() { Nombre = "Gaseosa Cola 350ml", Categoria = "Bebidas Azucaradas", PorcionReferencia = "1 lata (350ml)", GramosPorcion = 350m, Calorias = 150m, ProteinasGramos = 0m, CarbohidratosGramos = 39m, GrasasGramos = 0m, SemaforoNutricional = "Rojo", EsAptoQuioscoEscolar = false, Descripcion = "Bebida alta en azúcares refinados. Restringida en quiosco." }
            };
            await context.Alimentos.AddRangeAsync(alimentosSemilla);
            await context.SaveChangesAsync();

            // ==========================================
            // 4. REGISTRO DE HÁBITOS DE PRUEBA (Equipo 4)
            // ==========================================
            var habitoHoy = new RegistroHabitoDiario
            {
                EstudianteId = usuarioEstudiante.Id,
                Fecha = DateTime.Today,
                VasosAgua = 7,
                MililitrosAgua = 1750m,
                HorasSueno = 8.5m,
                MinutosActividadFisica = 45,
                TipoActividadFisica = "Educación Física y Fútbol Escolar",
                EstadoAnimo = "Con energía",
                Observaciones = "Buen rendimiento y buena hidratación durante el día escolar."
            };
            await context.RegistrosHabitosDiarios.AddAsync(habitoHoy);
            await context.SaveChangesAsync();

            await context.ConsumosAlimentosReales.AddAsync(new ConsumoAlimentoReal
            {
                RegistroHabitoDiarioId = habitoHoy.Id,
                AlimentoId = alimentosSemilla[0].Id,
                DescripcionAlimento = "Manzana Roja de refrigerio escolar",
                TipoComida = "Refrigerio Mañana",
                PorcionesConsumidas = 1.0m,
                CaloriasAproximadas = 78m,
                HoraConsumo = new TimeSpan(10, 30, 0)
            });

            // ==========================================
            // 5. REGLAS NUTRICIONALES BASE (Equipo 5)
            // ==========================================
            var reglasSemilla = new List<ReglaNutricional>
            {
                new()
                {
                    NombreRegla = "Adolescente Escolar - Rango Normal",
                    ClasificacionImc = "Normal",
                    Objetivo = "Mantener peso",
                    RangoEdadMin = 12,
                    RangoEdadMax = 18,
                    CaloriasMin = 1900m,
                    CaloriasMax = 2200m,
                    PorcentajeProteinas = 15.0m,
                    PorcentajeCarbohidratos = 55.0m,
                    PorcentajeGrasas = 30.0m,
                    MetaVasosAgua = 8,
                    MetaMinutosActividad = 60,
                    GuiaNutricional = "Mantener balance entre frutas, vegetales, proteínas magras y mínimo 8 vasos de agua.",
                    Activa = true
                },
                new()
                {
                    NombreRegla = "Adolescente Escolar - Sobrepeso",
                    ClasificacionImc = "Sobrepeso",
                    Objetivo = "Bajar de peso",
                    RangoEdadMin = 12,
                    RangoEdadMax = 18,
                    CaloriasMin = 1600m,
                    CaloriasMax = 1850m,
                    PorcentajeProteinas = 20.0m,
                    PorcentajeCarbohidratos = 50.0m,
                    PorcentajeGrasas = 30.0m,
                    MetaVasosAgua = 9,
                    MetaMinutosActividad = 75,
                    GuiaNutricional = "Reducir azúcares refinados (Semáforo Rojo) y aumentar actividad física aeróbica diaria.",
                    Activa = true
                }
            };
            await context.ReglasNutricionales.AddRangeAsync(reglasSemilla);

            // ==========================================
            // 6. NOTIFICACIONES Y ALERTAS (Equipo 6)
            // ==========================================
            await context.NotificacionesInternas.AddAsync(new NotificacionInterna
            {
                UsuarioReceptorId = usuarioEstudiante.Id,
                Titulo = "¡Bienvenido a SmartNutriTracker!",
                Mensaje = "Recuerda registrar hoy tus comidas, vasos de agua y horas de sueño para ganar hábitos saludables.",
                Tipo = "Recordatorio",
                Leida = false,
                FechaEnvio = DateTime.UtcNow
            });

            await context.AlertasNutricionales.AddAsync(new AlertaNutricional
            {
                EstudianteId = usuarioEstudiante.Id,
                TipoAlerta = "Baja Hidratación",
                NivelSeveridad = "Informativa",
                Mensaje = "El estudiante registró 4 vasos de agua ayer (meta sugerida: 8 vasos).",
                FechaGeneracion = DateTime.UtcNow,
                Atendida = false
            });

            await context.LogsGlobalesSistema.AddAsync(new LogGlobalSistema
            {
                Nivel = "INFO",
                ModuloOrigen = "Core",
                Mensaje = "Base de datos inicializada con éxito para los 6 equipos.",
                UsuarioEmail = "sistema@innovare.edu.bo",
                FechaHora = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
            logger.LogInformation("[SEED] Se estructuraron y sembraron con éxito las tablas de los 6 grupos.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[SEED ERROR] Error al inicializar la base de datos SmartNutriTracker.");
            throw;
        }
    }

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var parts = storedHash.Split('.');
        if (parts.Length != 2)
            return false;

        try
        {
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }
}
