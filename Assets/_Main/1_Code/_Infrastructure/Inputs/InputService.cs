using System;
using UnityEngine;
using VContainer;

public class InputService : IDisposable
{
    public event Action OnInteractPressed;
    public event Action OnJumpPerformed;
    public event Action OnSprintPerformed;
    public event Action OnSprintCanceled;
    public event Action OnCrouchPerformed;
    public event Action OnCrouchCanceled;
    public event Action OnFlashLightToggle;
    public event Action OnShowTasksMenu;
    public event Action<bool> OnZoomPressed;
    public event Action OnPauseStarted;
    public event Action OnOpenConsole;

    private InputSystem_Actions _inputs;
    private Vector2 _moveInput;
    private Vector2 _lookInput;


    [Inject]
    public InputService(InputSystem_Actions inputs)
    {
        _inputs = inputs;
    }

    public void Init()
    {
        _inputs.Enable();

        _inputs.Global.Console.started += OpenConsole;
        _inputs.Global.Escape.started += Pause_started;

        _inputs.Player.Interact.started += Interact_started;
        _inputs.Player.Jump.performed += Jump_performed;
        _inputs.Player.Sprint.performed += Sprint_performed;
        _inputs.Player.Sprint.canceled += Sprint_canceled;

        _inputs.Player.Crouch.performed += CrouchPerformed;
        _inputs.Player.Crouch.canceled += CrouchCanceled;

        _inputs.Player.Flashlight.started += StartFlashLight;

        _inputs.Player.ShowTasks.started += ShowTasks_started;

        _inputs.Player.Zoom.started += HandleZoom;
        _inputs.Player.Zoom.performed += HandleZoom;
        _inputs.Player.Zoom.canceled += HandleZoom;
        Debug.Log($"[{GetType().Name}] init complete");
    }

    public void ClearGameplayInputs()
    {
        _moveInput = Vector2.zero;
        _lookInput = Vector2.zero;
        OnInteractPressed = null;
        OnJumpPerformed = null;
        OnSprintPerformed = null;
        OnSprintCanceled = null;
        OnCrouchPerformed = null;
        OnCrouchCanceled = null;
        OnFlashLightToggle = null;
        OnShowTasksMenu = null;
        OnZoomPressed = null;
        OnPauseStarted = null;
    }


    public void Dispose()
    {
        _inputs.Disable();
        _inputs.Player.Interact.started -= Interact_started;
        _inputs.Player.Jump.performed -= Jump_performed;
        _inputs.Player.Sprint.performed -= Sprint_performed;
        _inputs.Player.Sprint.canceled -= Sprint_canceled;
        _inputs.Player.Pause.started -= Pause_started;
        _inputs.Player.Zoom.started -= HandleZoom;
        _inputs.Player.Zoom.performed -= HandleZoom;
        _inputs.Player.Zoom.canceled -= HandleZoom;
        _inputs.Player.ShowTasks.started -= ShowTasks_started;

        _moveInput = Vector2.zero;
        _lookInput = Vector2.zero;
        OnInteractPressed = null;
        OnJumpPerformed = null;
        OnSprintPerformed = null;
        OnSprintCanceled = null;
        OnCrouchPerformed = null;
        OnCrouchCanceled = null;
        OnFlashLightToggle = null;
        OnShowTasksMenu = null;
        OnZoomPressed = null;
        OnPauseStarted = null;
        OnOpenConsole = null;

    }






    






    public Vector2 GetMoveInputClamped()
    {
        _moveInput = _inputs.Player.Move.ReadValue<Vector2>();
        _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);
        return _moveInput;
    }

    public Vector2 GetLookInput()
    {
        _lookInput = _inputs.Player.Look.ReadValue<Vector2>();
        return _lookInput;
    }


    private void HandleZoom(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnZoomPressed?.Invoke(obj.ReadValueAsButton());
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPressed?.Invoke();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpPerformed?.Invoke();
    }

    private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintPerformed?.Invoke();
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintCanceled?.Invoke();
    }

    private void CrouchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouchPerformed?.Invoke();
    }

    private void CrouchCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouchCanceled?.Invoke();
    }

    private void StartFlashLight(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFlashLightToggle?.Invoke();
    }

    private void ShowTasks_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnShowTasksMenu?.Invoke();
    }

    private void Pause_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseStarted?.Invoke();
    }

    private void OpenConsole(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnOpenConsole?.Invoke();
    }

    public void ToggleGameplayMap(bool isActive)
    {
        if (isActive == true)
        {
            _inputs.Player.Enable();
        }
        else
        {
            _inputs.Player.Disable();
        }
    }


    public void ToggleUiMap(bool isActive)
    {
        if (isActive == true)
        {
            _inputs.UI.Enable();
        }
        else
        {
            _inputs.UI.Disable();
        }
    }
}
