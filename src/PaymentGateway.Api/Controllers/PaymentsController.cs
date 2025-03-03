using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsRepository paymentsRepository, IPaymentProcessor paymentProcessor, ILogger<PaymentsController> logger) : Controller
{
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentResponse>> GetPaymentAsync(Guid id)
    {
        Payment? payment = await paymentsRepository.Get(id);

        if (payment is not null && payment.Status != PaymentStatus.Rejected)
        {
            logger.LogInformation("Payment found for id {Id} with status {status}", id, payment.Status);
            return new OkObjectResult(new PaymentResponse(payment));
        }

        logger.LogInformation("Payment not found for id {Id}", id);
        return NotFound();
    }
    
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest request, CancellationToken cancellationToken)
    {
        Payment payment = await paymentProcessor.ProcessPayment(request.ToCardDetails(), request.ToMoney(), cancellationToken);
        logger.LogInformation("Payment processed with status {status}", payment.Status);
        return new OkObjectResult(new PaymentResponse(payment));
    }
}