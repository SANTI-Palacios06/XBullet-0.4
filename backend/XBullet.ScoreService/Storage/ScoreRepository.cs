using System.Text.Json;
using XBullet.ScoreService.Models;

namespace XBullet.ScoreService.Storage;

/// Persistencia simple basada en archivo JSON.
/// Evita dependencias externas y sigue siendo suficiente para una demo cliente-servidor.
public sealed class ScoreRepository
{
    private const string FileName = "score-data.json";
    private const int MaxCompletedSessions = 25;

    private readonly string filePath;
    private readonly SemaphoreSlim mutex = new(1, 1);
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public ScoreRepository()
    {
        string dataDirectory = Environment.GetEnvironmentVariable("SCORE_DATA_DIR") ?? "/app/data";
        Directory.CreateDirectory(dataDirectory);
        filePath = Path.Combine(dataDirectory, FileName);
    }

    public async Task CreateSessionAsync(ScoreSession session, CancellationToken cancellationToken)
    {
        await mutex.WaitAsync(cancellationToken);
        try
        {
            ScoreDatabase database = await ReadDatabaseInternalAsync(cancellationToken);
            database.Sessions.Add(session);

            // Mantiene solo las 25 sesiones con mayor score finalizadas
            // Las sesiones en progreso siempre se conservan
            List<ScoreSession> completed = database.Sessions
                .Where(s => s.Status != SessionStatus.InProgress)
                .OrderByDescending(s => s.FinalScore)
                .Take(MaxCompletedSessions)
                .ToList();

            List<ScoreSession> inProgress = database.Sessions
                .Where(s => s.Status == SessionStatus.InProgress)
                .ToList();

            database.Sessions = inProgress.Concat(completed).ToList();

            await WriteDatabaseInternalAsync(database, cancellationToken);
        }
        finally
        {
            mutex.Release();
        }
    }

    public async Task<ScoreSession?> GetSessionAsync(string sessionId, CancellationToken cancellationToken)
    {
        await mutex.WaitAsync(cancellationToken);
        try
        {
            ScoreDatabase database = await ReadDatabaseInternalAsync(cancellationToken);
            return database.Sessions.FirstOrDefault(session => session.Id == sessionId);
        }
        finally
        {
            mutex.Release();
        }
    }

    public async Task UpdateSessionAsync(ScoreSession updatedSession, CancellationToken cancellationToken)
    {
        await mutex.WaitAsync(cancellationToken);
        try
        {
            ScoreDatabase database = await ReadDatabaseInternalAsync(cancellationToken);
            int index = database.Sessions.FindIndex(session => session.Id == updatedSession.Id);

            if (index >= 0)
            {
                database.Sessions[index] = updatedSession;

                // Al actualizar también aplica el límite de sesiones completadas
                List<ScoreSession> completed = database.Sessions
                    .Where(s => s.Status != SessionStatus.InProgress)
                    .OrderByDescending(s => s.FinalScore)
                    .Take(MaxCompletedSessions)
                    .ToList();

                List<ScoreSession> inProgress = database.Sessions
                    .Where(s => s.Status == SessionStatus.InProgress)
                    .ToList();

                database.Sessions = inProgress.Concat(completed).ToList();

                await WriteDatabaseInternalAsync(database, cancellationToken);
            }
        }
        finally
        {
            mutex.Release();
        }
    }

    public async Task<IReadOnlyList<ScoreSession>> GetLeaderboardAsync(int limit, CancellationToken cancellationToken)
    {
        await mutex.WaitAsync(cancellationToken);
        try
        {
            ScoreDatabase database = await ReadDatabaseInternalAsync(cancellationToken);
            return database.Sessions
                .Where(session => session.Status != SessionStatus.InProgress)
                .OrderByDescending(session => session.FinalScore)
                .ThenBy(session => session.EndedAtUtc ?? session.StartedAtUtc)
                .Take(Math.Max(1, limit))
                .ToList();
        }
        finally
        {
            mutex.Release();
        }
    }

    private async Task<ScoreDatabase> ReadDatabaseInternalAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return new ScoreDatabase();
        }

        await using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        ScoreDatabase? database = await JsonSerializer.DeserializeAsync<ScoreDatabase>(stream, serializerOptions, cancellationToken);
        return database ?? new ScoreDatabase();
    }

    private async Task WriteDatabaseInternalAsync(ScoreDatabase database, CancellationToken cancellationToken)
    {
        await using FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, database, serializerOptions, cancellationToken);
    }

    private sealed class ScoreDatabase
    {
        public List<ScoreSession> Sessions { get; set; } = new();
    }
}