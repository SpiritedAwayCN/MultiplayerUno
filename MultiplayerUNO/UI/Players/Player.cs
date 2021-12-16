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
        public List<int> CardInHand;

        // GUI 相关的信息
        
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

        /// <summary>
        /// posX, posY 指的是 [-1,1]*[-1,1] 之间的索引
        /// </summary>
        public Player(MainForm form, string name, bool isUpDown, float posX, float posY) {
            Name = name;
            IsUpDown = isUpDown;
            CardInHand = new List<int>();
            PosX = posX;
            PosY = posY;
            // center 计算
            int x = (int)((1f + posX * RATE) / 2 * form.REF_WIDTH);
            int y = (int)((1f + posY * RATE) / 2 * form.REF_HEIGHT);
            Center = new Point(x, y);
        }

        public void AddCard(IEnumerable<int> collection, bool noAnimation) {
            foreach(var card in collection) {
                AddCard(card);
            }
        }

        public void AddCard(int cardId) {
            CardInHand.Add(cardId);
        }

        #region 一些 UI 的常数

        // 放缩大小
        public const float RATE = 0.66f;

        #endregion 一些 UI 的常数
        
        #region 位置编码方式
        // |    2 3    |    2 3    |     2     |           |   1   |  //
        // |  1     4  |  1     4  |  1     3  |  1     2  |       |  //
        // |    0 5    |     0     |     0     |     0     |   0   |  //
        // 二进制编码, 0(false), 1(true)
        public static int[] isUpDownMap = new int[] {
            0, 0,
            0, 0b110, 0b1010, 0b10010, 0b010010
        };
        // 中心位置,
        // 编码方式 [-1,1]*[-1,1],
        // 左上角 [-1, -1], 右下角 [1, 1]
        public static float[][] posX = new float[][] {
            null, null,
            new float[2]{   0,   0},
            new float[3]{   0,  -1f,    1f},
            new float[4]{   0,  -1f,     0,   1f},
            new float[5]{   0,  -1f, -0.5f, 0.5f, 1f},
            new float[6]{-0.5f, -1f, -0.5f, 0.5f, 1f, 0.5f}
        };
        public static float[][] posY = new float[][] {
            null, null,
            new float[2]{1f, -1f},
            new float[3]{1f,   0,  0},
            new float[4]{1f,   0, -1f,  0},
            new float[5]{1f,   0, -1f, -1f, 0},
            new float[6]{1f,   0, -1f, -1f, 0, 1f},
        };
        #endregion 位置编码方式
    }
}
