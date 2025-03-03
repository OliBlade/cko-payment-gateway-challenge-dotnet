using FluentValidation.Results;
using PaymentGateway.Api.Providers;
using PaymentGateway.Api.Validators;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Tests.Validators;

public class CardDetailsValidatorTests
{
    private readonly CardDetailsValidator _sut;
    private readonly DateTime _utcNow = DateTime.UtcNow;

    public CardDetailsValidatorTests()
    {
        Mock<IDateTimeProvider?> dateTimeProviderMock = new();
        dateTimeProviderMock.Setup(dp => dp.UtcNow).Returns(_utcNow);

        _sut = new CardDetailsValidator(dateTimeProviderMock.Object);
    }

    [Theory]
    [InlineData("12345678901234")] // 14-digit number
    [InlineData("1234567890123456789")] // 19-digit number
    public void Validate_WithValidValues_Succeeds(string cardNumber)
    {
        CardDetails cardDetails = new(cardNumber, 4, _utcNow.Year + 1, "123");

        ValidationResult? result = _sut.Validate(cardDetails);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("123456789012")] // 12-digit number
    [InlineData("12345678901234567890")] // 20-digit number
    public void CardNumber_InvalidLength_Fails(string cardNumber)
    {
        CardDetails cardDetails = new(cardNumber, 4, _utcNow.Year + 1, "123");

        ValidationResult? result = _sut.Validate(cardDetails);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(CardDetails.FullCardNumber));
    }

    [Fact]
    public void ExpiryDate_InThePast_Fails()
    {
        CardDetails cardDetails = new CardDetails("1234567890123456", _utcNow.Month - 1, _utcNow.Year, "123");

        ValidationResult? result = _sut.Validate(cardDetails);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void ExpiryMonth_NotBetween1And12_Fails(int month)
    {
        CardDetails cardDetails = new("1234567890123456", month, _utcNow.Year, "123");

        ValidationResult? result = _sut.Validate(cardDetails);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(CardDetails.ExpiryMonth));
    }
}