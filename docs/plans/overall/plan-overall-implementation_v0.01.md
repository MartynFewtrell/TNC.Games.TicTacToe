# Implementation Plan

Based on: `spec-system-overview_v0.01.md`, `spec-api-functional_v0.01.md`, `spec-engine-ai-technical_v0.01.md`, `spec-ui-functional_v0.01.md`, `spec-persistence-technical_v0.01.md`, `spec-observability-technical_v0.01.md`
Version: v0.01 (Draft)

## Project Setup & Structure
- [ ] Work Item 1: Repository and solution structure
  - [ ] Task 1: Establish folder layout and solution
    - [ ] Step 1: Create folders: `/src/api`, `/src/web`, `/src/shared`, `/src/workers` (reserved), `/docs`, `/tests`.
    - [ ] Step 2: Initialize .NET solution and projects (class libraries where needed).
    - [ ] Step 3: Add project references: `api` -> `shared`.
  - [ ] Task 2: Common props and analyzers
    - [ ] Step 1: Add a `Directory.Build.props` enabling nullable, LangVersion=preview (if needed), TreatWarningsAsErrors for CI.
    - [ ] Step 2: Add `stylecop` or Roslyn analyzers (optional for v1).
  - **Files**:
    - `/Directory.Build.props`: Common compilation settings.
  - **Work Item Dependencies**: None
  - **User Instructions**: Use .NET 9 SDK; commit the scaffolding.

- [ ] Work Item 2: Shared contracts and utilities
  - [ ] Task 1: Define API contracts (DTOs)
    - [ ] Step 1: `BoardCell` string values ("X","O","E") and `GameStatus` enum in shared.
    - [ ] Step 2: Request/response DTOs for `/turn`, `/state`, `/selfplay`, admin endpoints.
  - [ ] Task 2: Keypad mapping utilities and board helpers
    - [ ] Step 1: Implement keypad 1-9 <-> index 0-8 mapping.
    - [ ] Step 2: Implement state encoding to 9-char `X/O/E` string and array-of-strings conversions.
  - **Files**:
    - `/src/shared/Tnc.Games.TicTacToe.Shared/Contracts.cs`
    - `/src/shared/Tnc.Games.TicTacToe.Shared/BoardEncoding.cs`
  - **Work Item Dependencies**: Work Item 1
  - **User Instructions**: Keep shared project as `net9.0` class library.

## Game Engine & AI (Backend Domain)
- [ ] Work Item 3: Core domain model and rules engine
  - [ ] Task 1: Domain types
    - [ ] Step 1: `Cell` enum {E,X,O}, `Player` enum {X,O}.
    - [ ] Step 2: `GameState` record with `board: Cell[9]`, `nextPlayer`, `status`, `history` (moves with state snapshots).
  - [ ] Task 2: Rule enforcement and detection
    - [ ] Step 1: Implement legal-move check, apply-move, win lines, draw detection.
    - [ ] Step 2: Convert between engine `Cell[]` and API string board.
  - **Files**:
    - `/src/api/Domain/Cell.cs`
    - `/src/api/Domain/Player.cs`
    - `/src/api/Domain/GameStatus.cs`
    - `/src/api/Domain/GameState.cs`
    - `/src/api/Domain/Rules.cs`
  - **Work Item Dependencies**: Work Item 2

- [ ] Work Item 4: Q-table policy and learning
  - [ ] Task 1: Q-table store interface and in-memory impl
    - [ ] Step 1: Define `IRankingStore` (per spec persistence) in `api` domain.
    - [ ] Step 2: Implement `RankingStoreMemory` with clamp [-5,+5].
  - [ ] Task 2: Policy selection (epsilon-greedy)
    - [ ] Step 1: Implement epsilon=0.15; argmax with random tie-break; random fallback if no Q present.
  - [ ] Task 3: End-of-game update algorithm
    - [ ] Step 1: For each (state, move) in game history, compute R: win=+1, loss=-1, draw=c*(4.5-moves)/4.5 with c=0.2.
    - [ ] Step 2: Apply `Q <- clamp(Q + alpha*R, -5, +5)` with alpha=0.1.
  - **Files**:
    - `/src/api/Domain/IRankingStore.cs`
    - `/src/api/Infrastructure/RankingStoreMemory.cs`
    - `/src/api/Domain/Policy.cs`
    - `/src/api/Domain/Learning.cs`
  - **Work Item Dependencies**: Work Item 3

