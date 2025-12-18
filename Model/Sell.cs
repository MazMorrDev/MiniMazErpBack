using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MiniMazErpBack;

public class Sell
{
    [Key, Column("movement_id")]
    public int MovementId { get; set; }

    [Column("sale_price")]
    public decimal SalePrice { get; set; }

    [Column("discount_percentage", TypeName = "decimal(5,2)")]
    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100%")]
    public decimal DiscountPercentage { get; set; }

    // Navigation Property
    [ForeignKey("MovementId")]
    public required virtual Movement Movement { get; set; }
}
