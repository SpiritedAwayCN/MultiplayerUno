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
            GameControl.CardsDropped = ArrayList.Synchronized(new ArrayList());
        }

        /// <summary>
        /// 初始化的时候的发牌动画,
        /// 只会在初始化的时候被调用,
        /// 一人发一次的发牌方式
        /// </summary>
        private async Task DistributeCardAtGameStart_OnePersonOneTimeAsync() {
            // TODO
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
            // TODO (magic number)
            const int SIZE = 80;
            const int PADDING = 10;
            const int TOTAL_SIZE = PADDING * 2 + SIZE;
            this.PnlChooseColor.Size = new Size(TOTAL_SIZE * 4, TOTAL_SIZE);

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
                lbl.Size = new Size(SIZE, SIZE);
                this.PnlChooseColor.Controls.Add(lbl);
                lbl.Location = new Point(PADDING + TOTAL_SIZE * i, PADDING);
                lbl.BackgroundImage = img4[i];
                lbl.BackgroundImageLayout = ImageLayout.Stretch;
                lbl.Tag = color4[i];// 使用 Tag 保存按钮对应的颜色
                lbl.Click += (sender, e) => {
                    // 设置 GameControl.InvalidCardToChooseColor
                    GameControl.InvalidCardToChooseColor =
                        (CardColor)(((Label)sender).Tag);
                    // TODO 消失动画
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
                lbl.ForeColor = Color.White;
                lbl.Font = new Font("微软雅黑", 15F, FontStyle.Regular,
                                    GraphicsUnit.Point, ((byte)(134)));
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = player.Name + " (0)";
                lbl.AutoSize = true;
                int xOff = -lbl.Width / 2,
                    yOff = -lbl.Height / 2;
                if (player.IsUpDown) {
                    xOff += (int)(
                        (CardButton.WIDTH_MODIFIED / 2
                            + lbl.Width / 2 + OFFSET_FOR_LBLINFO)
                        * (player.PosX < 0 ? -1 : 1));
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
                this.Controls.Add(lbl);
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
                // color
                UpdateLblColor();
                lblcolor.BackgroundImageLayout = ImageLayout.Stretch;

                this.LblDirection.Visible = true;
                this.LblLeftTime.Visible = true;
                this.LblColor.Visible = true;
                // 开局第一个出才显示谁先出牌
                this.LblFirstShowCard.Visible = GameControl.FirstTurnFirstShow();
                this.PnlNormalShowCardorNot.Visible = (GameControl.TurnID == MyID);

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

            var pos = Piles[PileDropped].Location;
            Control lbl = null;

            // 倒计时 label, LblLeftTime
            lbl = this.LblLeftTime;
            lbl.Location = new Point(20, 20); // TODO

            // 打牌方向 label, LblDirection
            lbl = this.LblDirection;
            // tag 上保存了两张背景图, 0->顺时针, 1->逆时针
            lbl.Tag = new Bitmap[2] { UIImage.clockwise, UIImage.counterclockwise };
            lbl.Location = new Point(100, 20); // TODO

            // 打牌颜色 label
            lbl = this.LblColor;
            lbl.Location = new Point(180, 20); // TODO

            // 第一张牌随便出牌 label, LblFirstShowCard
            lbl = this.LblFirstShowCard;
            lbl.Location = new Point(260, 20); // TODO

            // 选择颜色的 panel, PnlChooseColor
            lbl = this.PnlChooseColor;
            lbl.Location = new Point(20, 100);// TODO
            lbl.Visible = false;

            // 质疑 panel, PnlQuestion
            lbl = this.PnlQuestion;
            lbl.Location = this.PnlChooseColor.Location;// TODO
            lbl.Visible = false;

            // 展示牌 pnl, PnlDisplayCard
            lbl = this.PnlDisplayCard;
            lbl.Location = this.PnlChooseColor.Location;// TODO
            lbl.Visible = false;

            // 游戏结束 lbl, LblGameOver
            lbl = this.LblGameOver;
            lbl.Location = this.PnlChooseColor.Location; // TODO
            lbl.Visible = false;

            // +2 展示 pnl, PnlPlus2
            lbl = this.PnlPlus2;
            lbl.Location = this.PnlChooseColor.Location; // TODO
            lbl.Visible = false;

            // 摸完一张牌之后是否决定出牌的 panel, PnlAfterGetOne
            lbl = this.PnlAfterGetOne;
            lbl.Location = this.PnlChooseColor.Location; // TODO
            lbl.Visible = false;

            // 常规的出牌、摸牌 panel
            lbl = this.PnlNormalShowCardorNot;
            lbl.Location = this.PnlChooseColor.Location; // TODO
            // 看看是不是轮到自己打牌(发牌动画结束之后设置)
            lbl.Visible = false;

            // 展示最终的出牌结果的 panel,
            lbl = this.PnlShowResultWhenGameOver;
            lbl.Location = this.PnlChooseColor.Location; // TODO
            // 看看是不是轮到自己打牌(发牌动画结束之后设置)
            lbl.Visible = false;

            // DEBUG
            lbl = this.TxtDebug;
            lbl.Location = new Point(20, 110 + this.PnlChooseColor.Location.Y);
            lbl.SendToBack();
        }
    }
}
