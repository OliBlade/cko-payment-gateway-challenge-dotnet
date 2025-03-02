using PaymentGateway.Domain;

namespace PaymentGateway.Adapters.AcquiringBankAdapter;

public interface IAcquiringBankAdapter
{
    Task<AuthorizationResult> ProcessPayment(CardDetails cardDetails, Money money, CancellationToken cancellationToken);
}