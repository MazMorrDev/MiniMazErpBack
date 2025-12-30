using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMazErpBack;

[Table("Client")]
public class Client
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, Column("name")]
    public required string Name { get; set; }

    [Required, Column("password")]
    public required string Password { get; set; }
}
