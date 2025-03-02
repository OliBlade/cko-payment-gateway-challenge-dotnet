using System.Text.Json.Serialization;

namespace PaymentGateway.Adapters.AcquiringBankAdapter.Contracts;

public record ProcessPaymentRequest(
    [property: JsonPropertyName("card_number")]
    string CardNumber,
    [property: JsonPropertyName("expiry_date")]
    string ExpiryDate,
    string Currency,
    int Amount,
    string Cvv);