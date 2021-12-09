using MultiplayerUNO.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MultiplayerUNO.UI {

    internal class CardButton : Button {

        // 静态常量
        private static Point[] CardPosInSprite = null;

        public static readonly int TotalCard = 109; // 6 张冗余 + 最后一张为背面
        public static int INVALID_UP = 3, PLUS4_BASE = 104, BACK = 108;

        private static Bitmap IMG_SPRITE = UI.UIImage.cards;
        public static readonly int ORI_HEIGHT = IMG_SPRITE.Height / 5;
        public static readonly int ORI_WIDTH = IMG_SPRITE.Width / 13;
        public static readonly double WIDTH_HEIGHT_RATIO = 1.0 * ORI_WIDTH / ORI_HEIGHT;

        private Card Card;

        public CardButton(int cardId) {
            // 越界
            if (cardId >= BACK || cardId < 0) {
                cardId = BACK;
            }

            Card = new Card(cardId);

            // Button 设置
            this.Width = ORI_WIDTH;
            this.Height = ORI_HEIGHT;
            SetBackGroundImage();
        }

        private void SetBackGroundImage() {
            if (CardPosInSprite == null) {
                InitCardPosInSprite();
            }
            Bitmap imgDest =
                new Bitmap(ORI_WIDTH, ORI_HEIGHT, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(imgDest);
            Rectangle rectSource = new Rectangle(
                CardPosInSprite[Card.CardId].X * ORI_WIDTH,
                CardPosInSprite[Card.CardId].Y * ORI_HEIGHT,
                ORI_WIDTH,
                ORI_HEIGHT
            );
            Rectangle rectDest = new Rectangle(0, 0, ORI_WIDTH, ORI_HEIGHT);
            g.DrawImage(IMG_SPRITE, rectDest, rectSource, GraphicsUnit.Pixel);
            this.BackgroundImage = imgDest;
            g.Dispose();
        }

        private void InitCardPosInSprite() {
            // TODO BB 和 Card 类结合更加紧密一下
            CardPosInSprite = new Point[TotalCard];
            for (int i = INVALID_UP + 1; i < PLUS4_BASE; ++i) {
                //  id: cannot +2      reverse
                // img: cannot reverse +2
                int startx = i >> 3;
                if (startx == 11) { startx = 12; }
                if (startx == 12) { startx = 11; }
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
        }
    }
}