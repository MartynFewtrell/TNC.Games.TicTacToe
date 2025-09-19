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
                var copy = state.Clone();
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
                var copy = state.Clone();
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
                    var copy2 = copy.Clone();
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

        private static RevertInfo ApplyMoveRecorded(GameState state, int index, Player player)
        {
            // Record previous values to enable revert
            var prevCell = state.Board[index];
            var prevStatus = state.Status;
            var prevNext = state.NextPlayer;
            var prevCount = state.MoveHistory?.Count ?? 0;

            Rules.ApplyMove(state, index, player);

            return new RevertInfo(prevCell, prevStatus, prevNext, prevCount);
        }

        private static void RevertMove(GameState state, int index, RevertInfo info)
        {
            // revert board cell
            state.Board[index] = info.PrevCell;
            // revert next player
            state.NextPlayer = info.PrevNext;
            // revert status
            state.Status = info.PrevStatus;
            // revert move history
            if (state.MoveHistory != null)
            {
                while (state.MoveHistory.Count > info.PrevHistoryCount)
                {
                    state.MoveHistory.RemoveAt(state.MoveHistory.Count - 1);
                }
            }
        }

        private readonly record struct RevertInfo(Cell PrevCell, Tnc.Games.TicTacToe.Api.Engine.GameStatus PrevStatus, Player PrevNext, int PrevHistoryCount);
    }
}
