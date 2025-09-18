using System;
using System.Linq;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    public class Policy
    {
        private readonly double _epsilon;
        private readonly Random _rng;

        public Policy(double epsilon = 0.15, Random? rng = null)
        {
            if (epsilon < 0 || epsilon > 1) throw new ArgumentOutOfRangeException(nameof(epsilon));
            _epsilon = epsilon;
            _rng = rng ?? new Random();
        }

        /// <summary>
        /// Selects a move using epsilon-greedy policy.
        /// - With probability epsilon returns a random legal move.
        /// - Otherwise returns argmax Q(state,move) with random tie-breaking.
        /// - If no Q entries exist for the legal moves, falls back to a random legal move.
        /// </summary>
        public int SelectMove(string stateKey, int[] legalMoves, IRankingStore rankingStore)
        {
            if (legalMoves == null || legalMoves.Length == 0) throw new ArgumentException("No legal moves provided", nameof(legalMoves));
            // Exploration
            if (_rng.NextDouble() < _epsilon)
            {
                return legalMoves[_rng.Next(legalMoves.Length)];
            }

            // Exploitation: query Q values
            var qValues = legalMoves.Select(m => new { Move = m, Q = rankingStore.Get(stateKey, m) }).ToArray();

            // If none have Q values (all null), fallback to random
            if (qValues.All(x => x.Q == null))
            {
                return legalMoves[_rng.Next(legalMoves.Length)];
            }

            // Treat null as 0.0 for comparison (but presence of at least one non-null ensured above)
            double maxQ = qValues.Max(x => x.Q ?? 0.0);
            var best = qValues.Where(x => (x.Q ?? 0.0) == maxQ).Select(x => x.Move).ToArray();
            return best[_rng.Next(best.Length)];
        }
    }
}
