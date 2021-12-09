using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            if(Int32.TryParse(txt.Text,out cardid)) {
                foreach(var s in this.Controls) {
                    if(s as CardButton != null) {
                        this.Controls.Remove((CardButton)s);
                        break;
                    }
                }
                Button btn = new CardButton(cardid);
                btn.Location = new Point(0, 0);
                this.Controls.Add(btn);
            } else {
                txt.Text = "Invalid";
            }
        }
    }
}
