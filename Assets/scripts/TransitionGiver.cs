using UnityEngine;

public class TransitionGiver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.GetComponent<MoveOnEnable>() == null)
        {
            other.gameObject.AddComponent<MoveOnEnable>();
            Debug.Log("✅ Script MoveOnEnable agregado exitosamente al jugador.");
        }
        else
        {
            Debug.Log("ℹ️ El jugador ya tiene el script MoveOnEnable.");
        }
    }
}
