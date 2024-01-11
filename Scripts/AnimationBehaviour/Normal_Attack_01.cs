using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_Attack_01 : StateMachineBehaviour
{
    private PlayerController playerController;
    private PlayerController.PlayerPosture playerPosture;
    private float vel;

    [SerializeField] AudioSource source;
    public AudioClip AttackSound01;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
        playerPosture = playerController.playerPosture;

        vel = 0.0f;
        if (playerController.VerticalVelocity < 0.0f)
        {
            vel = playerController.VerticalVelocity;
            playerController.VerticalVelocity = 0.0f;
        }

        source = animator.GetComponent<AudioSource>();

        source.PlayOneShot(AttackSound01);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController.IsAttacking = false;
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        if (vel < 0.0f)
            playerController.VerticalVelocity = vel;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
