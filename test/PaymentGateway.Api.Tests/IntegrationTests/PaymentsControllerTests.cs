using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

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
        PostPaymentResponse payment = new()
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP",
            Status = paymentStatus
        };
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
        PostPaymentResponse payment = new()
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP",
            Status = PaymentStatus.Rejected
        };
        _paymentsRepository.Add(payment);

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}