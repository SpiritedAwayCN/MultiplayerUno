using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MultiplayerUNO.Utils.Card;

namespace MultiplayerUNO.UI {
    public partial class MainForm : Form {

        /// <summary>
        /// 封装了复杂的 BeginInvoke(异步)
        /// </summary>
        private void UIInvoke(Action fun) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }

        /// <summary>
        /// 封装了复杂的 BeginInvoke(同步)
        /// </summary>
        public void UIInvokeSync(Action fun) {
            if (this.InvokeRequired) {
                var tmp = this.BeginInvoke(new Action(() => { fun(); }));
                this.EndInvoke(tmp);
            } else { fun(); }
        }

        /// <summary>
        /// 关闭窗口的时候返回到原始的连接界面
        /// </summary>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            // TODO
            if (MsgAgency.LoginForm != null)
                MsgAgency.LoginForm.Show();
        }

        /// <summary>
        /// 重新整理自己的手牌
        /// </summary>
        private async Task ReorganizeMyCardsAsync() {
            var myBtns = Players[ME].BtnsInHand;
            // z-index 排序, 顶部向右, 下面向左
            UIInvokeSync(() => {
                for (int i = 1; i < myBtns.Count; ++i) {
                    myBtns[i].SendToBack();
                }
            });
            AnimationSeq animaseq2 = new AnimationSeq();
            // 计算位置
            // 我们的设置保证自己的牌堆都在最下方的正中间, 两张牌之间间隔半张牌
            // dy = 0
            float dx = (int)(CardButton.WIDTH_MODIFIED * INTERVAL_BETWEEN_CARDS_RATIO);
            // 计算右边第一张牌的位置(左上角定位方式)
            float totalWidth = (dx * (myBtns.Count - 1) + CardButton.WIDTH_MODIFIED);
            float firstX = this.REF_WIDTH * 0.5f + (totalWidth / 2) - CardButton.WIDTH_MODIFIED;
            for (int i = 0; i < myBtns.Count; ++i) {
                var btn = myBtns[i];
                Animation anima = new Animation(this, btn);
                int offX = (int)(firstX - dx * i - btn.Location.X);
                int offY = Players[ME].Center.Y - CardButton.HEIGHT_MODIFIED / 2
                    - btn.Location.Y;
                anima.SetTranslate(offX, offY); // 移动的时候不小心点了的修正
                animaseq2.AddAnimation(anima);
            }
            await animaseq2.RunAtTheSameTime();
        }

        /// <summary>
        /// 根据 GameControl.LastColor 更新标识颜色的 label
        /// </summary>
        private void UpdateLblColor() {
            var c = GameControl.LastColor;
            Bitmap bmp = null;
            // TODO 不够精致
            if (c == CardColor.Blue) { bmp = UIImage._oButBlue; } 
            else if (c == CardColor.Green) { bmp = UIImage._oButGreen; } 
            else if (c == CardColor.Red) { bmp = UIImage._oButRed; } 
            else if (c == CardColor.Yellow) { bmp = UIImage._oButYellow; };
            var lbl = this.LblColor as Control;
            UIInvoke(() => {
                lbl.BackgroundImage = bmp;
            });
        }

        /// <summary>
        /// 每间隔 1s 检查一下时间, 更新标识事件时间的 label
        /// </summary>
        private void TmrCheckLeftTime_Tick(object sender, EventArgs e) {
            if (GameControl.TimeForYou <= 0) { return; }
            int t = GameControl.TimeForYou;
            t -= this.TmrCheckLeftTime.Interval;
            if (t < 0) { t = 0; }
            GameControl.TimeForYou = t;
            t /= 1000;
            UIInvoke(() => {
                this.LblLeftTime.Visible = true;
                this.LblLeftTime.Text =
                   t.ToString().PadLeft(2, '0');
                t.ToString().PadLeft(2, '0');
            });
        }

        /// <summary>
        /// ECS 退出
        /// </summary>
        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }

        #region 一些常数

        // UI
        // 如下的比例是相对于 [-1,1]*[-1,1] 的
        public const float PILE_OFFSET_RATE = 0.08f;
        public const int OFFSET_FOR_LBLINFO = 10;

        // 游戏
        public const int INITIAL_CARD_NUM = 7;
        public const float INTERVAL_BETWEEN_CARDS_RATIO = 0.5f;

        #endregion 一些常数
    }
}
