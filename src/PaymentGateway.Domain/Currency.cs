namespace PaymentGateway.Domain;

public sealed class Currency
{
    public string Code { get; private set; }
    
    private readonly HashSet<string> _allowedCurrencies =
    [
        "USD",
        "EUR",
        "GBP"
    ];
    
    public Currency(string code)
    {
        if (!_allowedCurrencies.Contains(code))
        {
            throw new ArgumentException("Invalid currency code");
        }
        Code = code;
    }
}