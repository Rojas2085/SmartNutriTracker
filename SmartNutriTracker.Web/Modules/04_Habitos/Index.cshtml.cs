using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Modules.Habitos.Models;
using SmartNutriTracker.Web.Modules.Habitos.Services;

namespace SmartNutriTracker.Web.Modules.Habitos;

/// <summary>
/// Página Razor del Equipo 4 (Registro de hábitos). Usa formularios con POST y redirección (PRG).
/// Solo accesible con sesión por cookie y rol Estudiante (o Admin, que también puede probar el módulo).
/// </summary>
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Estudiante,Admin")]
public class IndexModel(AppDbContext db) : PageModel
{
    public class HabitoForm
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;
        public int AguaMl { get; set; }
        public decimal? HorasSueno { get; set; }
        public int ActividadMin { get; set; }
        public string? Nota { get; set; }
    }

    public class ComidaForm
    {
        public string TipoComida { get; set; } = "Desayuno";
        public string Descripcion { get; set; } = string.Empty;
        public decimal? Kcal { get; set; }
    }

    [BindProperty]
    public HabitoForm Habito { get; set; } = new();

    [BindProperty]
    public ComidaForm Comida { get; set; } = new();

    /// <summary>Id del registro a editar (?Editar=5). Vacío = crear uno nuevo.</summary>
    [BindProperty(SupportsGet = true)]
    public int? Editar { get; set; }

    public List<RegistroHabito> Historial { get; set; } = new();
    public List<RegistroComida> ComidasHoy { get; set; } = new();
    public string[] TiposComida => HabitosLogica.TiposComida;
    public string? Mensaje { get; set; }
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (UsuarioId() is not int usuarioId) return Challenge();

        Mensaje ??= TempData["Mensaje"] as string;

        if (Editar is int idEditar)
        {
            var registro = await HabitosLogica.ObtenerHabitoAsync(db, usuarioId, idEditar);
            if (registro is null)
            {
                Error = "No se encontró el registro que intentas editar.";
                Editar = null;
            }
            else
            {
                Habito = new HabitoForm
                {
                    Id = registro.Id,
                    Fecha = registro.Fecha,
                    AguaMl = registro.AguaMl,
                    HorasSueno = registro.HorasSueno,
                    ActividadMin = registro.ActividadMin,
                    Nota = registro.Nota
                };
            }
        }

        await CargarAsync(usuarioId);
        return Page();
    }

    /// <summary>HU-20/HU-21/HU-22: crea o actualiza el registro diario de hábitos.</summary>
    public async Task<IActionResult> OnPostHabitoAsync()
    {
        if (UsuarioId() is not int usuarioId) return Challenge();

        // Si un campo llega vacío o con texto no numérico, el binding lo deja en su valor por defecto.
        // Lo detectamos aquí para no guardar datos incorrectos en silencio.
        if (!ModelState.IsValid)
        {
            Error = "Revisa los datos: la fecha, el agua, las horas de sueño y la actividad deben ser valores válidos.";
            await CargarAsync(usuarioId);
            return Page();
        }

        var resultado = await HabitosLogica.GuardarHabitoAsync(
            db, usuarioId,
            Habito.Id == 0 ? null : Habito.Id,
            Habito.Fecha, Habito.AguaMl, Habito.HorasSueno, Habito.ActividadMin, Habito.Nota);

        if (!resultado.Ok)
        {
            Error = resultado.Error;
            await CargarAsync(usuarioId);
            return Page();
        }

        TempData["Mensaje"] = Habito.Id == 0 ? "Hábitos guardados." : "Registro actualizado.";
        return RedirectToPage();
    }

    /// <summary>HU-19: registra una comida consumida.</summary>
    public async Task<IActionResult> OnPostComidaAsync()
    {
        if (UsuarioId() is not int usuarioId) return Challenge();

        var resultado = await HabitosLogica.RegistrarComidaAsync(
            db, usuarioId, Comida.TipoComida, Comida.Descripcion, Comida.Kcal);

        if (!resultado.Ok)
        {
            Error = resultado.Error;
            await CargarAsync(usuarioId);
            return Page();
        }

        TempData["Mensaje"] = "Comida registrada.";
        return RedirectToPage();
    }

    private async Task CargarAsync(int usuarioId)
    {
        var hoy = DateTime.Today;
        Historial = await HabitosLogica.HistorialAsync(db, usuarioId, hoy.AddDays(-30), hoy);
        ComidasHoy = await HabitosLogica.ComidasDelDiaAsync(db, usuarioId, hoy);
    }

    private int? UsuarioId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : null;
    }
}
