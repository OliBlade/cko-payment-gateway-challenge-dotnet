namespace PaymentGateway.Domain.Tests;

public class CurrencyTests
{
    [Fact]
    public void Constructor_WhenCreatingCurrency_ThenCurrencyIsCreated()
    {
        // Arrange
        const string code = "USD";

        // Act
        Currency currency = new (code);

        // Assert
        currency.Code.Should().Be(code);
    }
    
    [Fact]
    public void Constructor_WhenCreatingCurrencyWithInvalidCode_ThenThrowsArgumentException()
    {
        // Arrange
        const string code = "AUD";

        // Act
        Action act = () =>
        {
            Currency _ = new (code);
        };

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}