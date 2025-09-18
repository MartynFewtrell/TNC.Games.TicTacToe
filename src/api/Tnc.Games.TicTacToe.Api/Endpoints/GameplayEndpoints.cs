using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;
using Tnc.Games.TicTacToe.Api.Domain;
using Engine = Tnc.Games.TicTacToe.Api.Engine;
using Tnc.Games.TicTacToe.Shared;

namespace Tnc.Games.TicTacToe.Api.Endpoints
{
    public static class GameplayEndpoints
    {
        public static void MapGameplayEndpoints(this WebApplication app)
        {
            app.MapPost("/api/v1/new", async (HttpRequest request) =>
            {
                var logger = app.Logger;
                var sessionStore = app.Services.GetRequiredService<ISessionStore>();

                // Read request body manually to provide clearer errors and allow case-insensitive matching
                string body = string.Empty;
                try
                {
                    request.EnableBuffering();
                    using var reader = new System.IO.StreamReader(request.Body, leaveOpen: true);
                    body = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to read /api/v1/new request body");
                }

                logger.LogDebug("New session request body: {body}", body);

                TurnOptions? opt = null;
                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        var jsOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        opt = JsonSerializer.Deserialize<TurnOptions>(body, jsOptions);
                        logger.LogDebug("Deserialized TurnOptions: {@opt}", opt);
                    }
                    catch (JsonException jex)
                    {
                        logger.LogWarning(jex, "Invalid JSON for /api/v1/new");
                        return Results.BadRequest(new { code = "InvalidJson", message = jex.Message, raw = body });
                    }
                }

                // If no body or deserialization produced null, look for query parameters as fallback
                if (opt is null)
                {
                    var qs = request.Query;
                    var starterQs = qs.TryGetValue("starter", out var sVal) ? sVal.ToString() : null;
                    var humanSymbolQs = qs.TryGetValue("humanSymbol", out var hVal) ? hVal.ToString() : null;
                    if (!string.IsNullOrWhiteSpace(starterQs) || !string.IsNullOrWhiteSpace(humanSymbolQs))
                    {
                        opt = new TurnOptions(Mode: "HvsAI", Starter: starterQs ?? "Human", HumanSymbol: humanSymbolQs ?? "X");
                        logger.LogDebug("Using TurnOptions from query string: {@opt}", opt);
                    }
                }

                var state = new Engine.GameState();

                // Apply options
                var options = opt ?? new TurnOptions();
                // Validate HumanSymbol
                var humanSymbol = (options.HumanSymbol ?? "X").ToUpperInvariant();
                if (humanSymbol != "X" && humanSymbol != "O")
                {
                    var msg = "HumanSymbol must be 'X' or 'O'";
                    logger.LogWarning("Invalid option HumanSymbol: {symbol}", options.HumanSymbol);
                    return Results.BadRequest(new { code = "InvalidOption", message = msg });
                }

                // Validate Starter
                var starter = (options.Starter ?? "Human");
                if (!string.Equals(starter, "Human", StringComparison.OrdinalIgnoreCase) && !string.Equals(starter, "AI", StringComparison.OrdinalIgnoreCase))
                {
                    var msg = "Starter must be 'Human' or 'AI'";
                    logger.LogWarning("Invalid option Starter: {starter}", options.Starter);
                    return Results.BadRequest(new { code = "InvalidOption", message = msg });
                }

                logger.LogInformation("Creating new session with Starter={starter}, HumanSymbol={humanSymbol}", starter, humanSymbol);

                // Determine human player
                state.HumanPlayer = humanSymbol == "O" ? Engine.Player.O : Engine.Player.X;

                // Starter: if Human then next player should be human, else AI
                if (string.Equals(starter, "AI", StringComparison.OrdinalIgnoreCase))
                {
                    // If AI starts, set next player to the opposite of human
                    state.NextPlayer = state.HumanPlayer == Engine.Player.X ? Engine.Player.O : Engine.Player.X;
                }
                else
                {
                    state.NextPlayer = state.HumanPlayer;
                }

                var sessionId = sessionStore.Create(state);

                // If AI starts, apply a single AI move immediately so UI receives updated board and nextPlayer
                if (state.Status == Engine.GameStatus.InProgress && state.NextPlayer != state.HumanPlayer)
                {
                    try
                    {
                        var rankingStore = request.HttpContext.RequestServices.GetRequiredService<IRankingStore>();
                        var policy = request.HttpContext.RequestServices.GetRequiredService<Policy>();

                        var boardStrings = Engine.Rules.ToBoardStrings(state);
                        var legalMoves = BoardEncoding.GetLegalMoves(boardStrings);
                        var stateKey = BoardEncoding.ToStateKey(boardStrings);
                        var aiMove = policy.SelectMove(stateKey, legalMoves, rankingStore);

                        Engine.Rules.ApplyMove(state, aiMove, state.NextPlayer);
                        // persist updated state
                        sessionStore.Update(sessionId, state);
                        logger.LogInformation("AI starter applied move {move} for session {session}", aiMove, sessionId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to apply AI starter move for session {session}", sessionId);
                    }
                }

                var resp = new StateResponse(sessionId.ToString(), Engine.Rules.ToBoardStrings(state), state.NextPlayer.ToString(), state.Status.ToString(), state.MoveHistory.Count, state.HumanPlayer == Engine.Player.X ? "X" : "O", Engine.Rules.GetWinningLine(state));
                return Results.Ok(resp);
            });

