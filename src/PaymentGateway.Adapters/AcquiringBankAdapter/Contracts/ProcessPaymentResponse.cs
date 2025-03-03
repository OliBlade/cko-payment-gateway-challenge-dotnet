using System.Text.Json.Serialization;

namespace PaymentGateway.Adapters.AcquiringBankAdapter.Contracts;

public class ProcessPaymentResponse
{
    public bool Authorized { get; }

    [JsonPropertyName("authorization_code")]
    [JsonConverter(typeof(NullableGuidConverter))]
    public Guid? AuthorizationCode { get; }

    public ProcessPaymentResponse(bool authorized, Guid? authorizationCode)
    {
        Authorized = authorized;
        AuthorizationCode = authorizationCode;
    }
}