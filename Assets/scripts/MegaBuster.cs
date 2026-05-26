using UnityEngine;

public class MegaBuster : MonoBehaviour
{
    [Header("Prefab que se disparará (MegaShoot)")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Punto desde donde se dispara")]
    [SerializeField] private Transform shootPoint;

    [Header("Tiempo mínimo de carga (segundos)")]
    [SerializeField] private float chargeTime = 5f;

    [Header("Objeto inmune al proyectil")]
    [SerializeField] private GameObject globalImmuneObject;

    private float holdStartTime = -1f;
    private bool isCharging = false;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            holdStartTime = Time.time;
            isCharging = true;
        }

        if (Input.GetButtonUp("Fire1") && isCharging)
        {
            float heldDuration = Time.time - holdStartTime;

            if (heldDuration >= chargeTime)
            {
                Shoot();
            }

            isCharging = false;
            holdStartTime = -1f;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;

        // Rotación corregida: X=0, Y=90.845, Z=0
        Quaternion correctedRotation = Quaternion.Euler(0f, 90.845f, 0f);
        GameObject newProj = Instantiate(projectilePrefab, spawnPos, correctedRotation);

        var projScript = newProj.GetComponent<LemonProjectile>();
        if (projScript != null)
        {
            projScript.immuneObject = globalImmuneObject;
            projScript.SetDirection(Vector3.left);
        }
    }
}
