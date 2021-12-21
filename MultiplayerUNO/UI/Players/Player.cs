using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI.Players {
    public class Player {
        public string Name {
            get;
        }

        /// <summary>
        /// 手牌
        /// </summary>
        public List<int> CardsOrigin = null;  // 别人的牌我们不知道, 设置为 null
                                              // 只会在初始化被用到, 之后都使用 BtnsInHand

        public List<CardButton> BtnsInHand; // 别人的牌我们不知道, 只有一张背面的牌
                                            // Count=1, 而不是null

        /// <summary>
        /// 用于显示一些信息: 用户昵称 + 剩余牌张数
        /// </summary>
        public Label LblInfo;

        /// <summary>
        /// 牌堆整体的一个中心
        /// </summary>
        public Point Center;
        public float PosX, PosY;

        /// <summary>
        /// 牌是怎么排列的
        /// true : 上下排列,
        /// false: 左右排列
        /// </summary>
        public bool IsUpDown;

        /// 游戏参数
        public readonly int PlayerID;
        public volatile int CardsCount;
        public volatile bool IsRobot;

        /// <summary>
        /// posX, posY 指的是 [-1,1]*[-1,1] 之间的索引
        /// </summary>
        public Player(MainForm form, string name, bool isUpDown, float posX, float posY,
                int playerID, int cardsCount, bool isRobot, bool isMe) {
            Name = name;
            IsUpDown = isUpDown;
            if (isMe) {
                CardsOrigin = new List<int>();
            }
            BtnsInHand = new List<CardButton>();
            PosX = posX;
            PosY = posY;
            // center 计算
            int x = (int)((1f + posX * RATE) / 2 * form.REF_WIDTH);
            int y = (int)((1f + posY * RATE) / 2 * form.REF_HEIGHT);
            Center = new Point(x, y);
            PlayerID = playerID;
            CardsCount = cardsCount;
            IsRobot = isRobot;
        }

        public void UpdateInfo() {
            LblInfo.Text =
                (IsRobot ? "(AI)" : "")
                + Name + " (" + CardsCount + ")";
        }

        /// <summary>
        /// 出一张牌, 如果是最后一张返回 true
        /// </summary>
        public bool ShowOneCard() {
            return --CardsCount == 0;
        }

        #region 一些 UI 的常数

        // 放缩大小
        public const float RATE = 0.66f;

        #endregion 一些 UI 的常数

        #region 位置编码方式
        // |    2 3 4    |    2 3    |     2     |           |   1   |  //
        // |  1       5  |  1     4  |  1     3  |  1     2  |       |  //
        // |      0      |     0     |     0     |     0     |   0   |  //

        /// <summary>
        /// 牌的摆放方式, true 表示上下摆放
        /// 二进制编码, 0(false), 1(true)
        /// 用途: 
        ///     1. 牌的摆放方式, 现在不使用了
        ///       (对于其他人的牌, 现在只使用一个背面的牌和一个数字表示)
        ///     2. 名字的位置
        /// </summary>
        public static int[] isUpDownMap_CODE = new int[] {
            0, 0,
            0, 0b110, 0b1010, 0b10010, 0b100010
        };

        // 中心位置,
        // 编码方式 [-1,1]*[-1,1],
        // 左上角 [-1, -1], 右下角 [1, 1]
        public static float[][] posX_CODE = new float[][] {
            null, null,
            new float[2]{ 0,  0},
            new float[3]{ 0, -1f,    1f},
            new float[4]{ 0, -1f,     0,   1f},
            new float[5]{ 0, -1f, -0.5f, 0.5f,   1f},
            new float[6]{ 0, -1f, -0.6f,    0, 0.6f, 1f}
        };
        public static float[][] posY_CODE = new float[][] {
            null, null,
            new float[2]{1f, -1f},
            new float[3]{1f,   0,  0},
            new float[4]{1f,   0, -1f,  0},
            new float[5]{1f,   0, -1f, -1f, 0},
            new float[6]{1f,   0, -1f, -1f, -1f, 0},
        };
        #endregion 位置编码方式
    }
}
