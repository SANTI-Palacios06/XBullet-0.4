using UnityEngine;

public class SelectiveProximity : MonoBehaviour
{
    [SerializeField] private GameObject targetBlocker;
    [SerializeField] private float detectionRadius = 0.5f;

    private void Update()
    {
        if (Vector3.Distance(transform.position, targetBlocker.transform.position) < detectionRadius)
        {
            Debug.Log("Colisión simulada con el target.");
        }
    }
}
