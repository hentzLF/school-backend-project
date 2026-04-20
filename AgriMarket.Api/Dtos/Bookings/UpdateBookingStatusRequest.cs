using System.ComponentModel.DataAnnotations;
using AgriMarket.Domain.Enums;

namespace AgriMarket.Api.Dtos.Bookings;

public class UpdateBookingStatusRequest
{
    [Required]
    [EnumDataType(typeof(BookingStatus))]
    public BookingStatus Status { get; set; }
}
