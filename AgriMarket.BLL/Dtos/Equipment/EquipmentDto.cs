using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Dtos.Equipment;

public sealed class EquipmentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Make { get; init; } = default!;
    public string? Model { get; init; }
    public int? ManufactureYear { get; init; }
    public int? HorsePower { get; init; }
    public EquipmentCondition Condition { get; init; }
    public EquipmentStatus Status { get; init; }
    public string? Description { get; init; }
}
