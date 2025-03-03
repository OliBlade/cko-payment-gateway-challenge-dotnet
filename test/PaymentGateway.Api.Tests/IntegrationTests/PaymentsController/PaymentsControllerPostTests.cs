using System.Net;
using System.Net.Http.Json;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.IntegrationTests.PaymentsController;

public class PaymentsControllerPostTests : IntegrationTestBase
{
    [Fact]
    public async Task Post_WithValidPaymentRequest_ReturnsPaymentResponse()
    {
        // Arrange
        PostPaymentRequest paymentRequest = new("2222405343248877", _random.Next(1, 12), _random.Next(2023, 2030),
            "123", "GBP", _random.Next(1, 10000));
        
        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUrl, paymentRequest);
        PaymentResponse? paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.Should().NotBeNull();
    }
}