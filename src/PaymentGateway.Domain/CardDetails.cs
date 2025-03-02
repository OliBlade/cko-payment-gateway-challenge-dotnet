namespace PaymentGateway.Domain;

public sealed class CardDetails
{
    public int ExpiryMonth { get; }
    public int ExpiryYear { get; }
    public string Cvv { get; }
    private string CardNumber { get; }
    
    public CardDetails(string cardNumber, int expiryMonth, int expiryYear, string cvv)
    {
        CardNumber = cardNumber;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        Cvv = cvv;
    }
    
    public string FullCardNumber => CardNumber;
    public string CardNumberLastFour => CardNumber[^4..];
    public string FormattedExpiryDate => $"{ExpiryMonth.ToString().PadLeft(2, '0')}/{ExpiryYear}";
}