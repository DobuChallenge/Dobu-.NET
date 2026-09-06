using System.Net;
using System.Net.Http.Json;
using Dobu.Application.DTOs;
using Xunit;

namespace Dobu.IntegrationTests;

[Collection(ApiCollection.Name)]
public class AuthFlowTests(DobuApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RegisterELogin_CredenciaisValidas_RetornaTokenEPermiteEndpointProtegido()
    {
        // Arrange
        var email = $"usuario-{Guid.NewGuid():N}@dobu.com";

        // Act
        var register = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Ana", email, "Senha123", "Responsavel"));
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "Senha123"));
        var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
        var pets = await _client.GetAsync("/api/pets");

        // Assert
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(HttpStatusCode.OK, pets.StatusCode);
    }

    [Fact]
    public async Task Login_SenhaIncorreta_RetornaNaoAutorizado()
    {
        // Arrange
        var email = $"usuario-{Guid.NewGuid():N}@dobu.com";
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Ana", email, "Senha123", "Responsavel"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "SenhaErrada"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_DadosInvalidos_RetornaBadRequest()
    {
        // Arrange
        var request = new RegisterRequest("A", "invalido", "123", "Responsavel");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
