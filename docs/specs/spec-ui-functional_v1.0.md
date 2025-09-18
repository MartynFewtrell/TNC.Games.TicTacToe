# UI Functional Specification (v1.0)

Supersedes: spec-ui-functional_v0.01.md

Version: v1.0

Summary
- Blazor Web App scaffold provides board UI, game status, controls, self-play and admin panels (basic scaffolding). The UI calls the API endpoints for gameplay and admin operations.

Features implemented in v1.0
- Board rendering (3x3), mapping keypad 1-9 to board indices.
- API integration hooks in the UI to call `/api/v1/turn`, `/api/v1/state`, `/api/v1/selfplay`, and admin endpoints.
- Admin panel scaffold protected by Basic Auth prompt (UI prompt implementation deferred).

Change log
- v1.0: Scaffold delivered; full UI controls and polish planned for future iterations.
