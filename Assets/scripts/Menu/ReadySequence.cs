using UnityEngine;
using UnityEngine.SceneManagement;

/// Maneja la secuencia Ready? GO! antes de cargar la escena.
public class ReadySequence : MonoBehaviour
{
    [Tooltip("Duración total de la secuencia en segundos.")]
    [SerializeField] private float duration = 2f;

    [Tooltip("Sonido que suena al aparecer READY?.")]
    [SerializeField] private AudioClip readySound;

    [Tooltip("Sonido que suena al aparecer GO!.")]
    [SerializeField] private AudioClip goSound;

    private bool   isRunning     = false;
    private float  timer         = 0f;
    private string currentText   = "";
    private string targetScene   = "";
    private bool   goSoundPlayed = false;
    private float  goSoundLength = 0f;
    private bool   goSoundDone   = false;

    private AudioSource audioSource;

    public bool IsRunning     => isRunning;
    public string CurrentText => currentText;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake  = false;
        audioSource.loop         = false;
    }

    public void Begin(string sceneName)
    {
        targetScene   = sceneName;
        isRunning     = true;
        timer         = 0f;
        currentText   = "READY?";
        goSoundPlayed = false;
        goSoundDone   = false;
        goSoundLength = goSound != null ? goSound.length : 0f;

        // Suena READY?
        if (readySound != null)
        {
            audioSource.clip = readySound;
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;

        // Cambia a GO! a la mitad
        if (timer >= duration * 0.5f && !goSoundPlayed)
        {
            currentText   = "GO!";
            goSoundPlayed = true;

            if (goSound != null)
            {
                audioSource.clip = goSound;
                audioSource.Play();
            }
        }

        // Espera a que termine el audio de GO! antes de cargar
        if (goSoundPlayed && !goSoundDone)
        {
            if (!audioSource.isPlaying)
            {
                goSoundDone = true;
                isRunning   = false;
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}