# System Overview (v1.0)

Supersedes: spec-system-overview_v0.01.md

Version: v1.0 — First stage completed

Overview
- TNC.Games.TicTacToe implements a tic-tac-toe game with a backend API, an in-memory Q-table based learning policy, and a Blazor-based web UI.
- Goals for v1.0: provide a working minimal API, domain engine with rule enforcement, an AI policy with Q-table learning (in-memory), a session store (in-memory), self-play training endpoint, admin operations, and a Blazor UI scaffold.

Architecture
- API: ASP.NET Core minimal API (net9.0)
- Engine: Domain types and rules in `src/api/.../Engine/Rules.cs`
- Shared: DTOs and board encoding in `src/shared`
- Persistence: In-memory stores for sessions and Q-table (memory implementations)
- UI: Blazor Web App (scaffold)
- Tests: Unit tests (engine/policy) and integration tests (basic turn flow)

Completed in v1.0
- Project scaffolding and solution layout
- Domain engine (rules, detection, state model)
- Shared DTOs and board encoding utilities
- `IRankingStore` and `RankingStoreMemory` Q-table implementation
- `Policy` (epsilon-greedy) and `Learning` update algorithms
- `ISessionStore` and `InMemorySessionStore`
- Minimal API with gameplay endpoints (`/api/v1/new`, `/api/v1/turn`, `/api/v1/state`), self-play (`/api/v1/selfplay`) and admin endpoints (reset/export/import/stats)
- Basic auth handler for admin endpoints
- Unit and integration tests covering core scenarios

Constraints & notes
- All stores are in-memory for v1.0 (suitable for dev). Persistence swap planned for future versions.
- Telemetry hooks exist; OTLP/exporter wiring left as a no-op by default due to environment package constraints.

Change log
- v1.0: Consolidated all v0.01 work items implemented and validated via tests. See plans for detailed status.
