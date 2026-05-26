using UnityEngine;

public class InvincibleObject : MonoBehaviour
{
    // Este objeto ignorará cualquier colisión o trigger
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{gameObject.name} fue golpeado por {collision.gameObject.name}, pero es invencible.");
        // No hacemos nada, simplemente ignoramos la colisión
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{gameObject.name} fue tocado por {other.gameObject.name}, pero es invencible.");
        // No hacemos nada, simplemente ignoramos el trigger
    }
}
