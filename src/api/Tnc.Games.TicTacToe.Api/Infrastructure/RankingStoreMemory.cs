using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tnc.Games.TicTacToe.Api.Domain;
using Tnc.Games.TicTacToe.Shared;

namespace Tnc.Games.TicTacToe.Api.Infrastructure
{
    public class RankingStoreMemory : IRankingStore
    {
        private readonly ConcurrentDictionary<(string, int), double> _store = new();
        private readonly double _min = -5.0;
        private readonly double _max = 5.0;
        private readonly bool _compatProbeOnRead;
        private readonly ILogger<RankingStoreMemory>? _logger;

        public RankingStoreMemory(bool compatProbeOnRead = false, ILogger<RankingStoreMemory>? logger = null)
        {
            _compatProbeOnRead = compatProbeOnRead;
            _logger = logger;
        }

        public double? Get(string stateKey, int moveIndex)
        {
            if (_store.TryGetValue((stateKey, moveIndex), out var v)) return v;

            if (!_compatProbeOnRead) return null;

            // Basic validation: ensure key looks sane before attempting probe
            if (string.IsNullOrEmpty(stateKey) || stateKey.Length != 9)
            {
                _logger?.LogDebug("RankingStoreMemory.Get: skipping compat probe for invalid stateKey='{stateKey}'", stateKey);
                return null;
            }

            // Probe symmetric variants (compatibility mode). If a matching value is found under a symmetric key/move,
            // re-store it under the requested canonical key for future fast reads.
            try
            {
                var board = BoardEncoding.FromStateKey(stateKey);
                foreach (Domain.Transform t in System.Enum.GetValues(typeof(Domain.Transform)))
                {
                    // compute inverse transform
                    var inv = Inverse(t);
                    var variantBoard = Domain.Symmetry.ApplyTransform(board, inv);
                    var variantKey = BoardEncoding.ToStateKey(variantBoard);
                    var variantMove = Domain.Symmetry.MapMoveIndex(moveIndex, inv);
                    if (_store.TryGetValue((variantKey, variantMove), out var vv))
                    {
                        // re-store under canonical key
                        _store[(stateKey, moveIndex)] = vv;
                        _logger?.LogInformation("RankingStoreMemory: migrated value from variantKey={variantKey},variantMove={variantMove} to canonical {stateKey},{moveIndex}", variantKey, variantMove, stateKey, moveIndex);
                        return vv;
                    }
                }
            }
            catch (System.ArgumentException argEx)
            {
                // BoardEncoding/MapMoveIndex may throw for malformed input
                _logger?.LogDebug(argEx, "RankingStoreMemory compatibility probe failed due to argument issue for stateKey={stateKey}", stateKey);
            }
            catch (System.IndexOutOfRangeException ioEx)
            {
                _logger?.LogDebug(ioEx, "RankingStoreMemory compatibility probe failed due to index issue for stateKey={stateKey}", stateKey);
            }
            catch (System.Exception ex)
            {
                // Unexpected error: log at warning level but preserve previous behavior by returning null
                _logger?.LogWarning(ex, "RankingStoreMemory compatibility probe encountered unexpected error for stateKey={stateKey}", stateKey);
            }

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

        private static Domain.Transform Inverse(Domain.Transform t)
        {
            return t switch
            {
                Domain.Transform.Identity => Domain.Transform.Identity,
                Domain.Transform.Rot90 => Domain.Transform.Rot270,
                Domain.Transform.Rot180 => Domain.Transform.Rot180,
                Domain.Transform.Rot270 => Domain.Transform.Rot90,
                Domain.Transform.FlipH => Domain.Transform.FlipH,
                Domain.Transform.FlipV => Domain.Transform.FlipV,
                Domain.Transform.FlipMainDiag => Domain.Transform.FlipMainDiag,
                Domain.Transform.FlipAntiDiag => Domain.Transform.FlipAntiDiag,
                _ => Domain.Transform.Identity
            };
        }
    }
}
