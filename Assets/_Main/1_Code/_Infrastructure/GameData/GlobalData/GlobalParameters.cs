using System.Collections.Generic;

public class GlobalParameters : GameParametersData
{
    public IGameParameter<float> CameraSensitivity => _cameraSensitivity;
    public IGameParameter<float> CameraCrouchOffset => _cameraCrouchCameraOffset;
    public IGameParameter<float> CameraRotationSmoothTime => _cameraRotationSmoothTime;
    public IGameParameter<float> CameraCrouchSmoothSpeed => _cameraCrouchSmoothSpeed;
    public IGameParameter<float> CameraMinVerticalAngle => _cameraMinVerticalAngle;
    public IGameParameter<float> CameraMaxVerticalAngle => _cameraMaxVerticalAngle;
    public IGameParameter<float> CameraDefaultFov => _cameraDefaultFov;
    public IGameParameter<float> CameraZoomFov => _cameraZoomFov;
    public IGameParameter<float> CameraSprintFov => _cameraSprintFov;
    public IGameParameter<float> CameraFovChangeSmoothSpeed => _cameraFovChangeSmoothSpeed;

    // Инициализируем каждый параметр: ("имя", дефолт, мин, макс)
    private readonly GameParameterFloat _cameraSensitivity = new("camera_sensitivity", 20.0f, 1f, 100f);
    private readonly GameParameterFloat _cameraCrouchCameraOffset = new("camera_crouchcameraoffset", 1.0f, 0.1f, 1.8f);
    private readonly GameParameterFloat _cameraRotationSmoothTime = new("camera_rotationsmoothtime", 0.075f, 0f, 2f);
    private readonly GameParameterFloat _cameraCrouchSmoothSpeed = new("camera_crouchsmoothspeed", 10.0f, 0f, 100f);
    private readonly GameParameterFloat _cameraMinVerticalAngle = new("camera_minverticalangle", -70.0f, -85f, -50f);
    private readonly GameParameterFloat _cameraMaxVerticalAngle = new("camera_maxverticalangle", 80.0f, 60f, 89f);
    private readonly GameParameterFloat _cameraDefaultFov = new("camera_defaultfov", 70.0f, 60f, 150f);
    private readonly GameParameterFloat _cameraZoomFov = new("camera_zoomfov", 20.0f, 10f, 50f); // БАГФИКС: Поменял местами мин (10) и макс (50), в старом конфиге было наоборот
    private readonly GameParameterFloat _cameraSprintFov = new("camera_sprintfov", 90.0f, 90f, 150f);
    private readonly GameParameterFloat _cameraFovChangeSmoothSpeed = new("camera_fovchangesmoothspeed", 1.5f, 0.1f, 10f);

    public GlobalParameters()
    {
        DisplayName = "GlobalData";

        // Наполняем базовый словарь Parameters для работы консоли разработчика
        Parameters = new Dictionary<string, IGameParameter>
        {
            { _cameraSensitivity.Name, _cameraSensitivity },
            { _cameraCrouchCameraOffset.Name, _cameraCrouchCameraOffset },
            { _cameraRotationSmoothTime.Name, _cameraRotationSmoothTime },
            { _cameraCrouchSmoothSpeed.Name, _cameraCrouchSmoothSpeed },
            { _cameraMinVerticalAngle.Name, _cameraMinVerticalAngle },
            { _cameraMaxVerticalAngle.Name, _cameraMaxVerticalAngle },
            { _cameraDefaultFov.Name, _cameraDefaultFov },
            { _cameraZoomFov.Name, _cameraZoomFov },
            { _cameraSprintFov.Name, _cameraSprintFov },
            { _cameraFovChangeSmoothSpeed.Name, _cameraFovChangeSmoothSpeed }
        };
    }

}
