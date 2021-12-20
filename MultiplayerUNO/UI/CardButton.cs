using MultiplayerUNO.UI.Animations;
using MultiplayerUNO.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MultiplayerUNO.UI {

    public class CardButton : Button {

        // 静态常量
        private static Bitmap[] IMAGES = null;
        private static float scaleRatio = 0.8f;
        
        /// <summary>
        /// 修正后的长宽, 随着 ScaleRatio 的修改跟着修改
        /// </summary>
        public static int HEIGHT_MODIFIED = ORI_HEIGHT,
                          WIDTH_MODIFIED = ORI_WIDTH;

        // Tile 类型需要 Border, Stretch 不需要
        public static readonly int BoardPixel = 0;//4;
        public static readonly int BoardPixel2 = BoardPixel * 2;
        public static readonly int TotalCard = 109; // 6 张冗余 + 最后一张为背面
        public static int INVALID_UP = 3, PLUS4_BASE = 104, BACK = 108;

        private static Bitmap IMG_SPRITE = UI.UIImage.cards;
        public static readonly int ORI_HEIGHT = IMG_SPRITE.Height / 5;
        public static readonly int ORI_WIDTH = IMG_SPRITE.Width / 13;
        public static readonly double WIDTH_HEIGHT_RATIO = 1.0 * ORI_WIDTH / ORI_HEIGHT;

        public Card Card { get; }
        public static float ScaleRatio {
            get => scaleRatio;
            set {
                scaleRatio = value;
                HEIGHT_MODIFIED = (int)(ORI_HEIGHT * value);
                WIDTH_MODIFIED = (int)(ORI_WIDTH * value);
            }
        }

        /// <summary>
        /// 是否是背面展示
        /// </summary>
        public bool IsFlipped;

        // 高亮
        public bool IsHighlighted;
        public float HighLightRatio = 0.3f;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cardId">牌对应的数字编码</param>
        /// <param name="back">是否背面展示</param>
        /// <param name="isMine">
        /// 是否是我自己的牌(能否响应出牌),
        /// 当成为弃牌之后就不是我的牌了
        /// </param>
        public CardButton(int cardId, bool back = false, bool isMine = false) {
            // 越界
            if (cardId >= BACK || cardId < 0) {
                cardId = BACK;
            }

            Card = new Card(cardId);
            IsFlipped = back;
            IsHighlighted = false;

            // Button 设置
            this.Width = (int)((ORI_WIDTH + BoardPixel2) * ScaleRatio);
            this.Height = (int)((ORI_HEIGHT + BoardPixel2) * ScaleRatio);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            SetBackGroundImage();

            // 事件
            if (isMine) {
                this.Click += HighLightCard;
            }
        }

        /// <summary>
        /// 点击自己的牌会出现高亮的效果
        /// </summary>
        public void HighLightCard(object sender, EventArgs e) {
            if (!GameControl.GameInitialized) { return; }
            // 是否能够响应
            if (!GameControl.FirstTurn()
                && !Card.CanResponseTo(GameControl.CBtnLast.Card, GameControl.ColorLast)) {
                return;
            }
            int dy = (int)(HighLightRatio * WIDTH_MODIFIED);
            Animation anima = new Animation(GameControl.MainForm, this);

            // 是否出现这样子的情况, 先高亮了某一张牌, 然后去高亮另外一张牌
            // 这个时候需要将原来的牌的高亮状态取消
            if (GameControl.CBtnSelected != null && GameControl.CBtnSelected != this) {
                var t = GameControl.CBtnSelected;
                GameControl.CBtnSelected = null; // 设置为 null, 否则无限递归
                t.InvokeOnClick(t, null);
            }

            // 如果已经高亮
            if (IsHighlighted) {
                anima.SetTranslate(0, dy);
                GameControl.CBtnSelected = null;
            } else {
                anima.SetTranslate(0, -dy);
                GameControl.CBtnSelected = this;
            }
            IsHighlighted = !IsHighlighted;
            anima.Run();
        }

        private void SetBackGroundImage() {
            if (IMAGES == null) {
                InitCardPosInSprite();
            }
            if (IsFlipped) {
                this.BackgroundImage = IMAGES[BACK];
            } else {
                this.BackgroundImage = IMAGES[Card.CardId];
            }
        }

        /// <summary>
        /// 初始化数组 IMAGES(保存所有图片)
        /// </summary>
        private void InitCardPosInSprite() {
            // TODO BB 和 Card 类结合更加紧密一下
            // (1) 计算位置
            Point[] CardPosInSprite = new Point[TotalCard];
            for (int i = INVALID_UP + 1; i < PLUS4_BASE; ++i) {
                //  id: cannot +2      reverse
                // img: cannot reverse +2
                int startx = i >> 3;
                if (startx == 11) { startx = 12; }
                else if (startx == 12) { startx = 11; }
                // color: R Y G B
                //   img: R G B Y
                int starty = i & 0x3;
                if (starty != 0) { starty = ((starty - 1 + 2) % 3) + 1; }
                CardPosInSprite[i] = new Point(startx, starty);
            }

            for (int i = 0; i < 4; ++i) {
                // 万能牌
                CardPosInSprite[INVALID_UP - i] = new Point(0, 4);
                // +4
                CardPosInSprite[PLUS4_BASE + i] = new Point(1, 4);
            }
            // 背面
            CardPosInSprite[BACK] = new Point(2, 4);

            // (2) 读取图片
            IMAGES = new Bitmap[TotalCard];
            // 因为只会运行一次, 暂时没有考虑重复的优化
            for (int i = 0; i < TotalCard; ++i) {
                Bitmap imgDest = new Bitmap(
                    this.Width, this.Height,
                    PixelFormat.Format32bppArgb
                );
                Graphics g = Graphics.FromImage(imgDest);
                Rectangle srcRect = new Rectangle(
                    CardPosInSprite[i].X * ORI_WIDTH - BoardPixel,
                    CardPosInSprite[i].Y * ORI_HEIGHT - BoardPixel,
                    ORI_WIDTH + BoardPixel2,
                    ORI_HEIGHT + BoardPixel2
                );
                Rectangle dstRect =
                    new Rectangle(0, 0, this.Width, this.Height);
                g.DrawImage(IMG_SPRITE, dstRect, srcRect, GraphicsUnit.Pixel);
                IMAGES[i] = imgDest;
                g.Dispose();
            }
        }

        public void Flip() {
            IsFlipped = !IsFlipped;
            SetBackGroundImage();
        }
    }
}