            app.MapPost("/api/v1/turn", (HttpContext http, TurnRequest req) =>
            {
                var sessionIdQuery = http.Request.Query["sessionId"].ToString();
                var sessionStore = http.RequestServices.GetRequiredService<ISessionStore>();
                Engine.GameState state;
                Guid sessionGuid;

                if (string.IsNullOrEmpty(sessionIdQuery) || !Guid.TryParse(sessionIdQuery, out sessionGuid) || !sessionStore.TryGet(sessionGuid, out var maybeState))
                {
                    // create new session
                    state = new Engine.GameState();
                    sessionGuid = sessionStore.Create(state);
                }
                else
                {
                    // Found existing
                    state = maybeState!; // non-null because TryGet returned true
                }

                // get services needed for move selection
                var rankingStore = http.RequestServices.GetRequiredService<IRankingStore>();
                var policy = http.RequestServices.GetRequiredService<Policy>();

                // determine humanMove from request (support 1-based keypad or 0-based index)
                var rawMove = req.Move; // client provided value
                int humanMove;
                if (rawMove >= 1 && rawMove <= 9)
                {
                    humanMove = rawMove - 1;
                }
                else if (rawMove >= 0 && rawMove <= 8)
                {
                    humanMove = rawMove;
                }
                else
                {
                    // invalid move value
                    var boardStringsInvalid = Engine.Rules.ToBoardStrings(state);
                    var legal = BoardEncoding.GetLegalMoves(boardStringsInvalid);
                    http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Gameplay").LogInformation("Invalid move value. Session={session}, rawMove={rawMove}, legalIndices={legal}, board={board}", sessionGuid, rawMove, string.Join(',', legal), string.Join(',', boardStringsInvalid));
                    return Results.BadRequest(new { code = "InvalidMoveValue", message = "Move value must be 1..9 (keypad) or 0..8 (index)", rawMove });
                }

                // Telemetry objects
                var activitySource = http.RequestServices.GetService<ActivitySource>();
                var meter = http.RequestServices.GetService<Meter>();
                var movesCounter = meter?.CreateCounter<long>("tic_tac_toe_moves_applied");
                var gamesCounter = meter?.CreateCounter<long>("tic_tac_toe_games");

                using (var activity = activitySource?.StartActivity("Turn", ActivityKind.Server))
                {
                    activity?.SetTag("sessionId", sessionGuid.ToString());
                    activity?.SetTag("requestedMoveRaw", rawMove);
                    activity?.SetTag("requestedMove", humanMove + 1);

                    // Validate move
                    if (!Engine.Rules.IsLegal(state, humanMove))
                    {
                        var boardStringsInvalid = Engine.Rules.ToBoardStrings(state);
                        var legal = BoardEncoding.GetLegalMoves(boardStringsInvalid); // 0-based indices
                        var legalKeypad = legal.Select(i => i + 1).ToArray(); // 1-based for client clarity
                        activity?.SetTag("status", "InvalidMove");
                        http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Gameplay").LogInformation("Invalid move attempt. Session={session}, rawMove={rawMove}, attemptedIndex={attempt}, legalIndices={legalIndices}, legalKeypad={legalKeypad}, board={board}", sessionGuid, rawMove, humanMove, string.Join(',', legal), string.Join(',', legalKeypad), string.Join(',', boardStringsInvalid));
                        return Results.BadRequest(new { code = "InvalidMove", message = "Move is not legal", rawMove, attemptedIndex = humanMove, attemptedKeypad = humanMove + 1, board = boardStringsInvalid, legalMoves = legal, legalMovesKeypad = legalKeypad });
                    }

                    // Apply human move (honor human symbol)
                    var humanPlayer = state.HumanPlayer;
                    Engine.Rules.ApplyMove(state, humanMove, humanPlayer);
                    int humanApplied = 1;
                    int aiApplied = 0;
                    activity?.SetTag("humanMoveIndex", humanMove);

                    // If game not over and next player is AI, select and apply one AI move
                    if (state.Status == Engine.GameStatus.InProgress && state.NextPlayer != state.HumanPlayer)
                    {
                        var boardStrings = Engine.Rules.ToBoardStrings(state);
                        var legalMoves = BoardEncoding.GetLegalMoves(boardStrings);
                        var stateKey = BoardEncoding.ToStateKey(boardStrings);
                        var aiMove = policy.SelectMove(stateKey, legalMoves, rankingStore);
                        Engine.Rules.ApplyMove(state, aiMove, state.NextPlayer);
                        aiApplied = 1;
                        activity?.SetTag("aiMoveIndex", aiMove);
                    }

                    // persist
                    sessionStore.Update(sessionGuid, state);

                    // record metrics
                    movesCounter?.Add(humanApplied + aiApplied, tags: new KeyValuePair<string, object?>[] { new("sessionId", sessionGuid.ToString()), new("status", state.Status.ToString()) });
                    if (humanApplied + aiApplied > 0)
                    {
                        gamesCounter?.Add(1, tags: new KeyValuePair<string, object?>[] { new("status", state.Status.ToString()) });
                    }

                    activity?.SetTag("status", state.Status.ToString());

                    // Include WinningLine in TurnResponse and StateResponse creation so the UI can highlight the winning line.
                    var winningLine = Engine.Rules.GetWinningLine(state);
                    var resp = new TurnResponse(sessionGuid.ToString(), Engine.Rules.ToBoardStrings(state), state.NextPlayer.ToString(), state.Status.ToString(), state.MoveHistory.Count, new MovesApplied(humanApplied, aiApplied), winningLine);
                    return Results.Ok(resp);
                }
            });

            app.MapGet("/api/v1/state", (HttpContext http) =>
            {
                var sessionIdQuery = http.Request.Query["sessionId"].ToString();
                if (string.IsNullOrEmpty(sessionIdQuery) || !Guid.TryParse(sessionIdQuery, out var sessionGuid))
                {
                    return Results.BadRequest(new { code = "MissingSessionId", message = "sessionId query parameter is required and must be a GUID" });
                }

                var sessionStore = http.RequestServices.GetRequiredService<ISessionStore>();
                if (!sessionStore.TryGet(sessionGuid, out var state) || state is null)
                {
                    return Results.NotFound(new { code = "SessionNotFound", message = "Session not found" });
                }

                var resp = new StateResponse(sessionGuid.ToString(), Engine.Rules.ToBoardStrings(state), state.NextPlayer.ToString(), state.Status.ToString(), state.MoveHistory.Count, state.HumanPlayer == Engine.Player.X ? "X" : "O", Engine.Rules.GetWinningLine(state));
                return Results.Ok(resp);
            });
        }
    }
}
