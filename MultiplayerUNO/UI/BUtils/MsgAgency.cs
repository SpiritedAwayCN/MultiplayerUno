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
            // DEBUG
            MainForm.UIInvokeSync(() => {
                MainForm.DebugLog("MyID: " + MainForm.MyID + "\r\n" + json.ToJson());
            });
            // DEBUG

            int state = (int)json["state"];
            if(state == -1) {
                ToBeAI((int)json["playerID"]);
                return;
            }
            TurnInfo turnInfo = new TurnInfo(json);

            switch (state) {
                case 1: // 某人打出了某张牌
                        SomeBodyShowCard(turnInfo);
                    break;
                case 2: // 某人摸了一张牌(可能还可以出这张牌)
                    GetACard(turnInfo);
                    break;
                case 3: // +2 累加
                    ResponedToPlus2(turnInfo);
                    break;
                case 4: // 某人摸了若干张牌(摸牌之后结束了)
                    SomebodyGetSomeCards(turnInfo);
                    break;
                case 5: // 回应 +4
                    ResponedToPlus4(turnInfo);
                    break;
                case 6: // 质疑结果展示
                    ShowCardAfterPlus4(turnInfo);
                    break;
                case 7: // 游戏结束, 展示所有人手牌
                    GameOver(turnInfo);
                    break;
                default: break;
            }
            return;
        }

        private static void ResponedToPlus2(TurnInfo turnInfo) {
            // 每个人都会收到 +2 的消息, 只有下家需要回复
            if (turnInfo.TurnID != MainForm.MyID) { return; }
            MainForm.ShowOrGetAfterPlus2(turnInfo);
        }

        /// <summary>
        /// 某个玩家掉线, AI 接管
        /// </summary>
        private static void ToBeAI(int playerID) {
            int playerIdx = GameControl.PlayerId2PlayerIndex[playerID];
            MainForm.Players[playerIdx].IsRobot = true;
            MainFormUIInvoke(()=> {
                MainForm.Players[playerIdx].UpdateInfo();
            });
        }

        /// <summary>
        /// 游戏结束
        /// 1. 显示谁胜利了
        /// 2. 展示手牌
        /// </summary>
        private static void GameOver(TurnInfo turnInfo) {
            MainForm.GameOver(turnInfo);
        }

        /// <summary>
        /// 质疑结果展现, 只需要展示手牌
        /// </summary>
        private static void ShowCardAfterPlus4(TurnInfo turnInfo) {
            MainForm.DisplayOnesCard(turnInfo);
        }

        /// <summary>
        /// 回应 +4, 将含有质疑的两个按钮的 panel 可见即可
        /// </summary>
        private static void ResponedToPlus4(TurnInfo turnInfo) {
            // 首先展示出牌动画
            MainForm.ShowCard(turnInfo);
            // 每个人都会收到 +4 的消息, 只有下家可以质疑
            if (turnInfo.TurnID != MainForm.MyID) { return; }
            MainForm.RespondToPlus4(turnInfo);
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
        ///   2. 其他状态(设置按钮可按 enable = true)
        ///     (1) 如果是自己打出的牌
        ///     (2) 如果是别人打出的牌, 而且下家不是自己, 结束响应
        ///     (3) 如果是别人打出的牌, 而且下家是自己, 准备出牌
        /// </summary>
        private static void SomeBodyShowCard(TurnInfo turnInfo) {
            // TODO 功能牌效果展示
            // ban, reverse

            // 1
            if (GameControl.CardChange) {
                // 不一定, 可能上一个人没有出牌(GameControl.CardChange 来判断)
                MainForm.ShowCard(turnInfo);
            }
            // 2
            // (1), (2), (3)
            MainFormUIInvoke(() => {
                MainForm.SetCardButtonEnable(true);
            });
            // (3)
            if (turnInfo.TurnID == MainForm.MyID) {
                MainForm.ShowOrGetNormal(turnInfo);
            }
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

/// 连出 +2