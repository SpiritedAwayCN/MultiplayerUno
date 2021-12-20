using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO.UI {
    public partial class MainForm : Form {

        #region 测试函数

        private void TEST_StartGame() {
            //// 初始绘制场景
            //string[] tmp = new string[] { "a", "b", "c", "d", "e", "f" };
            //InitializeAllPlayers(tmp);
            //// 发牌
            //DistributeCardAtGameStart_OnePersonOneTimeAsync();
        }

        private void MainForm_Click(object sender, EventArgs e) {
            var emouse = e as MouseEventArgs;
            //if (emouse.Button == MouseButtons.Right) {
            //    Test_DifferentPlayers();
            //} else if (emouse.Button == MouseButtons.Left) {
            //    TEST_StartGame();
            //}
        }

        private void Test_DifferentPlayers() {
            //PlayersNumber++;
            //if (PlayersNumber > 6) {
            //    PlayersNumber = 2;
            //}
            //this.Controls.Clear();
            //string[] tmp = new string[] { "a", "b", "c", "d", "e", "f" };
            //InitializeAllPlayers(tmp);
        }

        #endregion 测试函数
    }
}
