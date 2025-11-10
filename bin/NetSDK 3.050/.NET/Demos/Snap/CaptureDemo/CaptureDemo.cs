using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSDKCS;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CaptureDemo
{
    public partial class CaptureDemo : Form
    {
        private IntPtr _LoginID = IntPtr.Zero;
        private IntPtr _PlayID = IntPtr.Zero;
        private fSnapRevCallBack _SnapRevCallBack;
        private NET_DEVICEINFO_Ex _DeviceInfo = new NET_DEVICEINFO_Ex();
        private bool _IsSpanCapture = false;

        public CaptureDemo()
        {
            InitializeComponent();
            try
            {
                _SnapRevCallBack = new fSnapRevCallBack(SnapRevCallBack);
                NETClient.Init(null, IntPtr.Zero, null);
                NETClient.SetSnapRevCallBack(_SnapRevCallBack, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            this.Load += new EventHandler(CaptureDemo_Load);
        }

        void CaptureDemo_Load(object sender, EventArgs e)
        {
            this.comboBox_channel.Enabled = false;
            this.button_realplay.Enabled = false;
            this.button_local.Enabled = false;
            this.button_remote.Enabled = false;
            this.button_span.Enabled = false;
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == _LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(this.textBox_port.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Input port error(输入端口错误)!");
                    return;
                }
                _LoginID = NETClient.Login(this.textBox_ip.Text.Trim(), port, this.textBox_name.Text.Trim(), this.textBox_password.Text, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref _DeviceInfo);
                if (IntPtr.Zero == _LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                for (int i = 0; i < _DeviceInfo.nChanNum; i++)
                {
                    this.comboBox_channel.Items.Add(i + 1);
                }
                this.comboBox_channel.SelectedIndex = 0;
                this.comboBox_channel.Enabled = true;
                this.button_realplay.Enabled = true;
                this.button_remote.Enabled = true;
                this.button_span.Enabled = true;
 
                this.button_login.Text = "Logout(登出)";
            }
            else
            {
                NETClient.Logout(_LoginID);
                _LoginID = IntPtr.Zero;
                this.comboBox_channel.Items.Clear();
                this.comboBox_channel.Enabled = false;
                this.button_realplay.Enabled = false;
                this.button_realplay.Text = "RealPlay(监视)";
                this.button_local.Enabled = false;
                this.button_remote.Enabled = false;
                this.button_span.Enabled = false;
                this.button_login.Text = "Login(登录)";
                this.pictureBox_preview.Refresh();
                this.pictureBox_image.Image = null;
                this.pictureBox_image.Refresh();
                if (_IsSpanCapture)
                {
                    _IsSpanCapture = false;
                    this.button_span.Text = "Span Capture(定时抓图)";
                }
            }
        }

        private void button_realplay_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == _PlayID)
            {
                _PlayID = NETClient.RealPlay(_LoginID, comboBox_channel.SelectedIndex, this.pictureBox_preview.Handle);
                if (IntPtr.Zero == _PlayID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_local.Enabled = true;
                this.button_realplay.Text = "StopRealPlay(停止监视)"; 
            }
            else
            {
                NETClient.StopRealPlay(_PlayID);
                _PlayID = IntPtr.Zero;
                this.button_local.Enabled = false;
                this.button_realplay.Text = "RealPlay(监视)";
                this.pictureBox_preview.Refresh();
            }
        }

        private void button_local_Click(object sender, EventArgs e)
        {
            if (pictureBox_image.Image != null)
            {
                pictureBox_image.Image.Dispose();
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "image";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            DateTime now = DateTime.Now;
            string filePath = path + "\\" + string.Format("{0}-{1}-{2}-{3}-{4}-{5}",now.Year,now.Month,now.Day,now.Hour,now.Minute,now.Second) + ".jpg";
            bool ret = NETClient.CapturePicture(_PlayID, filePath, EM_NET_CAPTURE_FORMATS.JPEG);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            Image image = Image.FromFile(filePath);
            pictureBox_image.Image = image;
        }

        private void button_remote_Click(object sender, EventArgs e)
        {
            NET_SNAP_PARAMS asyncSnap = new NET_SNAP_PARAMS();
            asyncSnap.Channel = (uint)this.comboBox_channel.SelectedIndex;
            asyncSnap.Quality = 6;
            asyncSnap.ImageSize = 2;
            asyncSnap.mode = 0;
            asyncSnap.InterSnap = 0;
            bool ret = NETClient.SnapPictureEx(_LoginID, asyncSnap, IntPtr.Zero);
            if (!ret)
            {
                MessageBox.Show(this, NETClient.GetLastError());
                return;
            }
        }

        private void button_span_Click(object sender, EventArgs e)
        {
            if (!_IsSpanCapture)
            {
                NET_SNAP_PARAMS asyncSnap = new NET_SNAP_PARAMS();
                asyncSnap.Channel = (uint)this.comboBox_channel.SelectedIndex;
                asyncSnap.Quality = 6;
                asyncSnap.ImageSize = 2;
                asyncSnap.mode = 1;
                bool ret = NETClient.SnapPictureEx(_LoginID, asyncSnap, IntPtr.Zero);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                _IsSpanCapture = true;
                this.comboBox_channel.Enabled = false;
                this.button_remote.Enabled = false;
                this.button_span.Text = "Stop Span Capture(停止定时抓图)";
            }
            else
            {
                NET_SNAP_PARAMS asyncSnap = new NET_SNAP_PARAMS();
                asyncSnap.Channel = (uint)this.comboBox_channel.SelectedIndex;
                asyncSnap.Quality = 6;
                asyncSnap.ImageSize = 2;
                asyncSnap.mode = 0xFFFFFFFF;
                asyncSnap.InterSnap = 0;
                bool ret = NETClient.SnapPictureEx(_LoginID, asyncSnap, IntPtr.Zero);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                _IsSpanCapture = false;
                this.comboBox_channel.Enabled = true;
                this.button_remote.Enabled = true;
                this.button_span.Text = "Span Capture(定时抓图)";
            }
        }

        private void SnapRevCallBack(IntPtr lLoginID, IntPtr pBuf, uint RevLen, uint EncodeType, uint CmdSerial, IntPtr dwUser)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "image";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (EncodeType == 10) //.jpg
            {
                DateTime now = DateTime.Now;
                string fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second) + ".jpg";
                string filePath = path + "\\" + fileName;
                byte[] data = new byte[RevLen];
                Marshal.Copy(pBuf, data, 0, (int)RevLen);
                try
                {
                    //when the file is operate by local capture will throw expection.
                    using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        stream.Write(data, 0, (int)RevLen);
                        stream.Flush();
                        stream.Dispose();
                    }
                }
                catch
                {
                    return;
                }
                this.BeginInvoke(new Action(() => 
                {
                    if (pictureBox_image.Image != null)
                    {
                        pictureBox_image.Image.Dispose();
                    }
                    Image image = Image.FromFile(filePath);
                    pictureBox_image.Image = image;
                }));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NETClient.Cleanup();
        }

        private void textBox_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
