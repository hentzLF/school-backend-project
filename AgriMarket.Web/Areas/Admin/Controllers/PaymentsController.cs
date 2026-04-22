using AgriMarket.BLL.Services;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<IActionResult> Index(PaymentStatus? status)
    {
        var payments = await _paymentService.GetAllAsync(status);

        var vm = new PaymentListViewModel
        {
            TotalCount = payments.Count(),
            FilterStatus = status,
            Payments = payments.Select(p => new PaymentListItemViewModel
            {
                Id = p.Id,
                BookingId = p.BookingId,
                Amount = p.Amount,
                PlatformFee = p.PlatformFee,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                ReleasedAt = p.ReleasedAt
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var payment = await _paymentService.GetByIdAsync(id);

        if (payment == null) return NotFound();

        var booking = payment.Booking;
        var listing = booking?.ServiceListing;
        var clientProfile = booking?.ClientProfile;
        var providerProfile = listing?.UserProfile;

        var vm = new PaymentDetailViewModel
        {
            Id = payment.Id,
            Amount = payment.Amount,
            PlatformFee = payment.PlatformFee,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt,
            ReleasedAt = payment.ReleasedAt,
            BookingId = payment.BookingId,
            BookingStatus = booking?.Status ?? default,
            ListingTitle = listing?.Title ?? "Unknown",
            ClientName = clientProfile != null
                ? $"{clientProfile.FirstName} {clientProfile.LastName}"
                : "Unknown",
            ClientProfileId = booking?.ClientProfileId ?? default,
            ProviderName = providerProfile != null
                ? $"{providerProfile.FirstName} {providerProfile.LastName}"
                : "Unknown",
            ProviderProfileId = listing?.UserProfileId ?? default
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(DisputeResolveViewModel vm)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = vm.PaymentId });

        var payment = await _paymentService.GetByIdAsync(vm.PaymentId);
        if (payment == null) return NotFound();

        if (payment.Status != PaymentStatus.Disputed)
        {
            TempData["Error"] = "Only disputed payments can be resolved";
            return RedirectToAction(nameof(Details), new { id = vm.PaymentId });
        }

        if (vm.Resolution != "Release" && vm.Resolution != "Refund")
        {
            TempData["Error"] = "Invalid resolution option";
            return RedirectToAction(nameof(Details), new { id = vm.PaymentId });
        }

        await _paymentService.ResolveDisputeAsync(vm.PaymentId, vm.Resolution);

        return RedirectToAction(nameof(Details), new { id = vm.PaymentId });
    }
}
