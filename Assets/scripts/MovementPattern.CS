using UnityEngine;

public class MovementPattern : MonoBehaviour
{
    [Header("Puntos de movimiento")]
    [SerializeField] private Transform point1;  // Lugar base
    [SerializeField] private Transform point2;
    [SerializeField] private Transform point3;

    [Header("Velocidad de movimiento")]
    [SerializeField] private float moveSpeed = 3f;

    private Transform[] points;
    private int currentTargetIndex = 0;
    private bool isActive = false;

    private void Start()
    {
        // Definir el patrón exacto: 1-2-3-1...
        points = new Transform[] { point1, point2, point3 };
        currentTargetIndex = 0;
        transform.position = point1.position; // Inicia en punto base
    }

    private void Update()
    {
        if (!isActive) return;

        Transform targetPoint = points[currentTargetIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % points.Length;
        }
    }

    public void ActivateMovement()
    {
        isActive = true;
    }
}
