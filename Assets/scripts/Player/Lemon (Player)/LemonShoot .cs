using UnityEngine;
using UnityEngine.InputSystem;

/// Es el encargado de las referencias del prefab, lugar de spawn y sus propiedades del proyectil.
public class LemonShoot : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private LemonProjectile projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject globalImmuneObject;

    [Header("Configuración del disparo")]
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float projectileSpeed = 20f;

    [Tooltip("Dirección de disparo en el espacio local del shootPoint")]
    [SerializeField] private Vector3 fireDirection = Vector3.left;

    [Header("Input")]
    [SerializeField] private InputActionReference shoot;

    private float nextAllowedFireTime;

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

        if (shoot.action.WasPressedThisFrame() && CanFire())
        {
            Fire();
        }
    }

    private bool CanFire()
    {
        return Time.time >= nextAllowedFireTime;
    }

    private void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("LemonShoot necesita projectilePrefab asignado en el Inspector.");
            return;
        }

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        Quaternion spawnRot = shootPoint != null ? shootPoint.rotation : transform.rotation;

        nextAllowedFireTime = Time.time + fireCooldown;

        LemonProjectile projectile = Instantiate(projectilePrefab, spawnPos, spawnRot);

        projectile.SetOwner(transform);
        projectile.SetImmuneObject(globalImmuneObject);

        Vector3 direction = shootPoint != null
            ? shootPoint.TransformDirection(fireDirection)
            : fireDirection;

        projectile.Launch(direction, projectileSpeed, projectileDamage);
    }
}