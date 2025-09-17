# Spec: UI Web App - Functional

- Version: v0.01 (Draft)
- Status: Draft
- Date: 2025-09-17
- Template: `docs/specs/spec-template_v1.1.md`
- Related: `spec-system-overview_v0.01.md`, `spec-api-functional_v0.01.md`, `spec-engine-ai-technical_v0.01.md`, `spec-persistence-technical_v0.01.md`, `spec-observability-technical_v0.01.md`

---

## 1. Objectives & Scope
- Deliver a responsive Blazor Web App (SSR + interactive server) providing gameplay and training controls.
- Keep client logic thin; rely on API for rules, moves, results.
- Non-goals: user authentication for gameplay, offline mode.

## 2. UX Flows (High-Level)
- Home/Play
  - Choose mode: Human vs Computer, Computer vs Computer (self-play)
  - Choose starter and symbol when applicable
  - Board renders 3x3 grid mapping keypad 1-9
  - On human click: call single-turn endpoint; UI updates with human and possible AI move
  - Game status banner (In Progress, X Won, O Won, Draw)
  - New Game button (resets session)
- Training (Self-Play)
  - Input N (default 100; cap 10,000); optional seed
  - Trigger batch; show progress/spinner and summary results
- Admin (optional toggle or separate page)
  - Upload rankings JSON (replace)
  - Export rankings JSON
  - Reset rankings
  - View basic stats (totals and by starter)

## 3. Views & Components
- `BoardGrid`: renders cells, maps keypad 1-9 labels; disables when not human's turn or finished
- `GameStatus`: shows current status and move count
- `ControlsPanel`: starter/symbol selectors, New Game, Self-Play form
- `AdminPanel`: export/import/reset buttons, stats display

## 4. State Management
- `sessionId` stored in component state and appended as query parameter on API calls
- Store last response to drive UI; no client-side rules
- Handle 400 error responses by displaying message and legal moves (highlight allowed cells)

## 5. API Integration
- Base URL configured per environment
- Endpoints: `/api/v1/turn`, `/api/v1/state`, `/api/v1/selfplay`, `/api/v1/admin/*` (admin behind Basic auth)
- CORS: credentials not required (session via query param)

## 6. Accessibility & Responsiveness
- Keyboard support using keypad numbers 1-9
- ARIA labels on cells and status
- Responsive grid; touch friendly

## 7. Telemetry
- Emit page view and interaction events (optional); rely on API OpenTelemetry for backend traces

## 8. Open Questions
- Should admin UI be included in the public app or behind a separate route with prompt for credentials?
- Provide visual heatmap overlay from stats (stretch)

---

Change Log
- v0.01: Initial functional spec for Blazor UI.
