using UnityEngine;

public class DetenerMapMusicTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StopMusic();
            Debug.Log("🎵 Música detenida.");
        }
    }
}
