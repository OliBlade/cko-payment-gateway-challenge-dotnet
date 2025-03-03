using FluentValidation;

using PaymentGateway.Api.Providers;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validators;
using PaymentGateway.Domain;

namespace PaymentGateway.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentGatewayServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentsRepository, InMemoryPaymentsRepository>();
        services.AddScoped<IPaymentProcessor, PaymentProcessor>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        services.AddTransient<IValidator<Payment>>(provider =>
            new PaymentValidator(provider.GetService<IDateTimeProvider>()!));
        
        return services;
    }
}