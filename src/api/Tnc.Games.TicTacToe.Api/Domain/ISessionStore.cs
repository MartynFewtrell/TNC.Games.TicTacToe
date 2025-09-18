using System;
using Tnc.Games.TicTacToe.Api.Engine;

namespace Tnc.Games.TicTacToe.Api.Domain
{
    public interface ISessionStore
    {
        bool TryGet(Guid sessionId, out GameState? state);
        Guid Create(GameState state);
        void Update(Guid sessionId, GameState state);
        void Delete(Guid sessionId);
    }
}
