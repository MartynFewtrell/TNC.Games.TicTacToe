namespace Tnc.Games.TicTacToe.Shared;

public enum CellValue
{
    E,
    X,
    O
}

public enum GameStatus
{
    InProgress,
    WinX,
    WinO,
    Draw
}

public record TurnRequest(int Move, TurnOptions? Options);

public record TurnOptions(string Mode = "HvsAI", string Starter = "Human", string HumanSymbol = "X");

// Added WinningLine (indices 0-based) at the end for UI highlighting
public record TurnResponse(string SessionId, string[] Board, string? NextPlayer, string Status, int MoveCount, MovesApplied MovesApplied, int[]? WinningLine);

public record MovesApplied(int? Human, int? Ai);

public record StateRequest(string SessionId);

public record StateResponse(string SessionId, string[] Board, string? NextPlayer, string Status, int MoveCount, string HumanSymbol, int[]? WinningLine);

public record SelfPlayRequest(int N, int? Seed);

public record SelfPlayResponse(int Requested, int Played, int WinsX, int WinsO, int Draws, object AvgMovesPerResult);

public record RankingsExport(Dictionary<string, double> Rankings);
