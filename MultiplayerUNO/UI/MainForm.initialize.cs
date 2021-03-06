using LitJson;
using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.UI.BUtils;
using MultiplayerUNO.UI.Players;
using System;
using System.Collections;
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
        /// 用于初始化的 json 文本
        /// </summary>
        private readonly JsonData InitJsonMsg;

        public MainForm(JsonData jsonMsg) {
            InitializeGlobalControl(); // 必须放在第一句
            // MyID is readonly
            MyID = (int)jsonMsg["yourID"];
            InitJsonMsg = jsonMsg;
            InitializeComponent();
        }

        /// <summary>
        /// 设置一些全局变量, 主要是 MsgAgency 以及 GameControl 中的量
        /// </summary>
        private void InitializeGlobalControl() {
            MsgAgency.MainForm = this;
            GameControl.PlayerId2PlayerIndex = new Dictionary<int, int>();
            GameControl.GameInitialized = false;
            GameControl.CBtnSelected = null;
            GameControl.LastColor = CardColor.Invalid;
            GameControl.CardsDropped = ArrayList.Synchronized(new ArrayList());
        }

        /// <summary>
        /// 初始化的时候的发牌动画,
        /// 只会在初始化的时候被调用,
        /// 一人发一次的发牌方式
        /// </summary>
        private async Task DistributeCardAtGameStart_OnePersonOneTimeAsync() {
            // 1. 表面上发 4 张牌
            AnimationSeq animaSeq = new AnimationSeq();
            // 自己最后发, 方便实现
            List<CardButton> add2FormControl = new List<CardButton>();
            int xSrc = Piles[PileToDistribute].Location.X,
                ySrc = Piles[PileToDistribute].Location.Y;
            for (int i = 0; i < PlayersNumber; ++i) {
                // 几号选手
                int p = (i + 1) % PlayersNumber;
                var player = Players[p];
                // 别人的牌我们不知道
                int cardNumber = (p == ME)
                    ? player.CardsOrigin[player.CardsOrigin.Count - 1]
                    : CardButton.BACK;
                var btn = new CardButton(
                    cardNumber,
                    // 背面
                    true,
                    // 自己的牌
                    p == ME
                );
                btn.Location = Piles[PileToDistribute].Location;
                player.BtnsInHand.Add(btn);
                add2FormControl.Add(btn);
                Animation anima = new Animation(this, btn);
                anima.SetTranslate(
                    player.Center.X - CardButton.WIDTH_MODIFIED / 2 - xSrc,
                    player.Center.Y - CardButton.HEIGHT_MODIFIED / 2 - ySrc
                );
                if (p == ME) {
                    anima.SetRotate();
                }
                animaSeq.AddAnimation(anima);
            }
            // 应该等到添加完毕才能够开始动画
            UIInvokeSync(() => {
                this.Controls.AddRange(add2FormControl.ToArray());
                for (int i = add2FormControl.Count - 1; i >= 0; --i) {
                    add2FormControl[i].BringToFront();
                }
            });
            await animaSeq.Run(); // 同步
            // 牌的数量
            UIInvokeSync(() => {
                foreach (var i in Players) {
                    i.LblInfo.Text = i.Name + " (" + i.CardsCount + ")";
                }
            });

            // 2. 发完之后添加剩余的牌(只有自己的牌)
            add2FormControl.Clear();
            var playerME = Players[ME];
            int dstX = playerME.Center.X - CardButton.WIDTH_MODIFIED / 2,
                dstY = playerME.Center.Y - CardButton.HEIGHT_MODIFIED / 2;
            // 添加剩余的牌
            for (int j = playerME.CardsOrigin.Count - 2; j >= 0; --j) {
                var btn = new CardButton(
                    playerME.CardsOrigin[j],
                    // 别人的手牌为背面
                    false,
                    // 自己的牌
                    true
                );
                btn.Location = new Point(dstX, dstY);
                add2FormControl.Add(btn);
                playerME.BtnsInHand.Add(btn);
            }
            UIInvokeSync(() => {
                foreach (var btn in add2FormControl) {
                    this.Controls.Add(btn);
                    btn.SendToBack();
                }
            });

            // 3. 展开自己的牌
            await ReorganizeMyCardsAsync();

            // 4. 需要随机初始化一张牌吗? 不需要

            // 5. 游戏开始
            GameControl.GameInitialized = true;
        }

        /// <summary>
        /// 初始化 Players
        /// </summary>
        private void InitializePlayers(JsonData jsonMsg) {
            int cardPileLeft = (int)jsonMsg["cardpileLeft"];
            int direction = (int)jsonMsg["direction"];
            GameControl.SetGameDirection(direction);

            TurnInfo turnInfo = new TurnInfo(jsonMsg["turnInfo"]);

            JsonData playerMap = jsonMsg["playerMap"];
            PlayersNumber = playerMap.Count;

            // 找到自己是第几个
            int idxME;
            for (idxME = 0; idxME < PlayersNumber; ++idxME) {
                if ((int)playerMap[idxME]["playerID"] == MyID) { break; }
            }

            // 构造 Players
            Players = new Player[PlayersNumber];
            var posX = Player.posX_CODE[PlayersNumber];
            var posY = Player.posY_CODE[PlayersNumber];
            var isUpDown = Player.isUpDownMap_CODE[PlayersNumber];
            for (int i = 0; i < PlayersNumber; ++i) {
                JsonData p = playerMap[i];
                int idx = (i + PlayersNumber - idxME) % PlayersNumber;
                // {"name":"127.0.0.1","playerID":1,"cardsCount":7,"isRobot":0}
                int playerID = (int)p["playerID"];
                GameControl.PlayerId2PlayerIndex[playerID] = idx;
                Players[idx] = new Player(
                    this,
                    (string)p["name"],
                    (isUpDown & (1 << idx)) != 0,
                    posX[idx], posY[idx],
                    playerID,
                    (int)p["cardsCount"],
                    (int)p["isRobot"] == 1,
                    idx == ME
                );
            }

            // 构造好自己的牌堆
            JsonData piles = playerMap[idxME]["handcards"];
            for (int i = 0; i < piles.Count; ++i) {
                Players[ME].CardsOrigin.Add((int)piles[i]);
            }
        }

        /// <summary>
        /// 构造用于选择颜色的 panel
        /// </summary>
        private void ConstructPnlChooseColor() {
            // 选择颜色的 panel 内部增加 4 个 label, 指代颜色
            const int size = SIGN_LABLE_SIZE;
            const int padding = SIGN_LABLE_PDDING;
            const int totalSize = padding * 2 + size;
            this.PnlChooseColor.Size = new Size(totalSize * 4, totalSize);

            Bitmap[] img4 = new Bitmap[4] {
                UIImage._oButBlue, UIImage._oButGreen,
                UIImage._oButRed, UIImage._oButYellow
            };
            CardColor[] color4 = new CardColor[4] {
                CardColor.Blue, CardColor.Green,
                CardColor.Red, CardColor.Yellow
            };

            for (int i = 0; i < 4; ++i) {
                Control lbl = new Label();
                lbl.AutoSize = false;
                lbl.Size = new Size(size, size);
                this.PnlChooseColor.Controls.Add(lbl);
                lbl.Location = new Point(padding + totalSize * i, padding);
                lbl.BackgroundImage = img4[i];
                lbl.BackgroundImageLayout = ImageLayout.Stretch;
                lbl.Tag = color4[i];// 使用 Tag 保存按钮对应的颜色
                lbl.Click += (sender, e) => {
                    // 设置 GameControl.InvalidCardToChooseColor
                    GameControl.InvalidCardToChooseColor =
                        (CardColor)(((Label)sender).Tag);
                    this.PnlChooseColor.Visible = false;
                    SendShowCardJson(
                        GameControl.ChooseColorIsTriggerAfterGetOneCard);
                };
            }
        }

        /// <summary>
        /// 窗口加载时的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e) {
            ScreenDisplaySetting();
            InitializeAllPlayers();
            GameControl.FinishDistributeCard =
                DistributeCardAtGameStart_OnePersonOneTimeAsync();

            // 等待发牌动画完全结束之后, 设置一些组件
            Task.Run(async () => {
                await GameControl.FinishDistributeCard;
                SettingComponentsAfterDistributeAnima();
            });
        }

        /// <summary>
        /// 绘制玩家位置以及相关信息
        /// </summary>
        private void DrawPlayers() {
            // 绘制 label
            for (int i = 0; i < PlayersNumber; ++i) {
                var player = Players[i];

                var lbl = new Label();
                lbl.Visible = false;
                this.Controls.Add(lbl); // 注意 Autosize 只有在 Add 之后才会生效

                lbl.ForeColor = Color.White;
                lbl.Font = new Font("微软雅黑", 15F, FontStyle.Regular,
                                    GraphicsUnit.Point, ((byte)(134)));
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = player.Name + " (0)";
                lbl.AutoSize = true;
                int xOff = -lbl.Width / 2,
                    yOff = -lbl.Height / 2;
                // 摆放规则 (1): 左右两张牌不太好
                //      *  *  *
                //      3  4  5
                //   *2         6*
                //         1
                //         *
                //if (player.IsUpDown) {
                //    xOff += (int)(
                //        (CardButton.WIDTH_MODIFIED / 2
                //            + lbl.Width / 2 + OFFSET_FOR_LBLINFO)
                //        * (player.PosX < 0 ? -1 : 1));
                //} else {
                //    yOff += (int)(
                //        (CardButton.HEIGHT_MODIFIED / 2
                //            + lbl.Height / 2 + OFFSET_FOR_LBLINFO)
                //        * (player.PosY < 0 ? -1 : 1));
                //}

                // 摆放规则 (2)
                //      *  *  *
                //      3  4  5
                //    2         6
                //    *    1    *
                //         *
                if (player.IsUpDown) {
                    yOff += (int)(
                        (CardButton.HEIGHT_MODIFIED / 2
                            + lbl.Height / 2 + OFFSET_FOR_LBLINFO));
                } else {
                    yOff += (int)(
                        (CardButton.HEIGHT_MODIFIED / 2
                            + lbl.Height / 2 + OFFSET_FOR_LBLINFO)
                        * (player.PosY < 0 ? -1 : 1));
                }
                lbl.Location = new Point(
                    player.Center.X + xOff,
                    player.Center.Y + yOff
                );
                player.LblInfo = lbl;
                lbl.Visible = true;
                lbl.SendToBack(); // 放到最底层
            }
        }

        /// <summary>
        /// 绘制牌堆位置
        /// </summary>
        private void DrawPiles() {
            Piles = new CardButton[2];
            int x = this.REF_WIDTH / 2,
                y = this.REF_HEIGHT / 2;
            for (int i = 0; i < PILES_NUM; ++i) {
                var btn = new CardButton(CardButton.BACK);
                float xOff = (i == PileToDistribute ? -1 : 1)
                    * PILE_OFFSET_RATE * x
                    - btn.Width / 2;
                float yOff = -btn.Height / 2;
                btn.Location = new Point(x + (int)xOff, y + (int)yOff);
                Piles[i] = btn;
                this.Controls.Add(btn);
            }
        }

        /// <summary>
        /// 在初始的发牌动画结束之后设置一些组建的初始属性
        /// </summary>
        private void SettingComponentsAfterDistributeAnima() {
            var lbldir = this.LblDirection as Control;
            var lblcolor = this.LblColor as Control;
            UIInvoke(() => {
                // direction
                lbldir.BackgroundImage = GameControl.DirectionIsClockwise
                    ? UIImage.clockwise : UIImage.counterclockwise;
                lbldir.BackgroundImageLayout = ImageLayout.Stretch;
                lbldir.Visible = true;

                // color
                UpdateLblColor();
                lblcolor.BackgroundImageLayout = ImageLayout.Stretch;
                lblcolor.Visible = true;

                this.LblLeftTime.Location = GetLblLeftTimeLocation();
                this.LblLeftTime.Visible = true;
                // 开局第一个出才显示谁先出牌
                this.LblFirstShowCard.Visible = GameControl.FirstTurnFirstShow();
                SetPnlNormalShowCardorNotVisible(GameControl.TurnID == MyID);

                this.TmrCheckLeftTime.Start();
                this.TmrControlGame.Start();
            });
        }

        /// <summary>
        /// 初始化所有的玩家, 初始化界面
        /// </summary>
        private void InitializeAllPlayers() {
            int w = this.REF_WIDTH,
                h = this.REF_HEIGHT;

            // 1. 构造 players
            InitializePlayers(InitJsonMsg);

            // 1.1 构造好牌堆
            // 上面已经完成

            // 2. 绘制初始 UI
            UIInvoke(DrawOriginScene);
        }

        /// <summary>
        /// 绘制原始的场景
        /// </summary>
        private void DrawOriginScene() {
            DrawPiles();
            DrawPlayers();
            DrawControlsDesignedByDesigner();
        }

        /// <summary>
        /// 屏幕显示的一些设置
        /// </summary>
        private void ScreenDisplaySetting() {
            bool FullScreen = false;
            float segs = 1.5f;

            if (FullScreen) {
                // 隐藏窗口边框
                this.FormBorderStyle = FormBorderStyle.None;
                segs = 1f;
            }

            // 获取屏幕的宽度和高度
            int w = (int)(SystemInformation.VirtualScreen.Width / segs);
            int h = (int)(SystemInformation.VirtualScreen.Height / segs);

            // 设置最大尺寸和最小尺寸
            this.MaximumSize = new Size(w, h);
            this.MinimumSize = new Size(w, h);
            // 设置窗口位置
            this.Location = new Point(0, 0);
            // 设置窗口大小
            this.Width = w;
            this.Height = h;

            // 置顶显示
            //this.TopMost = true;

            // 设置一些全局的相对大小信息
            REF_HEIGHT = this.ClientSize.Height;
            REF_WIDTH = this.ClientSize.Width;
            CardButton.ScaleRatio = Math.Min(h / 1152f * 0.8f, w / 2048f * 0.8f);
        }

        /// <summary>
        /// 设定一些使用可视化界面设计的一些组件(大小、位置、可见性属性)
        /// 初始全部都设置为不可见
        /// </summary>
        private void DrawControlsDesignedByDesigner() {
            ConstructPnlChooseColor();

            // 绘制功能按钮
            DrawButtons();

            // 参数
            int padding = SIGN_LABLE_PDDING,
                lblsize = SIGN_LABLE_SIZE;
            int totalsize = padding * 2 + lblsize;

            // 其他控件
            Control lbl = null;

            #region 3 个长时控件

            // 打牌方向 label, LblDirection
            lbl = this.LblDirection;
            // tag 上保存了两张背景图, 0->顺时针, 1->逆时针
            lbl.Tag = new Bitmap[2] { UIImage.clockwise, UIImage.counterclockwise };
            lbl.Location = new Point(padding, padding);
            lbl.BackColor = Color.Transparent;
            lbl.Visible = false;

            // 打牌颜色 label, LblColor
            lbl = this.LblColor;
            lbl.Location = new Point(padding + totalsize, padding);
            lbl.BackColor = Color.Transparent;
            lbl.Visible = false;

            // 倒计时 label, LblLeftTime
            lbl = this.LblLeftTime;
            lbl.Location = new Point(padding + totalsize * 2, padding);
            lbl.Visible = false;

            #endregion 3 个长时控件

            #region 提示信息 label

            // 这个位置作为下面位置的标杆
            Point loc = new Point(padding, padding + totalsize);
            // 当人数 > 4时会挡住某个玩家, 此时放到左下角
            // 放到右下角也会挡住自己的手牌, 感觉就直接放在左上角吧
            //if (Players.Length > 4) {
            //    var info = Players[1].LblInfo;
            //    int y = info.Location.Y + info.Height + 5; //offset:5
            //    loc = new Point(padding, y);
            //}

            List<Control> lbls = new List<Control>();

            // (1) 用于显示一些信息的 label, LblMsg
            lbl = this.LblMsg;
            lbls.Add(lbl);
            lbl.Location = loc;
            // 剩余显示时间记录在 Tag 上
            lbl.Tag = 0;
            // 显示一会后消失
            lbl.VisibleChanged += (sender, e) => {
                if (this.LblMsg.Visible) {
                    this.LblMsg.Tag = MSG_SHOW_TIME;
                }
            };
            lbl.Visible = false;

            loc.Y += lbl.Height + 5;//offset:5

            // (2) 第一张牌随便出牌 label, LblFirstShowCard
            lbl = this.LblFirstShowCard;
            lbls.Add(lbl);
            lbl.Location = loc;
            lbl.Visible = false;

            // (3) 用于在 Form 中显示游戏结束信息的 label, LblGameOverShowInForm
            lbl = this.LblGameOverShowInForm;
            lbls.Add(lbl);
            lbl.Location = loc;

            // (4) 摸牌累计总数 label, LblPlus2Total
            lbl = this.LblPlus2Total;
            lbls.Add(lbl);
            lbl.Location = loc;

            // (5) 选择颜色的 panel, PnlChooseColor
            lbl = this.PnlChooseColor;
            lbls.Add(lbl);
            lbl.Location = loc;
            // BringToFront() 失败? 应该是动画的问题, 现在暂时把它放在左上角
            //lbl.Location = new Point(
            //    (this.REF_WIDTH - lbl.Width) / 2,
            //    (this.REF_HEIGHT - lbl.Height) / 2
            //);
            //lbl.VisibleChanged += (sender, e) => { (Control(sender)).BringToFront(); };
            lbl.Visible = false;

            foreach (Control control in lbls) {
                control.VisibleChanged +=
                    (sender, e) => { ((Control)sender).BringToFront(); };
            }

            #endregion 提示信息 label

            // TODO 展示牌 panel, PnlDisplayCard(这个功能暂时废弃了)
            lbl = this.PnlDisplayCard;
            lbl.Location = this.PnlChooseColor.Location;
            lbl.Visible = false;

            // 游戏结束展示所有人手牌的 panel, PnlShowResultWhenGameOver
            lbl = this.PnlShowResultWhenGameOver;
            lbl.BackColor = Color.DimGray;
            lbl.Visible = false;

            // 回显收到 JSON 信息的 textbox, TxtDebug
            lbl = this.TxtDebug;
            lbl.Location = new Point(20, 110 + this.PnlChooseColor.Location.Y);
            lbl.SendToBack();
            lbl.Visible = false;
        }

        /// <summary>
        /// 绘制用于点击的 8 个按钮(出牌、摸牌等)
        /// </summary>
        private void DrawButtons() {
            // (1) 背景都是用同样的 icon
            List<Control> lbls = new List<Control>();
            lbls.Add(this.LblGetCard);
            lbls.Add(this.LblShowCard);
            lbls.Add(this.LblPlayPlus2);
            lbls.Add(this.LblDonotPlayPlus2);
            lbls.Add(this.LblShowAfterGetOne);
            lbls.Add(this.LblDonotShowAfterGetOne);
            lbls.Add(this.LblQuestion);
            lbls.Add(this.LblNoQuestion);

            foreach (Control c in lbls) {
                c.BackColor = Color.Transparent;
                c.BackgroundImage = NetImage.RoundedRectangle;
                c.BackgroundImageLayout = ImageLayout.Stretch;
            }

            // [2] panels 一般设置
            //   (1) 背景都修改为 transparent(事实上最终变成了删除 panel)
            //   (2) visible = false
            List<Control> pnls = new List<Control>();
            pnls.Add(this.PnlAfterGetOne);
            pnls.Add(this.PnlQuestion);
            pnls.Add(this.PnlPlus2);
            pnls.Add(this.PnlNormalShowCardorNot);

            foreach (Control c in pnls) {
                // 这样的设置会使得 panel 颜色和父控件一致
                c.BackColor = Color.Transparent;
                // panel 设置为透明的(一个解决方案, 全部置底, 会有闪烁感)
                c.VisibleChanged += (sender, e) => {
                    ((Control)sender).SendToBack();
                };
                // 位置大小统一设置
                c.Size = TWO_BUTTON_PANEL_SIZE;
                // 都是俩 label
                int idx = 0;
                int lblHeight = 0;
                foreach (Control l in c.Controls) {
                    //if (!lbls.Contains(l)) { continue; }
                    l.Location = new Point(
                        (1 + idx) * c.Size.Width / 3 - l.Width / 2,
                        (c.Height - l.Height) / 2
                    );
                    lblHeight = l.Height;
                    ++idx;
                }
                c.Location = new Point(
                    (this.REF_WIDTH - c.Size.Width) / 2,
                    //(this.REF_HEIGHT + CardButton.HEIGHT_MODIFIED) / 2
                    // 里面出牌按钮的最下方应该不能被遮住
                    Players[ME].Center.Y - (int)(
                        (CardButton.HighLightRatio + 0.5f) * CardButton.HEIGHT_MODIFIED
                        + (lblHeight + c.Height) / 2 + 1 // offset:1
                    )
                );

                // 算完了吧, 算完了 panel 就可以再见了
                // 清空 panel, 他们已经没用了
                while (c.Controls.Count > 0) {
                    Control l = c.Controls[0];
                    var loc = l.Location;
                    c.Controls.Remove(l);
                    this.Controls.Add(l);
                    l.Location = new Point(loc.X + c.Location.X,
                                           loc.Y + c.Location.Y);
                }
                this.Controls.Remove(c);
                c.Dispose(); // 注意 panel 被释放了, 后面不应该引用他们
            }
            pnls.Clear();

            SetPnlAfterGetOneVisible(false);
            SetPnlQuestionVisible(false);
            SetPnlPlus2Visible(false);
            SetPnlNormalShowCardorNotVisible(false);

            foreach (Control c in pnls) {
                c.Visible = false;
            }
        }
    }
}
