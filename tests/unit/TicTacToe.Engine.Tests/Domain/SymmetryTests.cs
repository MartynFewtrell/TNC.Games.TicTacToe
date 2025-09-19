using System.Linq;
using Tnc.Games.TicTacToe.Api.Domain;
using Tnc.Games.TicTacToe.Shared;
using Xunit;

namespace TicTacToe.Engine.Tests.Domain
{
    public class SymmetryTests
    {
        [Fact]
        public void MapMoveIndex_MatchesApplyTransform()
        {
            // For each transform and each index, applying transform moves marker to MapMoveIndex
            for (int t = 0; t < 8; t++)
            {
                var transform = (Transform)t;
                for (int idx = 0; idx < 9; idx++)
                {
                    var board = Enumerable.Repeat("E", 9).ToArray();
                    board[idx] = "X";
                    var transformed = Symmetry.ApplyTransform(board, transform);
                    var found = System.Array.FindIndex(transformed, s => s == "X");
                    var mapped = Symmetry.MapMoveIndex(idx, transform);
                    Assert.Equal(found, mapped);
                }
            }
        }

        [Fact]
        public void GetCanonicalKeyAndTransform_ReturnsConsistentKey()
        {
            var board = new string[9] { "X", "E", "E", "E", "E", "E", "E", "E", "E" }; // corner
            var (canonicalKey, transform) = Symmetry.GetCanonicalKeyAndTransform(board);
            var transformed = Symmetry.ApplyTransform(board, transform);
            var keyFromTransform = BoardEncoding.ToStateKey(transformed);
            Assert.Equal(canonicalKey, keyFromTransform);

            // Also verify canonicalKey is minimal among all transforms
            var keys = System.Enum.GetValues(typeof(Transform)).Cast<Transform>().Select(tr => BoardEncoding.ToStateKey(Symmetry.ApplyTransform(board, tr))).ToArray();
            var min = keys.OrderBy(k => k, System.StringComparer.Ordinal).First();
            Assert.Equal(min, canonicalKey);
        }
    }
}
