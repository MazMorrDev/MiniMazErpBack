using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class RegisterUserDto
{
    [Required, MaxLength(20)]
    public required string Name { get; init; }

    [Required]
    public required string Password { get; init; }
}
