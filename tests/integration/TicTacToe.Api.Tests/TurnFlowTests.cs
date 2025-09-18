using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TicTacToe.Api.Tests;

public class TurnFlowTests : IClassFixture<WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program>>
{
    private readonly WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> _factory;

    public TurnFlowTests(WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateSessionAndApplyTurn()
    {
        var client = _factory.CreateClient();
        var req = new { Move = 1 };
        var resp = await client.PostAsJsonAsync("/api/v1/turn", req);
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task InvalidMoveReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        // First move creates session and applies move 1
        var req1 = new { Move = 1 };
        var resp1 = await client.PostAsJsonAsync("/api/v1/turn", req1);
        resp1.EnsureSuccessStatusCode();
        var json1 = await resp1.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(json1.TryGetProperty("sessionId", out var sessionIdProp));
        var sessionId = sessionIdProp.GetString();
        Assert.False(string.IsNullOrEmpty(sessionId));

        // Second move try same position -> should be invalid
        var req2 = new { Move = 1 };
        var resp2 = await client.PostAsJsonAsync($"/api/v1/turn?sessionId={sessionId}", req2);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, resp2.StatusCode);
        var err = await resp2.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(err.TryGetProperty("code", out var code));
        Assert.Equal("InvalidMove", code.GetString());
    }

    [Fact]
    public async Task GetStateReturns404ForMissingSession()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync($"/api/v1/state?sessionId={System.Guid.NewGuid()}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task GetStateReturnsBadRequestForMissingOrInvalidId()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/v1/state");
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, resp.StatusCode);

        var resp2 = await client.GetAsync("/api/v1/state?sessionId=not-a-guid");
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, resp2.StatusCode);
    }
}
