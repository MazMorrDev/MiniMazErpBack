using System.ComponentModel.DataAnnotations;

namespace MiniMazErpBack;

public record class CreateWarehouseDto
{
    public required int ClientId {get; init;}

    [Required, MaxLength(30)]
    public required string Name { get; init; }

    public string? Description { get; init; }
}
