using MultiplayerUNO.UI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI {
    public partial class MainForm : Form {
        #region 废弃的一些函数
        /// <summary>
        /// 初始化的时候的发牌动画,
        /// 只会在初始化的时候被调用,
        /// 一人一张牌的发牌方式
        /// (动画效果太慢了, 未完全实现)
        /// (可能存在若干 BUG, 都是开发中途丢弃的, 丢弃后不再维护)
        /// </summary>
        private void DistributeCardAtGameStart_OneByOne() {
            AnimationSeq animaSeq = new AnimationSeq();
            int[] cardsNum = new int[PlayersNumber];
            for (int i = 0; i < PlayersNumber; ++i) {
                cardsNum[i] = Players[i].CardsOrigin.Count;
            }
            // 每人一张的发牌, 发牌顺序不重要(牌的顺序、人的顺序)
            // 这里考虑了大家有不一样的牌数目的情况, 但是实际上这种情况不会发生
            List<CardButton> add2FormControl = new List<CardButton>();
            int cnt = PlayersNumber;
            int xSrc = Piles[PileToDistribute].Location.X,
                ySrc = Piles[PileToDistribute].Location.Y;
            while (cnt > 0) {
                for (int i = 0; i < PlayersNumber; ++i) {
                    var player = Players[i];
                    if (cardsNum[i] <= 0) { continue; }
                    int left = --cardsNum[i];
                    if (left <= 0) { cnt--; }
                    var btn = new CardButton(player.CardsOrigin[left]);
                    btn.Location = Piles[PileToDistribute].Location;
                    add2FormControl.Add(btn);
                    //if (i == 0) {
                    // 自己的牌需要翻开, 同时需要展开
                    // TODO
                    //} else {
                    // 别人的牌不需要翻开, 而且只需要叠在一起
                    Animation anima = new Animation(this, btn);
                    anima.SetTranslate(
                        player.Center.X - CardButton.WIDTH_MODIFIED / 2 - xSrc,
                        player.Center.Y - CardButton.HEIGHT_MODIFIED / 2 - ySrc
                    );
                    animaSeq.AddAnimation(anima);
                    //}
                }
            }
            UIInvoke(() => {
                this.Controls.AddRange(add2FormControl.ToArray());
                animaSeq.Run();
            });
        }

        #endregion 废弃的一些函数
    }
}
