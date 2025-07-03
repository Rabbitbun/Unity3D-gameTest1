using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TheKiwiCoder
{
    public class BoolCondition : ConditionNode
    {
        public ComparisonType comparisonType;

        public enum ComparisonType
        {
            IsTrue,
            IsFalse
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
            
        }

        public override bool Check()
        {
            var value = blackboard.GetData<bool>(this.selectedBlackboardProperty);
            switch (comparisonType)
            {
                case ComparisonType.IsTrue:
                    return value;
                case ComparisonType.IsFalse:
                    return !value;
            }
            
            return false;
        }
    }
}
