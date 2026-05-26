using UnityEngine;

public class SpiralShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private int bulletsPerWave = 30;
    [SerializeField] private float timeBetweenWaves = 0.2f;
    [SerializeField] private float angleStep = 12f;
    [SerializeField] private float spiralSpeed = 5f;

    private float timer;
    private float currentAngle = 0f;

    private void Update()
    {
        if (projectilePrefab == null || shootPoint == null)
            return;

        timer += Time.deltaTime;
        if (timer >= timeBetweenWaves)
        {
            FireSpiral();
            timer = 0f;
        }
    }

    public void FireSpiral()
    {
        for (int i = 0; i < bulletsPerWave; i++)
        {
            float angleY = currentAngle + i * angleStep;
            float angleX = Mathf.Sin((currentAngle + i * 10f) * Mathf.Deg2Rad) * 45f;

            Quaternion localRotation = Quaternion.Euler(angleX, angleY, 0f);
            GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            Vector3 dir = localRotation * Vector3.forward;

            EvilLemonProjectile evilProj = proj.GetComponent<EvilLemonProjectile>();
            if (evilProj != null)
            {
                evilProj.SetDirection(dir);
            }
            else
            {
                Debug.LogWarning("El prefab instanciado NO tiene el script EvilLemonProjectile.");
            }
        }

        currentAngle += spiralSpeed;
    }
}
