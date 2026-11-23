using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Modules.Habitos.Services;

namespace SmartNutriTracker.Web.Modules.Habitos.Controllers;

/// <summary>
/// API REST del Equipo 4 (Módulo 04: Registro de Hábitos). Requiere token JWT Bearer (HU-19 a HU-22).
/// Cada operación se limita al estudiante dueño del token (claim NameIdentifier).
/// </summary>
[ApiController]
[Route("api/habitos")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Estudiante,Admin")]
public class HabitosApiController(AppDbContext db) : ControllerBase
{
    public record HabitoApiRequest(DateTime Fecha, int AguaMl, decimal? HorasSueno, int ActividadMin, string? Nota);
    public record ComidaApiRequest(string TipoComida, string Descripcion, decimal? Kcal);

    /// <summary>HU-21/HU-22: historial de hábitos. Por defecto, los últimos 30 días.</summary>
    [HttpGet("mis-registros")]
    public async Task<IActionResult> MisRegistros([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        if (UsuarioId() is not int usuarioId) return Unauthorized();

        var hoy = DateTime.Today;
        var datos = await HabitosLogica.HistorialAsync(db, usuarioId, desde ?? hoy.AddDays(-30), hasta ?? hoy);
        return Ok(datos.Select(r => new
        {
            r.Id, r.Fecha, r.AguaMl, r.HorasSueno, r.ActividadMin, r.Nota
        }));
    }

    /// <summary>HU-20/HU-21: registra agua, sueño y actividad física del día.</summary>
    [HttpPost]
    public async Task<IActionResult> Crear(HabitoApiRequest req)
    {
        if (UsuarioId() is not int usuarioId) return Unauthorized();

        var resultado = await HabitosLogica.GuardarHabitoAsync(
            db, usuarioId, null, req.Fecha, req.AguaMl, req.HorasSueno, req.ActividadMin, req.Nota);

        return resultado.Ok
            ? Ok(new { registroHabitoId = resultado.Id })
            : Conflict(new { mensaje = resultado.Error });
    }

    /// <summary>HU-22: corrige un registro diario guardado.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, HabitoApiRequest req)
    {
        if (UsuarioId() is not int usuarioId) return Unauthorized();

        var resultado = await HabitosLogica.GuardarHabitoAsync(
            db, usuarioId, id, req.Fecha, req.AguaMl, req.HorasSueno, req.ActividadMin, req.Nota);

        if (resultado.Ok) return Ok(new { registroHabitoId = resultado.Id });
        return resultado.Error == "No se encontró el registro."
            ? NotFound(new { mensaje = resultado.Error })
            : Conflict(new { mensaje = resultado.Error });
    }

    /// <summary>HU-19: registra una comida consumida.</summary>
    [HttpPost("comidas")]
    public async Task<IActionResult> RegistrarComida(ComidaApiRequest req)
    {
        if (UsuarioId() is not int usuarioId) return Unauthorized();

        var resultado = await HabitosLogica.RegistrarComidaAsync(
            db, usuarioId, req.TipoComida, req.Descripcion, req.Kcal);

        return resultado.Ok
            ? Ok(new { registroComidaId = resultado.Id })
            : BadRequest(new { mensaje = resultado.Error });
    }

    /// <summary>Comidas registradas en una fecha.</summary>
    [HttpGet("comidas")]
    public async Task<IActionResult> ComidasDelDia([FromQuery] DateTime fecha)
    {
        if (UsuarioId() is not int usuarioId) return Unauthorized();

        var datos = await HabitosLogica.ComidasDelDiaAsync(db, usuarioId, fecha);
        return Ok(datos.Select(c => new
        {
            c.Id, c.FechaHora, c.TipoComida, c.Descripcion, c.Kcal
        }));
    }

    private int? UsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return int.TryParse(valor, out var id) ? id : null;
    }
}
