using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Modules.Alimentos.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Extensión parcial de AppDbContext perteneciente exclusivamente al Equipo 3 (Módulo 03: Gestión de Alimentos).
/// Tablas para HU-14 a HU-17: Alimentos, MenusDiarios, DetallesMenuAlimentos.
/// </summary>
public partial class AppDbContext
{
    public DbSet<Alimento> Alimentos => Set<Alimento>();
    public DbSet<MenuDiario> MenusDiarios => Set<MenuDiario>();
    public DbSet<DetalleMenuAlimento> DetallesMenuAlimentos => Set<DetalleMenuAlimento>();
}
