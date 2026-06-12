using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Orquesta el puntaje del modo pinball.
/// Centraliza el score para que bumpers, palancas, pelota y fin de partida
/// no se conozcan entre sí.
public class PinballScoreManager : MonoBehaviour
{
    [Header("Puntajes base")]
    [Tooltip("Puntos por cada impulso válido de una palanca sobre la pelota.")]
    [SerializeField] private int flipperPoints = 200;

    [Tooltip("Puntos por cada golpe de la pelota sobre el jefe.")]
    [SerializeField] private int bossHitPoints = 500;

    [Tooltip("Puntos al derrotar al jefe.")]
    [SerializeField] private int bossDefeatPoints = 1000;

    [Tooltip("Puntos por cada proyectil enemigo destruido.")]
    [SerializeField] private int enemyProjectilePoints = 50;

    [Tooltip("Puntos por cada rebote en un bumper.")]
    [SerializeField] private int bumperPoints = 100;

    [Tooltip("Puntos bonus por cada vida restante del jugador al finalizar.")]
    [SerializeField] private int bonusPerLife = 2000;

    [Header("Autobúsqueda")]
    [Tooltip("Tag del jugador. El objeto con este tag debe tener CombatHealth.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Escenas")]
    [Tooltip("Nombre exacto de la escena del menú.")]
    [SerializeField] private string menuSceneName = "Menu";

    [Header("Sesión")]
    [SerializeField] private int currentScore;

    private CombatHealth playerHealth;
    private CombatHealth enemyHealth;
    private bool sessionClosed;

    // Nombre del jugador cargado desde PlayerPrefs, Dummy si no existe
    private string playerName = "Dummy";

    // Contador de golpes al jefe para reportar al servidor
    private int bossHitCount = 0;

    // Cliente de score para comunicación con el servidor
    private ScoreServiceClient scoreClient;

    public static PinballScoreManager Instance { get; private set; }

    public event Action ScoreChanged;

    public int CurrentScore   => currentScore;
    public bool SessionClosed => sessionClosed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Carga el nombre guardado desde el menú, usa Dummy si no existe o está vacío
        string savedName = PlayerPrefs.GetString("PlayerName", "Dummy");
        playerName = string.IsNullOrWhiteSpace(savedName) ? "Dummy" : savedName.Trim();

        Debug.Log($"PinballScoreManager — Jugador cargado: '{playerName}'");

        // Busca el cliente de score e inicia la sesión en el servidor
        scoreClient = ScoreServiceClient.Instance;
        if (scoreClient != null)
            scoreClient.BeginSession(playerName, "X-Bullet Pinball", 3);
        else
            Debug.LogWarning("PinballScoreManager — ScoreServiceClient no encontrado.");

