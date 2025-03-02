using System.Text.Json.Serialization;

namespace PaymentGateway.Adapters.AcquiringBankAdapter.Contracts;

public record ProcessPaymentResponse(bool Authorized, [property: JsonPropertyName("authorization_code")]  Guid AuthorizationCode);