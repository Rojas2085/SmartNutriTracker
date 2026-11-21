using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartNutriTracker.Web.Core.Models;

namespace SmartNutriTracker.Web.Modules.Notificaciones.Models;

/// <summary>
/// Alertas automáticas ante hábitos no saludables (excesos o déficits) para intervención docente (HU-27).
/// </summary>
[Table("AlertasNutricionales")]
public class AlertaNutricional
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int EstudianteId { get; set; }

    [ForeignKey(nameof(EstudianteId))]
    public Usuario? Estudiante { get; set; }

    /// <summary>
    /// Exceso Calorías, Déficit Nutricional, Baja Hidratación, Horas de Sueño Insuficientes, Sedentarismo
    /// </summary>
    [Required]
    [StringLength(80)]
    public string TipoAlerta { get; set; } = "Exceso Calorías";

    /// <summary>
    /// Informativa, Moderada, Severa, Crítica
    /// </summary>
    [Required]
    [StringLength(30)]
    public string NivelSeveridad { get; set; } = "Moderada";

    [Required]
    [StringLength(400)]
    public string Mensaje { get; set; } = string.Empty;

    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

    public bool Atendida { get; set; } = false;

    public DateTime? FechaAtencion { get; set; }

    public int? DocenteAtendioId { get; set; }

    [ForeignKey(nameof(DocenteAtendioId))]
    public Usuario? DocenteAtendio { get; set; }

    [StringLength(300)]
    public string ObservacionesAtencion { get; set; } = string.Empty;
}

/// <summary>
/// Notificaciones internas y recordatorios diarios dirigidos a los usuarios escolares (HU-28).
/// </summary>
[Table("NotificacionesInternas")]
public class NotificacionInterna
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UsuarioReceptorId { get; set; }

    [ForeignKey(nameof(UsuarioReceptorId))]
    public Usuario? UsuarioReceptor { get; set; }

    [Required]
    [StringLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [StringLength(400)]
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Recordatorio, Alerta, Felicitacion, MensajeDocente, Sistema
    /// </summary>
    [Required]
    [StringLength(40)]
    public string Tipo { get; set; } = "Recordatorio";

    public bool Leida { get; set; } = false;

    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    public DateTime? FechaLectura { get; set; }
}

/// <summary>
/// Bitácora global de logs del sistema para trazabilidad, excepciones y auditoría técnica (HU-29).
/// </summary>
[Table("LogsGlobalesSistema")]
public class LogGlobalSistema
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// INFO, WARNING, ERROR, CRITICAL
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Nivel { get; set; } = "INFO";

    /// <summary>
    /// Autenticacion, PerfilReportes, Alimentos, Habitos, Recomendaciones, Notificaciones, Core
    /// </summary>
    [Required]
    [StringLength(60)]
    public string ModuloOrigen { get; set; } = "Core";

    [Required]
    [StringLength(500)]
    public string Mensaje { get; set; } = string.Empty;

    public string? DetalleExcepcion { get; set; }

    [StringLength(150)]
    public string? RutaEndpoint { get; set; }

    [StringLength(150)]
    public string? UsuarioEmail { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
}
