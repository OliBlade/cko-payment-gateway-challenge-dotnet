namespace PaymentGateway.Api.Providers;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}