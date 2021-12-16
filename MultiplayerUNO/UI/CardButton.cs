using MultiplayerUNO.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MultiplayerUNO.UI {

    public class CardButton : Button {

        // 静态常量
        private static Bitmap[] IMAGES = null;
        public static float ScaleRatio = 0.8f; // TODO 和屏幕大小相结合

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
        public bool IsFlipped;

        public CardButton(int cardId, bool back = false) {
            // 越界
            if (cardId >= BACK || cardId < 0) {
                cardId = BACK;
            }

            Card = new Card(cardId);
            IsFlipped = back;

            // Button 设置
            this.Width = (int)((ORI_WIDTH + BoardPixel2) * ScaleRatio);
            this.Height = (int)((ORI_HEIGHT + BoardPixel2) * ScaleRatio);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            SetBackGroundImage();
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