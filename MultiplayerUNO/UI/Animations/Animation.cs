using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.Animations {

    /// <summary>
    /// 动画, 如果一个动画有多种类型, 计算迭代轮次使其同步.
    /// 时间优先级: Translate > Rotate,
    /// trick: 先设置 Rotate, 再设置 Translate(用于控制时间)
    /// </summary>
    public class Animation {
        // 动画可能的种类
        public const int TRANSLATE = 0b1;
        public const int ROTATE = 0b10;
        //public const int SCALE = 0b100;
        public const int SLEEP_TIME = 20;

        // 可能的状态
        private Translate Trans;
        private Rotate Rot;
        //private Scale Scale;

        // 当前动画种类
        private int Kind;

        Form Form;

        /// <summary>
        /// 被控制的组件
        /// </summary>
        public readonly Control Btn;

        /// <summary>
        /// 估计动画迭代轮数
        /// </summary>
        private int StepCost;

        public Animation(Form form, CardButton ButtonControlled) {
            Form = form;
            Btn = ButtonControlled;
            Kind = 0;
            Trans = null;
            Rot = null;
        }

        public void SetRotate() {
            // 直接覆盖
            Kind |= ROTATE;
            Rot = new Rotate();
        }

        public void SetTranslate(int dx, int dy) {
            // 直接覆盖
            Kind |= TRANSLATE;
            Trans = new Translate(dx, dy);
            StepCost = Trans.StepCost;
            if (Rot != null) {
                Rot.ResetStep(StepCost);
            }
        }

        /// <summary>
        /// 返回 true 表示还能继续更新
        /// </summary>
        private bool UpdateState() {
            bool canUpdate = false;
            if ((Kind & TRANSLATE) != 0 && !Trans.Finished) {
                canUpdate |= Trans.GetNextState();
            }
            if ((Kind & ROTATE) != 0) {
                canUpdate |= Rot.GetNextState();
            }
            return canUpdate;
        }

        /// <summary>
        /// 单独开一线程去做动画
        /// </summary>
        public Task Run() {
            return Task.Run(() => {
                int x = Btn.Location.X,
                    y = Btn.Location.Y;
                int w = Btn.Width;
                bool first = true;
                while (UpdateState()) {
                    Form.BeginInvoke(new Action(() => {
                        // 修改 width 的时候应该是相同对中心进行修改
                        int offX = 0, offY = 0;
                        if (Rot != null) {
                            // 牌翻面
                            if (first && Rot.FlipOver) {
                                first = false;
                                var btn = Btn as CardButton;
                                if (btn != null) { btn.Flip(); }
                            }
                            Btn.Width = (int)(w * Rot.GetXScale());
                            offX = (w - Btn.Width) / 2;
                        }
                        if (Trans != null) {
                            offX += Trans.NowX;
                            offY += Trans.NowY;
                        }
                        Btn.Location = new Point(x + offX, y + offY);
                    }));
                    Thread.Sleep(SLEEP_TIME);
                }
            });
        }
    }
}
