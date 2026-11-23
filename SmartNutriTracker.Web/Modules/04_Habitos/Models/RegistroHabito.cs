using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartNutriTracker.Web.Modules.Habitos.Models;

/// <summary>
/// Registro diario de hábitos del estudiante: agua, horas de sueño, actividad física y nota (HU-20, HU-21, HU-22).
/// Solo puede existir un registro por estudiante y fecha; la regla se valida en HabitosLogica.
/// </summary>
[Table("RegistrosHabito")]
public class RegistroHabito
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>Id del usuario (tabla Usuarios del núcleo) dueño del registro.</summary>
    [Required]
    public int UsuarioId { get; set; }

    /// <summary>Fecha del registro (solo la parte de fecha; la hora siempre es 00:00).</summary>
    [Required]
    public DateTime Fecha { get; set; }

    /// <summary>Agua consumida en mililitros (0 a 10000).</summary>
    public int AguaMl { get; set; }

    /// <summary>Horas de sueño (0 a 24).</summary>
    [Column(TypeName = "decimal(4,1)")]
    public decimal? HorasSueno { get; set; }

    /// <summary>Minutos de actividad física (0 a 1440).</summary>
    public int ActividadMin { get; set; }

    [StringLength(250)]
    public string? Nota { get; set; }

    public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
}
