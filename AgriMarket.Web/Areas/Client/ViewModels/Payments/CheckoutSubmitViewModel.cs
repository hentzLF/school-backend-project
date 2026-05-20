using AgriMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Web.Areas.Client.ViewModels.Payments;

public class CheckoutSubmitViewModel
{
    [Required]
    public Guid BookingId { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }
}
