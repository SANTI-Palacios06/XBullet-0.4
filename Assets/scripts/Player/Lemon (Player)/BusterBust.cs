using UnityEngine;

/// <summary>
/// Destruye únicamente al boss indicado cuando el proyectil colisiona con él.
/// </summary>
public class BusterBust : MonoBehaviour
{
    [Header("Nombre base del objeto a destruir")]
    [SerializeField] private string bossName = "Lemon (Boss)";

    private void OnCollisionEnter(Collision collision)
    {
        CheckTarget(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckTarget(other.gameObject);
    }

    private void CheckTarget(GameObject target)
    {
        if (target == null)
            return;

        if (!IsBossTarget(target))
            return;

        Destroy(target);
    }

    private bool IsBossTarget(GameObject target)
    {
        string cleanName = target.name.Replace("(Clone)", "").Trim();
        return cleanName == bossName;
    }
}