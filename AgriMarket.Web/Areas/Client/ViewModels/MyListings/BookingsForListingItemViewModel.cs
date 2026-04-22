using AgriMarket.Domain.Enums;

namespace AgriMarket.Web.Areas.Client.ViewModels.MyListings
{
    public class BookingsForListingItemViewModel
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; } = default!;
        public BookingStatus Status { get; set; }
        public double AreaInHectares { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}