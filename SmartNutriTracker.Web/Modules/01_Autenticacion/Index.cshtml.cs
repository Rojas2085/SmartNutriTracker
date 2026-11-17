using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Core.Models;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;

namespace SmartNutriTracker.Web.Modules.Autenticacion;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Usuario> Usuarios { get; set; } = new();
    public List<AuditoriaAcceso> LogsAuditoria { get; set; } = new();

    public async Task OnGetAsync()
    {
        Usuarios = await _context.Usuarios.OrderBy(u => u.Id).ToListAsync();
        LogsAuditoria = await _context.AuditoriasAcceso
            .OrderByDescending(a => a.FechaHora)
            .Take(20)
            .ToListAsync();
    }
}
