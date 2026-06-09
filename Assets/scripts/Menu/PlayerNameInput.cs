using UnityEngine;
using UnityEngine.InputSystem;

/// Maneja la escritura del nombre del jugador desde el teclado.
public class PlayerNameInput : MonoBehaviour
{
    [Tooltip("Máximo de caracteres permitidos.")]
    [SerializeField] private int maxLength = 15;

    private string playerName = "";
    private bool   isBlocked  = false;

    // Retorna el nombre escrito, sin fallback automático
    public string PlayerName => playerName;

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
    }

    private void Update()
    {
        if (isBlocked) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            if (playerName.Length > 0)
                playerName = playerName.Substring(0, playerName.Length - 1);
            return;
        }

        foreach (var key in System.Enum.GetValues(typeof(Key)))
        {
            Key k = (Key)key;
            if (k == Key.None) continue;

            if (Keyboard.current[k].wasPressedThisFrame)
            {
                try
                {
                    string character = KeyToChar(k);
                    if (!string.IsNullOrEmpty(character) && playerName.Length < maxLength)
                        playerName += character;
                }
                catch
                {
                    Debug.Log($"Input inválido: {k}");
                }
            }
        }
    }

    // Valida que el nombre no tenga espacios ni caracteres no permitidos
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(playerName)) return false;
        if (playerName.Contains(" ")) return false;

        foreach (char c in playerName)
        {
            bool esLetra  = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
            bool esNumero = c >= '0' && c <= '9';

            if (!esLetra && !esNumero)
                return false;
        }

        return true;
    }

    private string KeyToChar(Key key)
    {
        bool shift = Keyboard.current.shiftKey.isPressed;

        return key switch
        {
            Key.A => shift ? "A" : "a",
            Key.B => shift ? "B" : "b",
            Key.C => shift ? "C" : "c",
            Key.D => shift ? "D" : "d",
            Key.E => shift ? "E" : "e",
            Key.F => shift ? "F" : "f",
            Key.G => shift ? "G" : "g",
            Key.H => shift ? "H" : "h",
            Key.I => shift ? "I" : "i",
            Key.J => shift ? "J" : "j",
            Key.K => shift ? "K" : "k",
            Key.L => shift ? "L" : "l",
            Key.M => shift ? "M" : "m",
            Key.N => shift ? "N" : "n",
            Key.O => shift ? "O" : "o",
            Key.P => shift ? "P" : "p",
            Key.Q => shift ? "Q" : "q",
            Key.R => shift ? "R" : "r",
            Key.S => shift ? "S" : "s",
            Key.T => shift ? "T" : "t",
            Key.U => shift ? "U" : "u",
            Key.V => shift ? "V" : "v",
            Key.W => shift ? "W" : "w",
            Key.X => shift ? "X" : "x",
            Key.Y => shift ? "Y" : "y",
            Key.Z => shift ? "Z" : "z",
            Key.Digit0 => "0",
            Key.Digit1 => "1",
            Key.Digit2 => "2",
            Key.Digit3 => "3",
            Key.Digit4 => "4",
            Key.Digit5 => "5",
            Key.Digit6 => "6",
            Key.Digit7 => "7",
            Key.Digit8 => "8",
            Key.Digit9 => "9",
            _ => ""
        };
    }
}