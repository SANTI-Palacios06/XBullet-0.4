using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum SoundType
{
    shoot,
    chargeShoot,
    criticalHealth,
    victory,
    defeat,
    flipperHit,
    bumperHit
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioMixerGroup shootMixer;
    [SerializeField][Range(0f, 1f)] private float shootVolume = 1f;

    [Header("Disparo cargado")]
    [SerializeField] private AudioClip chargeShootClip;
    [SerializeField] private AudioMixerGroup chargeShootMixer;
    [SerializeField][Range(0f, 1f)] private float chargeShootVolume = 1f;

    [Header("Salud crítica")]
    [SerializeField] private AudioClip criticalHealthClip;
    [SerializeField] private AudioMixerGroup criticalHealthMixer;
    [SerializeField][Range(0f, 1f)] private float criticalHealthVolume = 1f;

    [Header("Victoria")]
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioMixerGroup victoryMixer;
    [SerializeField][Range(0f, 1f)] private float victoryVolume = 1f;

    [Header("Derrota")]
    [SerializeField] private AudioClip defeatClip1;
    [SerializeField] private AudioClip defeatClip2;
    [SerializeField] private AudioMixerGroup defeatMixer;
    [SerializeField][Range(0f, 1f)] private float defeatVolume = 1f;

    [Header("Flipper")]
    [SerializeField] private AudioClip flipperHitClip;
    [SerializeField] private AudioMixerGroup flipperHitMixer;
    [SerializeField][Range(0f, 1f)] private float flipperHitVolume = 1f;

    [Header("Bumper")]
    [SerializeField] private AudioClip bumperHitClip;
    [SerializeField] private AudioMixerGroup bumperHitMixer;
    [SerializeField][Range(0f, 1f)] private float bumperHitVolume = 1f;

    [Header("Configuración")]
    [SerializeField] private float chargeLoopStart = 2f;
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string leaderboardSceneName = "LeaderboardScene";

    private static SoundManager instance;

    private AudioSource audioSource;
    private AudioSource chargeLoopSource;
    private AudioSource resultSource;
    private AudioSource resultSource2;
    private AudioSource criticalLoopSource;
    private AudioSource flipperSource;
    private AudioSource bumperSource;

    private static bool resultPlaying = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        ConfigureSource(audioSource, false);

        chargeLoopSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(chargeLoopSource, false);

        resultSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(resultSource, false);

        resultSource2 = gameObject.AddComponent<AudioSource>();
        ConfigureSource(resultSource2, false);

        criticalLoopSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(criticalLoopSource, true);

        flipperSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(flipperSource, false);

        bumperSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(bumperSource, false);

        Debug.Log("SoundManager listo sin SoundSO.");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (chargeLoopSource == null)
        {
            return;
        }

        if (chargeLoopSource.isPlaying &&
            chargeLoopSource.clip != null &&
            chargeLoopSource.time >= chargeLoopSource.clip.length - 0.05f)
        {
            chargeLoopSource.time = Mathf.Clamp(chargeLoopStart, 0f, chargeLoopSource.clip.length);
        }
    }

    private static void ConfigureSource(AudioSource source, bool loop)
    {
        if (source == null)
        {
            return;
        }

        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.loop = loop;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuSceneName || scene.name == leaderboardSceneName)
        {
            resultPlaying = false;
            StopAllSounds();
            StopResultSounds();
        }
    }

    private static void PlayClip(AudioSource source, AudioClip clip, AudioMixerGroup mixer, float volume)
    {
        if (source == null)
        {
            Debug.LogWarning("No se puede reproducir sonido porque el AudioSource es null.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("No se puede reproducir sonido porque el AudioClip no está asignado.");
            return;
        }

        source.Stop();
        source.outputAudioMixerGroup = mixer;
        source.clip = clip;
        source.volume = Mathf.Clamp01(volume);
        source.Play();
    }

    public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1f)
    {
        if (instance == null)
        {
            Debug.LogWarning("No existe SoundManager en la escena.");
            return;
        }

        if (resultPlaying)
        {
            return;
        }

        switch (sound)
        {
            case SoundType.shoot:
                PlayClip(
                    source != null ? source : instance.audioSource,
                    instance.shootClip,
                    instance.shootMixer,
                    volume * instance.shootVolume
                );
                break;

            case SoundType.chargeShoot:
                StartChargeSound();
                break;

            case SoundType.criticalHealth:
                StartCriticalSound();
                break;

            case SoundType.flipperHit:
                PlayClip(
                    source != null ? source : instance.flipperSource,
                    instance.flipperHitClip,
                    instance.flipperHitMixer,
                    volume * instance.flipperHitVolume
                );
                break;

            case SoundType.bumperHit:
                PlayClip(
                    source != null ? source : instance.bumperSource,
                    instance.bumperHitClip,
                    instance.bumperHitMixer,
                    volume * instance.bumperHitVolume
                );
                break;

            case SoundType.victory:
            case SoundType.defeat:
                PlayResultSound(sound);
                break;
        }
    }

    public static void PlayResultSound(SoundType sound)
    {
        if (instance == null)
        {
            Debug.LogWarning("No existe SoundManager en la escena.");
            return;
        }

        resultPlaying = true;
        StopAllSounds();

        if (sound == SoundType.victory)
        {
            PlayClip(
                instance.resultSource,
                instance.victoryClip,
                instance.victoryMixer,
                instance.victoryVolume
            );
        }
        else if (sound == SoundType.defeat)
        {
            PlayClip(
                instance.resultSource,
                instance.defeatClip1,
                instance.defeatMixer,
                instance.defeatVolume
            );

            PlayClip(
                instance.resultSource2,
                instance.defeatClip2,
                instance.defeatMixer,
                instance.defeatVolume
            );
        }
    }

    public static void StartChargeSound()
    {
        if (instance == null || instance.chargeLoopSource == null)
        {
            return;
        }

        if (resultPlaying)
        {
            return;
        }

        if (instance.chargeLoopSource.isPlaying)
        {
            return;
        }

        PlayClip(
            instance.chargeLoopSource,
            instance.chargeShootClip,
            instance.chargeShootMixer,
            instance.chargeShootVolume
        );
    }

    public static void StopChargeSound()
    {
        if (instance == null || instance.chargeLoopSource == null)
        {
            return;
        }

        instance.chargeLoopSource.Stop();
    }

    public static void StartCriticalSound()
    {
        if (instance == null || instance.criticalLoopSource == null)
        {
            return;
        }

        if (resultPlaying)
        {
            return;
        }

        if (instance.criticalLoopSource.isPlaying)
        {
            return;
        }

        PlayClip(
            instance.criticalLoopSource,
            instance.criticalHealthClip,
            instance.criticalHealthMixer,
            instance.criticalHealthVolume
        );

        instance.criticalLoopSource.loop = true;
    }

    public static void StopCriticalSound()
    {
        if (instance == null || instance.criticalLoopSource == null)
        {
            return;
        }

        instance.criticalLoopSource.Stop();
    }

    public static void StopAllSounds()
    {
        if (instance == null)
        {
            return;
        }

        if (instance.audioSource != null)
        {
            instance.audioSource.Stop();
        }

        if (instance.chargeLoopSource != null)
        {
            instance.chargeLoopSource.Stop();
        }

        if (instance.criticalLoopSource != null)
        {
            instance.criticalLoopSource.Stop();
        }

        if (instance.flipperSource != null)
        {
            instance.flipperSource.Stop();
        }

        if (instance.bumperSource != null)
        {
            instance.bumperSource.Stop();
        }
    }

    public static void StopResultSounds()
    {
        if (instance == null)
        {
            return;
        }

        if (instance.resultSource != null)
        {
            instance.resultSource.Stop();
        }

        if (instance.resultSource2 != null)
        {
            instance.resultSource2.Stop();
        }
    }

    public static float GetClipLength(SoundType sound)
    {
        if (instance == null)
        {
            return 0f;
        }

        switch (sound)
        {
            case SoundType.shoot:
                return instance.shootClip != null ? instance.shootClip.length : 0f;

            case SoundType.chargeShoot:
                return instance.chargeShootClip != null ? instance.chargeShootClip.length : 0f;

            case SoundType.criticalHealth:
                return instance.criticalHealthClip != null ? instance.criticalHealthClip.length : 0f;

            case SoundType.victory:
                return instance.victoryClip != null ? instance.victoryClip.length : 0f;

            case SoundType.defeat:
                float defeat1 = instance.defeatClip1 != null ? instance.defeatClip1.length : 0f;
                float defeat2 = instance.defeatClip2 != null ? instance.defeatClip2.length : 0f;
                return Mathf.Max(defeat1, defeat2);

            case SoundType.flipperHit:
                return instance.flipperHitClip != null ? instance.flipperHitClip.length : 0f;

            case SoundType.bumperHit:
                return instance.bumperHitClip != null ? instance.bumperHitClip.length : 0f;

            default:
                return 0f;
        }
    }
}