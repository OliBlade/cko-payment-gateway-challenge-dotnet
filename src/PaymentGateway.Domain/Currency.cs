namespace PaymentGateway.Domain;

public sealed class Currency(string code)
{
    public string Code { get; private set; } = code;

    public static readonly string[] ValidCurrencies = { "USD", "EUR", "GBP" };
}