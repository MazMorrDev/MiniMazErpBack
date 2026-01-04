using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

[Table("Buy")]
public class Buy
{
    [Key, Column("movement_id")]
    public int MovementId { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    // Navigation Property
    [ForeignKey("MovementId")]
    public required virtual Movement Movement { get; set; }
}
