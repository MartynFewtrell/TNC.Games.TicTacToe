# Persistence Technical Specification (v1.0)

Supersedes: spec-persistence-technical_v0.01.md

Version: v1.0

Summary
- v1.0 uses in-memory stores for sessions and Q-table (RankingStoreMemory and InMemorySessionStore). These are simple, concurrent implementations suitable for local development and testing.

Stores
- Session store: `ISessionStore` interface, `InMemorySessionStore` using `ConcurrentDictionary<Guid, GameState>`.
- Q-table store: `IRankingStore` interface, `RankingStoreMemory` using `ConcurrentDictionary<(string state, int move), double>`.

Import/Export
- Ranking store supports `Export()` returning serializable structure and `ImportReplace(object doc)` accepting `JsonElement` arrays of `{ state, moveIndex, q }`.

Change log
- v1.0: Implemented in-memory stores and import/export for admin operations. Persistence backing to be added in later versions.
