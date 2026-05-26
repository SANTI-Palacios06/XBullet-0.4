using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnOnDestroy : MonoBehaviour
{
    [Header("Audios")]
    [SerializeField] private AudioClip sonido1;
    [SerializeField] private AudioClip sonido2;

    private bool alreadyTriggered = false;

    private readonly HashSet<string> allowedScripts = new HashSet<string>
    {
        "RespawnOnDestroy"
    };

    private void OnDestroy()
    {
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        Debug.Log("💥 Objeto destruido, iniciando proceso de cierre.");

        PlayAudioClip(sonido1);
        PlayAudioClip(sonido2);

        StartCoroutine(CloseGameAfterDelay(3f));
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip == null) return;

        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(tempAudio, clip.length);
    }

    private IEnumerator CloseGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        StopAllMusic();
        DisableAllScriptsExceptAllowed();

        Debug.Log("🛑 Cerrando juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StopAllMusic()
    {
        AudioSource[] allAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource audio in allAudioSources)
        {
            if (!audio.gameObject.name.StartsWith("TempAudio"))
            {
                audio.Stop();
            }
        }

        Debug.Log("🎵 Toda la música del juego fue detenida.");
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

        Debug.Log("✅ Todos los scripts desactivados excepto RespawnOnDestroy.");
    }
}
