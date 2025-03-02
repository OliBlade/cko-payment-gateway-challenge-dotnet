namespace PaymentGateway.Domain.Tests;

public class CardDetailsTests
{
    [Fact]
    public void CardNumberLastFour_WithValidCardNumber_ReturnsLastFourDigits()
    {
        // Arrange
        CardDetails cardDetails = new ("1234567890123456", 1, 2023, "123");
        
        // Act
        string result = cardDetails.CardNumberLastFour;
        
        // Assert
        result.Should().Be("3456");
    }
    
    [Fact]
    public void FormattedExpiryDate_WithValidExpiryDate_ReturnsFormattedDate()
    {
        // Arrange
        CardDetails cardDetails = new ("1234567890123456", 1, 2023, "123");
        
        // Act
        string result = cardDetails.FormattedExpiryDate;
        
        // Assert
        result.Should().Be("01/2023");
    }
}