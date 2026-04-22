using AgriMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Bookings;

public sealed class UpdateBookingStatusRequest
{
    [Required]
    [EnumDataType(typeof(BookingStatus))]
    public BookingStatus Status { get; init; }
}
