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
            this.LblRefuseToShowCardWhenGet = new System.Windows.Forms.Label();
            this.LblQuestion = new System.Windows.Forms.Label();
            this.PnlQuestion = new System.Windows.Forms.Panel();
            this.LblNoQuestion = new System.Windows.Forms.Label();
            this.PnlQuestion.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblShowCard
            // 
            this.LblShowCard.AutoSize = true;
            this.LblShowCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblShowCard.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblShowCard.Location = new System.Drawing.Point(308, 30);
            this.LblShowCard.Name = "LblShowCard";
            this.LblShowCard.Padding = new System.Windows.Forms.Padding(5);
            this.LblShowCard.Size = new System.Drawing.Size(72, 41);
            this.LblShowCard.TabIndex = 11;
            this.LblShowCard.Text = "出牌";
            this.LblShowCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblShowCard.Visible = false;
            this.LblShowCard.Click += new System.EventHandler(this.LblShowCard_Click);
            // 
            // LblGetCard
            // 
            this.LblGetCard.AutoSize = true;
            this.LblGetCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblGetCard.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblGetCard.Location = new System.Drawing.Point(396, 30);
            this.LblGetCard.Name = "LblGetCard";
            this.LblGetCard.Padding = new System.Windows.Forms.Padding(5);
            this.LblGetCard.Size = new System.Drawing.Size(72, 41);
            this.LblGetCard.TabIndex = 12;
            this.LblGetCard.Text = "摸牌";
            this.LblGetCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblGetCard.Visible = false;
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
            // LblRefuseToShowCardWhenGet
            // 
            this.LblRefuseToShowCardWhenGet.AutoSize = true;
            this.LblRefuseToShowCardWhenGet.BackColor = System.Drawing.Color.LightCyan;
            this.LblRefuseToShowCardWhenGet.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblRefuseToShowCardWhenGet.Location = new System.Drawing.Point(396, 100);
            this.LblRefuseToShowCardWhenGet.Name = "LblRefuseToShowCardWhenGet";
            this.LblRefuseToShowCardWhenGet.Padding = new System.Windows.Forms.Padding(5);
            this.LblRefuseToShowCardWhenGet.Size = new System.Drawing.Size(96, 41);
            this.LblRefuseToShowCardWhenGet.TabIndex = 19;
            this.LblRefuseToShowCardWhenGet.Text = "不出牌";
            this.LblRefuseToShowCardWhenGet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblRefuseToShowCardWhenGet.Visible = false;
            this.LblRefuseToShowCardWhenGet.Click += new System.EventHandler(this.LblRefuseToShowCardWhenGet_Click);
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
            this.LblQuestion.Visible = false;
            // 
            // PnlQuestion
            // 
            this.PnlQuestion.BackColor = System.Drawing.Color.OliveDrab;
            this.PnlQuestion.Controls.Add(this.LblNoQuestion);
            this.PnlQuestion.Controls.Add(this.LblQuestion);
            this.PnlQuestion.Location = new System.Drawing.Point(402, 208);
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
            this.LblNoQuestion.Size = new System.Drawing.Size(72, 41);
            this.LblNoQuestion.TabIndex = 21;
            this.LblNoQuestion.Text = "质疑";
            this.LblNoQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblNoQuestion.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(811, 335);
            this.Controls.Add(this.PnlQuestion);
            this.Controls.Add(this.LblRefuseToShowCardWhenGet);
            this.Controls.Add(this.PnlChooseColor);
            this.Controls.Add(this.LblFirstShowCard);
            this.Controls.Add(this.LblColor);
            this.Controls.Add(this.LblDirection);
            this.Controls.Add(this.LblLeftTime);
            this.Controls.Add(this.LblGetCard);
            this.Controls.Add(this.LblShowCard);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "右键切换不同的人";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.PnlQuestion.ResumeLayout(false);
            this.PnlQuestion.PerformLayout();
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
        private System.Windows.Forms.Label LblRefuseToShowCardWhenGet;
        private System.Windows.Forms.Label LblQuestion;
        private System.Windows.Forms.Panel PnlQuestion;
        private System.Windows.Forms.Label LblNoQuestion;
    }
}