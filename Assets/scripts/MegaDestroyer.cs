using UnityEngine;

public class MegaDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        string name = other.gameObject.name;

        // No destruye si es Lemon (el proyectil básico del jugador)
        if (name.Contains("Lemon") && !name.Contains("evil") && !name.Contains("boss"))
        {
            return;
        }

        // Si es cualquier otro proyectil, lo destruye
        if (name.Contains("Lemon") || name.Contains("MegaShoot"))
        {
            Destroy(other.gameObject);
        }
    }
}
