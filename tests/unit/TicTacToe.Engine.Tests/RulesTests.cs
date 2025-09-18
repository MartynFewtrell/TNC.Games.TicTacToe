using Tnc.Games.TicTacToe.Api.Engine;
using Xunit;

namespace TicTacToe.Engine.Tests;

public class RulesTests
{
    [Fact]
    public void DetectsWinRow()
    {
        var state = new GameState();
        Rules.ApplyMove(state, 0, Player.X);
        Rules.ApplyMove(state, 1, Player.X);
        Rules.ApplyMove(state, 2, Player.X);
        Assert.Equal(GameStatus.WinX, state.Status);
    }

    [Fact]
    public void DetectsDraw()
    {
        var state = new GameState();
        var moves = new int[] {0,1,2,4,3,5,7,6,8};
        var players = new Player[] { Player.X, Player.O };
        int p = 0;
        foreach (var m in moves)
        {
            Rules.ApplyMove(state, m, players[p]);
            p = 1 - p;
        }
        Assert.Equal(GameStatus.Draw, state.Status);
    }

    [Fact]
    public void LegalMovesDetected()
    {
        var state = new GameState();
        Assert.True(Rules.IsLegal(state, 0));
        Rules.ApplyMove(state, 0, Player.X);
        Assert.False(Rules.IsLegal(state, 0));
    }

    [Fact]
    public void NewGame_Defaults_HumanIsX()
    {
        var state = new GameState();
        Assert.Equal(Player.X, state.HumanPlayer);
        Assert.Equal(Player.X, state.NextPlayer);
        Assert.Equal(GameStatus.InProgress, state.Status);
        Assert.All(state.Board, c => Assert.Equal(Cell.E, c));
    }

    [Fact]
    public void ApplyMove_SetsCellAndSwitchesNextPlayer()
    {
        var state = new GameState();
        Rules.ApplyMove(state, 0, Player.X);
        Assert.Equal(Cell.X, state.Board[0]);
        Assert.Equal(Player.O, state.NextPlayer);
        Assert.Single(state.MoveHistory);
    }

    [Fact]
    public void ApplyMove_HumanOAndAiXWorks()
    {
        var state = new GameState();
        state.HumanPlayer = Player.O;
        state.NextPlayer = Player.O;

        // Human (O) moves
        Rules.ApplyMove(state, 0, Player.O);
        Assert.Equal(Cell.O, state.Board[0]);
        // Next should be X (AI)
        Assert.Equal(Player.X, state.NextPlayer);

        // AI (X) moves
        Rules.ApplyMove(state, 1, Player.X);
        Assert.Equal(Cell.X, state.Board[1]);
    }
}
