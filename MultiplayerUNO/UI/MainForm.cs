using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using MultiplayerUNO.UI.Players;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// 出牌动画, 某人出什么牌
        /// </summary>
        /// <param name="playerIdx">
        /// 注意这里是用于UI控制的 playerIdx, 而不是 playerID 
        /// </param>
        public void ShowCard(int playerIdx, int lastCardID) {
            // 功能牌, 反转方向
            if (GameControl.LastCard.IsReverse()) {
                // 透明度动画
                UpdateLblDirection();
            }
            CardButton cbtn = null;
            bool win = Players[playerIdx].ShowOneCard(); // 出牌之后更新一些属性
            if (ME == playerIdx) {
                // 如果是自己出牌, 则在牌堆中找到这张牌
                // 认为所有的牌都有不一样的编号(其实一样也没事, 问题不大)
                foreach (CardButton c in Players[ME].BtnsInHand) {
                    if (c.Card.CardId == lastCardID) {
                        cbtn = c;
                        break;
                    }
                }
                // 移除这张牌
                Players[playerIdx].BtnsInHand.Remove(cbtn);
            } else {
                // 如果是别人的牌则需要 new 一张牌
                cbtn = new CardButton(lastCardID, true, false);
                CardButton cbtnBack = Players[playerIdx].BtnsInHand[0] as CardButton;
                // 必须同步, 这里的 Location 设置会影响后面的发牌动作
                UIInvokeSync(() => {
                    this.Controls.Add(cbtn);
                    cbtn.Location = cbtnBack.Location;
                    cbtn.BringToFront();
                    // 注意如果是最后一张牌的话, 我们需要把底牌抹去
                    if (win) {
                        this.Controls.Remove((CardButton)Players[playerIdx].BtnsInHand[0]);
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
            }
            // 如果是自己出牌, 同时修正剩余牌的位置
            // 别人出牌也理一下牌
            Task.Run(async () => { await ReorganizeMyCardsAsync(); });
        }

        /// <summary>
        /// 上家出了+2之后, 选择出不出牌
        /// </summary>
        internal void ShowOrGetAfterPlus2(TurnInfo turnInfo) {
            int num = turnInfo.IntInfo;
            UIInvoke(() => {
                this.LblPlus2Total.Text = "需要摸牌数: " + num;
                SetPnlPlus2Visible(true);
            });
        }

        /// <summary>
        /// 游戏结束
        /// 1. 打牌动画
        /// 2. 显示谁胜利了
        /// 3. 展示手牌
        /// </summary>
        internal void GameOver(TurnInfo turnInfo) {
            // 1. 打牌动画
            int playerIdx = turnInfo.GetPlayerIndex();
            ShowCard(playerIdx, turnInfo.LastCardID); // TODO lastcard 可能不是上一张牌(平局)
            
            // 2. 显示谁胜利了
            string msgGameOver = "";
            int winnerID = turnInfo.TurnID;
            if (winnerID != 0) {
                // 某人获胜
                string playerName = Players[
                    GameControl.PlayerId2PlayerIndex[winnerID]
                ].Name;
                msgGameOver = "游戏结束, 胜利者是: " + playerName;
            } else {
                // 平局
                msgGameOver = "游戏平局";
            }
            UIInvokeSync(() => {
                Label lbl = this.LblGameOver;
                var h = lbl.Height; // 神奇的 autosize 和 height
                lbl.AutoSize = false;
                lbl.Height = h;
                lbl.Text = msgGameOver;
                lbl.Visible = true;
            });

            // 3. 展示手牌
            ShowCardsWhenGameOver(turnInfo);
        }

        private void ShowCardsWhenGameOver(TurnInfo turnInfo) {
            Panel pnl = this.PnlShowResultWhenGameOver;
            // (1) 获取玩家和牌的列表
            JsonData playerCards = turnInfo.JsonMsg["playerCards"];
            int cnt = playerCards.Count;
            string[] name = new string[cnt];
            CardButton[][] cards = new CardButton[cnt][];
            for (int i = 0; i < cnt; ++i) {
                JsonData p = playerCards[i];
                name[i] = (string)p["name"]
                          + ((int)p["isRobot"] == 1 ? "[AI 托管]" : "");
                p = p["handcards"];
                cards[i] = new CardButton[p.Count];
                for (int j = 0; j < p.Count; ++j) {
                    cards[i][j] = new CardButton((int)p[j]);
                }
                name[i] += " (" + p.Count + ")";
            }

            // (2) 构建 Label 和 CardButton
            Label[] lbls = new Label[cnt];
            Size lblSize = new Size(0, 0); // 最大的 label 大小
            for (int i = 0; i < cnt; ++i) {
                Label lbl = new Label();
                lbl.AutoSize = true;
                lbl.Text = name[i];
                lbl.BackColor = Color.Transparent;
                lbl.Font = new Font("微软雅黑", 13.8F,
                    FontStyle.Regular, GraphicsUnit.Point, 134);
                lbls[i] = lbl;
            }
            //float ratio = 0.9f;
            float ratio = 1.0f;
            UIInvokeSync(() => {
                pnl.Size = new Size(
                    (int)(this.REF_WIDTH * ratio), (int)(this.REF_HEIGHT * ratio));
                pnl.Location = new Point(
                    (int)(this.REF_WIDTH * (1 - ratio) / 2),
                    (int)(this.REF_HEIGHT * (1 - ratio) / 2));
                for (int i = 0; i < cnt; ++i) {
                    Label lbl = lbls[i];
                    pnl.Controls.Add(lbl);
                    lblSize.Width = Math.Max(lbl.Size.Width, lblSize.Width);
                }
            });
            lblSize.Height = lbls[0].Height;

            // (3) 放置到 panel 中
            //   [1] 如果是 >3 个人, 则需要分为两列
            //   [2] 如果剩余牌的数目实在太多, 我们就不完全展开
            int padding = 10;
            int startx = padding * 2 + lblSize.Width;
            Point[] startPoint = new Point[cnt];
            Point[] endPoint = new Point[cnt];
            // 每个人的牌堆大小(取整)
            int lengthPerPlayer = pnl.Size.Width / 2 - startx - CardButton.WIDTH_MODIFIED;
            lengthPerPlayer = (lengthPerPlayer / CardButton.WIDTH_MODIFIED)
                                * CardButton.WIDTH_MODIFIED;
            if (cnt <= 3) {
                // 一列
                lengthPerPlayer += pnl.Width / 2;
                for (int i = 0; i < cnt; ++i) {
                    startPoint[i] = new Point(startx,
                        (int)((i + 1) / (cnt + 1.0f) * pnl.Size.Height));
                    endPoint[i] = new Point(startPoint[i].X + lengthPerPlayer,
                        startPoint[i].Y);
                }
            } else {
                // 分两列
                int mod = (cnt + 1) / 2; // [4]=>2, [5,6]=>3
                for (int i = 0; i < cnt; ++i) {
                    startPoint[i] = new Point(
                        (int)(startx + (i / mod) * pnl.Size.Width / 2),
                        (int)((((i + 1) % mod) + 1) / (mod + 1.0f) * pnl.Size.Height));
                    endPoint[i] = new Point(startPoint[i].X + lengthPerPlayer,
                        startPoint[i].Y);
                }
            }

            // (4) 计算 label 位置, panel 可见
            UIInvokeSync(() => {
                for (int i = 0; i < cnt; ++i) {
                    var lbl = lbls[i];
                    int x = startPoint[i].X,
                        y = startPoint[i].Y;
                    lbl.Location = new Point(
                        x - (lbl.Width + lblSize.Width) / 2,
                        y - lbl.Height / 2);
                    foreach (var btn in cards[i]) {
                        pnl.Controls.Add(btn);
                        btn.Location = new Point(x, y - btn.Height / 2);
                    }
                }
                this.TmrCheckLeftTime.Stop(); // 停止 tmr 的愚蠢活动
                pnl.BringToFront();
                pnl.Visible = true;
            });

            // (5) 动画序列
            List<Animation> lst = new List<Animation>();
            bool notok = true;
            int idx = 0;
            while (notok) {
                Animation anima = null;
                notok = false;
                // 计算卡牌位置
                for (int player = 0; player < cnt; ++player) {
                    var p = cards[player];
                    if (idx + 1 < p.Length) { notok = true; }
                    if (idx >= p.Length) { continue; }
                    if (anima == null) {
                        anima = new Animation(this, p[idx]);
                        int x = CardButton.WIDTH_MODIFIED / 2 * (3 + idx);
                        // TODO 垃圾代码到时候优化一下 endPoint 直接使用 maxlength
                        if (x > (endPoint[player].X - startPoint[player].X)) {
                            x = endPoint[player].X - startPoint[player].X;
                        }
                        anima.SetTranslate(x, 0);
                    } else {
                        anima.AddControls(p[idx]);
                    }
                }
                ++idx;
                lst.Add(anima);
            }
            // 动画跑起来
            Task.Run(async () => {
                foreach (var anima in lst) { 
                    UIInvokeSync(() => {
                        foreach (var c in anima.Controls) {
                            c.BringToFront();
                        }
                    });
                    await anima.Run();
                }
            });
        }

        /// <summary>
        /// 回应 +4
        ///   1. 响应 panel 显示出来 
        /// </summary>
        internal void RespondToPlus4(TurnInfo turnInfo) {
            UIInvokeSync(() => {
                SetPnlQuestionVisible(true);
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
            var pos = ((CardButton)Players[ME].BtnsInHand[0]).Location;
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
                await ReorganizeMyCardsAsync();
            });
        }

        /// <summary>
        /// 能够正常出牌
        /// </summary>
        internal void ShowOrGetNormal(TurnInfo turnInfo) {
            UIInvoke(() => {
                SetPnlNormalShowCardorNotVisible(true);
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
            SetPnlNormalShowCardorNotVisible(false);
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
                        CardButton cbtn = Players[playerIdx].BtnsInHand[0] as CardButton;
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
            CardButton cbtn = Players[ME].BtnsInHand[0] as CardButton;
            Card c = new Card(turnInfo.LastCardID);
            bool canResponed = GameControl.FirstTurn() ||
                  c.CanResponseTo(GameControl.LastCard, GameControl.LastColor);
            Task.Run(async () => {
                // (2)
                await t; // 同步
                Players[ME].CardsCount++; // 更新牌
                // 摸完牌之后洗牌
                await ReorganizeMyCardsAsync();
                // (3) 是否准备打牌
                UIInvoke(() => {
                    if (canResponed) {
                        // 可以响应, 选择是否出牌
                        cbtn.PerformClick();
                        // 设置卡牌按钮都无法响应
                        SetCardButtonEnable(false);
                        SetPnlAfterGetOneVisible(true);
                    } else {
                        // 不能响应直接回复不出牌
                        DonotShowCardAfterGetJson();
                        // 更新 info
                    }
                    Players[ME].UpdateInfo();
                });
            });
        }

        private void DonotShowCardAfterGetJson() {
            // 构造 json
            JsonData json = new JsonData() {
                ["state"] = 1,
                ["action"] = 0,
                ["color"] = 0, // 缺了这个好像后端会报错 TODO
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
            var pos = ((CardButton)Players[playerIdx].BtnsInHand[0]).Location; // 第 0 张牌是最右边的
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
                // TODO 这里的动画(暂时不做了, 直接 visible=true)
                GameControl.InvalidCardToChooseColor = CardColor.Invalid;
                UIInvoke(() => {
                    GameControl.ChooseColorIsTriggerAfterGetOneCard = false;
                    this.PnlChooseColor.Visible = true;
                });
            } else {
                SendShowCardJson();
            }
            SetPnlNormalShowCardorNotVisible(false);
        }

        /// <summary>
        /// 构造出牌的 json
        /// </summary>
        /// <param name="afterGetOne">
        /// true 表示是摸到一张牌之后出的, false 表示正常出牌
        /// </param>
        private void SendShowCardJson(bool afterGetOne=false) {
            var btn = GameControl.CBtnSelected;
            // 构造 json
            // state=2: 这个字段无意义
            // state=1: 不是 万能牌/+4, 设置为 -1
            int color = -1;
            if (btn.Card.Color == CardColor.Invalid) {
                color = (int)GameControl.InvalidCardToChooseColor;
            }
            JsonData json = new JsonData() {
                ["state"] = 1,
                ["color"] = color,
                ["queryID"] = GameControl.QueryID
            };
            if (afterGetOne) {
                json["action"] = 1;
            } else {
                json["card"] = btn.Card.CardId;
            }
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
            SetPnlQuestionVisible(false);
        }

        /// <summary>
        /// 不质疑按钮点击事件发生
        /// </summary>
        private void LblNoQuestion_Click(object sender, EventArgs e) {
            SendMsgRespondToPlus4(4);
            SetPnlQuestionVisible(false);
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
            SetPnlPlus2Visible(false);
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
            SetPnlPlus2Visible(false);
        }

        public void SetCardButtonEnable(bool enable) {
            var lst = Players[ME].BtnsInHand;
            for (int i = 0; i < lst.Count; ++i) {
                var cbtn = lst[i] as CardButton;
                cbtn.Enabled = enable;
            }
        }

        /// <summary>
        /// 打出摸牌指令之后摸到的牌
        /// </summary>
        private void LblShowAfterGetOne_Click(object sender, EventArgs e) {
            var btn = GameControl.CBtnSelected;
            if (btn.Card.Color == CardColor.Invalid) {
                // +4/万能牌
                GameControl.InvalidCardToChooseColor = CardColor.Invalid;
                UIInvoke(() => {
                    GameControl.ChooseColorIsTriggerAfterGetOneCard = true;
                    this.PnlChooseColor.Visible = true;
                });
            } else {
                SendShowCardJson(true);
            }
            UIInvoke(() => {
                SetCardButtonEnable(true);
                SetPnlAfterGetOneVisible(false);
            });
        }

        private void LblDonotShowAfterGetOne_Click(object sender, EventArgs e) {
            // 不出牌
            DonotShowCardAfterGetJson();
            // UI 恢复
            UIInvoke(() => {
                SetCardButtonEnable(true);
                SetPnlAfterGetOneVisible(false);
            });
        }

        private void PnlPlus2_VisibleChanged(object sender, EventArgs e) {
            Control c = sender as Control;
            this.LblPlus2Total.Visible = c.Visible;
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

        public void DebugLog(string v) {
            this.TxtDebug.Text += v + "\r\n";
            this.TxtDebug.ScrollToCaret();
        }
    }
}
