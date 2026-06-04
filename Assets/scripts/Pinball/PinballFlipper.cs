using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Palanca de pinball controlada por InputAction.
/// El Rigidbody es cinemático para empujar la pelota de forma estable.
/// Cada palanca recibe sus ángulos y botón mediante Configure.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PinballFlipper : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private bool isLeftFlipper;

    [Header("Ángulos")]
    [SerializeField] private float restAngle;
    [SerializeField] private float activeAngle;

    [Header("Movimiento")]
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Impulso")]
    [Tooltip("Velocidad de salida de la pelota en m/s. La masa se toma del Rigidbody de la pelota.")]
    [SerializeField] private float launchSpeed = 40f;
    [Tooltip("Multiplicador extra sobre la masa para ajuste fino.")]
    [SerializeField] private float massMultiplier = 2f;

    [Header("Input")]
    [SerializeField] private InputActionReference flipperAction;

    private Rigidbody rb;
    private bool hasKicked = false;

    /// <summary>
    /// Guarda el Rigidbody y lo configura como cinemático.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        if (flipperAction != null)
            flipperAction.action.Enable();
    }

    private void OnDisable()
    {
        if (flipperAction != null)
            flipperAction.action.Disable();
    }

    /// <summary>
    /// Configura orientación y lado de la palanca.
    /// </summary>
    public void Configure(bool newIsLeftFlipper, float newRestAngle, float newActiveAngle)
    {
        isLeftFlipper = newIsLeftFlipper;
        restAngle = newRestAngle;
        activeAngle = newActiveAngle;
        transform.rotation = Quaternion.Euler(0f, restAngle, 0f);
    }

    /// <summary>
    /// Rota gradualmente hacia el ángulo activo o de reposo según el input.
    /// La rotación es sobre Y porque el tablero está en el plano XZ.
    /// Resetea hasKicked cuando se suelta el botón.
    /// </summary>
    private void FixedUpdate()
    {
        float targetAngle = IsPressed() ? activeAngle : restAngle;
        float current = rb.rotation.eulerAngles.y;
        float nextAngle = Mathf.MoveTowardsAngle(current, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(0f, nextAngle, 0f));

        // Cuando se suelta el botón se permite volver a golpear
        if (!IsPressed())
            hasKicked = false;
    }

    /// <summary>
    /// Aplica el impulso una sola vez por pulsación.
    /// La fuerza se calcula con la masa de la pelota para que
    /// siempre salga a la misma velocidad sin importar cuánto pese.
    /// Fuerza = masa * velocidad deseada (impulso = cambio de momento)
    /// </summary>
    private void OnCollisionStay(Collision collision)
    {
        if (hasKicked) return;

        PinballBall ball = collision.gameObject.GetComponent<PinballBall>();
        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();

        if (ball == null || ballRb == null || !IsPressed())
            return;

        hasKicked = true;

        // Limpiamos velocidad Z para que el impulso se sienta limpio
        Vector3 vel = ballRb.linearVelocity;
        vel.z = 0f;
        ballRb.linearVelocity = vel;

        float horizontalDirection = isLeftFlipper ? 0.3f : -0.3f;
        Vector3 impulseDirection = new Vector3(horizontalDirection, 0f, -1f).normalized;

        // Fuerza = masa * velocidad deseada * multiplicador
        float force = ballRb.mass * launchSpeed * massMultiplier;
        ballRb.AddForce(impulseDirection * force, ForceMode.Impulse);

        Debug.Log($"Flipper golpeó la pelota. Masa: {ballRb.mass} | Fuerza aplicada: {force}");
    }

    /// <summary>
    /// Lee el estado del InputAction asignado en el Inspector.
    /// </summary>
    private bool IsPressed()
    {
        if (flipperAction == null || flipperAction.action == null)
            return false;

        return flipperAction.action.IsPressed();
    }
}