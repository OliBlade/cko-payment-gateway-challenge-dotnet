using System.Net;
using System.Net.Http.Json;
using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Tests.IntegrationTests.PaymentsController;

public class PaymentsControllerGetTests : IntegrationTestBase
{
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
        await _inMemoryPaymentsRepository.Add(payment);

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{BaseUrl}/{payment.Id}");
        PaymentResponse? paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

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
        
        _inMemoryPaymentsRepository.Add(payment);

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}