using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Modules.Alimentos.Models;

namespace SmartNutriTracker.Web.Modules.Alimentos;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class MenusModel : PageModel
{
    private readonly AppDbContext _context;

    public MenusModel(AppDbContext context)
    {
        _context = context;
    }

    public List<MenuDiario> Menus { get; set; } = new();

    public async Task OnGetAsync()
    {
        Menus = await _context.MenusDiarios
            .Include(m => m.Items)
                .ThenInclude(i => i.Alimento)
            .OrderByDescending(m => m.FechaMenu)
            .ToListAsync();
    }
}
