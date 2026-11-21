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
    /// Frutas, Verduras, Cereales, Proteínas, Lácteos, Grasas Saludables, Snacks, Bebidas
    /// </summary>
    [Required]
    [StringLength(60)]
    public string Categoria { get; set; } = "General";

    [StringLength(80)]
    public string PorcionReferencia { get; set; } = "1 porción";

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
    /// Desayuno, Almuerzo, Refrigerio Escolar, Cena
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TipoComida { get; set; } = "Almuerzo";

    [Column(TypeName = "decimal(7,2)")]
    public decimal TotalCalorias { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal TotalProteinas { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal TotalCarbohidratos { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal TotalGrasas { get; set; }

    public bool EsSaludable { get; set; } = true;

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

    [ForeignKey(nameof(MenuDiarioId))]
    public MenuDiario? MenuDiario { get; set; }

    [Required]
    public int AlimentoId { get; set; }

    [ForeignKey(nameof(AlimentoId))]
    public Alimento? Alimento { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal CantidadPorciones { get; set; } = 1.0m;

    [Column(TypeName = "decimal(6,2)")]
    public decimal SubtotalCalorias { get; set; }
}
