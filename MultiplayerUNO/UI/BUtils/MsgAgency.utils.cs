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
            Console.WriteLine("GET: " + msg);
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
