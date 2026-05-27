using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


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
    [SerializeField] private string fireButtonName = "Fire1";
    [SerializeField] private KeyCode legacyFireKey = KeyCode.F;

    private float nextAllowedFireTime;

    public void Configure(LemonProjectile newProjectilePrefab, Transform newShootPoint)
    {
        projectilePrefab = newProjectilePrefab;
        shootPoint = newShootPoint;
    }

    private void Update()
    {
        if (WantsToFire() && CanFire())
        {
            Fire();
        }
    }

//asignacion de Inputs del disparo
    private bool WantsToFire()
    {
        bool wantsToFire = false;
#if ENABLE_INPUT_SYSTEM
        wantsToFire |= Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame;
        wantsToFire |= Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        wantsToFire |= Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        wantsToFire |= Input.GetButtonDown(fireButtonName) || Input.GetKeyDown(legacyFireKey);
#endif
        return wantsToFire;
    }

    private bool CanFire()
    {
        return Time.time >= nextAllowedFireTime;
    }

//Se encarga del manejo del disparo. verificando prefab, velocidad posisiconamiento, objeto inmune y cooldown
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