using UnityEngine;

public class LemonShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 0.5f;
    public Transform shootPoint;
    public GameObject globalImmuneObject;

    private float nextFireTime = 0f;

    void Update()
    {
        // Solo dispara una vez por pulsación de la tecla F
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("¡Advertencia! Projectile Prefab no asignado en LemonShoot. No se puede disparar.");
            return;
        }

        if (shootPoint == null)
        {
            Debug.LogWarning("¡Advertencia! Shoot Point no asignado en LemonShoot. El proyectil se instanciará en la posición del objeto que tiene este script.");
            Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            return;
        }

        GameObject newProj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        LemonProjectile projScript = newProj.GetComponent<LemonProjectile>();
        if (projScript != null)
        {
            projScript.immuneObject = globalImmuneObject;
        }
    }
}
