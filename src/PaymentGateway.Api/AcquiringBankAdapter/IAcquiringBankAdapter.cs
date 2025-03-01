using PaymentGateway.Api.AcquiringBankClient.Contracts;

namespace PaymentGateway.Api.AcquiringBankAdapter;

public interface IAcquiringBankAdapter
{
    Task<ProcessPaymentResponse?> ProcessPayment(ProcessPaymentRequest request);
}