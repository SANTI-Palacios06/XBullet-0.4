using UnityEngine;

public class LemonProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float zDrift = 0.5f; // Movimiento leve en Z
    public GameObject immuneObject;

    [Tooltip("Dirección principal del movimiento del proyectil (eje X, Y o Z)")]
    public Vector3 direction = Vector3.left; // Por defecto hacia la izquierda

    void Update()
    {
        // Movimiento compuesto: dirección principal + zDrift en Z
        Vector3 movement = (direction.normalized * speed + Vector3.forward * zDrift) * Time.deltaTime;
        transform.position += movement;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (immuneObject != null && collision.gameObject == immuneObject)
        {
            return;
        }

        Destroy(gameObject);
    }

    // Método para actualizar la dirección desde el script que instancia el proyectil
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }
}
