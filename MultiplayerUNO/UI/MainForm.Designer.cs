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
            this.LblChooseCard = new System.Windows.Forms.Label();
            this.LblGetCard = new System.Windows.Forms.Label();
            this.TmrCheckLeftTime = new System.Windows.Forms.Timer(this.components);
            this.LblLeftTime = new System.Windows.Forms.Label();
            this.LblDirection = new System.Windows.Forms.Label();
            this.LblColor = new System.Windows.Forms.Label();
            this.TmrControlGame = new System.Windows.Forms.Timer(this.components);
            this.LblFirstShowCard = new System.Windows.Forms.Label();
            this.PnlChooseColor = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // LblChooseCard
            // 
            this.LblChooseCard.AutoSize = true;
            this.LblChooseCard.BackColor = System.Drawing.Color.LightCyan;
            this.LblChooseCard.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblChooseCard.Location = new System.Drawing.Point(308, 30);
            this.LblChooseCard.Name = "LblChooseCard";
            this.LblChooseCard.Padding = new System.Windows.Forms.Padding(5);
            this.LblChooseCard.Size = new System.Drawing.Size(72, 41);
            this.LblChooseCard.TabIndex = 11;
            this.LblChooseCard.Text = "出牌";
            this.LblChooseCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblChooseCard.Visible = false;
            this.LblChooseCard.Click += new System.EventHandler(this.LblChooseCard_Click);
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
            this.PnlChooseColor.Location = new System.Drawing.Point(69, 183);
            this.PnlChooseColor.Name = "PnlChooseColor";
            this.PnlChooseColor.Size = new System.Drawing.Size(311, 95);
            this.PnlChooseColor.TabIndex = 18;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(811, 335);
            this.Controls.Add(this.PnlChooseColor);
            this.Controls.Add(this.LblFirstShowCard);
            this.Controls.Add(this.LblColor);
            this.Controls.Add(this.LblDirection);
            this.Controls.Add(this.LblLeftTime);
            this.Controls.Add(this.LblGetCard);
            this.Controls.Add(this.LblChooseCard);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "右键切换不同的人";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Click += new System.EventHandler(this.MainForm_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblChooseCard;
        private System.Windows.Forms.Label LblGetCard;
        private System.Windows.Forms.Timer TmrCheckLeftTime;
        private System.Windows.Forms.Label LblLeftTime;
        private System.Windows.Forms.Label LblDirection;
        private System.Windows.Forms.Label LblColor;
        private System.Windows.Forms.Timer TmrControlGame;
        private System.Windows.Forms.Label LblFirstShowCard;
        private System.Windows.Forms.Panel PnlChooseColor;
    }
}