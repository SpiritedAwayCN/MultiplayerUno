using LitJson;
using MultiplayerUNO.UI.Login;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.BUtils {
    public static partial class MsgAgency {
        /// <summary>
        /// 协议版本号
        /// </summary>
        public const string ProtocolVersion = "0.0.1";

        /// <summary>
        /// 和后端通信的适配器(只会被设置一次)
        /// </summary>
        public static PlayerAdapter PlayerAdapter = null;

        /// <summary>
        /// 专门收 server 消息的线程(只会被设置一次)
        /// </summary>
        public static Thread ShowInfoThread = null;

        /// <summary>
        /// 是否和服务器保持着连接
        /// </summary>
        public static volatile bool LostConnectionWithServer = true;

        /// <summary>
        /// 处理消息, 注意这里的消息处理应该是顺序的
        /// </summary>
        public static void WaitForMsgFromBackend() {
            while (true) {
                string msg = TakeOneMsg();
                if (msg == null) { break; }
                DealWithMsg(msg);
            }
        }

        /// <summary>
        /// 获取一条消息
        /// </summary>
        private static string TakeOneMsg() {
            string msg = null;
            try {
                msg = PlayerAdapter.RecvQueue.Take();
            } catch (InvalidOperationException e) {
                Console.WriteLine("[UI]: " + e.Message);
                MessageBox.Show(
                    "你似乎已经断开与服务器的网络连接! 建议重启程序!\n"
                    + "(收不到来自服务器的消息)"
                );
            }
            return msg;
        }

        /// <summary>
        /// 向服务器发送一条 JSON 消息, 在这个函数里面会捕获断开连接异常
        /// </summary>
        public static void SendOneJsonDataMsg(JsonData json) {
            try {
                PlayerAdapter.SendMsg2Server(json.ToJson());
            } catch (InvalidOperationException e) {
                Console.WriteLine("[UI]: " + e.Message);
                MessageBox.Show(
                    "你似乎已经断开与服务器的网络连接! 建议重启程序!\n"
                    + "(服务器无法响应你的请求)"
                );
            }
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        private static void DealWithMsg(string msg) {
            // TODO (UI DEUBG) 输出获取到的 JSON 信息
            //Console.WriteLine("GET: " + msg);
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
    }
}
