using UnityEngine;
using System.Collections.Generic;

public class DamageManager : MonoBehaviour 
{
    [Header("Salud inicial de cada tipo de objeto")]
    public float XHealth = 100f;
    public float BigbitHealth = 50f;
    public float StormEagleHealth = 300f;

    [Header("Daño de cada tipo de proyectil")]
    public float damageLemon = 5f;
    public float damageLemonEvil = 5f;
    public float damageLemonBoss = 8f;
    public float damageMegaShoot = 20f;

    [Header("Referencias a objetos con salud en la escena")]
    public GameObject playerObject;
    public GameObject stormEagleObject;
    public GameObject enemiesParent;

    private Dictionary<GameObject, float> healthDict = new Dictionary<GameObject, float>();

    void Start() 
    {
        if (playerObject == null)
            playerObject = GameObject.Find("X");
        if (stormEagleObject == null)
            stormEagleObject = GameObject.Find("Storm_Eagle");
        if (enemiesParent == null)
            enemiesParent = GameObject.Find("enemyshooting");

        if (playerObject != null)
            healthDict[playerObject] = XHealth;
        if (stormEagleObject != null)
            healthDict[stormEagleObject] = StormEagleHealth;
        if (enemiesParent != null)
        {
            foreach (Transform child in enemiesParent.transform)
            {
                if (child.name.Contains("Bigbit"))
                    healthDict[child.gameObject] = BigbitHealth;
            }
        }
    }

    public void ApplyDamage(GameObject target, GameObject projectile)
    {
        float damageAmount = GetProjectileDamage(projectile);
        if (damageAmount <= 0f) return;

        string projName = projectile.name;

        // Inmunidades
        if (target == playerObject)
        {
            if (projName.Contains("Lemon") && !projName.Contains("evil") && !projName.Contains("boss"))
                return;
            if (projName.Contains("MegaShoot"))
                return;
        }

        if (target.name.Contains("Bigbit"))
        {
            if (projName.Contains("Lemon (evil)"))
                return;
        }

        if (target == stormEagleObject)
        {
            if (projName.Contains("Lemon (boss)"))
                return;
        }

        // Aplicar daño
        float vidaAnterior = healthDict[target];
        healthDict[target] -= damageAmount;

        // Logs según el tipo de objetivo:
        if (target == playerObject)
        {
            Debug.Log($"X perdió {damageAmount} de vida. Vida restante: {healthDict[target]}");

            if (healthDict[target] <= 0f)
            {
                Debug.Log("Jugador X ha muerto. ¡Game Over!");
                Destroy(target);
                return;
            }
        }
        else if (target.name.Contains("Bigbit"))
        {
            if (projName.Contains("Lemon") || projName.Contains("MegaShoot"))
                Debug.Log($"Bigbit {target.name} fue impactado por {projName}");

            if (healthDict[target] <= 0f)
            {
                Debug.Log($"Bigbit {target.name} muerto");
                Destroy(target);
                return;
            }
        }
        else if (target == stormEagleObject)
        {
            if (healthDict[target] <= 0f)
            {
                Debug.Log("¡Storm_Eagle derrotado! ¡Victoria!");
                Destroy(target);
                return;
            }
        }

        if (!projName.Contains("MegaShoot"))
        {
            Destroy(projectile);
        }
    }

    private float GetProjectileDamage(GameObject projectile)
    {
        string projName = projectile.name;
        if (projName.Contains("MegaShoot"))
            return damageMegaShoot;
        else if (projName.Contains("Lemon (evil)"))
            return damageLemonEvil;
        else if (projName.Contains("Lemon (boss)"))
            return damageLemonBoss;
        else if (projName.Contains("Lemon"))
            return damageLemon;
        return 0f;
    }
}
