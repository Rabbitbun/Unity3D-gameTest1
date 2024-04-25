using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : StateMachineBehaviour
{
    [SerializeField] private PlayerController playerController;
    private PlayerController.PlayerPosture playerPosture;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
        playerPosture = playerController.playerPosture;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckGround();
        SwitchPlayerStates();
        CaculateGravity();
        Jump();
        SetupAnimator(animator);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerPosture != PlayerController.PlayerPosture.Jumping &&
            playerPosture != PlayerController.PlayerPosture.Falling &&
            playerController.armState != PlayerController.ArmState.Aim)

        {
            playerController.playerDeltaMovement = animator.deltaPosition;
            //playerDeltaMovement = playerTransform.TransformVector(playerMovement) * Time.deltaTime;
            //print("animator delta: " + playerDeltaMovement);
            playerController.playerDeltaMovement.y = playerController.VerticalVelocity * Time.deltaTime;
            playerController._averageVel = playerController.AverageVel(animator.velocity);
        }
        else
        {
            playerController.playerDeltaMovement = playerController.PlayerTransform.TransformVector(playerController.playerMovement) * Time.deltaTime;
            playerController._averageVel.y = playerController.VerticalVelocity;
            playerController.playerDeltaMovement += playerController._averageVel * Time.deltaTime;
            if (playerController.armState == PlayerController.ArmState.Aim)
            {
                playerController.playerDeltaMovement.x *= animator.GetFloat(playerController._moveSpeedHash) / 2.5f;
                playerController.playerDeltaMovement.z *= animator.GetFloat(playerController._moveSpeedHash) / 2.5f;
            }
        }

        if (playerController.armState == PlayerController.ArmState.Aim)
        {
            playerController._averageVel.x = 0;
            playerController._averageVel.z = 0;
        }
        playerController.CharacterController.Move(playerController.playerDeltaMovement);
    }

    // OnStateIK is called right after animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    void CheckGround()
    {
        if (Physics.SphereCast(playerController.PlayerTransform.position + (Vector3.up * playerController.GroundCheckOffset),
                              playerController.CharacterController.radius,
                              Vector3.down,
                              out RaycastHit hit,
                              playerController.GroundCheckOffset - playerController.CharacterController.radius + 2 * playerController.CharacterController.skinWidth))
        {
            playerController.IsGrounded = true;
        }
        else
        {
            playerController.IsGrounded = false;
            playerController.CouldFall = !Physics.Raycast(playerController.PlayerTransform.position, Vector3.down, playerController.FallHeight);
        }
    }

    void SwitchPlayerStates()
    {
        playerPosture = playerController.playerPosture;
        switch (playerPosture)
        {
            case PlayerController.PlayerPosture.Stand:
                if (playerController.VerticalVelocity > 0)
                {
                    playerPosture = PlayerController.PlayerPosture.Jumping;
                }
                else if (!playerController.IsGrounded && playerController.CouldFall)
                {
                    playerPosture = PlayerController.PlayerPosture.Falling;
                }
                else if (playerController.IsCrouch)
                {
                    playerPosture = PlayerController.PlayerPosture.Crouch;
                }
                break;

            case PlayerController.PlayerPosture.Crouch:
                if (!playerController.IsGrounded && playerController.CouldFall)
                {
                    playerPosture = PlayerController.PlayerPosture.Falling;
                }
                else if (!playerController.IsCrouch)
                {
                    playerPosture = PlayerController.PlayerPosture.Stand;
                }
                break;

            case PlayerController.PlayerPosture.Falling:
                if (playerController.IsGrounded)
                {
                    playerController.CalculateJumpCD();
                }
                if (playerController.IsLanding)
                {
                    playerPosture = PlayerController.PlayerPosture.Landing;
                }
                break;

            case PlayerController.PlayerPosture.Jumping:
                if (playerController.IsGrounded)
                {
                    playerController.CalculateJumpCD();
                }
                if (playerController.IsLanding)
                {
                    playerPosture = PlayerController.PlayerPosture.Landing;
                }
                break;

            case PlayerController.PlayerPosture.Landing:
                if (!playerController.IsLanding)
                {
                     playerPosture = PlayerController.PlayerPosture.Stand;
                }
                break;
        }

        if (playerController.MoveInput.magnitude == 0)
        {
            playerController.locomotionState = PlayerController.LocomotionState.Idle;
        }
        else if (playerController.IsRunning == false)
        {
            playerController.locomotionState = PlayerController.LocomotionState.Walk;
        }
        else
        {
            playerController.locomotionState = PlayerController.LocomotionState.Run;
        }

        if (playerController.IsAiming)
        {
            playerController.armState = PlayerController.ArmState.Aim;
        }
        else if (playerController.IsAttacking)
        {
            playerController.armState = PlayerController.ArmState.Attack;
        }
        else if (playerController.IsChanting)
        {
            playerController.armState = PlayerController.ArmState.Cast;
        }
        else
        {
            playerController.armState = PlayerController.ArmState.Normal;
        }
    }

    void CaculateGravity()
    {
        if (playerPosture != PlayerController.PlayerPosture.Jumping && playerPosture != PlayerController.PlayerPosture.Falling)
        {
            if (playerController.IsGrounded == false)
            {
                playerController.VerticalVelocity += playerController.Gravity * playerController.FallMultiplier * Time.deltaTime;
            }
            else
            {
                playerController.VerticalVelocity = playerController.Gravity * Time.deltaTime;
            }
        }
        else
        {
            if (playerController.VerticalVelocity <= 0 || playerController.IsJumping == false)
            {
                playerController.VerticalVelocity += playerController.Gravity * playerController.FallMultiplier * Time.deltaTime;
            }
            else
            {
                playerController.VerticalVelocity += playerController.Gravity * Time.deltaTime;

            }
        }
    }

    void Jump()
    {
        if (playerController.IsAttacking) return;
        if (playerPosture == PlayerController.PlayerPosture.Stand && 
            playerController.IsJumping && playerController.IsGrounded)
        {
            playerController.VerticalVelocity = Mathf.Sqrt(-2 * playerController.Gravity * playerController.MaxHeight);
            playerController.FeetTween = Mathf.Repeat(playerController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1);
            playerController.FeetTween = playerController.FeetTween < 0.5f ? 1 : -1;

            if (playerController.locomotionState == PlayerController.LocomotionState.Run)
            {
                playerController.FeetTween *= 3;
            }
            else if (playerController.locomotionState == PlayerController.LocomotionState.Walk)
            {
                playerController.FeetTween *= 2;
            }
            else
            {
                playerController.FeetTween = UnityEngine.Random.Range(0.5f, 1f) * playerController.FeetTween;
            }
        }
    }

    void SetupAnimator(Animator animator)
    {
        if (playerPosture == PlayerController.PlayerPosture.Stand)
        {
            animator.SetFloat(playerController._postureHash, playerController.StandThreshold, 0.1f, Time.deltaTime);
            switch (playerController.locomotionState)
            {
                case PlayerController.LocomotionState.Idle:
                    animator.SetFloat(playerController._moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case PlayerController.LocomotionState.Walk:
                    animator.SetFloat(playerController._moveSpeedHash, playerController.playerMovement.magnitude * playerController._walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case PlayerController.LocomotionState.Run:
                    animator.SetFloat(playerController._moveSpeedHash, playerController.playerMovement.magnitude * playerController._runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        else if (playerPosture == PlayerController.PlayerPosture.Crouch)
        {
            animator.SetFloat(playerController._postureHash, playerController.CrouchThreshold, 0.1f, Time.deltaTime);
            switch (playerController.locomotionState)
            {
                case PlayerController.LocomotionState.Idle:
                    animator.SetFloat(playerController._moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    animator.SetFloat(playerController._moveSpeedHash, playerController._crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }

        else if (playerPosture == PlayerController.PlayerPosture.Jumping)
        {
            animator.SetFloat(playerController._postureHash, playerController.MidairThreshold);
            animator.SetFloat(playerController._verticalVelHash, playerController.VerticalVelocity);
            animator.SetFloat(playerController._feetTweenHash, playerController.FeetTween);
        }
        else if (playerPosture == PlayerController.PlayerPosture.Landing)
        {
             animator.SetFloat(playerController._postureHash, playerController.LandingThreshold, 0.01f, Time.deltaTime);
            switch (playerController.locomotionState)
            {
                case PlayerController.LocomotionState.Idle:
                    animator.SetFloat(playerController._moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case PlayerController.LocomotionState.Walk:
                    animator.SetFloat(playerController._moveSpeedHash, playerController.playerMovement.magnitude * playerController._walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case PlayerController.LocomotionState.Run:
                    animator.SetFloat(playerController._moveSpeedHash, playerController.playerMovement.magnitude * playerController._runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }

        else if (playerPosture == PlayerController.PlayerPosture.Falling)
        {
            animator.SetFloat(playerController._postureHash, playerController.MidairThreshold);
            animator.SetFloat(playerController._verticalVelHash, playerController.VerticalVelocity);
        }

        if (playerController.armState == PlayerController.ArmState.Normal)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Casting", false);
            float rad = Mathf.Atan2(playerController.playerMovement.x, playerController.playerMovement.z);
            animator.SetFloat(playerController._rotateSpeedHash, rad, 0.1f, Time.deltaTime * 2.0f);
            playerController.PlayerTransform.Rotate(0f, rad * 300 * Time.deltaTime * 2.0f, 0f);
        }
        else if (playerController.armState == PlayerController.ArmState.Attack)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Casting", false);
        }
        else if (playerController.armState == PlayerController.ArmState.Cast)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Casting", true);
            float rad = Mathf.Atan2(playerController.playerMovement.x, playerController.playerMovement.z);
            animator.SetFloat(playerController._rotateSpeedHash, rad, 0.1f, Time.deltaTime * 2.0f);
            playerController.PlayerTransform.Rotate(0f, rad * 300 * Time.deltaTime * 2.0f, 0f);
        }
        else if (playerController.armState == PlayerController.ArmState.Aim)
        {
            animator.SetBool("Aim", true);
            animator.SetBool("Casting", false);
            animator.SetFloat(playerController._rotateSpeedHash, 0f);
        }
    }
}
