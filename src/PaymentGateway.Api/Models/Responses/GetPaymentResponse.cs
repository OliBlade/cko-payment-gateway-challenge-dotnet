using PaymentGateway.Api.Enums;
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
        Status = MapPaymentStatus(payment.Status);
        CardNumberLastFour = payment.CardDetails.CardNumberLastFour;
        ExpiryMonth = payment.CardDetails.ExpiryMonth;
        ExpiryYear = payment.CardDetails.ExpiryYear;
        Currency = payment.Amount.Currency.Code;
        Amount = payment.Amount.Amount;
    }

    private PaymentStatus MapPaymentStatus(Domain.Enums.PaymentStatus status) => status switch
    {
        Domain.Enums.PaymentStatus.Authorized => PaymentStatus.Authorized,
        Domain.Enums.PaymentStatus.Declined => PaymentStatus.Declined,
        Domain.Enums.PaymentStatus.Rejected => PaymentStatus.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };
}