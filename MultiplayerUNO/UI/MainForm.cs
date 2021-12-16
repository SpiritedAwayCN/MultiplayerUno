using MultiplayerUNO.UI.Animations;
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
        /// 前后端通信的消息队列
        /// </summary>
        public BlockingCollection<BackendFrontInfo> SendQueue, ReceiveQueue;

        /// <summary>
        /// 用于前后端收发消息的线程, 
        /// 不需要发消息的线程, 可以在具体执行操作的线程里面去发
        /// </summary>
        public Thread ReceiveThread;

        /// <summary>
        /// 游戏人数
        /// </summary>
        //public readonly int PlayersNumber;
        public int PlayersNumber;
        public Player[] Players; // 0 号表示自己

        // 牌堆
        private CardButton[] Piles;
        public const int PILES_NUM = 2;
        public const int PileToDistribute = 0;
        public const int PileDropped = 1;

        public MainForm(int players) {
            PlayersNumber = players;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            InitializeVars();
            FullScreenDisplay();
            // 前后端通信线程启动
            // TODO
            //ReceiveThread.Start();
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitializeVars() {
            // TODO 这个初始化不应该在这里进行, 应该是在前后端线程开始的地方
            ReceiveQueue = new BlockingCollection<BackendFrontInfo>();
            SendQueue = new BlockingCollection<BackendFrontInfo>();
            // 前后端通信线程初始化
            ReceiveThread = new Thread(ListenMsg);
        }

        /// <summary>
        /// 监听从后端发过来的消息
        /// </summary>
        private void ListenMsg() {
            while (true) {
                var msg = ReceiveQueue.Take();
                // case 0: 发牌(最常见的信息)
                Task.Run(() => {
                    DistributeCard();
                });
                // case 1: 显示一条消息

                // case 2: 游戏初始化的玩家信息
                // 一组玩家姓名, 第一个是自己的姓名
                // 开房
                string[] tmp = new string[] { "a", "b", "c", "d", "e", "f" };
                InitializeAllPlayers(tmp);
            }
        }

        /// <summary>
        /// 初始化的时候的发牌动画, 只会在初始化的时候被调用
        /// </summary>
        private void DistributeCard() {
            // TODO
        }

        /// <summary>
        /// 初始化所有的玩家, 初始化界面
        /// </summary>
        private void InitializeAllPlayers(string[] name) {
            int w = this.REF_WIDTH,
                h = this.REF_HEIGHT;
            Players = new Player[PlayersNumber];
            // 直接打表好了
            var posX = Player.posX[PlayersNumber];
            var posY = Player.posY[PlayersNumber];
            var isUpDown = Player.isUpDownMap[PlayersNumber];
            for (int i = 0; i < PlayersNumber; ++i) {
                Players[i] = new Player(
                    this,
                    name[i],
                    (isUpDown & (1 << i)) != 0,
                     posX[i], posY[i]
                );
            }
            if (this.InvokeRequired) {
                this.BeginInvoke(new Action(() => {
                    DrawOriginScene();
                }));
            } else {
                DrawOriginScene();
            }
        }

        private void DrawOriginScene() {
            DrawPiles();
            DrawPlayers();
        }

        /// <summary>
        /// 绘制玩家位置以及相关信息
        /// </summary>
        private void DrawPlayers() {
            // TODO 这只是简单测试
            for (int i = 0; i < PlayersNumber; ++i) {
                int id = rdm.Next(CardButton.TotalCard);
                CardButton btn = new CardButton(id);
                float x = Players[i].PosX;
                float y = Players[i].PosY;
                btn.Location = new Point(
                    Players[i].Center.X - btn.Width / 2,
                    Players[i].Center.Y - btn.Height / 2);
                this.Controls.Add(btn);
            }
        }

        /// <summary>
        /// 绘制牌堆位置
        /// </summary>
        private void DrawPiles() {
            Piles = new CardButton[2];
            int x = this.REF_WIDTH / 2,
                y = this.Height / 2;
            for (int i = 0; i < PILES_NUM; ++i) {
                var btn = new CardButton(CardButton.BACK);
                float xOff = (i == PileToDistribute ? -1 : 1)
                    * PILE_OFFSET_RATE * this.REF_WIDTH / 2
                    - btn.Width / 2;
                float yOff = -btn.Height / 2;
                btn.Location = new Point(x + (int)xOff, y + (int)yOff);
                Piles[i] = btn;
                this.Controls.Add(btn);
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            // ECS 退出
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }

        /// <summary>
        /// 全屏显示
        /// </summary>
        private void FullScreenDisplay() {
            bool FullScreen = false;
            float segs = 1.5f;

            if(FullScreen) {
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

        #region 测试函数

        private void MainForm_Click(object sender, EventArgs e) {
            var emouse = e as MouseEventArgs;
            if (emouse.Button == MouseButtons.Right) {
                Test_DifferentPlayers();
            } else if (emouse.Button == MouseButtons.Left) {
                Test_DistributeCard();
            }
        }

        private void Test_DistributeCard() {
            
        }

        private void Test_DifferentPlayers() {
            PlayersNumber++;
            if (PlayersNumber > 6) {
                PlayersNumber = 2;
            }
            string[] tmp = new string[] { "a", "b", "c", "d", "e", "f" };
            List<Control> l = new List<Control>();
            foreach (Control s in this.Controls) {
                if (s as CardButton != null) {
                    l.Add(s);
                }
            }
            foreach (Control s in l) {
                this.Controls.Remove(s);
            }
            InitializeAllPlayers(tmp);
        }

        #endregion 测试函数

        #region 一些 UI 的常数

        // 如下的比例是相对于 [-1,1]*[-1,1] 的

        public const float PILE_OFFSET_RATE = 0.1f;

        #endregion 一些 UI 的常数
    }
}
