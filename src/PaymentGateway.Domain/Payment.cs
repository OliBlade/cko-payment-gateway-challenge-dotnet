using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain;

public sealed class Payment
{
    public Guid Id { get; }
    public Guid? AuthorizationCode { get; private set; }
    public PaymentStatus Status { get; private set; }
    public CardDetails CardDetails { get; }
    public Money Amount { get;}

    public Payment(CardDetails cardDetails, Money amount)
    {
        Id = Guid.NewGuid();
        CardDetails = cardDetails;
        Amount = amount;
        Status = PaymentStatus.New;
    }

    public void Authorize(Guid authorizationCode)
    {
        AuthorizationCode = authorizationCode;
        Status = PaymentStatus.Authorized;
    }

    public void Decline() => Status = PaymentStatus.Declined;
    public void Reject() => Status = PaymentStatus.Rejected;
}