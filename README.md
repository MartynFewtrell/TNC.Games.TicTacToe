# TNC.Games.TicTacToe

A Tic Tac Toe game backend + web UI with a simple AI and learning Q-table.

## Prerequisites

- .NET 9 SDK installed (dotnet --version should report a 9.x preview). Tested with .NET 9 preview.
- Optional: a browser to use the Blazor web UI.

## Clone

```bash
git clone https://github.com/MartynFewtrell/TNC.Games.TicTacToe.git
cd TNC.Games.TicTacToe
```

## Build

From the repository root (where the `.sln` file is):

```bash
dotnet build
```

## Run the API

Run the minimal API (default settings):

```bash
dotnet run --project src/api/Tnc.Games.TicTacToe.Api
```

By default the API will listen on the ASP.NET Core default URL(s) for your environment (usually `http://localhost:5000` / `https://localhost:5001`). Use the console output to confirm the exact URL(s).

## Run the Web UI (Blazor)

The web project is a Blazor app. Run it with:

```bash
dotnet run --project src/web/Tnc.Games.TicTacToe.Web
```

Open the browser to the URL shown in the console (usually `https://localhost:5001` or a different port). The web UI will call the API endpoints.

## Configuration

- Admin credentials for admin endpoints (Basic Auth) are read from configuration keys `Admin:Username` and `Admin:Password`.
- Defaults used by the development code: `admin` / `password`.
- To override, set environment variables or update `appsettings.{Environment}.json` in the API project.

## API endpoints (examples)

- POST `/api/v1/turn` — apply a human move (creates a session if none provided). Body: `{ "Move": 1 }` (1-9 keypad mapping). Returns session state and applied moves.

Example (create session + turn):

```bash
curl -s -X POST http://localhost:5000/api/v1/turn -H "Content-Type: application/json" -d '{"Move":1}'
```

- GET `/api/v1/state?sessionId={id}` — get current session state.

- POST `/api/v1/selfplay` — run N self-play games to train Q-table. Body: `{ "N": 100, "Seed": 123 }`.

Admin endpoints (Basic auth required):

- `POST /admin/rankings/reset` — reset Q-table.
- `GET /admin/rankings/export` — export current Q-table.
- `POST /admin/rankings/import` — import Q-table JSON.
- `GET /admin/stats` — simple stats about rankings.

Example admin export with curl (default credentials):

```bash
curl -u admin:password http://localhost:5000/admin/rankings/export
```

## Running tests

From the solution root:

```bash
dotnet test
```

This runs unit and integration tests that exercise the engine and API.

## Notes

- Telemetry: OpenTelemetry wiring is included as an extensible hook. In this environment the OTLP packages are not enabled by default — you can enable and configure exporters in `Program.cs` or via `Telemetry/TelemetryExtensions.cs` when you have OTLP dependencies available.
- The in-memory ranking store and session store are simple memory-backed implementations intended for local development and testing. They will be reset when the process restarts.

If you have any specific environment (Docker, cloud) or CI requirements, tell me and I can add scripts and configuration.
