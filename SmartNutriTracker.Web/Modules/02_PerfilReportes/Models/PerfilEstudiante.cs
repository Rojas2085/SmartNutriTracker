using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartNutriTracker.Web.Core.Models;

namespace SmartNutriTracker.Web.Modules.PerfilReportes.Models;

/// <summary>
/// Ficha antropométrica y perfil de salud del estudiante (HU-08, HU-09, HU-10).
/// </summary>
[Table("PerfilesEstudiantes")]
public class PerfilEstudiante
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }

    [Required]
    [StringLength(30)]
    public string CodigoEstudiante { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string GradoSeccion { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }

    [StringLength(10)]
    public string Sexo { get; set; } = "M"; // "M" / "F"

    [Column(TypeName = "decimal(5,2)")]
    public decimal PesoKg { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TallaCm { get; set; }

    [Column(TypeName = "decimal(4,2)")]
    public decimal Imc { get; set; }

    /// <summary>
    /// Bajo peso, Normal, Sobrepeso, Obesidad
    /// </summary>
    [StringLength(40)]
    public string ClasificacionImc { get; set; } = "Normal";

    /// <summary>
    /// Mantener peso, Bajar de peso, Ganar masa muscular
    /// </summary>
    [StringLength(60)]
    public string ObjetivoNutricional { get; set; } = "Mantener";

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }
}

/// <summary>
/// Historial de evolución de peso, talla e IMC del estudiante (HU-08, HU-09).
/// </summary>
[Table("HistorialesAntropometricos")]
public class HistorialAntropometrico
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PerfilEstudianteId { get; set; }

    [ForeignKey(nameof(PerfilEstudianteId))]
    public PerfilEstudiante? PerfilEstudiante { get; set; }

    public DateTime FechaMedicion { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(5,2)")]
    public decimal PesoKg { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TallaCm { get; set; }

    [Column(TypeName = "decimal(4,2)")]
    public decimal ImcCalculado { get; set; }

    [StringLength(40)]
    public string ClasificacionImc { get; set; } = "Normal";

    [StringLength(250)]
    public string ObservacionesDocente { get; set; } = string.Empty;
}

/// <summary>
/// Resumen semanal de rendimiento nutricional por estudiante para el Docente (HU-11, HU-12).
/// </summary>
[Table("ReportesNutricionalesSemanales")]
public class ReporteNutricionalSemanal
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PerfilEstudianteId { get; set; }

    [ForeignKey(nameof(PerfilEstudianteId))]
    public PerfilEstudiante? PerfilEstudiante { get; set; }

    public int SemanaNumero { get; set; }

    public int Anio { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    [Column(TypeName = "decimal(7,2)")]
    public decimal PromedioCaloriasDiarias { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal CumplimientoAguaPorcentaje { get; set; }

    public int DiasActividadFisica { get; set; }

    [Column(TypeName = "decimal(4,2)")]
    public decimal PromedioHorasSueno { get; set; }

    /// <summary>
    /// Saludable, Moderado, Alto Riesgo
    /// </summary>
    [StringLength(30)]
    public string NivelRiesgo { get; set; } = "Saludable";

    [StringLength(300)]
    public string ObservacionesDocente { get; set; } = string.Empty;

    public bool GeneradoAutomaticamente { get; set; } = true;
}
