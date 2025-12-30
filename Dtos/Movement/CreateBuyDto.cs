namespace MiniMazErpBack;

public record class CreateBuyDto : CreateMovementDto
{
    public decimal UnitPrice { get; init; }
}
