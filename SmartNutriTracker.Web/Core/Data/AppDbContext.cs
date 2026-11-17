using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Models;

namespace SmartNutriTracker.Web.Core.Data;

/// <summary>
/// Contexto principal de base de datos definido como clase parcial (partial class).
/// REGLA DE ARQUITECTURA: Ningún equipo modifica este archivo directamente.
/// Cada equipo declara sus propios DbSet&lt;T&gt; en su archivo AppDbContext.[Modulo].cs dentro de su carpeta Modules/0X_...
/// </summary>
public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(120);
            entity.Property(u => u.Rol).IsRequired().HasMaxLength(40);
            entity.Property(u => u.EquipoAsignado).IsRequired().HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
