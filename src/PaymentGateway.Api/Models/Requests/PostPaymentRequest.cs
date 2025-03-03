using System.ComponentModel.DataAnnotations;
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
    [Required(ErrorMessage = "Card number is required.")]
    public string CardNumber { get; } = cardNumber;
    
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
    public int ExpiryMonth { get; } = expiryMonth;
    
    [Range(1, int.MaxValue, ErrorMessage = "Expiry year must be greater than zero.")]
    public int ExpiryYear { get; } = expiryYear;
    
    [Required(ErrorMessage = "Cvv is required.")]
    public string Cvv { get; } = cvv;
    
    [Required(ErrorMessage = "Currency is required.")]
    public string Currency { get; } = currency;
    
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public int Amount { get; } = amount;


    public CardDetails ToCardDetails() => new (CardNumber, ExpiryMonth, ExpiryYear, Cvv);
    public Money ToMoney() => new (Amount, new Currency(Currency));
}