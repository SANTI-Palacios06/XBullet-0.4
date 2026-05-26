using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField] private float firstSpeed = 5f;
    [SerializeField] private float secondSpeed = 2f;
    [SerializeField] private GameObject firstTarget;
    [SerializeField] private GameObject secondTarget;
    [SerializeField] private float stoppingDistance = 0.1f;

    private bool reachedFirstTarget = false;

    void Update()
    {
        if (!reachedFirstTarget)
        {
            if (firstTarget == null) return;

            MoveTowards(firstTarget.transform.position, firstSpeed);

            if (Vector3.Distance(transform.position, firstTarget.transform.position) < stoppingDistance)
            {
                reachedFirstTarget = true;
            }
        }
        else
        {
            if (secondTarget == null) return;

            MoveTowards(secondTarget.transform.position, secondSpeed);

            if (Vector3.Distance(transform.position, secondTarget.transform.position) < stoppingDistance)
            {
                // Al llegar al segundo target, desactivamos el script
                this.enabled = false;
            }
        }
    }

    void MoveTowards(Vector3 targetPosition, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}
