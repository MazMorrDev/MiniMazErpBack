using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

[Table("Movement")]
public class Movement
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, Column("warehouse_id")]
    public int WarehouseId { get; set; }

    [Required, Column("product_id")]
    public int ProductId { get; set; }

    [Column("description"), MaxLength(225)]
    public string? Description { get; set; }

    [Required, Column("quantity")]
    public int Quantity { get; set; }

    [Required, Column("movement_date")]
    public DateTimeOffset MovementDate { get; set; }

    [ForeignKey("WarehouseId")]
    public virtual Warehouse Warehouse { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;

}

