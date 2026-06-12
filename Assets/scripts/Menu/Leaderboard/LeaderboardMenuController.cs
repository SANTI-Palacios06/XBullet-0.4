using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LeaderboardMenuController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int limit = 10;
    [SerializeField] private string menuSceneName = "Menu";

    private readonly List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    private string message = "Cargando leaderboard...";
    private bool loading = false;
    private Vector2 scrollPosition;

    private int selectedButton = 0;
    private float navCooldown = 0f;

    private const int TotalButtons = 2;

    private GUIStyle titleStyle;
    private GUIStyle headerStyle;
    private GUIStyle rowStyle;
    private GUIStyle buttonStyle;
    private GUIStyle selectedButtonStyle;
    private GUIStyle messageStyle;
    private GUIStyle hintStyle;

    private void Start()
    {
        Time.timeScale = 1f;
        LoadLeaderboard();
    }

    private void Update()
    {
        if (navCooldown > 0f)
        {
            navCooldown -= Time.unscaledDeltaTime;
        }

        HandleKeyboardInput();
        HandleGamepadInput();
    }

    //Controla el manejo de teclas
    private void HandleKeyboardInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (navCooldown <= 0f)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame ||
                Keyboard.current.upArrowKey.wasPressedThisFrame ||
                Keyboard.current.aKey.wasPressedThisFrame ||
                Keyboard.current.wKey.wasPressedThisFrame)
            {
                MoveSelection(-1);
            }

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame ||
                Keyboard.current.downArrowKey.wasPressedThisFrame ||
                Keyboard.current.dKey.wasPressedThisFrame ||
                Keyboard.current.sKey.wasPressedThisFrame)
            {
                MoveSelection(1);
            }
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ConfirmSelection();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GoBackToMenu();
        }
    }

    //maneja el uso de ocntrol

    private void HandleGamepadInput()
    {
        if (Gamepad.current == null)
        {
            return;
        }

        if (navCooldown <= 0f)
        {
            if (Gamepad.current.dpad.left.wasPressedThisFrame ||
                Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                MoveSelection(-1);
            }

            if (Gamepad.current.dpad.right.wasPressedThisFrame ||
                Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                MoveSelection(1);
            }
        }

        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            ConfirmSelection();
        }

        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            GoBackToMenu();
        }
    }

    private void MoveSelection(int direction)
    {
        selectedButton += direction;

        if (selectedButton < 0)
        {
            selectedButton = TotalButtons - 1;
        }

        if (selectedButton >= TotalButtons)
        {
            selectedButton = 0;
        }

        navCooldown = 0.25f;

        Debug.Log($"Botón seleccionado en leaderboard: {selectedButton}");
    }

    private void ConfirmSelection()
    {
        switch (selectedButton)
        {
            case 0:
                LoadLeaderboard();
                break;

            case 1:
                GoBackToMenu();
                break;
        }
    }

    private void GoBackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private void LoadLeaderboard()
    {
        if (ScoreServiceClient.Instance == null)
        {
            message = "No se encontró ScoreServiceClient en la escena.";
            loading = false;
            return;
        }

        loading = true;
        message = "Cargando leaderboard...";
        entries.Clear();

        ScoreServiceClient.Instance.FetchLeaderboard(
            limit,
            result =>
            {
                loading = false;
                entries.Clear();
                entries.AddRange(result);

                message = entries.Count == 0
                    ? "Todavía no hay partidas terminadas."
                    : "";
            },
            error =>
            {
                loading = false;
                message = error;
            });
    }


