using System;
using System.Collections.Generic;
using Tnc.Games.TicTacToe.Shared;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    public static class Learning
    {
        private const double Alpha = 0.1;
        private const double ClampMin = -5.0;
        private const double ClampMax = 5.0;
        private const double DrawC = 0.2;

        // R computation per spec
        // win: +1, loss: -1, draw: c*(4.5 - moves)/4.5
        public static double ComputeReward(GameResult result, int moves)
        {
            return result switch
            {
                GameResult.Win => 1.0,
                GameResult.Loss => -1.0,
                GameResult.Draw => DrawC * ((4.5 - moves) / 4.5),
                _ => 0.0
            };
        }

        // Update Q-table for each (stateKey, move) with reward R: Q <- clamp(Q + alpha*R, -5, +5)
        // Canonicalizes stateKey and move before applying updates so ranking store always uses canonical keys
        public static void UpdateQ(IRankingStore store, IEnumerable<(string stateKey, int move)> history, GameResult result)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (history == null) throw new ArgumentNullException(nameof(history));

            int idx = 0;
            foreach (var (stateKey, move) in history)
            {
                idx++;
                var R = ComputeReward(result, idx);
                var delta = Alpha * R;

                // Reconstruct board and canonicalize
                var boardStrings = BoardEncoding.FromStateKey(stateKey);
                var (canonicalKey, transform) = Symmetry.GetCanonicalKeyAndTransform(boardStrings);
                var canonicalMove = Symmetry.MapMoveIndex(move, transform);

                var current = store.Get(canonicalKey, canonicalMove) ?? 0.0;
                var updated = Math.Clamp(current + delta, ClampMin, ClampMax);
                store.Set(canonicalKey, canonicalMove, updated);
            }
        }
    }

    public enum GameResult { Win, Loss, Draw }
}
