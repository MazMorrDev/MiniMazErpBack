using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateInventoryDto
{
    [Required]
    public required int UserId { get; init; }
    [Required]
    public required int ProductId { get; init; }
    [Required]
    public required int Stock { get; init; }
}
