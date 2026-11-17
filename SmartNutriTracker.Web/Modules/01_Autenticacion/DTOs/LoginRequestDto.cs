using System.ComponentModel.DataAnnotations;

namespace SmartNutriTracker.Web.Modules.Autenticacion.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Ingresa tu correo electrónico institucional.")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido (ej. equipo1.admin@innovare.edu.bo).")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu contraseña.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool Recordarme { get; set; } = true;
}

public class LoginResponseDto
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime? Expiracion { get; set; }
    public UsuarioInfoDto? Usuario { get; set; }
}

public class UsuarioInfoDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string EquipoAsignado { get; set; } = string.Empty;
}
