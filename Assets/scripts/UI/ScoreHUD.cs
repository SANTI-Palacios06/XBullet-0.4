using UnityEngine;

/// Muestra el score y nombre del jugador en la UI durante la partida.
public class ScoreHUD : MonoBehaviour
{
    private string playerName = "Dummy";
    private int    currentScore = 0;

    private void Start()
    {
        // Carga el nombre desde PlayerPrefs
        string savedName = PlayerPrefs.GetString("PlayerName", "Dummy");
        playerName = string.IsNullOrWhiteSpace(savedName) ? "Dummy" : savedName.Trim();

        // Se suscribe al evento del score manager
        if (PinballScoreManager.Instance != null)
        {
            currentScore = PinballScoreManager.Instance.CurrentScore;
            PinballScoreManager.Instance.ScoreChanged += OnScoreChanged;
        }
    }

    private void OnDestroy()
    {
        if (PinballScoreManager.Instance != null)
            PinballScoreManager.Instance.ScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged()
    {
        if (PinballScoreManager.Instance != null)
            currentScore = PinballScoreManager.Instance.CurrentScore;
    }

    private void OnGUI()
    {
        float w = Screen.width;

        // Fondo
        Texture2D bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
        bg.Apply();

        GUIStyle nameStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 18,
            alignment = TextAnchor.MiddleRight,
            fontStyle = FontStyle.Bold
        };
        nameStyle.normal.textColor = new Color(1f, 0.85f, 0.2f);

        GUIStyle scoreStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 28,
            alignment = TextAnchor.MiddleRight,
            fontStyle = FontStyle.Bold
        };
        scoreStyle.normal.textColor = Color.white;

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 13,
            alignment = TextAnchor.MiddleRight
        };
        labelStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

        // Panel
        float panelW = 220f;
        float panelH = 90f;
        float panelX = w - panelW - 16f;
        float panelY = 16f;

        GUI.DrawTexture(new Rect(panelX, panelY, panelW, panelH), bg);

        // Nombre del jugador
        GUI.Label(new Rect(panelX, panelY + 6f, panelW - 10f, 24f), playerName, nameStyle);

        // Label SCORE
        GUI.Label(new Rect(panelX, panelY + 30f, panelW - 10f, 20f), "SCORE", labelStyle);

        // Valor del score
        GUI.Label(new Rect(panelX, panelY + 46f, panelW - 10f, 36f), currentScore.ToString("N0"), scoreStyle);
    }
}