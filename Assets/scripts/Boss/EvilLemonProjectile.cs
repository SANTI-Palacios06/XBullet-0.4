using UnityEngine;

/// Se encarga de las propiedades del proyectil, incluyendo su movimiento, cooldown y preparación para el daño al jugador.
public class EvilLemonProjectile : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 20f;

    [Header("Tiempo de vida")]
    [Tooltip("Segundos antes de destruirse si no choca con nada.")]
    [SerializeField] private float lifeTime = 5f;
//incompleto, es otra HU
    [Header("Daño")]
    [Tooltip("Daño que hará esta bala al jugador.")]
    [SerializeField] private int damage = 1;

    [Tooltip("El objeto con el que el proyectil colisiona pero no se destruye.")]
    public GameObject inmuneObject;

    [Tooltip("Tag del jugador, para reconocer cuándo lo golpea.")]
    [SerializeField] private string playerTag = "Player";

    private Vector3 direction = Vector3.left;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    // Permite que BossShoot fije la velocidad de cada patrón por separado.
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // Valor de daño accesible para cuando el sistema de vida lo necesite.
    public int Damage => damage;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Vector3 movement = direction * speed * Time.deltaTime;
        transform.position += movement;
    }

    // Trigger en vez de colisión física: detecta el contacto pero NO empuja al jugador.
    private void OnTriggerEnter(Collider other)
    {
        // El objeto inmune (el propio jefe, normalmente) no destruye la bala.
        if (inmuneObject != null && other.gameObject == inmuneObject)
        {
            return;
        }

        // Si tocó al jugador: daño al jugador.
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"💥 Bala golpeó al Player (daño preparado: {damage}).");

            //Se ecnargara del daño cuando el sistema exista
        }

        // Desaparece al tocar cualquier cosa, sin empujar (trigger).
        Destroy(gameObject);
    }
}