using UnityEngine;
using System.Collections;

public class PatternController : MonoBehaviour
{
    [Header("Tiempo total que dura cada patrón antes de cambiar")]
    [SerializeField] private float patternDuration = 10f;

    [Header("Tiempo de espera justo antes de iniciar cada nuevo patrón")]
    [SerializeField] private float delayBetweenPatterns = 0.5f;

    private BossShoot bossShoot;
    private int patternIndex = 0;
    private bool isActive = false;

    private void Start()
    {
        bossShoot = Object.FindFirstObjectByType<BossShoot>();

        if (bossShoot == null)
        {
            Debug.LogError("❌ PatternController: No se encontró BossShoot.");
            enabled = false;
            return;
        }
    }

    public void ActivatePatterns()
    {
        if (!isActive)
        {
            isActive = true;
            Debug.Log("🟢 PatternController: Patrones activados.");
            StartCoroutine(PatternLoop());
        }
    }

    private IEnumerator PatternLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenPatterns);

            switch (patternIndex)
            {
                case 0:
                    Debug.Log("🔄 Activando patrón 0: Classic Spiral");
                    yield return StartCoroutine(RunClassicSpiral());
                    break;

                case 1:
                    Debug.Log("🔄 Activando patrón 1: Rosette Pattern");
                    yield return StartCoroutine(RunRosettePattern());
                    break;

                case 2:
                    Debug.Log("🔄 Activando patrón 2: Segmented Flower");
                    yield return StartCoroutine(RunSegmentedFlower());
                    break;

                case 3:
                    Debug.Log("🔄 Activando patrón 3: Helical Fan");
                    yield return StartCoroutine(RunHelicalFan());
                    break;
            }

            patternIndex = (patternIndex + 1) % 4;
        }
    }

    private IEnumerator RunClassicSpiral()
    {
        float duration = patternDuration;
        float interval = 0.2f;

        while (duration > 0f)
        {
            bossShoot.ShootClassicSpiral();
            yield return new WaitForSeconds(interval);
            duration -= interval;
        }
    }

    private IEnumerator RunRosettePattern()
    {
        float duration = patternDuration;
        float interval = 0.4f;

        while (duration > 0f)
        {
            bossShoot.ShootRosettePattern();
            yield return new WaitForSeconds(interval);
            duration -= interval;
        }
    }

    private IEnumerator RunSegmentedFlower()
    {
        float duration = patternDuration;
        float interval = 0.5f;

        while (duration > 0f)
        {
            bossShoot.ShootSegmentedFlower();
            yield return new WaitForSeconds(interval);
            duration -= interval;
        }
    }

    private IEnumerator RunHelicalFan()
    {
        float duration = patternDuration;
        float interval = 0.25f;

        while (duration > 0f)
        {
            bossShoot.ShootHelicalFan();
            yield return new WaitForSeconds(interval);
            duration -= interval;
        }
    }
}
