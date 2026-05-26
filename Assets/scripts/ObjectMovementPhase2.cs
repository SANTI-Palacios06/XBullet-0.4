using UnityEngine;

public class ObjectMovementPhase2 : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private GameObject target;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
}
