using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using MultiplayerUNO.UI.Players;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// 出牌动画
        /// </summary>
        public void ShowCard(TurnInfo turnInfo) {
            CardButton cbtn = null;
            int playerIdx = turnInfo.GetPlayerIndex();
            bool win = Players[playerIdx].ShowOneCard(); // 出牌之后更新一些属性
            if (ME == playerIdx) {
                // 如果是自己出牌, 则在牌堆中找到这张牌
                // 认为所有的牌都有不一样的编号(其实一样也没事, 问题不大)
                foreach (var c in Players[ME].BtnsInHand) {
                    if (c.Card.CardId == turnInfo.LastCardID) {
                        cbtn = c;
                        break;
                    }
                }
                // 移除这张牌
                Players[playerIdx].BtnsInHand.Remove(cbtn);
            } else {
                // 如果是别人的牌则需要 new 一张牌
                cbtn = new CardButton(turnInfo.LastCardID, true, false);
                CardButton cbtnBack = Players[playerIdx].BtnsInHand[0];
                // 必须同步, 这里的 Location 设置会影响后面的发牌动作
                UIInvokeSync(() => {
                    this.Controls.Add(cbtn);
                    cbtn.Location = cbtnBack.Location;
                    cbtn.BringToFront();
                    // 注意如果是最后一张牌的话, 我们需要把底牌抹去
                    if (win) {
                        Players[playerIdx].BtnsInHand.Clear();
                        this.Controls.Remove(cbtn);
                    }
                });
            }

            Animation anima = new Animation(this, cbtn);
            var pos = Piles[PileDropped].Location;
            anima.SetTranslate(pos.X - cbtn.Location.X, pos.Y - cbtn.Location.Y);
            // 别人出牌需要翻面
            if (playerIdx != ME) {
                anima.SetRotate();
            }
            UIInvoke(() => {
                cbtn.BringToFront();
                Task.Run(async () => {
                    await anima.Run(); // 动画结束加入弃牌堆
                    UIInvoke(() => {
                        GameControl.AddDroppedCard(cbtn);
                    });
                });
            });

            // 更新牌的张数
            UIInvoke(() => { Players[playerIdx].UpdateInfo(); });

            if (playerIdx == ME) {
                cbtn.Click -= cbtn.HighLightCard;
                // 如果是自己出牌, 同时修正剩余牌的位置
                ReorganizeMyCardsAsync();
            }
        }

        /// <summary>
        /// 上家出了+2之后, 选择出不出牌
        /// </summary>
        internal void ShowOrGetAfterPlus2(TurnInfo turnInfo) {
            int num = turnInfo.IntInfo;
            UIInvoke(() => {
                this.LblPlus2Total.Text = "需要摸牌数: " + num;
                this.PnlPlus2.Visible = true;
            });
        }

        /// <summary>
        /// 游戏结束
        /// 1. 显示谁胜利了
        /// 2. 展示手牌
        /// </summary>
        internal void GameOver(TurnInfo turnInfo) {
            // 显示胜利
            string playerName = Players[
                GameControl.PlayerId2PlayerIndex[turnInfo.GetPlayerID()]
            ].Name;
            UIInvoke(() => {
                this.LblGameOver.Text = "游戏结束, 胜利者是: " + playerName;
                this.LblGameOver.Visible = true;
            });
            // 展示手牌 TODO
        }

        /// <summary>
        /// 回应 +4
        ///   1. 响应 panel 显示出来 
        /// </summary>
        internal void RespondToPlus4(TurnInfo turnInfo) {
            UIInvokeSync(() => {
                this.PnlQuestion.Visible = true;
                // TODO 其他功能键不能响应
            });
        }

        /// <summary>
        /// 某人摸了好几张牌, 这里只需要构造动画即可
        /// </summary>
        public void GetManyCards(TurnInfo turnInfo) {
            int num = turnInfo.LastCardID;
            if (turnInfo.TurnID != MyID) {
                // 别人, 假装摸一张牌即可
                GetCardForOtherOne(turnInfo, num);
                return;
            }
            // 自己, 需要摸好几张牌
            AnimationSeq animaseq = new AnimationSeq();
            var pos = Players[ME].BtnsInHand[0].Location;
            pos.Y = Players[ME].Center.Y - CardButton.HEIGHT_MODIFIED / 2;
            for (int i = 0; i < num; ++i) {
                int cardID = turnInfo.PlayerCards[i];
                CardButton cbtn = new CardButton(cardID, true, true);
                cbtn.Location = Piles[PileToDistribute].Location;
                UIInvokeSync(() => {
                    this.Controls.Add(cbtn);
                    cbtn.BringToFront();
                });
                // (1)
                Animation anima = new Animation(this, cbtn);
                pos.X +=
                    (int)(INTERVAL_BETWEEN_CARDS_RATIO * CardButton.WIDTH_MODIFIED);
                anima.SetTranslate(pos.X - cbtn.Location.X,
                                   pos.Y - cbtn.Location.Y);
                anima.SetRotate();
                Players[ME].BtnsInHand.Insert(0, cbtn);
                animaseq.AddAnimation(anima);
            }
            var w = animaseq.Run();
            Task.Run(async () => {
                await w; // 同步
                Players[ME].CardsCount += num; // 更新卡牌数量
                UIInvoke(() => { Players[ME].UpdateInfo(); });
                _ = ReorganizeMyCardsAsync();
            });
        }

        /// <summary>
        /// 能够正常出牌
        /// </summary>
        internal void ShowOrGetNormal(TurnInfo turnInfo) {
            UIInvoke(() => {
                this.PnlNormalShowCardorNot.Visible = true;
            });
        }

        private void LblGetCard_Click(object sender, EventArgs e) {
            // 构造 json
            JsonData json = new JsonData() {
                ["state"] = 2,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(json.ToJson());
            if (GameControl.CBtnSelected != null) {
                // 选牌归位
                GameControl.CBtnSelected.PerformClick();
                GameControl.CBtnSelected = null;
            }
            this.PnlNormalShowCardorNot.Visible = false;
        }


        public void SetShowOrNotVisibility(bool visible) {
            this.LblShowCard.Visible = visible;
        }

        /// <summary>
        /// 别人摸 num 张牌
        /// </summary>
        public void GetCardForOtherOne(TurnInfo turnInfo, int num = 1) {
            int playerIdx = GameControl.PlayerId2PlayerIndex[turnInfo.TurnID];
            Animation anima = GetACardAnima(playerIdx, turnInfo.LastCardID);
            var w = anima.Run();
            Task.Run(async () => {
                await w;
                Players[playerIdx].CardsCount += num;
                UIInvoke(() => {
                    Players[playerIdx].UpdateInfo();
                    // 去除多余卡牌
                    while (Players[playerIdx].BtnsInHand.Count > 1) {
                        var cbtn = Players[playerIdx].BtnsInHand[0];
                        this.Controls.Remove(cbtn);
                        Players[playerIdx].BtnsInHand.RemoveAt(0); // 加头去头
                    }
                });
            });
        }

        /// <summary>
        /// 自己摸一张牌
        /// (1) 摸牌动画
        /// (2) 高亮摸到的这张牌
        /// (3) 然后设置所有的卡牌按钮都无法响应, 只能出牌或者不出牌
        /// </summary>
        public void GetACardForMe(TurnInfo turnInfo) {
            // (1)
            Animation anima = GetACardAnima(ME, turnInfo.LastCardID);
            var t = anima.Run();
            var cbtn = Players[ME].BtnsInHand[0];
            Card c = new Card(turnInfo.LastCardID);
            bool canResponed = c.CanResponseTo(
                GameControl.LastCard, GameControl.LastColor);
            if (!canResponed) {
                // 不能响应直接回复不出牌
                DonotShowCardAfterGetJson();
                return;
            }
            Task.Run(async () => {
                // (2)
                await t; // 同步
                Players[ME].CardsCount++; // 更新牌
                // (3)
                UIInvokeSync(() => {
                    cbtn.PerformClick();
                    Players[ME].UpdateInfo();
                    SetCardButtonEnable(false);
                    this.LblShowCard.Visible = true;
                    this.LblGetCard.Visible = false; // TODO
                });
                // (4) 是否准备打牌
                UIInvoke(() => {
                    // 设置卡牌按钮都无法响应
                    SetShowOrNotVisibility(true);
                    this.PnlAfterGetOne.Visible = true;
                });
            });
        }

        private void DonotShowCardAfterGetJson() {
            // 构造 json
            JsonData json = new JsonData() {
                ["state"] = 1,
                ["action"] = 0,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(json.ToJson());
        }

        /// <summary>
        /// 获取摸一张牌的动画
        /// </summary>
        public Animation GetACardAnima(int playerIdx, int cardID) {
            bool isMe = (playerIdx == ME);
            // 添加按钮到 Form 中
            CardButton cbtn = new CardButton(cardID, true, isMe);
            cbtn.Location = Piles[PileToDistribute].Location;
            UIInvokeSync(() => {
                this.Controls.Add(cbtn);
                cbtn.BringToFront();
            });
            // (1)
            Animation anima = new Animation(this, cbtn);
            var pos = Players[playerIdx].BtnsInHand[0].Location; // 第 0 张牌是最右边的
            pos.Y = Players[playerIdx].Center.Y - CardButton.HEIGHT_MODIFIED / 2;
            // 别人摸牌, 不需要翻开, 直接叠在一起即可
            // 自己摸牌, 需要翻开, 还需要插入手牌
            if (isMe) {
                pos.X +=
                    (int)(INTERVAL_BETWEEN_CARDS_RATIO * CardButton.WIDTH_MODIFIED);
            }
            anima.SetTranslate(pos.X - cbtn.Location.X,
                               pos.Y - cbtn.Location.Y);
            if (isMe) {
                anima.SetRotate();
            }
            // 注意这里即使不是自己, 我们也把牌加进去了(await 动画结束后删除)
            Players[playerIdx].BtnsInHand.Insert(0, cbtn);
            return anima;
        }

        /// <summary>
        /// 控制整个游戏流程
        ///    1. 检查能否出牌、摸牌
        ///    2. 是不是第一轮第一个出牌
        ///    3. 更新颜色
        /// </summary>
        private void TmrControlGame_Tick(object sender, EventArgs e) {
            bool myturn = (GameControl.TurnID == MyID);
            bool ff = GameControl.FirstTurnFirstShow();
            UIInvoke(() => {
                // 更新颜色
                UpdateLblColor();
                // 第一次出第一张牌随便出
                this.LblFirstShowCard.Visible = ff;
            });
        }

        /// <summary>
        /// 出牌按钮点击事件
        /// </summary>
        private void LblShowCard_Click(object sender, EventArgs e) {
            // 这个问题应该是不会产生的, 但是为了避免出现问题, 还是先设置这个
            // 可能在消失之前很快的被点了, 压力测试
            var btn = GameControl.CBtnSelected;
            if (btn == null) { return; }

            if (btn.Card.Color == CardColor.Invalid) {
                // +4/万能牌
                // TODO 这里的动画
                GameControl.InvalidCardToChooseColor = CardColor.Invalid;
                UIInvoke(() => {
                    this.PnlChooseColor.Visible = true;
                });
            } else {
                SendShowCardJson();
            }
            this.PnlNormalShowCardorNot.Visible = false;
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
                ["card"] = btn.Card.CardId,
                ["color"] = color,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(json.ToJson());

            // 此时需要阻塞, 向服务器寻求验证, 直到服务器反馈之后再出牌
            // 这里不处理, 认为这个事件处理结束, 可以直接让 Msg 中的收消息线程处理

            // 出牌动画不在这里做了
            // ShowCard(GameControl.CBtnSelected, Players[ME]);
            GameControl.CBtnSelected = null;
        }

        /// <summary>
        /// 质疑按钮点击事件发生
        /// </summary>
        private void LblQuestion_Click(object sender, EventArgs e) {
            SendMsgRespondToPlus4(6);
            this.PnlQuestion.Visible = false;
        }

        /// <summary>
        /// 不质疑按钮点击事件发生
        /// </summary>
        private void LblNoQuestion_Click(object sender, EventArgs e) {
            SendMsgRespondToPlus4(6);
            this.PnlQuestion.Visible = false;
        }

        /// <summary>
        /// 发送响应 +4 的 json
        /// </summary>
        private void SendMsgRespondToPlus4(int state) {
            JsonData json = new JsonData() {
                ["state"] = state,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(json.ToJson());
        }


        /// <summary>
        /// 某人展示手牌
        /// </summary>
        public void DisplayOnesCard(TurnInfo turnInfo) {
            List<CardButton> lst = new List<CardButton>();
            foreach (int i in turnInfo.PlayerCards) {
                lst.Add(new CardButton(i, false, false));
            }
            string playerName = Players[
                GameControl.PlayerId2PlayerIndex[turnInfo.GetPlayerID()]
            ].Name;
            Panel pnl = this.PnlDisplayCard;
            Label lbl = this.LblDisplayCardsPlayerName;
            UIInvoke(() => {
                lbl.Text = playerName;
                int h = lbl.Height;
                for (int i = 0; i < lst.Count; ++i) {
                    var cbtn = lst[i];
                    pnl.Controls.Add(cbtn);
                    cbtn.Location = new Point(CardButton.WIDTH_MODIFIED * i, h);
                }
                lbl.Width = pnl.Width;
                pnl.Visible = true;
            });
        }

        /// <summary>
        /// 可见性发生修改的时候, 事件发生
        ///   1.变为不可见的时候去掉其中的所有按钮并释放资源
        ///   2. 变为可见的时候, 计时器启动, 5 秒之后展示牌结束
        /// </summary>
        private void PnlDisplayCard_VisibleChanged(object sender, EventArgs e) {
            if (this.PnlDisplayCard.Visible) {
                this.TmrDisplayCard.Interval = 5000;
                this.TmrDisplayCard.Start();
                return;
            }
            Panel pnl = this.PnlDisplayCard;

            // 去除按钮, 释放资源
            List<CardButton> lst = new List<CardButton>();
            foreach (var c in pnl.Controls) {
                var cbtn = c as CardButton;
                if (cbtn != null) {
                    lst.Add(cbtn);
                }
            }
            UIInvoke(() => {
                foreach (var c in lst) {
                    pnl.Controls.Remove(c);
                    c.Dispose();
                }
            });
        }

        private void TmrDisplayCard_Tick(object sender, EventArgs e) {
            this.TmrDisplayCard.Stop();
            this.PnlDisplayCard.Visible = false;
        }

        /// <summary>
        /// 打出 +2
        /// </summary>
        private void LblPlayPlus2_Click(object sender, EventArgs e) {
            CardButton cbtn = GameControl.CBtnSelected;
            if (cbtn == null || !cbtn.Card.IsPlus2()) { return; }

            JsonData json = new JsonData() {
                ["state"] = 3,
                ["card"] = cbtn.Card.CardId,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(json.ToJson());
        }

        /// <summary>
        /// 不打出 +2
        /// </summary>
        private void LblDonotPlayPlus2_Click(object sender, EventArgs e) {
            JsonData json = new JsonData() {
                ["state"] = 4,
                ["queryID"] = GameControl.QueryID
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(json.ToJson());
        }

        public void SetCardButtonEnable(bool enable) {
            foreach (var cbtn in Players[ME].BtnsInHand) {
                cbtn.Enabled = enable;
            }
        }







        // DEBUG

        private bool TxtDebugIsFront = false;
        private void TxtDebug_Click(object sender, EventArgs e) {
            TxtDebugIsFront = !TxtDebugIsFront;
            if (TxtDebugIsFront) {
                this.TxtDebug.BringToFront();
            } else {
                this.TxtDebug.SendToBack();
            }
        }

        /// <summary>
        /// 打出摸牌指令之后摸到的牌
        /// </summary>
        private void LblShowAfterGetOne_Click(object sender, EventArgs e) {
            SendShowCardJson();
            UIInvoke(() => {
                this.PnlAfterGetOne.Visible = false;
            });
        }

        private void LblDonotShowAfterGetOne_Click(object sender, EventArgs e) {
            // 不出牌
            DonotShowCardAfterGetJson();
            // UI 恢复
            UIInvoke(() => {
                SetCardButtonEnable(true);
            });
        }

        public void DebugLog(string v) {
            this.TxtDebug.Text += v + "\r\n";
        }
    }
}
