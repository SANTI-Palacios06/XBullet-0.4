using UnityEngine;

public class Desactivar3DTrigger : MonoBehaviour
{
    [SerializeField] private PlayerMovement3D player3D;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player3D.DisableMovement();
        Debug.Log("⛔ Movimiento 3D desactivado completamente.");
    }
}
