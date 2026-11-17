using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartNutriTracker.Web.Modules.Recomendaciones;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
