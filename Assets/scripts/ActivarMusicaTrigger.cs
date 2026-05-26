using UnityEngine;

public class ActivarMusicaTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip cancion;

    private static GameObject reproductorGlobal = null;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (reproductorGlobal == null)
        {
            reproductorGlobal = new GameObject("ReproductorMusica");
            reproductorGlobal.transform.position = other.transform.position;

            AudioSource source = reproductorGlobal.AddComponent<AudioSource>();
            source.clip = cancion;
            source.loop = true;
            source.volume = 1f;
            source.spatialBlend = 0f;
            source.playOnAwake = false;

            source.Play();

            Debug.Log("🎵 Música activada.");
        }
        else
        {
            AudioSource source = reproductorGlobal.GetComponent<AudioSource>();
            if (source != null)
            {
                source.Stop();  // ⚠ Aquí es donde estaba el detalle clave
                source.clip = cancion;
                source.Play();
                Debug.Log("🎵 Música cambiada correctamente.");
            }
        }
    }
}
