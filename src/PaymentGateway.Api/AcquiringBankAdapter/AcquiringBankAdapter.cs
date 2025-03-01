using PaymentGateway.Api.AcquiringBankClient.Contracts;

namespace PaymentGateway.Api.AcquiringBankAdapter;

public class AcquiringBankAdapter(HttpClient client)  : IAcquiringBankAdapter
{
    private const string PaymentsUri = "payments";
    
    public async Task<ProcessPaymentResponse?> ProcessPayment(ProcessPaymentRequest request)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(PaymentsUri, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProcessPaymentResponse>();
    }
}