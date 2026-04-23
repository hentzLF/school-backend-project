using AgriMarket.Domain.Entities;
using AgriMarket.Web.Areas.Admin.ViewModels;

namespace AgriMarket.Web.Mappers;

public static class PaymentViewModelMapper
{
    public static PaymentListItemViewModel ToAdminListItem(this Payment p)
    {
        return new PaymentListItemViewModel
        {
            Id = p.Id,
            BookingId = p.BookingId,
            Amount = p.Amount,
            PlatformFee = p.PlatformFee,
            Status = p.Status,
            CreatedAt = p.CreatedAt,
            ReleasedAt = p.ReleasedAt
        };
    }
}
