using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            // TODO (UI DEUBG) TestForm
            //Application.Run(new UI.TestForm());
            Application.Run(new UI.Login.LoginForm());
        }
    }
}