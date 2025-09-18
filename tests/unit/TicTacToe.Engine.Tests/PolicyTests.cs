using System;
using System.Linq;
using Tnc.Games.TicTacToe.Api.Domain;
using Tnc.Games.TicTacToe.Api.Infrastructure;
using Xunit;

namespace TicTacToe.Engine.Tests;

public class PolicyTests
{
    [Fact]
    public void EpsilonGreedy_Explores()
    {
        var store = new RankingStoreMemory();
        var rng = new Random(123);
        var policy = new Policy(1.0, rng); // epsilon=1 should always explore
        var legal = new int[] {0,1,2};
        var move = policy.SelectMove("EEEEEEEEE", legal, store);
        Assert.Contains(move, legal);
    }

    [Fact]
    public void EpsilonGreedy_Exploits()
    {
        var store = new RankingStoreMemory();
        store.Set("EEEEEEEEE", 1, 1.0);
        store.Set("EEEEEEEEE", 2, 0.5);
        var rng = new Random(42);
        var policy = new Policy(0.0, rng); // epsilon=0 always exploit
        var legal = new int[] {0,1,2};
        var move = policy.SelectMove("EEEEEEEEE", legal, store);
        Assert.Equal(1, move);
    }
}
