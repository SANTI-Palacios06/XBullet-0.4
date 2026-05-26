using UnityEngine;

public class ActivatePatternTrigger : MonoBehaviour
{
    private MovementPattern movementScript;

    private void Start()
    {
        // Buscar el primer MovementPattern de forma moderna (Unity 2023+)
        movementScript = Object.FindFirstObjectByType<MovementPattern>();

        if (movementScript == null)
        {
            Debug.LogWarning("⚠ No se encontró ningún MovementPattern en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (movementScript != null)
        {
            movementScript.ActivateMovement();
            Debug.Log("✅ Movimiento en patrón activado por trigger.");
        }
    }
}
