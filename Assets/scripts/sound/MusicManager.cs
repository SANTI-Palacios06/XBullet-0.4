using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Música por escena (índice = buildIndex)")]
    public AudioClip[] musicByScene;

    private AudioSource audioSource;

// Se encarga de reproducir la música.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop         = true;
        audioSource.volume       = 1f;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake  = false;
    }

// Se encarga de cargar la música al entrar en una escena.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

// Se encarga de gestionar la música entre escenas.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicByScene == null || scene.buildIndex >= musicByScene.Length)
            return;

        AudioClip clip = musicByScene[scene.buildIndex];

        if (clip != null)
            PlayMusic(clip);
        else
            StopMusic();
    }

// Se encarga de cargar y reproducir la música seleccionada.
    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}