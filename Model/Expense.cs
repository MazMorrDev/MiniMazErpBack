using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

public class Expense
{
    [Column("expense_type")]
    public ExpenseType ExpenseType { get; set; }
}
