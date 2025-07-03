using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TheKiwiCoder
{
    public abstract class ConditionNode : DecoratorNode
    {
        protected bool lastConditionCheckResult = false;
        public string selectedBlackboardProperty;
        public string selectedEnumValue;
        public bool ShouldAbort = true;

        protected override void OnStart()
        {
            // 在節點開始時可以執行一些初始化操作
        }

        protected override void OnStop()
        {
            // 在節點停止時可以執行一些清理操作
            
        }

        protected override State OnUpdate()
        {
            lastConditionCheckResult = Check();
            if (lastConditionCheckResult == false)
            {
                if (ShouldAbort)
                    this.Abort();
                return State.Failure;
            }
            // if true, then enter child node
            var state = child.Update();
            if (state == State.Success || state == State.Failure)
            {
                return state;
            }
            return State.Running;


            //if (!TryGetChild(out Node ChildNode))
            //{
            //    return State.Failure;
            //}

            //if (ChildNode.state == State.Success || ChildNode.state == State.Failure)
            //{
            //    return ChildNode.state;
            //}
            //lastConditionCheckResult = Check();
            //if (lastConditionCheckResult == false)
            //{
            //    return State.Failure;
            //}
            //// if true, then enter child node
            //var state = child.Update();
            //if (state == State.Success)
            //{
            //    return State.Success;
            //}
            //return State.Running;

            //return State.Running;
        }

        /// <summary>
        /// Method called to check condition
        /// </summary>
        /// <returns>Condition result</returns>
        public abstract bool Check();


    }
}