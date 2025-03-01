using System.Text.Json.Serialization;

namespace PaymentGateway.Api.AcquiringBankClient.Contracts;

public record ProcessPaymentResponse(bool Authorized, [property: JsonPropertyName("authorization_code")]  Guid AuthorizationCode);