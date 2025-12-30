using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateMovementDto
{
    [Required]
    public int WarehouseId { get; init; }

    [Required]
    public int ProductId { get; init; }

    [ MaxLength(225)]
    public string? Description { get; init; }

    [Required]
    public int Quantity { get; init; }

    [Required]
    public DateTimeOffset MovementDate { get; init; }
}
