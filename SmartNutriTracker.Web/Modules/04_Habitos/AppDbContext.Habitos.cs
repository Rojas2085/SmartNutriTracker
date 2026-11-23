using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Modules.Habitos.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Extensión parcial de AppDbContext exclusiva para el Equipo 4 (Módulo 04: Registro de Hábitos).
/// Las tablas se crean automáticamente con EnsureCreated al arrancar (ver DbInitializer).
/// </summary>
public partial class AppDbContext
{
    // Entrega Módulo 04 (HU-19 a HU-22)
    public DbSet<RegistroHabito> RegistrosHabito => Set<RegistroHabito>();
    public DbSet<RegistroComida> RegistrosComida => Set<RegistroComida>();

    // Modelos complementarios de seguimiento diario
    public DbSet<RegistroHabitoDiario> RegistrosHabitosDiarios => Set<RegistroHabitoDiario>();
    public DbSet<ConsumoAlimentoReal> ConsumosAlimentosReales => Set<ConsumoAlimentoReal>();
}
