using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Core.Security;
using SmartNutriTracker.Web.Modules.Autenticacion.DTOs;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;

namespace SmartNutriTracker.Web.Modules.Autenticacion.Controllers;

/// <summary>
/// Controlador API REST del Equipo 1 (Módulo 01: Autenticación y Seguridad).
/// Emite tokens JWT y registra auditoría en ILogger y Base de Datos (HU-02, HU-03, HU-05).
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthApiController> _logger;

    public AuthApiController(
        AppDbContext context,
        IJwtTokenService jwtTokenService,
        ILogger<AuthApiController> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Inicia sesión vía API REST y retorna un token JWT firmado (HU-02, HU-03).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var fecha = DateTime.Now;
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNormalizado);

        if (usuario == null || !usuario.Activo || !DbInitializer.VerifyPassword(request.Password, usuario.PasswordHash))
        {
            var rolFallido = usuario?.Rol ?? "Desconocido";
            var equipoFallido = usuario?.EquipoAsignado ?? "No identificado";

            // HU-05: Auditoría en ILogger
            _logger.LogWarning(
                "[AUDITORÍA LOGIN API - FALLIDO] Fecha: {Fecha:yyyy-MM-dd HH:mm:ss} | Correo: {Email} | Rol: {Rol} | Equipo: {Equipo}",
                fecha, request.Email, rolFallido, equipoFallido);

            _context.AuditoriasAcceso.Add(new AuditoriaAcceso
            {
                Email = request.Email,
                Rol = rolFallido,
                EquipoAsignado = equipoFallido,
                Exitoso = false,
                Origen = "API REST (JWT)",
                Detalle = "Intento de inicio de sesión fallido en POST /api/auth/login",
                FechaHora = fecha
            });
            await _context.SaveChangesAsync();

            return Unauthorized(new LoginResponseDto
            {
                Exito = false,
                Mensaje = "Credenciales inválidas. Verifica correo y contraseña."
            });
        }

        var (token, expiracion) = _jwtTokenService.GenerarToken(usuario);

        // HU-05: Auditoría en ILogger
        _logger.LogInformation(
            "[AUDITORÍA LOGIN API - EXITOSO] Fecha: {Fecha:yyyy-MM-dd HH:mm:ss} | Correo: {Email} | Rol: {Rol} | Equipo: {Equipo}",
            fecha, usuario.Email, usuario.Rol, usuario.EquipoAsignado);

        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = usuario.Email,
            Rol = usuario.Rol,
            EquipoAsignado = usuario.EquipoAsignado,
            Exitoso = true,
            Origen = "API REST (JWT)",
            Detalle = "Token JWT emitido exitosamente",
            FechaHora = fecha
        });
        await _context.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            Exito = true,
            Mensaje = $"Autenticación exitosa para {usuario.NombreCompleto} ({usuario.Rol})",
            Token = token,
            Expiracion = expiracion,
            Usuario = new UsuarioInfoDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Rol = usuario.Rol,
                EquipoAsignado = usuario.EquipoAsignado
            }
        });
    }

    /// <summary>
    /// Valida el token JWT Bearer enviado en la cabecera Authorization y devuelve los claims del usuario autenticado.
    /// </summary>
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult ObtenerUsuarioActual()
    {
        return Ok(new
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            NombreCompleto = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Rol = User.FindFirstValue(ClaimTypes.Role),
            EquipoAsignado = User.FindFirstValue("EquipoAsignado"),
            Estado = "Token JWT válido y verificado"
        });
    }

    /// <summary>
    /// Consulta el historial de auditoría de accesos (HU-05). Requiere token JWT con Rol 'Admin'.
    /// </summary>
    [HttpGet("auditoria")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> ObtenerAuditoria()
    {
        var logs = await _context.AuditoriasAcceso
            .OrderByDescending(a => a.FechaHora)
            .Take(50)
            .ToListAsync();

        return Ok(logs);
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema con su rol correspondiente (HU-01, HU-04). Requiere token JWT de Administrador.
    /// </summary>
    [HttpPost("usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(CrearUsuarioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CrearUsuarioResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errores = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(new CrearUsuarioResponseDto { Exito = false, Mensaje = errores });
        }

        var rolesPermitidos = new[] { "Admin", "Docente", "Nutricionista", "Estudiante" };
        var rolValido = rolesPermitidos.FirstOrDefault(r => r.Equals(request.Rol?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (rolValido == null)
        {
            return BadRequest(new CrearUsuarioResponseDto
            {
                Exito = false,
                Mensaje = $"Rol inválido '{request.Rol}'. Los roles válidos son: {string.Join(", ", rolesPermitidos)}"
            });
        }

        var emailNormalizado = request.Email.Trim().ToLowerInvariant();
        var yaExiste = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == emailNormalizado);
        if (yaExiste)
        {
            return BadRequest(new CrearUsuarioResponseDto
            {
                Exito = false,
                Mensaje = $"El correo '{request.Email}' ya se encuentra registrado en el sistema."
            });
        }

        var nuevoUsuario = new SmartNutriTracker.Web.Core.Models.Usuario
        {
            NombreCompleto = request.NombreCompleto.Trim(),
            Email = emailNormalizado,
            PasswordHash = DbInitializer.HashPassword(request.Password),
            Rol = rolValido,
            EquipoAsignado = string.IsNullOrWhiteSpace(request.EquipoAsignado) ? "General / Innovare" : request.EquipoAsignado.Trim(),
            Activo = request.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        // Si es Estudiante, inicializamos su ficha en PerfilesEstudiantes
        if (rolValido == "Estudiante")
        {
            var perfilExiste = await _context.PerfilesEstudiantes.AnyAsync(p => p.UsuarioId == nuevoUsuario.Id);
            if (!perfilExiste)
            {
                _context.PerfilesEstudiantes.Add(new SmartNutriTracker.Web.Modules.PerfilReportes.Models.PerfilEstudiante
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

        // Auditoría del registro
        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "admin@innovare.edu.bo";
        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = nuevoUsuario.Email,
            Rol = nuevoUsuario.Rol,
            EquipoAsignado = nuevoUsuario.EquipoAsignado,
            Exitoso = true,
            Origen = "API REST Admin (POST /api/auth/usuarios)",
            Detalle = $"Usuario '{nuevoUsuario.NombreCompleto}' creado por Admin '{adminEmail}'",
            FechaHora = DateTime.Now
        });
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new CrearUsuarioResponseDto
        {
            Exito = true,
            Mensaje = $"Usuario '{nuevoUsuario.NombreCompleto}' creado exitosamente con rol {nuevoUsuario.Rol}.",
            Usuario = new UsuarioInfoDto
            {
                Id = nuevoUsuario.Id,
                NombreCompleto = nuevoUsuario.NombreCompleto,
                Email = nuevoUsuario.Email,
                Rol = nuevoUsuario.Rol,
                EquipoAsignado = nuevoUsuario.EquipoAsignado
            }
        });
    }

    /// <summary>
    /// Lista los usuarios registrados en el sistema (HU-01, HU-04). Requiere token JWT de Administrador.
    /// </summary>
    [HttpGet("usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _context.Usuarios
            .OrderByDescending(u => u.Id)
            .Select(u => new
            {
                u.Id,
                u.NombreCompleto,
                u.Email,
                u.Rol,
                u.EquipoAsignado,
                u.Activo,
                u.FechaCreacion
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    public record CambiarRolRequestDto(string NuevoRol);
    public record EditarUsuarioRequestDto(string NombreCompleto, string Email, string Rol, string EquipoAsignado, bool Activo, string? NuevaPassword = null);

    /// <summary>
    /// Edita los datos completos de un usuario existente (HU-01, HU-04, HU-05). Requiere token JWT de Administrador.
    /// </summary>
    [HttpPut("usuarios/{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> EditarUsuario(int id, [FromBody] EditarUsuarioRequestDto request)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound(new { exito = false, mensaje = "Usuario no encontrado." });

        if (string.IsNullOrWhiteSpace(request.NombreCompleto) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { exito = false, mensaje = "NombreCompleto y Email son obligatorios." });

        var emailNormalizado = request.Email.Trim().ToLowerInvariant();
        if (await _context.Usuarios.AnyAsync(u => u.Id != id && u.Email.ToLower() == emailNormalizado))
            return BadRequest(new { exito = false, mensaje = $"El correo '{emailNormalizado}' ya está registrado." });

        var rolesPermitidos = new[] { "Admin", "Docente", "Nutricionista", "Estudiante" };
        var rolValido = rolesPermitidos.FirstOrDefault(r => r.Equals(request.Rol?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (rolValido == null)
            return BadRequest(new { exito = false, mensaje = $"Rol inválido. Roles permitidos: {string.Join(", ", rolesPermitidos)}" });

        if (!string.IsNullOrWhiteSpace(request.NuevaPassword))
        {
            if (request.NuevaPassword.Length < 6)
                return BadRequest(new { exito = false, mensaje = "La nueva contraseña debe tener al menos 6 caracteres." });
            usuario.PasswordHash = DbInitializer.HashPassword(request.NuevaPassword);
        }

        usuario.NombreCompleto = request.NombreCompleto.Trim();
        usuario.Email = emailNormalizado;
        usuario.Rol = rolValido;
        usuario.EquipoAsignado = string.IsNullOrWhiteSpace(request.EquipoAsignado) ? "General / Innovare" : request.EquipoAsignado.Trim();
        usuario.Activo = request.Activo;
        await _context.SaveChangesAsync();

        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "admin@innovare.edu.bo";
        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = usuario.Email,
            Rol = usuario.Rol,
            EquipoAsignado = usuario.EquipoAsignado,
            Exitoso = true,
            Origen = "API REST Admin (PUT /api/auth/usuarios)",
            Detalle = $"Usuario '{usuario.NombreCompleto}' editado por Admin '{adminEmail}'",
            FechaHora = DateTime.Now
        });
        await _context.SaveChangesAsync();

        return Ok(new
        {
            exito = true,
            mensaje = $"Usuario '{usuario.NombreCompleto}' actualizado correctamente.",
            usuario = new UsuarioInfoDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Rol = usuario.Rol,
                EquipoAsignado = usuario.EquipoAsignado
            }
        });
    }

    /// <summary>
    /// Cambia el rol de un usuario existente y registra el evento en auditoría (HU-04, HU-05).
    /// </summary>
    [HttpPut("usuarios/{id:int}/rol")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> CambiarRolUsuario(int id, [FromBody] CambiarRolRequestDto request)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound(new { exito = false, mensaje = "Usuario no encontrado." });

        var rolesPermitidos = new[] { "Admin", "Docente", "Nutricionista", "Estudiante" };
        var rolValido = rolesPermitidos.FirstOrDefault(r => r.Equals(request.NuevoRol?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (rolValido == null)
            return BadRequest(new { exito = false, mensaje = $"Rol inválido. Roles permitidos: {string.Join(", ", rolesPermitidos)}" });

        var rolAnterior = usuario.Rol;
        usuario.Rol = rolValido;
        await _context.SaveChangesAsync();

        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "admin@innovare.edu.bo";
        _context.AuditoriasAcceso.Add(new AuditoriaAcceso
        {
            Email = usuario.Email,
            Rol = usuario.Rol,
            EquipoAsignado = usuario.EquipoAsignado,
            Exitoso = true,
            Origen = "API REST Admin (PUT /api/auth/usuarios/rol)",
            Detalle = $"Cambio de rol de '{rolAnterior}' a '{rolValido}' por Admin '{adminEmail}'",
            FechaHora = DateTime.Now
        });
        await _context.SaveChangesAsync();

        return Ok(new { exito = true, mensaje = $"Rol actualizado de '{rolAnterior}' a '{rolValido}'.", usuarioId = usuario.Id, rol = usuario.Rol });
    }

    /// <summary>
    /// Elimina (o desactiva si posee historial) un usuario y registra auditoría (HU-01, HU-05).
    /// </summary>
    [HttpDelete("usuarios/{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound(new { exito = false, mensaje = "Usuario no encontrado." });

        var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "admin@innovare.edu.bo";
        if (usuario.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { exito = false, mensaje = "No puedes eliminar tu propia cuenta de administrador." });

        var emailEliminado = usuario.Email;
        var rolEliminado = usuario.Rol;
        var equipoEliminado = usuario.EquipoAsignado;

        try
        {
            var perfiles = await _context.PerfilesEstudiantes.Where(p => p.UsuarioId == usuario.Id).ToListAsync();
            if (perfiles.Count > 0) _context.PerfilesEstudiantes.RemoveRange(perfiles);

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            _context.AuditoriasAcceso.Add(new AuditoriaAcceso
            {
                Email = emailEliminado,
                Rol = rolEliminado,
                EquipoAsignado = equipoEliminado,
                Exitoso = true,
                Origen = "API REST Admin (DELETE /api/auth/usuarios)",
                Detalle = $"Usuario '{emailEliminado}' eliminado por Admin '{adminEmail}'",
                FechaHora = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return Ok(new { exito = true, mensaje = $"Usuario '{emailEliminado}' eliminado correctamente." });
        }
        catch (DbUpdateException)
        {
            usuario.Activo = false;
            await _context.SaveChangesAsync();

            _context.AuditoriasAcceso.Add(new AuditoriaAcceso
            {
                Email = emailEliminado,
                Rol = rolEliminado,
                EquipoAsignado = equipoEliminado,
                Exitoso = true,
                Origen = "API REST Admin (Baja Lógica)",
                Detalle = $"Usuario '{emailEliminado}' desactivado (tiene registros históricos) por Admin '{adminEmail}'",
                FechaHora = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return Ok(new { exito = true, mensaje = $"Usuario '{emailEliminado}' desactivado para preservar integridad histórica." });
        }
    }
}
