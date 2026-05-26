using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera bossCamera;

    private AudioListener mainAudioListener;
    private AudioListener bossAudioListener;

    private void Start()
    {
        if (mainCamera != null)
        {
            mainCamera.enabled = true;
            mainAudioListener = mainCamera.GetComponent<AudioListener>();
            if (mainAudioListener != null) mainAudioListener.enabled = true;
        }

        if (bossCamera != null)
        {
            bossCamera.enabled = false;
            bossAudioListener = bossCamera.GetComponent<AudioListener>();
            if (bossAudioListener != null) bossAudioListener.enabled = false;
        }
    }

    public void ActivateBossCamera()
    {
        if (mainCamera != null) mainCamera.enabled = false;
        if (bossCamera != null) bossCamera.enabled = true;

        if (mainAudioListener != null) mainAudioListener.enabled = false;
        if (bossAudioListener != null) bossAudioListener.enabled = true;
    }

    public void ActivateMainCamera()
    {
        if (mainCamera != null) mainCamera.enabled = true;
        if (bossCamera != null) bossCamera.enabled = false;

        if (mainAudioListener != null) mainAudioListener.enabled = true;
        if (bossAudioListener != null) bossAudioListener.enabled = false;
    }
}
