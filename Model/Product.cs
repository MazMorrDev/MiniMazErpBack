using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

[Table("Product")]
public class Product
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, Column("name"), MaxLength(40)]
    public string Name { get; set; } = string.Empty;

    // Hay q decidir en q moneda gestionamos los price dentro de la base de datos
    [Column("sell_price")]
    public decimal? SellPrice { get; set; }

    // Navigation Properties
    public virtual ICollection<Movement> Movements { get; set; } = new HashSet<Movement>();
    public virtual ICollection<Inventory> Inventories { get; set; } = new HashSet<Inventory>();
}
