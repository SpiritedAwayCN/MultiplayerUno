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
        /// 登陆窗口
        /// </summary>
        public static LoginForm LoginForm = null;
        
        /// <summary>
        /// 该字段为 true 标识是情况 (1), 在 InitState() 中使用.
        ///   (1) 在刚进入房间的时候, 服务器会给客户端发送两条信息 {type=0,type=3};
        ///   (2) 在主动请求房间信息的时候, 服务器只会发一条信息 {type=0};
        /// </summary>
        public static bool WhenFirstEnterTheRoom = true;

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
                case 6: // 游戏开始失败了
                        // 这个时候可能是前端信息和后端不一致导致的, 
                        // 需要向后端重新发一条消息确认房间信息, 更新界面
                    SendMsgToQueryRoomStateWhenLogin();
                    break;
                default: break;
            }
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
            // 重置后更新
            LoginForm.ResetPlayerState();
            var players = json["player"];
            int num = players.Count;
            for (int i = 0; i < num; ++i) {
                var p = players[i];
                PlayerState ps =
                    (bool)(p["isReady"]) ? PlayerState.READY : PlayerState.WAIT;
                LoginForm.SetPlayerState((int)p["seatID"], ps);
            }

            if (WhenFirstEnterTheRoom) {
                WhenFirstEnterTheRoom = false;
            } else { return; }

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
                MessageBox.Show("初始化的时候未能马上收到一条 type=3 的信息\r\n" + js2.ToJson());
                return;
            }
            LoginForm.SeatID = (int)js2["player"]["seatID"];
        }

        /// <summary>
        /// 给服务器发送一条信息用于确认房间中的等待状态
        /// </summary>
        public static void SendMsgToQueryRoomStateWhenLogin() {
            JsonData json = new JsonData() { ["type"] = 0 };
            PlayerAdapter.SendMsg2Server(json.ToJson());
        }
    }
}