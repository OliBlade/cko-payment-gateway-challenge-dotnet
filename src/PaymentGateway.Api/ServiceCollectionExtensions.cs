using PaymentGateway.Api.Providers;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentGatewayServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentsRepository, InMemoryPaymentsRepository>();
        services.AddScoped<IPaymentProcessor, PaymentProcessor>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        return services;
    }
}