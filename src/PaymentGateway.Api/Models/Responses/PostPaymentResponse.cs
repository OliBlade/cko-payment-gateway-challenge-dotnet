using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    
    public PostPaymentResponse(Payment payment)
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