- [ ] Work Item 5: Session store (in-memory)
  - [ ] Task 1: Define `ISessionStore`
    - [ ] Step 1: Interface: `TryGet`, `Create`, `Update`, `Delete`.
  - [ ] Task 2: Implement memory store
    - [ ] Step 1: Backed by `ConcurrentDictionary<Guid, GameState>`.
  - **Files**:
    - `/src/api/Domain/ISessionStore.cs`
    - `/src/api/Infrastructure/SessionStoreMemory.cs`
  - **Work Item Dependencies**: Work Item 3

## Backend API (Minimal API)
- [ ] Work Item 6: API host and configuration
  - [ ] Task 1: Minimal API project scaffolding
    - [ ] Step 1: Create ASP.NET Core `net9.0` minimal API.
    - [ ] Step 2: Configure CORS (dev: allow any; non-prod/prod: configured origins).
    - [ ] Step 3: Configure OpenTelemetry (logs, metrics, traces) and ActivitySource.
    - [ ] Step 4: Configure Swagger only in non-prod.
  - [ ] Task 2: DI registration
    - [ ] Step 1: Register `ISessionStore`, `IRankingStore` memory implementations.
    - [ ] Step 2: Register domain services `Rules`, `Policy`, `Learning`.
  - **Files**:
    - `/src/api/Program.cs`
    - `/src/api/Telemetry/Tracing.cs`
  - **Work Item Dependencies**: Work Items 3–5

- [ ] Work Item 7: Gameplay endpoints
  - [ ] Task 1: POST /api/v1/turn
    - [ ] Step 1: Parse `sessionId` (optional) and request body.
    - [ ] Step 2: If no session -> create with defaults/options; else load state.
    - [ ] Step 3: Validate human move; apply; if AI to move and game not over -> select and apply one AI move.
    - [ ] Step 4: Return sessionId, board, nextPlayer, status, movesApplied, moveCount.
    - [ ] Step 5: On invalid -> return 400 with standardized code/message and include board & legal moves.
  - [ ] Task 2: GET /api/v1/state
    - [ ] Step 1: Load session by id; return state or 404.
  - **Files**:
    - `/src/api/Endpoints/GameplayEndpoints.cs`
  - **Work Item Dependencies**: Work Item 6

- [ ] Work Item 8: Self-play endpoint
  - [ ] Task 1: POST /api/v1/selfplay
    - [ ] Step 1: Accept `n` (cap 10k) and optional `seed`.
    - [ ] Step 2: Run sequential self-play games; update Q-table per game.
    - [ ] Step 3: Aggregate and return summary.
  - **Files**:
    - `/src/api/Endpoints/SelfPlayEndpoints.cs`
  - **Work Item Dependencies**: Work Items 4, 6

- [ ] Work Item 9: Admin endpoints (Basic auth)
  - [ ] Task 1: Basic auth middleware/config
    - [ ] Step 1: Implement simple Basic auth handler with in-memory credentials via config.
  - [ ] Task 2: Reset/Export/Import/Stats
    - [ ] Step 1: `POST /admin/rankings/reset` -> `IRankingStore.Reset()`.
    - [ ] Step 2: `GET /admin/rankings/export` -> JSON from store.
    - [ ] Step 3: `POST /admin/rankings/import` -> replace from uploaded JSON.
    - [ ] Step 4: `GET /admin/stats` -> compute or fetch from stats store.
  - **Files**:
    - `/src/api/Security/BasicAuthHandler.cs`
    - `/src/api/Endpoints/AdminEndpoints.cs`
  - **Work Item Dependencies**: Work Items 4, 6

## Observability
- [ ] Work Item 10: OpenTelemetry setup
  - [ ] Task 1: Logging, Metrics, Traces
    - [ ] Step 1: Add OTLP exporters configurable by environment.
    - [ ] Step 2: Emit counters and histograms per spec.
  - [ ] Task 2: Span attributes
    - [ ] Step 1: Add `sessionId`, `moveIndex`, `aiMoveIndex`, `status` attributes to spans.
  - **Files**:
    - `/src/api/Telemetry/TelemetryExtensions.cs`
  - **Work Item Dependencies**: Work Item 6

