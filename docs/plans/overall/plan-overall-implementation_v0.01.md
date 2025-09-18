# Implementation Plan

Based on: `spec-system-overview_v0.01.md`, `spec-api-functional_v0.01.md`, `spec-engine-ai-technical_v0.01.md`, `spec-ui-functional_v0.01.md`, `spec-persistence-technical_v0.01.md`, `spec-observability-technical_v0.01.md`
Version: v0.01 (Draft)

## Project Setup & Structure
- [x] Work Item 1: Repository and solution structure
  - [x] Task 1: Establish folder layout and solution
    - [x] Step 1: Create folders: `/src/api`, `/src/web`, `/src/shared`, `/src/workers` (reserved), `/docs`, `/tests`.
    - [x] Step 2: Initialize .NET solution and projects (class libraries where needed).
    - [x] Step 3: Add project references: `api` -> `shared`.
  - [x] Task 2: Common props and analyzers
    - [x] Step 1: Add a `Directory.Build.props` enabling nullable, LangVersion=preview (if needed), TreatWarningsAsErrors for CI.
    - [x] Step 2: Add `stylecop` or Roslyn analyzers (optional for v1).
  - **Files**:
    - `/Directory.Build.props`: Common compilation settings.
  - **Work Item Dependencies**: None
  - **User Instructions**: Use .NET 9 SDK; commit the scaffolding.

- [x] Work Item 2: Shared contracts and utilities
  - [x] Task 1: Define API contracts (DTOs)
    - [x] Step 1: `BoardCell` string values ("X","O","E") and `GameStatus` enum in shared.
    - [x] Step 2: Request/response DTOs for `/turn`, `/state`, `/selfplay`, admin endpoints.
  - [x] Task 2: Keypad mapping utilities and board helpers
    - [x] Step 1: Implement keypad 1-9 <-> index 0-8 mapping.
    - [x] Step 2: Implement state encoding to 9-char `X/O/E` string and array-of-strings conversions.
  - **Files**:
    - `/src/shared/Tnc.Games.TicTacToe.Shared/Contracts.cs`
    - `/src/shared/Tnc.Games.TicTacToe.Shared/BoardEncoding.cs`
  - **Work Item Dependencies**: Work Item 1
  - **User Instructions**: Keep shared project as `net9.0` class library.

## Game Engine & AI (Backend Domain)
- [x] Work Item 3: Core domain model and rules engine
  - [x] Task 1: Domain types
    - [x] Step 1: `Cell` enum {E,X,O}, `Player` enum {X,O}.
    - [x] Step 2: `GameState` record with `board: Cell[9]`, `nextPlayer`, `status`, `history` (moves with state snapshots).
  - [x] Task 2: Rule enforcement and detection
    - [x] Step 1: Implement legal-move check, apply-move, win lines, draw detection.
    - [x] Step 2: Convert between engine `Cell[]` and API string board.
  - **Files**:
    - `/src/api/Tnc.Games.TicTacToe.Api/Engine/Rules.cs` (implements domain enums, `GameState` and `Rules` methods)
  - **Work Item Dependencies**: Work Item 2

- [x] Work Item 4: Q-table policy and learning
  - [x] Task 1: Q-table store interface and in-memory impl
    - [x] Step 1: Define `IRankingStore` (per spec persistence) in `api` domain.
    - [x] Step 2: Implement `RankingStoreMemory` with clamp [-5,+5].
  - [x] Task 2: Policy selection (epsilon-greedy)
    - [x] Step 1: Implement epsilon=0.15; argmax with random tie-break; random fallback if no Q present.
  - [x] Task 3: End-of-game update algorithm
    - [x] Step 1: For each (state, move) in game history, compute R: win=+1, loss=-1, draw=c*(4.5-moves)/4.5 with c=0.2.
    - [x] Step 2: Apply `Q <- clamp(Q + alpha*R, -5, +5)` with alpha=0.1.
  - **Files**:
    - `/src/api/Domain/IRankingStore.cs`
    - `/src/api/Infrastructure/RankingStoreMemory.cs`
    - `/src/api/Domain/Policy.cs`
    - `/src/api/Domain/Learning.cs`
  - **Work Item Dependencies**: Work Item 3

