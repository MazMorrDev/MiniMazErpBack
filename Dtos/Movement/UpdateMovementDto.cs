using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class UpdateMovementDto
{
    [Required]
    public int InventoryId { get; set; }

    [MaxLength(225)]
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset MovementDate { get; set; }
}
