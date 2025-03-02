namespace PaymentGateway.Adapters.AcquiringBankAdapter;

public class AuthorizationResult(bool authorized, Guid? authorizationCode)
{
    public bool Authorized { get; } = authorized;

    public Guid AuthorizationCode
    {
        get
        {
            if (!Authorized || !authorizationCode.HasValue)
            {
                throw new InvalidOperationException("Authorization has not been set. The payment may have been declined.");
            }
            return authorizationCode.Value;
        }
    }
}