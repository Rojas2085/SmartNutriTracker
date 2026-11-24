using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Core.Models;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;
using SmartNutriTracker.Web.Modules.PerfilReportes.Models;

namespace SmartNutriTracker.Web.Modules.Autenticacion;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Usuario> Usuarios { get; set; } = new();
    public List<AuditoriaAcceso> LogsAuditoria { get; set; } = new();

    // Métricas del Panel de Administración
    public int TotalUsuarios { get; set; }
    public int TotalEstudiantes { get; set; }
    public int TotalDocentes { get; set; }
    public int TotalNutricionistas { get; set; }
    public int TotalAdmins { get; set; }

    [BindProperty]
    public NuevoUsuarioForm Input { get; set; } = new();

    public string? MensajeExito { get; set; }
    public string? MensajeError { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool Nuevo { get; set; }

    public class NuevoUsuarioForm
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico institucional es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150)]
        [Display(Name = "Correo Electrónico Institucional")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña inicial es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Inicial")]
        public string Password { get; set; } = "Innovare2026*";

        [Required(ErrorMessage = "Debes confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; } = "Innovare2026*";

        [Required(ErrorMessage = "Selecciona un rol para el usuario.")]
        [Display(Name = "Rol en el Sistema")]
        public string Rol { get; set; } = "Estudiante";

        [Required(ErrorMessage = "El equipo o área asignada es obligatoria.")]
        [StringLength(100)]
        [Display(Name = "Equipo o Área Asignada")]
        public string EquipoAsignado { get; set; } = "General / Innovare";

        [Display(Name = "Cuenta Activa")]
        public bool Activo { get; set; } = true;
    }

    public async Task OnGetAsync()
    {
        MensajeExito = TempData["MensajeExito"] as string;
        MensajeError = TempData["MensajeError"] as string;

        await CargarDatosAsync();
    }

    /// <summary>
    /// Registra un nuevo usuario en la base de datos (HU-01, HU-04).
    /// </summary>
    public async Task<IActionResult> OnPostCrearUsuarioAsync()
    {
        if (!ModelState.IsValid)
        {
            MensajeError = "Por favor verifica los datos ingresados en el formulario.";
            await CargarDatosAsync();
            Nuevo = true;
            return Page();
        }

        var emailNormalizado = Input.Email.Trim().ToLowerInvariant();

        // Validar que el correo no esté duplicado
        var yaExiste = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == emailNormalizado);
        if (yaExiste)
        {
            MensajeError = $"El correo institucional '{Input.Email.Trim()}' ya está registrado por otro usuario.";
            await CargarDatosAsync();
            Nuevo = true;
            return Page();
        }

        // Validar rol
        var rolesValidos = new[] { "Admin", "Docente", "Nutricionista", "Estudiante" };
        var rolFinal = rolesValidos.FirstOrDefault(r => r.Equals(Input.Rol?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? "Estudiante";

        // Crear usuario
        var nuevoUsuario = new Usuario
        {
            NombreCompleto = Input.NombreCompleto.Trim(),
            Email = emailNormalizado,
            PasswordHash = DbInitializer.HashPassword(Input.Password),
            Rol = rolFinal,
            EquipoAsignado = string.IsNullOrWhiteSpace(Input.EquipoAsignado) ? "General / Innovare" : Input.EquipoAsignado.Trim(),
            Activo = Input.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        // Si el usuario es Estudiante, crear registro base en PerfilesEstudiantes para que pueda operar
        if (rolFinal == "Estudiante")
        {
            var perfilExiste = await _context.PerfilesEstudiantes.AnyAsync(p => p.UsuarioId == nuevoUsuario.Id);
            if (!perfilExiste)
            {
                _context.PerfilesEstudiantes.Add(new PerfilEstudiante
                {
                    UsuarioId = nuevoUsuario.Id,
                    CodigoEstudiante = $"EST-{DateTime.Now.Year}-{nuevoUsuario.Id:D3}",
                    GradoSeccion = "Por Asignar",
                    FechaNacimiento = DateTime.Today.AddYears(-15),
                    Sexo = "M",
                    PesoKg = 50.0m,
                    TallaCm = 160.0m,
                    Imc = 19.53m,
                    ClasificacionImc = "Normal",
                    ObjetivoNutricional = "Mantener",
                    FechaRegistro = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
        }

        // Registrar auditoría (HU-05)
        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Administrador";
        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = nuevoUsuario.Email,
            Rol = nuevoUsuario.Rol,
            EquipoAsignado = nuevoUsuario.EquipoAsignado,
            Exitoso = true,
            Origen = "Panel Admin Web (/Modules/01_Autenticacion)",
            Detalle = $"Nuevo usuario '{nuevoUsuario.NombreCompleto}' creado por el Administrador '{adminEmail}'.",
            FechaHora = DateTime.Now
        });
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "[PANEL ADMIN] Nuevo usuario creado: {Email} con rol {Rol} por {Admin}",
            nuevoUsuario.Email, nuevoUsuario.Rol, adminEmail);

        TempData["MensajeExito"] = $"¡Usuario '{nuevoUsuario.NombreCompleto}' ({nuevoUsuario.Email}) creado exitosamente con el rol '{nuevoUsuario.Rol}'!";
        return RedirectToPage(new { nuevo = false });
    }

    /// <summary>
    /// Cambia el estado (Activo / Inactivo) de un usuario en el sistema.
    /// </summary>
    public async Task<IActionResult> OnPostToggleEstadoAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            TempData["MensajeError"] = "El usuario seleccionado no existe.";
            return RedirectToPage();
        }

        // Prevenir que el admin se desactive a sí mismo
        var currentEmail = User.FindFirstValue(ClaimTypes.Email)?.ToLowerInvariant();
        if (usuario.Email.ToLowerInvariant() == currentEmail)
        {
            TempData["MensajeError"] = "No puedes desactivar tu propia cuenta de administrador activa.";
            return RedirectToPage();
        }

        usuario.Activo = !usuario.Activo;
        await _context.SaveChangesAsync();

        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Administrador";
        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = usuario.Email,
            Rol = usuario.Rol,
            EquipoAsignado = usuario.EquipoAsignado,
            Exitoso = true,
            Origen = "Panel Admin Web",
            Detalle = $"Estado de cuenta cambiado a {(usuario.Activo ? "Activo" : "Inactivo")} por Admin '{adminEmail}'.",
            FechaHora = DateTime.Now
        });
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] = $"Estado de la cuenta '{usuario.NombreCompleto}' actualizado a: {(usuario.Activo ? "Activo" : "Inactivo")}.";
        return RedirectToPage();
    }

    private async Task CargarDatosAsync()
    {
        Usuarios = await _context.Usuarios
            .OrderByDescending(u => u.Id)
            .ToListAsync();

        TotalUsuarios = Usuarios.Count;
        TotalEstudiantes = Usuarios.Count(u => u.Rol == "Estudiante");
        TotalDocentes = Usuarios.Count(u => u.Rol == "Docente");
        TotalNutricionistas = Usuarios.Count(u => u.Rol == "Nutricionista");
        TotalAdmins = Usuarios.Count(u => u.Rol == "Admin");

        LogsAuditoria = await _context.AuditoriasAcceso
            .OrderByDescending(a => a.FechaHora)
            .Take(25)
            .ToListAsync();
    }
}
