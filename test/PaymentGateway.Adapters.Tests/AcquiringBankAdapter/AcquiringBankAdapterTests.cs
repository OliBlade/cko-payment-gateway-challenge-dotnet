using System.Net;
using System.Text.Json;
using Moq.Protected;
using PaymentGateway.Adapters.AcquiringBankAdapter;
using PaymentGateway.Adapters.AcquiringBankAdapter.Contracts;
using PaymentGateway.Domain;

namespace PaymentGateway.Adapters.Tests.AcquiringBankAdapter;

public class AcquiringBankAdapterTests
{
    [Fact]
    public async Task ProcessPayment_WithValidRequest_ReturnsResponse()
    {
        // Arrange
        CardDetails cardDetails = new ("2222405343248877", 4, 25, "123");
        Money money = new (100, new Currency("GBP"));
        
        ProcessPaymentResponse expectedResponse = new (true, Guid.NewGuid());
        IAcquiringBankAdapter acquiringBankAdapter = SetupAcquiringBankClient(HttpStatusCode.OK, expectedResponse);
        CancellationToken cancellationToken = new();

        // Act
        AuthorizationResult response = (await acquiringBankAdapter.ProcessPayment(cardDetails, money, cancellationToken));

        // Assert
        response.Should().NotBeNull();
        response.Authorized.Should().Be(expectedResponse.Authorized);
        response.AuthorizationCode.Should().Be(expectedResponse.AuthorizationCode!.Value);
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task ProcessPayment_WhenResponseCodeIsInvalid_ThrowsHttpRequestException(HttpStatusCode statusCode)
    {
        // Arrange
        IAcquiringBankAdapter acquiringBankAdapter = SetupAcquiringBankClient(statusCode);
        CardDetails cardDetails = new ("2222405343248877", 4, 25, "123");
        Money money = new (100, new Currency("GBP"));
        
        CancellationToken cancellationToken = new();
        
        // Act
        Func<Task> act = async () => await acquiringBankAdapter.ProcessPayment(cardDetails, money, cancellationToken);

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
        return new Adapters.AcquiringBankAdapter.AcquiringBankAdapter(httpClient);
    }
}