using AgriMarket.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgriMarket.Web.Areas.Admin.ViewModels;

public class BookingEditViewModel
{
    public Guid Id { get; set; }
    public BookingStatus Status { get; set; }
    public string ListingTitle { get; set; } = default!;
    public string ClientName { get; set; } = default!;
    public IEnumerable<SelectListItem> Statuses { get; set; } = [];
}
