using System;

namespace Tnc.Games.TicTacToe.Api.Background;

public enum SelfPlayJobStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}

public class SelfPlayJobResult
{
    public int Requested { get; set; }
    public int Played { get; set; }
    public int WinsX { get; set; }
    public int WinsO { get; set; }
    public int Draws { get; set; }
    public object AvgMovesPerResult { get; set; } = new { avgMoves = 0.0, elapsedMs = 0.0 };
}

public class SelfPlayJob
{
    public Guid Id { get; } = Guid.NewGuid();
    public SelfPlayJobStatus Status { get; set; } = SelfPlayJobStatus.Pending;
    public int Requested { get; set; }
    public int Played { get; set; }
    public int WinsX { get; set; }
    public int WinsO { get; set; }
    public int Draws { get; set; }
    public double? AvgMoves { get; set; }
    public double? ElapsedMs { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
