using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private bool isActive = false;

    private void Update()
    {
        if (!isActive) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    public void ActivateMovement() => isActive = true;

    public void DeactivateMovement() => isActive = false;

    // 👉 Este es el método nuevo
    public void ResetMovement()
    {
        isActive = true;
        Debug.Log("🔄 Movimiento 2D reiniciado correctamente.");
    }
}
