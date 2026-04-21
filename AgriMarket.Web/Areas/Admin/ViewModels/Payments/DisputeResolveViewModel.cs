using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Web.Areas.Admin.ViewModels;

public class DisputeResolveViewModel
{
    public Guid PaymentId { get; set; }

    [Required]
    public string Resolution { get; set; } = default!; // "Release" or "Refund"
}
