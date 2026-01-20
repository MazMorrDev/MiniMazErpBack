using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class UpdateInventoryDto
{
    [Required]
    public required int UserId { get; init; }
    [Required]
    public required int ProductId { get; init; }
    [Required]
    public required int Stock { get; init; }
}
