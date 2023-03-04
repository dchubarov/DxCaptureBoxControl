using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DxCaptureBoxTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateSize()
        {
            if (dxCaptureBox1.State == Gyrosoft.WinForms.DxCaptureBoxState.Playing ||
                dxCaptureBox1.State == Gyrosoft.WinForms.DxCaptureBoxState.Paused) {
                label1.Text = dxCaptureBox1.VideoWidth.ToString() + "x" + dxCaptureBox1.VideoHeight.ToString();
            }
            else {
                label1.Text = String.Empty;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateSize();

            MessageBox.Show(dxCaptureBox1.GetDeviceNames()[0]);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            dxCaptureBox1.Stop();
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            dxCaptureBox1.Play(0);
            UpdateSize();
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            dxCaptureBox1.Pause();
            UpdateSize();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            dxCaptureBox1.Stop();
            UpdateSize();
        }

        private void smallResBtn_Click(object sender, EventArgs e)
        {
            dxCaptureBox1.Stop();
            dxCaptureBox1.Play(0, 320, 240, 24, false);
            UpdateSize();
        }

        private void mediumResBtn_Click(object sender, EventArgs e)
        {
            dxCaptureBox1.Stop();
            dxCaptureBox1.Play(0, 640, 480, 24, true);
            UpdateSize();
        }

        private void largeResBtn_Click(object sender, EventArgs e)
        {
            dxCaptureBox1.Stop();
            dxCaptureBox1.Play(0, 720, 576, 24, true);
            UpdateSize();
        }

        private void snapBtn_Click(object sender, EventArgs e)
        {
            try {
                Bitmap img = dxCaptureBox1.Snap();
                if (img != null) {
                    dxCaptureBox1.ShowStillImage(img);

                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}