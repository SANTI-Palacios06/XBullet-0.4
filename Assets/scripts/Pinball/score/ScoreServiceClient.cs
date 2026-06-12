using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// Maneja la comunicación con el servidor de score.
public class ScoreServiceClient : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("URL base del servidor de score.")]
    [SerializeField] private string serverUrl = "http://127.0.0.1:8080";

    [Tooltip("Versión del cliente.")]
    [SerializeField] private string clientVersion = "1.0.0";

    private string sessionId = "";
    private bool sessionActive = false;
    private string connectionStatus = "Local";

    public static ScoreServiceClient Instance { get; private set; }

    public string SessionId => sessionId;
    public bool SessionActive => sessionActive;
    public string ConnectionStatus => connectionStatus;
    public string ServerUrl => serverUrl;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!string.IsNullOrWhiteSpace(serverUrl))
        {
            serverUrl = serverUrl.Replace("localhost", "127.0.0.1").TrimEnd('/');
        }

        Debug.Log($"ScoreServiceClient listo. serverUrl={serverUrl}");
    }

    public void Configure(string newServerUrl, string newClientVersion = null)
    {
        if (!string.IsNullOrWhiteSpace(newServerUrl))
        {
            serverUrl = newServerUrl.Replace("localhost", "127.0.0.1").TrimEnd('/');
        }

        if (!string.IsNullOrWhiteSpace(newClientVersion))
        {
            clientVersion = newClientVersion.Trim();
        }
    }

    /// Inicia una sesión en el servidor.
    public void BeginSession(string playerName, string stageName, int targetBossHits)
    {
        StartCoroutine(BeginSessionCoroutine(playerName, stageName, targetBossHits));
    }

    private IEnumerator BeginSessionCoroutine(string playerName, string stageName, int targetBossHits)
    {
        StartSessionRequest payload = new StartSessionRequest
        {
            playerName = string.IsNullOrWhiteSpace(playerName) ? "Jugador" : playerName.Trim(),
            stageName = string.IsNullOrWhiteSpace(stageName) ? "X-Bullet Pinball" : stageName.Trim(),
            targetBossHits = Mathf.Max(1, targetBossHits),
            clientVersion = clientVersion
        };

        using UnityWebRequest request = BuildJsonRequest($"{serverUrl}/api/sessions/start", "POST", payload);

        connectionStatus = "Conectando";
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            SessionResponse response = JsonUtility.FromJson<SessionResponse>(request.downloadHandler.text);

            sessionId = response.sessionId;
            sessionActive = !string.IsNullOrWhiteSpace(sessionId);
            connectionStatus = sessionActive ? "Servidor OK" : "Local";

            Debug.Log($"Sesión iniciada: {sessionId} | Jugador: {payload.playerName}");
        }
        else
        {
            sessionId = "";
            sessionActive = false;
            connectionStatus = "Local";

            Debug.LogWarning($"Error al iniciar sesión: {request.error}");
            Debug.LogWarning($"Respuesta servidor: {request.downloadHandler.text}");
        }
    }

    /// Reporta un evento de score al servidor.
    public void ReportEvent(string eventType, int points, int totalScore, string sourceName, int bossHits, bool bossDefeated)
    {
        if (!sessionActive || string.IsNullOrEmpty(sessionId))
        {
            Debug.LogWarning($"No se reportó el evento porque no hay sesión activa. Evento={eventType}");
            return;
        }

        StartCoroutine(ReportEventCoroutine(eventType, points, totalScore, sourceName, bossHits, bossDefeated));
    }

    private IEnumerator ReportEventCoroutine(string eventType, int points, int totalScore, string sourceName, int bossHits, bool bossDefeated)
    {
        ScoreEventRequest payload = new ScoreEventRequest
        {
            eventType = string.IsNullOrWhiteSpace(eventType) ? "unknown" : eventType.Trim(),
            points = Mathf.Max(0, points),
            totalScore = Mathf.Max(0, totalScore),
            sourceName = string.IsNullOrWhiteSpace(sourceName) ? "unknown" : sourceName.Trim(),
            bossHits = Mathf.Max(0, bossHits),
            bossDefeated = bossDefeated
        };

        using UnityWebRequest request = BuildJsonRequest($"{serverUrl}/api/sessions/{sessionId}/events", "POST", payload);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            connectionStatus = "Servidor OK";
            Debug.Log($"Evento reportado: {payload.eventType} +{payload.points} | Total: {payload.totalScore}");
        }
        else
        {
            connectionStatus = "Local";
            Debug.LogWarning($"Error al reportar evento: {request.error}");
            Debug.LogWarning($"Respuesta servidor: {request.downloadHandler.text}");
        }
    }

    /// Cierra la sesión con el score final y llama al callback cuando termina.
    public void CompleteSession(int finalScore, bool victory, string reason, int bossHits, Action onCompleted = null)
    {
        if (!sessionActive || string.IsNullOrEmpty(sessionId))
        {
            onCompleted?.Invoke();
            return;
        }

        StartCoroutine(CompleteSessionCoroutine(finalScore, victory, reason, bossHits, onCompleted));
    }

    private IEnumerator CompleteSessionCoroutine(int finalScore, bool victory, string reason, int bossHits, Action onCompleted)
    {
        FinishSessionRequest payload = new FinishSessionRequest
        {
            finalScore = Mathf.Max(0, finalScore),
            victory = victory,
            reason = string.IsNullOrWhiteSpace(reason) ? "unknown" : reason.Trim(),
            bossHits = Mathf.Max(0, bossHits)
        };

        using UnityWebRequest request = BuildJsonRequest($"{serverUrl}/api/sessions/{sessionId}/finish", "POST", payload);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            connectionStatus = "Partida sincronizada";
            Debug.Log($"Sesión completada. Victoria={victory} | Score final={finalScore}");
        }
        else
        {
            connectionStatus = "Local";
            Debug.LogWarning($"Error al completar sesión: {request.error}");
            Debug.LogWarning($"Respuesta servidor: {request.downloadHandler.text}");
        }

        sessionActive = false;
        sessionId = "";

        onCompleted?.Invoke();
    }

    /// Trae el leaderboard desde el backend.
    public void FetchLeaderboard(int limit, Action<List<LeaderboardEntry>> onSuccess, Action<string> onError = null)
    {
        int safeLimit = Mathf.Clamp(limit, 1, 100);
        StartCoroutine(FetchLeaderboardCoroutine(safeLimit, onSuccess, onError));
    }

    private IEnumerator FetchLeaderboardCoroutine(int limit, Action<List<LeaderboardEntry>> onSuccess, Action<string> onError)
    {
        string url = $"{serverUrl}/api/leaderboard?limit={limit}";

        using UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Accept", "application/json");

        connectionStatus = "Cargando leaderboard";

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            connectionStatus = "Local";

            string error = $"No se pudo cargar el leaderboard: {request.error}";
            Debug.LogWarning(error);
            Debug.LogWarning($"Respuesta servidor: {request.downloadHandler.text}");

            onError?.Invoke(error);
            yield break;
        }

        connectionStatus = "Servidor OK";

        string rawJson = request.downloadHandler.text;

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            onSuccess?.Invoke(new List<LeaderboardEntry>());
            yield break;
        }

        string wrappedJson = "{\"items\":" + rawJson + "}";

        LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(wrappedJson);

        List<LeaderboardEntry> entries = response != null && response.items != null
            ? new List<LeaderboardEntry>(response.items)
            : new List<LeaderboardEntry>();

        Debug.Log($"Leaderboard cargado. Registros={entries.Count}");
        onSuccess?.Invoke(entries);
    }

    /// Verifica la conexión con el servidor.
    public void CheckHealth()
    {
        StartCoroutine(CheckHealthCoroutine());
    }

    private IEnumerator CheckHealthCoroutine()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{serverUrl}/health");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            connectionStatus = "Servidor OK";
            Debug.Log($"Servidor OK: {request.downloadHandler.text}");
        }
        else
        {
            connectionStatus = "Local";
            Debug.LogWarning($"Servidor no disponible: {request.error}");
        }
    }

    private UnityWebRequest BuildJsonRequest(string url, string method, object payload)
    {
        string json = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, method);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        return request;
    }

    private void OnApplicationQuit()
    {
        if (sessionActive && !string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("Cierre detectado. Intentando cerrar sesión en el servidor.");
            StartCoroutine(CompleteSessionCoroutine(0, false, "app_quit", 0, null));
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && sessionActive && !string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("Pausa detectada. Intentando cerrar sesión en el servidor.");
            StartCoroutine(CompleteSessionCoroutine(0, false, "app_pause", 0, null));
        }
    }

    [Serializable]
    private class StartSessionRequest
    {
        public string playerName;
        public string stageName;
        public int targetBossHits;
        public string clientVersion;
    }

    [Serializable]
    private class ScoreEventRequest
    {
        public string eventType;
        public int points;
        public int totalScore;
        public string sourceName;
        public int bossHits;
        public bool bossDefeated;
    }

    [Serializable]
    private class FinishSessionRequest
    {
        public int finalScore;
        public bool victory;
        public string reason;
        public int bossHits;
    }

    [Serializable]
    private class SessionResponse
    {
        public string sessionId;
        public string playerName;
        public string stageName;
        public int currentScore;
        public int bossHits;
        public int targetBossHits;
        public string status;
        public string startedAtUtc;
        public string endedAtUtc;
    }
}

[Serializable]
public class LeaderboardEntry
{
    public string sessionId;
    public string playerName;
    public string stageName;
    public int finalScore;
    public int bossHits;
    public int targetBossHits;
    public string status;
    public string startedAtUtc;
    public string endedAtUtc;
    public string reason;
}

[Serializable]
public class LeaderboardResponse
{
    public LeaderboardEntry[] items;
}