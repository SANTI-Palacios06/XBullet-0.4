using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveOnEnable : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody rb;
    private Vector3 targetPosition = new Vector3(-339.3f, 45.7028f, -33.2f);
    private bool moving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moving = true;
    }

    private void FixedUpdate()
    {
        if (!moving) return;

        Vector3 direction = (targetPosition - rb.position).normalized;
        Vector3 nextPosition = rb.position + direction * speed * Time.fixedDeltaTime;

        if (Vector3.Distance(rb.position, targetPosition) <= 0.1f)
        {
            rb.position = targetPosition;
            rb.linearVelocity = Vector3.zero;
            moving = false;
            return;
        }

        rb.linearVelocity = direction * speed;
    }
}
