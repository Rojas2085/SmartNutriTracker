using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartNutriTracker.Web.Modules.Autenticacion.Models;

/// <summary>
/// Entidad del Módulo 01 (Equipo 1) para registrar la bitácora de auditoría de inicios de sesión (HU-05).
/// </summary>
[Table("AuditoriasAcceso")]
public class AuditoriaAcceso
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(40)]
    public string Rol { get; set; } = "Desconocido";

    [StringLength(100)]
    public string EquipoAsignado { get; set; } = "No identificado";

    public bool Exitoso { get; set; }

    [StringLength(50)]
    public string Origen { get; set; } = "Web Razor";

    [StringLength(250)]
    public string Detalle { get; set; } = string.Empty;

    public DateTime FechaHora { get; set; } = DateTime.Now;
}
