namespace MultiplayerUNO.UI {
    partial class TestForm {
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
            this.label1 = new System.Windows.Forms.Label();
            this.PnlInfo = new System.Windows.Forms.Panel();
            this.LblInfo = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.BtnZIndex = new System.Windows.Forms.Button();
            this.LblTestAlign = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.LblChooseCard = new System.Windows.Forms.Label();
            this.BtnAddAlpha = new System.Windows.Forms.Button();
            this.LblAlpha = new System.Windows.Forms.Label();
            this.PnlInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "请输入 cardid";
            // 
            // PnlInfo
            // 
            this.PnlInfo.Controls.Add(this.LblInfo);
            this.PnlInfo.Controls.Add(this.label1);
            this.PnlInfo.Controls.Add(this.textBox1);
            this.PnlInfo.Location = new System.Drawing.Point(520, 12);
            this.PnlInfo.Name = "PnlInfo";
            this.PnlInfo.Size = new System.Drawing.Size(387, 211);
            this.PnlInfo.TabIndex = 3;
            // 
            // LblInfo
            // 
            this.LblInfo.AutoSize = true;
            this.LblInfo.Location = new System.Drawing.Point(52, 137);
            this.LblInfo.Name = "LblInfo";
            this.LblInfo.Size = new System.Drawing.Size(55, 15);
            this.LblInfo.TabIndex = 3;
            this.LblInfo.Text = "label2";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(164, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 25);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(702, 262);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(119, 67);
            this.button2.TabIndex = 5;
            this.button2.Text = "加移动";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // BtnZIndex
            // 
            this.BtnZIndex.Location = new System.Drawing.Point(849, 262);
            this.BtnZIndex.Name = "BtnZIndex";
            this.BtnZIndex.Size = new System.Drawing.Size(119, 69);
            this.BtnZIndex.TabIndex = 6;
            this.BtnZIndex.Text = "改变层级";
            this.BtnZIndex.UseVisualStyleBackColor = true;
            this.BtnZIndex.Click += new System.EventHandler(this.BtnZIndex_Click);
            // 
            // LblTestAlign
            // 
            this.LblTestAlign.AutoSize = true;
            this.LblTestAlign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblTestAlign.Location = new System.Drawing.Point(400, 300);
            this.LblTestAlign.Name = "LblTestAlign";
            this.LblTestAlign.Size = new System.Drawing.Size(57, 17);
            this.LblTestAlign.TabIndex = 7;
            this.LblTestAlign.Text = "label2";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(400, 300);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 100);
            this.label3.TabIndex = 8;
            this.label3.Text = "label3";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(543, 449);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 66);
            this.button1.TabIndex = 9;
            this.button1.Text = "移动 (0,0)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LblChooseCard
            // 
            this.LblChooseCard.AutoSize = true;
            this.LblChooseCard.Location = new System.Drawing.Point(139, 237);
            this.LblChooseCard.Name = "LblChooseCard";
            this.LblChooseCard.Size = new System.Drawing.Size(37, 15);
            this.LblChooseCard.TabIndex = 10;
            this.LblChooseCard.Text = "出牌";
            // 
            // BtnAddAlpha
            // 
            this.BtnAddAlpha.Location = new System.Drawing.Point(702, 369);
            this.BtnAddAlpha.Name = "BtnAddAlpha";
            this.BtnAddAlpha.Size = new System.Drawing.Size(119, 67);
            this.BtnAddAlpha.TabIndex = 11;
            this.BtnAddAlpha.Text = "加透明度变化";
            this.BtnAddAlpha.UseVisualStyleBackColor = true;
            this.BtnAddAlpha.Click += new System.EventHandler(this.BtnAddAlpha_Click);
            // 
            // LblAlpha
            // 
            this.LblAlpha.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.LblAlpha.Location = new System.Drawing.Point(80, 384);
            this.LblAlpha.Name = "LblAlpha";
            this.LblAlpha.Size = new System.Drawing.Size(100, 100);
            this.LblAlpha.TabIndex = 12;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 619);
            this.Controls.Add(this.LblAlpha);
            this.Controls.Add(this.BtnAddAlpha);
            this.Controls.Add(this.LblChooseCard);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LblTestAlign);
            this.Controls.Add(this.BtnZIndex);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.PnlInfo);
            this.Controls.Add(this.label3);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.PnlInfo.ResumeLayout(false);
            this.PnlInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PnlInfo;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button BtnZIndex;
        private System.Windows.Forms.Label LblTestAlign;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label LblChooseCard;
        private System.Windows.Forms.Button BtnAddAlpha;
        private System.Windows.Forms.Label LblAlpha;
    }
}