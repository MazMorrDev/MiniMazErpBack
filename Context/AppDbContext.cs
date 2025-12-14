using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    DbSet<Buy> Buys { get; set; }
    DbSet<Inventory> Inventories { get; set; }
    DbSet<Movement> Movements { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Sell> Sells { get; set; }
    DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
