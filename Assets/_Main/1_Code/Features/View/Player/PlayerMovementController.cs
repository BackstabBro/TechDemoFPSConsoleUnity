using System;
using UnityEngine;
using VContainer;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private LayerMask _probeMask = -1;
    [SerializeField] private LayerMask _stairsMask = -1;

    public event Action<float> OnCalculateFallDAmage;
    public bool IsWantToStand { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsWantToSprint { get; private set; }
    public bool IsSprinting { get; private set; }

    private GameData _gameData;

    private IGameParameter<float> _speedParam;
    private IGameParameter<float> _sprintSpeedParam;
    private IGameParameter<float> _crouchSpeedParam;
    private IGameParameter<float> _maxAccelerationParam;
    private IGameParameter<float> _maxAirAccelerationParam;
    private IGameParameter<float> _jumpHeightParam;
    private IGameParameter<float> _crouchHeightParam;
    private IGameParameter<int> _maxAirJumpsParam;
    private IGameParameter<float> _maxGroundAngleParam;
    private IGameParameter<float> _maxStairsAngleParam;
    private IGameParameter<float> _maxSnapSpeedParam;
    private IGameParameter<float> _probeDistanceParam;
    private IGameParameter<float> _safeFallSpeedParam;
    private IGameParameter<float> _damageMultiplierParam;
    private IGameParameter<bool> _noclipParam;

    private Vector2 _moveInput;
    private Vector3 _moveDir;
    private Vector3 _velocity;
    private float _currentMaxSpeed;
    private float _defaultHeight;
    private bool _toggleNoclip = false;
    private bool _isJumpRequested;
    private int _groundContactCount;
    private int _jumpPhase;
    private float _minGroundDotProduct;
    private float _minStairsDotProduct;
    private Vector3 _contactNormal;
    private float _fallSpeed;
    private int _stepsSinceLastGrounded;
    private int _stepsSinceLastJump;
    private int _wallContactCount;
    private Vector3 _wallNormal;

    private Vector3 _cameraForward;
    private Vector3 _cameraRight;
    private Vector3 _forwardRelativeVerticalInput;
    private Vector3 _rightRelativeHorizontalInput;
    private Vector3 _cameraRelativeMovement;
    private bool OnGround => _groundContactCount > 0;

    [Inject]
    public void Construct(GameData gameData)
    {
        _gameData = gameData;

        PlayerParameters playerData = _gameData.PlayerParameters;

        _speedParam = playerData.Speed;
        _sprintSpeedParam = playerData.SprintSpeed;
        _crouchSpeedParam = playerData.CrouchSpeed;
        _maxAccelerationParam = playerData.MaxAcceleration;
        _maxAirAccelerationParam = playerData.MaxAirAcceleration;
        _jumpHeightParam = playerData.JumpHeight;
        _crouchHeightParam = playerData.CrouchHeight;
        _maxAirJumpsParam = playerData.MaxAirJumps;
        _maxGroundAngleParam = playerData.MaxGroundAngle;
        _maxStairsAngleParam = playerData.MaxStairsAngle;
        _maxSnapSpeedParam = playerData.MaxSnapSpeed;
        _probeDistanceParam = playerData.ProbeDistance;
        _safeFallSpeedParam = playerData.SafeFallSpeed;
        _damageMultiplierParam = playerData.FallDamageMultiplier;
        _noclipParam = playerData.NoClip;
    }


    public void Init()
    {
        _noclipParam.OnChanged += HandleNoclipParameterChanged;

        // Подписки на динамическое изменение углов
        _maxGroundAngleParam.OnChanged += HandleGroundAngleChanged;
        _maxStairsAngleParam.OnChanged += HandleStairsAngleChanged;

        // Подписки на динамическое изменение скоростей
        _speedParam.OnChanged += HandleSpeedParametersChanged;
        _sprintSpeedParam.OnChanged += HandleSpeedParametersChanged;
        _crouchSpeedParam.OnChanged += HandleSpeedParametersChanged;

        Debug.Log($"[{GetType().Name}] init complete");
    }




    public void Startup()
    {
        RecalculateGroundDot();
        RecalculateStairsDot();
        UpdateCurrentMaxSpeed();

        if (_capsuleCollider == null) _capsuleCollider = GetComponent<CapsuleCollider>();
        _defaultHeight = _capsuleCollider.height;
    }


    public void CleanUp()
    {
        _noclipParam.OnChanged -= HandleNoclipParameterChanged;

        _maxGroundAngleParam.OnChanged -= HandleGroundAngleChanged;
        _maxStairsAngleParam.OnChanged -= HandleStairsAngleChanged;

        _speedParam.OnChanged -= HandleSpeedParametersChanged;
        _sprintSpeedParam.OnChanged -= HandleSpeedParametersChanged;
        _crouchSpeedParam.OnChanged -= HandleSpeedParametersChanged;
    }

    private void HandleGroundAngleChanged(IGameParameter param) => RecalculateGroundDot();
    private void HandleStairsAngleChanged(IGameParameter param) => RecalculateStairsDot();
    private void HandleSpeedParametersChanged(IGameParameter param) => UpdateCurrentMaxSpeed();

    private void RecalculateGroundDot()
    {
        _minGroundDotProduct = Mathf.Cos(_maxGroundAngleParam.Value * Mathf.Deg2Rad);
    }

    private void RecalculateStairsDot()
    {
        _minStairsDotProduct = Mathf.Cos(_maxStairsAngleParam.Value * Mathf.Deg2Rad);
    }

    // Этот метод автоматически пересчитает текущую скорость в зависимости от того, что делает игрок в данный момент
    private void UpdateCurrentMaxSpeed()
    {
        if (IsCrouching)
        {
            _currentMaxSpeed = _crouchSpeedParam.Value;
        }
        else if (IsSprinting)
        {
            _currentMaxSpeed = _sprintSpeedParam.Value;
        }
        else
        {
            _currentMaxSpeed = _speedParam.Value;
        }
    }



    public void ReciveInputAndConvertIntoMoveDir(Vector2 moveInput, Transform cam)
    {
        _moveInput = moveInput;
        _moveDir = GetCameraRelativeMoveDirection(cam);
        _moveDir = Vector3.ClampMagnitude(_moveDir, 1f);
    }


    public void ApplyRbVelocity()
    {
        if (_toggleNoclip)
        {
            Vector3 nextPosition = _rigidbody.position + _velocity * Time.deltaTime;
            _rigidbody.MovePosition(nextPosition);
        }
        else
        {
            _rigidbody.linearVelocity = _velocity;
        }
    }





    public void CheckFallSpeed()
    {
        if (!OnGround && _rigidbody.linearVelocity.y < 0f)
        {
            _fallSpeed = Mathf.Abs(_rigidbody.linearVelocity.y);
        }
    }


    public void Rotate(Transform cameraTransform)
    {
        Vector3 cameraAngles = cameraTransform.eulerAngles;
        Quaternion targetRotation = Quaternion.Euler(0f, cameraAngles.y, 0f);
        _rigidbody.MoveRotation(targetRotation);
    }

    public void UpdateState()
    {
        if (_toggleNoclip)
        {
            _velocity = _rigidbody.linearVelocity;
            return;
        }


        _stepsSinceLastGrounded += 1;
        _stepsSinceLastJump += 1;
        _velocity = _rigidbody.linearVelocity;

        if (OnGround || SnapToGround())
        {
            _stepsSinceLastGrounded = 0;
            _jumpPhase = 0;
            _fallSpeed = 0f;
            if (_groundContactCount > 1)
            {
                _contactNormal.Normalize();
            }
        }
        else
        {
            _contactNormal = Vector3.up;
        }
    }






    public void ClearState()
    {
        _groundContactCount = 0;
        _contactNormal = Vector3.zero;

        _wallContactCount = 0;
        _wallNormal = Vector3.zero;
    }
    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - _contactNormal * Vector3.Dot(vector, _contactNormal);
    }

    public void AdjustVelocity()
    {
        if (_toggleNoclip)
        {
            _velocity = _moveDir * _currentMaxSpeed;
            return; 
        }


        if (IsCrouching && !CanStandUp()) _currentMaxSpeed = _crouchSpeedParam.Value;

        Vector3 xAxis = ProjectOnContactPlane(_cameraRight).normalized;
        Vector3 zAxis = ProjectOnContactPlane(_cameraForward).normalized;


        float currentX = Vector3.Dot(_velocity, xAxis);
        float currentZ = Vector3.Dot(_velocity, zAxis);


        float targetX = _moveInput.x * _currentMaxSpeed;
        float targetZ = _moveInput.y * _currentMaxSpeed;

        float acceleration = OnGround ? _maxAccelerationParam.Value : _maxAirAccelerationParam.Value;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, targetX, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, targetZ, maxSpeedChange);

        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        if (_wallContactCount > 0)
        {

            Vector3 wallNormalHorizontal = _wallNormal.normalized;
            wallNormalHorizontal.y = 0f;
            wallNormalHorizontal.Normalize();

            float dotWithWall = Vector3.Dot(_velocity, wallNormalHorizontal);

            if (dotWithWall < 0f)
            {

                _velocity -= wallNormalHorizontal * dotWithWall;
            }
        }
    }





    public void Jump()
    {
        if (_toggleNoclip) return;

        if (_isJumpRequested)
        {
            _isJumpRequested = false;
            if (OnGround || _jumpPhase < _maxAirJumpsParam.Value)
            {
                _stepsSinceLastJump = 0;
                _jumpPhase += 1;
                float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * _jumpHeightParam.Value);
                float alignedSpeed = Vector3.Dot(_rigidbody.linearVelocity, _contactNormal);

                if (alignedSpeed > 0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
                }
                _velocity += _contactNormal * jumpSpeed;
            }
        }
    }




    public void ProcessOnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
        CalculateFallDamage(collision);
    }



    public void ProcessOnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        float minDot = GetMinDot(collision.gameObject.layer);

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minDot)
            {
                _groundContactCount += 1;
                _contactNormal += normal;
            }
            else if (normal.y > -0.01f)
            {
                _wallContactCount += 1;
                _wallNormal += normal;
            }
        }
    }


    private bool SnapToGround()
    {
        if (_stepsSinceLastGrounded > 1 || _stepsSinceLastJump <= 2)
        {
            return false;
        }
        float speed = _velocity.magnitude;
        if (speed > _maxSnapSpeedParam.Value)
        {
            return false;
        }
        if (!Physics.Raycast(_rigidbody.position, Vector3.down, out RaycastHit hit, _probeDistanceParam.Value, _probeMask))
        {
            return false;
        }
        if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }
        _groundContactCount = 1;
        _contactNormal = hit.normal;
        float dot = Vector3.Dot(_velocity, hit.normal);
        if (dot > 0f)
        {
            _velocity = (_velocity - hit.normal * dot).normalized * speed;
        }
        return true;
    }


    public void HandleJumpInput()
    {
        _isJumpRequested = true;
    }

    private float GetMinDot(int layer)
    {
        return (_stairsMask & (1 << layer)) == 0 ? _minGroundDotProduct : _minStairsDotProduct;
    }




    private void CalculateFallDamage(Collision collision)
    {
        bool hitGround = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= _minGroundDotProduct)
            {
                hitGround = true;
                break;
            }
        }

        if (hitGround && _fallSpeed > _safeFallSpeedParam.Value)
        {
            float excessSpeed = _fallSpeed - _safeFallSpeedParam.Value;
            float damage = excessSpeed * _damageMultiplierParam.Value;

            OnCalculateFallDAmage?.Invoke(Mathf.Round(damage));
            _fallSpeed = 0f;
        }
    }









    private Vector3 GetCameraRelativeMoveDirection(Transform camera)
    {
        _cameraForward = camera.forward; //World space
        _cameraRight = camera.right; //World space

        if (!_toggleNoclip)
        {
            _cameraForward.y = 0f;
            _cameraRight.y = 0f;
        }

        _cameraForward = _cameraForward.normalized;
        _cameraRight = _cameraRight.normalized;

        _forwardRelativeVerticalInput = _moveInput.y * _cameraForward;
        _rightRelativeHorizontalInput = _moveInput.x * _cameraRight;

        _cameraRelativeMovement = _forwardRelativeVerticalInput + _rightRelativeHorizontalInput;
        return _cameraRelativeMovement;
    }






    public void Sprint()
    {
        IsWantToSprint = true;

        if (IsCrouching) return;

        _currentMaxSpeed = _sprintSpeedParam.Value;

        //reset crouch state
        IsSprinting = true;
        IsCrouching = false;
        IsWantToStand = false;

        _capsuleCollider.height = _defaultHeight;
        _capsuleCollider.center = new Vector3(0f, _defaultHeight / 2f, 0f);

    }

    public void CancelSprint()
    {
        IsWantToSprint = false;

        if (IsCrouching) return;
        _currentMaxSpeed = _speedParam.Value;
        IsSprinting = false;
    }


    public bool GetIsMovingOnTheGround()
    {
        return _moveInput != Vector2.zero && OnGround && _velocity.sqrMagnitude > 0.01f;
    }


    public void SetCrouchState(bool startCrouch)
    {
        if (startCrouch)
        {
            IsCrouching = true;
            IsSprinting = false;
            IsWantToStand = false;
            _currentMaxSpeed = _crouchSpeedParam.Value;
            _capsuleCollider.height = _crouchHeightParam.Value;
            _capsuleCollider.center = new Vector3(0f, _crouchHeightParam.Value / 2f, 0f);
        }
        else
        {
            if (!CanStandUp())
            {
                IsWantToStand = true;
                IsSprinting = false;
                return;
            }

            // ВЫХОД ИЗ ПРИСЕДА
            IsCrouching = false;
            IsWantToStand = false;

            // ПРОВЕРКА: если кнопка спринта всё еще зажата — сразу включаем бег
            IsSprinting = IsWantToSprint;
            _currentMaxSpeed = IsWantToSprint ? _sprintSpeedParam.Value : _speedParam.Value;

            _capsuleCollider.height = _defaultHeight;
            _capsuleCollider.center = new Vector3(0f, _defaultHeight / 2f, 0f);
        }
    }

    public void CheckCanStandUpFromCrouch()
    {
        if (IsCrouching && IsWantToStand && CanStandUp())
        {
            IsCrouching = false;
            IsWantToStand = false;

            IsSprinting = IsWantToSprint;
            _currentMaxSpeed = IsWantToSprint ? _sprintSpeedParam.Value : _speedParam.Value;

            _capsuleCollider.height = _defaultHeight;
            _capsuleCollider.center = new Vector3(0f, _defaultHeight / 2f, 0f);
        }
    }

    private bool CanStandUp()
    {
        float radius = _capsuleCollider.radius;
        Vector3 headPosition = transform.position + Vector3.up * _crouchHeightParam.Value;

        float checkDistance = _defaultHeight - _crouchHeightParam.Value - radius;
        Vector3 checkCenter = headPosition + Vector3.up * (checkDistance + radius - 0.05f);

        int layerMask = ~LayerMask.GetMask("Player");

        bool isBlocked = Physics.CheckSphere(checkCenter, radius, layerMask, QueryTriggerInteraction.Ignore);
        return !isBlocked;
    }


    private void HandleNoclipParameterChanged(IGameParameter param)
    {
        bool noclipActive = _noclipParam.Value;
        if (noclipActive != _toggleNoclip)
        {
            Noclip();
        }
    }



    //console commands

    public void Noclip()
    {
        _toggleNoclip = !_toggleNoclip;

        _capsuleCollider.enabled = !_toggleNoclip;

        if (_toggleNoclip)
        {
            _velocity = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
            _fallSpeed = 0f;

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;

            Debug.Log("[MovementController] Noclip активирован.");
        }
        else
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;

            _stepsSinceLastGrounded = 5;

            Debug.Log("[MovementController] Noclip деактивирован.");
        }

    }

}
