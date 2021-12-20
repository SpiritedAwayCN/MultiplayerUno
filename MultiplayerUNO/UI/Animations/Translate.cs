using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.Animations {
    // 平移动画
    public class Translate {
        // 常量
        public const int DELTA_PER_FRAME = 5;
        public const float EPS = 0.001f;

        // 状态量
        public float NowX, NowY;
        public float DstX, DstY;
        public bool Finished;
        public float Dx, Dy;

        public int StepCost { get; }
        private int StepNow;

        /// <summary>
        /// true 表示动画还未结束,
        /// false 表示动画已经结束
        /// </summary>
        public bool GetNextState() {
            if(Finished) {
                Dx = DstX;
                Dy = DstY;
                return false;
            }

            // 更新
            NowX += Dx;
            NowY += Dy;
            if (++StepNow == StepCost) {
                NowX = DstX;
                NowY = DstY;
                Finished = true;
            }
            return true;
        }

        public Translate(float dstX, float dstY) {
            NowX = NowY = 0;
            DstX = dstX;
            DstY = dstY;
            Finished = false;
            StepNow = 0;

            // 按照慢的为准
            float tx = 1.0f * Math.Abs(DstX) / DELTA_PER_FRAME,
                  ty = 1.0f * Math.Abs(DstY) / DELTA_PER_FRAME;
            float t = Math.Max(tx, ty);
            // special case: t=0
            if (t == 0) { t = 1f; }
            StepCost = (int)Math.Ceiling(t);
            Dx = DstX / t;
            Dy = DstY / t;
        }
    }
}
