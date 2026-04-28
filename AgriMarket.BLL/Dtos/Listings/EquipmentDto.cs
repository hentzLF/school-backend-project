namespace AgriMarket.BLL.Dtos.Listings;

public sealed class EquipmentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Model { get; init; }
    public int? ManufactureYear { get; init; }
    public string? Description { get; init; }
}