- [x] Work Item 5: Session store (in-memory)
  - [x] Task 1: Define `ISessionStore`
    - [x] Step 1: Interface: `TryGet`, `Create`, `Update`, `Delete`.
  - [x] Task 2: Implement memory store
    - [x] Step 1: Backed by `ConcurrentDictionary<Guid, GameState>`.
  - **Files**:
    - `/src/api/Domain/ISessionStore.cs`
    - `/src/api/Infrastructure/SessionStoreMemory.cs`
  - **Work Item Dependencies**: Work Item 3

## Backend API (Minimal API)
- [x] Work Item 6: API host and configuration
  - [x] Task 1: Minimal API project scaffolding
    - [x] Step 1: Create ASP.NET Core `net9.0` minimal API.
    - [x] Step 2: Configure CORS (dev: allow any; non-prod/prod: configured origins).
    - [x] Step 3: Configure OpenTelemetry (logs, metrics, traces) and ActivitySource. (no-op wiring in this env)
    - [x] Step 4: Configure Swagger only in non-prod.
  - [x] Task 2: DI registration
    - [x] Step 1: Register `ISessionStore`, `IRankingStore` memory implementations.
    - [x] Step 2: Register domain services `Rules`, `Policy`, `Learning`.
  - **Files**:
    - `/src/api/Program.cs`
    - `/src/api/Telemetry/TelemetryExtensions.cs`
  - **Work Item Dependencies**: Work Items 3–5

- [x] Work Item 7: Gameplay endpoints
  - [x] Task 1: POST /api/v1/turn
    - [x] Step 1: Parse `sessionId` (optional) and request body.
    - [x] Step 2: If no session -> create with defaults/options; else load state.
    - [x] Step 3: Validate human move; apply; if AI to move and game not over -> select and apply one AI move.
    - [x] Step 4: Return sessionId, board, nextPlayer, status, movesApplied, moveCount.
    - [x] Step 5: On invalid -> return 400 with standardized code/message and include board & legal moves.
  - [x] Task 2: GET /api/v1/state
    - [x] Step 1: Load session by id; return state or 404.
  - **Files**:
    - `/src/api/Endpoints/GameplayEndpoints.cs`
  - **Work Item Dependencies**: Work Item 6

- [x] Work Item 8: Self-play endpoint
  - [x] Task 1: POST /api/v1/selfplay
    - [x] Step 1: Accept `n` (cap 10k) and optional `seed`.
    - [x] Step 2: Run sequential self-play games; update Q-table per game.
    - [x] Step 3: Aggregate and return summary.
  - **Files**:
    - `/src/api/Endpoints/SelfPlayEndpoints.cs`
  - **Work Item Dependencies**: Work Items 4, 6

- [x] Work Item 9: Admin endpoints (Basic auth)
  - [x] Task 1: Basic auth middleware/config
    - [x] Step 1: Implement simple Basic auth handler with in-memory credentials via config.
  - [x] Task 2: Reset/Export/Import/Stats
    - [x] Step 1: `POST /admin/rankings/reset` -> `IRankingStore.Reset()`.
    - [x] Step 2: `GET /admin/rankings/export` -> JSON from store.
    - [x] Step 3: `POST /admin/rankings/import` -> replace from uploaded JSON.
    - [x] Step 4: `GET /admin/stats` -> compute or fetch from stats store.
  - **Files**:
    - `/src/api/Security/BasicAuthHandler.cs`
    - `/src/api/Endpoints/AdminEndpoints.cs` (endpoints mapped in Program.cs)
  - **Work Item Dependencies**: Work Items 4, 6

## Observability
- [x] Work Item 10: OpenTelemetry setup (partial -> complete)
  - [x] Task 1: Logging, Metrics, Traces
    - [x] Step 1: Add OTLP exporters configurable by environment. (placeholder)
    - [x] Step 2: Emit counters and histograms per spec. (basic counters added)
  - [x] Task 2: Span attributes
    - [x] Step 1: Add `sessionId`, `moveIndex`, `aiMoveIndex`, `status` attributes to spans. (implemented on endpoints)
    - [x] **Files**:
    - `/src/api/Telemetry/TelemetryExtensions.cs` (ActivitySource and Meter registered)
  - **Work Item Dependencies**: Work Item 6

