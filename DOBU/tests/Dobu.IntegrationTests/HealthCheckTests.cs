using System.Net;
using Xunit;

namespace Dobu.IntegrationTests;

[Collection(ApiCollection.Name)]
public class HealthCheckTests
{
    private readonly HttpClient _client;

    public HealthCheckTests(DobuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ApiDisponivel_RetornaSucesso()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetPets_SemAutenticacao_RetornaNaoAutorizado()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/pets");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMetrics_AplicacaoInstrumentada_RetornaConteudoPrometheus()
    {
        // Arrange
        await _client.GetAsync("/api/pets");

        // Act
        var response = await _client.GetAsync("/metrics");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("http_server_request_duration", body);
        Assert.Contains("dobu_api_errors_total", body);
    }

    [Fact]
    public async Task GetReady_HealthChecksConfigurados_RetornaStatusDeDependencias()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/health/ready");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("database", body);
        Assert.Contains("external-service", body);
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable });
    }

    [Fact]
    public async Task GetHealth_CorrelationIdInformado_DevolveMesmoIdentificador()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("X-Correlation-ID", "correlation-test-123");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal("correlation-test-123", response.Headers.GetValues("X-Correlation-ID").Single());
    }
}
