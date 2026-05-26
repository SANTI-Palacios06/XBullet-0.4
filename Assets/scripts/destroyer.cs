using UnityEngine;

public class destroyer : MonoBehaviour
{
    [Header("Nombres de objetos que serán destruidos si colisionan")]
    [SerializeField] private string[] targetNames = { "Lemon", "Lemon (evil)", "Lemon (boss)" };

    [Header("Objetos inmunes (no se destruyen)")]
    [SerializeField] private GameObject immuneObject1;
    [SerializeField] private GameObject immuneObject2;

    private void OnCollisionEnter(Collision collision)
    {
        TryDestroy(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDestroy(other.gameObject);
    }

    private void TryDestroy(GameObject target)
    {
        if (target == null || IsImmune(target) || !IsTargetToDestroy(target)) return;

        Destroy(target);
    }

    private bool IsTargetToDestroy(GameObject obj)
    {
        string cleanName = obj.name.Replace("(Clone)", "").Trim();
        foreach (var name in targetNames)
        {
            if (cleanName == name)
                return true;
        }
        return false;
    }

    private bool IsImmune(GameObject obj)
    {
        return (immuneObject1 != null && obj == immuneObject1) ||
               (immuneObject2 != null && obj == immuneObject2);
    }
}
