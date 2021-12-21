namespace MultiplayerUNO.UI {
    partial class MainForm {
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
            this.components = new System.ComponentModel.Container();
            this.LblShowCard = new System.Windows.Forms.Label();
            this.LblGetCard = new System.Windows.Forms.Label();
            this.TmrCheckLeftTime = new System.Windows.Forms.Timer(this.components);
            this.LblLeftTime = new System.Windows.Forms.Label();
            this.LblDirection = new System.Windows.Forms.Label();
            this.LblColor = new System.Windows.Forms.Label();
            this.TmrControlGame = new System.Windows.Forms.Timer(this.components);
            this.LblFirstShowCard = new System.Windows.Forms.Label();
            this.PnlChooseColor = new System.Windows.Forms.Panel();
            this.LblQuestion = new System.Windows.Forms.Label();
            this.PnlQuestion = new System.Windows.Forms.Panel();
            this.LblNoQuestion = new System.Windows.Forms.Label();
            this.PnlDisplayCard = new System.Windows.Forms.Panel();
            this.LblDisplayCardsPlayerName = new System.Windows.Forms.Label();
            this.TmrDisplayCard = new System.Windows.Forms.Timer(this.components);
            this.LblGameOver = new System.Windows.Forms.Label();
            this.PnlPlus2 = new System.Windows.Forms.Panel();
            this.LblPlus2Total = new System.Windows.Forms.Label();
            this.LblDonotPlayPlus2 = new System.Windows.Forms.Label();
            this.LblPlayPlus2 = new System.Windows.Forms.Label();
            this.TxtDebug = new System.Windows.Forms.TextBox();
            this.PnlAfterGetOne = new System.Windows.Forms.Panel();
            this.LblDonotShowAfterGetOne = new System.Windows.Forms.Label();
            this.LblShowAfterGetOne = new System.Windows.Forms.Label();
            this.PnlNormalShowCardorNot = new System.Windows.Forms.Panel();
            this.PnlQuestion.SuspendLayout();
            this.PnlDisplayCard.SuspendLayout();
            this.PnlPlus2.SuspendLayout();
            this.PnlAfterGetOne.SuspendLayout();
            this.PnlNormalShowCardorNot.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblShowCard
            // 
            this.LblShowCard.AutoSize = true;
            this.LblShowCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblShowCard.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblShowCard.Location = new System.Drawing.Point(51, 51);
            this.LblShowCard.Name = "LblShowCard";
            this.LblShowCard.Padding = new System.Windows.Forms.Padding(5);
            this.LblShowCard.Size = new System.Drawing.Size(72, 41);
            this.LblShowCard.TabIndex = 11;
            this.LblShowCard.Text = "出牌";
            this.LblShowCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblShowCard.Click += new System.EventHandler(this.LblShowCard_Click);
            // 
            // LblGetCard
            // 
            this.LblGetCard.AutoSize = true;
            this.LblGetCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblGetCard.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblGetCard.Location = new System.Drawing.Point(219, 51);
            this.LblGetCard.Name = "LblGetCard";
            this.LblGetCard.Padding = new System.Windows.Forms.Padding(5);
            this.LblGetCard.Size = new System.Drawing.Size(72, 41);
            this.LblGetCard.TabIndex = 12;
            this.LblGetCard.Text = "摸牌";
            this.LblGetCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblGetCard.Click += new System.EventHandler(this.LblGetCard_Click);
            // 
            // TmrCheckLeftTime
            // 
            this.TmrCheckLeftTime.Interval = 1000;
            this.TmrCheckLeftTime.Tick += new System.EventHandler(this.TmrCheckLeftTime_Tick);
            // 
            // LblLeftTime
            // 
            this.LblLeftTime.BackColor = System.Drawing.Color.LightCyan;
            this.LblLeftTime.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblLeftTime.Location = new System.Drawing.Point(20, 20);
            this.LblLeftTime.Name = "LblLeftTime";
            this.LblLeftTime.Padding = new System.Windows.Forms.Padding(5);
            this.LblLeftTime.Size = new System.Drawing.Size(60, 60);
            this.LblLeftTime.TabIndex = 14;
            this.LblLeftTime.Text = "60";
            this.LblLeftTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblLeftTime.Visible = false;
            // 
            // LblDirection
            // 
            this.LblDirection.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.LblDirection.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDirection.Image = global::MultiplayerUNO.Properties.Resources.clockwise;
            this.LblDirection.Location = new System.Drawing.Point(98, 20);
            this.LblDirection.Name = "LblDirection";
            this.LblDirection.Padding = new System.Windows.Forms.Padding(5);
            this.LblDirection.Size = new System.Drawing.Size(60, 60);
            this.LblDirection.TabIndex = 15;
            this.LblDirection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblDirection.Visible = false;
            // 
            // LblColor
            // 
            this.LblColor.BackColor = System.Drawing.Color.Black;
            this.LblColor.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblColor.Image = global::MultiplayerUNO.Properties.Resources.clockwise;
            this.LblColor.Location = new System.Drawing.Point(175, 20);
            this.LblColor.Name = "LblColor";
            this.LblColor.Padding = new System.Windows.Forms.Padding(5);
            this.LblColor.Size = new System.Drawing.Size(60, 60);
            this.LblColor.TabIndex = 16;
            this.LblColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblColor.Visible = false;
            // 
            // TmrControlGame
            // 
            this.TmrControlGame.Interval = 500;
            this.TmrControlGame.Tick += new System.EventHandler(this.TmrControlGame_Tick);
            // 
            // LblFirstShowCard
            // 
            this.LblFirstShowCard.AutoSize = true;
            this.LblFirstShowCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblFirstShowCard.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblFirstShowCard.Location = new System.Drawing.Point(549, 30);
            this.LblFirstShowCard.Name = "LblFirstShowCard";
            this.LblFirstShowCard.Padding = new System.Windows.Forms.Padding(5);
            this.LblFirstShowCard.Size = new System.Drawing.Size(192, 41);
            this.LblFirstShowCard.TabIndex = 17;
            this.LblFirstShowCard.Text = "第一张牌随便出";
            this.LblFirstShowCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblFirstShowCard.Visible = false;
            // 
            // PnlChooseColor
            // 
            this.PnlChooseColor.BackColor = System.Drawing.Color.Azure;
            this.PnlChooseColor.Location = new System.Drawing.Point(26, 112);
            this.PnlChooseColor.Name = "PnlChooseColor";
            this.PnlChooseColor.Size = new System.Drawing.Size(311, 95);
            this.PnlChooseColor.TabIndex = 18;
            // 
            // LblQuestion
            // 
            this.LblQuestion.AutoSize = true;
            this.LblQuestion.BackColor = System.Drawing.Color.LightCyan;
            this.LblQuestion.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblQuestion.Location = new System.Drawing.Point(37, 31);
            this.LblQuestion.Name = "LblQuestion";
            this.LblQuestion.Padding = new System.Windows.Forms.Padding(5);
            this.LblQuestion.Size = new System.Drawing.Size(72, 41);
            this.LblQuestion.TabIndex = 20;
            this.LblQuestion.Text = "质疑";
            this.LblQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblQuestion.Click += new System.EventHandler(this.LblQuestion_Click);
            // 
            // PnlQuestion
            // 
            this.PnlQuestion.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlQuestion.Controls.Add(this.LblNoQuestion);
            this.PnlQuestion.Controls.Add(this.LblQuestion);
            this.PnlQuestion.Location = new System.Drawing.Point(779, 20);
            this.PnlQuestion.Name = "PnlQuestion";
            this.PnlQuestion.Size = new System.Drawing.Size(319, 101);
            this.PnlQuestion.TabIndex = 21;
            // 
            // LblNoQuestion
            // 
            this.LblNoQuestion.AutoSize = true;
            this.LblNoQuestion.BackColor = System.Drawing.Color.LightCyan;
            this.LblNoQuestion.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblNoQuestion.Location = new System.Drawing.Point(195, 31);
            this.LblNoQuestion.Name = "LblNoQuestion";
            this.LblNoQuestion.Padding = new System.Windows.Forms.Padding(5);
            this.LblNoQuestion.Size = new System.Drawing.Size(96, 41);
            this.LblNoQuestion.TabIndex = 21;
            this.LblNoQuestion.Text = "不质疑";
            this.LblNoQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblNoQuestion.Click += new System.EventHandler(this.LblNoQuestion_Click);
            // 
            // PnlDisplayCard
            // 
            this.PnlDisplayCard.AutoSize = true;
            this.PnlDisplayCard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PnlDisplayCard.BackColor = System.Drawing.Color.DodgerBlue;
            this.PnlDisplayCard.Controls.Add(this.LblDisplayCardsPlayerName);
            this.PnlDisplayCard.Location = new System.Drawing.Point(392, 167);
            this.PnlDisplayCard.Name = "PnlDisplayCard";
            this.PnlDisplayCard.Size = new System.Drawing.Size(172, 20);
            this.PnlDisplayCard.TabIndex = 22;
            this.PnlDisplayCard.VisibleChanged += new System.EventHandler(this.PnlDisplayCard_VisibleChanged);
            // 
            // LblDisplayCardsPlayerName
            // 
            this.LblDisplayCardsPlayerName.AutoSize = true;
            this.LblDisplayCardsPlayerName.BackColor = System.Drawing.Color.GhostWhite;
            this.LblDisplayCardsPlayerName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDisplayCardsPlayerName.Location = new System.Drawing.Point(0, 0);
            this.LblDisplayCardsPlayerName.Name = "LblDisplayCardsPlayerName";
            this.LblDisplayCardsPlayerName.Size = new System.Drawing.Size(169, 20);
            this.LblDisplayCardsPlayerName.TabIndex = 0;
            this.LblDisplayCardsPlayerName.Text = "展示牌的玩家姓名";
            this.LblDisplayCardsPlayerName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TmrDisplayCard
            // 
            this.TmrDisplayCard.Interval = 5000;
            this.TmrDisplayCard.Tick += new System.EventHandler(this.TmrDisplayCard_Tick);
            // 
            // LblGameOver
            // 
            this.LblGameOver.AutoSize = true;
            this.LblGameOver.BackColor = System.Drawing.Color.LightCyan;
            this.LblGameOver.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblGameOver.Location = new System.Drawing.Point(518, 100);
            this.LblGameOver.Name = "LblGameOver";
            this.LblGameOver.Padding = new System.Windows.Forms.Padding(5);
            this.LblGameOver.Size = new System.Drawing.Size(240, 41);
            this.LblGameOver.TabIndex = 22;
            this.LblGameOver.Text = "游戏结束，获胜者是";
            this.LblGameOver.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlPlus2
            // 
            this.PnlPlus2.AutoSize = true;
            this.PnlPlus2.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlPlus2.Controls.Add(this.LblPlus2Total);
            this.PnlPlus2.Controls.Add(this.LblDonotPlayPlus2);
            this.PnlPlus2.Controls.Add(this.LblPlayPlus2);
            this.PnlPlus2.Location = new System.Drawing.Point(30, 213);
            this.PnlPlus2.Name = "PnlPlus2";
            this.PnlPlus2.Size = new System.Drawing.Size(360, 143);
            this.PnlPlus2.TabIndex = 22;
            // 
            // LblPlus2Total
            // 
            this.LblPlus2Total.AutoSize = true;
            this.LblPlus2Total.BackColor = System.Drawing.Color.LightCyan;
            this.LblPlus2Total.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblPlus2Total.Location = new System.Drawing.Point(40, 9);
            this.LblPlus2Total.Name = "LblPlus2Total";
            this.LblPlus2Total.Padding = new System.Windows.Forms.Padding(5);
            this.LblPlus2Total.Size = new System.Drawing.Size(200, 41);
            this.LblPlus2Total.TabIndex = 22;
            this.LblPlus2Total.Text = "当前+2累计张数";
            this.LblPlus2Total.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblDonotPlayPlus2
            // 
            this.LblDonotPlayPlus2.AutoSize = true;
            this.LblDonotPlayPlus2.BackColor = System.Drawing.Color.LightCyan;
            this.LblDonotPlayPlus2.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDonotPlayPlus2.Location = new System.Drawing.Point(208, 72);
            this.LblDonotPlayPlus2.Name = "LblDonotPlayPlus2";
            this.LblDonotPlayPlus2.Padding = new System.Windows.Forms.Padding(5);
            this.LblDonotPlayPlus2.Size = new System.Drawing.Size(128, 41);
            this.LblDonotPlayPlus2.TabIndex = 21;
            this.LblDonotPlayPlus2.Text = "不打出+2";
            this.LblDonotPlayPlus2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblDonotPlayPlus2.Click += new System.EventHandler(this.LblDonotPlayPlus2_Click);
            // 
            // LblPlayPlus2
            // 
            this.LblPlayPlus2.AutoSize = true;
            this.LblPlayPlus2.BackColor = System.Drawing.Color.LightCyan;
            this.LblPlayPlus2.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblPlayPlus2.Location = new System.Drawing.Point(23, 72);
            this.LblPlayPlus2.Name = "LblPlayPlus2";
            this.LblPlayPlus2.Padding = new System.Windows.Forms.Padding(5);
            this.LblPlayPlus2.Size = new System.Drawing.Size(104, 41);
            this.LblPlayPlus2.TabIndex = 20;
            this.LblPlayPlus2.Text = "打出+2";
            this.LblPlayPlus2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblPlayPlus2.Click += new System.EventHandler(this.LblPlayPlus2_Click);
            // 
            // TxtDebug
            // 
            this.TxtDebug.Location = new System.Drawing.Point(1143, 20);
            this.TxtDebug.Multiline = true;
            this.TxtDebug.Name = "TxtDebug";
            this.TxtDebug.ReadOnly = true;
            this.TxtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtDebug.Size = new System.Drawing.Size(515, 256);
            this.TxtDebug.TabIndex = 24;
            this.TxtDebug.WordWrap = false;
            this.TxtDebug.Click += new System.EventHandler(this.TxtDebug_Click);
            // 
            // PnlAfterGetOne
            // 
            this.PnlAfterGetOne.AutoSize = true;
            this.PnlAfterGetOne.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlAfterGetOne.Controls.Add(this.LblDonotShowAfterGetOne);
            this.PnlAfterGetOne.Controls.Add(this.LblShowAfterGetOne);
            this.PnlAfterGetOne.Location = new System.Drawing.Point(30, 373);
            this.PnlAfterGetOne.Name = "PnlAfterGetOne";
            this.PnlAfterGetOne.Size = new System.Drawing.Size(360, 143);
            this.PnlAfterGetOne.TabIndex = 25;
            // 
            // LblDonotShowAfterGetOne
            // 
            this.LblDonotShowAfterGetOne.AutoSize = true;
            this.LblDonotShowAfterGetOne.BackColor = System.Drawing.Color.LightCyan;
            this.LblDonotShowAfterGetOne.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblDonotShowAfterGetOne.Location = new System.Drawing.Point(213, 52);
            this.LblDonotShowAfterGetOne.Name = "LblDonotShowAfterGetOne";
            this.LblDonotShowAfterGetOne.Padding = new System.Windows.Forms.Padding(5);
            this.LblDonotShowAfterGetOne.Size = new System.Drawing.Size(96, 41);
            this.LblDonotShowAfterGetOne.TabIndex = 21;
            this.LblDonotShowAfterGetOne.Text = "不打出";
            this.LblDonotShowAfterGetOne.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblDonotShowAfterGetOne.Click += new System.EventHandler(this.LblDonotShowAfterGetOne_Click);
            // 
            // LblShowAfterGetOne
            // 
            this.LblShowAfterGetOne.AutoSize = true;
            this.LblShowAfterGetOne.BackColor = System.Drawing.Color.LightCyan;
            this.LblShowAfterGetOne.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblShowAfterGetOne.Location = new System.Drawing.Point(54, 52);
            this.LblShowAfterGetOne.Name = "LblShowAfterGetOne";
            this.LblShowAfterGetOne.Padding = new System.Windows.Forms.Padding(5);
            this.LblShowAfterGetOne.Size = new System.Drawing.Size(72, 41);
            this.LblShowAfterGetOne.TabIndex = 20;
            this.LblShowAfterGetOne.Text = "打出";
            this.LblShowAfterGetOne.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblShowAfterGetOne.Click += new System.EventHandler(this.LblShowAfterGetOne_Click);
            // 
            // PnlNormalShowCardorNot
            // 
            this.PnlNormalShowCardorNot.AutoSize = true;
            this.PnlNormalShowCardorNot.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlNormalShowCardorNot.Controls.Add(this.LblShowCard);
            this.PnlNormalShowCardorNot.Controls.Add(this.LblGetCard);
            this.PnlNormalShowCardorNot.Location = new System.Drawing.Point(586, 248);
            this.PnlNormalShowCardorNot.Name = "PnlNormalShowCardorNot";
            this.PnlNormalShowCardorNot.Size = new System.Drawing.Size(360, 143);
            this.PnlNormalShowCardorNot.TabIndex = 26;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1692, 578);
            this.Controls.Add(this.PnlNormalShowCardorNot);
            this.Controls.Add(this.PnlAfterGetOne);
            this.Controls.Add(this.TxtDebug);
            this.Controls.Add(this.PnlPlus2);
            this.Controls.Add(this.LblGameOver);
            this.Controls.Add(this.PnlDisplayCard);
            this.Controls.Add(this.PnlQuestion);
            this.Controls.Add(this.PnlChooseColor);
            this.Controls.Add(this.LblFirstShowCard);
            this.Controls.Add(this.LblColor);
            this.Controls.Add(this.LblDirection);
            this.Controls.Add(this.LblLeftTime);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "右键切换不同的人";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.PnlQuestion.ResumeLayout(false);
            this.PnlQuestion.PerformLayout();
            this.PnlDisplayCard.ResumeLayout(false);
            this.PnlDisplayCard.PerformLayout();
            this.PnlPlus2.ResumeLayout(false);
            this.PnlPlus2.PerformLayout();
            this.PnlAfterGetOne.ResumeLayout(false);
            this.PnlAfterGetOne.PerformLayout();
            this.PnlNormalShowCardorNot.ResumeLayout(false);
            this.PnlNormalShowCardorNot.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblShowCard;
        private System.Windows.Forms.Label LblGetCard;
        private System.Windows.Forms.Timer TmrCheckLeftTime;
        private System.Windows.Forms.Label LblLeftTime;
        private System.Windows.Forms.Label LblDirection;
        private System.Windows.Forms.Label LblColor;
        private System.Windows.Forms.Timer TmrControlGame;
        private System.Windows.Forms.Label LblFirstShowCard;
        private System.Windows.Forms.Panel PnlChooseColor;
        private System.Windows.Forms.Label LblQuestion;
        private System.Windows.Forms.Panel PnlQuestion;
        private System.Windows.Forms.Label LblNoQuestion;
        private System.Windows.Forms.Panel PnlDisplayCard;
        private System.Windows.Forms.Label LblDisplayCardsPlayerName;
        private System.Windows.Forms.Timer TmrDisplayCard;
        private System.Windows.Forms.Label LblGameOver;
        private System.Windows.Forms.Panel PnlPlus2;
        private System.Windows.Forms.Label LblDonotPlayPlus2;
        private System.Windows.Forms.Label LblPlayPlus2;
        private System.Windows.Forms.Label LblPlus2Total;
        private System.Windows.Forms.TextBox TxtDebug;
        private System.Windows.Forms.Panel PnlAfterGetOne;
        private System.Windows.Forms.Label LblDonotShowAfterGetOne;
        private System.Windows.Forms.Label LblShowAfterGetOne;
        private System.Windows.Forms.Panel PnlNormalShowCardorNot;
    }
}