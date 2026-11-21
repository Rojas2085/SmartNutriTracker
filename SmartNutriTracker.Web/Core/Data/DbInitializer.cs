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
                new() { Nombre = "Pechuga de Pollo", Categoria = "Carnes", Icono = "🥩", PorcionReferencia = "100 g", GramosPorcion = 100m, Calorias = 165m, ProteinasGramos = 31m, CarbohidratosGramos = 0m, GrasasGramos = 3.6m, SemaforoNutricional = "Verde", EsFavorito = true, EsAptoQuioscoEscolar = true, Descripcion = "Proteína magra de alto valor biológico." },
                new() { Nombre = "Arroz Integral", Categoria = "Cereales", Icono = "🌾", PorcionReferencia = "100 g", GramosPorcion = 100m, Calorias = 216m, ProteinasGramos = 5m, CarbohidratosGramos = 45m, GrasasGramos = 1.8m, SemaforoNutricional = "Verde", EsFavorito = true, EsAptoQuioscoEscolar = true, Descripcion = "Carbohidrato complejo rico en fibra." },
                new() { Nombre = "Manzana", Categoria = "Frutas", Icono = "🍎", PorcionReferencia = "1 unidad (100g)", GramosPorcion = 100m, Calorias = 52m, ProteinasGramos = 0.3m, CarbohidratosGramos = 14m, GrasasGramos = 0.2m, SemaforoNutricional = "Verde", EsFavorito = true, EsAptoQuioscoEscolar = true, Descripcion = "Fruta fresca rica en pectina y antioxidantes." },
                new() { Nombre = "Leche Entera", Categoria = "Lácteos", Icono = "🥛", PorcionReferencia = "1 vaso (200ml)", GramosPorcion = 200m, Calorias = 124m, ProteinasGramos = 6.4m, CarbohidratosGramos = 9.6m, GrasasGramos = 7.0m, SemaforoNutricional = "Amarillo", EsFavorito = false, EsAptoQuioscoEscolar = true, Descripcion = "Fuente de calcio y vitamina D." },
                new() { Nombre = "Brócoli al Vapor", Categoria = "Verduras", Icono = "🥦", PorcionReferencia = "1 taza (100g)", GramosPorcion = 100m, Calorias = 35m, ProteinasGramos = 2.8m, CarbohidratosGramos = 7m, GrasasGramos = 0.4m, SemaforoNutricional = "Verde", EsFavorito = false, EsAptoQuioscoEscolar = true, Descripcion = "Verdura crucífera rica en vitamina C y fibra." },
                new() { Nombre = "Salmón a la Plancha", Categoria = "Carnes", Icono = "🐟", PorcionReferencia = "100 g", GramosPorcion = 100m, Calorias = 206m, ProteinasGramos = 22m, CarbohidratosGramos = 0m, GrasasGramos = 12m, SemaforoNutricional = "Verde", EsFavorito = false, EsAptoQuioscoEscolar = true, Descripcion = "Alto en ácidos grasos Omega-3." },
                new() { Nombre = "Quinoa Cocida", Categoria = "Cereales", Icono = "🥣", PorcionReferencia = "100 g", GramosPorcion = 100m, Calorias = 120m, ProteinasGramos = 4.4m, CarbohidratosGramos = 21.3m, GrasasGramos = 1.9m, SemaforoNutricional = "Verde", EsFavorito = false, EsAptoQuioscoEscolar = true, Descripcion = "Grano andino con aminoácidos esenciales." },
                new() { Nombre = "Yogurt Natural", Categoria = "Lácteos", Icono = "🥣", PorcionReferencia = "150 g", GramosPorcion = 150m, Calorias = 92m, ProteinasGramos = 5.2m, CarbohidratosGramos = 7.0m, GrasasGramos = 5.0m, SemaforoNutricional = "Verde", EsFavorito = true, EsAptoQuioscoEscolar = true, Descripcion = "Aporte de probióticos y proteínas." },
                new() { Nombre = "Almendras", Categoria = "Saludables", Icono = "🥜", PorcionReferencia = "30 g", GramosPorcion = 30m, Calorias = 174m, ProteinasGramos = 6.3m, CarbohidratosGramos = 6.6m, GrasasGramos = 15m, SemaforoNutricional = "Verde", EsFavorito = false, EsAptoQuioscoEscolar = true, Descripcion = "Grasas saludables y vitamina E." },
                new() { Nombre = "Espinacas Frescas", Categoria = "Verduras", Icono = "🥬", PorcionReferencia = "100 g", GramosPorcion = 100m, Calorias = 23m, ProteinasGramos = 2.9m, CarbohidratosGramos = 3.6m, GrasasGramos = 0.4m, SemaforoNutricional = "Verde", EsFavorito = false, EsAptoQuioscoEscolar = true, Descripcion = "Hojas verdes ricas en hierro y folatos." }
            };
            await context.Alimentos.AddRangeAsync(alimentosSemilla);
            await context.SaveChangesAsync();

            // MENÚS DIARIOS DE EJEMPLO (MOCKUP GRUPO 3)
            var menu1 = new MenuDiario
            {
                NombreMenu = "Desayuno Energético",
                FechaMenu = new DateTime(2024, 1, 15),
                TipoComida = "Desayuno",
                EstadoVariante = "Estándar",
                TotalCalorias = 432m,
                TotalProteinas = 31m,
                TotalCarbohidratos = 45m,
                TotalGrasas = 9m,
                Semaforo = "Verde",
                EsSaludable = true,
                DetalleAlimentosResumen = "Pechuga de Pollo, Arroz Integral, Manzana...",
                NutricionistaId = usuarioNutriAlimentos.Id,
                EstudianteId = usuarioEstudiante.Id
            };

            var menu2 = new MenuDiario
            {
                NombreMenu = "Almuerzo Completo",
                FechaMenu = new DateTime(2024, 1, 14),
                TipoComida = "Almuerzo",
                EstadoVariante = "Estándar",
                TotalCalorias = 520m,
                TotalProteinas = 36m,
                TotalCarbohidratos = 52m,
                TotalGrasas = 14m,
                Semaforo = "Verde",
                EsSaludable = true,
                DetalleAlimentosResumen = "Salmón, Quinoa, Brócoli, Espinaca...",
                NutricionistaId = usuarioNutriAlimentos.Id,
                EstudianteId = usuarioEstudiante.Id
            };

            var menu3 = new MenuDiario
            {
                NombreMenu = "Merienda Ligera",
                FechaMenu = new DateTime(2024, 1, 13),
                TipoComida = "Merienda",
                EstadoVariante = "Usado esta semana",
                TotalCalorias = 245m,
                TotalProteinas = 12m,
                TotalCarbohidratos = 28m,
                TotalGrasas = 8m,
                Semaforo = "Verde",
                EsSaludable = true,
                DetalleAlimentosResumen = "Yogurt Natural, Manzana, Almendras...",
                NutricionistaId = usuarioNutriAlimentos.Id,
                EstudianteId = usuarioEstudiante.Id
            };

            var menu4 = new MenuDiario
            {
                NombreMenu = "Cena Saludable",
                FechaMenu = new DateTime(2024, 1, 15),
                TipoComida = "Cena",
                EstadoVariante = "Nuevo",
                TotalCalorias = 378m,
                TotalProteinas = 26m,
                TotalCarbohidratos = 35m,
                TotalGrasas = 11m,
                Semaforo = "Amarillo",
                EsSaludable = true,
                DetalleAlimentosResumen = "Pollo, Verduras al vapor, Ensalada...",
                NutricionistaId = usuarioNutriAlimentos.Id,
                EstudianteId = usuarioEstudiante.Id
            };

            await context.MenusDiarios.AddRangeAsync(menu1, menu2, menu3, menu4);
            await context.SaveChangesAsync();

            // Detalles del Menú 1
            await context.DetallesMenuAlimentos.AddRangeAsync(
                new DetalleMenuAlimento { MenuDiarioId = menu1.Id, AlimentoId = alimentosSemilla[0].Id, Cantidad = 150m, Unidad = "g", SubtotalCalorias = 248m, SubtotalProteinas = 31m, SubtotalCarbohidratos = 0m, SubtotalGrasas = 3.6m },
                new DetalleMenuAlimento { MenuDiarioId = menu1.Id, AlimentoId = alimentosSemilla[1].Id, Cantidad = 80m, Unidad = "g", SubtotalCalorias = 173m, SubtotalProteinas = 4m, SubtotalCarbohidratos = 36m, SubtotalGrasas = 1.4m },
                new DetalleMenuAlimento { MenuDiarioId = menu1.Id, AlimentoId = alimentosSemilla[2].Id, Cantidad = 1m, Unidad = "unid", SubtotalCalorias = 52m, SubtotalProteinas = 0.3m, SubtotalCarbohidratos = 14m, SubtotalGrasas = 0.2m }
            );
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
