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
        }
    }

    // Gestiona el loop manual al llegar al final vuelve a chargeLoopStart
    private void Update()
    {
        if (chargeLoopSource.isPlaying &&
            chargeLoopSource.time >= chargeLoopSource.clip.length - 0.05f)
        {
            chargeLoopSource.time = chargeLoopStart;
        }
    }

    // Reproduce un sonido del SO para cualquier sonido anterior para evitar solapamiento
    public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
    {
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
            instance.audioSource.Stop();
            instance.audioSource.outputAudioMixerGroup = soundList.mixer;
            instance.audioSource.clip   = randomClip;
            instance.audioSource.volume = volume * soundList.volume;
            instance.audioSource.Play();
        }
    }

    // Reproduce soniso de victoria o derrota
    public static void PlayResultSound(SoundType sound)
    {
        SoundList soundList = instance.SO.sounds[(int)sound];
        if (soundList.sounds == null || soundList.sounds.Length == 0) return;

        instance.resultSource.Stop();
        instance.resultSource.outputAudioMixerGroup = soundList.mixer;
        instance.resultSource.clip   = soundList.sounds[0];
        instance.resultSource.volume = soundList.volume;
        instance.resultSource.Play();
    }

    // Inicia el sonido de carga desde el inicio 
    public static void StartChargeSound()
    {
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
        instance.chargeLoopSource.Stop();
    }

    //Corte abrupto de todos los efectos de sonido al ser llamado
    public static void StopAllSounds()
    {
        instance.audioSource.Stop();
        instance.chargeLoopSource.Stop();
    }

    // Recuperra la duraccion del clip
    public static float GetClipLength(SoundType sound)
    {
        SoundList soundList = instance.SO.sounds[(int)sound];
        if (soundList.sounds == null || soundList.sounds.Length == 0) return 0f;
        return soundList.sounds[0].length;
    }
}