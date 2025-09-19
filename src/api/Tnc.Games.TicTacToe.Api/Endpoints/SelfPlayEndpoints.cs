using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;
using Tnc.Games.TicTacToe.Api.Domain;
using Engine = Tnc.Games.TicTacToe.Api.Engine;
using Tnc.Games.TicTacToe.Shared;
using Tnc.Games.TicTacToe.Api.Background;

namespace Tnc.Games.TicTacToe.Api.Endpoints
{
    public static class SelfPlayEndpoints
    {
        public static void MapSelfPlayEndpoints(this WebApplication app)
        {
            // Register the job queue as singleton
            // Note: registration handled in Program.cs

            app.MapPost("/api/v1/selfplay", async (HttpRequest request) =>
            {
                var logger = app.Logger;

                // Read body manually to support camelCase or PascalCase properties
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
                    logger.LogWarning(ex, "Failed to read /api/v1/selfplay request body");
                }

                SelfPlayRequest? req = null;
                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        var jsOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        req = JsonSerializer.Deserialize<SelfPlayRequest>(body, jsOptions);
                    }
                    catch (JsonException jex)
                    {
                        logger.LogWarning(jex, "Invalid JSON for /api/v1/selfplay");
                        return Results.BadRequest(new { code = "InvalidJson", message = jex.Message });
                    }
                }

                if (req == null)
                {
                    return Results.BadRequest(new { code = "MissingBody", message = "Request body is required" });
                }

                // Run inline for backward compatibility
                var inlineResult = await Task.Run(() => RunSelfPlayInline(app, req));
                return Results.Ok(inlineResult);
            });

            // New background job API: start job
            app.MapPost("/api/v1/selfplay/start", (SelfPlayRequest req, SelfPlayJobQueue queue) =>
            {
                var job = queue.Create(Math.Min(req.N, 100000));
                job.Status = SelfPlayJobStatus.Pending;

                // Kick off background work
                _ = Task.Run(() =>
                {
                    job.Status = SelfPlayJobStatus.Running;
                    queue.Update(job);
                    try
                    {
                        var result = RunSelfPlayInline(app, req);
                        job.Played = result.Played;
                        job.WinsX = result.WinsX;
                        job.WinsO = result.WinsO;
                        job.Draws = result.Draws;
                        // extract avgMoves and elapsedMs
                        try
                        {
                            var avgObj = result.AvgMovesPerResult as object;
                            var propAvg = avgObj.GetType().GetProperty("avgMoves");
                            var propElapsed = avgObj.GetType().GetProperty("elapsedMs");
                            if (propAvg != null) job.AvgMoves = Convert.ToDouble(propAvg.GetValue(avgObj));
                            if (propElapsed != null) job.ElapsedMs = Convert.ToDouble(propElapsed.GetValue(avgObj));
                        }
                        catch { }

                        job.Status = SelfPlayJobStatus.Completed;
                        job.CompletedAt = DateTime.UtcNow;
                        queue.Update(job);
                    }
                    catch (Exception ex)
                    {
                        job.Status = SelfPlayJobStatus.Failed;
                        job.Error = ex.Message;
                        job.CompletedAt = DateTime.UtcNow;
                        queue.Update(job);
                    }
                });

                return Results.Accepted($"/api/v1/selfplay/status?id={job.Id}", new { jobId = job.Id, status = job.Status.ToString() });
            });

            // Status endpoint
            app.MapGet("/api/v1/selfplay/status", (HttpRequest req, SelfPlayJobQueue queue) =>
            {
                var qs = req.Query;
                if (!qs.TryGetValue("id", out var idv) || !Guid.TryParse(idv, out var id))
                {
                    return Results.BadRequest(new { code = "MissingId", message = "id query parameter is required and must be a GUID" });
                }

                if (!queue.TryGet(id, out var job) || job == null)
                {
                    return Results.NotFound(new { code = "JobNotFound", message = "Job not found" });
                }

                return Results.Ok(new { job.Id, status = job.Status.ToString(), requested = job.Requested, played = job.Played, winsX = job.WinsX, winsO = job.WinsO, draws = job.Draws, avgMoves = job.AvgMoves, elapsedMs = job.ElapsedMs, error = job.Error });
            });
        }

        private static SelfPlayResponse RunSelfPlayInline(WebApplication app, SelfPlayRequest req)
        {
            // Copy of previous inline implementation
            var logger = app.Logger;
            var n = Math.Min(req.N, 100000);
            var seed = req.Seed;

            var sw = Stopwatch.StartNew();

            var rng = seed.HasValue ? new Random(seed.Value) : new Random();

            var rankingStore = app.Services.GetRequiredService<IRankingStore>();
            var activitySource = app.Services.GetService<ActivitySource>();

            int played = 0, winsX = 0, winsO = 0, draws = 0;
            long totalMoves = 0;

            using (var span = activitySource?.StartActivity("SelfPlay", ActivityKind.Internal))
            {
                span?.SetTag("requestedGames", n);

                for (int i = 0; i < n; i++)
                {
                    var gameRng = new Random(rng.Next());
                    var policy = new Policy(0.15, gameRng);

                    var state = new Engine.GameState();
                    var historyX = new List<(string stateKey, int move)>();
                    var historyO = new List<(string stateKey, int move)>();

                    while (state.Status == Engine.GameStatus.InProgress)
                    {
                        var boardStrings = Engine.Rules.ToBoardStrings(state);
                        var legalMoves = BoardEncoding.GetLegalMoves(boardStrings);
                        var stateKey = BoardEncoding.ToStateKey(boardStrings);

                        var move = policy.SelectMove(state, rankingStore);

                        if (state.NextPlayer == Engine.Player.X)
                        {
                            historyX.Add((stateKey, move));
                            Engine.Rules.ApplyMove(state, move, Engine.Player.X);
                        }
                        else
                        {
                            historyO.Add((stateKey, move));
                            Engine.Rules.ApplyMove(state, move, Engine.Player.O);
                        }
                    }

                    played++;
                    totalMoves += state.MoveHistory.Count;

                    if (state.Status == Engine.GameStatus.WinX) winsX++;
                    else if (state.Status == Engine.GameStatus.WinO) winsO++;
                    else draws++;

                    var resultX = state.Status == Engine.GameStatus.WinX ? GameResult.Win : (state.Status == Engine.GameStatus.Draw ? GameResult.Draw : GameResult.Loss);
                    Learning.UpdateQ(rankingStore, historyX, resultX);

                    var resultO = state.Status == Engine.GameStatus.WinO ? GameResult.Win : (state.Status == Engine.GameStatus.Draw ? GameResult.Draw : GameResult.Loss);
                    Learning.UpdateQ(rankingStore, historyO, resultO);

                    if ((i + 1) % 100 == 0)
                    {
                        logger.LogInformation("SelfPlay progress: {completed}/{total} games completed", i + 1, n);
                    }
                }

                span?.SetTag("played", played);
                span?.SetTag("winsX", winsX);
                span?.SetTag("winsO", winsO);
                span?.SetTag("draws", draws);
            }

            sw.Stop();
            var elapsedMs = sw.Elapsed.TotalMilliseconds;
            var avgMoves = played > 0 ? (double)totalMoves / played : 0.0;

            return new SelfPlayResponse(n, played, winsX, winsO, draws, new { avgMoves, elapsedMs });
        }
    }
}
