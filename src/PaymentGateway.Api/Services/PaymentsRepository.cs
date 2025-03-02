using PaymentGateway.Domain;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    private readonly Dictionary<Guid, Payment> _payments = [];
    
    public void Add(Payment payment)
    {
        _payments.Add(payment.Id, payment);
    }

    public Payment? Get(Guid id)
    {
        _payments.TryGetValue(id, out Payment? payment);
        return payment;
    }
}