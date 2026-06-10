namespace XBullet.ScoreService.Models;

public sealed record StartSessionRequest(
    string PlayerName,
    string StageName,
    int TargetBossHits,
    string? ClientVersion);

public sealed record ScoreEventRequest(
    string EventType,
    int Points,
    int TotalScore,
    string? SourceName,
    int BossHits,
    bool BossDefeated);

public sealed record FinishSessionRequest(
    int FinalScore,
    bool Victory,
    string? Reason,
    int BossHits);

public sealed record SessionSummaryResponse(
    string SessionId,
    string PlayerName,
    string StageName,
    int CurrentScore,
    int BossHits,
    int TargetBossHits,
    string Status,
    DateTime StartedAtUtc,
    DateTime? EndedAtUtc);