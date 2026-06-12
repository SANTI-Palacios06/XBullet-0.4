using UnityEngine;

/// Muestra la vida del jugador en la UI durante la partida.
public class HealthHUD : MonoBehaviour
{
    //Obtiene la salud del jugador
    private CombatHealth playerHealth;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            playerHealth = playerObject.GetComponentInChildren<CombatHealth>()
                        ?? playerObject.GetComponentInParent<CombatHealth>()
                        ?? playerObject.GetComponent<CombatHealth>();

        if (playerHealth == null)
            Debug.LogWarning("HealthHUD — no se encontró CombatHealth en el jugador.");
    }

    //Maneja la UI del Jugador
    private void OnGUI()
    {
        if (playerHealth == null) return;

        float w = Screen.width;

        Texture2D bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
        bg.Apply();

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 13,
            alignment = TextAnchor.MiddleRight,
            fontStyle = FontStyle.Bold
        };
        labelStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

        GUIStyle valueStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 28,
            alignment = TextAnchor.MiddleRight,
            fontStyle = FontStyle.Bold
        };
        valueStyle.normal.textColor = playerHealth.GetCurrentPhaseColor();

        float panelW = 220f;
        float panelH = 60f;
        float panelX = w - panelW - 16f;
        float panelY = 106f; 

        GUI.DrawTexture(new Rect(panelX, panelY, panelW, panelH), bg);

        GUI.Label(new Rect(panelX, panelY + 4f, panelW - 10f, 20f), "HP", labelStyle);

        GUI.Label(new Rect(panelX, panelY + 18f, panelW - 10f, 36f),
            $"{playerHealth.CurrentHealth}", valueStyle);
    }
}