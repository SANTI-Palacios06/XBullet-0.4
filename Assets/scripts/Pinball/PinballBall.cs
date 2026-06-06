using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PinballBall : MonoBehaviour
{
    [Header("Lanzamiento")]
    [SerializeField] private Vector3 initialImpulse = new Vector3(0f, 4f, 8.5f);

    [Header("Atracción")]
    [SerializeField] private float attractForce = 6f;

    [Header("Velocidad")]
    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float maxSpeed = 18f;

    [Header("Antiatasque")]
    [Tooltip("Tiempo en segundos sin moverse antes de expulsar la pelota.")]
    [SerializeField] private float stuckTime = 1f;
    [Tooltip("Fuerza de expulsión hacia arriba cuando se detecta que está atascada.")]
    [SerializeField] private float unstuckForce = 15f;

    [Header("Daño al jefe")]
    [SerializeField] private int damageOnHit = 10;
    [Tooltip("Tag que debe tener el jefe para recibir daño.")]
    [SerializeField] private string bossTag = "boss";

    private Rigidbody rb;
    private bool isActive = true;
    private Vector3 lastPosition;
    private float stuckTimer;

    public static event System.Action<PinballBall> OnDrain;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Launch();
    }

    // Ignora colisión con el jugador y destruye la pelota si toca Destroyer
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destroyer"))
        {
            Drain();
            return;
        }

        // Daña al jefe si la pelota lo toca y registra score
        if (collision.gameObject.CompareTag(bossTag))
        {
            IDamageable target = collision.gameObject.GetComponent<IDamageable>();
            PinballScoreManager.Instance?.RegisterBossHit();
            target?.TakeDamage(damageOnHit);
            return;
        }

        if (!collision.gameObject.CompareTag("Player")) return;

        Collider ballCollider = GetComponentInChildren<Collider>();
        if (ballCollider == null) return;

        Physics.IgnoreCollision(ballCollider, collision.collider, true);
    }

    // Comprueba el tiempo que la pelota lleva estancada
    private void Update()
    {
        if (!isActive) return;

        if (Vector3.Distance(rb.position, lastPosition) < 0.05f)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTime)
            {
                Unstuck();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
            lastPosition = rb.position;
        }
    }

    // Expulsa la pelota cuando está estancada
    private void Unstuck()
    {
        //rb.linearVelocity  = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        rb.AddForce(new Vector3(0f, 0f, -unstuckForce), ForceMode.Impulse);
        Debug.Log("Pelota atascada, expulsando.");
    }

    // Maneja la atracción y velocidad de la pelota
    private void FixedUpdate()
    {
        if (!isActive) return;

        ApplyAttraction();
        ClampSpeed();
    }

    // Empuja hacia la parte de abajo del mapa
    private void ApplyAttraction()
    {
        rb.AddForce(new Vector3(0f, 0f, attractForce), ForceMode.Force);
    }

    // Controla la velocidad para que no se salga fuera de control
    private void ClampSpeed()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed > 0.01f && speed < minSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * minSpeed;
        else if (speed > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    // Reinicia la pelota
    public void Launch()
    {
        isActive = true;
        gameObject.SetActive(true);
        rb.linearVelocity  = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(initialImpulse, ForceMode.Impulse);
        lastPosition = rb.position;
        stuckTimer   = 0f;
    }

    // Detiene la pelota cuando cae fuera del tablero
    public void Drain()
    {
        if (!isActive) return;
        isActive = false;
        rb.linearVelocity  = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        OnDrain?.Invoke(this);
        gameObject.SetActive(false);
    }

    // Activa el comportamiento de la pelota
    public void ActivateBall()
    {
        isActive = true;
    }

    // Detiene todo movimiento sin desactivar el objeto
    public void DeactivateBall()
    {
        isActive = false;
        rb.linearVelocity  = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Reinicia la pelota a su estado inicial
    public void ResetBall()
    {
        isActive = true;
        rb.linearVelocity  = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        lastPosition = rb.position;
        stuckTimer   = 0f;
        Debug.Log("Pelota reiniciada correctamente.");
    }
}