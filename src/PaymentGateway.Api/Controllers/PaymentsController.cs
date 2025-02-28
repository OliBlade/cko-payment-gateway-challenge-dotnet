using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(PaymentsRepository paymentsRepository, ILogger<PaymentsController> logger) : Controller
{
    [HttpGet("{id:guid}")]
    public ActionResult<GetPaymentResponse> GetPaymentAsync(Guid id)
    {
        PostPaymentResponse? payment = paymentsRepository.Get(id);

        if (payment is not null && payment.Status != PaymentStatus.Rejected)
        {
            return new OkObjectResult(new GetPaymentResponse(payment));
        }

        logger.LogInformation("Payment not found for id {Id}", id);
        return NotFound();
    }
}