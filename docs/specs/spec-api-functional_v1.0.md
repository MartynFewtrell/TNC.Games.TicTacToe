# API Functional Specification (v1.0)

Supersedes: spec-api-functional_v0.01.md

Version: v1.0 — First stage completed

Summary
- Minimal API exposing endpoints for gameplay, state retrieval, self-play training, and admin operations.

Endpoints
- `POST /api/v1/new` — create a new game session. Accepts JSON options: `Mode`, `Starter` (Human|AI), `HumanSymbol` (X|O). Returns sessionId, board, nextPlayer, status, moveCount, humanSymbol, winningLine.
- `POST /api/v1/turn` — apply a human move. Accepts `sessionId` query or creates a new session if missing. Request body: `{ "Move": <1-9 or 0-8> }`. Returns updated state and applied moves.
- `GET /api/v1/state` — returns session state given `sessionId` query parameter.
- `POST /api/v1/selfplay` — runs N self-play games (cap 10k) updating Q-table.

Admin Endpoints (Basic Auth)
- `POST /admin/rankings/reset` — reset Q-table store.
- `GET /admin/rankings/export` — export Q-table as JSON.
- `POST /admin/rankings/import` — import Q-table JSON.
- `GET /admin/stats` — simple stats for rankings.

Behavioral notes
- When `Starter=AI`, the API applies one AI starter move immediately and returns the updated board and nextPlayer to the client.
- `Turn` endpoint validates moves and returns `400` with a structured payload for invalid moves including legal moves and board.

Errors and validation
- Invalid JSON requests return `400 InvalidJson` with parse messages.
- Invalid options return `400 InvalidOption`.
- Invalid move values return `400 InvalidMoveValue`.
- Attempting a move on an occupied cell returns `400 InvalidMove` with legal moves included.

Security
- Admin endpoints are protected by Basic Auth. Default development credentials: `admin`/`password`.

Change log
- v1.0: Implemented endpoints and documented actual behavior including immediate AI starter move.
