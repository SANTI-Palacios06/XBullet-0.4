using UnityEngine;

public class ObjectMovementPhase1 : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject target;

    void Update()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
}
