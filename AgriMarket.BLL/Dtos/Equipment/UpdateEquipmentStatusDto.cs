using System.ComponentModel.DataAnnotations;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Dtos.Equipment;

public sealed class UpdateEquipmentStatusDto
{
    [Required]
    [EnumDataType(typeof(EquipmentStatus))]
    public EquipmentStatus Status { get; init; }
}