//Creaccion de la UI
    private void OnGUI()
    {
        InitStyles();

        float panelWidth = Mathf.Min(900f, Screen.width - 60f);
        float panelHeight = Mathf.Min(580f, Screen.height - 60f);

        Rect panelRect = new Rect(
            (Screen.width - panelWidth) * 0.5f,
            (Screen.height - panelHeight) * 0.5f,
            panelWidth,
            panelHeight
        );

        GUI.color = new Color(0f, 0f, 0f, 0.86f);
        GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUILayout.BeginArea(new Rect(
            panelRect.x + 24f,
            panelRect.y + 20f,
            panelRect.width - 48f,
            panelRect.height - 40f
        ));

        GUILayout.Label("LEADERBOARD", titleStyle);
        GUILayout.Space(12f);

        if (loading || !string.IsNullOrWhiteSpace(message))
        {
            GUILayout.Label(message, messageStyle);
            GUILayout.Space(16f);
        }

        if (entries.Count > 0)
        {
            DrawHeader();

            GUILayout.Space(6f);

            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Height(panelHeight - 210f)
            );

            for (int i = 0; i < entries.Count; i++)
            {
                DrawRow(i + 1, entries[i]);
            }

            GUILayout.EndScrollView();
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label("D-Pad ← → / ↑ ↓ para navegar — Enter o A para confirmar — Esc para volver", hintStyle);
        GUILayout.Space(8f);

        GUILayout.BeginHorizontal();

        string refreshText = selectedButton == 0 ? "► Refrescar" : "  Refrescar";
        string backText = selectedButton == 1 ? "► Volver al menú" : "  Volver al menú";

        if (GUILayout.Button(
            refreshText,
            selectedButton == 0 ? selectedButtonStyle : buttonStyle,
            GUILayout.Height(42f)))
        {
            selectedButton = 0;
            LoadLeaderboard();
        }

        GUILayout.Space(12f);

        if (GUILayout.Button(
            backText,
            selectedButton == 1 ? selectedButtonStyle : buttonStyle,
            GUILayout.Height(42f)))
        {
            selectedButton = 1;
            GoBackToMenu();
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void DrawHeader()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("#", headerStyle, GUILayout.Width(40f));
        GUILayout.Label("Jugador", headerStyle, GUILayout.Width(180f));
        GUILayout.Label("Score", headerStyle, GUILayout.Width(120f));
        GUILayout.Label("Boss Hits", headerStyle, GUILayout.Width(110f));
        GUILayout.Label("Estado", headerStyle, GUILayout.Width(130f));
        GUILayout.Label("Escenario", headerStyle, GUILayout.Width(230f));

        GUILayout.EndHorizontal();
    }

    private void DrawRow(int rank, LeaderboardEntry entry)
    {
        GUILayout.BeginHorizontal("box");

        GUILayout.Label(rank.ToString(), rowStyle, GUILayout.Width(40f));
        GUILayout.Label(SafeText(entry.playerName, "Jugador"), rowStyle, GUILayout.Width(180f));
        GUILayout.Label(entry.finalScore.ToString(), rowStyle, GUILayout.Width(120f));
        GUILayout.Label($"{entry.bossHits}/{entry.targetBossHits}", rowStyle, GUILayout.Width(110f));
        GUILayout.Label(TranslateStatus(entry.status), rowStyle, GUILayout.Width(130f));
        GUILayout.Label(SafeText(entry.stageName, "Sin escenario"), rowStyle, GUILayout.Width(230f));

        GUILayout.EndHorizontal();
    }

    private string TranslateStatus(string status)
    {
        switch (status)
        {
            case "Victory":
                return "Victoria";

            case "Defeat":
                return "Derrota";

            case "InProgress":
                return "En progreso";

            default:
                return string.IsNullOrWhiteSpace(status) ? "Desconocido" : status;
        }
    }

    private string SafeText(string value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private void InitStyles()
    {
        if (titleStyle != null)
        {
            return;
        }

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 36,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = Color.white;

        headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };
        headerStyle.normal.textColor = Color.white;

        rowStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15
        };
        rowStyle.normal.textColor = Color.white;

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 17,
            fontStyle = FontStyle.Bold
        };
        buttonStyle.normal.textColor = Color.white;

        selectedButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 17,
            fontStyle = FontStyle.Bold
        };
        selectedButtonStyle.normal.textColor = Color.yellow;
        selectedButtonStyle.hover.textColor = Color.yellow;
        selectedButtonStyle.active.textColor = Color.yellow;
        selectedButtonStyle.focused.textColor = Color.yellow;

        messageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 17,
            alignment = TextAnchor.MiddleCenter
        };
        messageStyle.normal.textColor = Color.white;

        hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };
        hintStyle.normal.textColor = Color.white;
    }
}