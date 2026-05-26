using UnityEngine;

public class ActivadorBossCounter : MonoBehaviour
{
    private BulletAndEnemyCounter contador;

    void Start()
    {
        contador = Object.FindFirstObjectByType<BulletAndEnemyCounter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (contador != null)
        {
            contador.ActivarModoBoss();
            Debug.Log("✅ Cambiando a modo Boss");
        }

        Destroy(gameObject); // El trigger solo se usa una vez
    }
}
