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
    public static partial class MsgAgency {
        /// <summary>
        /// 登陆窗口
        /// </summary>
        public static LoginForm LoginForm = null;

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