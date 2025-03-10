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
    
    /// <summary>
    /// Retrieves a payment by its ID.
    /// </summary>
    /// <returns>If found, returns a payment which has been Authorized or Declined</returns>
    /// <param name="id">The ID of the payment.</param>
    /// <response code="200">Returns a payment with a status of Authorized or Declined</response>
    /// <response code="404">The payment was not found or rejected</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> GetPaymentAsync(Guid id)
    {
        Payment? payment = await paymentsRepository.Get(id);

        if (payment is not null && payment.Status != PaymentStatus.Rejected)
        {
            logger.LogDebug("Payment found for id {Id} with status {status}", id, payment.Status);
            return new OkObjectResult(new PaymentResponse(payment));
        }

        logger.LogInformation("Payment not found for id {Id}", id);
        return NotFound();
    }

    /// <summary>
    /// Processes a payment
    /// </summary>
    /// <returns>Payment info</returns>
    /// <param name="request">The payment request including card and payment details</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Payment Authorized</response>
    /// <response code="400">The payment has been rejected due to invalid properties</response>
    /// <response code="402">The payment has been declined</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResponse))]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired, Type = typeof(PaymentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            List<string> errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            logger.LogInformation("Invalid model state");
            return BadRequest(new { Errors = errorMessages });
        }
        
        Payment payment = await paymentProcessor.ProcessPayment(request.ToCardDetails(), request.ToMoney(), cancellationToken);
        logger.LogDebug("Payment processed with status {status}", payment.Status);

        return payment.Status switch
        {
            PaymentStatus.Rejected => BadRequest(),
            PaymentStatus.Declined => StatusCode(402, new PaymentResponse(payment)), // Payment Required
            _ => new OkObjectResult(new PaymentResponse(payment))
        };
    }
}