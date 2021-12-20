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

            // TODO BB UI 测试
            //Application.Run(new UI.TestForm());
            //Application.Run(new UI.MainForm(JsonMapper.ToObject(
            //    "{\"cardpileLeft\":94,\"direction\":1,\"turnInfo\":{\"state\":1,\"queryID\":1,\"lastCard\":-1,\"intInfo\":-1,\"turnID\":2,\"time\":44995},\"yourID\":2,\"playerMap\":[{\"name\":\"Alice188\",\"playerID\":1,\"cardsCount\":7,\"isRobot\":0},{\"name\":\"Alice199\",\"playerID\":2,\"cardsCount\":7,\"isRobot\":0,\"handcards\":[57,39,90,31,61,16,104]}]}"
            //)));
            Application.Run(new UI.OtherForm.LoginForm());
        }
    }
}