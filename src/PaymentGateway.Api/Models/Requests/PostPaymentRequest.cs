using PaymentGateway.Domain;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest(
    string cardNumber,
    int expiryMonth,
    int expiryYear,
    string cvv,
    string currency,
    int amount)
{
    public string CardNumber { get; } = cardNumber;
    public int ExpiryMonth { get; } = expiryMonth;
    public int ExpiryYear { get; } = expiryYear;
    public string Cvv { get; } = cvv;
    public string Currency { get; } = currency;
    public int Amount { get; } = amount;


    public CardDetails ToCardDetails() => new (CardNumber, ExpiryMonth, ExpiryYear, Cvv);
    public Money ToMoney() => new (Amount, new Currency(Currency));
}