namespace DxCaptureBoxTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
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
        private void InitializeComponent()
        {
            this.playBtn = new System.Windows.Forms.Button();
            this.pauseBtn = new System.Windows.Forms.Button();
            this.stopBtn = new System.Windows.Forms.Button();
            this.smallResBtn = new System.Windows.Forms.Button();
            this.mediumResBtn = new System.Windows.Forms.Button();
            this.largeResBtn = new System.Windows.Forms.Button();
            this.dxCaptureBox1 = new Gyrosoft.WinForms.DxCaptureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.snapBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // playBtn
            // 
            this.playBtn.Location = new System.Drawing.Point(12, 12);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(50, 23);
            this.playBtn.TabIndex = 1;
            this.playBtn.Text = "play";
            this.playBtn.UseVisualStyleBackColor = true;
            this.playBtn.Click += new System.EventHandler(this.playBtn_Click);
            // 
            // pauseBtn
            // 
            this.pauseBtn.Location = new System.Drawing.Point(68, 12);
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(50, 23);
            this.pauseBtn.TabIndex = 2;
            this.pauseBtn.Text = "pause";
            this.pauseBtn.UseVisualStyleBackColor = true;
            this.pauseBtn.Click += new System.EventHandler(this.pauseBtn_Click);
            // 
            // stopBtn
            // 
            this.stopBtn.Location = new System.Drawing.Point(124, 12);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(50, 23);
            this.stopBtn.TabIndex = 3;
            this.stopBtn.Text = "stop";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // smallResBtn
            // 
            this.smallResBtn.Location = new System.Drawing.Point(291, 12);
            this.smallResBtn.Name = "smallResBtn";
            this.smallResBtn.Size = new System.Drawing.Size(58, 23);
            this.smallResBtn.TabIndex = 4;
            this.smallResBtn.Text = "320x240";
            this.smallResBtn.UseVisualStyleBackColor = true;
            this.smallResBtn.Click += new System.EventHandler(this.smallResBtn_Click);
            // 
            // mediumResBtn
            // 
            this.mediumResBtn.Location = new System.Drawing.Point(355, 12);
            this.mediumResBtn.Name = "mediumResBtn";
            this.mediumResBtn.Size = new System.Drawing.Size(58, 23);
            this.mediumResBtn.TabIndex = 5;
            this.mediumResBtn.Text = "640x480";
            this.mediumResBtn.UseVisualStyleBackColor = true;
            this.mediumResBtn.Click += new System.EventHandler(this.mediumResBtn_Click);
            // 
            // largeResBtn
            // 
            this.largeResBtn.Location = new System.Drawing.Point(419, 12);
            this.largeResBtn.Name = "largeResBtn";
            this.largeResBtn.Size = new System.Drawing.Size(58, 23);
            this.largeResBtn.TabIndex = 6;
            this.largeResBtn.Text = "720x576";
            this.largeResBtn.UseVisualStyleBackColor = true;
            this.largeResBtn.Click += new System.EventHandler(this.largeResBtn_Click);
            // 
            // dxCaptureBox1
            // 
            this.dxCaptureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.dxCaptureBox1.InactiveBorderColor = System.Drawing.SystemColors.Control;
            this.dxCaptureBox1.Location = new System.Drawing.Point(12, 51);
            this.dxCaptureBox1.Name = "dxCaptureBox1";
            this.dxCaptureBox1.Padding = new System.Windows.Forms.Padding(2);
            this.dxCaptureBox1.PauseBorderColor = System.Drawing.Color.Yellow;
            this.dxCaptureBox1.PlayBorderColor = System.Drawing.Color.Red;
            this.dxCaptureBox1.Size = new System.Drawing.Size(644, 484);
            this.dxCaptureBox1.StillBorderColor = System.Drawing.Color.Green;
            this.dxCaptureBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(517, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "label1";
            // 
            // snapBtn
            // 
            this.snapBtn.Location = new System.Drawing.Point(180, 13);
            this.snapBtn.Name = "snapBtn";
            this.snapBtn.Size = new System.Drawing.Size(50, 23);
            this.snapBtn.TabIndex = 8;
            this.snapBtn.Text = "snap";
            this.snapBtn.UseVisualStyleBackColor = true;
            this.snapBtn.Click += new System.EventHandler(this.snapBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 547);
            this.Controls.Add(this.snapBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.largeResBtn);
            this.Controls.Add(this.mediumResBtn);
            this.Controls.Add(this.smallResBtn);
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.pauseBtn);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.dxCaptureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Gyrosoft.WinForms.DxCaptureBox dxCaptureBox1;
        private System.Windows.Forms.Button playBtn;
        private System.Windows.Forms.Button pauseBtn;
        private System.Windows.Forms.Button stopBtn;
        private System.Windows.Forms.Button smallResBtn;
        private System.Windows.Forms.Button mediumResBtn;
        private System.Windows.Forms.Button largeResBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button snapBtn;
    }
}

