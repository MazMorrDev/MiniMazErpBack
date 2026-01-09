using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateSellDto : CreateMovementDto
{
    public decimal SalePrice { get; init; }

    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100%")]
    public decimal? DiscountPercentage { get; init; }
}
