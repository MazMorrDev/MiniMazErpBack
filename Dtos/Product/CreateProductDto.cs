using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateProductDto
{
    [Required, MaxLength(40)]
    public required string Name { get; init; }

    public decimal? SellPrice { get; init; }
}
