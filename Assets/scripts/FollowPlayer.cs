using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;

    void Update()
    {
        if (player == null) return;

        // Mueve la cámara para seguir al jugador con un desplazamiento fijo
        transform.position = player.transform.position + new Vector3(5, 3, 0);
    }
}
