using UnityEngine;
using UnityEngine.InputSystem;

/// Maneja la navegación del menú con el control.
public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private InputActionReference navigateAction;
    [SerializeField] private InputActionReference confirmAction;

    private int   selectedOption = 0; // 0 = Iniciar, 1 = Salir
    private float navCooldown    = 0f;
    private bool  isBlocked      = false;

    public int SelectedOption => selectedOption;

    public event System.Action OnConfirmPressed;

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
    }

    private void OnEnable()
    {
        if (navigateAction != null)
        {
            navigateAction.action.Enable();
            navigateAction.action.performed += OnNavigate;
        }

        if (confirmAction != null)
        {
            confirmAction.action.Enable();
            confirmAction.action.performed += OnConfirm;
        }
    }

    private void OnDisable()
    {
        if (navigateAction != null)
        {
            navigateAction.action.performed -= OnNavigate;
            navigateAction.action.Disable();
        }

        if (confirmAction != null)
        {
            confirmAction.action.performed -= OnConfirm;
            confirmAction.action.Disable();
        }
    }

    private void Update()
    {
        if (navCooldown > 0f)
            navCooldown -= Time.deltaTime;
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (isBlocked) return;
        if (navCooldown > 0f) return;

        float value = 0f;
        try { value = ctx.ReadValue<float>(); }
        catch { return; }

        if (value > 0.5f)
        {
            if (ctx.control.name == "up")
            {
                selectedOption = 0;
                navCooldown    = 0.3f;
            }
            else if (ctx.control.name == "down")
            {
                selectedOption = 1;
                navCooldown    = 0.3f;
            }
        }
    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (isBlocked) return;
        OnConfirmPressed?.Invoke();
    }
}