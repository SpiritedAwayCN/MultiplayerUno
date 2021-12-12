using MultiplayerUNO.Utils;
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

namespace MultiplayerUNO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PlayerAdapter playerAdapter;
        Thread showInfoThread;

        // 开服
        private void runButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 开服需要：端口，(开服的)玩家名称
                playerAdapter = new LocalPlayerAdapter(int.Parse(portTextBox.Text), sendTextBox.Text);
                playerAdapter.Initialize(); // 无论什么adapter都要先初始化

                runButton.Enabled = false;
                connectButton.Enabled = false;
                sendButton.Enabled = true;

                showInfoThread = new Thread(ShowMsgThread); // 专门收server消息的线程，函数在下面
                showInfoThread.IsBackground = true; // 后台线程，主线程退出自己没掉

                showInfoThread.Start();
            }
            catch(Exception expt)
            {
                MessageBox.Show(expt.Message);
                return;
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 连别人服务器需要：ip(可域名)，端口
                playerAdapter = new RemotePlayerAdapter(ipInputBox.Text ,int.Parse(portTextBox.Text));
                playerAdapter.Initialize(); // 无论什么adapter都先初始化
                // 连上后，服务端不会发任何消息，要求先发自己的玩家名称（后面会修改）

                runButton.Enabled = false;
                connectButton.Enabled = false;
                sendButton.Enabled = true;

                showInfoThread = new Thread(ShowMsgThread); // 专门收server消息的线程
                showInfoThread.IsBackground = true; // 后台线程

                showInfoThread.Start();
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message);
                return;
            }
        }

        // 回显
        private void ShowMsgThread()
        {
            // 收到的消息会在playerAdapter.RecvQueue里面，这里只是取出来显示在UI上，主要工作会在这里
            while (true)
            {
                string msg = null;
                try
                {
                    msg = playerAdapter.RecvQueue.Take();
                }catch(InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }

                BeginInvoke(new Action(() =>
                {
                    outputBox.AppendText(msg + "\r\n");
                }));
            }
        }

        // 发送
        private void sendButton_Click(object sender, EventArgs e)
        {
            playerAdapter.SendMsg2Server(sendTextBox.Text);
            sendTextBox.Text = "";
        }
    }
}
