using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Tests.IntegrationTests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    private readonly HttpClient _client;
    private readonly PaymentsRepository _paymentsRepository = new();

    private const string BaseUrl = "/api/Payments";

    public PaymentsControllerTests()
    {
        WebApplicationFactory<PaymentsController> webApplicationFactory = new();

        _client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton(_paymentsRepository)))
            .CreateClient();
    }

    [Theory]
    [InlineData(PaymentStatus.Authorized)]
    [InlineData(PaymentStatus.Declined)]
    public async Task Get_PaymentFound_ReturnsPayment(PaymentStatus paymentStatus)
    {
        // Arrange
        CardDetails cardDetails = new("2222405343248877", _random.Next(1, 12), _random.Next(2023, 2030), "123");
        Money money = new(_random.Next(1, 10000), new Currency("GBP"));
        Payment payment = new(cardDetails, money);

        switch (paymentStatus)
        {
            case PaymentStatus.Authorized:
                payment.Authorize(Guid.NewGuid());
                break;
            case PaymentStatus.Declined:
                payment.Decline();
                break;
        }
        _paymentsRepository.Add(payment);

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{BaseUrl}/{payment.Id}");
        PostPaymentResponse? paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_WhenPaymentNotFound_Returns404()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_WhenPaymentRejected_Returns404()
    {
        // Arrange
        CardDetails cardDetails = new("2222405343248877", _random.Next(1, 12), _random.Next(2023, 2030), "123");
        Money money = new(_random.Next(1, 10000), new Currency("GBP"));
        Payment payment = new(cardDetails, money);
        payment.Reject();
        
        _paymentsRepository.Add(payment);

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}