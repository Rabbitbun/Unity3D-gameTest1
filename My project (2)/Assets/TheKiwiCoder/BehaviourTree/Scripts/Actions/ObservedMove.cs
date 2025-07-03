using UnityEngine;
using TheKiwiCoder;
using AbilitySystem;
using System.Threading.Tasks;

public class ObservedMove : ActionNode
{
    public string rightWalkAnim;
    public string leftWalkAnim;
    public string WalkAnim;

    private bool rotateClockwise; // 顺时针旋转

    public float speed = 1;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    [SerializeField] private Vector3 targetPosition;

    protected override void OnStart()
    {
        targetPosition = blackboard.GetData<AbilitySystemCharacter>("target").transform.position;

        rotateClockwise = Random.value > 0.5f;

        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop()
    {
        context.agent.speed = 0;
        context.agent.ResetPath();
    }

    protected override State OnUpdate()
    {
        // 计算旋转方向
        Vector3 direction = rotateClockwise ? Vector3.right : Vector3.left;
        Vector3 desiredPosition = targetPosition + (direction * speed * Time.deltaTime);

        // 设置 NavMeshAgent 目标位置
        context.agent.SetDestination(desiredPosition);

        if (rotateClockwise) context.animator.Play(leftWalkAnim);
        else context.animator.Play(rightWalkAnim);

        if (context.agent.pathPending)
        {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance)
        {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }
}
