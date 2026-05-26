using UnityEngine;

public class CameraTriggerZone : MonoBehaviour
{
    [SerializeField] private CameraToggle cameraToggle;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        cameraToggle.ActivateBossCamera();
        Destroy(gameObject); // Se activa una vez
    }
}
