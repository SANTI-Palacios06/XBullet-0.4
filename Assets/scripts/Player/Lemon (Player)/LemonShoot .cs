using UnityEngine;
using UnityEngine.InputSystem;

/// Maneja el disparo del jugador con un solo botón (Input System):
public class LemonShoot : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Prefab del disparo normal.")]
    [SerializeField] private LemonProjectile projectilePrefab;
    [Tooltip("Prefab del ataque cargado (diferente al normal).")]
    [SerializeField] private LemonProjectile chargedProjectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject globalImmuneObject;

    [Header("Configuración del disparo normal")]
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float projectileSpeed = 20f;

    [Header("Configuración del ataque cargado")]
    [Tooltip("Segundos que hay que mantener el botón para que sea ataque cargado.")]
    [SerializeField] private float chargeTime = 1.5f;
    [SerializeField] private int chargedDamage = 3;
    [SerializeField] private float chargedSpeed = 30f;

    [Tooltip("Dirección de disparo en el espacio local del shootPoint")]
    [SerializeField] private Vector3 fireDirection = Vector3.left;

    [Header("Input")]
    [SerializeField] private InputActionReference shoot;

//
    private float nextAllowedFireTime;
    private float holdStartTime = -1f;
    private bool isCharging;

//Configura el punto de disparo del disparo
    public void Configure(LemonProjectile newProjectilePrefab, Transform newShootPoint)
    {
        projectilePrefab = newProjectilePrefab;
        shootPoint = newShootPoint;
    }

    private void OnEnable()
    {
        if (shoot != null && shoot.action != null)
        {
            shoot.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (shoot != null && shoot.action != null)
        {
            shoot.action.Disable();
        }
    }

    private void Update()
    {
        if (shoot == null || shoot.action == null)
            return;

        // Empezó a presionar: arranca a contar la carga.
        if (shoot.action.WasPressedThisFrame())
        {
            holdStartTime = Time.time;
            isCharging = true;
        }

        // Soltó el botón: decide si fue disparo normal o cargado.
        if (shoot.action.WasReleasedThisFrame() && isCharging)
        {
            float heldDuration = Time.time - holdStartTime;
            isCharging = false;

            if (!CanFire())
                return;

            if (heldDuration >= chargeTime)
            {
                // Ataque cargado: prefab distinto, más daño y velocidad.
                Fire(chargedProjectilePrefab, chargedDamage, chargedSpeed);
            }
            else
            {
                // Disparo normal.
                Fire(projectilePrefab, projectileDamage, projectileSpeed);
            }
        }
    }

    private bool CanFire()
    {
        return Time.time >= nextAllowedFireTime;
    }

    /// Instancia y lanza el proyectil indicado con el daño y velocidad dados.
    private void Fire(LemonProjectile prefab, int damage, float speed)
    {
        if (prefab == null)
        {
            Debug.LogWarning("LemonShoot: falta asignar el prefab del proyectil en el Inspector.");
            return;
        }

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        Quaternion spawnRot = shootPoint != null ? shootPoint.rotation : transform.rotation;

        nextAllowedFireTime = Time.time + fireCooldown;

        LemonProjectile projectile = Instantiate(prefab, spawnPos, spawnRot);
        projectile.SetOwner(transform);
        projectile.SetImmuneObject(globalImmuneObject);

        Vector3 direction = shootPoint != null
            ? shootPoint.TransformDirection(fireDirection)
            : fireDirection;

        projectile.Launch(direction, speed, damage);
    }
}