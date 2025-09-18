using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TicTacToe.Api.Tests;

public class AdminEndpointsTests : IClassFixture<WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program>>
{
    private readonly WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> _factory;

    public AdminEndpointsTests(WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> factory)
    {
        _factory = factory;
    }

    private void AddBasicAuthHeader(HttpClient client)
    {
        var auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("admin:password"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
    }

    [Fact]
    public async Task Export_Reset_Stats_RequireAuth()
    {
        var client = _factory.CreateClient();

        // Unauthorized should return 401 or challenge
        var resp = await client.GetAsync("/admin/rankings/export");
        Assert.False(resp.IsSuccessStatusCode);

        AddBasicAuthHeader(client);
        var exp = await client.GetAsync("/admin/rankings/export");
        Assert.True(exp.IsSuccessStatusCode);

        var reset = await client.PostAsync("/admin/rankings/reset", null);
        Assert.True(reset.IsSuccessStatusCode);

        var stats = await client.GetAsync("/admin/stats");
        Assert.True(stats.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ImportRequiresAuth()
    {
        var client = _factory.CreateClient();
        var json = "[]";
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var resp = await client.PostAsync("/admin/rankings/import", content);
        Assert.False(resp.IsSuccessStatusCode);

        AddBasicAuthHeader(client);
        var resp2 = await client.PostAsync("/admin/rankings/import", content);
        Assert.True(resp2.IsSuccessStatusCode);
    }
}
