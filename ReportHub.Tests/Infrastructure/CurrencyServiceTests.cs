using Microsoft.Extensions.Caching.Memory;
using Moq.Protected;
using Moq;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Infrastructure.Services.CurrencyServices;
using System.Net;

namespace ReportHub.Tests.Infrastructure;

public class CurrencyServiceTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<IMongoRepositoryBase<Currency>> _currencyRepoMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly CurrencyService _currencyService;

    public CurrencyServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _currencyRepoMock = new Mock<IMongoRepositoryBase<Currency>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        _currencyService = new CurrencyService(
            _currencyRepoMock.Object,
            _memoryCache,
            _httpClientFactoryMock.Object
        );
    }

    [Fact]
    public async Task GetExchangeRateAsync_Returns_1_If_Same_Currency()
    {
        var result = await _currencyService.GetExchangeRateAsync("USD", "USD", DateTime.UtcNow, CancellationToken.None);
        Assert.Equal(1m, result);
    }

    [Fact]
    public async Task GetExchangeRateAsync_Uses_Cache_If_Available()
    {
        string from = "EUR", to = "USD";
        DateTime date = DateTime.UtcNow.Date;
        string cacheKey = $"ExchangeRate_{from}_{to}_{date:yyyy-MM-dd}";
        _memoryCache.Set(cacheKey, 1.23m);

        var result = await _currencyService.GetExchangeRateAsync(from, to, date, CancellationToken.None);

        Assert.Equal(1.23m, result);
    }

    [Fact]
    public async Task GetExchangeRateAsync_Calls_Ecb_For_Eur_To_Other()
    {
        // Arrange
        var currencyCode = "USD";
        var date = DateTime.UtcNow.Date;

        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    @"<gesmes:Envelope xmlns:gesmes='http://www.gesmes.org/xml/2002-08-01' xmlns='http://www.ecb.int/vocabulary/2002-08-01/eurofxref'>
                        <Cube>
                            <Cube time='" + date.ToString("yyyy-MM-dd") + @"'>
                                <Cube currency='USD' rate='1.10'/>
                            </Cube>
                        </Cube>
                    </gesmes:Envelope>")
            });

        var client = new HttpClient(httpMessageHandler.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        // Act
        var result = await _currencyService.GetExchangeRateAsync("EUR", currencyCode, date, CancellationToken.None);

        // Assert
        Assert.Equal(1.10m, result);
    }

    [Fact]
    public async Task GetExchangeRateAsync_Falls_Back_And_Succeeds_On_Earlier_Date()
    {
        var date = new DateTime(2000, 1, 1);
        int callCount = 0;

        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount < 3)
                {
                    throw new Exception("ECB down");
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($@"
                <gesmes:Envelope xmlns:gesmes='http://www.gesmes.org/xml/2002-08-01' xmlns='http://www.ecb.int/vocabulary/2002-08-01/eurofxref'>
                    <Cube>
                        <Cube time='{date.AddDays(-2):yyyy-MM-dd}'>
                            <Cube currency='USD' rate='1.10'/>
                            <Cube currency='GBP' rate='0.90'/>
                        </Cube>
                    </Cube>
                </gesmes:Envelope>")
                };
            });

        var client = new HttpClient(handler.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var result = await _currencyService.GetExchangeRateAsync("USD", "GBP", date, CancellationToken.None);

        Assert.Equal(0.90m / 1.10m, result, 2);
        Assert.True(callCount >= 3);
    }

}
