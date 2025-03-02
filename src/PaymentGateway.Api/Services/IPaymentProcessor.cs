using PaymentGateway.Domain;

namespace PaymentGateway.Api.Services;

public interface IPaymentProcessor
{
    Task<Payment> ProcessPayment(CardDetails cardDetails, Money amount, CancellationToken cancellationToken);
}