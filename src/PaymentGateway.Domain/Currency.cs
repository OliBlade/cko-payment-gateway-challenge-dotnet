namespace PaymentGateway.Domain;

public sealed class Currency
{
    public string Code { get; }

    public static readonly string[] ValidCurrencies = { "USD", "EUR", "GBP" };
    
    public Currency(string code)
    {
        if (!ValidCurrencies.Contains(code))
        {
            throw new ArgumentException($"Invalid currency code: {code}");
        }
        Code = code;
    }
}