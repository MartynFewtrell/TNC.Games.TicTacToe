using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Tnc.Games.TicTacToe.Api.Domain;
using Tnc.Games.TicTacToe.Shared;
using Xunit;

namespace TicTacToe.Api.Tests
{
    public class SelfPlayCanonicalizationTests : IClassFixture<WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program>>
    {
        private readonly WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> _factory;
        public SelfPlayCanonicalizationTests(WebApplicationFactory<Tnc.Games.TicTacToe.Api.Program> factory) => _factory = factory;

        [Fact]
        public async Task SelfPlay_StoresCanonicalKeys()
        {
            var client = _factory.CreateClient();
            var req = new { N = 5, Seed = 123 };
            var resp = await client.PostAsJsonAsync("/api/v1/selfplay", req);
            resp.EnsureSuccessStatusCode();

            // Inspect in-memory ranking store used by the app
            var store = _factory.Services.GetService(typeof(IRankingStore)) as IRankingStore;
            Assert.NotNull(store);

            var exported = store!.Export();
            // exported is an array of objects with 'state' field
            if (exported is System.Collections.IEnumerable e)
            {
                foreach (var item in e)
                {
                    var itemJson = System.Text.Json.JsonSerializer.Serialize(item);
                    using var doc = System.Text.Json.JsonDocument.Parse(itemJson);
                    var root = doc.RootElement;
                    if (!root.TryGetProperty("state", out var stateEl)) continue;
                    var stateKey = stateEl.GetString();
                    Assert.False(string.IsNullOrEmpty(stateKey));

                    // Verify stored key is canonical by re-canonicalizing its board
                    var board = BoardEncoding.FromStateKey(stateKey!);
                    var (canonicalKey, _) = Symmetry.GetCanonicalKeyAndTransform(board);
                    Assert.Equal(canonicalKey, stateKey);
                }
            }
        }
    }
}
