namespace AgriMarket.Web.Areas.Client.ViewModels.Payments;

public class PaymentHistoryViewModel
{
    public IEnumerable<PaymentHistoryItemViewModel> Payments { get; set; } = [];
}
