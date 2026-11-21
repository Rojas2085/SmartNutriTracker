using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Modules.Alimentos.Models;

namespace SmartNutriTracker.Web.Modules.Alimentos;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Alimento> Alimentos { get; set; } = new();
    public int TotalAlimentos { get; set; }
    public int TotalSaludables { get; set; }
    public int TotalFavoritos { get; set; }

    public async Task OnGetAsync()
    {
        Alimentos = await _context.Alimentos
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombre)
            .ToListAsync();

        TotalAlimentos = Alimentos.Count;
        TotalSaludables = Alimentos.Count(a => a.SemaforoNutricional == "Verde");
        TotalFavoritos = Alimentos.Count(a => a.EsFavorito);
    }
}
