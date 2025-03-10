using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.IntegrationTests.PaymentsController;

public class PaymentsControllerPostTests : IntegrationTestBase
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new (JsonSerializerOptions.Default)
    {
        Converters = { new JsonStringEnumConverter() }
    };
    
    [Fact]
    public async Task Post_WithValidPaymentRequest_ReturnsPaymentResponse()
    {
        // Arrange
        DateTime utcNow = DateTime.UtcNow;
        DateTime dateInFuture = utcNow.AddYears(1);
        
        PostPaymentRequest paymentRequest = new("2222405343248877", _random.Next(1, 12), dateInFuture.Year,
            "123", "GBP", _random.Next(1, 10000));
        
        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUrl, paymentRequest);
        PaymentResponse? paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>(_jsonSerializerOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.Should().NotBeNull();
    }
    
    [Theory]
    [MemberData(nameof(InvalidModelRequests))]
    public async Task Post_WithModelInvalidErrors_ReturnsBadRequest(PostPaymentRequest paymentRequest, string failureMessage)
    {
        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(BaseUrl, paymentRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, $"{failureMessage} model validation error");
    }
    
    public static IEnumerable<object[]> InvalidModelRequests()
    {
        yield return new object[] { new PostPaymentRequest(null, 1, 2023, "123", "GBP", 1), "Card number null" };
        yield return new object[] { new PostPaymentRequest("2222405343248877", 13, 2023, "123", "GBP", 1), "Expiry month more than 12" };
        yield return new object[] { new PostPaymentRequest("2222405343248877", 0, 2023, "123", "GBP", 1), "Expiry month less than 1" };
        yield return new object[] { new PostPaymentRequest("2222405343248877", 12, 0, "123", "GBP", 1), "Expiry year less than 1" };
        yield return new object[] { new PostPaymentRequest("2222405343248877", 12, 2023, null, "GBP", 1), "Cvv null" };
        yield return new object[] { new PostPaymentRequest("2222405343248877", 12, 2023, "123", null, 0), "Currency null" };
        yield return new object[] { new PostPaymentRequest("2222405343248877", 12, 2023, "123", "GBP", 0), "Amount less than 1" };
    }
}