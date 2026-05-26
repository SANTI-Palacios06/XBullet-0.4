using UnityEngine;

public class DestroyAllLemonEvilsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var obj in allObjects)
        {
            if (obj.name == "Lemon (evil)" || obj.name == "Lemon (evil) (Clone)")
            {
                Destroy(obj);
            }
        }

        Destroy(gameObject);
    }
}
