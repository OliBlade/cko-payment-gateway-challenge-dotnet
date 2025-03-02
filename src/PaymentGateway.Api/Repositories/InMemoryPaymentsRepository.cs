using PaymentGateway.Domain;

namespace PaymentGateway.Api.Repositories;

public class InMemoryPaymentsRepository : IPaymentsRepository
{
    private readonly Dictionary<Guid, Payment> _payments = [];
    
    public Task Add(Payment payment)
    {
        _payments.Add(payment.Id, payment);
        return Task.CompletedTask;
    }

    public Task<Payment?> Get(Guid id)
    {
        _payments.TryGetValue(id, out Payment? payment);
        return Task.FromResult(payment);
    }
}