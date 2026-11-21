using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Modules.Habitos.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Extensión parcial de AppDbContext perteneciente exclusivamente al Equipo 4 (Módulo 04: Registro de Hábitos).
/// Tablas para HU-19 a HU-22: RegistrosHabitosDiarios, ConsumosAlimentosReales.
/// </summary>
public partial class AppDbContext
{
    public DbSet<RegistroHabitoDiario> RegistrosHabitosDiarios => Set<RegistroHabitoDiario>();
    public DbSet<ConsumoAlimentoReal> ConsumosAlimentosReales => Set<ConsumoAlimentoReal>();
}
