using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class UpdateWarehouseDto
{
    [Required, MaxLength(30)]
    public required string Name { get; init; }

    public string? Description { get; init; }
}
