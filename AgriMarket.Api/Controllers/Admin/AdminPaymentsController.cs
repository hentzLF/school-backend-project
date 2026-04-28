using AgriMarket.BLL.Services;
using AgriMarket.Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers.Admin;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/admin/payments")]
[Authorize(Policy = "AdminOnly")]
public class AdminPaymentsController(IPaymentService paymentService) : ApiControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaymentStatus? status)
    {
        var payments = await _paymentService.GetAllAsync(status);
        return Ok(payments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Payment {id} not found.");

        return Ok(payment);
    }

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveDisputeRequest req)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Payment {id} not found.");

        if (payment.Status != PaymentStatus.Disputed)
            return Problem(statusCode: 400, title: "Bad Request", detail: "Only disputed payments can be resolved.");

        await _paymentService.ResolveDisputeAsync(id, req.Resolution);

        var updated = await _paymentService.GetByIdAsync(id);
        return Ok(updated);
    }
}

public sealed record ResolveDisputeRequest(PaymentResolution Resolution);
