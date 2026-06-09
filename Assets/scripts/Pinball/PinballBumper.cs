using UnityEngine;

/// Bumper de pinball que empuja la pelota al contacto y registra score.
[RequireComponent(typeof(Collider))]
public class PinballBumper : MonoBehaviour
{
    [Header("Impulso")]
    [Tooltip("Fuerza con la que el bumper empuja la pelota.")]
    [SerializeField] private float pushForce = 15f;

    [Header("Configuración")]
    [Tooltip("Tag que debe tener la pelota para ser empujada.")]
    [SerializeField] private string ballTag = "PinballBall";

    private void Awake()
    {
        // Contact: NO es trigger
        GetComponent<Collider>().isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(ballTag)) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Calcula dirección de empuje desde el bumper hacia la pelota
        Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
        rb.AddForce(pushDirection * pushForce, ForceMode.VelocityChange);

        Debug.Log($"Bumper golpeó la pelota. Fuerza: {pushForce}");

        // Registra score
        PinballScoreManager.Instance?.RegisterBumperBounce();
    }
}