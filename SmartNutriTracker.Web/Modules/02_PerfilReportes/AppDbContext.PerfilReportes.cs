using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Modules.PerfilReportes.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Extensión parcial de AppDbContext perteneciente exclusivamente al Equipo 2 (Módulo 02: Perfil y Reportes).
/// Tablas para HU-08 a HU-12: PerfilesEstudiantes, HistorialesAntropometricos, ReportesNutricionalesSemanales.
/// </summary>
public partial class AppDbContext
{
    public DbSet<PerfilEstudiante> PerfilesEstudiantes => Set<PerfilEstudiante>();
    public DbSet<HistorialAntropometrico> HistorialesAntropometricos => Set<HistorialAntropometrico>();
    public DbSet<ReporteNutricionalSemanal> ReportesNutricionalesSemanales => Set<ReporteNutricionalSemanal>();
}
