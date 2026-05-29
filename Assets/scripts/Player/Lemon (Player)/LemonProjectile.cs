using UnityEngine;

//Manejo del proyectil, maneja su impacti, movimiento, direccion, impacto, interraccion con proyectil enemigo y gvelocidad

[RequireComponent(typeof(Rigidbody))]
public class LemonProjectile : MonoBehaviour
{
    [Header("Tiempo de vida")]
    [SerializeField] private float lifeTime = 2f;

    [Header("Movimiento")]
    [SerializeField] private float defaultSpeed = 20f;
    [SerializeField] private float zDrift = 0.5f;

    [Header("Reglas de impacto")]
    [SerializeField] private LayerMask destroyOnContactLayers;

    [Header("Intercepción de proyectiles enemigos")]
    [Tooltip("Tag que deben tener los proyectiles enemigos para ser destruidos al contacto.")]
    [SerializeField] private string enemyProjectileTag = "EnemyProjectile";
    [Tooltip("Si es true, este proyectil también se destruye al interceptar uno enemigo.")]
    [SerializeField] private bool destroySelfOnIntercept = true;

    private Rigidbody rb;
    private Transform owner;
    private GameObject immuneObject;
    private int damage;
    private Vector3 travelDirection = Vector3.left;
    private float travelSpeed;
    private bool wasLaunched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        travelSpeed = defaultSpeed;
    }

    private void Update()
    {
        if (!wasLaunched) return;
        Vector3 movement = (travelDirection * travelSpeed + Vector3.forward * zDrift) * Time.deltaTime;
        transform.position += movement;
    }

    public void ConfigureCollisionLayers(LayerMask layers) => destroyOnContactLayers = layers;
    public void SetOwner(Transform newOwner) => owner = newOwner;
    public void SetImmuneObject(GameObject obj) => immuneObject = obj;

//Velocidad de lanzamiento del proyectil
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

    private void OnTriggerEnter(Collider other) => HandleContact(other.gameObject);
    private void OnCollisionEnter(Collision collision) => HandleContact(collision.gameObject);


//Maneja el contacto del proyectil, su interraccion y destruccion. tambien maneja la destruccion del enemigo 
    private void HandleContact(GameObject other)
    {
        if (!wasLaunched) return;
        if (immuneObject != null && other == immuneObject) return;
        if (IsOwnerOrOwnerChild(other.transform)) return;
        if (!string.IsNullOrEmpty(enemyProjectileTag) && other.CompareTag(enemyProjectileTag))
        {
            Destroy(other);
            if (destroySelfOnIntercept) Destroy(gameObject);
            return;
        }

        if (IsInLayerMask(other.layer, destroyOnContactLayers))
        {
            Destroy(gameObject);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    private bool IsOwnerOrOwnerChild(Transform other)
    {
        if (owner == null || other == null) return false;
        return other == owner || other.IsChildOf(owner) || owner.IsChildOf(other);
    }
}