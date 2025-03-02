using PaymentGateway.Adapters.AcquiringBankAdapter.Contracts;
using PaymentGateway.Domain;

namespace PaymentGateway.Adapters.AcquiringBankAdapter;

public interface IAcquiringBankAdapter
{
    Task<ProcessPaymentResponse?> ProcessPayment(CardDetails cardDetails, Money money, CancellationToken cancellationToken);
}