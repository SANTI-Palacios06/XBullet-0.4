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
    [Tooltip("Si es true, cierra la aplicación al morir. Si es false, solo desactiva el objeto.")]
    [SerializeField] private bool quitOnDeath = false;
    [Tooltip("Segundos antes de cerrar la aplicación al morir.")]
    [SerializeField] private float quitDelay = 3f;

    /// Nombre público para que el HUD pueda mostrar a quién pertenece la barra.
    public string DisplayName => displayName;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

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
    }

    /// Recibe daño desde cualquier objeto que llame IDamageable.
    /// Clamp evita que la vida baje de cero.
    public void TakeDamage(int amount)
    {
        if (IsDead)
        {
            return;
        }
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Max(0, amount), 0, maxHealth);
        Debug.Log($"{displayName} recibió {amount} de daño. Vida: {currentHealth}/{maxHealth}");
        if (IsDead)
        {
            Die();
        }
    }

    /// Ui de la barra de vida, 
    public float GetCurrentPhaseFill()
    {
        int redPhaseHealth = GetRedPhaseHealth();
        int greenPhaseHealth = maxHealth - redPhaseHealth;
        if (currentHealth > redPhaseHealth)
        {
            return Mathf.InverseLerp(0, greenPhaseHealth, currentHealth - redPhaseHealth);
        }
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

    /// Resuelve la muerte del objeto.
    private void Die()
    {
        Debug.Log($"{displayName} murió.");
        gameObject.SetActive(false);
        
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

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}