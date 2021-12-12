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
                playerAdapter = new LocalPlayerAdapter(int.Parse(portTextBox.Text), sendTextBox.Text);
                playerAdapter.Initialize();

                runButton.Enabled = false;
                connectButton.Enabled = false;
                sendButton.Enabled = true;

                showInfoThread = new Thread(ShowMsgThread);
                showInfoThread.IsBackground = true; // 后台线程

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
                playerAdapter = new RemotePlayerAdapter(ipInputBox.Text ,int.Parse(portTextBox.Text));
                playerAdapter.Initialize();

                runButton.Enabled = false;
                connectButton.Enabled = false;
                sendButton.Enabled = true;

                showInfoThread = new Thread(ShowMsgThread);
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
