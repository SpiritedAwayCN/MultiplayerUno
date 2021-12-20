using MultiplayerUNO.UI.Players;
using MultiplayerUNO.UI.BUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MultiplayerUNO.Utils;
using LitJson;
using System.Collections;
using System.Runtime.CompilerServices;

namespace MultiplayerUNO.UI.OtherForm {
    public partial class LoginForm : Form {
        private Random rdm = new Random();
        private ArrayList Players = ArrayList.Synchronized(new ArrayList());
        public volatile int SeatID = 0;

        public LoginForm() {
            InitializeComponent();
            MsgAgency.LoginForm = this;
        }

        private void BtnServer_Click(object sender, EventArgs e) {
            SCSelect.PlayerKind = PlayerKind.Server;
            InputGameInfo();
        }

        private void BtnClient_Click(object sender, EventArgs e) {
            SCSelect.PlayerKind = PlayerKind.Client;
            InputGameInfo();
        }

        private void BtnRechoose_Click(object sender, EventArgs e) {
            UIInvoke(() => {
                this.GrpUserInfo.Hide();
                this.BtnClient.Show();
                this.BtnServer.Show();
            });
        }

        private void InputGameInfo() {
            UIInvoke(() => {
                this.BtnClient.Hide();
                this.BtnServer.Hide();
                // 服务器和客户端显示不一样的字
                this.LblHost.Text = SCSelect.HostInfo;
                this.LblPort.Text = SCSelect.PortInfo;

                // TODO 去掉
                // SCSelect.DEFAULT_NAME 
                this.TxtUserName.Text =
                (SCSelect.PlayerKind == PlayerKind.Client ? "Client" : "Server")
                + BUtil.RandomNumberString(3);

                this.GrpUserInfo.Show();

            });
        }

        private void LoginForm_Load(object sender, EventArgs e) {
            UIInvoke(() => {
                InitGUIPosition();
                this.GrpUserInfo.Hide();
                this.GrpReady.Hide();
                // 设置默认值
                this.TxtHost.Text = SCSelect.DEFAULT_HOST;
                this.TxtPort.Text = SCSelect.DEFAULT_PORT;

                this.TxtUserName.Text =
                    SCSelect.DEFAULT_NAME + BUtil.RandomNumberString(3);
            });
        }

        /// <summary>
        /// 封装了复杂的 BeginInvoke(异步)
        /// </summary>
        private void UIInvoke(Action fun) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }

        /// <summary>
        /// 初始化组件的位置
        /// </summary>
        private void InitGUIPosition() {
            SetPositionRelativeToTheFormCenter(this.BtnServer, 0.2f);
            SetPositionRelativeToTheFormCenter(this.BtnClient, -0.2f);
            SetPositionRelativeToTheFormCenter(this.GrpUserInfo);
            SetPositionRelativeToTheFormCenter(this.GrpReady);
        }

