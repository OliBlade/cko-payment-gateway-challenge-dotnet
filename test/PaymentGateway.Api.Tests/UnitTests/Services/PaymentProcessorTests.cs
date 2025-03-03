using FluentValidation;
using FluentValidation.Results;
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
    private readonly Mock<IValidator<Payment>> _paymentValidatorMock;
    
    private readonly CardDetails _cardDetails = new("1234567890123456", 11, 24, "233");
    private readonly Money _amount = new(100, new Currency("GBP"));
    
    private readonly PaymentProcessor _sut;
    
    public PaymentProcessorTests()
    {
        _acquiringBankAdapterMock = new Mock<IAcquiringBankAdapter>(MockBehavior.Strict);
        _paymentsRepositoryMock = new Mock<IPaymentsRepository>(MockBehavior.Strict);
        _paymentValidatorMock = new Mock<IValidator<Payment>>(MockBehavior.Strict);
        Mock<ILogger<PaymentProcessor>> loggerMock = new();

        _sut = new PaymentProcessor(_acquiringBankAdapterMock.Object, _paymentsRepositoryMock.Object,
            _paymentValidatorMock.Object,
            loggerMock.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ProcessPayment_WithValidPayment_ReturnsAuthorizedPayment(bool authorized)
    {
        // Arrange
        CancellationToken cancellationToken = new();
        AuthorizationResult authorizationResult = new(authorized, authorized ? Guid.NewGuid() : null);

        _paymentsRepositoryMock.Setup(x => x.Add(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        _acquiringBankAdapterMock.Setup(x => x.ProcessPayment(_cardDetails, _amount, cancellationToken))
            .ReturnsAsync(authorizationResult);
        _paymentValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<Payment>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        // Act
        Payment result = await _sut.ProcessPayment(_cardDetails, _amount, cancellationToken);

        // Assert
        PaymentStatus expectedStatus = authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
        Guid? expectedAuthorizationCode = authorized ? authorizationResult.AuthorizationCode : null;

        result.Status.Should().Be(expectedStatus);
        result.AuthorizationCode.Should().Be(expectedAuthorizationCode);

        _paymentsRepositoryMock.Verify(x => x.Add(It.Is<Payment>(p => p.Id == result.Id)), Times.Once);
        _acquiringBankAdapterMock.Verify(x => x.ProcessPayment(_cardDetails, _amount, cancellationToken), Times.Once);
        _paymentValidatorMock.Verify(x => x.ValidateAsync(It.IsAny<Payment>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_WhenAcquiringBankAdapterThrowsException_ThrowsException()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        Exception exception = new("Test exception");

        _acquiringBankAdapterMock.Setup(x => x.ProcessPayment(_cardDetails, _amount, cancellationToken))
            .ThrowsAsync(exception);
        _paymentValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<Payment>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        // Act
        Func<Task> act = async () => await _sut.ProcessPayment(_cardDetails, _amount, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<Exception>();

        _paymentsRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Never);
        _acquiringBankAdapterMock.Verify(x => x.ProcessPayment(_cardDetails, _amount, cancellationToken), Times.Once);
        _paymentValidatorMock.Verify(x => x.ValidateAsync(It.IsAny<Payment>(), cancellationToken), Times.Once);
    }
    
    [Fact]
    public async Task ProcessPayment_WhenValidationFails_RejectsPayment()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        Exception exception = new("Test exception");

        _paymentsRepositoryMock.Setup(x => x.Add(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        _acquiringBankAdapterMock.Setup(x => x.ProcessPayment(_cardDetails, _amount, cancellationToken))
            .ThrowsAsync(exception);
        _paymentValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<Payment>(), cancellationToken))
            .ReturnsAsync(new ValidationResult
            {
                Errors = { new ValidationFailure("TestError", "Validation Error") }
            });

        // Act
        Payment result = await _sut.ProcessPayment(_cardDetails, _amount, cancellationToken);

        // Assert
        result.Status.Should().Be(PaymentStatus.Rejected);

        _paymentsRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Once);
        _acquiringBankAdapterMock.Verify(x => x.ProcessPayment(_cardDetails, _amount, cancellationToken), Times.Never);
        _paymentValidatorMock.Verify(x => x.ValidateAsync(It.IsAny<Payment>(), cancellationToken), Times.Once);
    }
}