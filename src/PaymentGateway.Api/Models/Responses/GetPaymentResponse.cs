using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Models.Responses;

public class GetPaymentResponse
{
    public Guid Id { get; }
    public PaymentStatus Status { get; }
    public string CardNumberLastFour { get; }
    public int ExpiryMonth { get; }
    public int ExpiryYear { get; }
    public string Currency { get; }
    public int Amount { get; }

    public GetPaymentResponse(Payment payment)
    {
        Id = payment.Id;
        Status = payment.Status.ToModelPaymentStatus();
        CardNumberLastFour = payment.CardDetails.CardNumberLastFour;
        ExpiryMonth = payment.CardDetails.ExpiryMonth;
        ExpiryYear = payment.CardDetails.ExpiryYear;
        Currency = payment.Amount.Currency.Code;
        Amount = payment.Amount.Amount;
    }
}