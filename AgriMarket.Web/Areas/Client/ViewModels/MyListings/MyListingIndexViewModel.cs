using System;
using System.Collections.Generic;

namespace AgriMarket.Web.Areas.Client.ViewModels.MyListings
{
    public class MyListingIndexViewModel
    {
        public IEnumerable<MyListingIndexItemViewModel> Listings { get; set; } = new List<MyListingIndexItemViewModel>();
    }
}