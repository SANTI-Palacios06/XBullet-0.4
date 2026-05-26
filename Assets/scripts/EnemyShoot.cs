using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("Disparo normal")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private bool isActive = true;

    [Header("Patrón de disparo circular")]
    [SerializeField] private int patternBullets = 12;
    [SerializeField] private float patternAngleOffset = 0f;

    private SpiralShooter spiralShooter;
    private float nextShootTime = 0f;

    private void Awake()
    {
        spiralShooter = GetComponent<SpiralShooter>();
        if (spiralShooter == null)
        {
            Debug.LogWarning($"[{gameObject.name}] No se encontró SpiralShooter en este objeto.");
        }
    }

    private void Start()
    {
        nextShootTime = Mathf.Ceil(Time.time / shootInterval) * shootInterval;
    }

    private void Update()
    {
        if (!isActive) return;

        if (Time.time >= nextShootTime)
        {
            Shoot();
            ShootPattern();
            ShootSpiral();
            nextShootTime += shootInterval;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        EvilLemonProjectile evilProj = projectileInstance.GetComponent<EvilLemonProjectile>();
        if (evilProj != null)
        {
            evilProj.SetDirection(Vector3.left);
        }
    }

    private void ShootPattern()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        float angleStep = 360f / patternBullets;

        for (int i = 0; i < patternBullets; i++)
        {
            float angle = i * angleStep + patternAngleOffset;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));

            GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            EvilLemonProjectile evilProj = projectileInstance.GetComponent<EvilLemonProjectile>();
            if (evilProj != null)
            {
                evilProj.SetDirection(dir);
            }
            else
            {
                Debug.LogWarning("El prefab instanciado NO tiene el script EvilLemonProjectile.");
            }
        }
    }

    private void ShootSpiral()
    {
        if (spiralShooter != null)
        {
            spiralShooter.FireSpiral();
        }
    }

    public void DisableShooting()
    {
        isActive = false;
    }
}
