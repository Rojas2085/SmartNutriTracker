using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartNutriTracker.Web.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync() => await CerrarSesionAsync();

    public async Task<IActionResult> OnPostAsync() => await CerrarSesionAsync();

    private async Task<IActionResult> CerrarSesionAsync()
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "Anónimo";
        var rol = User.FindFirstValue(ClaimTypes.Role) ?? "N/A";
        var equipo = User.FindFirstValue("EquipoAsignado") ?? "N/A";

        _logger.LogInformation(
            "[AUDITORÍA LOGOUT] Fecha: {Fecha:yyyy-MM-dd HH:mm:ss} | Correo: {Email} | Rol: {Rol} | Equipo: {Equipo}",
            DateTime.Now, email, rol, equipo);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/Account/Login");
    }
}
