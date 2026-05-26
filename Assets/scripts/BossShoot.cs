using UnityEngine;

public class BossShoot : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    private float classicAngle = 0f;
    private float helicalAngle = 0f;
    private float segmentedAngle = 0f;

    public void ShootClassicSpiral()
    {
        int bullets = 30;
        float angleStep = 12f;

        for (int i = 0; i < bullets; i++)
        {
            float angle = classicAngle + i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)).normalized;
            GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            if (projectileInstance == null) continue;

            EvilLemonProjectile lemonProj = projectileInstance.GetComponent<EvilLemonProjectile>();
            if (lemonProj != null)
                lemonProj.SetDirection(direction);
        }

        classicAngle += 5f;
    }

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

            GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            if (projectileInstance == null) continue;

            EvilLemonProjectile lemonProj = projectileInstance.GetComponent<EvilLemonProjectile>();
            if (lemonProj != null)
                lemonProj.SetDirection(direction);
        }
    }

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

                GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
                if (projectileInstance == null) continue;

                EvilLemonProjectile lemonProj = projectileInstance.GetComponent<EvilLemonProjectile>();
                if (lemonProj != null)
                    lemonProj.SetDirection(direction);
            }
        }

        segmentedAngle += 8f;
    }

    public void ShootHelicalFan()
    {
        int bullets = 72;
        float angleStep = 30f;

        for (int i = 0; i < bullets; i++)
        {
            float angle = helicalAngle + i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)).normalized;
            GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            if (projectileInstance == null) continue;

            EvilLemonProjectile lemonProj = projectileInstance.GetComponent<EvilLemonProjectile>();
            if (lemonProj != null)
                lemonProj.SetDirection(direction);
        }

        helicalAngle += 15f;
    }
}
