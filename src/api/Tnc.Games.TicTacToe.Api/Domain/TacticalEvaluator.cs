using System;
using System.Linq;
using Tnc.Games.TicTacToe.Api.Engine;
using Tnc.Games.TicTacToe.Shared;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    public static class TacticalEvaluator
    {
        // Returns true and sets move if current player has a win-in-1 (smallest index). Otherwise false.
        public static bool TryWinIn1(GameState state, out int move)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            var boardStrings = Rules.ToBoardStrings(state);
            var legalMoves = BoardEncoding.GetLegalMoves(boardStrings).OrderBy(x => x).ToArray();
            var currentPlayer = state.NextPlayer;

            foreach (var m in legalMoves)
            {
                var copy = CloneState(state);
                Rules.ApplyMove(copy, m, currentPlayer);
                // Check if this results in a win for current player
                if ((currentPlayer == Player.X && copy.Status == Engine.GameStatus.WinX) ||
                    (currentPlayer == Player.O && copy.Status == Engine.GameStatus.WinO))
                {
                    move = m;
                    return true;
                }
            }

            move = -1;
            return false;
        }

        // Returns true and sets move if there exists a move that blocks opponent immediate wins.
        // Selects the smallest-index move that prevents all opponent immediate wins.
        public static bool TryBlockIn1(GameState state, out int move)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            var boardStrings = Rules.ToBoardStrings(state);
            var legalMoves = BoardEncoding.GetLegalMoves(boardStrings).OrderBy(x => x).ToArray();
            var currentPlayer = state.NextPlayer;
            var opponent = currentPlayer == Player.X ? Player.O : Player.X;

            // For each candidate move m, simulate applying m and check if opponent has any immediate win on next move.
            // If opponent has no immediate winning moves after m, m is a valid block.
            foreach (var m in legalMoves)
            {
                var copy = CloneState(state);
                Rules.ApplyMove(copy, m, currentPlayer);
                // If game already ended (win/draw) after our move, then it prevents opponent wins
                if (copy.Status != Engine.GameStatus.InProgress)
                {
                    move = m;
                    return true;
                }

                var oppBoardStrings = Rules.ToBoardStrings(copy);
                var oppLegal = BoardEncoding.GetLegalMoves(oppBoardStrings);
                bool opponentHasImmediateWin = false;
                foreach (var om in oppLegal)
                {
                    var copy2 = CloneState(copy);
                    Rules.ApplyMove(copy2, om, opponent);
                    if ((opponent == Player.X && copy2.Status == Engine.GameStatus.WinX) ||
                        (opponent == Player.O && copy2.Status == Engine.GameStatus.WinO))
                    {
                        opponentHasImmediateWin = true;
                        break;
                    }
                }

                if (!opponentHasImmediateWin)
                {
                    move = m;
                    return true;
                }
            }

            move = -1;
            return false;
        }

        private static GameState CloneState(GameState src)
        {
            return new GameState((Cell[])src.Board.Clone(), src.NextPlayer, src.Status, new System.Collections.Generic.List<int>(src.MoveHistory))
            {
                HumanPlayer = src.HumanPlayer
            };
        }
    }
}
