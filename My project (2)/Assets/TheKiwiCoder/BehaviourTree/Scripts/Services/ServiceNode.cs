using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Threading.Tasks;

namespace TheKiwiCoder
{
    public abstract class ServiceNode : DecoratorNode
    {
        public float interval = 1f;
        private float timer;

        protected override void OnStart()
        {
            timer = interval;
        }

        protected abstract State OnServiceUpdate();

        protected override State OnUpdate()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = interval;
                return OnServiceUpdate();
            }

            return child.Update();
        }
    }
}
