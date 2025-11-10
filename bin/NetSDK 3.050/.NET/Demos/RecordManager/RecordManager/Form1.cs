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
using System.Runtime.InteropServices;

namespace RecordManager
{
    public partial class Form1 : Form
    {
        IntPtr _LoginID = IntPtr.Zero;
        NET_DEVICEINFO_Ex _DeviceInfo = new NET_DEVICEINFO_Ex();
        bool _IsListen = false;
        fMessCallBackEx _AlarmCallBack;

        public Form1()
        {
            InitializeComponent();
            try
            {
                NETClient.Init(null, IntPtr.Zero, null);
                _AlarmCallBack = new fMessCallBackEx(AlarmCallBackEx);
                NETClient.SetDVRMessCallBack(_AlarmCallBack, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            this.button_listen.Enabled = false;
            this.button_schedule.Enabled = false;
        }

        private void textBox_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox_serverport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox_updateperiod_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (_LoginID == IntPtr.Zero)
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


                this.button_listen.Enabled = true;
                this.button_schedule.Enabled = true;
                this.button_login.Text = "Logout(登出)";
            }
            else
            {
                NETClient.Logout(_LoginID);
                _LoginID = IntPtr.Zero;
                this.button_login.Text = "Login(登录)";
                this.button_listen.Enabled = false;
                this.button_schedule.Enabled = false;
                
                this.listView_alarm.Items.Clear();
                this.button_listen.Text = "Start Listen(开始监听)";
                _IsListen = false;
            }
        }


        private void button_listen_Click(object sender, EventArgs e)
        {
            if (_IsListen == false)
            {
                bool ret = NETClient.StartListen(_LoginID);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                this.button_listen.Text = "Stop Listen(停止监听)";
                _IsListen = true;
            }
            else
            {
                bool ret = NETClient.StopListen(_LoginID);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                this.button_listen.Text = "Start Listen(开始监听)";
                _IsListen = false;
            }
        }

        private bool AlarmCallBackEx(int lCommand, IntPtr lLoginID, IntPtr pBuf, uint dwBufLen, IntPtr pchDVRIP, int nDVRPort, bool bAlarmAckFlag, int nEventID, IntPtr dwUser)
        {
            EM_ALARM_TYPE type = (EM_ALARM_TYPE)lCommand;
            switch (type)
            {
                case EM_ALARM_TYPE.ALARM_RECORD_SCHEDULE_CHANGE:
                    {
                        NET_ALARM_RECORD_SCHEDULE_CHANGE_INFO info = (NET_ALARM_RECORD_SCHEDULE_CHANGE_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_ALARM_RECORD_SCHEDULE_CHANGE_INFO));
                        this.BeginInvoke(new Action<object>(UpdateUI), info);
                    }
                    break;
                default:
                    Console.WriteLine(lCommand.ToString("X"));
                    break;
            }
            return true;
        }

        private void UpdateUI(object obj)
        {
            ListViewItem item = new ListViewItem();
            if(obj is NET_ALARM_RECORD_SCHEDULE_CHANGE_INFO)
            {
                NET_ALARM_RECORD_SCHEDULE_CHANGE_INFO info = (NET_ALARM_RECORD_SCHEDULE_CHANGE_INFO)obj;
                item.Text = info.stuTime.ToShortString();
                item.SubItems.Add("ALARM_RECORD_SCHEDULE_CHANGE(录像计划改变)");
                item.SubItems.Add(info.nChannelID.ToString());
                item.SubItems.Add(info.szUser);
                string msg = "";
                if(info.nEventAction == 0)
                {
                    msg = "Pulse event(脉冲事件)";
                }
                else if(info.nEventAction == 1)
                {
                    msg = "Start(开始)";
                }
                else
                {
                    msg = "Stop(结束)";
                }
                item.SubItems.Add(msg);
            }
            listView_alarm.BeginUpdate();
            listView_alarm.Items.Insert(0, item);
            if (listView_alarm.Items.Count > 100)
            {
                listView_alarm.Items.RemoveAt(100);
            }
            listView_alarm.EndUpdate();
        }

        private void button_schedule_Click(object sender, EventArgs e)
        {
            Schedule s = new Schedule(_LoginID, _DeviceInfo.nChanNum);
            s.ShowDialog();
            s.Dispose();
        }
    }
}
