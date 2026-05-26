using UnityEngine;

public class LemonDestroyer : MonoBehaviour
{
    [Header("Nombre base del objeto a destruir")]
    [SerializeField] private string bossName = "Lemon (Boss)";

    private void OnCollisionEnter(Collision collision)
    {
        TryDestroy(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDestroy(other.gameObject);
    }

    private void TryDestroy(GameObject target)
    {
        if (target == null) return;

        string cleanName = target.name.Replace("(Clone)", "").Trim();

        if (cleanName == bossName)
        {
            Destroy(target);
            Destroy(gameObject);
        }
    }
}
