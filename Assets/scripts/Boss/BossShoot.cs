using UnityEngine;

/// Se encarga de asignar quien va a disparar los proyectiles enemigos 
public class BossShoot : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Prefab del proyectil del jefe.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Punto desde donde nacen los disparos.")]
    [SerializeField] private Transform shootPoint;

    [Header("Configuración del ataque")]
    [Tooltip("Tiempo entre oleadas de disparo.")]
    [SerializeField] private float fireInterval = 0.3f;

    [Header("Velocidad de cada patrón")]
    [SerializeField] private float classicSpiralSpeed = 20f;
    [SerializeField] private float rosetteSpeed = 20f;
    [SerializeField] private float segmentedFlowerSpeed = 20f;
    [SerializeField] private float helicalFanSpeed = 20f;

    // Ángulos acumulados: dan el efecto de giro en cada oleada.
    private float classicAngle = 0f;
    private float helicalAngle = 0f;
    private float segmentedAngle = 0f;

    private float nextFireTime;
    private int lastPatternIndex = -1;

    public void Configure(GameObject newProjectilePrefab, Transform newShootPoint)
    {
        projectilePrefab = newProjectilePrefab;
        shootPoint = newShootPoint;
    }

  
    /// Se encarga del patron random de las oleadas 
    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            ShootRandomPattern();
        }
    }


    /// Se encarga de los 4 patrones de disparos del enemigo
    private void ShootRandomPattern()
    {
        int index;
        do
        {
            index = Random.Range(0, 4);
        }
        while (index == lastPatternIndex);

        lastPatternIndex = index;

        switch (index)
        {
            case 0:
                ShootClassicSpiral();
                break;
            case 1:
                ShootRosettePattern();
                break;
            case 2:
                ShootSegmentedFlower();
                break;
            case 3:
                ShootHelicalFan();
                break;
        }
    }

  
    /// Espiral clásica: lanza una rueda de balas y gira un poco en cada oleada.

    public void ShootClassicSpiral()
    {
        int bullets = 30;
        float angleStep = 12f;

        for (int i = 0; i < bullets; i++)
        {
            float angle = classicAngle + i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)).normalized;
            SpawnProjectile(shootPoint.position, direction, classicSpiralSpeed);
        }

        classicAngle += 5f;
    }


    /// Roseta: balas distribuidas en un patrón de pétalos usando seno.

    public void ShootRosettePattern()
    {
        int bulletsPerWave = 72;
        float angleStep = 360f / bulletsPerWave;
        int petals = 6;
        float scale = 3f;

        for (int i = 0; i < bulletsPerWave; i++)
        {
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float radius = Mathf.Abs(Mathf.Sin(petals * radians)) * scale + 1f;
            Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)).normalized;
            Vector3 spawnPosition = shootPoint.position + direction * radius;
            SpawnProjectile(spawnPosition, direction, rosetteSpeed);
        }
    }


    /// Flor segmentada: varios pétalos en arco, con huecos para dar forma.

    public void ShootSegmentedFlower()
    {
        int petals = 12;
        int bulletsPerPetal = 12;
        float arcAngle = 30f;
        float angleBetweenPetals = 360f / petals;

        for (int p = 0; p < petals; p++)
        {
            float baseAngle = segmentedAngle + p * angleBetweenPetals;
            for (int i = 0; i < bulletsPerPetal; i++)
            {
                if ((i % 4) >= 2) continue;
                float t = (float)i / (bulletsPerPetal - 1);
                float offset = Mathf.Lerp(-arcAngle / 2f, arcAngle / 2f, t);
                float finalAngle = baseAngle + offset;
                float rad = finalAngle * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)).normalized;
                SpawnProjectile(shootPoint.position, direction, segmentedFlowerSpeed);
            }
        }

        segmentedAngle += 8f;
    }

    /// Abanico helicoidal: rueda amplia que gira más rápido cada oleada.

    public void ShootHelicalFan()
    {
        int bullets = 72;
        float angleStep = 30f;

        for (int i = 0; i < bullets; i++)
        {
            float angle = helicalAngle + i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)).normalized;
            SpawnProjectile(shootPoint.position, direction, helicalFanSpeed);
        }

        helicalAngle += 15f;
    }

   
    /// Crea un proyectil, le asigna dirección y velocidad.
    private void SpawnProjectile(Vector3 spawnPosition, Vector3 direction, float speed)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        if (projectileInstance == null) return;

        EvilLemonProjectile lemonProj = projectileInstance.GetComponent<EvilLemonProjectile>();
        if (lemonProj != null)
        {
            lemonProj.SetDirection(direction);
            lemonProj.SetSpeed(speed);
        }
    }
}