using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateInventoryDto
{
    [Required]
    public required int ClientId { get; init; }
    [Required]
    public required int ProductId { get; init; }
    [Required]
    public required int Stock { get; init; }
}
