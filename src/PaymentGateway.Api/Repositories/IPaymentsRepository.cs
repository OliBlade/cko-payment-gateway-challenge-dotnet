using PaymentGateway.Domain;

namespace PaymentGateway.Api.Repositories;

public interface IPaymentsRepository
{
    Task Add(Payment payment);
    Task<Payment?> Get(Guid id);
}