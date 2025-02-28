using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public class GetPaymentResponse(PostPaymentResponse postPaymentResponse)
{
    public Guid Id { get; init; } = postPaymentResponse.Id;
    public PaymentStatus Status { get; init; } = postPaymentResponse.Status;
    public int CardNumberLastFour { get; init; } = postPaymentResponse.CardNumberLastFour;
    public int ExpiryMonth { get; init; } = postPaymentResponse.ExpiryMonth;
    public int ExpiryYear { get; init; } = postPaymentResponse.ExpiryYear;
    public string Currency { get; init; } = postPaymentResponse.Currency;
    public int Amount { get; init; } = postPaymentResponse.Amount;
}