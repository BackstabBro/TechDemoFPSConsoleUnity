using UnityEngine;
using VContainer;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Transform _camPos;
    [SerializeField] private Transform _flashLight;

    private GameStateService _gameStateService;

    private InputService _inputService;
    private PlayerService _playerService;
    private PlayerMovementController _MovementController;
    private PlayerCameraController _CameraController;
    private PlayerInteractController _InteractController;
    private PlayerSoundController _SoundController;

    private HeadBlobber _headBlobber;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private bool _isZooming = false;
    private bool _isFlashLightTurnedOn = false;

    [Inject]
    public void Construct(
        GameStateService gameStateService,
        InputService inputService,
        PlayerService playerService,
        PlayerMovementController playerMovementController,
        PlayerCameraController playerCameraController,
        PlayerInteractController playerInteractController,
        PlayerSoundController playerSoundController,
        HeadBlobber headBlobber
        )
    {
        _inputService = inputService;
        _playerService = playerService;
        _gameStateService = gameStateService;
        _MovementController = playerMovementController;
        _CameraController = playerCameraController;
        _InteractController = playerInteractController;
        _SoundController = playerSoundController;
        _headBlobber = headBlobber;
    }

    public void Init()
    {
        _inputService.OnInteractPressed += HandleInteract;
        _inputService.OnJumpPerformed += Jump;
        _inputService.OnSprintPerformed += Sprint;
        _inputService.OnSprintCanceled += CancelSprint;
        _inputService.OnCrouchPerformed += StartCrouch;
        _inputService.OnCrouchCanceled += StopCrouch;
        _inputService.OnFlashLightToggle += HandleFlashlight;
        _inputService.OnZoomPressed += HandleZoom;
        _playerService.OnPlayerDead += HandlePlayerDead;
        _MovementController.OnCalculateFallDAmage += ApplyFallDamage;

        _CameraController.Init();
        _MovementController.Init();
        Debug.Log($"[{GetType().Name}] init complete");

    }

    public void CleanUp()
    {
        _inputService.OnInteractPressed -= HandleInteract;
        _inputService.OnJumpPerformed -= Jump;
        _inputService.OnSprintPerformed -= Sprint;
        _inputService.OnSprintCanceled -= CancelSprint;
        _inputService.OnCrouchPerformed -= StartCrouch;
        _inputService.OnCrouchCanceled -= StopCrouch;
        _inputService.OnFlashLightToggle -= HandleFlashlight;
        _inputService.OnZoomPressed -= HandleZoom;
        _playerService.OnPlayerDead -= HandlePlayerDead;
        _MovementController.OnCalculateFallDAmage -= ApplyFallDamage;

        _MovementController.CleanUp();
        _CameraController.CleanUp();
    }


    public void Startup()
    {
        _CameraController.Startup(_camPos);
        _MovementController.Startup();
        _headBlobber.Startup(_camPos);
        _flashLight.gameObject.SetActive(_isFlashLightTurnedOn);
        Debug.Log($"[{GetType().Name}] startup complete");

    }

    private void Update()
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay)
        {
            _moveInput = Vector2.zero;
            _lookInput = Vector2.zero;
            _MovementController.ReciveInputAndConvertIntoMoveDir(_moveInput, _CameraController.GetCameraTransform());
            _SoundController.PlayFootStepsSound(_MovementController.GetIsMovingOnTheGround(), _MovementController.IsSprinting);
            return;
        }

        _moveInput = _inputService.GetMoveInputClamped();
        _lookInput = _inputService.GetLookInput();
        _MovementController.ReciveInputAndConvertIntoMoveDir(_moveInput, _CameraController.GetCameraTransform());
        _InteractController.RayCastInteract(_CameraController.GetCameraTransform());
        _CameraController.SelectCorrectFOV(_isZooming, _MovementController.IsSprinting);
        _CameraController.AdjustFov();
        _SoundController.PlayFootStepsSound(_MovementController.GetIsMovingOnTheGround(), _MovementController.IsSprinting);
    }

    private void LateUpdate()
    {
        _CameraController.UpdateRotation(_lookInput);
        _CameraController.UpdatePosition(_camPos, _MovementController.IsCrouching);
        _headBlobber.Blob(_MovementController.GetIsMovingOnTheGround());
    }

    private void FixedUpdate()
    {
        _MovementController.UpdateState();                                  // 1. Обновляем состояние (берем скорость, сбрасываем фазы прыжка на земле)
        _MovementController.CheckFallSpeed();                               // 2. Считаем скорость падения в воздухе
        _MovementController.CheckCanStandUpFromCrouch();                    // 3. Автоматический подъем из приседа, если игрок отпустил кнопку, но над головой был потолок
        _MovementController.AdjustVelocity();                               // 4. Корректируем скорость с учетом инпута и уклонов плоскости
        _MovementController.Jump();                                         // 5. Реализуем прыжок
        _MovementController.ApplyRbVelocity();                              // 6. Применяем посчитанную скорость в Rigidbody
        _MovementController.Rotate(_CameraController.GetCameraTransform()); // 7. Разворачиваем тело игрока по направлению камеры
        _MovementController.ClearState();                                   // 8. Очищаем контакты для следующего кадра физики
    }

    private void OnCollisionEnter(Collision collision)
    {
        _MovementController.ProcessOnCollisionEnter(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        _MovementController.ProcessOnCollisionStay(collision);
    }



    private void HandlePlayerDead()
    {
        _moveInput = Vector2.zero;
        _lookInput = Vector2.zero;
        _CameraController.DeactivateHandsCamera();
        _SoundController.PlayDead();
        _camPos.position = new Vector3(_camPos.position.x, 0.2f, _camPos.position.z);
        _MovementController.ReciveInputAndConvertIntoMoveDir(_moveInput, _CameraController.GetCameraTransform());
        _CameraController.UpdateRotation(_lookInput);
    }


    private void HandleInteract()
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay) return;
        _InteractController.Interact();
        _SoundController.PlayInteract();
    }

    private void Jump()
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay) return;
        _MovementController.HandleJumpInput();
    }


    private void Sprint()
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay) return;
        _MovementController.Sprint();

        if (_MovementController.IsSprinting) _SoundController.PlaySprint();
    }

    private void CancelSprint()
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay) return;
        _MovementController.CancelSprint();
    }

    private void HandleZoom(bool isActive)
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay) return;
        _isZooming = isActive;
    }

    private void ApplyFallDamage(float fallDamage)
    {
        _playerService.TakeDamage(fallDamage);
        _SoundController.PlayFallDamage();
    }


    private void StartCrouch()
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay) return;
        _MovementController.SetCrouchState(true);
    }

    private void StopCrouch()
    {
        if (_gameStateService.CurrentState != GameStates.Gameplay) return;
        _MovementController.SetCrouchState(false);
    }


    private void HandleFlashlight()
    {
        _isFlashLightTurnedOn = !_isFlashLightTurnedOn;
        _flashLight.gameObject.SetActive(_isFlashLightTurnedOn);
        _SoundController.PlayFlashlight();
    }

}
