using PaymentGateway.Adapters.AcquiringBankAdapter;
using PaymentGateway.Adapters.AcquiringBankAdapter.Contracts;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Services;

public class PaymentProcessor
{
    private readonly IAcquiringBankAdapter _acquiringBankAdapter;
    private readonly PaymentsRepository _paymentsRepository;

    public PaymentProcessor(IAcquiringBankAdapter acquiringBankAdapter)
    {
        _acquiringBankAdapter = acquiringBankAdapter;
    }

    public async Task<Payment> ProcessPayment(CardDetails cardDetails, Money amount, CancellationToken cancellationToken)
    {
        Payment payment = new(cardDetails, amount);
        ProcessPaymentResponse? result = await _acquiringBankAdapter.ProcessPayment(payment.CardDetails, payment.Amount, cancellationToken);

        if (result == null || result.Authorized == false)
        {
            payment.Decline();
        }
        
        if (result != null && result.Authorized)
        {
            payment.Authorize(result.AuthorizationCode);
        }

        return payment;
    }
}