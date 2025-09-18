# Implementation Plan

Supersedes: plan-overall-implementation_v0.01.md
Version: v1.0

This plan summarizes the work completed for the v1.0 milestone and records remaining TODOs for future releases.

## Summary of completed work (v1.0)
- Project scaffolding and solution established.
- Shared contracts, keypad mapping, and board encoding implemented.
- Core engine (rules, GameState) implemented and tested.
- Q-table store, policy, and learning implemented with in-memory store and self-play endpoint.
- Session store in-memory implemented.
- Minimal API implemented with gameplay endpoints (`/api/v1/new`, `/api/v1/turn`, `/api/v1/state`), self-play and admin endpoints.
- Basic Auth handler and admin endpoints implemented.
- Telemetry hooks added (no-op wiring in this environment).
- Unit and integration tests added and passing.

## Work Items status
- [x] Work Item 1: Repository and solution structure
- [x] Work Item 2: Shared contracts and utilities
- [x] Work Item 3: Core domain model and rules engine
- [x] Work Item 4: Q-table policy and learning
- [x] Work Item 5: Session store (in-memory)
- [x] Work Item 6: API host and DI registration
- [x] Work Item 7: Gameplay endpoints
- [x] Work Item 8: Self-play endpoint
- [x] Work Item 9: Admin endpoints (Basic auth)
- [x] Work Item 14: Unit tests for engine and policy
- [x] Work Item 15: Integration tests for API (basic turn flow + new session) — initial tests added and passing
- [ ] Work Item 10: Observability — full OTLP exporters to be enabled when packages are available
- [ ] Work Item 11–13: UI improvements and Admin panel polish
- [ ] Work Item 16–17: DevOps, CORS config, Swagger gating (stretch goals)

## Next steps (for v1.x)
- Add persistent backing for ranking store (file, DB) and session store.
- Enhance UI: complete board interactions, admin panel, self-play UI.
- Add more integration tests and E2E tests for full flows.
- Re-enable OpenTelemetry exporters and configure collectors for staging/prod.

## Files changed/added for v1.0
- See repository history; main files include:
  - `src/api/Tnc.Games.TicTacToe.Api/Engine/Rules.cs`
  - `src/api/Tnc.Games.TicTacToe.Api/Domain/*` (Policy, Learning, IRankingStore, ISessionStore)
  - `src/api/Tnc.Games.TicTacToe.Api/Infrastructure/*` (RankingStoreMemory, InMemorySessionStore)
  - `src/api/Tnc.Games.TicTacToe.Api/Endpoints/*` (GameplayEndpoints, SelfPlayEndpoints)
  - `src/api/Tnc.Games.TicTacToe.Api/Security/BasicAuthHandler.cs`
  - `tests/unit/*` and `tests/integration/*` test projects

User instructions
- Continue from `main` or create a release branch for v1.0.
- To enable OTLP exporters, add OpenTelemetry package references and wire up exporters in `TelemetryExtensions`.
