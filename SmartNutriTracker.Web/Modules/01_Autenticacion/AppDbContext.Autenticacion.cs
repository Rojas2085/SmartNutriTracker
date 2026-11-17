using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Modules.Autenticacion.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Extensión parcial de AppDbContext perteneciente exclusivamente al Equipo 1 (Módulo 01: Autenticación y Seguridad).
/// Permite agregar tablas del módulo sin editar Core/Data/AppDbContext.cs.
/// </summary>
public partial class AppDbContext
{
    public DbSet<AuditoriaAcceso> AuditoriasAcceso => Set<AuditoriaAcceso>();
}
