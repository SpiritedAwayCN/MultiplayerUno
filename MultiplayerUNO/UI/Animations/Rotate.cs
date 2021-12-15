using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.Animations {
    /// <summary>
    /// 旋转动画, 现在的实现只支持横向翻转
    /// </summary>
    public class Rotate {
        // 常量
        public const int DEFAULT_STEPS = 40;
        public const float TOTAL_ROTATE_TRANSFORM = 2.0f;
        public const float END_SCALE = 2.0f;
        public const float HALF_SCALE = 1.0f;

        // 状态量
        public bool Finished;
        public bool FlipOver;

        /// <summary>
        /// 0 -> 2 表示 1 -> 0 -> 1,
        /// 先缩小后翻面
        /// </summary>
        private float XScale;
        public float DeltaScale;
        public int StepCost;
        private int StepNow;

        public float GetXScale() {
            return Math.Abs(XScale - 1.0f);
        }

        /// <summary>
        /// true 表示动画还未结束,
        /// false 表示动画已经结束
        /// </summary>
        public bool GetNextState() {
            if (Finished) {
                XScale = END_SCALE;
                return false;
            }
            XScale += DeltaScale;
            
            // 翻面
            if (XScale > HALF_SCALE) { FlipOver = true; }

            if (++StepNow == StepCost) {
                XScale = END_SCALE;
                Finished = true;
            }
            return true;
        }

        public Rotate(int steps = DEFAULT_STEPS) {
            Finished = false;
            FlipOver = false;
            StepNow = 0;
            ResetStep(steps);
        }

        public void ResetStep(int steps) {
            StepCost = steps;
            DeltaScale = TOTAL_ROTATE_TRANSFORM / steps;
        }
    }
}
