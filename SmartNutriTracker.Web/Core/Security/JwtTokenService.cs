using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmartNutriTracker.Web.Core.Models;

namespace SmartNutriTracker.Web.Core.Security;

public interface IJwtTokenService
{
    (string Token, DateTime Expiracion) GenerarToken(Usuario usuario);
}

/// <summary>
/// Servicio del núcleo (Core) encargado de emitir tokens JWT firmados con claims estándar (NameIdentifier, Email, Name, Role, EquipoAsignado).
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime Expiracion) GenerarToken(Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? "ClaveSecretaSuperSegura_SmartNutriTracker_ColegioInnovare_2026_Min32Bytes!";
        var issuer = jwtSettings["Issuer"] ?? "SmartNutriTracker.Innovare";
        var audience = jwtSettings["Audience"] ?? "SmartNutriTracker.Equipos";
        var expirationMinutes = int.TryParse(jwtSettings["ExpirationMinutes"], out int mins) ? mins : 480;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.Role, usuario.Rol),
            new("EquipoAsignado", usuario.EquipoAsignado)
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiracion,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (tokenString, expiracion);
    }
}
