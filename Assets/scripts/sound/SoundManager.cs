using System;
using UnityEngine;
using UnityEngine.Audio;

//Especifica los tipos de sonido que va a tener el juego
public enum SoundType
{
    shoot,
    chargeShoot,
    criticalHealth,
    victory,
    defeat
}

//Se encarga de leer la lista de efectos de sonido
[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip[] sounds;
}

//administra el sound SO
[CreateAssetMenu(menuName = "Sounds SO", fileName = "Sounds SO")]
public class SoundSO : ScriptableObject
{
    public SoundList[] sounds;
}

//Revisa las fuente de los audios del SoundSO
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundSO SO;
    [SerializeField] private float chargeLoopStart = 2f;
    private static SoundManager instance = null;
    private AudioSource audioSource;
    private AudioSource chargeLoopSource;
    private AudioSource resultSource;
    private AudioSource criticalLoopSource;

    // Flag para bloquear sonidos mientras suena el resultado
    private static bool resultPlaying = false;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0f;
            audioSource.playOnAwake  = false;
            audioSource.loop         = false;

            chargeLoopSource = gameObject.AddComponent<AudioSource>();
            chargeLoopSource.spatialBlend = 0f;
            chargeLoopSource.playOnAwake  = false;
            chargeLoopSource.loop         = false;

            // AudioSource dedicado para victoria y derrota, no se solapa con el principal
            resultSource = gameObject.AddComponent<AudioSource>();
            resultSource.spatialBlend = 0f;
            resultSource.playOnAwake  = false;
            resultSource.loop         = false;

            // AudioSource dedicado para el sonido crítico en loop
            criticalLoopSource = gameObject.AddComponent<AudioSource>();
            criticalLoopSource.spatialBlend = 0f;
            criticalLoopSource.playOnAwake  = false;
            criticalLoopSource.loop         = true;
        }
    }

    // Gestiona el loop manual al llegar al final vuelve a chargeLoopStart
    private void Update()
    {
        if (chargeLoopSource == null) return;
        if (chargeLoopSource.isPlaying &&
            chargeLoopSource.time >= chargeLoopSource.clip.length - 0.05f)
        {
            chargeLoopSource.time = chargeLoopStart;
        }
    }

    // Reproduce un sonido del SO para cualquier sonido anterior para evitar solapamiento
    // No interrumpe si está sonando el resultado de victoria o derrota
    public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
    {
        if (instance == null) return;
        if (resultPlaying) return;

        SoundList soundList  = instance.SO.sounds[(int)sound];
        AudioClip[] clips    = soundList.sounds;
        if (clips == null || clips.Length == 0) return;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (source)
        {
            source.Stop();
            source.outputAudioMixerGroup = soundList.mixer;
            source.clip   = randomClip;
            source.volume = volume * soundList.volume;
            source.Play();
        }
        else
        {
            if (instance.audioSource == null) return;
            instance.audioSource.Stop();
            instance.audioSource.outputAudioMixerGroup = soundList.mixer;
            instance.audioSource.clip   = randomClip;
            instance.audioSource.volume = volume * soundList.volume;
            instance.audioSource.Play();
        }
    }

    // Reproduce sonido de victoria o derrota, ambos clips al mismo tiempo
    public static void PlayResultSound(SoundType sound)
    {
        if (instance == null) return;
        SoundList soundList = instance.SO.sounds[(int)sound];
        if (soundList.sounds == null || soundList.sounds.Length == 0) return;

        resultPlaying = true;

        // Primer clip en audioSource principal
        if (instance.audioSource == null) return;
        instance.audioSource.Stop();
        instance.audioSource.outputAudioMixerGroup = soundList.mixer;
        instance.audioSource.clip   = soundList.sounds[0];
        instance.audioSource.volume = soundList.volume;
        instance.audioSource.Play();

        // Segundo clip en resultSource al mismo tiempo
        if (soundList.sounds.Length > 1 && soundList.sounds[1] != null)
        {
            if (instance.resultSource == null) return;
            instance.resultSource.Stop();
            instance.resultSource.outputAudioMixerGroup = soundList.mixer;
            instance.resultSource.clip   = soundList.sounds[1];
            instance.resultSource.volume = soundList.volume;
            instance.resultSource.Play();
        }
    }

    // Inicia el sonido de carga desde el inicio 
    public static void StartChargeSound()
    {
        if (instance == null || instance.chargeLoopSource == null) return;
        if (instance.chargeLoopSource.isPlaying) return;
        SoundList soundList = instance.SO.sounds[(int)SoundType.chargeShoot];
        if (soundList.sounds == null || soundList.sounds.Length == 0) return;
        instance.chargeLoopSource.outputAudioMixerGroup = soundList.mixer;
        instance.chargeLoopSource.clip   = soundList.sounds[0];
        instance.chargeLoopSource.volume = soundList.volume;
        instance.chargeLoopSource.time   = 0f;
        instance.chargeLoopSource.Play();
    }

    // Corta el sonido de carga inmediatamente tras soltar X
    public static void StopChargeSound()
    {
        if (instance == null || instance.chargeLoopSource == null) return;
        instance.chargeLoopSource.Stop();
    }

    // Inicia el sonido crítico en loop
    public static void StartCriticalSound()
    {
        if (instance == null || instance.criticalLoopSource == null) return;
        if (resultPlaying) return;
        if (instance.criticalLoopSource.isPlaying) return;
        SoundList soundList = instance.SO.sounds[(int)SoundType.criticalHealth];
        if (soundList.sounds == null || soundList.sounds.Length == 0) return;
        instance.criticalLoopSource.outputAudioMixerGroup = soundList.mixer;
        instance.criticalLoopSource.clip   = soundList.sounds[0];
        instance.criticalLoopSource.volume = soundList.volume;
        instance.criticalLoopSource.Play();
    }

    // Detiene el sonido crítico
    public static void StopCriticalSound()
    {
        if (instance == null || instance.criticalLoopSource == null) return;
        instance.criticalLoopSource.Stop();
    }

    //Corte abrupto de todos los efectos de sonido al ser llamado
    // resultSource NO se detiene aquí para que el sonido de resultado suene completo
    public static void StopAllSounds()
    {
        if (instance == null) return;
        if (instance.audioSource != null)      instance.audioSource.Stop();
        if (instance.chargeLoopSource != null)  instance.chargeLoopSource.Stop();
        if (instance.criticalLoopSource != null) instance.criticalLoopSource.Stop();
    }

    // Recupera la duración del clip más largo
    public static float GetClipLength(SoundType sound)
    {
        if (instance == null) return 0f;
        SoundList soundList = instance.SO.sounds[(int)sound];
        if (soundList.sounds == null || soundList.sounds.Length == 0) return 0f;
        float maxLength = 0f;
        foreach (AudioClip clip in soundList.sounds)
            if (clip != null) maxLength = Mathf.Max(maxLength, clip.length);
        return maxLength;
    }
}