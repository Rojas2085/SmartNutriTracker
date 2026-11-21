using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Core.Models;
using SmartNutriTracker.Web.Modules.Alimentos.Models;

namespace SmartNutriTracker.Web.Modules.Alimentos;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class CrearMenuModel : PageModel
{
    private readonly AppDbContext _context;

    public CrearMenuModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Alimento> CatalogoAlimentos { get; set; } = new();
    public List<Usuario> Estudiantes { get; set; } = new();

    public async Task OnGetAsync()
    {
        CatalogoAlimentos = await _context.Alimentos
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombre)
            .ToListAsync();

        Estudiantes = await _context.Usuarios
            .Where(u => u.Rol == "Estudiante" || u.Rol == "Admin")
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();
    }
}
