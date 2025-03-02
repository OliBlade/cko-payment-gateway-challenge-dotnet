using PaymentGateway.Adapters.AcquiringBankAdapter;

namespace PaymentGateway.Adapters.Tests.AcquiringBankAdapter;

public class AuthorizationResultTests
{
    [Fact]
    public void AuthorizationCode_WhenNotAuthorized_ThrowsInvalidOperationException()
    {
        // Arrange
        AuthorizationResult authorizationResult = new (false, null);
        
        // Act
        Action act = () => _ = authorizationResult.AuthorizationCode;
        
        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void AuthorizationCode_WhenAuthorized_ReturnsAuthorizationCode()
    {
        // Arrange
        Guid expectedAuthorizationCode = Guid.NewGuid();
        AuthorizationResult authorizationResult = new (true, expectedAuthorizationCode);
        
        // Act
        Guid result = authorizationResult.AuthorizationCode;
        
        // Assert
        result.Should().Be(expectedAuthorizationCode);
    }
}