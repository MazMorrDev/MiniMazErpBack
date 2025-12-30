using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class RegisterClientDto
{
    [Required, MaxLength(20)]
    public required string Name { get; init; }

    [Required]
    public required string Password { get; init; }
}
