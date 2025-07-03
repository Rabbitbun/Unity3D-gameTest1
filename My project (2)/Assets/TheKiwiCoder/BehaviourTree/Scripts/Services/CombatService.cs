using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using AbilitySystem;

namespace TheKiwiCoder
{
    public class CombatService : ServiceNode
    {
        public float minObservationTime = 1f;
        public float maxObservationTime = 5f;
        [SerializeField] private float ObservationTime;

        [SerializeField] private float minDistance = 5f;
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float movementSpeed = 2f;

        [SerializeField] private AbilitySystemCharacter target;
        [SerializeField] private Vector3 targetPosition;

        protected override void OnStart()
        {
            ObservationTime = Random.Range(minObservationTime, maxObservationTime);

            //target = blackboard.GetData<AbilitySystemCharacter>("target");
            //targetPosition = target.transform.position;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            ObservationTime -= Time.deltaTime;
            //blackboard.UpdateData<Vector3>("moveToPosition", targetPosition);
            //context.agent.destination = blackboard.GetData<Vector3>("moveToPosition");

            //Vector3 playerPos = blackboard.GetData<AbilitySystemCharacter>("target").transform.position;
            //float distance = Vector3.Distance(context.agent.transform.position, playerPos);
            //// 如果距離小於最小距離,讓 NPC 後退
            ////if (distance < minDistance)
            ////{
            ////    context.animator.Play("Back_Walk");
            ////}
            //if (distance > maxDistance)
            //{
            //    context.animator.Play("Walk");
            //}
            //// 計算繞著玩家移動的角度
            //else
            //{
            //    Vector3 direction = Quaternion.Euler(0f, 45f, 0f) * (playerPos - context.agent.transform.position);
            //    //context.agent.Move(direction.normalized * movementSpeed * Time.deltaTime);
            //    var r = Random.Range(0, 3);
            //    if (r == 0) context.animator.Play("Back_Walk");
            //    if (r == 1) context.animator.Play("LSide_Walk");
            //    if (r == 2) context.animator.Play("RSide_Walk");
            //}

            return base.OnUpdate();
        }

        protected override State OnServiceUpdate()
        {
            if (ObservationTime <= 0)
            {
                blackboard.UpdateData<AICombatState>("AICombatState", AICombatState.Attack);
                Debug.Log("觀察時間結束, 進入戰鬥狀態...");
                return State.Success;
            }

            return State.Running;
        }

    }
}
