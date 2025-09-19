using Tnc.Games.TicTacToe.Api.Domain;
using Tnc.Games.TicTacToe.Api.Engine;
using Xunit;

namespace TicTacToe.Engine.Tests.Domain
{
    public class TacticalEvaluatorTests
    {
        [Fact]
        public void TryWinIn1_ReturnsWinningMove()
        {
            // X to play and can win by placing at index 2
            var state = new GameState();
            state.Board[0] = Cell.X;
            state.Board[1] = Cell.X;
            state.NextPlayer = Player.X;

            var ok = TacticalEvaluator.TryWinIn1(state, out var move);
            Assert.True(ok);
            Assert.Equal(2, move);
        }

        [Fact]
        public void TryBlockIn1_ReturnsBlockingMove()
        {
            // O to play but X threatens at index 2; O should block at 2
            var state = new GameState();
            state.Board[0] = Cell.X;
            state.Board[1] = Cell.X;
            state.NextPlayer = Player.O;

            var ok = TacticalEvaluator.TryBlockIn1(state, out var move);
            Assert.True(ok);
            Assert.Equal(2, move);
        }
    }
}
