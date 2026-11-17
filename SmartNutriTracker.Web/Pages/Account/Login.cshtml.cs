using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Core.Security;
using SmartNutriTracker.Web.Modules.Autenticacion.DTOs;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;

namespace SmartNutriTracker.Web.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        AppDbContext context,
        IJwtTokenService jwtTokenService,
        ILogger<LoginModel> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [BindProperty]
    public LoginRequestDto Input { get; set; } = new();

    public string? MensajeError { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return Redirect("/Dashboard");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            MensajeError = "Por favor, completa tu correo electrónico y contraseña.";
            return Page();
        }

        var fecha = DateTime.Now;
        var emailNormalizado = Input.Email.Trim().ToLowerInvariant();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNormalizado);

        if (usuario == null || !usuario.Activo || !DbInitializer.VerifyPassword(Input.Password, usuario.PasswordHash))
        {
            var rolFallido = usuario?.Rol ?? "Desconocido";
            var equipoFallido = usuario?.EquipoAsignado ?? "No identificado";

            // HU-05: Auditoría en ILogger
            _logger.LogWarning(
                "[AUDITORÍA LOGIN WEB - FALLIDO] Fecha: {Fecha:yyyy-MM-dd HH:mm:ss} | Correo: {Email} | Rol: {Rol} | Equipo: {Equipo}",
                fecha, Input.Email, rolFallido, equipoFallido);

            _context.AuditoriasAcceso.Add(new AuditoriaAcceso
            {
                Email = Input.Email,
                Rol = rolFallido,
                EquipoAsignado = equipoFallido,
                Exitoso = false,
                Origen = "Web Razor (Cookie)",
                Detalle = "Credenciales incorrectas desde Login.cshtml",
                FechaHora = fecha
            });
            await _context.SaveChangesAsync();

            MensajeError = "Correo o contraseña incorrectos. Usa los botones de Acceso Rápido para Desarrollo (Equipos 1–6).";
            return Page();
        }

        var (jwtToken, _) = _jwtTokenService.GenerarToken(usuario);

        // HU-05: Auditoría en ILogger
        _logger.LogInformation(
            "[AUDITORÍA LOGIN WEB - EXITOSO] Fecha: {Fecha:yyyy-MM-dd HH:mm:ss} | Correo: {Email} | Rol: {Rol} | Equipo: {Equipo}",
            fecha, usuario.Email, usuario.Rol, usuario.EquipoAsignado);

        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = usuario.Email,
            Rol = usuario.Rol,
            EquipoAsignado = usuario.EquipoAsignado,
            Exitoso = true,
            Origen = "Web Razor (Cookie)",
            Detalle = "Acceso exitoso al sistema web Razor",
            FechaHora = fecha
        });
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Rol),
            new("EquipoAsignado", usuario.EquipoAsignado),
            new("JwtToken", jwtToken)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties
        {
            IsPersistent = Input.Recordarme,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);

        return Redirect("/Dashboard");
    }
}
