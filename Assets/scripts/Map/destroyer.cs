using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] private string projectileTag = "EnemyProjectile";

//Todo proyectil enemigo es eliminado
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(projectileTag))
        {
            Destroy(other.gameObject);
        }
    }
}