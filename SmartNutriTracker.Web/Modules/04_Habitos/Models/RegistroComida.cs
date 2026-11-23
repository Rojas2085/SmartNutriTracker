using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartNutriTracker.Web.Modules.Habitos.Models;

/// <summary>
/// Consumo real de una comida (HU-19). Se guarda como texto libre para no depender del catálogo
/// de alimentos del Equipo 3. Las kcal son opcionales (estimadas por el estudiante).
/// </summary>
[Table("RegistrosComida")]
public class RegistroComida
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>Id del usuario (tabla Usuarios del núcleo) dueño del registro.</summary>
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    /// <summary>Desayuno, Almuerzo, Merienda o Cena.</summary>
    [Required]
    [StringLength(20)]
    public string TipoComida { get; set; } = string.Empty;

    /// <summary>Qué comió realmente, por ejemplo "Arroz integral 1 taza y pollo 150 g".</summary>
    [Required]
    [StringLength(300)]
    public string Descripcion { get; set; } = string.Empty;

    [Column(TypeName = "decimal(7,1)")]
    public decimal? Kcal { get; set; }
}
