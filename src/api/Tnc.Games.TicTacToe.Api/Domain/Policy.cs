using System;
using System.Linq;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Tnc.Games.TicTacToe.Api.Engine;
using Tnc.Games.TicTacToe.Shared;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    public class Policy
    {
        private readonly double _epsilon;
        private readonly Random _rng;
        private readonly bool _deterministicTieBreak;
        private readonly ILogger<Policy>? _logger;
        private readonly Meter? _meter;

        public Policy(double epsilon = 0.15, Random? rng = null, bool deterministicTieBreak = false, ILogger<Policy>? logger = null, Meter? meter = null)
        {
            if (epsilon < 0 || epsilon > 1) throw new ArgumentOutOfRangeException(nameof(epsilon));
            _epsilon = epsilon;
            _rng = rng ?? new Random();
            _deterministicTieBreak = deterministicTieBreak;
            _logger = logger;
            _meter = meter;
        }

        /// <summary>
        /// Selects a move using the runtime policy described in the spec:
        /// 1) Tactical shortcuts (win-in-1, block-in-1) performed on original orientation.
        /// 2) Greedy selection with symmetry canonicalization: canonicalize state, map moves to canonical orientation for Q lookup, treat null as 0.0.
        /// 3) Deterministic tie-break by smallest original move index when enabled.
        /// Exploration (epsilon) applies only after tactical checks; deterministic exploration picks smallest index when enabled.
        /// </summary>
        public int SelectMove(GameState state, IRankingStore rankingStore)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (rankingStore == null) throw new ArgumentNullException(nameof(rankingStore));

            // Metrics: optional counter
            var tacticalCounter = _meter?.CreateCounter<long>("ai_tactical_shortcuts_total");

            // 1) Tactical shortcuts on original orientation
            if (TacticalEvaluator.TryWinIn1(state, out var winMove))
            {
                _logger?.LogDebug("Policy: win-in-1 found at {move}", winMove);
                tacticalCounter?.Add(1, new KeyValuePair<string, object?>("type", "win"));
                return winMove;
            }

            if (TacticalEvaluator.TryBlockIn1(state, out var blockMove))
            {
                _logger?.LogDebug("Policy: block-in-1 found at {move}", blockMove);
                tacticalCounter?.Add(1, new KeyValuePair<string, object?>("type", "block"));
                return blockMove;
            }

            // 2) Exploration (epsilon) - after tactical checks
            var boardStrings = Rules.ToBoardStrings(state);
            var legalMoves = BoardEncoding.GetLegalMoves(boardStrings);
            if (legalMoves == null || legalMoves.Length == 0) throw new ArgumentException("No legal moves available", nameof(legalMoves));

            if (_rng.NextDouble() < _epsilon)
            {
                var chosen = _deterministicTieBreak ? legalMoves.OrderBy(x => x).First() : legalMoves[_rng.Next(legalMoves.Length)];
                _logger?.LogDebug("Policy: exploration chose {move}", chosen);
                return chosen;
            }

            // 3) Greedy with canonicalization
            var (canonicalKey, transform) = Symmetry.GetCanonicalKeyAndTransform(boardStrings);

            double bestQ = double.NegativeInfinity;
            int bestMove = legalMoves.OrderBy(m => m).First(); // default deterministic fallback

            foreach (var m in legalMoves)
            {
                var mc = Symmetry.MapMoveIndex(m, transform);
                var qNullable = rankingStore.Get(canonicalKey, mc);
                var q = qNullable ?? 0.0;
                if (q > bestQ)
                {
                    bestQ = q;
                    bestMove = m;
                }
                else if (q == bestQ)
                {
                    // deterministic tie-break: prefer smallest original index
                    if (m < bestMove)
                    {
                        bestMove = m;
                    }
                }
            }

            _logger?.LogDebug("Policy: selected {move} with bestQ={q} transform={transform}", bestMove, bestQ, transform);
            return bestMove;
        }

        // Back-compat: legacy epsilon-greedy over provided stateKey and legalMoves (no tactics/canonicalization)
        public int SelectMove(string stateKey, int[] legalMoves, IRankingStore rankingStore)
        {
            if (stateKey == null) throw new ArgumentNullException(nameof(stateKey));
            if (legalMoves == null || legalMoves.Length == 0) throw new ArgumentException("No legal moves provided", nameof(legalMoves));
            if (rankingStore == null) throw new ArgumentNullException(nameof(rankingStore));

            // Exploration
            if (_rng.NextDouble() < _epsilon)
            {
                if (_deterministicTieBreak)
                {
                    return legalMoves.OrderBy(x => x).First();
                }
                return legalMoves[_rng.Next(legalMoves.Length)];
            }

            // Exploitation: query Q values
            var qValues = legalMoves.Select(m => new { Move = m, Q = rankingStore.Get(stateKey, m) }).ToArray();

            // If none have Q values (all null), fallback to random
            if (qValues.All(x => x.Q == null))
            {
                if (_deterministicTieBreak) return legalMoves.OrderBy(x => x).First();
                return legalMoves[_rng.Next(legalMoves.Length)];
            }

            // Treat null as 0.0 for comparison
            double maxQ = qValues.Max(x => x.Q ?? 0.0);
            var best = qValues.Where(x => (x.Q ?? 0.0) == maxQ).Select(x => x.Move).ToArray();
            if (_deterministicTieBreak)
            {
                return best.OrderBy(x => x).First();
            }
            return best[_rng.Next(best.Length)];
        }
    }
}
