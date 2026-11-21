using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Modules.Recomendaciones.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Extensión parcial de AppDbContext perteneciente exclusivamente al Equipo 5 (Módulo 05: Recomendaciones Inteligentes).
/// Tablas para HU-23 a HU-26: ReglasNutricionales, RecomendacionesInteligentes.
/// </summary>
public partial class AppDbContext
{
    public DbSet<ReglaNutricional> ReglasNutricionales => Set<ReglaNutricional>();
    public DbSet<RecomendacionInteligente> RecomendacionesInteligentes => Set<RecomendacionInteligente>();
}
