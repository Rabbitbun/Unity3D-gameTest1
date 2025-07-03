using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public abstract class DecoratorNode : Node {
        [HideInInspector] public Node child;

        public override Node Clone() {
            DecoratorNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }

        //protected bool TryGetChild(out Node node)
        //{
        //    if (child != null)
        //    {
        //        node = child;
        //        return true;
        //    }
        //    else
        //    {
        //        node = null;
        //        return false;
        //    }
        //}
        //protected Node GetChild()
        //{
        //    return child;
        //}
    }
}
