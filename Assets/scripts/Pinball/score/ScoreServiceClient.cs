using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// Maneja la comunicación con el servidor de score.
public class ScoreServiceClient : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("URL base del servidor de score.")]
    [SerializeField] private string serverUrl = "http://localhost:8080";

    [Tooltip("Versión del cliente.")]
    [SerializeField] private string clientVersion = "1.0.0";

    private string sessionId   = "";
    private bool sessionActive = false;

    public static ScoreServiceClient Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// Inicia una sesión en el servidor
    public void BeginSession(string playerName, string stageName, int targetBossHits)
    {
        StartCoroutine(BeginSessionCoroutine(playerName, stageName, targetBossHits));
    }

    private IEnumerator BeginSessionCoroutine(string playerName, string stageName, int targetBossHits)
    {
        string json = $"{{\"playerName\":\"{playerName}\",\"stageName\":\"{stageName}\",\"targetBossHits\":{targetBossHits},\"clientVersion\":\"{clientVersion}\"}}";

        using UnityWebRequest request = new UnityWebRequest($"{serverUrl}/api/sessions/start", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            SessionResponse response = JsonUtility.FromJson<SessionResponse>(request.downloadHandler.text);
            sessionId     = response.sessionId;
            sessionActive = true;
            Debug.Log($"Sesión iniciada: {sessionId} | Jugador: {playerName}");
        }
        else
        {
            Debug.LogWarning($"Error al iniciar sesión: {request.error}");
        }
    }

    /// Reporta un evento de score al servidor
    public void ReportEvent(string eventType, int points, int totalScore, string sourceName, int bossHits, bool bossDefeated)
    {
        if (!sessionActive || string.IsNullOrEmpty(sessionId)) return;
        StartCoroutine(ReportEventCoroutine(eventType, points, totalScore, sourceName, bossHits, bossDefeated));
    }

    private IEnumerator ReportEventCoroutine(string eventType, int points, int totalScore, string sourceName, int bossHits, bool bossDefeated)
    {
        string json = $"{{\"eventType\":\"{eventType}\",\"points\":{points},\"totalScore\":{totalScore},\"sourceName\":\"{sourceName}\",\"bossHits\":{bossHits},\"bossDefeated\":{bossDefeated.ToString().ToLower()}}}";

        using UnityWebRequest request = new UnityWebRequest($"{serverUrl}/api/sessions/{sessionId}/events", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log($"Evento reportado: {eventType} +{points} | Total: {totalScore}");
        else
            Debug.LogWarning($"Error al reportar evento: {request.error}");
    }

    /// Cierra la sesión con el score final
    public void CompleteSession(int finalScore, bool victory, string reason, int bossHits)
    {
        if (!sessionActive || string.IsNullOrEmpty(sessionId)) return;
        StartCoroutine(CompleteSessionCoroutine(finalScore, victory, reason, bossHits));
    }

    private IEnumerator CompleteSessionCoroutine(int finalScore, bool victory, string reason, int bossHits)
    {
        string json = $"{{\"finalScore\":{finalScore},\"victory\":{victory.ToString().ToLower()},\"reason\":\"{reason}\",\"bossHits\":{bossHits}}}";

        using UnityWebRequest request = new UnityWebRequest($"{serverUrl}/api/sessions/{sessionId}/finish", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Sesión completada. Victoria={victory} | Score final={finalScore}");
            sessionActive = false;
        }
        else
        {
            Debug.LogWarning($"Error al completar sesión: {request.error}");
        }
    }

    /// Verifica la conexión con el servidor
    public void CheckHealth()
    {
        StartCoroutine(CheckHealthCoroutine());
    }

    private IEnumerator CheckHealthCoroutine()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{serverUrl}/health");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log($"Servidor OK: {request.downloadHandler.text}");
        else
            Debug.LogWarning($"Servidor no disponible: {request.error}");
    }

    // Detecta cierre abrupto de la aplicación
    private void OnApplicationQuit()
    {
        if (sessionActive && !string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("Cierre detectado — cerrando sesión en el servidor.");
            StartCoroutine(CompleteSessionCoroutine(0, false, "app_quit", 0));
        }
    }

    // Detecta pausa abrupta de la aplicación
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && sessionActive && !string.IsNullOrEmpty(sessionId))
        {
            Debug.Log("Pausa detectada — cerrando sesión en el servidor.");
            StartCoroutine(CompleteSessionCoroutine(0, false, "app_pause", 0));
        }
    }

    [Serializable]
    private class SessionResponse
    {
        public string sessionId;
        public string playerName;
        public string stageName;
        public int currentScore;
        public int bossHits;
        public string status;
    }
}