# Spec: Persistence - Technical

- Version: v0.01 (Draft)
- Status: Draft
- Date: 2025-09-17
- Template: `docs/specs/spec-template_v1.1.md`
- Related: `spec-system-overview_v0.01.md`, `spec-api-functional_v0.01.md`, `spec-engine-ai-technical_v0.01.md`, `spec-ui-functional_v0.01.md`, `spec-observability-technical_v0.01.md`

---

## 1. Goals
- Provide a pluggable persistence abstraction with an in-memory default.
- Store transient sessions and global Q-table rankings; support optional stats accumulation.
- Enable export/import of rankings JSON (replace semantics).

## 2. Abstractions
- `ISessionStore`
  - `TryGet(sessionId)` -> GameState or null
  - `Create(options)` -> (sessionId, initialState)
  - `Update(sessionId, state)`
  - `Delete(sessionId)`
- `IRankingStore`
  - `Get(stateKey, moveIndex)` -> double? (null if missing)
  - `Set(stateKey, moveIndex, value)`
  - `Reset()`
  - `Export()` / `ImportReplace(exportDoc)`
- `IStatsStore` (optional; else compute in-memory on demand)
  - `RecordGame(summary)`
  - `GetSummary()`

## 3. In-Memory Implementations (v1)
- `SessionStoreMemory`: `ConcurrentDictionary<Guid, GameState>`; TTL not enforced (lifetime = process).
- `RankingStoreMemory`: `ConcurrentDictionary<(string stateKey,int move), double>`; bounds enforced at write.
- `StatsStoreMemory`: aggregate counters and moving averages; resettable.

## 4. Data Contracts
- Rankings export format per API spec (§7.2) with `version` field.

## 5. Concurrency & Consistency
- Single API instance: simple lock-free updates acceptable.
- Future providers must ensure atomic `Set` and compatible merge strategy (out of scope v1).

## 6. Configuration
- DI registers memory providers by default.
- Future: add SQLite/EF Core provider behind `IRankingStore` & `ISessionStore`.

## 7. Telemetry
- Emit counters for sessions created, active, expired (if TTL added later), rankings updated, games recorded.

## 8. Open Questions
- Do we want per-environment seed files auto-imported on startup? (future)

---

Change Log
- v0.01: Initial technical persistence spec with in-memory defaults and contracts.
