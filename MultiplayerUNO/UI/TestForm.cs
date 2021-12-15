using MultiplayerUNO.UI.Animations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI {
    public partial class TestForm : Form {
        public TestForm() {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode != Keys.Enter) { return; }
            TextBox txt = sender as TextBox;
            int cardid;
            if (Int32.TryParse(txt.Text, out cardid)) {
                foreach (var s in this.Controls) {
                    if (s as CardButton != null) {
                        this.Controls.Remove((CardButton)s);
                        break;
                    }
                }
                CardButton btn = new CardButton(cardid);
                btn.Location = new Point(0, 0);
                this.Controls.Add(btn);
                this.LblInfo.Text =
                    "Number: " + btn.Card.Number
                    + "\nColor: " + btn.Card.Color;
                ;
            } else {
                txt.Text = "Invalid";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
        }

        private void TestForm_Load(object sender, EventArgs e) {
            this.textBox1.Text = "11";
            textBox1_KeyDown(this.textBox1, new KeyEventArgs(Keys.Enter));
        }

        private void button2_Click(object sender, EventArgs e) {
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
    }
}
