using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public float speed = 5f; // Ajusta este valor en el Inspector para cambiar la velocidad a la que se mueve

    void Update()
    {
        // Mueve el objeto hacia abajo a lo largo del eje Y
        transform.position += Vector3.down * speed * Time.deltaTime;
    }
}