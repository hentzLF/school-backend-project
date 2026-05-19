using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Equipment;

public sealed class AssignEquipmentRequest
{
    [Required]
    public IReadOnlyList<Guid> EquipmentIds { get; init; } = [];
}
