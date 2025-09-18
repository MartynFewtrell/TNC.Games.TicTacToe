using System;
using System.Collections.Generic;

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
        public static void UpdateQ(IRankingStore store, IEnumerable<(string stateKey, int move)> history, GameResult result)
        {
            var movesCount = 0;
            foreach (var _ in history) movesCount++;
            int idx = 0;
            foreach (var (stateKey, move) in history)
            {
                idx++;
                var R = ComputeReward(result, idx);
                // delta = alpha * R
                var delta = Alpha * R;
                // Update using import store API: Get current (nullable)
                var current = store.Get(stateKey, move) ?? 0.0;
                var updated = Math.Clamp(current + delta, ClampMin, ClampMax);
                store.Set(stateKey, move, updated);
            }
        }
    }

    public enum GameResult { Win, Loss, Draw }
}
