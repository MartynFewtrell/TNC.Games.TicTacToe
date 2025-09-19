using System;
using System.Collections.Generic;
using System.Linq;
using Tnc.Games.TicTacToe.Shared;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    public enum Transform
    {
        Identity = 0,
        Rot90 = 1,
        Rot180 = 2,
        Rot270 = 3,
        FlipH = 4,
        FlipV = 5,
        FlipMainDiag = 6,
        FlipAntiDiag = 7
    }

    public static class Symmetry
    {
        // Apply transform to a board (string[9] of "X","O","E") and return transformed board
        public static string[] ApplyTransform(string[] board, Transform t)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (board.Length != 9) throw new ArgumentException("Board must be length 9", nameof(board));

            var outBoard = new string[9];
            for (int idx = 0; idx < 9; idx++)
            {
                var (r, c) = IndexToRC(idx);
                (int nr, int nc) = TransformRC(r, c, t);
                int newIdx = nr * 3 + nc;
                outBoard[newIdx] = board[idx];
            }
            return outBoard;
        }

        // Map a move index in the original board to the index in the transformed board
        public static int MapMoveIndex(int moveIndex, Transform t)
        {
            if (moveIndex < 0 || moveIndex > 8) throw new ArgumentOutOfRangeException(nameof(moveIndex));
            var (r, c) = IndexToRC(moveIndex);
            var (nr, nc) = TransformRC(r, c, t);
            return nr * 3 + nc;
        }

        // Given a board (string[9]), compute canonical state key and chosen transform that produces lexicographically smallest key
        public static (string canonicalKey, Transform transform) GetCanonicalKeyAndTransform(string[] board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (board.Length != 9) throw new ArgumentException("Board must be length 9", nameof(board));

            string bestKey = null!;
            Transform bestT = Transform.Identity;

            foreach (Transform t in Enum.GetValues(typeof(Transform)).Cast<Transform>())
            {
                var transformed = ApplyTransform(board, t);
                var key = BoardEncoding.ToStateKey(transformed);
                if (bestKey == null || string.CompareOrdinal(key, bestKey) < 0)
                {
                    bestKey = key;
                    bestT = t;
                }
            }

            return (bestKey, bestT);
        }

        // Helpers
        private static (int r, int c) IndexToRC(int idx) => (idx / 3, idx % 3);

        private static (int r, int c) TransformRC(int r, int c, Transform t)
        {
            return t switch
            {
                Transform.Identity => (r, c),
                Transform.Rot90 => (c, 2 - r),
                Transform.Rot180 => (2 - r, 2 - c),
                Transform.Rot270 => (2 - c, r),
                Transform.FlipH => (r, 2 - c),
                Transform.FlipV => (2 - r, c),
                Transform.FlipMainDiag => (c, r),
                Transform.FlipAntiDiag => (2 - c, 2 - r),
                _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
            };
        }
    }
}
