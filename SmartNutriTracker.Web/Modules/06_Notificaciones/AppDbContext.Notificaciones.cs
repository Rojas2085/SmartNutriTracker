using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Modules.Notificaciones.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Extensión parcial de AppDbContext perteneciente exclusivamente al Equipo 6 (Módulo 06: Notificaciones y Testing).
/// Tablas para HU-27 a HU-32: AlertasNutricionales, NotificacionesInternas, LogsGlobalesSistema.
/// </summary>
public partial class AppDbContext
{
    public DbSet<AlertaNutricional> AlertasNutricionales => Set<AlertaNutricional>();
    public DbSet<NotificacionInterna> NotificacionesInternas => Set<NotificacionInterna>();
    public DbSet<LogGlobalSistema> LogsGlobalesSistema => Set<LogGlobalSistema>();
}
