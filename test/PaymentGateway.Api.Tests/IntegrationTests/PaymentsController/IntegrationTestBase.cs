using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Tests.IntegrationTests.PaymentsController;

public class IntegrationTestBase
{
    protected readonly HttpClient _client;
    protected readonly InMemoryPaymentsRepository _inMemoryPaymentsRepository = new();
    protected readonly Random _random = new();
    
    protected const string BaseUrl = "/api/Payments";

    protected IntegrationTestBase()
    {
        WebApplicationFactory<Controllers.PaymentsController> webApplicationFactory = new();

        _client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton<IPaymentsRepository>(_inMemoryPaymentsRepository)))
            .CreateClient();
    }
}