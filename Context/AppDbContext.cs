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

        /*
        CONFIGURACIÓN PARA PRODUCT:
        - Precisión decimal para SellPrice: Define el formato de almacenamiento para precios
          (18 dígitos totales, 4 decimales) para evitar errores de redondeo
        */
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.SellPrice).HasPrecision(18, 4);
        });

        /*
        CONFIGURACIÓN PARA INVENTORY:
        - Relación con Warehouse y Product: Cada registro de inventario vincula un producto 
          con un warehouse específico
        - Índice único compuesto: Evita registros duplicados de inventario para la misma
          combinación warehouse-producto
        - Check constraint: Garantiza que el stock nunca sea negativo
        - Eliminación en cascada: Si se elimina un producto o warehouse, se eliminan sus registros de inventario
        */
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasOne(i => i.Warehouse)
                .WithMany()
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.WarehouseId, e.ProductId }).IsUnique();

            entity.ToTable(t => t.HasCheckConstraint("CK_Inventory_Stock", "[Stock] >= 0"));
        });

        /*
        CONFIGURACIÓN PARA MOVEMENT:
        - Relaciones con Client, Warehouse y Product: Registra movimientos vinculados a estas entidades
        - Precisión decimal para UnitaryCost: Formato consistente para costos unitarios
        - Valor por defecto en MovementDate: Fecha UTC actual al crear un movimiento
        - Check constraint: Asegura que la cantidad de movimiento sea siempre positiva
        - Eliminación en cascada: Mantiene la integridad referencial al eliminar entidades relacionadas
        */
        modelBuilder.Entity<Movement>(entity =>
        {
            entity.HasOne(m => m.Warehouse)
                .WithMany()
                .HasForeignKey(m => m.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Product)
                .WithMany(p => p.Movements)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.MovementDate).HasDefaultValueSql("GETUTCDATE()");

            entity.ToTable(t => t.HasCheckConstraint("CK_Movement_Quantity", "[Quantity] > 0"));
        });

        /*
        CONFIGURACIÓN DE ENUMS:
        - Conversión a string: Almacena los valores de enum como cadenas legibles en la BD
        - Longitud máxima: Limita el tamaño de almacenamiento para los campos de enum
        - Esto mejora la legibilidad de la base de datos y facilita consultas directas
        */
        modelBuilder.Entity<Expense>()
            .Property(e => e.ExpenseType)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
