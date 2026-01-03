using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Buy> Buys { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Movement> Movements { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sell> Sells { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PRODUCT - Configuración de producto
        modelBuilder.Entity<Product>(entity =>
        {
            // Precisión decimal para precio de venta: 18 dígitos totales (14 enteros + 4 decimales)
            // Esto evita errores de redondeo en cálculos financieros
            entity.Property(e => e.SellPrice).HasPrecision(18, 4);
        });

        // INVENTORY - Configuración de inventario
        modelBuilder.Entity<Inventory>(entity =>
        {
            // Índice único compuesto: Evita tener múltiples registros para la misma combinación
            // de almacén y producto. Un producto solo puede tener un registro de inventario por almacén
            entity.HasIndex(e => new { e.ClientId, e.ProductId }).IsUnique();

            // Check constraint a nivel de tabla: Garantiza que el stock nunca sea negativo
            // Se ejecuta en la base de datos, previniendo datos inválidos incluso desde SQL directo
            entity.ToTable(t => t.HasCheckConstraint("CK_Inventory_Stock", "stock >= 0"));
        });

        // MOVEMENT - Configuración de movimiento
        modelBuilder.Entity<Movement>(entity =>
        {
            // Valor por defecto para fecha de movimiento: Usa la fecha/hora actual del servidor PostgreSQL
            // Asegura que todos los movimientos tengan fecha incluso si no se especifica
            entity.Property(e => e.MovementDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Check constraint: La cantidad de movimiento debe ser siempre positiva (entrada o salida)
            // Cantidad = 0 no tiene sentido en un movimiento
            entity.ToTable(t => t.HasCheckConstraint("CK_Movement_Quantity", "quantity > 0"));

            // Índice para consultas por fecha: Acelera búsquedas de movimientos en rangos de fechas
            // Ej: "Movimientos del último mes", "Reporte mensual"
            entity.HasIndex(e => e.MovementDate);

            // Índice para consultas por producto: Optimiza búsquedas del historial de un producto específico
            entity.HasIndex(e => e.ProductId);
        });

        // SELL - Configuración de venta
        modelBuilder.Entity<Sell>(entity =>
        {
            // Precisión para porcentaje de descuento: 5 dígitos totales (3 enteros + 2 decimales)
            // Permite descuentos como 12.5%, 25.75%, 100.00%
            entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);

            // Check constraint: El descuento debe estar entre 0% y 100%
            // Previene descuentos negativos o mayores al 100%
            entity.ToTable(t => t.HasCheckConstraint("CK_Sell_Discount",
                "discount_percentage >= 0 AND discount_percentage <= 100"));
        });

        // BUY - Configuración de compra
        modelBuilder.Entity<Buy>(entity =>
        {
            // Misma precisión que Product.SellPrice para consistencia en precios
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
        });

        // EXPENSE - Configuración de gasto
        modelBuilder.Entity<Expense>(entity =>
        {
            // Precisión consistente con otros valores monetarios
            entity.Property(e => e.TotalPrice).HasPrecision(18, 4);

            // Convertir enum a string para almacenamiento en BD
            // HasMaxLength(20) limita el tamaño y mejora performance de índices
            entity.Property(e => e.ExpenseType)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // WAREHOUSE - Configuración de almacén
        modelBuilder.Entity<Client>(entity =>
        {
            // Índice único: Evita nombres de clientes duplicados
            // Mejora integridad de datos y búsquedas por nombre
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }
}
