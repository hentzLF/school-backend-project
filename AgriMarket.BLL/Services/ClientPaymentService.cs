using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Payments;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public sealed class ClientPaymentService(
    IBookingRepository bookingRepo,
    IRepository<Payment> paymentRepo,
    IUnitOfWork uow,
    IQueryMaterializer mat) : IClientPaymentService
{
    private const decimal PlatformFeeRate = 0.05m;

    public async Task<PaymentReceiptDto> PayAsync(Guid callerProfileId, PayRequest request)
    {
        var booking = await bookingRepo.GetForUpdateAsync(request.BookingId);
        if (booking is null)
            throw new KeyNotFoundException($"Booking {request.BookingId} not found.");

        if (booking.ClientProfileId != callerProfileId)
            throw new UnauthorizedAccessException("You are not the client of this booking.");

        if (booking.Status != BookingStatus.AwaitingPayment)
            throw new BusinessRuleException("Booking is not in a payable state.");

        if (!Enum.IsDefined(request.Method))
            throw new BusinessRuleException("Invalid payment method.");

        var payment = CreatePaymentEntity(booking, request.Method);
        paymentRepo.Add(payment);
        booking.Status = BookingStatus.Confirmed;

        await uow.SaveChangesAsync();

        return ToReceiptDto(payment);
    }

    public async Task<List<PaymentHistoryItemDto>> GetHistoryAsync(Guid callerProfileId)
    {
        var query = paymentRepo.Query()
            .Where(p => p.Booking!.ClientProfileId == callerProfileId
                     || p.Booking!.ServiceListing!.UserProfileId == callerProfileId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentHistoryItemDto(
                p.Id,
                p.BookingId,
                p.Booking!.ServiceListing!.Title,
                p.Amount,
                p.PlatformFee,
                p.Method.ToString(),
                p.Status.ToString(),
                p.CreatedAt,
                p.ReleasedAt));

        return await mat.ToListAsync(query);
    }

    private static Payment CreatePaymentEntity(Booking booking, PaymentMethod method)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Held,
            Amount = booking.TotalPrice,
            PlatformFee = booking.TotalPrice * PlatformFeeRate,
            CreatedAt = DateTime.UtcNow,
            BookingId = booking.Id,
            Method = method
        };
    }

    private static PaymentReceiptDto ToReceiptDto(Payment payment)
    {
        return new PaymentReceiptDto(
            payment.Id,
            payment.BookingId,
            payment.Amount,
            payment.PlatformFee,
            payment.Amount,
            payment.Method.ToString(),
            payment.Status.ToString(),
            payment.CreatedAt);
    }
}
