using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// Orquesta el menú principal conectando todos los módulos.
[RequireComponent(typeof(ReadySequence))]
[RequireComponent(typeof(PlayerNameInput))]
[RequireComponent(typeof(MenuNavigator))]
[RequireComponent(typeof(MenuGUI))]
public class MainMenu : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nombre exacto de la escena del juego.")]
    [SerializeField] private string gameSceneName = "Pinball";

    [Tooltip("Nombre exacto de la escena del leaderboard.")]
    [SerializeField] private string leaderboardSceneName = "LeaderboardScene";

    private ReadySequence readySequence;
    private PlayerNameInput nameInput;
    private MenuNavigator navigator;
    private MenuGUI gui;
    private bool nombreInvalido = false;

    private void Awake()
    {
        readySequence = GetComponent<ReadySequence>();
        nameInput = GetComponent<PlayerNameInput>();
        navigator = GetComponent<MenuNavigator>();
        gui = GetComponent<MenuGUI>();

        navigator.OnConfirmPressed += OnConfirmPressed;
    }

    private void Update()
    {
        bool isStarting = readySequence.IsRunning;

        nameInput.SetBlocked(isStarting);
        navigator.SetBlocked(isStarting);

        if (!isStarting &&
            Keyboard.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            OnConfirmPressed();
        }

        gui.UpdateState(
            nameInput.PlayerName,
            navigator.SelectedOption,
            nombreInvalido,
            isStarting,
            readySequence.CurrentText
        );
    }

    private void OnConfirmPressed()
    {
        if (readySequence.IsRunning)
        {
            return;
        }

        switch (navigator.SelectedOption)
        {
            case 0:
                StartGame();
                break;

            case 1:
                OpenLeaderboard();
                break;

            case 2:
                QuitGame();
                break;

            default:
                Debug.LogWarning($"Opción de menú no reconocida: {navigator.SelectedOption}");
                break;
        }
    }

    private void StartGame()
    {
        if (!nameInput.IsValid())
        {
            nombreInvalido = true;
            return;
        }

        nombreInvalido = false;

        string finalName = string.IsNullOrWhiteSpace(nameInput.PlayerName)
            ? "Dummy"
            : nameInput.PlayerName.Trim();

        PlayerPrefs.SetString("PlayerName", finalName);
        PlayerPrefs.Save();

        Debug.Log($"Iniciando juego como: {finalName}");

        readySequence.Begin(gameSceneName);
    }

    private void OpenLeaderboard()
    {
        nombreInvalido = false;

        Debug.Log($"Abriendo leaderboard: {leaderboardSceneName}");

        Time.timeScale = 1f;
        SceneManager.LoadScene(leaderboardSceneName);
    }

    private void QuitGame()
    {
        Debug.Log("Saliendo del juego.");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        if (navigator != null)
        {
            navigator.OnConfirmPressed -= OnConfirmPressed;
        }
    }
}