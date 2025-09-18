# Observability Technical Specification (v1.0)

Supersedes: spec-observability-technical_v0.01.md

Version: v1.0

Summary
- Telemetry hooks are available. Full OpenTelemetry (OTLP) wiring was prepared but not enabled by default due to package availability in this environment. The code includes `TelemetryExtensions.AddAppTelemetry()` as an extension point.

Traces and Metrics
- `Turn` endpoint starts an Activity and sets tags: `sessionId`, `requestedMoveRaw`, `requestedMove`, `humanMoveIndex`, `aiMoveIndex`, `status`.
- Metrics counters created (when Meter available): `tic_tac_toe_moves_applied`, `tic_tac_toe_games`.

Change log
- v1.0: Added telemetry hooks and activity tags. Full OTLP exporters and runtime instrumentation can be enabled when dependency packages are available.
