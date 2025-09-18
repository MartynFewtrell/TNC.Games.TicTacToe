using Tnc.Games.TicTacToe.Shared;
using Tnc.Games.TicTacToe.Api.Infrastructure;
using Tnc.Games.TicTacToe.Api.Domain;
using Tnc.Games.TicTacToe.Api.Endpoints;
using Tnc.Games.TicTacToe.Api.Security;
using Tnc.Games.TicTacToe.Api.Telemetry;
using Tnc.Games.TicTacToe.Api.Background;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDev", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// DI: stores and services
builder.Services.AddSingleton<ISessionStore, InMemorySessionStore>();
builder.Services.AddSingleton<IRankingStore, RankingStoreMemory>();
builder.Services.AddSingleton<Policy>();

// Telemetry
builder.Services.AddAppTelemetry();

// Job queue
builder.Services.AddSingleton<SelfPlayJobQueue>();

// Basic auth
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", null);

// Authorization services required when using app.UseAuthorization()
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowDev");
    app.UseDeveloperExceptionPage();
}

// Enable Swagger UI in non-production environments (Development, Staging, etc.)
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Tic Tac Toe API");

// Map gameplay endpoints
app.MapGameplayEndpoints();
app.MapSelfPlayEndpoints();

// Admin endpoints
app.MapPost("/admin/rankings/reset", [Microsoft.AspNetCore.Authorization.Authorize] (IRankingStore store) =>
{
    store.Reset();
    return Results.Ok(new { status = "reset" });
});

app.MapGet("/admin/rankings/export", [Microsoft.AspNetCore.Authorization.Authorize] (IRankingStore store) =>
{
    var exported = store.Export();
    return Results.Ok(exported);
});

app.MapPost("/admin/rankings/import", [Microsoft.AspNetCore.Authorization.Authorize] async (HttpContext http, IRankingStore store) =>
{
    var doc = await System.Text.Json.JsonSerializer.DeserializeAsync<System.Text.Json.JsonElement>(http.Request.Body);
    store.ImportReplace(doc);
    return Results.Ok(new { status = "imported" });
});

app.MapGet("/admin/stats", [Microsoft.AspNetCore.Authorization.Authorize] (IRankingStore store) =>
{
    // Simple stats: number of entries
    var exported = store.Export();
    int count = 0;
    if (exported is System.Collections.IEnumerable e)
    {
        foreach (var _ in e) count++;
    }
    return Results.Ok(new { entries = count });
});

app.Run();
