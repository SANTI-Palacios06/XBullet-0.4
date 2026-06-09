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

    //Se enarga de manejar el input del teclado
    private void Update()
    {
        bool isStarting = readySequence.IsRunning;

        // Bloquea input durante Ready GO
        nameInput.SetBlocked(isStarting);
        navigator.SetBlocked(isStarting);

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

    //Módulo de Confirmación  
    private void OnConfirmPressed()
    {
        if (navigator.SelectedOption == 0)
            StartGame();
        else
            QuitGame();
    }

    //Módulo de empezar el juego
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

    //Módulo de salir del juego
    private void QuitGame()
    {
        Debug.Log("Saliendo del juego.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}