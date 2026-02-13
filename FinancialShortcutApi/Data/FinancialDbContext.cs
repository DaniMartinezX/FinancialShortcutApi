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
}
