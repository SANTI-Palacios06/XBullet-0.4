using System.Text.Json;
using XBullet.ScoreService.Models;
using XBullet.ScoreService.Storage;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<ScoreRepository>();

WebApplication app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "xbullet-score-service",
    timestampUtc = DateTime.UtcNow
}));

app.MapPost("/api/sessions/start", async (StartSessionRequest request, ScoreRepository repository, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.PlayerName))
    {
        return Results.BadRequest(new { error = "playerName is required" });
    }

    ScoreSession session = new()
    {
        Id = Guid.NewGuid().ToString("N"),
        PlayerName = request.PlayerName.Trim(),
        StageName = string.IsNullOrWhiteSpace(request.StageName) ? "X-Bullet Pinball" : request.StageName.Trim(),
        ClientVersion = request.ClientVersion?.Trim(),
        TargetBossHits = Math.Max(1, request.TargetBossHits),
        StartedAtUtc = DateTime.UtcNow,
        Status = SessionStatus.InProgress,
        CurrentScore = 0,
        BossHits = 0,
        Events = new List<ScoreEvent>()
    };

    await repository.CreateSessionAsync(session, cancellationToken);

    return Results.Ok(new SessionSummaryResponse(
        session.Id,
        session.PlayerName,
        session.StageName,
        session.CurrentScore,
        session.BossHits,
        session.TargetBossHits,
        session.Status.ToString(),
        session.StartedAtUtc,
        session.EndedAtUtc));
});

app.MapPost("/api/sessions/{sessionId}/events", async (string sessionId, ScoreEventRequest request, ScoreRepository repository, CancellationToken cancellationToken) =>
{
    ScoreSession? session = await repository.GetSessionAsync(sessionId, cancellationToken);
    if (session is null)
    {
        return Results.NotFound(new { error = "session not found" });
    }

    if (session.Status != SessionStatus.InProgress)
    {
        return Results.BadRequest(new { error = "session is already closed" });
    }

    ScoreEvent scoreEvent = new()
    {
        Id = Guid.NewGuid().ToString("N"),
        EventType = string.IsNullOrWhiteSpace(request.EventType) ? "unknown" : request.EventType.Trim(),
        SourceName = request.SourceName?.Trim(),
        Points = Math.Max(0, request.Points),
        TotalScoreAfterEvent = Math.Max(0, request.TotalScore),
        BossHits = Math.Max(0, request.BossHits),
        BossDefeated = request.BossDefeated,
        CreatedAtUtc = DateTime.UtcNow
    };

    session.Events.Add(scoreEvent);
    session.CurrentScore = scoreEvent.TotalScoreAfterEvent;
    session.BossHits = Math.Max(session.BossHits, scoreEvent.BossHits);

    await repository.UpdateSessionAsync(session, cancellationToken);

    return Results.Ok(new
    {
        sessionId = session.Id,
        currentScore = session.CurrentScore,
        bossHits = session.BossHits,
        eventsCount = session.Events.Count
    });
});

app.MapPost("/api/sessions/{sessionId}/finish", async (string sessionId, FinishSessionRequest request, ScoreRepository repository, CancellationToken cancellationToken) =>
{
    ScoreSession? session = await repository.GetSessionAsync(sessionId, cancellationToken);
    if (session is null)
    {
        return Results.NotFound(new { error = "session not found" });
    }

    if (session.Status != SessionStatus.InProgress)
    {
        return Results.Ok(new
        {
            sessionId = session.Id,
            status = session.Status.ToString(),
            finalScore = session.FinalScore
        });
    }

    session.FinalScore = Math.Max(request.FinalScore, session.CurrentScore);
    session.CurrentScore = session.FinalScore;
    session.BossHits = Math.Max(session.BossHits, request.BossHits);
    session.Reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
    session.EndedAtUtc = DateTime.UtcNow;
    session.Status = request.Victory ? SessionStatus.Victory : SessionStatus.Defeat;

    await repository.UpdateSessionAsync(session, cancellationToken);

    return Results.Ok(new SessionSummaryResponse(
        session.Id,
        session.PlayerName,
        session.StageName,
        session.CurrentScore,
        session.BossHits,
        session.TargetBossHits,
        session.Status.ToString(),
        session.StartedAtUtc,
        session.EndedAtUtc));
});

app.MapGet("/api/sessions/{sessionId}", async (string sessionId, ScoreRepository repository, CancellationToken cancellationToken) =>
{
    ScoreSession? session = await repository.GetSessionAsync(sessionId, cancellationToken);
    return session is null ? Results.NotFound() : Results.Ok(session);
});

app.MapGet("/api/leaderboard", async (int? limit, ScoreRepository repository, CancellationToken cancellationToken) =>
{
    IReadOnlyList<ScoreSession> sessions = await repository.GetLeaderboardAsync(limit ?? 10, cancellationToken);

    return Results.Ok(sessions.Select(session => new
    {
        sessionId = session.Id,
        playerName = session.PlayerName,
        stageName = session.StageName,
        finalScore = session.FinalScore,
        bossHits = session.BossHits,
        targetBossHits = session.TargetBossHits,
        status = session.Status.ToString(),
        startedAtUtc = session.StartedAtUtc,
        endedAtUtc = session.EndedAtUtc,
        reason = session.Reason
    }));
});

app.Run();