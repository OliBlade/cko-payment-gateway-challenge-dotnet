using Microsoft.Extensions.Logging;
using PaymentGateway.Adapters.AcquiringBankAdapter;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Api.Tests.UnitTests.Services;

public class PaymentProcessorTests
{
    private readonly Mock<IAcquiringBankAdapter> _acquiringBankAdapterMock;
    private readonly Mock<IPaymentsRepository> _paymentsRepositoryMock;

    private readonly PaymentProcessor _sut;

    public PaymentProcessorTests()
    {
        _acquiringBankAdapterMock = new Mock<IAcquiringBankAdapter>(MockBehavior.Strict);
        _paymentsRepositoryMock = new Mock<IPaymentsRepository>(MockBehavior.Strict);
        Mock<ILogger<PaymentProcessor>> loggerMock = new();

        _sut = new PaymentProcessor(_acquiringBankAdapterMock.Object, _paymentsRepositoryMock.Object,
            loggerMock.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ProcessPayment_WithValidPayment_ReturnsAuthorizedPayment(bool authorized)
    {
        // Arrange
        CardDetails cardDetails = new("1234567890123456", 11, 24, "233");
        Money amount = new(100, new Currency("GBP"));

        CancellationToken cancellationToken = new();
        AuthorizationResult authorizationResult = new(authorized, authorized ? Guid.NewGuid() : null);
        
        _paymentsRepositoryMock.Setup(x => x.Add(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        _acquiringBankAdapterMock.Setup(x => x.ProcessPayment(cardDetails, amount, cancellationToken))
            .ReturnsAsync(authorizationResult);

        // Act
        Payment result = await _sut.ProcessPayment(cardDetails, amount, cancellationToken);

        // Assert
        PaymentStatus expectedStatus = authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
        Guid? expectedAuthorizationCode = authorized ? authorizationResult.AuthorizationCode : null;
        
        result.Status.Should().Be(expectedStatus);
        result.AuthorizationCode.Should().Be(expectedAuthorizationCode);
        
        _paymentsRepositoryMock.Verify(x => x.Add(It.Is<Payment>(p => p.Id == result.Id)), Times.Once);
        _acquiringBankAdapterMock.Verify(x => x.ProcessPayment(cardDetails, amount, cancellationToken), Times.Once);
    }
    
    [Fact]
    public async Task ProcessPayment_WhenAcquiringBankAdapterThrowsException_ThrowsException()
    {
        // Arrange
        CardDetails cardDetails = new("1234567890123456", 11, 24, "233");
        Money amount = new(100, new Currency("GBP"));

        CancellationToken cancellationToken = new();
        Exception exception = new ("Test exception");
        
        _acquiringBankAdapterMock.Setup(x => x.ProcessPayment(cardDetails, amount, cancellationToken))
            .ThrowsAsync(exception);

        // Act
        Func<Task> act = async () => await _sut.ProcessPayment(cardDetails, amount, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        
        _paymentsRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Never);
        _acquiringBankAdapterMock.Verify(x => x.ProcessPayment(cardDetails, amount, cancellationToken), Times.Once);
    }
}