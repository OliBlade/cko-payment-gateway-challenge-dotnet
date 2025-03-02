using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Tests;

public class PaymentTests
{
    [Fact]
    public void Authorise_WithValidAuthorizationCode_SetsStatusToAuthorized()
    {
        // Arrange
        Payment payment = new(new CardDetails("1234567890123456", 1, 2023, "123"), new Money(100, new Currency("USD")));

        // Act
        payment.Authorize(Guid.NewGuid());

        // Assert
        payment.Status.Should().Be(PaymentStatus.Authorized);
    }

    [Fact]
    public void Decline_WithDeclinedPayment_SetsStatusToDeclined()
    {
        // Arrange
        Payment payment = new(new CardDetails("1234567890123456", 1, 2023, "123"), new Money(100, new Currency("USD")));

        // Act
        payment.Decline();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Declined);
    }

    [Fact]
    public void Reject_WithRejectedPayment_SetsStatusToRejected()
    {
        // Arrange
        Payment payment = new(new CardDetails("1234567890123456", 1, 2023, "123"), new Money(100, new Currency("USD")));

        // Act
        payment.Reject();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Rejected);
    }
}