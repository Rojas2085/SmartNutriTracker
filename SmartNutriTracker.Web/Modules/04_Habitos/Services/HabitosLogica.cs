using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Modules.Habitos.Models;

namespace SmartNutriTracker.Web.Modules.Habitos.Services;

/// <summary>
/// Reglas de negocio del Equipo 4 (HU-19 a HU-22). Compartida por la página Razor y por la API REST.
/// Es estática para no tener que registrar un servicio nuevo en Program.cs (zona protegida del núcleo).
/// Todas las consultas filtran por UsuarioId: cada estudiante solo ve y edita sus propios registros.
/// </summary>
public static class HabitosLogica
{
    public static readonly string[] TiposComida = ["Desayuno", "Almuerzo", "Merienda", "Cena"];

    public const int MaxAguaMl = 10000;
    public const int MaxHorasSueno = 24;
    public const int MaxActividadMin = 1440;
    public const int MaxKcal = 5000;

    public readonly record struct Resultado(bool Ok, string? Error = null, int? Id = null);

    /// <summary>Valida los campos del registro diario. Devuelve null si todo es válido.</summary>
    public static string? ValidarHabito(DateTime fecha, int aguaMl, decimal? horasSueno, int actividadMin)
    {
        if (fecha == default)
            return "La fecha no es válida.";
        if (aguaMl < 0 || aguaMl > MaxAguaMl)
            return $"El agua debe estar entre 0 y {MaxAguaMl} ml.";
        if (horasSueno is < 0m or > 24m)
            return $"Las horas de sueño deben estar entre 0 y {MaxHorasSueno}.";
        if (actividadMin < 0 || actividadMin > MaxActividadMin)
            return $"La actividad física debe estar entre 0 y {MaxActividadMin} minutos.";
        return null;
    }

    /// <summary>Crea (id == null) o actualiza (id != null) el registro diario del estudiante.</summary>
    public static async Task<Resultado> GuardarHabitoAsync(
        AppDbContext db, int usuarioId, int? id, DateTime fecha,
        int aguaMl, decimal? horasSueno, int actividadMin, string? nota)
    {
        var error = ValidarHabito(fecha, aguaMl, horasSueno, actividadMin);
        if (error is not null) return new Resultado(false, error);

        fecha = fecha.Date;
        nota = string.IsNullOrWhiteSpace(nota) ? null : nota.Trim();
        if (nota is not null && nota.Length > 250)
            return new Resultado(false, "La nota no puede superar 250 caracteres.");

        if (id is null)
        {
            var yaExiste = await db.RegistrosHabito.AnyAsync(r => r.UsuarioId == usuarioId && r.Fecha == fecha);
            if (yaExiste)
                return new Resultado(false, "Ya existe un registro para esa fecha. Edítalo desde el historial.");

            var nuevo = new RegistroHabito
            {
                UsuarioId = usuarioId,
                Fecha = fecha,
                AguaMl = aguaMl,
                HorasSueno = horasSueno,
                ActividadMin = actividadMin,
                Nota = nota,
                FechaModificacion = DateTime.UtcNow
            };
            db.RegistrosHabito.Add(nuevo);
            await db.SaveChangesAsync();
            return new Resultado(true, null, nuevo.Id);
        }

        var registro = await db.RegistrosHabito
            .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);
        if (registro is null)
            return new Resultado(false, "No se encontró el registro.");

        if (registro.Fecha != fecha &&
            await db.RegistrosHabito.AnyAsync(r => r.UsuarioId == usuarioId && r.Fecha == fecha))
            return new Resultado(false, "Ya existe otro registro para esa fecha.");

        registro.Fecha = fecha;
        registro.AguaMl = aguaMl;
        registro.HorasSueno = horasSueno;
        registro.ActividadMin = actividadMin;
        registro.Nota = nota;
        registro.FechaModificacion = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return new Resultado(true, null, registro.Id);
    }

    /// <summary>Registros de hábitos del estudiante entre dos fechas (inclusive), más recientes primero.</summary>
    public static Task<List<RegistroHabito>> HistorialAsync(AppDbContext db, int usuarioId, DateTime desde, DateTime hasta)
    {
        desde = desde.Date;
        hasta = hasta.Date;
        return db.RegistrosHabito
            .Where(r => r.UsuarioId == usuarioId && r.Fecha >= desde && r.Fecha <= hasta)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();
    }

    /// <summary>Busca un registro diario del estudiante por id (null si no es suyo o no existe).</summary>
    public static Task<RegistroHabito?> ObtenerHabitoAsync(AppDbContext db, int usuarioId, int id) =>
        db.RegistrosHabito.FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);

    /// <summary>Valida y guarda una comida consumida (HU-19).</summary>
    public static async Task<Resultado> RegistrarComidaAsync(
        AppDbContext db, int usuarioId, string tipoComida, string descripcion, decimal? kcal)
    {
        if (!TiposComida.Contains(tipoComida))
            return new Resultado(false, "El tipo de comida no es válido.");

        descripcion = (descripcion ?? string.Empty).Trim();
        if (descripcion.Length == 0)
            return new Resultado(false, "Describe qué comiste.");
        if (descripcion.Length > 300)
            return new Resultado(false, "La descripción no puede superar 300 caracteres.");

        if (kcal is < 0m or > 5000m)
            return new Resultado(false, $"Las kcal deben estar entre 0 y {MaxKcal}.");

        var comida = new RegistroComida
        {
            UsuarioId = usuarioId,
            FechaHora = DateTime.Now,
            TipoComida = tipoComida,
            Descripcion = descripcion,
            Kcal = kcal
        };
        db.RegistrosComida.Add(comida);
        await db.SaveChangesAsync();
        return new Resultado(true, null, comida.Id);
    }

    /// <summary>Comidas registradas por el estudiante en una fecha, en orden de hora.</summary>
    public static Task<List<RegistroComida>> ComidasDelDiaAsync(AppDbContext db, int usuarioId, DateTime fecha)
    {
        var inicio = fecha.Date;
        var fin = inicio.AddDays(1);
        return db.RegistrosComida
            .Where(c => c.UsuarioId == usuarioId && c.FechaHora >= inicio && c.FechaHora < fin)
            .OrderBy(c => c.FechaHora)
            .ToListAsync();
    }
}
