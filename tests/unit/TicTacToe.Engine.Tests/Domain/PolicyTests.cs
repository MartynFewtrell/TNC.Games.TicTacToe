using System;
using Tnc.Games.TicTacToe.Api.Domain;
using Tnc.Games.TicTacToe.Api.Infrastructure;
using Tnc.Games.TicTacToe.Api.Engine;
using Xunit;

namespace TicTacToe.Engine.Tests.Domain
{
    public class PolicyTests
    {
        [Fact]
        public void DeterministicTieBreak_PrefersSmallestIndex()
        {
            var store = new RankingStoreMemory();
            store.Set("EEEEEEEEE", 0, 1.0);
            store.Set("EEEEEEEEE", 2, 1.0);
            var policy = new Policy(0.0, new Random(42), deterministicTieBreak: true);
            var legal = new int[] { 0, 1, 2 };
            var move = policy.SelectMove("EEEEEEEEE", legal, store);
            Assert.Equal(0, move);
        }

        [Fact]
        public void MissingQ_TreatedAsZero()
        {
            var store = new RankingStoreMemory();
            store.Set("EEEEEEEEE", 1, 0.5);
            var policy = new Policy(0.0, new Random(42), deterministicTieBreak: true);
            var legal = new int[] { 0, 1, 2 };
            var move = policy.SelectMove("EEEEEEEEE", legal, store);
            // with Qs [null,0.5,null] treated as [0,0.5,0] -> choose 1
            Assert.Equal(1, move);
        }

        [Fact]
        public void Tactics_Override_Greedy()
        {
            var store = new RankingStoreMemory();
            // Greedy would pick 0 because set high, but X has win-in-1 at 2
            store.Set("XEXEEEEEE", 0, 1.0);
            var policy = new Policy(0.0, new Random(42), deterministicTieBreak: true);

            // Build state where X to play and can win at 2
            var state = new GameState();
            state.Board[0] = Cell.X;
            state.Board[1] = Cell.X;
            state.NextPlayer = Player.X;

            var move = policy.SelectMove(state, store);
            Assert.Equal(2, move);
        }
    }
}
