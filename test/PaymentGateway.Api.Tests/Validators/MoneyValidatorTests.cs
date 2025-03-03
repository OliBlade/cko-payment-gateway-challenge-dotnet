using FluentValidation.Results;

using PaymentGateway.Api.Validators;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Tests.Validators;

public class MoneyValidatorTests
{
    private readonly MoneyValidator _sut = new();

    [Fact]
    public void Validate_GivenValidMoney_ShouldPass()
    {
        // Arrange
        Currency currency = new("USD");
        Money money = new(1000, currency);

        // Act
        ValidationResult? result = _sut.Validate(money);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenAmountIsZero_ShouldFail()
    {
        // Arrange
        Currency currency = new("USD");
        Money money = new(0, currency);

        // Act
        ValidationResult? result = _sut.Validate(money);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Money.Amount));
    }

    [Fact]
    public void Validate_WhenCurrencyIsNull_ShouldFail()
    {
        // Arrange
        Money money = new(1000, null);

        // Act
        ValidationResult? result = _sut.Validate(money);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Money.Currency));
    }

    [Fact]
    public void Validate_WhenCurrencyIsInvalid_ShouldFail()
    {
        // Arrange
        Currency currency = new("AUD");
        Money money = new(1000, currency);

        // Act
        ValidationResult? result = _sut.Validate(money);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Currency.Code");
    }
}