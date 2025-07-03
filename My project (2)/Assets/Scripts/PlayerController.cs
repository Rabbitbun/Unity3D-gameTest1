using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader = default;

    public Transform PlayerTransform;
    public Animator animator;
    private Transform _cameraTransform;
    public CharacterController CharacterController;
    public Vector2 mouseXY;

    private PlayerInputManager playerInput;
    private MasterManager masterManagerInstance;

    public Vector3 playerMovement = Vector3.zero;

    public GameObject CinemachineCameraTarget;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

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

    public float CrouchThreshold = 0f;
    public float StandThreshold = 1f;
    public float MidairThreshold = 2.1f;
    public float LandingThreshold = 1f;
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

    #region 玩家特殊狀態
    public enum ArmState
    {
        Normal,
        Aim,
        Attack,
        Cast,
    };
    public ArmState armState = ArmState.Normal;
    #endregion

    #region 不同狀態的運動速度
    public float _crouchSpeed = 1.5f;
    public float _walkSpeed = 3f;
    public float _runSpeed = 6f;
    #endregion

    #region 輸入相關
    public Vector2 MoveInput;
    public bool IsCrouch;
    public bool IsRunning;
    public bool IsAiming;
    public bool IsJumping;
    public bool IsAttacking;
    public bool IsChanting;
    public bool IsLockCamera;
    #endregion

    #region 狀態機參數的Hash
    public int _postureHash;
    public int _moveSpeedHash;
    public int _rotateSpeedHash;
    public int _verticalVelHash;
    public int _feetTweenHash;
    #endregion

    #region 一些變數
    //重力
    public float Gravity = -13f;

    //垂直速度
    public float VerticalVelocity;

    //下落時加速度的倍速
    public float FallMultiplier = 1.5f;

    //跳躍最大高度
    public float MaxHeight = 1.5f;

    //跳躍冷卻
    public float JumpCD = 0.15f;

    //是否處於跳躍CD
    public bool IsLanding;

    //可否跌落
    public bool CouldFall;

    //可跌落的最小高度，小於此高度不會切換到跌落姿態
    public float FallHeight = 0.4f;

    //切換左右腳狀態
    public float FeetTween;

    //是否著地
    public bool IsGrounded;

    //射線檢測偏移量
    public float GroundCheckOffset = 0.5f;
    #endregion

    #region 平均速度緩存池
    static readonly int CACHE_SIZE = 3;
    Vector3[] _valCache = new Vector3[CACHE_SIZE];
    int _currentCacheIndex = 0;
    public Vector3 _averageVel = Vector3.zero;
    #endregion

    public Vector3 playerDeltaMovement = Vector3.zero;

    //public Rig rig;

    private bool IsNormalAttacking;

    private void OnEnable()
    {
        _inputReader.moveEvent += OnMove;
        _inputReader.startedRunning += OnStartedRunning;
        _inputReader.stoppedRunning += OnStoppedRunning;
        _inputReader.jumpEvent += OnJump;
        _inputReader.jumpCanceledEvent += OnJumpCanceled;
        _inputReader.cameraMoveEvent += OnMouseXY;

        //_inputReader.startedChanting += OnStartedChanting;
        //_inputReader.stoppedChanting += OnStoppedChanting;
        _inputReader.startedAiming += OnStartedAiming;
        _inputReader.stoppedAiming += OnStoppedAiming;

        _inputReader.crouchEvent += OnCrouch;

        //_inputReader.attackEvent += OnAttack;

        _inputReader.lockCameraEvent += OnLockCamera;
        //...
    }

    private void OnDisable()
    {
        _inputReader.moveEvent -= OnMove;
        _inputReader.startedRunning -= OnStartedRunning;
        _inputReader.stoppedRunning -= OnStoppedRunning;
        _inputReader.jumpEvent -= OnJump;
        _inputReader.jumpCanceledEvent -= OnJumpCanceled;
        _inputReader.cameraMoveEvent -= OnMouseXY;

        //_inputReader.startedChanting -= OnStartedChanting;
        //_inputReader.stoppedChanting -= OnStoppedChanting;
        _inputReader.startedAiming -= OnStartedAiming;
        _inputReader.stoppedAiming -= OnStoppedAiming;

        _inputReader.crouchEvent -= OnCrouch;

        //_inputReader.attackEvent -= OnAttack;

        _inputReader.lockCameraEvent -= OnLockCamera;
        //...
    }

    void Start()
    {
        PlayerTransform = this.transform;
        animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;
        CharacterController = GetComponent<CharacterController>();

        masterManagerInstance = MasterManager.Instance;
        playerInput = masterManagerInstance.PlayerInputManager;

        _postureHash = Animator.StringToHash("Posture");
        _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        _rotateSpeedHash = Animator.StringToHash("RotateSpeed");
        _verticalVelHash = Animator.StringToHash("VerticalSpeed");
        _feetTweenHash = Animator.StringToHash("LRFoot");

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        //rig = GetComponentInChildren<Rig>();
    }

    void Update()
    {
        DealWithInput();
        //CheckGround();
        //SwitchPlayerStates(); //
        //CaculateGravity(); //
        //Jump(); //
        CaculateInputDirection();
        //SetupAnimator(); //
    }
    
    void LateUpdate()
    {
        //if (MasterManager.Instance.GameEventManager.IsgamePaused == false)
            //CameraRotation();

        //switch (IsChanting)
        //{
        //    case true:
        //        rig.weight = Mathf.Lerp(rig.weight, 1.0f, Time.deltaTime * 10.0f);
        //        break;
        //    case false:
        //        rig.weight = Mathf.Lerp(rig.weight, 0.0f, Time.deltaTime * 15.0f);
        //        break;
        //}

        DealWithInputInFixUpdate();
    }

    void DealWithInputInFixUpdate()
    {
        if (MasterManager.Instance.GameEventManager.IsgamePaused)
            return;

        // IsJumping = PlayerInputManager.Instance.jump;
        // IsChanting = PlayerInputManager.Instance.Casting;
        // IsAiming = PlayerInputManager.Instance.Aiming;

        //if (IsAiming && PlayerInputManager.Instance.RightClickPressed)
        //{
        //    PlayerInputManager.Instance.Aiming = false;
        //    PlayerInputManager.Instance.Casting = false;
        //    IsChanting = false;
        //    //print("在FixUpdate()中1:" + IsChanting);
        //}

        // 如果在 Chanting 時做其他動作如普通攻擊等, chanting會被取消, 需重新按下 chanting鍵
        //if (IsAttacking)
        //{
        //    if (IsChanting)
        //    {
        //        IsChanting = false;
        //    }

        //    animator.SetTrigger("Attack1");
        //}

        //if (PlayerInputManager.Instance.leftClick)
        //{
        //    //print("Left clicked " + PlayerInputManager.Instance.leftClick);
        //    if (IsChanting)
        //    {
        //        PlayerInputManager.Instance.Casting = false;
        //        IsChanting = false;
        //    }
        //    if (VerticalVelocity <= 0.0f || IsGrounded)
        //    {
        //        IsAttacking = true;
        //        if (armState == ArmState.Normal)
        //        {
        //            animator.SetTrigger("Attack1");
        //        }
        //        else if (armState == ArmState.Aim)
        //        {
        //            animator.SetTrigger("Attack2");
        //        }
        //        else if (armState == ArmState.Cast)
        //        {
        //            animator.SetTrigger("Attack1");
        //        }
        //    }

        //}

    }
    
    // 讀取輸入
    void DealWithInput()
    {
        if (MasterManager.Instance.GameEventManager.IsgamePaused)
            return;

        /// MoveInput = playerInput.move;
        /// IsRunning = playerInput.run;
        /// IsCrouch = playerInput.crouch;
        //IsJumping = playerInput.jump;
        /// mouseXY = playerInput.look;
        //IsAiming = playerInput.rightClick;

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
        if(playerPosture == PlayerPosture.Stand && IsJumping)
        {
            VerticalVelocity = Mathf.Sqrt(-2 * Gravity * MaxHeight);
            FeetTween = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1);
            FeetTween = FeetTween < 0.5f ? 1 : -1;

            if(locomotionState == LocomotionState.Run)
            {
                FeetTween *= 3;
            }
            else if(locomotionState == LocomotionState.Walk)
            {
                FeetTween *= 2;
            }
            else
            {
                FeetTween = Random.Range(0.5f, 1f) * FeetTween;
            }
        }
    }

    IEnumerator CoolDownJump()
    {
        LandingThreshold = Mathf.Clamp(VerticalVelocity, -10, 0);
        LandingThreshold /= 20f;
        LandingThreshold += 1f;
        IsLanding = true;
        playerPosture = PlayerPosture.Landing;
        yield return new WaitForSeconds(JumpCD);
        
        IsLanding = false;
    }

    public void CalculateJumpCD()
    {
        StartCoroutine(CoolDownJump());
    }

    void CheckGround()
    {
        if(Physics.SphereCast(PlayerTransform.position + (Vector3.up * GroundCheckOffset), 
                              CharacterController.radius, 
                              Vector3.down, 
                              out RaycastHit hit, 
                              GroundCheckOffset - CharacterController.radius + 2 *CharacterController.skinWidth))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
            CouldFall = !Physics.Raycast(PlayerTransform.position, Vector3.down, FallHeight);
        }
    }

    void CaculateGravity()
    {
        if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling)
        {
            if(IsGrounded == false)
            {
                VerticalVelocity += Gravity * FallMultiplier * Time.deltaTime;
            }
            else
            {
                VerticalVelocity = Gravity * Time.deltaTime;
            }
        }
        else
        {
            if (VerticalVelocity <= 0 || IsJumping == false)
            {
                VerticalVelocity += Gravity * FallMultiplier * Time.deltaTime;
            }
            else
            {
                VerticalVelocity += Gravity * Time.deltaTime;

            }
        }
    }

    void SwitchPlayerStates()
    {
        switch (playerPosture)
        {
            case PlayerPosture.Stand:
                if (VerticalVelocity > 0)
                {
                    playerPosture = PlayerPosture.Jumping;
                }
                else if (!IsGrounded && CouldFall)
                {
                    playerPosture = PlayerPosture.Falling;
                }
                else if (IsCrouch)
                {
                    playerPosture = PlayerPosture.Crouch;
                }
                break;

            case PlayerPosture.Crouch:
                if (!IsGrounded && CouldFall)
                {
                    playerPosture = PlayerPosture.Falling;
                }
                else if (!IsCrouch)
                {
                    playerPosture = PlayerPosture.Stand;
                }
                break;

            case PlayerPosture.Falling:
                if (IsGrounded)
                {
                    StartCoroutine(CoolDownJump());
                }
                if (IsLanding)
                {
                    playerPosture = PlayerPosture.Landing;
                }
                break;

            case PlayerPosture.Jumping:
                if (IsGrounded)
                {
                    StartCoroutine(CoolDownJump());
                }
                if (IsLanding)
                {
                    playerPosture = PlayerPosture.Landing;
                }
                break;

            case PlayerPosture.Landing:
                if (!IsLanding)
                {
                    playerPosture = PlayerPosture.Stand;
                }
                break;
        }

        if (MoveInput.magnitude == 0)
        {
            locomotionState = LocomotionState.Idle;
        }
        else if (IsRunning == false)
        {
            locomotionState = LocomotionState.Walk;
        }
        else
        {
            locomotionState = LocomotionState.Run;
        }

        if (IsAiming)
        {
            armState = ArmState.Aim;
        }
        else if (IsAttacking)
        {
            armState = ArmState.Attack;
        }
        else
        {
            armState = ArmState.Normal;
        }
    }

    /// <summary>
    /// 將移動方向轉為以攝影機前方為基準
    /// </summary>
    public void CaculateInputDirection()
    {
        //if (!IsLockCamera)
        //{
        //    Vector3 camForwardProjection = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z).normalized;
        //    playerMovement = camForwardProjection * MoveInput.y + _cameraTransform.right * MoveInput.x;

        //    playerMovement = PlayerTransform.InverseTransformVector(playerMovement);
        //}
        //else
        //{
        //    //var moveDir = new Vector3(MoveInput.x, 0, MoveInput.y);
        //    //playerMovement = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveDir;


        //    //playerMovement = new Vector3(MoveInput.x, 0, MoveInput.y);

        //    //var moveDir = new Vector3(MoveInput.x, 0, MoveInput.y);
        //    //playerMovement = _cameraTransform.rotation * moveDir;

        //    playerMovement = _cameraTransform.forward * MoveInput.y + _cameraTransform.right * MoveInput.x;
        //}

        Vector3 camForwardProjection = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z).normalized;
        playerMovement = camForwardProjection * MoveInput.y + _cameraTransform.right * MoveInput.x;

        playerMovement = PlayerTransform.InverseTransformVector(playerMovement);
    }

    void SetupAnimator()
    {
        if(playerPosture == PlayerPosture.Stand)
        {
            animator.SetFloat(_postureHash, StandThreshold, 0.1f, Time.deltaTime);
            switch(locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        else if(playerPosture == PlayerPosture.Crouch)
        {
            animator.SetFloat(_postureHash, CrouchThreshold, 0.1f, Time.deltaTime);
            switch(locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    animator.SetFloat(_moveSpeedHash, _crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }

        else if(playerPosture == PlayerPosture.Jumping)
        {
            animator.SetFloat(_postureHash, MidairThreshold);
            animator.SetFloat(_verticalVelHash, VerticalVelocity);
            animator.SetFloat(_feetTweenHash, FeetTween);
        }
        else if(playerPosture == PlayerPosture.Landing)
        {
            animator.SetFloat(_postureHash, LandingThreshold, 0.03f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    animator.SetFloat(_moveSpeedHash, playerMovement.magnitude * _runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }

        else if (playerPosture == PlayerPosture.Falling)
        {
            animator.SetFloat(_postureHash, MidairThreshold);
            animator.SetFloat(_verticalVelHash, VerticalVelocity);
        }

        if (armState == ArmState.Normal)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Casting", false);
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            animator.SetFloat(_rotateSpeedHash, rad, 0.1f, Time.deltaTime * 2.0f);
            PlayerTransform.Rotate(0f, rad * 300 * Time.deltaTime * 2.0f, 0f);
        }
        else if (armState == ArmState.Attack)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Casting", false);
        }
        else if (armState == ArmState.Cast)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Casting", true);
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            animator.SetFloat(_rotateSpeedHash, rad, 0.1f, Time.deltaTime * 2.0f);
            PlayerTransform.Rotate(0f, rad * 300 * Time.deltaTime * 2.0f, 0f);
        }
        else if (armState == ArmState.Aim)
        {
            animator.SetBool("Aim", true);
            animator.SetBool("Casting", false);
            animator.SetFloat(_rotateSpeedHash, 0f);
        }
    }

    // 平均速度
    public Vector3 AverageVel(Vector3 newVel)
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
    /*
    private void OnAnimatorMove()
    {

        //if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling)
        //{
        //    Vector3 playerDeltaMovement = animator.deltaPosition;
        //    playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;
        //    characterController.Move(playerDeltaMovement);
        //    averageVel = AverageVel(animator.velocity);
        //}
        //else
        //{
        //    averageVel.y = VerticalVelocity;
        //    Vector3 playerDeltaMovement = averageVel * Time.deltaTime;
        //    characterController.Move(playerDeltaMovement);
        //}




        if (playerPosture != PlayerPosture.Jumping && 
            playerPosture != PlayerPosture.Falling && 
            armState != ArmState.Aim)
        {
            playerDeltaMovement = animator.deltaPosition;
            //playerDeltaMovement = playerTransform.TransformVector(playerMovement) * Time.deltaTime;
            //print("animator delta: " + playerDeltaMovement);
            playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;
            _averageVel = AverageVel(animator.velocity);
        }
        else
        {
            playerDeltaMovement = PlayerTransform.TransformVector(playerMovement) * Time.deltaTime;
            _averageVel.y = VerticalVelocity;
            playerDeltaMovement += _averageVel * Time.deltaTime;
            if (armState == ArmState.Aim || armState == ArmState.Cast)
            {
                playerDeltaMovement.x *= animator.GetFloat(_moveSpeedHash) / 2.5f;
                playerDeltaMovement.z *= animator.GetFloat(_moveSpeedHash) / 2.5f;
            }
        }

        if (armState == ArmState.Aim)
        {
            _averageVel.x = 0;
            _averageVel.z = 0;
        }
        CharacterController.Move(playerDeltaMovement);

    }
    */

    //---- EVENT LISTENERS ----

    private void OnMove(Vector2 movement) => MoveInput = movement;

    private void OnStartedRunning() => IsRunning = true;

    private void OnStoppedRunning() => IsRunning = false;

    private void OnMouseXY(Vector2 mousePos, bool isDeviceMouse)
    {
        mouseXY = mousePos;
    }

    private void OnJump() => IsJumping = true;

    private void OnJumpCanceled() => IsJumping = false;

    private void OnStartedChanting() => IsChanting = true;

    private void OnStoppedChanting() => IsChanting = false;

    private void OnCrouch() => IsCrouch = !IsCrouch;

    private void OnStartedAiming() => IsAiming = true;

    private void OnStoppedAiming() => IsAiming = false;

    //private void OnAttack() => IsAttacking = true;

    private void OnLockCamera()
    {
        IsLockCamera = !IsLockCamera;
        animator.SetBool("IsCameraLockOn", IsLockCamera);
        //Debug.Log("設定相機固定: " + IsLockCamera);
    }

}
