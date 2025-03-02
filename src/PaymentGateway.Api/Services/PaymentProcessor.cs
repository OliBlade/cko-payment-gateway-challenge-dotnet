using PaymentGateway.Adapters.AcquiringBankAdapter;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Services;

public class PaymentProcessor : IPaymentProcessor
{
    private readonly IAcquiringBankAdapter _acquiringBankAdapter;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly ILogger<PaymentProcessor> _logger;

    public PaymentProcessor(IAcquiringBankAdapter acquiringBankAdapter, IPaymentsRepository paymentsRepository, ILogger<PaymentProcessor> logger)
    {
        _acquiringBankAdapter = acquiringBankAdapter;
        _paymentsRepository = paymentsRepository;
        _logger = logger;
    }

    public async Task<Payment> ProcessPayment(CardDetails cardDetails, Money amount, CancellationToken cancellationToken)
    {
        Payment payment = new(cardDetails, amount);
        AuthorizationResult result;

        try
        {
            result = await _acquiringBankAdapter.ProcessPayment(payment.CardDetails, payment.Amount, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process payment");
            throw;
        }

        switch (result.Authorized)
        {
            case true:
                payment.Authorize(result.AuthorizationCode);
                break;
            case false:
                payment.Decline();
                break;
        }
        
        _logger.LogInformation("Payment {Id} processed with status {Status}", payment.Id, payment.Status);

        // TODO: what happens if the payment fails to be added to the repository?
        await _paymentsRepository.Add(payment);
        return payment;
    }
}