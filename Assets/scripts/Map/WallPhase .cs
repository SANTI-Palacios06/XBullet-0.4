using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WallPhase : MonoBehaviour
{
    //Ignora la pelota y la permite pasar
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("PinballBall")) return;

        Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);
    }
}