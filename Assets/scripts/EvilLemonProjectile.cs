using UnityEngine;

public class EvilLemonProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    private Vector3 direction = Vector3.left;

    [Tooltip("El objeto con el que el proyectil colisiona pero no se destruye")]
    public GameObject inmuneObject;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        Vector3 movement = direction * speed * Time.deltaTime;
        transform.position += movement;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (inmuneObject != null && collision.gameObject == inmuneObject)
        {
            Debug.Log($"Colisión con objeto inmune: {collision.gameObject.name} (NO se destruye).");
            return;
        }

        Debug.Log($"Colisión con: {collision.gameObject.name} (proyectil DESTRUIDO).");
        Destroy(gameObject);
    }
}
