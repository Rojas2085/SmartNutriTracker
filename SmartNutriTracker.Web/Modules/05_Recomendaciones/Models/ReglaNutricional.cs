using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartNutriTracker.Web.Core.Models;
using SmartNutriTracker.Web.Modules.Alimentos.Models;

namespace SmartNutriTracker.Web.Modules.Recomendaciones.Models;

/// <summary>
/// Reglas base del modelo nutricional según IMC, edad y objetivos (HU-24, HU-25, HU-26).
/// </summary>
[Table("ReglasNutricionales")]
public class ReglaNutricional
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreRegla { get; set; } = string.Empty;

    /// <summary>
    /// Bajo peso, Normal, Sobrepeso, Obesidad
    /// </summary>
    [Required]
    [StringLength(40)]
    public string ClasificacionImc { get; set; } = "Normal";

    /// <summary>
    /// Mantener peso, Bajar de peso, Ganar peso
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Objetivo { get; set; } = "Mantener peso";

    public int RangoEdadMin { get; set; } = 6;

    public int RangoEdadMax { get; set; } = 18;

    [Column(TypeName = "decimal(6,2)")]
    public decimal CaloriasMin { get; set; } = 1800m;

    [Column(TypeName = "decimal(6,2)")]
    public decimal CaloriasMax { get; set; } = 2200m;

    [Column(TypeName = "decimal(4,2)")]
    public decimal PorcentajeProteinas { get; set; } = 15.0m;

    [Column(TypeName = "decimal(4,2)")]
    public decimal PorcentajeCarbohidratos { get; set; } = 55.0m;

    [Column(TypeName = "decimal(4,2)")]
    public decimal PorcentajeGrasas { get; set; } = 30.0m;

    public int MetaVasosAgua { get; set; } = 8;

    public int MetaMinutosActividad { get; set; } = 60;

    [StringLength(300)]
    public string GuiaNutricional { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;
}

/// <summary>
/// Recomendaciones personalizadas generadas automáticamente para los estudiantes (HU-23, HU-26).
/// </summary>
[Table("RecomendacionesInteligentes")]
public class RecomendacionInteligente
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int EstudianteId { get; set; }

    [ForeignKey(nameof(EstudianteId))]
    public Usuario? Estudiante { get; set; }

    public int? ReglaNutricionalId { get; set; }

    [ForeignKey(nameof(ReglaNutricionalId))]
    public ReglaNutricional? ReglaNutricional { get; set; }

    public int? MenuSugeridoId { get; set; }

    [ForeignKey(nameof(MenuSugeridoId))]
    public MenuDiario? MenuSugerido { get; set; }

    [Required]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string MensajePersonalizado { get; set; } = string.Empty;

    [Column(TypeName = "decimal(6,2)")]
    public decimal CaloriasSugeridas { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProteinasGramos { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal CarbohidratosGramos { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal GrasasGramos { get; set; }

    public int VasosAguaSugeridos { get; set; } = 8;

    public int MinutosEjercicioSugeridos { get; set; } = 60;

    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

    public bool Leida { get; set; } = false;
}
