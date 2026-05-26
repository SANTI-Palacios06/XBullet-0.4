using UnityEngine;

public class HealthReceiver : MonoBehaviour
{
    private DamageManager damageManager;

    void Start()
    {
        damageManager = Object.FindFirstObjectByType<DamageManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damageManager == null) return;

        GameObject other = collision.gameObject;

        if (IsProjectile(other))
        {
            damageManager.ApplyDamage(this.gameObject, other);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageManager == null) return;

        if (IsProjectile(other.gameObject))
        {
            damageManager.ApplyDamage(this.gameObject, other.gameObject);
        }
    }

    private bool IsProjectile(GameObject obj)
    {
        string name = obj.name;
        return name.Contains("Lemon") || name.Contains("MegaShoot");
    }
}
