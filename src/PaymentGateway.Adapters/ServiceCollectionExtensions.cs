using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Adapters.AcquiringBankAdapter;

namespace PaymentGateway.Adapters;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdapters(this IServiceCollection services, IConfiguration configuration)
    {
        Uri? uri = configuration.GetSection("AcquiringBank:BaseUrl").Get<Uri>();
        ArgumentNullException.ThrowIfNull(uri);

        services.AddHttpClient<IAcquiringBankAdapter, AcquiringBankAdapter.AcquiringBankAdapter>(client =>
            client.BaseAddress = uri);
        
        return services;
    }
}