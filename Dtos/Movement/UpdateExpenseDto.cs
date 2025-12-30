namespace MiniMazErpBack;

public record class UpdateExpenseDto : UpdateMovementDto
{
    public ExpenseType ExpenseType { get; init; }
    public decimal TotalPrice { get; init; }
}
