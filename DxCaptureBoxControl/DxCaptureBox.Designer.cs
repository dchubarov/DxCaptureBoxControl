namespace Gyrosoft.WinForms
{
    partial class DxCaptureBox
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
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }

#if DEBUG
                if (_rot != null) {
                    _rot.Dispose();
                }
#endif

                ReleaseInterfaces();

                if (_pictureReadyEvent != null) {
                    _pictureReadyEvent.Close();
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.videoBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // videoBox
            // 
            this.videoBox.BackColor = System.Drawing.SystemColors.Control;
            this.videoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoBox.Location = new System.Drawing.Point(2, 2);
            this.videoBox.Name = "videoBox";
            this.videoBox.Size = new System.Drawing.Size(320, 240);
            this.videoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.videoBox.TabIndex = 0;
            this.videoBox.TabStop = false;
            // 
            // DxCaptureBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.videoBox);
            this.Name = "DxCaptureBox";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(324, 244);
            this.Load += new System.EventHandler(this.DxCaptureBox_Load);
            this.Resize += new System.EventHandler(this.DxCaptureBox_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox videoBox;
    }
}
