# Spec: Engine AI - Technical

- Version: v1.01 (Draft)
- Status: Draft
- Date: 2025-09-18
- Template: `docs/specs/spec-template_v1.1.md`
- Supersedes: `spec-engine-ai-technical_v0.01.md`
- Related: `spec-system-overview_v0.01.md`, `spec-api-functional_v0.01.md`, `spec-ui-functional_v0.01.md`, `spec-persistence-technical_v0.01.md`, `spec-observability-technical_v0.01.md`

---

## 1. Objectives & Scope
- Improve AI strength at runtime using a greedy, ranking-based move policy.
- Add tactical shortcuts prior to ranking: win-in-1, then block-in-1, else greedy.
- Apply state symmetry canonicalization for Q lookups and updates to share experience across symmetric boards.
- Deterministic tie-break among equally ranked moves (stable order: lowest move index).
- Treat missing Q-values as 0; if all candidate moves have equal score (e.g., all 0), choose deterministically via tie-break (no randomness needed).
- Out of scope for this version: minimax/alpha-beta and new learning algorithms beyond current Q-updates. Additional tactical rules (fork/block-fork) can be considered later.

## 2. Constraints & Assumptions
- Runtime policy uses epsilon = 0 (no exploration) during normal play.
- Missing Q-values are treated as 0 during selection.
- Symmetry canonicalization:
  - Consider the 8 symmetries of the 3x3 board (identity; rotations 90/180/270; reflections vertical/horizontal/main-diagonal/anti-diagonal).
  - Canonical state is chosen by computing all symmetric state keys and selecting the lexicographically smallest.
  - Maintain mapping functions to translate move indices between original and canonical orientations.
- Encodings and lookups:
  - State key via `BoardEncoding.ToStateKey(boardStrings)` applied after transform when canonicalizing.
  - Legal moves via `BoardEncoding.GetLegalMoves(boardStrings)`.
  - Q lookup via `IRankingStore.Get(stateKey, moveIndex)` returning `double?` (null if unknown), but runtime treats `null` as `0.0`.
- Determinism: given identical state and rankings, the chosen move is deterministic (no randomness at runtime).
- Training pipeline remains as-is for learning rate and reward; but updates now use canonical state keys and remapped move indices.
- Target framework: .NET 9; no new external dependencies required.

## 3. Components Overview
- AI Runtime Policy
  - Inputs: current `Engine.GameState`, `IRankingStore`.
  - Behavior:
    A) Tactical shortcuts (performed on the original orientation for clarity and determinism):
       1) Win-in-1: For each legal move `m`, apply to a copy of the current state; if it yields an immediate win for current player, select the smallest-index such `m` and return.
       2) Block-in-1: If no winning move exists, simulate opponent's next turn: for each legal `m`, apply it and check if opponent would have a win-in-1 on their next move; prefer the smallest-index `m` that prevents all opponent immediate wins. If multiple block moves exist, pick the smallest index.
    B) Greedy selection with symmetry canonicalization (when no tactical shortcut selected):
       3) Compute `boardStrings` and enumerate `legalMoves`.
       4) Compute canonicalization: determine transform `T` such that `T(state)` has the lexicographically smallest key among all symmetries; record `T` and its inverse `T?¹` for move mapping.
       5) Let `stateKeyC = ToStateKey(T(state))`.
       6) For each legal move `m` in original orientation, map to canonical `mc = T(m)` and query `q? = store.Get(stateKeyC, mc)`; let `q = q? ?? 0.0`.
       7) Let `bestQ = max(q)`; choose moves with `q == bestQ`; apply deterministic tie-break using the original indices (i.e., compare `m`, not `mc`), then return the selected `m`.
  - Telemetry (optional): log whether a tactical shortcut fired (`winIn1`, `blockIn1`), the chosen transform (e.g., `Rot90`), selected move, and `bestQ`.

- Ranking Store
  - Keys are canonical: all reads/writes use the canonical state key and canonical move index.
  - In-memory implementation remains default; export/import format unchanged, but exported `state` values are canonical.

- Training Pipeline
  - Unchanged reward and learning rate; add canonicalization during updates:
    - When recording `(stateKey, moveIndex)` pairs for histories, convert to `(stateKeyC, moveIndexC)` using the same canonical transform as runtime at the moment of recording.
    - `Learning.UpdateQ` should perform canonicalization (state and move) before reading/updating the store.

---

Open Decisions
- None for v1.01; additional tactics (fork/block-fork) and heuristic tie-breaks can be evaluated in a future version.

Change Log
- v1.01: Add tactical shortcuts (win-in-1, block-in-1) and symmetry canonicalization for Q lookups/updates; keep greedy selection with deterministic tie-break; treat missing Q-values as 0 and maintain deterministic runtime.
