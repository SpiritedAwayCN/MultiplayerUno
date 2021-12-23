using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MultiplayerUNO.Utils.Card;

namespace MultiplayerUNO.UI {
    public partial class MainForm : Form {

        /// <summary>
        /// 封装了复杂的 BeginInvoke(异步)
        /// </summary>
        public void UIInvoke(Action fun) {
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
            MsgAgency.MainForm = null;
            if (MsgAgency.LoginForm != null) {
                MsgAgency.SendMsgToQueryRoomStateWhenLogin();
                MsgAgency.LoginForm.Show();
            }
        }

        /// <summary>
        /// 重新整理自己的手牌
        /// </summary>
        private async Task ReorganizeMyCardsAsync() {
            var myBtns = Players[ME].BtnsInHand;
            GameControl.CBtnSelected = null; // 洗牌的时候之前选中的牌失效
            // z-index 排序, 顶部向右, 下面向左
            UIInvokeSync(() => {
                foreach (CardButton btn in myBtns) {
                    btn.SendToBack();
                }
                // 洗牌的时候不能点击
                SetCardButtonEnable(false);
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
                var btn = myBtns[i] as CardButton;
                btn.IsHighlighted = false; // 所有的按钮都变为非高亮状态
                Animation anima = new Animation(this, btn);
                int offX = (int)(firstX - dx * i - btn.Location.X);
                int offY = Players[ME].Center.Y - CardButton.HEIGHT_MODIFIED / 2
                    - btn.Location.Y;
                anima.SetTranslate(offX, offY); // 移动的时候不小心点了的修正
                animaseq2.AddAnimation(anima);
            }
            await animaseq2.RunAtTheSameTime();
            UIInvokeSync(() => {
                SetCardButtonEnable(true);
            });
        }

        /// <summary>
        /// 每调用一次, 更新一次旋转方向
        /// </summary>
        public void UpdateLblDirection() {
            bool clockwise = !GameControl.DirectionIsClockwise;
            AnimationHighLight anima = new AnimationHighLight(this, this.LblDirection);
            anima.SetDirection(!clockwise);
            anima.SetSteps(200);
            anima.SetScale(2.0f, 1);

            var t = GameControl.LastLblDirectionTask;
            if (t != null && !t.IsCompleted) {
                // 停止动画
                while (!t.IsCanceled && !t.IsCompleted) {
                    // 因为是在处理消息的线程里面, sleep 不会导致消息的乱序
                    GameControl.LastLblDirectionTaskCancleToken.Cancel();
                    Thread.Sleep(10);
                }
                UIInvokeSync(()=> {
                    // 重置
                    var c = this.LblDirection as Control;
                    c.Size = new Size(SIGN_LABLE_SIZE, SIGN_LABLE_SIZE);
                    c.Location = new Point(SIGN_LABLE_PDDING, SIGN_LABLE_PDDING);
                    // 注意这里是原来的, 需要反一下
                    c.BackgroundImage = ((Bitmap[])c.Tag)[clockwise ? 1 : 0];
                });
            }
            var token = new CancellationTokenSource();
            GameControl.LastLblDirectionTaskCancleToken = token;
            GameControl.LastLblDirectionTask = anima.Run(token);
            GameControl.DirectionIsClockwise = clockwise;
        }

        /// <summary>
        /// 根据 GameControl.LastColor 更新标识颜色的 label
        /// </summary>
        private void UpdateLblColor() {
            var c = GameControl.LastColor;
            Bitmap bmp = null;
            // TODO 不够精致
            if (c == CardColor.Blue) { bmp = UIImage._oButBlue; } else if (c == CardColor.Green) { bmp = UIImage._oButGreen; } else if (c == CardColor.Red) { bmp = UIImage._oButRed; } else if (c == CardColor.Yellow) { bmp = UIImage._oButYellow; };
            var lbl = this.LblColor as Control;
            UIInvoke(() => {
                lbl.BackgroundImage = bmp;
            });
        }

        /// <summary>
        /// 每间隔 1s 检查一下时间, 更新标识事件时间的 label
        /// </summary>
        private void TmrCheckLeftTime_Tick(object sender, EventArgs e) {
            if (GameControl.TimeForYou <= 0) {
                UIInvoke(()=> {
                    ImDummy();
                });
                return;
            }
            Label lbl = this.LblLeftTime;
            int t = GameControl.TimeForYou;
            t -= this.TmrCheckLeftTime.Interval;
            if (t < 0) { t = 0; }
            GameControl.TimeForYou = t;
            t /= 1000;
            // 更新下位置
            int playerIdx = GameControl.PlayerId2PlayerIndex[GameControl.TurnID];
            Point pos;
            if (playerIdx != ME) {
                // 如果不是我自己就居中
                pos = Players[playerIdx].Center;
            } else {
                var pnl = this.PnlNormalShowCardorNot;
                pos = pnl.Location;
                pos.Y += pnl.Height / 2;
            }
            pos.X -= lbl.Width / 2;
            pos.Y -= lbl.Height / 2;
            UIInvoke(() => {
                lbl.Location = pos;
                lbl.BringToFront();
                lbl.Visible = true;
                lbl.Text = t.ToString().PadLeft(2, '0');
            });
        }

        /// <summary>
        /// 这个时候我不能出牌了(可能原因: 超时)
        /// </summary>
        private void ImDummy() {
            this.PnlAfterGetOne.Visible = false;
            this.PnlChooseColor.Visible = false;
            this.PnlNormalShowCardorNot.Visible = false;
            this.PnlPlus2.Visible = false;
            this.PnlQuestion.Visible = false;
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
        public const int SIGN_LABLE_SIZE = 80;
        public const int SIGN_LABLE_PDDING = 10;

        // 游戏
        public const int INITIAL_CARD_NUM = 7;
        public const float INTERVAL_BETWEEN_CARDS_RATIO = 0.5f;

        #endregion 一些常数
    }
}