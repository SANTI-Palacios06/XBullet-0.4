using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference move;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isActive = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (move != null)
            move.action.Enable();
    }

    private void OnDisable()
    {
        if (move != null)
            move.action.Disable();
    }

//Actualiza el movimiento
    private void Update()
    {
        if (!isActive)
        {
            moveInput = Vector3.zero;
            return;
        }

        Vector2 input = move.action.ReadValue<Vector2>();

        moveInput = new Vector3(-input.x, 0f, -input.y);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void ActivateMovement()
    {
        isActive = true;
    }
//Desactiva todo movimiento 
    public void DeactivateMovement()
    {
        isActive = false;
        moveInput = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }

//Se reinicia el movimiento
    public void ResetMovement()
    {
        isActive = true;
        moveInput = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        Debug.Log("Movimiento reiniciado correctamente.");
    }
}