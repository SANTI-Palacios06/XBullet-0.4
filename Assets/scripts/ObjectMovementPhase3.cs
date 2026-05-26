using UnityEngine;

public class ObjectMovementPhase3 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ObjectMovementPhase1 phase1Script = other.GetComponent<ObjectMovementPhase1>();
        ObjectMovementPhase2 phase2Script = other.GetComponent<ObjectMovementPhase2>();

        if (phase1Script != null)
            phase1Script.enabled = false;

        if (phase2Script != null)
            phase2Script.enabled = true;
    }
}
