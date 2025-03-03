using PaymentGateway.Api.Models.Enums;

namespace PaymentGateway.Api.Tests;

public record PaymentResponse(Guid Id, PaymentStatus Status, string CardNumberLastFour, int ExpiryMonth, int ExpiryYear, string Currency, int Amount);