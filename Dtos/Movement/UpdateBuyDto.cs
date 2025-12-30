
namespace MiniMazErpBack;

public record class UpdateBuyDto : UpdateMovementDto
{
    public decimal UnitPrice {get; init;}
}
