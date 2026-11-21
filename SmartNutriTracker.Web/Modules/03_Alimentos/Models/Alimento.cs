using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartNutriTracker.Web.Core.Models;

namespace SmartNutriTracker.Web.Modules.Alimentos.Models;

/// <summary>
/// Catálogo de alimentos escolares con valor nutricional y semáforo (HU-14, HU-15).
/// </summary>
[Table("Alimentos")]
public class Alimento
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Frutas, Verduras, Carnes, Lácteos, Cereales, Saludables, Snacks, Bebidas
    /// </summary>
    [Required]
    [StringLength(60)]
    public string Categoria { get; set; } = "General";

    [StringLength(30)]
    public string Icono { get; set; } = "🥗";

    [StringLength(80)]
    public string PorcionReferencia { get; set; } = "100 g";

    [Column(TypeName = "decimal(6,2)")]
    public decimal GramosPorcion { get; set; } = 100m;

    [Column(TypeName = "decimal(6,2)")]
    public decimal Calorias { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProteinasGramos { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal CarbohidratosGramos { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal GrasasGramos { get; set; }

    /// <summary>
    /// Clasificación de semáforo nutricional: "Verde" (Saludable), "Amarillo" (Moderado), "Rojo" (Restringido)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string SemaforoNutricional { get; set; } = "Verde";

    public bool EsFavorito { get; set; } = false;

    public bool EsAptoQuioscoEscolar { get; set; } = true;

    [StringLength(250)]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Menú diario planificado por el Nutricionista para el comedor escolar o recomendación (HU-16, HU-17).
/// </summary>
[Table("MenusDiarios")]
public class MenuDiario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string NombreMenu { get; set; } = string.Empty;

    public DateTime FechaMenu { get; set; } = DateTime.Today;

    /// <summary>
    /// Desayuno, Almuerzo, Merienda, Cena
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TipoComida { get; set; } = "Desayuno";

    [StringLength(50)]
    public string EstadoVariante { get; set; } = "Estándar"; // "Estándar", "Usado esta semana", "Nuevo"

    [Column(TypeName = "decimal(7,2)")]
    public decimal TotalCalorias { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal TotalProteinas { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal TotalCarbohidratos { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal TotalGrasas { get; set; }

    [StringLength(20)]
    public string Semaforo { get; set; } = "Verde"; // Verde, Amarillo, Rojo

    public bool EsSaludable { get; set; } = true;

    [StringLength(300)]
    public string DetalleAlimentosResumen { get; set; } = string.Empty;

    public int? EstudianteId { get; set; }

    [ForeignKey(nameof(EstudianteId))]
    public Usuario? Estudiante { get; set; }

    public int? NutricionistaId { get; set; }

    [ForeignKey(nameof(NutricionistaId))]
    public Usuario? Nutricionista { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<DetalleMenuAlimento> Items { get; set; } = new List<DetalleMenuAlimento>();
}

/// <summary>
/// Relación de alimentos asignados a cada menú diario con sus porciones (HU-16, HU-17).
/// </summary>
[Table("DetallesMenuAlimentos")]
public class DetalleMenuAlimento
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int MenuDiarioId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [ForeignKey(nameof(MenuDiarioId))]
    public MenuDiario? MenuDiario { get; set; }

    [Required]
    public int AlimentoId { get; set; }

    [ForeignKey(nameof(AlimentoId))]
    public Alimento? Alimento { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal Cantidad { get; set; } = 100m;

    [StringLength(20)]
    public string Unidad { get; set; } = "g"; // "g" o "unid"

    [Column(TypeName = "decimal(6,2)")]
    public decimal SubtotalCalorias { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal SubtotalProteinas { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal SubtotalCarbohidratos { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal SubtotalGrasas { get; set; }
}
