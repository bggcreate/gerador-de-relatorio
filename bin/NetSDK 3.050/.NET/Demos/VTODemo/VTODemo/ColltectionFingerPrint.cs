using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSDKCS;
using System.Runtime.InteropServices;

namespace VTODemo
{
    public partial class ColltectionFingerPrint : Form
    {
        IntPtr loginID = IntPtr.Zero;
        NET_DEVICEINFO_Ex deviceInfo = new NET_DEVICEINFO_Ex();
        public int PacketLen { get; set; }
        public byte[] PactetData { get; set; }
        bool isCollection = false;
        bool isCollectionFailed = false;
        bool isListen = false;
        Timer timer;

        public ColltectionFingerPrint(VTO main)
        {
            InitializeComponent();
            main.MessCallBackEx += new Action<IntPtr, NET_ALARM_CAPTURE_FINGER_PRINT_INFO, byte[]>(main_MessCallBackEx);
            timer = new Timer();
            timer.Interval = 30000;
            timer.Tick += new EventHandler(Timer_Tick);
            this.button_collection.Enabled = false;
            this.button_ok.Enabled = false;
            this.label_result.Text = "";
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.label_result.Text = "Collection failed(采集失败)";
            this.button_collection.Enabled = true;
            isCollection = false;
            isCollectionFailed = true;
            if (isListen)
            {
                NETClient.StopListen(loginID);
                isListen = false;
            }
        }

        void main_MessCallBackEx(IntPtr arg1, NET_ALARM_CAPTURE_FINGER_PRINT_INFO arg2, byte[] arg3)
        {
            if (this.loginID == arg1)
            {
                this.BeginInvoke(new Action(() => 
                {
                    timer.Stop();
                    PacketLen = arg2.nPacketLen * arg2.nPacketNum;
                    PactetData = arg3;
                    this.label_result.Text = "Collection Completed(采集完成)";
                    this.button_collection.Enabled = true;
                    isCollection = false;
                    if (isListen)
                    {
                        NETClient.StopListen(loginID);
                        isListen = false;
                    }
                }));
            }
        }


        private void textBox_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == loginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(this.textBox_port.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Port error(端口错误)");
                    return;
                }
                loginID = NETClient.Login(this.textBox_ip.Text.Trim(), port, this.textBox_name.Text.Trim(), this.textBox_password.Text, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref deviceInfo);
                if (loginID == IntPtr.Zero)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_login.Text = "Logout(登出)";
                this.button_collection.Enabled = true;
                this.button_ok.Enabled = true;
            }
            else
            {
                NETClient.Logout(loginID);
                loginID = IntPtr.Zero;
                timer.Stop();
                this.button_login.Text = "Login(登录)";
                this.button_collection.Enabled = false;
                this.button_ok.Enabled = false;
                this.label_result.Text = "";
            }
        }

        private void button_collection_Click(object sender, EventArgs e)
        {
            isListen = NETClient.StartListen(loginID);
            if (isListen == false)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            isCollection = true;
            PacketLen = 0;
            PactetData = null;
            NET_CTRL_CAPTURE_FINGER_PRINT capture = new NET_CTRL_CAPTURE_FINGER_PRINT();
            capture.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_CAPTURE_FINGER_PRINT));
            capture.nChannelID = 0;
            capture.szReaderID = "1";
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_CAPTURE_FINGER_PRINT)));
                Marshal.StructureToPtr(capture, inPtr, true);
                bool ret = NETClient.ControlDevice(loginID, EM_CtrlType.CAPTURE_FINGER_PRINT, inPtr, 100000);
                if (!ret)
                {
                    MessageBox.Show("Start collection failed(开始采集失败)");
                    return;
                }
                timer.Start();
                this.label_result.Text = "Start Collection(开始采集)";
                this.button_collection.Enabled = false;
                isCollectionFailed = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (PactetData == null || PacketLen == 0)
            {
                if (isCollectionFailed)
                {
                    MessageBox.Show("No fingerprint data,because collection failed(没有指纹数据,因为采集失败)");
                    return;
                }
                if (isCollection == false)
                {
                    MessageBox.Show("Did not start collecting(没有开始采集)");
                    return;
                }
                MessageBox.Show("In the collection(采集中)");
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (isListen)
            {
                NETClient.StopListen(loginID);
                isListen = false;
            }
            if (loginID != IntPtr.Zero)
            {
                NETClient.Logout(loginID);
                loginID = IntPtr.Zero;
            }
            base.OnClosed(e);
        }
    }
}
