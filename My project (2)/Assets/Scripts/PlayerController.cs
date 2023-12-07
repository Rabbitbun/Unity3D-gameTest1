using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform _playerTransform;
    private Animator _animator;
    private Transform _cameraTransform;
    private CharacterController _characterController;
    Vector2 mouseXY;

    //[SerializeField] public MasterManager masterManager;
    private PlayerInputManager playerInput;
    private MasterManager masterManagerInstance;

    Vector3 playerMovement = Vector3.zero;

    public GameObject CinemachineCameraTarget;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float _cinemachineTargetYawTemp;
    private float _cinemachineTargetPitchTemp;

    #region 玩家姿態、動畫臨界點參數
    public enum PlayerPosture
    {
        Crouch,
        Stand,
        Jumping,
        Falling,
        Landing,
    };
    public PlayerPosture playerPosture = PlayerPosture.Stand;

    private float _crouchThreshold = 0f;
    private float _standThreshold = 1f;
    private float _midairThreshold = 2.1f;
    private float _landingThreshold = 1f;
    #endregion

    #region 玩家運動狀態
    public enum LocomotionState
    {
        Idle,
        Walk,
        Run,
    };
    public LocomotionState locomotionState = LocomotionState.Idle;
    #endregion

    #region 玩家裝備狀態
    public enum ArmState
    {
        Normal,
        Aim,
    };
    public ArmState armState = ArmState.Normal;
    #endregion

    #region 不同狀態的運動速度
    private float _crouchSpeed = 1.5f;
    private float _walkSpeed = 3f;
    private float _runSpeed = 6f;
    #endregion

    #region 輸入值
    private Vector2 _moveInput;
    private bool isCrouch;
    private bool isRunning;
    private bool isAiming;
    private bool isJumping;
    #endregion

    #region 狀態機參數的Hash
    private int _postureHash;
    private int _moveSpeedHash;
    private int _rotateSpeedHash;
    private int _verticalVelHash;
    private int _feetTweenHash;
    #endregion

    #region 一些變數
    //重力
    private float _gravity = -13f;

    //垂直速度
    private float _VerticalVelocity;

    //下落時加速度的倍速
    private float _fallMultiplier = 1.5f;

    //跳躍最大高度
    private float _maxHeight = 1.5f;

    //跳躍冷卻
    private float _JumpCD = 0.15f;

    //是否處於跳躍CD
    private bool isLanding;

    //可否跌落
    private bool couldFall;

    //可跌落的最小高度，小於此高度不會切換到跌落姿態
    private float _fallHeight = 0.5f;

    //切換左右腳狀態
    private float _feetTween;

    //是否著地
    private bool isGrounded;

    //射線檢測偏移量
    private float _groundCheckOffset = 0.5f;
    #endregion

    #region 平均速度緩存池
    static readonly int CACHE_SIZE = 3;
    Vector3[] _valCache = new Vector3[CACHE_SIZE];
    int _currentCacheIndex = 0;
    Vector3 _averageVel = Vector3.zero;
    #endregion

    public Vector3 playerDeltaMovement = Vector3.zero;
    void Start()
    {
        _playerTransform = this.transform;
        _animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;
        _characterController = GetComponent<CharacterController>();

        masterManagerInstance = MasterManager.Instance;
        playerInput = masterManagerInstance.PlayerInputManager;

        _postureHash = Animator.StringToHash("Posture");
        _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        _rotateSpeedHash = Animator.StringToHash("RotateSpeed");
        _verticalVelHash = Animator.StringToHash("VerticalSpeed");
        _feetTweenHash = Animator.StringToHash("LRFoot");

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 50f, Color.blue);
        DealWithInput();
        CheckGround();
        SwitchPlayerStates();
        CaculateGravity();
        Jump();
        CaculateInputDirection();
        SetupAnimator();
    }
    
    void LateUpdate()
    {
        if (MasterManager.Instance.GameEventManager.IsgamePaused == false)
            CameraRotation();

        if (PlayerInputManager.Instance.leftClick)
        {
            print("Left clicked " + PlayerInputManager.Instance.leftClick);
            _animator.SetTrigger("Attack");
        }
        else
        {
            _animator.ResetTrigger("Attack");
        }
            
    }
    
    // 讀取輸入
    void DealWithInput()
    {
        _moveInput = playerInput.move;
        isRunning = playerInput.run;
        isCrouch = playerInput.crouch;
        isJumping = playerInput.jump;
        mouseXY = playerInput.look;
        isAiming = playerInput.rightClick;
    }
 
    private void CameraRotation()
    {
        if (armState == ArmState.Normal)
        {
            _cinemachineTargetYaw += mouseXY.x * 0.3f;
            _cinemachineTargetPitch += mouseXY.y * 0.3f;
        }
        else 
        {
            _cinemachineTargetYaw += mouseXY.x * 0.2f;
            _cinemachineTargetPitch += mouseXY.y * 0.2f;
        }
        // clamp our rotations so our values are limited 360 degrees
        if (armState == ArmState.Normal)
        {
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -55f, 20f);
        }
        else if (armState == ArmState.Aim)
        {
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -45f, 30f);
        }


        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(-_cinemachineTargetPitch,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    void Jump()
    {
        if(playerPosture == PlayerPosture.Stand && isJumping)
        {
            _VerticalVelocity = Mathf.Sqrt(-2 * _gravity * _maxHeight);
            _feetTween = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1);
            _feetTween = _feetTween < 0.5f ? 1 : -1;

            if(locomotionState == LocomotionState.Run)
            {
                _feetTween *= 3;
            }
            else if(locomotionState == LocomotionState.Walk)
            {
                _feetTween *= 2;
            }
            else
            {
                _feetTween = Random.Range(0.5f, 1f) * _feetTween;
            }
        }
    }

    IEnumerator CoolDownJump()
    {
        _landingThreshold = Mathf.Clamp(_VerticalVelocity, -10, 0);
        _landingThreshold /= 20f;
        _landingThreshold += 1f;
        isLanding = true;
        playerPosture = PlayerPosture.Landing;
        yield return new WaitForSeconds(_JumpCD);
        isLanding = false;
    }

    void CheckGround()
    {
        if(Physics.SphereCast(_playerTransform.position + (Vector3.up * _groundCheckOffset), 
                              _characterController.radius, 
                              Vector3.down, 
                              out RaycastHit hit, 
                              _groundCheckOffset - _characterController.radius + 2 *_characterController.skinWidth))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            couldFall = !Physics.Raycast(_playerTransform.position, Vector3.down, _fallHeight);
        }
    }

    void CaculateGravity()
    {
        if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling)
        {
            if(isGrounded == false)
            {
                _VerticalVelocity += _gravity * _fallMultiplier * Time.deltaTime;
            }
            else
            {
                _VerticalVelocity = _gravity * Time.deltaTime;
            }
        }
        else
        {
            if (_VerticalVelocity <= 0 || isJumping == false)
            {
                _VerticalVelocity += _gravity * _fallMultiplier * Time.deltaTime;
            }
            else
            {
                _VerticalVelocity += _gravity * Time.deltaTime;

            }
        }
    }

    void SwitchPlayerStates()
    {
        switch(playerPosture)
        {
            case PlayerPosture.Stand:
                if(_VerticalVelocity > 0)
                {
                    playerPosture = PlayerPosture.Jumping;
                }
                else if (!isGrounded && couldFall)
                {
                    playerPosture = PlayerPosture.Falling;
                }
                else if (isCrouch)
                {
                    playerPosture = PlayerPosture.Crouch;
                }
                break;

            case PlayerPosture.Crouch:
                if (!isGrounded && couldFall)
                {
                    playerPosture = PlayerPosture.Falling;
                }
                else if (!isCrouch)
                {
                    playerPosture = PlayerPosture.Stand;
                }
                break;

            case PlayerPosture.Falling:
                if (isGrounded)
                {
                    StartCoroutine(CoolDownJump());
                }
                if (isLanding)
                {
                    playerPosture = PlayerPosture.Landing;
                }
                break;

            case PlayerPosture.Jumping:
                if (isGrounded)
                {
                    StartCoroutine(CoolDownJump());
                }
                if (isLanding)
                {
                    playerPosture = PlayerPosture.Landing;
                }
                break;

            case PlayerPosture.Landing:
                if (!isLanding)
                {
                    playerPosture = PlayerPosture.Stand;
                }
                break; 
        }

        if(_moveInput.magnitude == 0)
        {
            locomotionState = LocomotionState.Idle;
        }
        else if(isRunning == false)
        {
            locomotionState = LocomotionState.Walk;
        }
        else
        {
            locomotionState = LocomotionState.Run;
        }

        if(isAiming == true)
        {
            armState = ArmState.Aim;
        }
        else
        {
            armState = ArmState.Normal;
        }
    }

    void CaculateInputDirection()
    {
        Vector3 camForwardProjection = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z).normalized;
        playerMovement = camForwardProjection * _moveInput.y + _cameraTransform.right * _moveInput.x;

        playerMovement = _playerTransform.InverseTransformVector(playerMovement);

        //Vector3 forward = transform.InverseTransformVector(_cameraTransform.forward);
        //Vector3 right = transform.InverseTransformVector(_cameraTransform.right);
        //forward.y = 0f;
        //right.y = 0f;
        //forward = forward.normalized;
        //right = right.normalized;

        //playerMovement = _moveInput.y * forward + _moveInput.x * right;
    }

    void SetupAnimator()
    {
        if(playerPosture == PlayerPosture.Stand)
        {
            _animator.SetFloat(_postureHash, _standThreshold, 0.1f, Time.deltaTime);
            switch(locomotionState)
            {
                case LocomotionState.Idle:
                    _animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    _animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    _animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        else if(playerPosture == PlayerPosture.Crouch)
        {
            _animator.SetFloat(_postureHash, _crouchThreshold, 0.1f, Time.deltaTime);
            switch(locomotionState)
            {
                case LocomotionState.Idle:
                    _animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    _animator.SetFloat(_moveSpeedHash, _crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }

        else if(playerPosture == PlayerPosture.Jumping)
        {
            _animator.SetFloat(_postureHash, _midairThreshold);
            _animator.SetFloat(_verticalVelHash, _VerticalVelocity);
            _animator.SetFloat(_feetTweenHash, _feetTween);
        }
        else if(playerPosture == PlayerPosture.Landing)
        {
            _animator.SetFloat(_postureHash, _landingThreshold, 0.03f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    _animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    _animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    _animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }

        else if (playerPosture == PlayerPosture.Falling)
        {
            _animator.SetFloat(_postureHash, _midairThreshold);
            _animator.SetFloat(_verticalVelHash, _VerticalVelocity);
        }

        if (armState == ArmState.Normal)
        {
            _animator.SetBool("Aim", false);
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            _animator.SetFloat(_rotateSpeedHash, rad, 0.1f, Time.deltaTime * 2.0f);
            _playerTransform.Rotate(0f, rad * 300 * Time.deltaTime * 2.0f, 0f);
        }
        else if(armState == ArmState.Aim)
        {
            _animator.SetBool("Aim", true);
            _animator.SetFloat(_rotateSpeedHash, 0f);
        }
    }

    Vector3 AverageVel(Vector3 newVel)
    {
        _valCache[_currentCacheIndex] = newVel;
        _currentCacheIndex++;
        _currentCacheIndex %= CACHE_SIZE;
        Vector3 average = Vector3.zero;
        foreach(Vector3 vel in _valCache)
        {
            average += vel;
        }
        return average / CACHE_SIZE;
    }

    private void OnAnimatorMove()
    {
        /*
        if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling)
        {
            Vector3 playerDeltaMovement = animator.deltaPosition;
            playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;
            characterController.Move(playerDeltaMovement);
            averageVel = AverageVel(animator.velocity);
        }
        else
        {
            averageVel.y = VerticalVelocity;
            Vector3 playerDeltaMovement = averageVel * Time.deltaTime;
            characterController.Move(playerDeltaMovement);
        }
        */
        
        

        if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling && armState != ArmState.Aim)
        {
            playerDeltaMovement = _animator.deltaPosition;
            //playerDeltaMovement = playerTransform.TransformVector(playerMovement) * Time.deltaTime;
            //print("animator delta: " + playerDeltaMovement);
            playerDeltaMovement.y = _VerticalVelocity * Time.deltaTime;
            _averageVel = AverageVel(_animator.velocity);
        }
        else
        {
            playerDeltaMovement = _playerTransform.TransformVector(playerMovement) * Time.deltaTime;
            _averageVel.y = _VerticalVelocity;
            playerDeltaMovement += _averageVel * Time.deltaTime;
            if(armState == ArmState.Aim)
            {
                playerDeltaMovement.x *= _animator.GetFloat(_moveSpeedHash) / 2.5f;
                playerDeltaMovement.z *= _animator.GetFloat(_moveSpeedHash) / 2.5f;
            }
        }

        if (armState == ArmState.Aim)
        {
            _averageVel.x = 0;
            _averageVel.z = 0;
        }
        _characterController.Move(playerDeltaMovement);

    }
    
}
