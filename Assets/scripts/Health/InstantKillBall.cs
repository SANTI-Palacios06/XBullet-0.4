using UnityEngine;

/// Cuando la pelota es desactivada o destruida, mata al jugador.
public class InstantKillBall : MonoBehaviour
{
    [Tooltip("El jugador que será eliminado cuando la pelota desaparezca.")]
    [SerializeField] private CombatHealth playerHealth;

    private void OnDisable()
    {
        if (playerHealth == null) return;
        if (playerHealth.IsDead) return;
        playerHealth.TakeDamage(playerHealth.MaxHealth);
    }
}