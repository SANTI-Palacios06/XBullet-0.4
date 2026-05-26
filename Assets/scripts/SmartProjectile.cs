using UnityEngine;

public class SmartProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private GameObject[] ignoredObjects; // Objetos a ignorar manualmente

    private Vector3 direction = Vector3.forward; // Dirección por defecto

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Primero, ignoramos si es uno de los objetos de la lista
        foreach (GameObject ignored in ignoredObjects)
        {
            if (collision.gameObject == ignored)
            {
                Debug.Log($"Impacto ignorado con: {collision.gameObject.name} (objeto específico)");
                return;
            }
        }

        // Luego, ignoramos si es "Lemon (boss)(Clone)"
        if (collision.gameObject.name == "Lemon (boss)(Clone)")
        {
            Debug.Log("Impacto ignorado con Lemon (boss)(Clone) desde código.");
            return;
        }

        // Si no es ninguno de los anteriores, destruimos el proyectil
        Debug.Log($"Impacto con: {collision.gameObject.name}, destruyendo proyectil.");
        Destroy(gameObject);
    }
}