        /// <summary>
        /// 将一个组件放置在 Form 的中心位置, 可以设置偏移量,
        /// 偏移量的比例是相对于整个 Form 而言
        /// </summary>
        private void SetPositionRelativeToTheFormCenter(Control control,
            float xOffsetRatio = 0, float yOffsetRatio = 0) {
            int w = this.ClientSize.Width,
                h = this.ClientSize.Height;
            int x = (w - control.Size.Width) / 2,
                y = (h - control.Size.Height) / 2;
            x += (int)(xOffsetRatio * w);
            y += (int)(yOffsetRatio * h);
            control.Location = new Point(x, y);
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        private void BtnJoinGame_Click(object sender, EventArgs e) {
            SetAllControlsEnable(false);
            Task.Run(() => { InitializeAdapter(); });
        }

        /// <summary>
        /// 初始化用于前后端通信、网络通信的相关组件
        /// </summary>
        private void InitializeAdapter() {
            SCSelect.UserHost = this.TxtHost.Text;
            SCSelect.UserPort = this.TxtPort.Text;
            SCSelect.UserName = this.TxtUserName.Text;

            // 头信息
            JsonData header = new JsonData();
            header["version"] = MsgAgency.ProtocolVersion;
            header["name"] = SCSelect.UserName;
            try {
                if (SCSelect.PlayerKind == PlayerKind.Server) {
                    // 开服需要: 端口, (开服的)玩家名称
                    MsgAgency.PlayerAdapter = new LocalPlayerAdapter(
                        int.Parse(SCSelect.UserPort), header.ToJson());
                } else {
                    // 连别人服务器需要: ip(可域名), 端口
                    MsgAgency.PlayerAdapter = new RemotePlayerAdapter(
                        TxtHost.Text, int.Parse(TxtPort.Text));
                }
                // 无论什么 adapter 都要先初始化
                MsgAgency.PlayerAdapter.Initialize();

                // 准备按钮
                MsgAgency.ShowInfoThread = new Thread(MsgAgency.WaitForMsgFromBackend);
                MsgAgency.ShowInfoThread.IsBackground = true; // 后台线程
                MsgAgency.ShowInfoThread.Start();
            } catch (Exception e) {
                MessageBox.Show(e.Message);
                // TODO 返回到上一个界面
                return;
            } finally {
                UIInvoke(() => { SetAllControlsEnable(true); });
            }
            // 成功加入游戏 / 成功开服
            UIInvoke(() => {
                // GUI 配置
                this.GrpUserInfo.Hide();
                this.GrpReady.Show();
                // 当准备人数和总人数相等的时候设置为 true
                this.BtnStart.Enabled = false;
                this.BtnCancelReady.Enabled = false;
            });
            // 此时应该还要发送自己的信息
            JsonData msg = new JsonData();
            msg["version"] = MsgAgency.ProtocolVersion;
            msg["name"] = SCSelect.UserName;
            MsgAgency.PlayerAdapter.SendMsg2Server(msg.ToJson());
            // TODO 
            // 我看服务器端开始的时候不会给自己发自己的消息
            // 所以这里如下处理
            if (SCSelect.PlayerKind == PlayerKind.Server) {
                SeatID = 0;
                SetPlayerState(0, PlayerState.WAIT);
                UpdateReadyInfo();
            }
        }

        /// <summary>
        /// 设置玩家的状态(加锁的)
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetPlayerState(int seatID, PlayerState ps) {
            int seatID1 = seatID + 1;
            while (seatID1 >= Players.Count) {
                Players.Add(PlayerState.LEFT);
            }
            Players[seatID] = ps;
            UpdateReadyInfo();
        }

        /// <summary>
        /// 更新现在有多少人在准备的消息提示
        /// </summary>
        public void UpdateReadyInfo() {
            int ready = 0, total = 0;
            foreach (PlayerState ps in Players) {
                if (ps == PlayerState.WAIT) {
                    ++total;
                } else if (ps == PlayerState.READY) {
                    ++ready; ++total;
                }
            }
            UIInvoke(() => {
                this.LblReadyInfo.Text = GetReadyInfo(ready, total);
                bool enable = (ready == total) &&
                    (total >= Backend.Room.MinPlayerNumber
                        && total <= Backend.Room.MaxPlayerNumber);
                this.BtnStart.Enabled = enable;
            });
        }

        /// <summary>
        /// 获取一个字符串("x/x 玩家已准备")
        /// </summary>
        private string GetReadyInfo(int ready, int total) {
            return ready.ToString()
                + "/" + total.ToString()
                + " 玩家已准备";
        }

        /// <summary>
        /// 设置控件的 Enable
        /// </summary>
        private void SetAllControlsEnable(bool enable) {
            foreach (Control c in this.Controls) {
                c.Enabled = enable;
            }
        }

        private void BtnReady_Click(object sender, EventArgs e) {
            JsonData msg = new JsonData() {
                ["type"] = 1
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(msg.ToJson());
            SetReadyBtnsEnable(false);
        }

        /// <summary>
        /// 设置 BtnReady, BtnCancelReady 的 Enable
        /// </summary>
        private void SetReadyBtnsEnable(bool canReady) {
            this.BtnReady.Enabled = canReady;
            this.BtnCancelReady.Enabled = !canReady;
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void BtnStart_Click(object sender, EventArgs e) {
            JsonData msg = new JsonData() {
                ["type"] = 3
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(msg.ToJson());
        }

        private void BtnCancelReady_Click(object sender, EventArgs e) {
            JsonData msg = new JsonData() {
                ["type"] = 2
            };
            MsgAgency.PlayerAdapter.SendMsg2Server(msg.ToJson());
            SetReadyBtnsEnable(true);
        }


        // For DEBUG
        public string GetUserName() {
            return this.TxtUserName.Text;
        }
    }
}
