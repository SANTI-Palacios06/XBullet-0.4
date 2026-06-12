using UnityEngine;

/// Dibuja la interfaz del menú principal.
public class MenuGUI : MonoBehaviour
{
    private string playerName = "";
    private int selectedOption = 0;
    private bool nombreInvalido = false;
    private bool isStarting = false;
    private string readyText = "";

    public void UpdateState(string name, int option, bool invalido, bool starting, string ready)
    {
        playerName = name;
        selectedOption = option;
        nombreInvalido = invalido;
        isStarting = starting;
        readyText = ready;
    }

    private void OnGUI()
    {
        float w = Screen.width;
        float h = Screen.height;

        Texture2D bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
        bg.Apply();

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 32,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.white;

        GUIStyle optionStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            alignment = TextAnchor.MiddleCenter
        };
        optionStyle.normal.textColor = Color.white;

        GUIStyle selectedStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        selectedStyle.normal.textColor = Color.yellow;

        GUIStyle inputStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter
        };
        inputStyle.normal.textColor = Color.white;

        GUIStyle placeholderStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Italic
        };
        placeholderStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        GUIStyle hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };
        hintStyle.normal.textColor = Color.white;

        GUIStyle errorStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        errorStyle.normal.textColor = Color.red;

        GUIStyle readyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 64,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        readyStyle.normal.textColor = readyText == "GO!" ? Color.green : Color.white;

        if (isStarting)
        {
            Texture2D fullBg = new Texture2D(1, 1);
            fullBg.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.75f));
            fullBg.Apply();

            GUI.DrawTexture(new Rect(0, 0, w, h), fullBg);
            GUI.Label(new Rect(0, h * 0.4f, w, 100), readyText, readyStyle);
            return;
        }

        GUI.DrawTexture(new Rect(w * 0.2f, h * 0.08f, w * 0.6f, 60), bg);
        GUI.Label(new Rect(w * 0.2f, h * 0.08f, w * 0.6f, 60), "X-BULLET PINBALL", titleStyle);

        GUI.DrawTexture(new Rect(w * 0.25f, h * 0.26f, w * 0.5f, 30), bg);
        GUI.Label(new Rect(w * 0.25f, h * 0.26f, w * 0.5f, 30), "Ingresa tu nombre:", hintStyle);

        if (string.IsNullOrEmpty(playerName))
        {
            GUI.Box(new Rect(w * 0.3f, h * 0.32f, w * 0.4f, 44), "Inserta tu nickname", placeholderStyle);
        }
        else
        {
            GUI.Box(new Rect(w * 0.3f, h * 0.32f, w * 0.4f, 44), playerName + "|", inputStyle);
        }

        if (nombreInvalido)
        {
            GUI.DrawTexture(new Rect(w * 0.25f, h * 0.41f, w * 0.5f, 34), bg);
            GUI.Label(new Rect(w * 0.25f, h * 0.41f, w * 0.5f, 34), "¡Nombre no permitido!", errorStyle);
        }

        GUI.DrawTexture(new Rect(w * 0.3f, h * 0.50f, w * 0.4f, 44), bg);
        GUI.Label(
            new Rect(w * 0.3f, h * 0.50f, w * 0.4f, 44),
            selectedOption == 0 ? "► INICIAR" : "  INICIAR",
            selectedOption == 0 ? selectedStyle : optionStyle
        );

        GUI.DrawTexture(new Rect(w * 0.3f, h * 0.58f, w * 0.4f, 44), bg);
        GUI.Label(
            new Rect(w * 0.3f, h * 0.58f, w * 0.4f, 44),
            selectedOption == 1 ? "► LEADERBOARD" : "  LEADERBOARD",
            selectedOption == 1 ? selectedStyle : optionStyle
        );

        GUI.DrawTexture(new Rect(w * 0.3f, h * 0.66f, w * 0.4f, 44), bg);
        GUI.Label(
            new Rect(w * 0.3f, h * 0.66f, w * 0.4f, 44),
            selectedOption == 2 ? "► SALIR" : "  SALIR",
            selectedOption == 2 ? selectedStyle : optionStyle
        );

        GUI.DrawTexture(new Rect(w * 0.1f, h * 0.82f, w * 0.8f, 60), bg);
        GUI.Label(new Rect(w * 0.1f, h * 0.82f, w * 0.8f, 30), "Palanca ↑↓ para navegar — A para confirmar", hintStyle);
        GUI.Label(new Rect(w * 0.1f, h * 0.87f, w * 0.8f, 30), "Teclado para escribir nombre — Enter para confirmar", hintStyle);
    }
}