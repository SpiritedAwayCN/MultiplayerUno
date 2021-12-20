using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using MultiplayerUNO.UI.Players;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MultiplayerUNO.Utils.Card;

/// <summary>
/// 一些设计
/// 1. 游戏开始流程
///     (1) 绘制初始场景
///     (2) 发牌
///     (3) 游戏开始
/// </summary>
namespace MultiplayerUNO.UI {
    /// <summary>
    /// TEST 开头的函数只用于测试
    /// </summary>
    public partial class MainForm : Form {
        /// <summary>
        /// 窗口大小, 除去标题大小
        /// </summary>
        public int REF_HEIGHT, REF_WIDTH;

        Random rdm = new Random();

        /// <summary>
        /// 游戏人数
        /// </summary>
        //public readonly int PlayersNumber; // TODO
        public int PlayersNumber;
        public Player[] Players; // 0 号表示自己
        public const int ME = 0;
        public readonly int MyID;

        // 牌堆
        // 为了处理方便, 这两张牌不会在被 MainForm 中去除
        private CardButton[] Piles;
        public const int PILES_NUM = 2;
        public const int PileToDistribute = 0;
        public const int PileDropped = 1;

        /// <summary>
        /// 判断是否无牌可出, true: 无牌可出
        /// </summary>
        private bool CheckNoCardCanShow() {
            foreach (var cbtn in Players[ME].BtnsInHand) {
                if (cbtn.Card.CanResponseTo(
                    GameControl.CBtnLast.Card, GameControl.ColorLast)) {
                    return false;
                }
            }
            return true;
        }

        public void SetLblChooseCardVisible(bool visible) {
            this.LblChooseCard.Visible = visible;
        }

        /// <summary>
        /// 出牌动画
        /// </summary>
        private void ShowCard(CardButton cbtn, Player player) {
            // 上一张牌修改为现在的牌
            GameControl.CBtnLast = cbtn;
            GameControl.ColorLast = cbtn.Card.Color; // TODO Color.Invalid

            Animation anima = new Animation(this, cbtn);
            var pos = Piles[PileDropped].Location;
            anima.SetTranslate(pos.X - cbtn.Location.X, pos.Y - cbtn.Location.Y);
            // 别人出牌需要翻面
            if (player != Players[ME]) {
                anima.SetRotate();
            }
            UIInvoke(() => {
                cbtn.BringToFront();
                Task.Run(async () => {
                    await anima.Run(); // 动画结束加入弃牌堆
                    GameControl.AddDroppedCard(cbtn);
                });
            });
            // 移除这张牌
            player.BtnsInHand.Remove(cbtn);
            UIInvoke(() => { player.UpdateInfo(); });

            if (player == Players[ME]) {
                cbtn.Click -= cbtn.HighLightCard;
                // 如果是自己出牌, 同时修正剩余牌的位置
                ReorganizeMyCardsAsync();
            }

            // TODO 测试
            this.LblGetCard.Visible = CheckNoCardCanShow();
        }

        private void LblGetCard_Click(object sender, EventArgs e) {
            GetCardAsync(Players[ME], 1);
        }

        /// <summary>
        /// 摸 n 张牌
        /// </summary>
        private async Task GetCardAsync(Player player, int n) {
            // TODO 暂时就是随机生成一张, 需要大改
            CardButton cbtn = new CardButton(
                rdm.Next(CardButton.TotalCard), true, true);
            cbtn.Location = Piles[PileToDistribute].Location;
            UIInvokeSync(() => {
                this.Controls.Add(cbtn);
                cbtn.BringToFront();
            });

            // 摸牌动画
            Animation anima = new Animation(this, cbtn);
            var pos = player.BtnsInHand[0].Location;
            anima.SetTranslate(
                pos.X - cbtn.Location.X
                    + (int)(INTERVAL_BETWEEN_CARDS_RATIO * CardButton.WIDTH_MODIFIED),
                pos.Y - cbtn.Location.Y);
            anima.SetRotate();
            player.BtnsInHand.Insert(0, cbtn);
            await anima.Run(); // 同步
            player.UpdateInfo();
            ReorganizeMyCardsAsync();
        }

        /// <summary>
        /// 控制整个游戏流程
        ///    1. 检查能否出牌
        /// </summary>
        private void TmrControlGame_Tick(object sender, EventArgs e) {
            // 只有我的回合才可以出牌
            GameControl.MainForm.SetLblChooseCardVisible(
                GameControl.TurnID == MyID);
        }

        /// <summary>
        /// 出牌按钮点击事件
        /// </summary>
        private void LblChooseCard_Click(object sender, EventArgs e) {
            // 这个问题应该是不会产生的, 但是为了避免出现问题, 还是先设置这个
            // 可能在消失之前很快的被点了, 压力测试
            var btn = GameControl.CBtnSelected;
            if (btn == null) { return; }

            SetMyShowCardAbility(false);

            if(btn.Card.Color == CardColor.Invalid) {
                // +4/万能牌
                // TODO 这里的动画
                GameControl.InvalidCardToChooseColor = CardColor.Invalid;
                UIInvoke(() => {
                    this.PnlChooseColor.Visible = true;
                });
            } else {
                SendShowCardJson();
            }
        }

        /// <summary>
        /// 构造出牌的 json
        /// </summary>
        private void SendShowCardJson() {
            var btn = GameControl.CBtnSelected;

            // 构造 json
            int color = -1; // 不是 万能牌/+4, 设置为 -1
            if (btn.Card.Color == CardColor.Invalid) {
                color = (int)GameControl.InvalidCardToChooseColor;
            }
            JsonData json = new JsonData() {
                ["state"] = 1,
                ["card"] = btn.Card.Number,
                ["color"] = color,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(json.ToJson());

            // 此时需要阻塞, 向服务器寻求验证, 直到服务器反馈之后再出牌
            // 这里不处理, 认为这个事件处理结束, 可以直接让 Msg 中的收消息线程处理

            // TODO 出牌动画不在这里做了
            // ShowCard(GameControl.CBtnSelected, Players[ME]);
            GameControl.CBtnSelected = null;
        }

        /// <summary>
        /// 设置自己的牌、出牌按钮、摸牌按钮的可执行性
        /// </summary>
        private void SetMyShowCardAbility(bool enable) {
            this.LblChooseCard.Enabled = enable;
            this.LblGetCard.Enabled = enable;
            foreach (var cbtn in Players[ME].BtnsInHand) {
                cbtn.Enabled = enable;
            }
        }
    }
}