## UI Web App (Blazor Web App)
- [ ] Work Item 11: Blazor project scaffolding
  - [ ] Task 1: Create .NET 9 Blazor Web App (SSR + interactive server)
    - [ ] Step 1: Setup base route(s), shared layout, Bootstrap theme.
    - [ ] Step 2: Configure API base URL per environment.
  - **Files**:
    - `/src/web/Tnc.Games.TicTacToe.Web/Program.cs`
    - `/src/web/Tnc.Games.TicTacToe.Web/App.razor`
    - `/src/web/Tnc.Games.TicTacToe.Web/Shared/MainLayout.razor`

- [ ] Work Item 12: Board and gameplay UI
  - [ ] Task 1: `BoardGrid` component
    - [ ] Step 1: Render 3x3, show `X/O/E`, map keypad 1-9 labels.
    - [ ] Step 2: Disable when not human turn or finished; highlight legal moves on error response.
  - [ ] Task 2: `GameStatus` and controls
    - [ ] Step 1: Display status, next player, move count.
    - [ ] Step 2: Starter/symbol selectors, New Game.
  - [ ] Task 3: API integration
    - [ ] Step 1: Call `/turn` and `/state`; handle 400/404; carry `sessionId` in query.
  - **Files**:
    - `/src/web/.../Components/BoardGrid.razor`
    - `/src/web/.../Components/GameStatus.razor`
    - `/src/web/.../Pages/Index.razor`
  - **Work Item Dependencies**: Work Items 6–7

- [ ] Work Item 13: Self-play UI and Admin panel
  - [ ] Task 1: Self-play form
    - [ ] Step 1: Inputs for `n` and optional `seed`; call `/selfplay`; show summary.
  - [ ] Task 2: Admin panel (behind Basic auth prompt)
    - [ ] Step 1: Buttons to Reset, Export (download JSON), Import (upload JSON), and view Stats.
  - **Files**:
    - `/src/web/.../Pages/Admin.razor`
    - `/src/web/.../Pages/Training.razor`
  - **Work Item Dependencies**: Work Items 8–9

## Testing
- [ ] Work Item 14: Unit tests for engine and policy
  - [ ] Task 1: Rules and detection tests
    - [ ] Step 1: Win lines, draw detection, legal moves, keypad mapping.
  - [ ] Task 2: Policy and learning
    - [ ] Step 1: Epsilon-greedy behavior; tie-break randomness; Q update and clamping.
  - **Files**:
    - `/tests/unit/TicTacToe.Engine.Tests/RulesTests.cs`
    - `/tests/unit/TicTacToe.Engine.Tests/PolicyTests.cs`
  - **Work Item Dependencies**: Work Items 3–4

- [ ] Work Item 15: Integration tests for API
  - [ ] Task 1: Turn flow
    - [ ] Step 1: Create session implicitly; apply human and AI move; validate responses.
  - [ ] Task 2: Error cases
    - [ ] Step 1: Invalid move (occupied), wrong turn, finished game -> 400 shape validated.
  - **Files**:
    - `/tests/integration/TicTacToe.Api.Tests/TurnFlowTests.cs`
  - **Work Item Dependencies**: Work Items 6–7

## DevOps & Config
- [ ] Work Item 16: Environment configuration and CORS
  - [ ] Task 1: App settings per environment
    - [ ] Step 1: `appsettings.Development.json` (allow any CORS), `appsettings.Staging/Production.json` (allowed origins list).
  - **Files**:
    - `/src/api/appsettings.{Environment}.json`
  - **Work Item Dependencies**: Work Item 6

- [ ] Work Item 17: Swagger configuration gating
  - [ ] Task 1: Enable Swagger in non-prod only
    - [ ] Step 1: Add conditional registration based on environment.
  - **Files**:
    - `/src/api/Program.cs`
  - **Work Item Dependencies**: Work Item 6

---

Summary
- Incrementally build shared contracts, domain engine, and in-memory stores; host minimal API with required endpoints and admin ops; add OpenTelemetry; then deliver Blazor UI for gameplay, self-play, and admin. Finish with tests and environment config. This aligns with the v0.01 specs and keeps future persistence swaps and scale-out feasible.
