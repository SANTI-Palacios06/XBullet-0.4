using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private bool canMove = true;

    private void Update()
    {
        if (!canMove) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveX, moveY);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    public void EnableMovement() => canMove = true;

    public void DisableMovement() => canMove = false;
}
