using MultiplayerUNO.UI.Animations;
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
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            FullScreenDisplay();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            // ECS 退出
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }

        private void FullScreenDisplay() {
            // 隐藏窗口边框
            this.FormBorderStyle = FormBorderStyle.None;
            // 获取屏幕的宽度和高度
            int w = SystemInformation.VirtualScreen.Width;
            int h = SystemInformation.VirtualScreen.Height;
            // 设置最大尺寸和最小尺寸
            this.MaximumSize = new Size(w, h);
            this.MinimumSize = new Size(w, h);
            // 设置窗口位置
            this.Location = new Point(0, 0);
            // 设置窗口大小
            this.Width = w;
            this.Height = h;
            // 置顶显示
            this.TopMost = true;
        }
    }
}
