using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class Wait : ActionNode {
        public float duration = 1;
        float startTime;

        [Tooltip("若設為true, duration會被替換成隨機值")]
        public bool setRandom = false;
        public float maxValue = 5.0f;
        public float minValue = 1.0f;

        protected override void OnStart() {
            startTime = Time.time;
            if (setRandom)
                duration = Random.Range(minValue, maxValue);
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            if (Time.time - startTime > duration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}
