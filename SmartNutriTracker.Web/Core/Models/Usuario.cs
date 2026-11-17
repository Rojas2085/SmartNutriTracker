using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartNutriTracker.Web.Core.Models;

/// <summary>
/// Entidad base de Usuario en el núcleo compartido (Core).
/// Soporta los roles: Admin, Docente, Nutricionista y Estudiante.
/// </summary>
[Table("Usuarios")]
public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(120)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Roles soportados: Admin, Docente, Nutricionista, Estudiante (HU-04).
    /// </summary>
    [Required]
    [StringLength(40)]
    public string Rol { get; set; } = "Estudiante";

    /// <summary>
    /// Identifica a cuál de los 6 equipos pertenece la cuenta semilla.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string EquipoAsignado { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
