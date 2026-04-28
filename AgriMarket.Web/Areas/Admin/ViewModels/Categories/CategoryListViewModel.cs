namespace AgriMarket.Web.Areas.Admin.ViewModels;

public class CategoryListViewModel
{
    public IEnumerable<CategoryListItemViewModel> Categories { get; set; } = [];
    public int TotalCount { get; set; }
}
