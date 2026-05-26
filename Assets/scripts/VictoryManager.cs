using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject victoryText;  // AQUI SOLO EL TEXT, NO EL CANVAS

    [Header("Musica")]
    [SerializeField] private AudioClip cancion;

    private AudioSource reproductor;

    private readonly HashSet<string> allowedScripts = new HashSet<string>
    {
        "VictoryManager",
        "VictoryTrigger",
        "VictoryUI"
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        reproductor = GetComponent<AudioSource>();
        if (reproductor == null)
            reproductor = gameObject.AddComponent<AudioSource>();

        reproductor.playOnAwake = false;
    }

    public void TriggerVictory()
    {
        Debug.Log("🎯 ¡VICTORIA ACTIVADA!");

        if (victoryText != null)
            victoryText.SetActive(true);

        DisableAllScriptsExceptAllowed();

        StopAllOtherMusic();

        if (cancion != null)
        {
            reproductor.clip = cancion;
            reproductor.Play();
        }

        StartCoroutine(EndGameAfterDelay(9f));
    }

    private void StopAllOtherMusic()
    {
        AudioSource[] allAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource audio in allAudioSources)
        {
            if (audio != reproductor)
            {
                audio.Stop();
            }
        }

        Debug.Log("🎵 Todas las canciones previas detenidas.");
    }

    private void DisableAllScriptsExceptAllowed()
    {
        MonoBehaviour[] allScripts = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour script in allScripts)
        {
            if (!allowedScripts.Contains(script.GetType().Name))
            {
                script.enabled = false;
            }
        }

        Debug.Log("✅ Todos los scripts desactivados excepto los permitidos.");
    }

    private IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("🛑 Cerrando juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
