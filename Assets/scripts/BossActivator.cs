using UnityEngine;

public class BossActivator : MonoBehaviour
{
    private PatternController patternController;

    private void Start()
    {
        patternController = Object.FindFirstObjectByType<PatternController>();

        if (patternController == null)
        {
            Debug.LogError("❌ BossActivator: No se encontró PatternController en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("✅ BossActivator: Trigger activado por el jugador.");
        
        if (patternController != null)
        {
            patternController.ActivatePatterns();
            Debug.Log("🚀 BossActivator: Patrones de disparo activados.");
        }

        Destroy(gameObject);
    }
}
