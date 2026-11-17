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
}
