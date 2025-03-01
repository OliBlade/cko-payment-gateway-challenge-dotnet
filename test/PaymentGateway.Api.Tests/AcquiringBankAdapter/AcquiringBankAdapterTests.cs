using System.Net;
using System.Text.Json;
using Moq.Protected;
using PaymentGateway.Api.AcquiringBankAdapter;
using PaymentGateway.Api.AcquiringBankClient.Contracts;

namespace PaymentGateway.Api.Tests.AcquiringBankAdapter;

public class AcquiringBankAdapterTests
{
    [Fact]
    public async Task ProcessPayment_WithValidRequest_ReturnsResponse()
    {
        // Arrange
        ProcessPaymentRequest request = new ("2222405343248877", "04/2025", "GBP", 100, "123");
        ProcessPaymentResponse expectedResponse = new (true, Guid.NewGuid());
        IAcquiringBankAdapter acquiringBankAdapter = SetupAcquiringBankClient(HttpStatusCode.OK, expectedResponse);

        // Act
        ProcessPaymentResponse response = (await acquiringBankAdapter.ProcessPayment(request))!;

        // Assert
        response.Should().NotBeNull();
        response.Authorized.Should().Be(expectedResponse.Authorized);
        response.AuthorizationCode.Should().Be(expectedResponse.AuthorizationCode);
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task ProcessPayment_WhenResponseCodeIsInvalid_ThrowsHttpRequestException(HttpStatusCode statusCode)
    {
        // Arrange
        IAcquiringBankAdapter acquiringBankAdapter = SetupAcquiringBankClient(statusCode);
        ProcessPaymentRequest request = new ("2222405343248877", "04/2025", "GBP", 100, "123");

        // Act
        Func<Task> act = async () => await acquiringBankAdapter.ProcessPayment(request);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }
    
    private static IAcquiringBankAdapter SetupAcquiringBankClient(HttpStatusCode responseStatusCode, object? responseContent = null)
    {
        Mock<HttpMessageHandler> messageHandlerMock = new (MockBehavior.Strict);

        messageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken token) => {
                
                HttpResponseMessage response = new ();
                response.StatusCode = responseStatusCode;

                if (responseContent != null)
                {
                    response.Content = new StringContent(JsonSerializer.Serialize(responseContent));
                }
                return Task.FromResult(response);
            })
            .Verifiable();

        HttpClient httpClient = new(messageHandlerMock.Object) { BaseAddress = new Uri("https://notreal.com") };
        return new Api.AcquiringBankAdapter.AcquiringBankAdapter(httpClient);
    }
}