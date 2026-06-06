using UnityEngine;

public class InstantKillBall : MonoBehaviour
{
    [Tooltip("El jugador que será eliminado cuando la pelota desaparezca.")]
    [SerializeField] private CombatHealth playerHealth;

    private void OnDisable()
    {
        if (!Application.isPlaying) return;

     
        if (PinballScoreManager.Instance != null &&
            PinballScoreManager.Instance.SessionClosed)
        {
            Debug.Log("InstantKillBall ignorado: la sesión ya estaba cerrada.");
            return;
        }

        if (playerHealth == null) return;
        if (playerHealth.IsDead) return;

        Debug.Log("InstantKillBall ejecutado: la pelota desapareció y el jugador pierde.");
        playerHealth.TakeDamage(playerHealth.MaxHealth);
    }
}