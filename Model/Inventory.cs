using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

[Table("Inventory")]
public class Inventory
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, Column("client_id")]
    public int ClientId { get; set; }

    [Required, Column("product_id")]
    public int ProductId { get; set; }

    [Required, Column("stock")]
    public int Stock { get; set; }

    // Navigation Properties
    [ForeignKey("ClientId")]
    public virtual Client Client { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Movement> Movements { get; set; } = new HashSet<Movement>();
}