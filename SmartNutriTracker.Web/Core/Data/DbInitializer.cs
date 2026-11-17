using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Models;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Inicializador de base de datos y siembra automática de las 6 credenciales de prueba por equipo.
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
                logger.LogInformation("[SEED] Base de datos verificada. Las 6 cuentas de equipo ya existen.");
                return;
            }

            logger.LogInformation("[SEED] Insertando las 6 cuentas semilla de desarrollo para el Colegio Innovare...");

            var passwordHash = HashPassword(DefaultSeedPassword);

            var usuariosSemilla = new List<Usuario>
            {
                new()
                {
                    NombreCompleto = "Ing. Carlos Ramos (Líder Equipo 1)",
                    Email = "equipo1.admin@innovare.edu.bo",
                    PasswordHash = passwordHash,
                    Rol = "Admin",
                    EquipoAsignado = "Equipo 1 - Autenticación",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new()
                {
                    NombreCompleto = "Prof. Roberto Mendoza (Líder Equipo 2)",
                    Email = "equipo2.docente@innovare.edu.bo",
                    PasswordHash = passwordHash,
                    Rol = "Docente",
                    EquipoAsignado = "Equipo 2 - Perfil y Reportes",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new()
                {
                    NombreCompleto = "Lic. Sofía Valverde (Líder Equipo 3)",
                    Email = "equipo3.nutri@innovare.edu.bo",
                    PasswordHash = passwordHash,
                    Rol = "Nutricionista",
                    EquipoAsignado = "Equipo 3 - Gestión de Alimentos",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new()
                {
                    NombreCompleto = "Mateo Quispe Flores (Líder Equipo 4)",
                    Email = "equipo4.est@innovare.edu.bo",
                    PasswordHash = passwordHash,
                    Rol = "Estudiante",
                    EquipoAsignado = "Equipo 4 - Registro de Hábitos",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new()
                {
                    NombreCompleto = "Lic. Mariana Morales (Líder Equipo 5)",
                    Email = "equipo5.nutri@innovare.edu.bo",
                    PasswordHash = passwordHash,
                    Rol = "Nutricionista",
                    EquipoAsignado = "Equipo 5 - Recomendaciones",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new()
                {
                    NombreCompleto = "Ing. Elena Ríos (Líder Equipo 6)",
                    Email = "equipo6.qa@innovare.edu.bo",
                    PasswordHash = passwordHash,
                    Rol = "Admin",
                    EquipoAsignado = "Equipo 6 - Notificaciones y QA",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                }
            };

            await context.Usuarios.AddRangeAsync(usuariosSemilla);

            await context.AuditoriasAcceso.AddAsync(new AuditoriaAcceso
            {
                Email = "sistema@innovare.edu.bo",
                Rol = "Admin",
                EquipoAsignado = "Arquitectura Core",
                Exitoso = true,
                Origen = "DbInitializer Seed",
                Detalle = "Siembra automática de las 6 credenciales de equipo completada (HU-01 / HU-05).",
                FechaHora = DateTime.Now
            });

            await context.SaveChangesAsync();
            logger.LogInformation("[SEED] Se crearon las 6 cuentas de prueba exitosamente (Contraseña: Innovare2026*).");
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
