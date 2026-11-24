using System.ComponentModel.DataAnnotations;

namespace SmartNutriTracker.Web.Modules.Autenticacion.DTOs;

/// <summary>
/// DTO para la creación de nuevos usuarios desde la API REST o formularios del sistema (HU-01, HU-04).
/// </summary>
public class CrearUsuarioRequestDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres.")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña inicial es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Roles permitidos: Admin, Docente, Nutricionista, Estudiante
    /// </summary>
    [Required(ErrorMessage = "El rol es obligatorio (Admin, Docente, Nutricionista o Estudiante).")]
    public string Rol { get; set; } = "Estudiante";

    /// <summary>
    /// Área de pertenencia institucional o equipo modular asignado
    /// </summary>
    [Required(ErrorMessage = "El equipo o área asignada es obligatoria.")]
    public string EquipoAsignado { get; set; } = "General / Innovare";

    public bool Activo { get; set; } = true;
}

public class CrearUsuarioResponseDto
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public UsuarioInfoDto? Usuario { get; set; }
}
