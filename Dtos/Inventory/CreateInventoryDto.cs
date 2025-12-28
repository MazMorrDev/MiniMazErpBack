using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateInventoryDto
{
    [Required]
    public required int WarehouseId { get; init; }
    [Required]
    public required int ProductId { get; init; }
    [Required]
    public required int Stock { get; init; }
    public int? AlertStock { get; init; }
    public int? WarningStock { get; init; }
}
