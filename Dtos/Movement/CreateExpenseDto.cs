namespace MiniMazErpBack;

public record class CreateExpenseDto : CreateMovementDto
{
    public ExpenseType ExpenseType { get; init; }
    public decimal TotalPrice { get; init; }
}
