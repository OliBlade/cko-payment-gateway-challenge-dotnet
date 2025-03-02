using System.Net.Http.Json;
using PaymentGateway.Adapters.AcquiringBankAdapter.Contracts;
using PaymentGateway.Domain;

namespace PaymentGateway.Adapters.AcquiringBankAdapter;

public class AcquiringBankAdapter(HttpClient client) : IAcquiringBankAdapter
{
    private const string PaymentsUri = "payments";

    public async Task<ProcessPaymentResponse?> ProcessPayment(CardDetails cardDetails, Money money,
        CancellationToken cancellationToken)
    {
        ProcessPaymentRequest request = new ProcessPaymentRequest(cardDetails.FullCardNumber,
            cardDetails.FormattedExpiryDate, money.Currency.Code, money.Amount, cardDetails.Cvv);
        
        HttpResponseMessage response = await client.PostAsJsonAsync(PaymentsUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProcessPaymentResponse>(cancellationToken);
    }
}