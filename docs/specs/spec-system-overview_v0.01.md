# Spec: System Overview - Tic Tac Toe

- Version: v0.01 (Draft)
- Status: Draft
- Date: 2025-09-17
- Supersedes: n/a
- Source Prompt: `.github/prompts/spec.research.prompt.md` (template: `docs/specs/spec-template_v1.1.md`)
- Related/Planned Component Specs:
  - `spec-ui-functional_v0.01.md`
  - `spec-api-functional_v0.01.md`
  - `spec-engine-ai-technical_v0.01.md`
  - `spec-persistence-technical_v0.01.md`
  - `spec-observability-technical_v0.01.md`

---

## 1. System Overview

### 1.1 Purpose & Scope
A turn-based Tic Tac Toe game with a separate frontend UI and backend API. The backend owns game rules, state management, and a learning policy that ranks moves based on game outcomes. The system supports human vs computer and computer vs computer (self-play) modes.

### 1.2 Objectives (High-Level)
- Provide a responsive, accessible web UI for playing Tic Tac Toe.
- Expose a clean API for session lifecycle, move application, and game status.
- Centralize game logic and learning policy in the backend.
- Record moves and outcomes to influence future AI decisions.
- Enable batch self-play to accelerate learning.

### 1.3 Out of Scope (for this overview)
- Detailed endpoint contracts, UI flows, or database schemas.
- Authentication for gameplay (admin/ops security is addressed at a high level only).
- Production-grade scale-out of in-memory rankings (deferred until a shared store is added).

### 1.4 Users & Modes
- Human player vs Computer.
- Computer vs Computer (on-demand self-play batches for training and demonstration).

---

## 2. Architecture Overview

### 2.1 Target Architecture & Technology (High-Level)
- UI: Blazor Web App (.NET 8) using SSR with interactive server.
- API: ASP.NET Core minimal API hosted as a separate service.
- Orchestration & Observability: .NET Aspire-ready with OpenTelemetry instrumentation.
- Security (admin/ops): Basic authentication (high-level; details in API spec).
- Hosting: UI and API deployed as separate services; CORS configured appropriately by environment.

### 2.2 Runtime Topology
- UI and API are separate deployables communicating over HTTP(S).
- Single API instance per environment until a shared persistent store is introduced.
- In-memory repositories by default, with a pluggable abstraction to enable future persistence backends.

### 2.3 Data & Control Flows (Conceptual)
- Gameplay
  - The UI creates/continues a session and submits player moves to the API.
  - The API validates, applies rules, may auto-play the computer move, and returns the updated game state.
- Learning
  - The API records moves during the game; at game end, it updates move rankings based on outcome and simple draw weighting.
- Self-Play
  - The UI can request N auto-played games; the API runs a batch and returns aggregate results.
- Admin/Ops
  - Administrative endpoints support ranking reset, export/import, and basic statistics.

### 2.4 Game Engine & AI (Conceptual)
- Engine
  - Board representation, legal move generation, and win/draw detection.
- Policy
  - Ranking-driven move selection that balances exploitation and exploration.
  - Random fallback when no recommendation exists.
- Learning
  - End-of-game updates to rankings with configurable scaling and bounds.

### 2.5 Non-Functional Considerations (High-Level)
- Simplicity first: minimal moving parts, clear separations.
- Extensibility: repository interfaces, componentized engine, and replaceable policy.
- Operability: basic telemetry and environment-specific configuration.

---

## 3. Component Inventory (and Planned Specs)

1) UI Web App (Blazor)
- Role: Presents the game board, captures inputs, displays state/results, triggers self-play.
- Spec: `docs/specs/spec-ui-functional_v0.01.md` (to be authored)

2) Backend API (Minimal API)
- Role: Session lifecycle, move validation/application, AI move generation, results, learning, self-play orchestration, and admin/ops endpoints.
- Spec: `docs/specs/spec-api-functional_v0.01.md` (to be authored)

3) Game Engine & AI Policy
- Role: Board model, rules, legal moves, outcome evaluation, ranking-based policy, and update algorithm at game end.
- Spec: `docs/specs/spec-engine-ai-technical_v0.01.md` (to be authored)

4) Persistence (Pluggable)
- Role: In-memory default for sessions, games, moves, and rankings; interface-based design to allow future storage providers.
- Spec: `docs/specs/spec-persistence-technical_v0.01.md` (to be authored)

5) Observability
- Role: Logging/metrics/traces via OpenTelemetry; environment-driven configuration; Swagger/OpenAPI exposure policy by environment.
- Spec: `docs/specs/spec-observability-technical_v0.01.md` (to be authored)

---

Change Log
- v0.01: Initial draft covering Sections 1-3 (overview and component inventory), aligned to discovery outcomes; references to component specs added.