## UI Web App (Blazor Web App)
- [x] Work Item 11: Blazor project scaffolding
    - [x] Task 1: Create .NET 9 Blazor Web App (SSR + interactive server)
      - [x] Step 1: Setup base route(s), shared layout, Bootstrap theme.
      - [x] Step 2: Configure API base URL per environment.
    - **Files**:
      - `/src/web/Tnc.Games.TicTacToe.Web/Program.cs`
      - `/src/web/Tnc.Games.TicTacToe.Web/App.razor`
      - `/src/web/Tnc.Games.TicTacToe.Web/Shared/MainLayout.razor`
    - Notes: Replaced default Index with a TicTacToe scaffold that calls `/api/v1/turn` and displays the board. Project updated to `net9.0` and references shared DTOs.

- [x] Work Item 12: Board and gameplay UI
  - [x] Task 1: `BoardGrid` component
    - [x] Step 1: Render 3x3, show `X/O/E`, map keypad 1-9 labels.
    - [x] Step 2: Disable when not human turn or finished; highlight legal moves on error response. (implemented)
  - [x] Task 2: `GameStatus` and controls
    - [x] Step 1: Display status, next player, move count. (implemented)
    - [x] Step 2: Starter/symbol selectors, New Game. (implemented)
  - [x] Task 3: API integration
    - [x] Step 1: Call `/turn` and `/state`; handle 400/404; carry `sessionId` in query. (basic wiring in Index.razor)
    - **Files**:
    - `/src/web/.../Components/BoardGrid.razor`
    - `/src/web/.../Components/GameStatus.razor`
    - `/src/web/.../Pages/Index.razor`
  - **Work Item Dependencies**: Work Items 6–7

- [x] Work Item 13: Self-play UI and Admin panel
  - [x] Task 1: Self-play form
    - [x] Step 1: Inputs for `n` and optional `seed`; call `/selfplay`; show summary. (Training.razor created)
  - [x] Task 2: Admin panel (behind Basic auth prompt)
    - [x] Step 1: Buttons to Reset, Export (download JSON), Import (upload JSON), and view Stats. (Admin.razor created; uses per-request basic auth header)
  - **Files**:
    - `/src/web/.../Pages/Admin.razor` (scaffolded)
    - `/src/web/.../Pages/Training.razor` (scaffolded)
  - **Work Item Dependencies**: Work Items 8–9

## Testing
- [x] Work Item 14: Unit tests for engine and policy
  - [x] Task 1: Rules and detection tests
    - [x] Step 1: Win lines, draw detection, legal moves, keypad mapping.
  - [x] Task 2: Policy and learning
    - [x] Step 1: Epsilon-greedy behavior; tie-break randomness; Q update and clamping.
    - **Files**:
    - `/tests/unit/TicTacToe.Engine.Tests/RulesTests.cs`
    - `/tests/unit/TicTacToe.Engine.Tests/PolicyTests.cs`
  - **Work Item Dependencies**: Work Items 3–4

- [x] Work Item 15: Integration tests for API
  - [x] Task 1: Turn flow
    - [x] Step 1: Create session implicitly; apply human and AI move; validate responses.
  - [x] Task 2: Error cases
    - [x] Step 1: Invalid move (occupied), wrong turn, finished game -> 400 shape validated.
    - **Files**:
    - `/tests/integration/TicTacToe.Api.Tests/TurnFlowTests.cs`
    - `/tests/integration/TicTacToe.Api.Tests/NewSessionAndTurnTests.cs`
    - `/tests/integration/TicTacToe.Api.Tests/AdminEndpointsTests.cs`
    - **Work Item Dependencies**: Work Items 6–7

## DevOps & Config
- [x] Work Item 16: Environment configuration and CORS (partial)
  - [x] Task 1: App settings per environment
    - [x] Step 1: `appsettings.Development.json` (allow any CORS), `appsettings.Staging/Production.json` (allowed origins list). (created)
  - **Files**:
    - `/src/api/appsettings.{Environment}.json`
  - **Work Item Dependencies**: Work Item 6

- [x] Work Item 17: Swagger configuration gating
  - [x] Task 1: Enable Swagger in non-prod only
    - [x] Step 1: Add conditional registration based on environment.
    - **Files**:
      - `/src/api/Program.cs`
  - **Work Item Dependencies**: Work Item 6

---

Summary
- Incrementally build shared contracts, domain engine, and in-memory stores; host minimal API with required endpoints and admin ops; add OpenTelemetry; then deliver Blazor UI for gameplay, self-play, and admin. Finish with tests and environment config. This aligns with the v0.01 specs and keeps future persistence swaps and scale-out feasible.
