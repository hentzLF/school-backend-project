namespace AgriMarket.Web.Areas.Admin.ViewModels;

public class UserListViewModel
{
    public IEnumerable<UserListItemViewModel> Users { get; set; } = [];
    public int TotalCount { get; set; }
}