        if (playerHealth == null)
        {
            TryFindPlayerHealth();
        }
    }

    /// Configura referencias de vida y eventos de muerte.
    public void Configure(CombatHealth newPlayerHealth, CombatHealth newEnemyHealth)
    {
        UnsubscribeHealthEvents();

        playerHealth  = newPlayerHealth;
        enemyHealth   = newEnemyHealth;
        currentScore  = 0;
        sessionClosed = false;
        bossHitCount  = 0;

        if (playerHealth != null)
        {
            playerHealth.Died += OnPlayerDied;
        }
        else
        {
            TryFindPlayerHealth();

            if (playerHealth != null)
            {
                playerHealth.Died += OnPlayerDied;
            }
        }

        if (enemyHealth != null)
        {
            enemyHealth.Died += OnEnemyDied;
        }

        Debug.Log($"Score inicial: {currentScore}");

        ScoreChanged?.Invoke();
    }

    // Busca la vida del jugador para registrar los puntos extras de la vida del jugador
    private bool TryFindPlayerHealth()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            CombatHealth foundPlayerHealth = playerObject.GetComponent<CombatHealth>();

            if (foundPlayerHealth != null)
            {
                playerHealth = foundPlayerHealth;
                return true;
            }

            foundPlayerHealth = playerObject.GetComponentInParent<CombatHealth>();

            if (foundPlayerHealth != null)
            {
                playerHealth = foundPlayerHealth;
                return true;
            }

            foundPlayerHealth = playerObject.GetComponentInChildren<CombatHealth>();

            if (foundPlayerHealth != null)
            {
                playerHealth = foundPlayerHealth;
                return true;
            }
        }

        //ignorar, es una advertencia menor
        CombatHealth[] allHealths = FindObjectsByType<CombatHealth>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (CombatHealth health in allHealths)
        {
            if (health.CompareTag(playerTag))
            {
                playerHealth = health;
                return true;
            }
        }

        return false;
    }

    //Registra lo referente a los golpes del flipper
    public void RegisterFlipperImpulse()
    {
        AddScore(flipperPoints, "flipper_impulse");
    }

    //Se registro el golpe del jefe
    public void RegisterBossHit()
    {
        bossHitCount++;
        AddScore(bossHitPoints, "boss_hit");
    }

    //Registra la destruccion del proyectil enemigo
    public void RegisterEnemyProjectileDestroyed()
    {
        AddScore(enemyProjectilePoints, "enemy_projectile_destroyed");
    }

    //Registra el rebote en el bumper
    public void RegisterBumperBounce()
    {
        AddScore(bumperPoints, "bumper_bounce");
    }

    //Publica los puntos del score
    private void AddScore(int points, string eventType)
    {
        if (sessionClosed)
        {
            return;
        }

        int safePoints = Mathf.Max(0, points);
        currentScore += safePoints;

        Debug.Log($"Score +{safePoints} | Evento={eventType} | Total={currentScore}");

        // Reporta el evento al servidor
        if (scoreClient != null)
            scoreClient.ReportEvent(eventType, safePoints, currentScore, playerName, bossHitCount, false);

        ScoreChanged?.Invoke();
    }

    //estado donde el enemigo muere
    private void OnEnemyDied(CombatHealth deadEnemy)
    {
        NotifyEnemyDefeated(deadEnemy);
    }

    //Estado de la muerte del jugador
    private void OnPlayerDied(CombatHealth deadPlayer)
    {
        NotifyPlayerDefeated(deadPlayer);
    }

    //Notificacion del enemigo derrotado
    public void NotifyEnemyDefeated(CombatHealth deadEnemy)
    {
        if (sessionClosed)
        {
            return;
        }

        AddScore(bossDefeatPoints, "boss_defeated");
        FinishSession(true);
    }

    //Notificacion del jugador derrotado
    public void NotifyPlayerDefeated(CombatHealth deadPlayer)
    {
        if (sessionClosed)
        {
            return;
        }

        FinishSession(false);
    }

    //Seccion terminada y cuenta puntos extra de salud del jugador
    private void FinishSession(bool victory)
    {
        if (sessionClosed)
        {
            return;
        }

        // Re-lee el nombre por si no se cargó correctamente en Start
        if (playerName == "Dummy" || string.IsNullOrWhiteSpace(playerName))
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "Dummy");
            playerName = string.IsNullOrWhiteSpace(savedName) ? "Dummy" : savedName.Trim();
        }

        if (victory)
        {
            if (playerHealth == null)
            {
                TryFindPlayerHealth();
            }

            if (playerHealth != null)
            {
                int livesRemaining = Mathf.Max(0, playerHealth.CurrentHealth);
                int lifeBonus      = livesRemaining * bonusPerLife;

                currentScore += lifeBonus;

                Debug.Log($"Bonus por vidas restantes: {livesRemaining} vidas x {bonusPerLife} = +{lifeBonus}");
            }
        }

        sessionClosed = true;

        string resultLabel = victory ? "VICTORIA" : "DERROTA";

        Debug.Log("=============================");
        Debug.Log($"{resultLabel}");
        Debug.Log($"El score del jugador {playerName} es de {currentScore}");
        Debug.Log("=============================");

        ScoreChanged?.Invoke();

        // Guarda en el servidor en paralelo
        if (scoreClient != null)
            scoreClient.CompleteSession(currentScore, victory, victory ? "boss_defeated" : "player_dead", bossHitCount);

        // Espera a que termine el audio de resultado antes de cargar el menú
        float audioDelay = SoundManager.GetClipLength(victory ? SoundType.victory : SoundType.defeat);
        StartCoroutine(LoadMenuAfterDelay(audioDelay));
    }

    // Espera a que termine el audio y carga el menú
    private IEnumerator LoadMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        UnsubscribeHealthEvents();
    }

    private void UnsubscribeHealthEvents()
    {
        if (playerHealth != null)
        {
            playerHealth.Died -= OnPlayerDied;
        }

        if (enemyHealth != null)
        {
            enemyHealth.Died -= OnEnemyDied;
        }
    }
}