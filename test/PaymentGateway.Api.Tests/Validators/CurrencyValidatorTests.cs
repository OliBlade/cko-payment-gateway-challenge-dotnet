using FluentValidation.Results;
using PaymentGateway.Api.Validators;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Tests.Validators;

public class CurrencyValidatorTests
{
    private readonly CurrencyValidator _sut = new();

    [Fact]
    public void Validate_GivenValidCurrency_ShouldPass()
    {
        // Arrange
        Currency currency = new("USD");

        // Act
        ValidationResult? result = _sut.Validate(currency);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenCurrencyCodeIsEmpty_ShouldFail()
    {
        // Arrange
        Currency currency = new Currency("");

        // Act
        ValidationResult result = _sut.Validate(currency);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(Currency.Code));
    }

    [Fact]
    public void Validate_WhenCurrencyCodeIsNotThreeCharacters_ShouldFail()
    {
        // Arrange
        Currency currency = new Currency("US");

        // Act
        ValidationResult result = _sut.Validate(currency);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(Currency.Code));
    }

    [Fact]
    public void Validate_WhenCurrencyCodeIsNotInValidList_ShouldFail()
    {
        // Arrange
        Currency currency = new Currency("AUD");

        // Act
        ValidationResult result = _sut.Validate(currency);

        // Assert
        result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(Currency.Code));
    }
}