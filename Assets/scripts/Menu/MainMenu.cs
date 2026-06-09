using UnityEngine;
using UnityEngine.InputSystem;

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

    private ReadySequence   readySequence;
    private PlayerNameInput nameInput;
    private MenuNavigator   navigator;
    private MenuGUI         gui;
    private bool            nombreInvalido = false;

    private void Awake()
    {
        readySequence = GetComponent<ReadySequence>();
        nameInput     = GetComponent<PlayerNameInput>();
        navigator     = GetComponent<MenuNavigator>();
        gui           = GetComponent<MenuGUI>();

        navigator.OnConfirmPressed += OnConfirmPressed;
    }

    private void Update()
    {
        bool isStarting = readySequence.IsRunning;

        // Bloquea input durante Ready GO
        nameInput.SetBlocked(isStarting);
        navigator.SetBlocked(isStarting);

        // Enter desde teclado solo si no está iniciando
        if (!isStarting && Keyboard.current != null &&
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
        if (navigator.SelectedOption == 0)
            StartGame();
        else
            QuitGame();
    }

    private void StartGame()
    {
        if (!nameInput.IsValid())
        {
            nombreInvalido = true;
            return;
        }

        nombreInvalido = false;

        string finalName = string.IsNullOrWhiteSpace(nameInput.PlayerName) ? "Dummy" : nameInput.PlayerName.Trim();

        PlayerPrefs.SetString("PlayerName", finalName);
        PlayerPrefs.Save();

        Debug.Log($"Iniciando juego como: {finalName}");

        readySequence.Begin(gameSceneName);
    }

    private void QuitGame()
    {
        Debug.Log("Saliendo del juego.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}