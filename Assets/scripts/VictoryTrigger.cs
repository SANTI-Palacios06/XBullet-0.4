using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (VictoryManager.Instance != null)
        {
            VictoryManager.Instance.TriggerVictory();
        }
    }
}
