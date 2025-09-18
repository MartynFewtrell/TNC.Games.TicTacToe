namespace Tnc.Games.TicTacToe.Api.Engine;

public enum Cell { E = 0, X = 1, O = 2 }
public enum Player { X = 1, O = 2 }
public enum GameStatus { InProgress, WinX, WinO, Draw }

public class GameState
{
    public Cell[] Board { get; set; }
    public Player NextPlayer { get; set; }
    public GameStatus Status { get; set; }
    public List<int> MoveHistory { get; set; }

    // Which player is the human: X or O. Defaults to X.
    public Player HumanPlayer { get; set; }

    public GameState()
    {
        Board = Enumerable.Repeat(Cell.E, 9).ToArray();
        NextPlayer = Player.X;
        Status = GameStatus.InProgress;
        MoveHistory = new List<int>();
        HumanPlayer = Player.X;
    }

    public GameState(Cell[] board, Player nextPlayer, GameStatus status, List<int> moveHistory)
    {
        Board = board;
        NextPlayer = nextPlayer;
        Status = status;
        MoveHistory = moveHistory;
        HumanPlayer = Player.X;
    }
}

public static class Rules
{
    public static readonly int[][] Lines = new[] {
        new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8},
        new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8},
        new[] {0,4,8}, new[] {2,4,6}
    };

    public static bool IsLegal(GameState state, int index)
    {
        if (index < 0 || index >= state.Board.Length) return false;
        return state.Status == GameStatus.InProgress && state.Board[index] == Cell.E;
    }

    public static void ApplyMove(GameState state, int index, Player player)
    {
        if (index < 0 || index >= state.Board.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Move index must be between 0 and 8");

        state.Board[index] = player == Player.X ? Cell.X : Cell.O;
        state.MoveHistory.Add(index);
        // switch player
        state.NextPlayer = player == Player.X ? Player.O : Player.X;

        // Check win
        foreach (var line in Lines)
        {
            if (state.Board[line[0]] != Cell.E && state.Board[line[0]] == state.Board[line[1]] && state.Board[line[1]] == state.Board[line[2]])
            {
                state.Status = state.Board[line[0]] == Cell.X ? GameStatus.WinX : GameStatus.WinO;
                return;
            }
        }

        // Check draw
        if (state.Board.All(c => c != Cell.E))
        {
            state.Status = GameStatus.Draw;
        }
    }

    public static string[] ToBoardStrings(GameState state)
    {
        return state.Board.Select(c => c == Cell.X ? "X" : c == Cell.O ? "O" : "E").ToArray();
    }

    // Return the winning line indices (0-based) if there is a winner, otherwise null
    public static int[]? GetWinningLine(GameState state)
    {
        foreach (var line in Lines)
        {
            var a = state.Board[line[0]];
            if (a == Cell.E) continue;
            if (state.Board[line[1]] == a && state.Board[line[2]] == a)
            {
                return line.ToArray();
            }
        }
        return null;
    }
}
