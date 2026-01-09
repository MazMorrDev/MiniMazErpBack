using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

[Table("Movement")]
public class Movement
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, Column("inventory_id")]
    public int InventoryId { get; set; }

    [Column("description"), MaxLength(225)]
    public string? Description { get; set; }

    [Required, Column("quantity")]
    public int Quantity { get; set; }

    [Required, Column("movement_date")]
    public DateTimeOffset MovementDate { get; set; }

    [ForeignKey("InventoryId")]
    public virtual Inventory Inventory { get; set; } = null!;
}

