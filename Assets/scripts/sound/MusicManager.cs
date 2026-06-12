using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Música")]
    [Tooltip("Música del menú principal.")]
    public AudioClip menuMusic;

    [Tooltip("Música de la escena de juego.")]
    public AudioClip gameMusic;

    [Tooltip("Nombre exacto de la escena del juego.")]
    [SerializeField] private string gameSceneName = "Pinball";

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
        if (scene.name == gameSceneName)
            PlayMusic(gameMusic);
        else
            PlayMusic(menuMusic);
    }

    // Se encarga de cargar y reproducir la música seleccionada.
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
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