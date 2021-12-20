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
    public static partial class MsgAgency {
        public static MainForm MainForm = null;

        /// <summary>
        /// 初始化参数, 打开新界面
        /// </summary>
        private static void InitializeGame(JsonData json) {
            // 解析 Json 构造 MainForm
            MainForm mainForm = new MainForm(json);

            // 关闭窗口
            LoginMainUIInvoke(() => {
                LoginForm.Hide();
                mainForm.Show();
            });
        }


        /// <summary>
        /// 处理 state 的信息(游戏开始之后从服务端发来的消息)
        /// </summary>
        private static void DealWithMsgAfterGameStart(JsonData json) {
            // For DEBUG
            string msg2Show =
                "UI(state)\r\n" +
                "\tI am " + LoginForm.GetUserName() + "!\r\n"
                + "\t" + json.ToJson() + "\r\n";
            //MessageBox.Show(msg2Show);
            Console.WriteLine(msg2Show);
            int state = (int)json["state"];
            if(state == -1) {
                // TODO 特判
                return;
            }
            TurnInfo turnInfo = new TurnInfo(json);

            switch (state) {
                case 1: // 某人打出了某张牌
                        // 不一定, 可能上一个人没有出牌(GameControl.CardChange 来判断)
                        // TODO 功能牌
                    if (GameControl.CardChange) {
                        SomeBodyShowCard(turnInfo);
                    }
                    break;
                case 2: // 某人摸了一张牌(可能还可以出这张牌)
                    GetACard(turnInfo);
                    break;
                case 3: break;
                case 4: // 某人摸了若干张牌(摸牌之后结束了)
                    SomebodyGetSomeCards(turnInfo);
                    break;
                case 5: // 回应 +4(TODO 一定是自己回应)

                    break;
                case 6: break;
                case 7: break;
                default: break;
            }
            return;
        }

        /// <summary>
        /// 某人摸了好几张牌, 然后结束
        /// </summary>
        private static void SomebodyGetSomeCards(TurnInfo turnInfo) {
            MainForm.GetManyCards(turnInfo);
        }

        /// <summary>
        /// 摸一张牌
        /// </summary>
        private static void GetACard(TurnInfo turnInfo) {
            // TODO 平局不响应?如何实现, 什么意思?

            // 别人摸牌
            if(turnInfo.TurnID != MainForm.MyID) {
                MainForm.GetCardForOtherOne(turnInfo);
                return;
            }
            // 自己摸牌, 是否准备把抓到的牌打出去
            MainForm.GetACardForMe(turnInfo);
        }

        /// <summary>
        /// 某人打出了某张牌
        ///   1. 游戏动画
        ///   2. 其他状态
        ///     (1) 如果是自己打出的牌, 设置 CardButton 的 enable = true
        ///     (2) 如果是别人打出的牌, 而且下家不是自己, 结束响应(无事发生)
        ///     (3) 如果是别人打出的牌, 而且下家是自己, 准备出牌(无事发生)
        /// </summary>
        private static void SomeBodyShowCard(TurnInfo turnInfo) {
            // 1
            MainForm.ShowCard(turnInfo);
            // 2
            if (turnInfo.GetPlayerIndex() == MainForm.ME) {
                // (1)
                MainFormUIInvoke(() => {
                    MainForm.SetCardButtonEnable(true);
                });
            }
            // (2), (3) 无事发生
        }

        /// <summary>
        /// 封装了复杂的 BeginInvoke(异步)
        /// </summary>
        private static void MainFormUIInvoke(Action fun) {
            if (MainForm.InvokeRequired) {
                MainForm.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }
    }
}

/// 连出+2
/// 开始就摸牌