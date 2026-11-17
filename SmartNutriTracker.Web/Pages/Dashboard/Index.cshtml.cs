using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;

namespace SmartNutriTracker.Web.Pages.Dashboard;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public string NombreUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public string RolUsuario { get; set; } = string.Empty;
    public string EquipoAsignado { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public List<AuditoriaAcceso> UltimosAccesos { get; set; } = new();

    public async Task OnGetAsync()
    {
        NombreUsuario = User.Identity?.Name ?? "Usuario Innovare";
        EmailUsuario = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        RolUsuario = User.FindFirstValue(ClaimTypes.Role) ?? "Estudiante";
        EquipoAsignado = User.FindFirstValue("EquipoAsignado") ?? "Equipo General";
        JwtToken = User.FindFirstValue("JwtToken") ?? string.Empty;

        UltimosAccesos = await _context.AuditoriasAcceso
            .OrderByDescending(a => a.FechaHora)
            .Take(8)
            .ToListAsync();
    }
}
