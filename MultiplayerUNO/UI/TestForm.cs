using MultiplayerUNO.UI.Animations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI {
    public partial class TestForm : Form {
        CardButton CBtn = null;

        public TestForm() {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Enter) { return; }
            TextBox txt = sender as TextBox;
            int cardid;
            if (Int32.TryParse(txt.Text, out cardid)) {
                if (CBtn != null) {
                    this.Controls.Remove(CBtn);
                }
                CBtn = new CardButton(cardid);
                CBtn.Location = new Point(0, 0);
                this.Controls.Add(CBtn);
                this.LblInfo.Text =
                    "Number: " + CBtn.Card.Number
                    + "\nColor: " + CBtn.Card.Color;
                ;
                this.LblTestAlign.Text = this.LblInfo.Text;
                this.LblTestAlign.Location = new Point(
                        this.label3.Location.X - this.LblTestAlign.Width / 2,
                        this.label3.Location.Y
                        );
            } else {
                txt.Text = "Invalid";
            }
        }


        private void TestForm_Load(object sender, EventArgs e) {
            // 设置一些全局的相对大小信息
            int h = this.ClientSize.Height;
            int w = this.ClientSize.Width;
            CardButton.ScaleRatio = Math.Min(h / 1152f * 0.8f, w / 2048f * 0.8f);

            this.textBox1.Text = "11";
            textBox1_KeyDown(this.textBox1, new KeyEventArgs(Keys.Enter));

            // 透明度动画 label
            Control c = this.LblAlpha;
            c.Tag = new Bitmap[2] { UIImage.clockwise, UIImage.counterclockwise };
            c.BackgroundImage = UIImage.clockwise;
            c.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void button2_Click(object sender, EventArgs e) {
            this.button2.Click -= this.button1_Click;
            CardButton btn = null;
            foreach (var s in this.Controls) {
                if (s as CardButton != null) {
                    btn = (CardButton)s;
                    break;
                }
            }
            if(btn == null) {
                return;
            }
            AnimationSeq animaSeq = new AnimationSeq();
            Animation anima = new Animation(this, btn);
            anima.SetTranslate(200, 0);
            Animation anima2 = new Animation(this, btn);
            anima2.SetRotate();
            animaSeq.AddAnimation(anima);
            animaSeq.AddAnimation(anima2);
            animaSeq.Run();
        }


        private bool BtnTop = false;
        private void BtnZIndex_Click(object sender, EventArgs e) {
            if (CBtn == null) {
                return;
            }
            BtnTop = !BtnTop;
            if(BtnTop) {
                CBtn.BringToFront();
            } else {
                this.PnlInfo.BringToFront();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (CBtn == null) { return; }
            Animation anima = new Animation(this, CBtn);
            anima.SetTranslate(0, 0);
            anima.Run();
        }

        private bool zero2One = true;
        private void BtnAddAlpha_Click(object sender, EventArgs e) {
            AnimationHighLight anima = new AnimationHighLight(this, this.LblAlpha);
            anima.SetDirection(zero2One);
            anima.SetSteps(100);
            anima.Run();
            zero2One = !zero2One;
        }
    }
}
