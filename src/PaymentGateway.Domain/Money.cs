namespace PaymentGateway.Domain;

public sealed class Money
{
    public int Amount { get; }
    public Currency Currency { get; }
    
    public Money(int amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
}