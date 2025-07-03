using UnityEngine;

namespace TheKiwiCoder
{
    public class EnumCondition : ConditionNode
    {
        public enum ComparisonType
        {
            IsEqual,
            IsNotEqual
        }
        [Tooltip("比對等於或不等於")]
        public ComparisonType comparisonType;

        private enum AIStateEnums
        {
            AICombatState,
            AIState
        } 

        [Tooltip("選擇需要的 其他的可以不管")]
        public AICombatState AICombatState; // 目標 AICombatState
        [Tooltip("選擇需要的 其他的可以不管")]
        public AIState AIState; // 目標 AIState

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        public override bool Check()
        {
            //var value = blackboard.GetData<AIState>(this.selectedBlackboardProperty);
            //switch (comparisonType)
            //{
            //    case ComparisonType.IsEqual:
            //        //Debug.Log(value.ToString() + " " + targetState);
            //        return value == targetState;
            //    case ComparisonType.IsNotEqual:
            //        return value != targetState;
            //}

            //return false;
            if (this.selectedBlackboardProperty.ToString() == AIStateEnums.AIState.ToString())
            {
                var value = blackboard.GetData<AIState>(this.selectedBlackboardProperty);
                switch (comparisonType)
                {
                    case ComparisonType.IsEqual:
                        return value == AIState;
                    case ComparisonType.IsNotEqual:
                        return value != AIState;
                }
            }
            else if (this.selectedBlackboardProperty.ToString() == AIStateEnums.AICombatState.ToString())
            {
                var value = blackboard.GetData<AICombatState>(this.selectedBlackboardProperty);
                switch (comparisonType)
                {
                    case ComparisonType.IsEqual:
                        return value == AICombatState;
                    case ComparisonType.IsNotEqual:
                        return value != AICombatState;
                }
            }

            
            return false;
        }
    }

}
