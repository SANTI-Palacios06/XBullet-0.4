using System;
using UnityEngine;

/// Vida reutilizable para jugador, enemigo o futuros objetos dañables.
/// Implementa IDamageable para que cualquier proyectil pueda aplicar daño a cualquier cosa sin importar que sea
public class CombatHealth : MonoBehaviour, IDamageable
{
    [Header("Identidad")]
    [Tooltip("Nombre que se mostrará en el HUD.")]
    [SerializeField] private string displayName = "Objetivo";

    [Header("Vida")]
    [Tooltip("Vida máxima del objeto.")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("Vida actual. Se inicializa con maxHealth.")]
    [SerializeField] private int currentHealth;

    [Header("Al morir")]
    [Tooltip("Si es true, cierra la aplicación al morir. Solo aplica a objetos sin tag Player o boss.")]
    [SerializeField] private bool quitOnDeath = false;
    [Tooltip("Segundos antes de cerrar la aplicación al morir.")]
    [SerializeField] private float quitDelay = 3f;

    /// Nombre público para que el HUD pueda mostrar a quién pertenece la barra.
    public string DisplayName => displayName;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    /// Evento que se dispara cuando el objeto muere.
    public event Action<CombatHealth> Died;

    /// Flag para que el sonido crítico suene solo una vez al bajar del umbral.
    private bool criticalSoundPlayed = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// Permite configurar vida y nombre desde el constructor de escena o demo runtime.
    public void Configure(string newDisplayName, int newMaxHealth)
    {
        displayName = newDisplayName;
        maxHealth = Mathf.Max(1, newMaxHealth);
        currentHealth = maxHealth;
        criticalSoundPlayed = false;
    }

    /// Recibe daño desde cualquier objeto que llame IDamageable.
    /// Clamp evita que la vida baje de cero.
    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Max(0, amount), 0, maxHealth);
        Debug.Log($"{displayName} recibió {amount} de daño. Vida: {currentHealth}/{maxHealth}");

        // Suena criticalHealth en loop una sola vez al bajar del umbral de 1/3 de vida
        if (CompareTag("Player") && !criticalSoundPlayed && currentHealth <= Mathf.CeilToInt(maxHealth / 3f))
        {
            criticalSoundPlayed = true;
            SoundManager.StartCriticalSound();
        }

        if (IsDead) Die();
    }

    /// Ui de la barra de vida
    public float GetCurrentPhaseFill()
    {
        int redPhaseHealth   = GetRedPhaseHealth();
        int greenPhaseHealth = maxHealth - redPhaseHealth;
        if (currentHealth > redPhaseHealth)
            return Mathf.InverseLerp(0, greenPhaseHealth, currentHealth - redPhaseHealth);

        return Mathf.InverseLerp(0, redPhaseHealth, currentHealth);
    }

    /// Color de la barra de salud Verde cuando aún queda la primera fase; rojo cuando solo queda la fase final.
    public Color GetCurrentPhaseColor()
    {
        return currentHealth > GetRedPhaseHealth()
            ? new Color(0.2f, 0.9f, 0.35f)
            : new Color(0.95f, 0.18f, 0.18f);
    }

    /// Calcula la mitad roja de la vida, para funciones de poca vida.
    private int GetRedPhaseHealth()
    {
        return Mathf.CeilToInt(maxHealth * 0.5f);
    }

    /// Congela la pelota de pinball
    private void FreezeBall()
    {
        GameObject ball = GameObject.FindWithTag("PinballBall");
        if (ball == null) return;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb == null) return;
        //rb.linearVelocity  = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        rb.isKinematic     = true;
    }

    /// Resuelve los casos de victoria y derrota del jugador, igualmente4 maneja el socre tras morir y el sonido de victoria y derrota.
    private void Die()
    {
        int subscribers = Died?.GetInvocationList().Length ?? 0;
        Debug.Log($"{displayName} entró a Die(). Suscriptores al evento Died: {subscribers}");

        Died?.Invoke(this);

        if (CompareTag("Player"))
        {
            if (PinballScoreManager.Instance != null)
            {
                PinballScoreManager.Instance.NotifyPlayerDefeated(this);
            }
            else
            {
                Debug.LogWarning("No existe PinballScoreManager.Instance en la escena.");
            }

            Debug.Log("DERROTA — el jugador ha muerto.");
            FreezeBall();

            if (MusicManager.Instance != null)
                MusicManager.Instance.StopMusic();

            SoundManager.StopAllSounds();
            SoundManager.PlayResultSound(SoundType.defeat);

            float delay = SoundManager.GetClipLength(SoundType.defeat);
            Invoke(nameof(QuitGame), delay);
        }
        else if (CompareTag("boss"))
        {
            if (PinballScoreManager.Instance != null)
            {
                PinballScoreManager.Instance.NotifyEnemyDefeated(this);
            }
            else
            {
                Debug.LogWarning("No existe PinballScoreManager.Instance en la escena.");
            }

            Debug.Log("VICTORIA — el jefe ha sido derrotado.");
            FreezeBall();

            DestroyAllLemonEvilsTrigger.DestroyAllLemonEvils();

            if (MusicManager.Instance != null)
                MusicManager.Instance.StopMusic();

            SoundManager.StopAllSounds();
            SoundManager.PlayResultSound(SoundType.victory);

            float delay = SoundManager.GetClipLength(SoundType.victory);
            Invoke(nameof(QuitGame), delay);
        }
        else
        {
            Debug.Log($"Muerte neutral — tag: {tag}");

            if (GetComponent<LemonShoot>() != null)
            {
                SoundManager.StopChargeSound();
                SoundManager.StopAllSounds();
            }

            if (quitOnDeath)
            {
                Invoke(nameof(QuitGame), quitDelay);
            }
        }

        gameObject.SetActive(false);
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}