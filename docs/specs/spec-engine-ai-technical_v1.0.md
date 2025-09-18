# Engine & AI Technical Specification (v1.0)

Supersedes: spec-engine-ai-technical_v0.01.md

Version: v1.0

Domain model
- `Cell` enum: `E`, `X`, `O`.
- `Player` enum: `X`, `O`.
- `GameState`: mutable class with `Board: Cell[9]`, `NextPlayer`, `Status`, `MoveHistory`, `HumanPlayer` (which player the human controls).

Rules
- Legal move: cell empty and game `InProgress`.
- Win detection: 8 winning lines; once a line completed by same player's cells -> `WinX` or `WinO`.
- Draw detection: all cells filled and no win -> `Draw`.

Q-table and Policy
- `IRankingStore` interface with `Get`, `Set`, `Reset`, `Export`, `ImportReplace`.
- `RankingStoreMemory`: concurrent dictionary keyed by `(stateKey, moveIndex)` storing double Q values clamped [-5,+5].
- `Policy`: epsilon-greedy selection, epsilon default 0.15, ties broken randomly; missing Q values treated as null and fallback to random.

Learning
- Reward: win = +1, loss = -1, draw = c * ((4.5 - moves) / 4.5) with c=0.2.
- Update rule: Q <- clamp(Q + alpha*R, -5, +5) with alpha=0.1, applied for each (state, move) in game history.

Self-play
- `POST /api/v1/selfplay` runs sequential games using the same policy and updates Q-table after each completed game.
- Summary returned includes wins/draws and average moves.

Change log
- v1.0: Implemented memory Q-table, policy and learning functions, and self-play endpoint.
