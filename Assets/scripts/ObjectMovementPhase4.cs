using UnityEngine;

public class ObjectMovementPhase4 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ObjectMovementPhase2 phase2Script = other.GetComponent<ObjectMovementPhase2>();

        if (phase2Script != null)
            phase2Script.enabled = false;
    }
}
