using UnityEngine;

//Se encarga de la destruccion  del proyectil enemigo 
public class DestroyAllLemonEvilsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        DestroyAllLemonEvils();
        Destroy(gameObject);
    }

    public static void DestroyAllLemonEvils()
    {
        GameObject[] enemyProjectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (var obj in enemyProjectiles)
        {
            Destroy(obj);
        }
    }
}