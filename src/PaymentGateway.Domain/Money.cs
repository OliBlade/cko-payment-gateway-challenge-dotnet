namespace PaymentGateway.Domain;

public sealed class Money
{
    public int Amount { get; private set; }
    public Currency Currency { get; private set; }
    
    public Money(int amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
}