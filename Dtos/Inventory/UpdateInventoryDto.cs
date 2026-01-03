using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class UpdateInventoryDto
{
    [Required]
    public required int ClientId { get; init; }
    [Required]
    public required int ProductId { get; init; }
    [Required]
    public required int Stock { get; init; }
    public int? AlertStock { get; init; }
    public int? WarningStock { get; init; }
}
