using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TicTacToe.Api.Tests;

public class NewSessionAndTurnTests : IClassFixture<WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program>>
{
    private readonly WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> _factory;

    public NewSessionAndTurnTests(WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateSession_WithOptions_ReturnsHumanSymbolAndNextPlayer()
    {
        var client = _factory.CreateClient();
        var opts = new { Mode = "HvsAI", Starter = "AI", HumanSymbol = "O" };
        var resp = await client.PostAsJsonAsync("/api/v1/new", opts);
        resp.EnsureSuccessStatusCode();
        var doc = await resp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(doc.TryGetProperty("sessionId", out var sid));
        Assert.True(doc.TryGetProperty("humanSymbol", out var hs));
        Assert.Equal("O", hs.GetString());
        Assert.True(doc.TryGetProperty("nextPlayer", out var np));
        // If human is O and starter is AI, nextPlayer should be X
        Assert.Equal("X", np.GetString());
    }

    [Fact]
    public async Task Turn_HonorsHumanSymbol_O()
    {
        var client = _factory.CreateClient();
        // Create session where human is O and human starts
        var opts = new { Mode = "HvsAI", Starter = "Human", HumanSymbol = "O" };
        var createResp = await client.PostAsJsonAsync("/api/v1/new", opts);
        createResp.EnsureSuccessStatusCode();
        var created = await createResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(created.TryGetProperty("sessionId", out var sid));
        var sessionId = sid.GetString();

        // Make a human move at position 1
        var turnReq = new { Move = 1 };
        var turnResp = await client.PostAsJsonAsync($"/api/v1/turn?sessionId={sessionId}", turnReq);
        turnResp.EnsureSuccessStatusCode();
        var turnDoc = await turnResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(turnDoc.TryGetProperty("board", out var board));
        Assert.Equal("O", board[0].GetString());
    }
}
