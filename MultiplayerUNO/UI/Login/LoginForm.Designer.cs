namespace MultiplayerUNO.UI.Login {
    partial class LoginForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.BtnClient = new System.Windows.Forms.Button();
            this.BtnServer = new System.Windows.Forms.Button();
            this.GrpUserInfo = new System.Windows.Forms.GroupBox();
            this.BtnJoinGame = new System.Windows.Forms.Button();
            this.BtnRechoose = new System.Windows.Forms.Button();
            this.TxtUserName = new System.Windows.Forms.TextBox();
            this.LblUserName = new System.Windows.Forms.Label();
            this.TxtPort = new System.Windows.Forms.TextBox();
            this.LblPort = new System.Windows.Forms.Label();
            this.TxtHost = new System.Windows.Forms.TextBox();
            this.LblHost = new System.Windows.Forms.Label();
            this.GrpReady = new System.Windows.Forms.GroupBox();
            this.LblReadyInfo = new System.Windows.Forms.Label();
            this.BtnReady = new System.Windows.Forms.Button();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnCancelReady = new System.Windows.Forms.Button();
            this.GrpUserInfo.SuspendLayout();
            this.GrpReady.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnClient
            // 
            this.BtnClient.Location = new System.Drawing.Point(12, 255);
            this.BtnClient.Name = "BtnClient";
            this.BtnClient.Size = new System.Drawing.Size(117, 93);
            this.BtnClient.TabIndex = 3;
            this.BtnClient.Text = "客户端";
            this.BtnClient.UseVisualStyleBackColor = true;
            this.BtnClient.Click += new System.EventHandler(this.BtnClient_Click);
            // 
            // BtnServer
            // 
            this.BtnServer.Location = new System.Drawing.Point(12, 354);
            this.BtnServer.Name = "BtnServer";
            this.BtnServer.Size = new System.Drawing.Size(117, 93);
            this.BtnServer.TabIndex = 2;
            this.BtnServer.Text = "服务器";
            this.BtnServer.UseVisualStyleBackColor = true;
            this.BtnServer.Click += new System.EventHandler(this.BtnServer_Click);
            // 
            // GrpUserInfo
            // 
            this.GrpUserInfo.Controls.Add(this.BtnJoinGame);
            this.GrpUserInfo.Controls.Add(this.BtnRechoose);
            this.GrpUserInfo.Controls.Add(this.TxtUserName);
            this.GrpUserInfo.Controls.Add(this.LblUserName);
            this.GrpUserInfo.Controls.Add(this.TxtPort);
            this.GrpUserInfo.Controls.Add(this.LblPort);
            this.GrpUserInfo.Controls.Add(this.TxtHost);
            this.GrpUserInfo.Controls.Add(this.LblHost);
            this.GrpUserInfo.Location = new System.Drawing.Point(647, 255);
            this.GrpUserInfo.Name = "GrpUserInfo";
            this.GrpUserInfo.Size = new System.Drawing.Size(412, 384);
            this.GrpUserInfo.TabIndex = 4;
            this.GrpUserInfo.TabStop = false;
            this.GrpUserInfo.Text = "请输入你的信息";
            // 
            // BtnJoinGame
            // 
            this.BtnJoinGame.Location = new System.Drawing.Point(39, 297);
            this.BtnJoinGame.Name = "BtnJoinGame";
            this.BtnJoinGame.Size = new System.Drawing.Size(125, 68);
            this.BtnJoinGame.TabIndex = 5;
            this.BtnJoinGame.Text = "加入游戏";
            this.BtnJoinGame.UseVisualStyleBackColor = true;
            this.BtnJoinGame.Click += new System.EventHandler(this.BtnJoinGame_Click);
            // 
            // BtnRechoose
            // 
            this.BtnRechoose.Location = new System.Drawing.Point(245, 297);
            this.BtnRechoose.Name = "BtnRechoose";
            this.BtnRechoose.Size = new System.Drawing.Size(125, 68);
            this.BtnRechoose.TabIndex = 6;
            this.BtnRechoose.Text = "上一步";
            this.BtnRechoose.UseVisualStyleBackColor = true;
            this.BtnRechoose.Click += new System.EventHandler(this.BtnRechoose_Click);
            // 
            // TxtUserName
            // 
            this.TxtUserName.Location = new System.Drawing.Point(187, 201);
            this.TxtUserName.Name = "TxtUserName";
            this.TxtUserName.Size = new System.Drawing.Size(168, 25);
            this.TxtUserName.TabIndex = 5;
            this.TxtUserName.Text = "Alice";
            // 
            // LblUserName
            // 
            this.LblUserName.Location = new System.Drawing.Point(24, 188);
            this.LblUserName.Name = "LblUserName";
            this.LblUserName.Size = new System.Drawing.Size(122, 46);
            this.LblUserName.TabIndex = 4;
            this.LblUserName.Text = "玩家昵称";
            this.LblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtPort
            // 
            this.TxtPort.Location = new System.Drawing.Point(187, 128);
            this.TxtPort.Name = "TxtPort";
            this.TxtPort.Size = new System.Drawing.Size(168, 25);
            this.TxtPort.TabIndex = 3;
            this.TxtPort.Text = "25564";
            // 
            // LblPort
            // 
            this.LblPort.Location = new System.Drawing.Point(24, 115);
            this.LblPort.Name = "LblPort";
            this.LblPort.Size = new System.Drawing.Size(122, 46);
            this.LblPort.TabIndex = 2;
            this.LblPort.Text = "端口信息";
            this.LblPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtHost
            // 
            this.TxtHost.Location = new System.Drawing.Point(187, 60);
            this.TxtHost.Name = "TxtHost";
            this.TxtHost.Size = new System.Drawing.Size(168, 25);
            this.TxtHost.TabIndex = 1;
            this.TxtHost.Text = "127.0.0.1";
            // 
            // LblHost
            // 
            this.LblHost.Location = new System.Drawing.Point(24, 47);
            this.LblHost.Name = "LblHost";
            this.LblHost.Size = new System.Drawing.Size(122, 46);
            this.LblHost.TabIndex = 0;
            this.LblHost.Text = "域名信息";
            this.LblHost.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GrpReady
            // 
            this.GrpReady.Controls.Add(this.BtnCancelReady);
            this.GrpReady.Controls.Add(this.LblReadyInfo);
            this.GrpReady.Controls.Add(this.BtnReady);
            this.GrpReady.Controls.Add(this.BtnStart);
            this.GrpReady.Location = new System.Drawing.Point(210, 33);
            this.GrpReady.Name = "GrpReady";
            this.GrpReady.Size = new System.Drawing.Size(412, 384);
            this.GrpReady.TabIndex = 7;
            this.GrpReady.TabStop = false;
            this.GrpReady.Text = "游戏准备";
            // 
            // LblReadyInfo
            // 
            this.LblReadyInfo.Location = new System.Drawing.Point(79, 62);
            this.LblReadyInfo.Name = "LblReadyInfo";
            this.LblReadyInfo.Size = new System.Drawing.Size(273, 46);
            this.LblReadyInfo.TabIndex = 7;
            this.LblReadyInfo.Text = "0/0 玩家已准备";
            this.LblReadyInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnReady
            // 
            this.BtnReady.Location = new System.Drawing.Point(55, 175);
            this.BtnReady.Name = "BtnReady";
            this.BtnReady.Size = new System.Drawing.Size(125, 68);
            this.BtnReady.TabIndex = 5;
            this.BtnReady.Text = "准备游戏";
            this.BtnReady.UseVisualStyleBackColor = true;
            this.BtnReady.Click += new System.EventHandler(this.BtnReady_Click);
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(144, 276);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(125, 68);
            this.BtnStart.TabIndex = 6;
            this.BtnStart.Text = "开始游戏";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnCancelReady
            // 
            this.BtnCancelReady.Location = new System.Drawing.Point(227, 175);
            this.BtnCancelReady.Name = "BtnCancelReady";
            this.BtnCancelReady.Size = new System.Drawing.Size(125, 68);
            this.BtnCancelReady.TabIndex = 8;
            this.BtnCancelReady.Text = "取消准备";
            this.BtnCancelReady.UseVisualStyleBackColor = true;
            this.BtnCancelReady.Click += new System.EventHandler(this.BtnCancelReady_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.GrpUserInfo);
            this.Controls.Add(this.GrpReady);
            this.Controls.Add(this.BtnClient);
            this.Controls.Add(this.BtnServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.GrpUserInfo.ResumeLayout(false);
            this.GrpUserInfo.PerformLayout();
            this.GrpReady.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnClient;
        private System.Windows.Forms.Button BtnServer;
        private System.Windows.Forms.GroupBox GrpUserInfo;
        private System.Windows.Forms.Label LblHost;
        private System.Windows.Forms.TextBox TxtHost;
        private System.Windows.Forms.TextBox TxtPort;
        private System.Windows.Forms.Label LblPort;
        private System.Windows.Forms.TextBox TxtUserName;
        private System.Windows.Forms.Label LblUserName;
        private System.Windows.Forms.Button BtnJoinGame;
        private System.Windows.Forms.Button BtnRechoose;
        private System.Windows.Forms.GroupBox GrpReady;
        private System.Windows.Forms.Button BtnReady;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Label LblReadyInfo;
        private System.Windows.Forms.Button BtnCancelReady;
    }
}