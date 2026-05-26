using UnityEngine;

public class DisableAllTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement3D player3D = other.GetComponent<PlayerMovement3D>();
        PlayerMovement2D player2D = other.GetComponent<PlayerMovement2D>();

        if (player3D != null)
            player3D.enabled = false;

        if (player2D != null)
            player2D.enabled = false;

        Debug.Log("🚫 Movimiento completamente desactivado.");
    }
}
