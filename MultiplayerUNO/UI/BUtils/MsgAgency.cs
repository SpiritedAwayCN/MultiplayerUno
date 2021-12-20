using LitJson;
using MultiplayerUNO.UI.OtherForm;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.BUtils {
    /// <summary>
    /// 前后端通信类
    /// </summary>
    public static class MsgAgency {
        /// <summary>
        /// 登陆窗口
        /// </summary>
        public static LoginForm LoginForm = null;

        /// <summary>
        /// 和后端通信的适配器
        /// </summary>
        public static PlayerAdapter PlayerAdapter = null;

        /// <summary>
        /// 协议版本号
        /// </summary>
        public const string ProtocolVersion = "0.0.1";
        /// <summary>
        /// 专门收 server 消息的线程
        /// </summary>
        public static Thread ShowInfoThread = null;

        /// <summary>
        /// 处理消息, 注意这里的消息处理应该是顺序的
        /// </summary>
        public static void WaitForMsgFromBackend() {
            while (true) {
                string msg = TakeAMsg();
                if (msg == null) { break; }
                DealWithMsg(msg);
            }
        }

        /// <summary>
        /// 获取一条消息
        /// </summary>
        private static string TakeAMsg() {
            string msg = null;
            try {
                msg = PlayerAdapter.RecvQueue.Take();
            } catch (InvalidOperationException e) {
                Console.WriteLine(e.Message);
                return msg;
            }
            return msg;
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        private static void DealWithMsg(string msg) {
            Console.WriteLine("UI(ORI): " + msg);
            JsonData json = JsonMapper.ToObject(msg);
            if (json.Keys.Contains("state")) {
                // 游戏开始后的信息
                DealWithMsgAfterGameStart(json);
            } else if (json.Keys.Contains("type")) {
                // 游戏开始前的信息
                DealWithMsgBeforeGameStart(json);
            } else if (json.Keys.Contains("cardpileLeft")) {
                // 游戏开始瞬间的信息(只会发送一次)
                InitializeGame(json);
            } else {
                MessageBox.Show("无法识别的 json, 不包含 type/state!\n" + msg);
            }
        }

        private static void DealWithMsgAfterGameStart(JsonData json) {
            Console.WriteLine("UI(state): " + json.ToJson());
            Console.WriteLine("\tstate: " + json["state"]);
            return;
        }

        private static void DealWithMsgBeforeGameStart(JsonData json) {
            //Console.WriteLine("UI: " + json.ToJson());
            int type = (int)json["type"];
            switch (type) {
                case 0: // 只会在刚进入的时候收到, 初始等待局面
                    InitState(json);
                    break;
                case 1: // 有人开始准备
                case 2: // 有人离开了房间
                case 3: // 有人加入了房间
                case 4: // 有人取消了准备, 变为等待
                    UpdatePlayerState(json);
                    break;
                case 5: // 游戏开始(无事发生, 反正只要收到初始化的 JSON 就说明开始成功了)
                    break;
                case 6: // 游戏开始失败了(无事发生)
                    break;
                default: break;
            }
        }

        /// <summary>
        /// 初始化参数, 打开新界面
        /// </summary>
        private static void InitializeGame(JsonData json) {
            // 解析 Json 构造 MainForm
            MainForm mainForm = new MainForm(json);

            // 关闭窗口
            LoginMainUIInvoke(() => {
                // TODO 释放资源
                LoginForm.Hide();
                mainForm.Show();
            });
        }

        /// <summary>
        /// 封装了复杂的 BeginInvoke(异步)
        /// </summary>
        private static void LoginMainUIInvoke(Action fun) {
            if (LoginForm.InvokeRequired) {
                LoginForm.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }

        /// <summary>
        /// 更新状态
        ///     有人取消了准备 (4)
        ///     有人加入了房间 (3)
        ///     有人离开了房间 (2)
        ///     有人开始准备   (1)
        /// </summary>
        private static void UpdatePlayerState(JsonData json) {
            int type = (int)json["type"];
            int seatID = (int)json["player"]["seatID"];
            PlayerState ps = PlayerState.LEFT;
            if (type == 1) {
                ps = PlayerState.READY;
            } else if (type == 2) {
                ps = PlayerState.LEFT;
            } else if (type == 3 || type == 4) {
                ps = PlayerState.WAIT;
            }
            LoginForm.SetPlayerState(seatID, ps);
        }

        /// <summary>
        /// 初始化等待局面
        /// </summary>
        private static void InitState(JsonData json) {
            var players = json["player"];
            int num = players.Count;
            for (int i = 0; i < num; ++i) {
                var p = players[i];
                PlayerState ps =
                    (bool)(p["isReady"]) ? PlayerState.READY : PlayerState.WAIT;
                LoginForm.SetPlayerState((int)p["seatID"], ps);
            }
            // TODO 
            // 认为马上就能收到一条 type=3 的消息
            // 用于设置 seatID
            string msg = TakeAMsg();
            if (msg == null) {
                MessageBox.Show("初始化的时候未能马上收到一条 type=3 的信息");
                return;
            }
            JsonData js2 = JsonMapper.ToObject(msg);
            if (!js2.Keys.Contains("type") || ((int)js2["type"]) != 3) {
                MessageBox.Show("初始化的时候未能马上收到一条 type=3 的信息");
                return;
            }
            LoginForm.SeatID = (int)js2["player"]["seatID"];
        }
    }
}
