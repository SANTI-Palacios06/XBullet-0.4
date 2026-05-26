using UnityEngine;

public class ProximityBlocker : MonoBehaviour
{
    [Header("Radio de detección de proximidad")]
    [SerializeField] private float detectionRadius = 1.5f;

    [Header("Objetos que no deben atravesar")]
    [SerializeField] private string targetTag = "Player";

    [Header("Fuerza de empuje")]
    [SerializeField] private float pushBackForce = 5f;

    private void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null)
                {
                    Vector3 directionAway = (hit.transform.position - transform.position).normalized;
                    rb.AddForce(directionAway * pushBackForce, ForceMode.VelocityChange);
                }
            }
        }
    }

    // Opcional: Visualiza el radio en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
