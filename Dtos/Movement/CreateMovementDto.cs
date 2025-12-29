using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateMovementDto
{
    [Required]
    public int WarehouseId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ MaxLength(225)]
    public string? Description { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public DateTimeOffset MovementDate { get; set; }
}
