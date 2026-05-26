using UnityEngine;

public class Activar2DTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement2D player2D = other.GetComponent<PlayerMovement2D>();
        PlayerMovement3D player3D = other.GetComponent<PlayerMovement3D>();

        if (player3D != null)
            player3D.enabled = false;

        if (player2D != null)
        {
            player2D.enabled = true;
            player2D.ResetMovement(); // 👉 Aquí llamamos el "reinicio" al activar 2D
            Debug.Log("✅ Activado modo 2D.");
        }
    }
}
