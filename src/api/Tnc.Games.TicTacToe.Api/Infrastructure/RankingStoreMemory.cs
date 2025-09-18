using System.Collections.Concurrent;
using System.Linq;
using Tnc.Games.TicTacToe.Api.Domain;

namespace Tnc.Games.TicTacToe.Api.Infrastructure
{
    public class RankingStoreMemory : IRankingStore
    {
        private readonly ConcurrentDictionary<(string, int), double> _store = new();
        private readonly double _min = -5.0;
        private readonly double _max = 5.0;

        public double? Get(string stateKey, int moveIndex)
        {
            if (_store.TryGetValue((stateKey, moveIndex), out var v)) return v;
            return null;
        }

        public void Set(string stateKey, int moveIndex, double value)
        {
            var clamped = Math.Max(_min, Math.Min(_max, value));
            _store[(stateKey, moveIndex)] = clamped;
        }

        public void Reset() => _store.Clear();

        public object Export() => _store.Select(kv => new { state = kv.Key.Item1, moveIndex = kv.Key.Item2, q = kv.Value }).ToArray();

        public void ImportReplace(object doc)
        {
            _store.Clear();
            // Expect array of { state, moveIndex, q } as JsonElement
            if (doc is System.Text.Json.JsonElement el && el.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var item in el.EnumerateArray())
                {
                    var state = item.GetProperty("state").GetString()!;
                    var moveIndex = item.GetProperty("moveIndex").GetInt32();
                    var q = item.GetProperty("q").GetDouble();
                    _store[(state, moveIndex)] = Math.Max(_min, Math.Min(_max, q));
                }
            }
        }
    }
}
