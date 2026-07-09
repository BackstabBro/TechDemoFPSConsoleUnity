using UnityEngine;
using VContainer;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _handsCamera;

    private Vector3 _eulerAngles;
    private Vector2 _currentLookInput;
    private Vector2 _lookInputVelocity;
    private float _currentCamYOffset;
    private float _targetCamYOffset;
    private float _currentFov;

    private GlobalParameters _globalData;
    private IGameParameter<float> _sensitivityParam;
    private IGameParameter<float> _crouchOffsetParam;
    private IGameParameter<float> _rotationSmoothTimeParam;
    private IGameParameter<float> _crouchSmoothSpeedParam;
    private IGameParameter<float> _minVerticalAngleParam;
    private IGameParameter<float> _maxVerticalAngleParam;
    private IGameParameter<float> _defaultFOVParam;
    private IGameParameter<float> _zoomFOVParam;
    private IGameParameter<float> _sprintFOVParam;
    private IGameParameter<float> _fovChangeSmoothSpeedParam;

    [Inject]
    public void Construct(GlobalParameters gameData)
    {
        _globalData = gameData;
        _sensitivityParam = _globalData.CameraSensitivity;
        _crouchOffsetParam = _globalData.CameraCrouchOffset;
        _rotationSmoothTimeParam = _globalData.CameraRotationSmoothTime;
        _crouchSmoothSpeedParam = _globalData.CameraCrouchSmoothSpeed;
        _minVerticalAngleParam = _globalData.CameraMinVerticalAngle;
        _maxVerticalAngleParam = _globalData.CameraMaxVerticalAngle;
        _defaultFOVParam = _globalData.CameraDefaultFov;
        _zoomFOVParam = _globalData.CameraZoomFov;
        _sprintFOVParam = _globalData.CameraSprintFov;
        _fovChangeSmoothSpeedParam = _globalData.CameraFovChangeSmoothSpeed;
    }

    public void Init()
    {
        _defaultFOVParam.OnChanged += HandleFovParameterChanged;
        _zoomFOVParam.OnChanged += HandleFovParameterChanged;
        _sprintFOVParam.OnChanged += HandleFovParameterChanged;
    }

    public void CleanUp()
    {
        if (_defaultFOVParam != null) _defaultFOVParam.OnChanged -= HandleFovParameterChanged;
        if (_zoomFOVParam != null) _zoomFOVParam.OnChanged -= HandleFovParameterChanged;
        if (_sprintFOVParam != null) _sprintFOVParam.OnChanged -= HandleFovParameterChanged;
    }

    public void Startup(Transform target)
    {
        _currentFov = _defaultFOVParam.Value;
        _cameraTransform.position = target.position;
        _cameraTransform.eulerAngles = _eulerAngles = target.eulerAngles;
    }


    public void UpdateRotation(Vector2 input)
    {
        // Плавно сглаживаем значения мыши, как это делает Cinemachine
        _currentLookInput.x = Mathf.SmoothDamp(_currentLookInput.x, input.x, ref _lookInputVelocity.x, _rotationSmoothTimeParam.Value);
        _currentLookInput.y = Mathf.SmoothDamp(_currentLookInput.y, input.y, ref _lookInputVelocity.y, _rotationSmoothTimeParam.Value);

        _eulerAngles.y += _currentLookInput.x * _sensitivityParam.Value * Time.deltaTime;
        _eulerAngles.x -= _currentLookInput.y * _sensitivityParam.Value * Time.deltaTime;
        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, _minVerticalAngleParam.Value, _maxVerticalAngleParam.Value);
        _cameraTransform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition(Transform target, bool isCrouching)
    {
        _targetCamYOffset = isCrouching ? _crouchOffsetParam.Value : 0f;
        _currentCamYOffset = Mathf.Lerp(_currentCamYOffset, _targetCamYOffset, Time.deltaTime * _crouchSmoothSpeedParam.Value);
        Vector3 finalPosition = target.position - new Vector3(0f, _currentCamYOffset, 0f);
        _cameraTransform.position = finalPosition;
    }

    public Transform GetCameraTransform() { return _cameraTransform; }

    public void SelectCorrectFOV(bool isZooming, bool isSprinting)
    {
        if (isZooming)
        {
            _currentFov = _zoomFOVParam.Value;
            return;
        }
        if (isSprinting)
        {
            _currentFov = _sprintFOVParam.Value;
            return;
        }
        _currentFov = _defaultFOVParam.Value;
    }

    public void AdjustFov()
    {
        float targetFOV = _currentFov;

        if (Mathf.Abs(_mainCamera.fieldOfView - targetFOV) < 0.01f && Mathf.Abs(_handsCamera.fieldOfView - targetFOV) < 0.01f)
        {
            _mainCamera.fieldOfView = targetFOV;
            _handsCamera.fieldOfView = targetFOV;
            return;
        }
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, Time.deltaTime * _fovChangeSmoothSpeedParam.Value);
        _handsCamera.fieldOfView = Mathf.Lerp(_handsCamera.fieldOfView, targetFOV, Time.deltaTime * _fovChangeSmoothSpeedParam.Value);
    }

    public void DeactivateHandsCamera() { _handsCamera.transform.gameObject.SetActive(false); }

    // Метод отслеживания изменения FOV из консоли в реальном времени
    private void HandleFovParameterChanged(IGameParameter param)
    {
        _currentFov = _defaultFOVParam.Value;
    }


    public void OnDestroy()
    {

    }
}