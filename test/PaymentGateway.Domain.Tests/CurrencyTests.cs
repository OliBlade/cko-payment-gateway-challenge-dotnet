using System.Diagnostics;

namespace PaymentGateway.Domain.Tests;

public class CurrencyTests
{
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    public void Constructor_WithValidCurrencyCode_SetsCode(string code)
    {
        // Arrange
        Currency? currency = null;
        
        // Act
        Func<Currency> act = () => currency = new Currency(code);
        
        // Assert
        act.Should().NotThrow();
        currency?.Code.Should().Be(code);
    }
    
    [Theory]
    [InlineData("CAD")]
    [InlineData("JPY")]
    [InlineData("AUD")]
    public void Constructor_WithInvalidCurrencyCode_ThrowsArgumentException(string code)
    {
        // Arrange
        Currency? currency = null;
        
        // Act
        Func<Currency> act = () => currency = new Currency(code);
        
        // Assert
        act.Should().Throw<ArgumentException>();
        currency?.Code.Should().BeNull();
    }
}