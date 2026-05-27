using UnityEngine;

public class MegaBuster : MonoBehaviour
{
    [Header("Prefab que se disparará (MegaShoot)")]
    [SerializeField] private LemonProjectile projectilePrefab;

    [Header("Punto desde donde se dispara")]
    [SerializeField] private Transform shootPoint;

    [Header("Tiempo mínimo de carga (segundos)")]
    [SerializeField] private float chargeTime = 5f;

    [Header("Configuración del disparo cargado")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private int projectileDamage = 3;

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

//HOTFIX
        Quaternion correctedRotation = Quaternion.Euler(0f, 90.845f, 0f);

        LemonProjectile newProj = Instantiate(projectilePrefab, spawnPos, correctedRotation);
        newProj.SetOwner(transform);
        newProj.SetImmuneObject(globalImmuneObject);
        newProj.Launch(Vector3.left, projectileSpeed, projectileDamage);
    }
}