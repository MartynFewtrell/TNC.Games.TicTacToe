using System.Collections.Concurrent;
using Tnc.Games.TicTacToe.Api.Engine;
using Tnc.Games.TicTacToe.Api.Domain;

namespace Tnc.Games.TicTacToe.Api.Infrastructure
{
    public class InMemorySessionStore : ISessionStore
    {
        private readonly ConcurrentDictionary<Guid, GameState> _store = new();

        public bool TryGet(Guid sessionId, out GameState? state) => _store.TryGetValue(sessionId, out state);

        public Guid Create(GameState state)
        {
            var id = Guid.NewGuid();
            _store[id] = state;
            return id;
        }

        public void Update(Guid sessionId, GameState state) => _store[sessionId] = state;

        public void Delete(Guid sessionId) => _store.TryRemove(sessionId, out _);
    }
}
