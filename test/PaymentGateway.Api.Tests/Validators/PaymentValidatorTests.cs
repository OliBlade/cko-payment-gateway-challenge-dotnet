using FluentValidation.Results;

using PaymentGateway.Api.Providers;
using PaymentGateway.Api.Validators;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Tests.Validators;

public class PaymentValidatorTests
{
    private readonly PaymentValidator _sut;
    private readonly DateTime _utcNow = DateTime.UtcNow;

    public PaymentValidatorTests()
    {
        Mock<IDateTimeProvider?> dateTimeProviderMock = new();
        dateTimeProviderMock.Setup(dp => dp.UtcNow).Returns(_utcNow);
        
        _sut = new PaymentValidator(dateTimeProviderMock.Object);
    }

    [Fact]
    public void Validate_WhenCardDetailsIsNull_ShouldFail()
    {
        // Arrange
        Money money = new(1000, new Currency("USD"));
        Payment payment = new(null, money);

        // Act
        ValidationResult? result = _sut.Validate(payment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Payment.CardDetails));
    }

    [Fact]
    public void Validate_WhenAmountIsNull_ShouldFail()
    {
        // Arrange
        CardDetails cardDetails = new("1234567890123456", _utcNow.Month, _utcNow.Year + 1, "123");
        Payment payment = new(cardDetails, null);

        // Act
        ValidationResult? result = _sut.Validate(payment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Payment.Amount) );
    }

    [Fact]
    public void Validate_GivenValidPayment_ShouldPass()
    {
        // Arrange
        CardDetails cardDetails = new("1234567890123456", _utcNow.Month, _utcNow.Year + 1, "123");
        Money money = new(1000, new Currency("USD"));
        Payment payment = new(cardDetails, money);

        // Act
        ValidationResult? result = _sut.Validate(payment);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}