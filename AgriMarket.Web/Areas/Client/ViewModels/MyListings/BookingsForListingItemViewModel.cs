using System;

namespace AgriMarket.Web.Areas.Client.ViewModels.MyListings
{
    public class BookingsForListingItemViewModel
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public double AreaInHectares { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}