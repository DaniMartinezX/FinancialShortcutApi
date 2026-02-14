using Microsoft.EntityFrameworkCore;
using FinancialShortcutApi.Models;

namespace FinancialShortcutApi.Data;

public class FinancialDbContext : DbContext
{
    public FinancialDbContext(DbContextOptions<FinancialDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaccion> Transacciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaccion>(entity =>
        {
            // Configurar precisión para valores monetarios (18 dígitos totales, 2 decimales)
            entity.Property(t => t.Cantidad)
                .HasPrecision(18, 2);
        });
    }
}
