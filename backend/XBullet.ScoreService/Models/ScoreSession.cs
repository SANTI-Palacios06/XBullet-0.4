namespace XBullet.ScoreService.Models;

public sealed class ScoreSession
{
    public string Id { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public string? ClientVersion { get; set; }
    public int TargetBossHits { get; set; }
    public int CurrentScore { get; set; }
    public int FinalScore { get; set; }
    public int BossHits { get; set; }
    public string? Reason { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public SessionStatus Status { get; set; }
    public List<ScoreEvent> Events { get; set; } = new();
}

public sealed class ScoreEvent
{
    public string Id { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? SourceName { get; set; }
    public int Points { get; set; }
    public int TotalScoreAfterEvent { get; set; }
    public int BossHits { get; set; }
    public bool BossDefeated { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public enum SessionStatus
{
    InProgress = 0,
    Victory = 1,
    Defeat = 2
}