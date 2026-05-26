using UnityEngine;

public class ContactDetector : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Debug.Log($"Contacto detectado con: {collision.gameObject.name}");
        }
    }
}
