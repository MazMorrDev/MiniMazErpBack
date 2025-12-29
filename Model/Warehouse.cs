using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

[Table("Warehouse")]
public class Warehouse
{

    [Key, Column("client_id")]
    public int ClientId { get; set; }

    [Required, Column("name"), MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [Column("description"), MaxLength(255)]
    public string? Description { get; set; }

    // Navigation Property
    [ForeignKey("ClientId")]
    public required virtual Client Client { get; set; }
}
