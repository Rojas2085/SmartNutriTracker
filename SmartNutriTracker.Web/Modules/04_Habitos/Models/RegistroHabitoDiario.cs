using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartNutriTracker.Web.Core.Models;
using SmartNutriTracker.Web.Modules.Alimentos.Models;

namespace SmartNutriTracker.Web.Modules.Habitos.Models;

/// <summary>
/// Registro diario integral de hábitos del estudiante: agua, sueño y actividad física (HU-20, HU-21, HU-22).
/// </summary>
[Table("RegistrosHabitosDiarios")]
public class RegistroHabitoDiario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int EstudianteId { get; set; }

    [ForeignKey(nameof(EstudianteId))]
    public Usuario? Estudiante { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Today;

    public int VasosAgua { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal MililitrosAgua { get; set; } = 0m;

    [Column(TypeName = "decimal(4,2)")]
    public decimal HorasSueno { get; set; } = 8.0m;

    public int MinutosActividadFisica { get; set; } = 0;

    /// <summary>
    /// Caminata, Deporte Escolar, Gimnasio, Bicicleta, Sedentario
    /// </summary>
    [StringLength(80)]
    public string TipoActividadFisica { get; set; } = "Caminata";

    /// <summary>
    /// Excelente, Con energía, Normal, Cansado, Con sueño
    /// </summary>
    [StringLength(50)]
    public string EstadoAnimo { get; set; } = "Normal";

    [StringLength(300)]
    public string Observaciones { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime? FechaUltimaModificacion { get; set; }

    public ICollection<ConsumoAlimentoReal> ComidasConsumidas { get; set; } = new List<ConsumoAlimentoReal>();
}

/// <summary>
/// Registro del consumo real de alimentos efectuado por el estudiante (HU-19, HU-21, HU-22).
/// </summary>
[Table("ConsumosAlimentosReales")]
public class ConsumoAlimentoReal
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int RegistroHabitoDiarioId { get; set; }

    [ForeignKey(nameof(RegistroHabitoDiarioId))]
    public RegistroHabitoDiario? RegistroHabitoDiario { get; set; }

    public int? AlimentoId { get; set; }

    [ForeignKey(nameof(AlimentoId))]
    public Alimento? Alimento { get; set; }

    [Required]
    [StringLength(150)]
    public string DescripcionAlimento { get; set; } = string.Empty;

    /// <summary>
    /// Desayuno, Almuerzo, Refrigerio Mañana, Refrigerio Tarde, Cena
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TipoComida { get; set; } = "Almuerzo";

    [Column(TypeName = "decimal(5,2)")]
    public decimal PorcionesConsumidas { get; set; } = 1.0m;

    [Column(TypeName = "decimal(6,2)")]
    public decimal CaloriasAproximadas { get; set; }

    public TimeSpan? HoraConsumo { get; set; }
}
