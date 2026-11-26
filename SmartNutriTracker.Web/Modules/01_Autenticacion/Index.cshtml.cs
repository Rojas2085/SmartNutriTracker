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

    public class EditarUsuarioForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; } = "Estudiante";

        [Required(ErrorMessage = "El equipo o área asignada es obligatoria.")]
        [StringLength(100)]
        public string EquipoAsignado { get; set; } = "General / Innovare";

        public string? NuevaPassword { get; set; }

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

    /// <summary>
    /// Actualiza el rol asignado a un usuario existente y registra el cambio en la auditoría (HU-04, HU-05).
    /// </summary>
    public async Task<IActionResult> OnPostCambiarRolAsync(int id, string nuevoRol)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            TempData["MensajeError"] = "El usuario seleccionado no existe.";
            return RedirectToPage();
        }

        var rolesValidos = new[] { "Admin", "Docente", "Nutricionista", "Estudiante" };
        var rolNormalizado = rolesValidos.FirstOrDefault(r => r.Equals(nuevoRol?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (rolNormalizado == null)
        {
            TempData["MensajeError"] = $"Rol inválido '{nuevoRol}'.";
            return RedirectToPage();
        }

        var currentEmail = User.FindFirstValue(ClaimTypes.Email)?.ToLowerInvariant();
        if (usuario.Email.ToLowerInvariant() == currentEmail && rolNormalizado != "Admin")
        {
            TempData["MensajeError"] = "No puedes quitarte el rol de Admin a tu propia cuenta activa.";
            return RedirectToPage();
        }

        if (usuario.Rol == rolNormalizado)
        {
            TempData["MensajeExito"] = $"El usuario '{usuario.NombreCompleto}' ya posee el rol '{rolNormalizado}'.";
            return RedirectToPage();
        }

        var rolAnterior = usuario.Rol;
        usuario.Rol = rolNormalizado;
        await _context.SaveChangesAsync();

        // Si cambió a Estudiante, aseguramos que tenga su perfil base
        if (rolNormalizado == "Estudiante")
        {
            var perfilExiste = await _context.PerfilesEstudiantes.AnyAsync(p => p.UsuarioId == usuario.Id);
            if (!perfilExiste)
            {
                _context.PerfilesEstudiantes.Add(new PerfilEstudiante
                {
                    UsuarioId = usuario.Id,
                    CodigoEstudiante = $"EST-{DateTime.Now.Year}-{usuario.Id:D3}",
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

        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Administrador";
        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = usuario.Email,
            Rol = usuario.Rol,
            EquipoAsignado = usuario.EquipoAsignado,
            Exitoso = true,
            Origen = "Panel Admin Web (HU-04)",
            Detalle = $"Cambio de rol de '{rolAnterior}' a '{rolNormalizado}' para '{usuario.NombreCompleto}' por Admin '{adminEmail}'.",
            FechaHora = DateTime.Now
        });
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "[AUDITORÍA CAMBIO ROL] Usuario: {Email} | Anterior: {RolAnterior} -> Nuevo: {RolNuevo} | Por: {Admin}",
            usuario.Email, rolAnterior, rolNormalizado, adminEmail);

        TempData["MensajeExito"] = $"Rol de '{usuario.NombreCompleto}' actualizado de '{rolAnterior}' a '{rolNormalizado}'.";
        return RedirectToPage();
    }

    /// <summary>
    /// Edita los datos completos de cualquier usuario existente (Estudiante, Docente, Nutricionista o Admin)
    /// y registra la modificación en la auditoría (HU-01, HU-04, HU-05).
    /// </summary>
    public async Task<IActionResult> OnPostEditarUsuarioAsync(EditarUsuarioForm editInput)
    {
        var usuario = await _context.Usuarios.FindAsync(editInput.Id);
        if (usuario == null)
        {
            TempData["MensajeError"] = "El usuario que intentas editar no existe.";
            return RedirectToPage();
        }

        if (string.IsNullOrWhiteSpace(editInput.NombreCompleto) || string.IsNullOrWhiteSpace(editInput.Email))
        {
            TempData["MensajeError"] = "El nombre completo y el correo electrónico son obligatorios.";
            return RedirectToPage();
        }

        var emailAttr = new EmailAddressAttribute();
        var emailNormalizado = editInput.Email.Trim().ToLowerInvariant();
        if (!emailAttr.IsValid(emailNormalizado))
        {
            TempData["MensajeError"] = $"El correo '{editInput.Email}' no tiene un formato válido.";
            return RedirectToPage();
        }

        // Validar que el correo no esté usado por otro usuario
        var correoDuplicado = await _context.Usuarios
            .AnyAsync(u => u.Id != usuario.Id && u.Email.ToLower() == emailNormalizado);
        if (correoDuplicado)
        {
            TempData["MensajeError"] = $"El correo '{emailNormalizado}' ya está registrado por otro usuario.";
            return RedirectToPage();
        }

        // Validar rol permitido
        var rolesValidos = new[] { "Admin", "Docente", "Nutricionista", "Estudiante" };
        var rolNormalizado = rolesValidos.FirstOrDefault(r => r.Equals(editInput.Rol?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (rolNormalizado == null)
        {
            TempData["MensajeError"] = $"Rol inválido '{editInput.Rol}'.";
            return RedirectToPage();
        }

        // Proteger la cuenta del administrador activo
        var currentEmail = User.FindFirstValue(ClaimTypes.Email)?.ToLowerInvariant();
        var esMismoAdmin = usuario.Email.ToLowerInvariant() == currentEmail;
        if (esMismoAdmin && rolNormalizado != "Admin")
        {
            TempData["MensajeError"] = "No puedes cambiar el rol de tu propia cuenta de administrador activa.";
            return RedirectToPage();
        }
        if (esMismoAdmin && !editInput.Activo)
        {
            TempData["MensajeError"] = "No puedes desactivar tu propia cuenta de administrador activa.";
            return RedirectToPage();
        }

        // Validar nueva contraseña si se ingresó
        var cambioPassword = false;
        if (!string.IsNullOrWhiteSpace(editInput.NuevaPassword))
        {
            if (editInput.NuevaPassword.Length < 6)
            {
                TempData["MensajeError"] = "La nueva contraseña debe tener al menos 6 caracteres.";
                return RedirectToPage();
            }
            usuario.PasswordHash = DbInitializer.HashPassword(editInput.NuevaPassword);
            cambioPassword = true;
        }

        var rolAnterior = usuario.Rol;
        usuario.NombreCompleto = editInput.NombreCompleto.Trim();
        usuario.Email = emailNormalizado;
        usuario.Rol = rolNormalizado;
        usuario.EquipoAsignado = string.IsNullOrWhiteSpace(editInput.EquipoAsignado)
            ? "General / Innovare"
            : editInput.EquipoAsignado.Trim();
        usuario.Activo = editInput.Activo;

        await _context.SaveChangesAsync();

        // Si el usuario es o pasó a ser Estudiante, garantizar su ficha base en PerfilesEstudiantes
        if (rolNormalizado == "Estudiante")
        {
            var perfilExiste = await _context.PerfilesEstudiantes.AnyAsync(p => p.UsuarioId == usuario.Id);
            if (!perfilExiste)
            {
                _context.PerfilesEstudiantes.Add(new PerfilEstudiante
                {
                    UsuarioId = usuario.Id,
                    CodigoEstudiante = $"EST-{DateTime.Now.Year}-{usuario.Id:D3}",
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

        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Administrador";
        var detallePass = cambioPassword ? " (incluye cambio de contraseña)" : "";
        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = usuario.Email,
            Rol = usuario.Rol,
            EquipoAsignado = usuario.EquipoAsignado,
            Exitoso = true,
            Origen = "Panel Admin Web (Edición Usuario)",
            Detalle = $"Usuario '{usuario.NombreCompleto}' editado (Rol: '{rolAnterior}' -> '{usuario.Rol}', Estado: {(usuario.Activo ? "Activo" : "Inactivo")}){detallePass} por Admin '{adminEmail}'.",
            FechaHora = DateTime.Now
        });
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "[AUDITORÍA EDICIÓN USUARIO] Id: {Id} | Email: {Email} | Rol: {Rol} | Por: {Admin}",
            usuario.Id, usuario.Email, usuario.Rol, adminEmail);

        TempData["MensajeExito"] = $"¡Usuario '{usuario.NombreCompleto}' actualizado correctamente!";
        return RedirectToPage();
    }

    /// <summary>
    /// Elimina un usuario del sistema y registra la eliminación en la auditoría (HU-01, HU-05).
    /// </summary>
    public async Task<IActionResult> OnPostEliminarUsuarioAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            TempData["MensajeError"] = "El usuario seleccionado no existe.";
            return RedirectToPage();
        }

        var currentEmail = User.FindFirstValue(ClaimTypes.Email)?.ToLowerInvariant();
        if (usuario.Email.ToLowerInvariant() == currentEmail)
        {
            TempData["MensajeError"] = "No puedes eliminar tu propia cuenta de administrador activa.";
            return RedirectToPage();
        }

        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Administrador";
        var emailEliminado = usuario.Email;
        var nombreEliminado = usuario.NombreCompleto;
        var rolEliminado = usuario.Rol;
        var equipoEliminado = usuario.EquipoAsignado;

        try
        {
            // Eliminar perfiles asociados si existen para evitar violaciones de clave foránea
            var perfiles = await _context.PerfilesEstudiantes.Where(p => p.UsuarioId == usuario.Id).ToListAsync();
            if (perfiles.Count > 0)
            {
                _context.PerfilesEstudiantes.RemoveRange(perfiles);
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            _context.AuditoriasAcceso.Add(new AuditoriaAcceso
            {
                Email = emailEliminado,
                Rol = rolEliminado,
                EquipoAsignado = equipoEliminado,
                Exitoso = true,
                Origen = "Panel Admin Web (Eliminación)",
                Detalle = $"Usuario '{nombreEliminado}' ({emailEliminado}) eliminado por Admin '{adminEmail}'.",
                FechaHora = DateTime.Now
            });
            await _context.SaveChangesAsync();

            _logger.LogWarning(
                "[AUDITORÍA ELIMINACIÓN] Usuario eliminado: {Email} ({Rol}) por Admin {Admin}",
                emailEliminado, rolEliminado, adminEmail);

            TempData["MensajeExito"] = $"Usuario '{nombreEliminado}' ({emailEliminado}) eliminado del sistema.";
        }
        catch (DbUpdateException)
        {
            // Si tiene registros históricos en otros módulos (menús, hábitos, etc.), se desactiva en lugar de romper integridad
            usuario.Activo = false;
            await _context.SaveChangesAsync();

            _context.AuditoriasAcceso.Add(new AuditoriaAcceso
            {
                Email = emailEliminado,
                Rol = rolEliminado,
                EquipoAsignado = equipoEliminado,
                Exitoso = true,
                Origen = "Panel Admin Web (Baja Lógica)",
                Detalle = $"Usuario '{nombreEliminado}' desactivado (posee registros históricos asociados) por Admin '{adminEmail}'.",
                FechaHora = DateTime.Now
            });
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = $"'{nombreEliminado}' tiene registros históricos en otros módulos, por lo que fue desactivado para proteger la integridad de datos.";
        }

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
