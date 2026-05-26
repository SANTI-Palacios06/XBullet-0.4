using UnityEngine;
using TMPro;

public class BulletAndEnemyCounter : MonoBehaviour
{
    [Header("Balas a contar (asignar prefabs o referencias manuales)")]
    public GameObject[] bulletPrefabs;

    private TextMeshProUGUI bulletCounterTMP;
    private TextMeshProUGUI enemyCounterTMP;

    private bool modoBoss = false;

    void Start()
    {
        bulletCounterTMP = GameObject.Find("BulletCounterText")?.GetComponent<TextMeshProUGUI>();
        enemyCounterTMP = GameObject.Find("EnemyCounterText")?.GetComponent<TextMeshProUGUI>();

        if (bulletCounterTMP == null)
            Debug.LogWarning("⚠ No se encontró BulletCounterText");
        else
            ColocarEnEsquinaSuperiorIzquierda(bulletCounterTMP.rectTransform, new Vector2(10f, -10f));

        if (enemyCounterTMP == null)
            Debug.LogWarning("⚠ No se encontró EnemyCounterText");
        else
            ColocarEnEsquinaInferiorIzquierda(enemyCounterTMP.rectTransform, new Vector2(10f, 100f));
    }

    void Update()
    {
        if (!modoBoss)
        {
            int bulletCount = 0;
            foreach (GameObject bulletPrefab in bulletPrefabs)
            {
                bulletCount += CountActiveInstances(bulletPrefab.name);
            }

            int enemyCount = CountEnemiesByTag();

            if (bulletCounterTMP != null)
                bulletCounterTMP.text = $"Balas activas: {bulletCount}";
            if (enemyCounterTMP != null)
                enemyCounterTMP.text = $"Enemigos: {enemyCount}";
        }
        else
        {
            int bossCount = CountSpecific("Lemon (boss)");

            if (bulletCounterTMP != null)
                bulletCounterTMP.text = $"Boss activos: {bossCount}";
            if (enemyCounterTMP != null)
                enemyCounterTMP.text = $"";
        }
    }

    int CountActiveInstances(string prefabName)
    {
        int count = 0;
        Transform[] allObjects = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (Transform t in allObjects)
        {
            if (t == null || t.gameObject == null) continue;
            if (!t.gameObject.activeInHierarchy) continue;

            string objName;
            try { objName = t.name; }
            catch { continue; }

            if (objName.Contains(prefabName))
                count++;
        }
        return count;
    }

    int CountEnemiesByTag()
    {
        int count = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeInHierarchy) continue;
            count++;
        }
        return count;
    }

    int CountSpecific(string prefabName)
    {
        int count = 0;
        Transform[] allObjects = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (Transform t in allObjects)
        {
            if (t == null || t.gameObject == null) continue;
            if (!t.gameObject.activeInHierarchy) continue;

            string objName;
            try { objName = t.name; }
            catch { continue; }

            if (objName.Contains(prefabName))
                count++;
        }
        return count;
    }

    void ColocarEnEsquinaSuperiorIzquierda(RectTransform rectTransform, Vector2 offset)
    {
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = offset;
    }

    void ColocarEnEsquinaInferiorIzquierda(RectTransform rectTransform, Vector2 offset)
    {
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.anchoredPosition = offset;
    }

    public void ActivarModoBoss()
    {
        modoBoss = true;
    }
}
