# Spec: Backend API - Functional

- Version: v0.01 (Draft)
- Status: Draft
- Date: 2025-09-17
- Supersedes: n/a
- Template: `docs/specs/spec-template_v1.1.md`
- Related: `spec-system-overview_v0.01.md`, `spec-engine-ai-technical_v0.01.md`, `spec-ui-functional_v0.01.md`, `spec-persistence-technical_v0.01.md`, `spec-observability-technical_v0.01.md`

---

## 1. Objectives & Scope
- Provide versioned, documented HTTP endpoints to play Tic Tac Toe, manage implicit sessions, and expose admin/ops and training capabilities.
- Encapsulate all rules, AI decisions, and learning; frontend is a thin client.
- Non-goals: multi-tenant auth for gameplay (admin has basic auth only), horizontal scale-out of rankings (until shared store exists).

## 2. Constraints & Assumptions
- Separate service from UI; CORS enforced per environment.
- Versioned routes under `/api/v1`. Swagger UI enabled in non-prod only.
- Admin endpoints guarded by Basic Auth (in-memory credentials via config).
- Sessions are anonymous, in-memory, and created implicitly on first move; carried via `sessionId` query parameter.
- Board encoding in API: array of 9 strings each in `{ "X", "O", "E" }`.
- Move addressing: keypad integers 1-9 (layout 7-8-9 top, 1-2-3 bottom).

## 3. Resource Model
- Session: transient game state, options (starter/symbol), move history, and status.
- Move: attempted action with keypad index; server validates and applies.
- Result: in-progress, win (X/O), or draw.
- Rankings: global Q-table stored in memory; replaceable via import.

## 4. Endpoints (v1)

### 4.1 Gameplay - Single Turn (implicit session create)
- `POST /api/v1/turn?sessionId={guid?}`
- Purpose: Apply a human move; if it becomes AI's turn and game not over, auto-play exactly one AI move. Returns updated state and the moves applied in the call.
- Request (JSON):
  - `move`: integer (1-9) keypad index (required unless creating a Computer-first game).
  - `options` (only used if creating a new session i.e., no `sessionId`):
    - `mode`: `"HvsAI" | "AIvsAI"` (default `"HvsAI"`).
    - `starter`: `"Human" | "Computer"` (default `"Human"`).
    - `humanSymbol`: `"X" | "O"` (default `"X"`).
- Responses:
  - 200 OK - JSON body:
    - `sessionId`: string (GUID)
    - `board`: string[9] with `"X"|"O"|"E"`
    - `nextPlayer`: `"X" | "O" | null` (null if game over)
    - `status`: `"InProgress" | "WinX" | "WinO" | "Draw"`
    - `movesApplied`: object with:
      - `human`: integer (1-9) | null
      - `ai`: integer (1-9) | null
    - `moveCount`: integer (total moves so far)
  - 201 Created - same shape as 200, when a new session was created.
  - 400 Bad Request - invalid move/turn or game finished. Body:
    - `code`: `"InvalidMove" | "WrongTurn" | "GameFinished" | ...`
    - `message`: string
    - `board`: string[9]
    - `legalMoves`: integer[] (1-9)

### 4.2 Get State
- `GET /api/v1/state?sessionId={guid}`
- Purpose: Retrieve the current board and status.
- 200 OK - body: `{ sessionId, board, nextPlayer, status, moveCount }`
- 404 Not Found if session missing/expired.

### 4.3 Self-Play (Batch)
- `POST /api/v1/selfplay`
- Purpose: Run N AI-vs-AI games sequentially to train rankings.
- Request:
  - `n`: integer (required). Server caps at 10,000.
  - `seed`: integer (optional). If provided, random generator seeded; else time-based.
- Response 202/200 OK:
  - `requested`: n
  - `played`: integer
  - `winsX`: integer, `winsO`: integer, `draws`: integer
  - `avgMovesPerResult`: object `{ winX: number, winO: number, draw: number }`

### 4.4 Admin - Rankings Reset
- `POST /api/v1/admin/rankings/reset` (Basic Auth)
- 204 No Content.

### 4.5 Admin - Rankings Export
- `GET /api/v1/admin/rankings/export` (Basic Auth)
- 200 OK - JSON export document (see §7 Data Contracts).

### 4.6 Admin - Rankings Import (Replace)
- `POST /api/v1/admin/rankings/import` (Basic Auth)
- Behavior: replaces existing rankings with uploaded data.
- 202 Accepted or 204 No Content on success; 400 on schema/validation errors.

### 4.7 Admin - Stats
- `GET /api/v1/admin/stats` (Basic Auth)
- 200 OK - basic summary (see §7 Data Contracts).

## 5. Versioning & Docs
- Route versioning: `/api/v1`.
- OpenAPI/Swagger: enabled in non-prod only; disabled in prod.

## 6. Security
- Admin endpoints use Basic authentication; in-memory credentials via configuration.
- Non-admin gameplay does not require auth.

## 7. Data Contracts (JSON)

### 7.1 Board & Status
- `board`: string[9] with values `"X"|"O"|"E"`.
- `status`: `"InProgress" | "WinX" | "WinO" | "Draw"`.

### 7.2 Export Format (Rankings)
```
{
  "version": 1,
  "createdUtc": "2025-09-17T00:00:00Z",
  "qValues": [
    { "state": "XOEEXEEOX", "moveIndex": 4, "q": 0.35 },
    { "state": "EEEEEEEEE", "moveIndex": 4, "q": 0.10 }
  ]
}
```
- `state`: 9-char string using `X`,`O`,`E` (row-major); `moveIndex`: 0-8.

### 7.3 Stats Summary
```
{
  "totalGames": 1234,
  "byStarter": {
    "X": { "wins": 600, "losses": 500, "draws": 134, "avgMoves": 5.8 },
    "O": { "wins": 500, "losses": 600, "draws": 134, "avgMoves": 5.9 }
  },
  "overall": { "wins": 1100, "losses": 1100, "draws": 268, "avgMoves": 5.85 }
}
```

## 8. Errors
- 400 with `code` and `message`, and include `board` and `legalMoves` for invalid submissions.
- 404 for unknown session.

## 9. Non-Functional
- CORS: dev allow any; non-prod/prod restrict to configured UI origin(s).
- Telemetry: OpenTelemetry (logs/metrics/traces) - see observability spec.
- Performance: single API instance; self-play runs sequentially; cap N at 10k.

## 10. Open Questions
- Should `GET /state` also include full history? (UI currently does not need.)
- Should we expose legal moves endpoint for hints? (Nice-to-have; not required.)

---

Change Log
- v0.01: Initial endpoint set, contracts, and behaviors aligned with discovery decisions.
