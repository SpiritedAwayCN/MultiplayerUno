using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using MultiplayerUNO.UI.Players;
using static MultiplayerUNO.Utils.Card;
using MultiplayerUNO.Utils;

namespace MultiplayerUNO.UI {
    /// <summary>
    /// 一些参数用于控制游戏(全局变量)
    /// </summary>
    public static class GameControl {
        /// <summary>
        /// true: 选择颜色这个事件是由摸 1 张牌之后出牌触发的
        /// false: 选择颜色这个事件是由正常出牌触发的
        /// </summary>
        public static volatile bool ChooseColorIsTriggerAfterGetOneCard = false;

        /// <summary>
        /// 当出的牌是 +4/万能牌 的时候需要选择颜色
        /// </summary>
        public static CardColor InvalidCardToChooseColor = CardColor.Invalid;

        /// <summary>
        /// 标识发牌动画(可等待)
        /// </summary>
        public static Task FinishDistributeCard = null;

        /// <summary>
        /// 从 player 映射到 UI 上面的玩家编码
        /// </summary>
        public static Dictionary<int, int> PlayerId2PlayerIndex;

        /// <summary>
        /// 标识游戏是否初始化完成
        /// </summary>
        public static volatile bool GameInitialized = false;

        /// <summary>
        /// 游戏运行的 MainForm 主窗口
        /// </summary>
        public static MainForm MainForm = null;

        /// <summary>
        /// 顺时针进行游戏
        /// </summary>
        public static bool DirectionIsClockwise;
        public static void SetGameDirection(int direction) {
            // UI 方向编码:
            //      1(顺时针)
            // 服务器方向编码:
            //      1(逆时针, 编号升序), -1(顺时针, 编号降序)
            // 在这里服务器默认发 1, 我们认为 1 是顺时针
            // !!和上面定义不一致, 但是 UI 保持一致即可!!
            DirectionIsClockwise = (direction == 1);
        }

        /// <summary>
        /// 一轮游戏参数
        /// </summary>
        public static volatile int QueryID, LastCardID, TurnID, TimeForYou = -1;
        public static volatile CardColor LastColor = CardColor.Invalid;
        public static volatile Card LastCard = null;
        /// <summary>
        /// 上一个人是否打牌
        /// </summary>
        public static bool CardChange;

        /// <summary>
        /// 当前自己选中的牌
        /// </summary>
        public static CardButton CBtnSelected = null;

        /// <summary>
        /// 弃牌堆, 保证只有一张弃牌 (线程安全的)
        /// </summary>
        private static ArrayList CardsDropped = ArrayList.Synchronized(new ArrayList());
        /// <summary>
        /// 注意只有动画结束才能添加到弃牌堆中
        /// </summary>
        public static void AddDroppedCard(CardButton cbtn) {
            CardsDropped.Add(cbtn);
            while (CardsDropped.Count > 1) {
                MainForm.Controls.Remove((CardButton)CardsDropped[0]);
                CardsDropped.RemoveAt(0);
            }
        }

        /// <summary>
        /// 一些游戏控件只会被生成一次(在构造函数中生成), 在清除整个桌面的时候需要被保留
        /// </summary>
        public static List<Control> ControlsNeededAtGameStart = new List<Control>();

        public static void AddControlsNeededAtGameStart() {
            MainForm.Controls.AddRange(ControlsNeededAtGameStart.ToArray());
        }

        /// <summary>
        /// 我是开局第一个出牌的
        /// </summary>
        public static bool FirstTurnFirstShow() {
            return FirstTurn() && TurnID == MainForm.MyID;
        }

        /// <summary>
        /// 开局第一次出牌
        /// </summary>
        public static bool FirstTurn() {
            return LastCardID == -1;
        }
    }
}