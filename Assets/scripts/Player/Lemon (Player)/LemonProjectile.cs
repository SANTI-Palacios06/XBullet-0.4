using UnityEngine;

/// Proyectil del jugador: dicta su movimiento y su autodestrucción
[RequireComponent(typeof(Rigidbody))]
public class LemonProjectile : MonoBehaviour
{
    [Header("Tiempo de vida")]
    [Tooltip("Segundos antes de destruirse si no choca con nada.")]
    [SerializeField] private float lifeTime = 2f;

    [Header("Movimiento")]
    [Tooltip("Velocidad por defecto. LemonShoot puede sobrescribirla al disparar.")]
    [SerializeField] private float defaultSpeed = 20f;
    [Tooltip("Deriva leve en el eje Z mientras viaja.")]
    [SerializeField] private float zDrift = 0.5f;

    [Header("Reglas de impacto")]
    [Tooltip("Layers que destruyen este proyectil al chocar.")]
    [SerializeField] private LayerMask destroyOnContactLayers;

    private Rigidbody rb;
    private Transform owner;
    private GameObject immuneObject;
    private int damage;
    private Vector3 travelDirection = Vector3.left;
    private float travelSpeed;
    private bool wasLaunched;

    // Creación del objeto
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        travelSpeed = defaultSpeed; // Arranca con la velocidad del Inspector.
    }

    // Llamado del proyectil frame por frame
    private void Update()
    {
        if (!wasLaunched) return;

        Vector3 movement = (travelDirection * travelSpeed + Vector3.forward * zDrift) * Time.deltaTime;
        transform.position += movement;
    }

    public void ConfigureCollisionLayers(LayerMask layers) => destroyOnContactLayers = layers;
    public void SetOwner(Transform newOwner) => owner = newOwner;
    public void SetImmuneObject(GameObject obj) => immuneObject = obj;

    // Configuración del lanzamiento del proyectil: daño fijo, velocidad, autodestrucción y dirección
    public void Launch(Vector3 direction, float speed, int projectileDamage)
    {
        damage = projectileDamage;
        travelSpeed = speed;
        travelDirection = direction.sqrMagnitude < 0.01f ? Vector3.left : direction.normalized;
        wasLaunched = true;
        Destroy(gameObject, lifeTime);
        Debug.Log($"Disparo lanzado hacia {travelDirection} con velocidad {speed}");
    }

    public int Damage => damage;
    public Transform Owner => owner;

    // Detección de colisiones
    private void OnTriggerEnter(Collider other) => HandleContact(other.gameObject);
    private void OnCollisionEnter(Collision collision) => HandleContact(collision.gameObject);

    // Maneja el control de colisiones del objeto al tocar
    private void HandleContact(GameObject other)
    {
        if (!wasLaunched) return;
        if (immuneObject != null && other == immuneObject) return;
        if (IsOwnerOrOwnerChild(other.transform)) return;

        if (IsInLayerMask(other.layer, destroyOnContactLayers))
        {
            Destroy(gameObject);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    // Verifica la familia del objeto tocado para ver si es hijo o padre
    private bool IsOwnerOrOwnerChild(Transform other)
    {
        if (owner == null || other == null) return false;
        return other == owner || other.IsChildOf(owner) || owner.IsChildOf(other);
    }
}