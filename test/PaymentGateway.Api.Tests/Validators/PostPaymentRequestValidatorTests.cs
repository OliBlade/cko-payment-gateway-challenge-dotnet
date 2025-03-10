using FluentValidation.Results;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Providers;
using PaymentGateway.Api.Validators;

namespace PaymentGateway.Api.Tests.Validators;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _sut;
    private readonly DateTime _utcNow = DateTime.UtcNow;
    
    public PostPaymentRequestValidatorTests()
    {
        Mock<IDateTimeProvider> dateTimeProviderMock = new();
        dateTimeProviderMock.Setup(dp => dp.UtcNow).Returns(_utcNow);
        
        _sut = new PostPaymentRequestValidator(dateTimeProviderMock.Object);
    }
    
    [Fact]
    public void Validate_WithValidValues_Succeeds()
    {
        // Arrange
        PostPaymentRequest request = new("12345678901234", 4, _utcNow.Year + 1, "123", "GBP", 1);

        // Act
        ValidationResult? result = _sut.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("123456789012")] // 12-digit number
    [InlineData("12345678901234567890")] // 20-digit number
    public void Validate_CardNumberInvalidLength_Fails(string cardNumber)
    {
        // Arrange
        PostPaymentRequest request = new(cardNumber, 4, _utcNow.Year + 1, "123", "GBP", 1);

        // Act
        ValidationResult? result = _sut.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(request.CardNumber));
    }
    
    [Fact]
    public void Validate_ExpiryDateOneMonthInThePast_Fails()
    {
        // Arrange
        DateTime dateInPast = DateTime.UtcNow - TimeSpan.FromDays(31);
        PostPaymentRequest request = new("12345678901234", dateInPast.Month, dateInPast.Year, "123", "GBP", 1);

        // Act
        ValidationResult? result = _sut.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public void Validate_CurrencyCodeIsEmpty_ShouldFail()
    {
        // Arrange
        PostPaymentRequest request = new("12345678901234", 4, _utcNow.Year + 1, "123", null, 1);

        // Act
        ValidationResult result = _sut.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(request.Currency));
    }
    
    
    [Fact]
    public void Validate_WhenCurrencyCodeIsNotInValidList_ShouldFail()
    {
        // Arrange
        PostPaymentRequest request = new("12345678901234", 4, _utcNow.Year + 1, "123", "AUD", 1);

        // Act
        ValidationResult result = _sut.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(request.Currency));
    }
    
    [Fact]
    public void Validate_AmountIsZero_ShouldFail()
    {
        // Arrange
        PostPaymentRequest request = new("12345678901234", 4, _utcNow.Year + 1, "123", "AUD", 0);

        // Act
        ValidationResult? result = _sut.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(request.Amount));
    }
}