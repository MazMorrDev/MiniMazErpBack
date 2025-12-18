using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

public class Expense
{
    [Key, Column("movement_id")]
    public int? MovementId { get; set; }

    [Column("expense_type")]
    public ExpenseType ExpenseType { get; set; }

    [Column("total_price")]
    public double TotalPrice { get; set; }

    // Navigation Properties
    [ForeignKey("MovementId"), Required]
    public required virtual Movement Movement { get; set; } 

}
