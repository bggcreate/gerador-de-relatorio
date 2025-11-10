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

namespace AlarmDemo
{
    public partial class AlarmDemo : Form
    {
        private static fDisConnectCallBack m_DisConnectCallBack;
        private static fHaveReConnectCallBack m_ReConnectCallBack;
        private static fMessCallBackEx m_AlarmCallBack;
        private const int ALARM_START = 1;
        private const int ALARM_STOP = 0;
        private const int ListViewCount = 100;

        private IntPtr m_LoginID = IntPtr.Zero;
        private NET_DEVICEINFO_Ex m_DeviceInfo;
        private bool m_IsListen = false;
        private Int64 m_ID = 1;
        private List<AlarmInfo> m_ManangeAlarmInfo = new List<AlarmInfo>();
        private byte[] data;

        public AlarmDemo()
        {
            InitializeComponent();
            this.Load += new EventHandler(AlarmDemo_Load);
        }

        private void AlarmDemo_Load(object sender, EventArgs e)
        {
            m_DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
            m_ReConnectCallBack = new fHaveReConnectCallBack(ReConnectCallBack);
            m_AlarmCallBack = new fMessCallBackEx(AlarmCallBackEx);
            try
            {
                NETClient.Init(m_DisConnectCallBack, IntPtr.Zero, null);
                NETClient.SetAutoReconnect(m_ReConnectCallBack, IntPtr.Zero);
                NETClient.SetDVRMessCallBack(m_AlarmCallBack, IntPtr.Zero);
                InitOrLogoutUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        private bool AlarmCallBackEx(int lCommand, IntPtr lLoginID, IntPtr pBuf, uint dwBufLen, IntPtr pchDVRIP, int nDVRPort, bool bAlarmAckFlag, int nEventID, IntPtr dwUser)
        {
            EM_ALARM_TYPE type = (EM_ALARM_TYPE)lCommand;
            switch (type)
            {
                case EM_ALARM_TYPE.ALARM_ALARM_EX:
                case EM_ALARM_TYPE.MOTION_ALARM_EX:
                case EM_ALARM_TYPE.VIDEOLOST_ALARM_EX:
                case EM_ALARM_TYPE.SHELTER_ALARM_EX:
                case EM_ALARM_TYPE.DISKFULL_ALARM_EX:
                case EM_ALARM_TYPE.DISKERROR_ALARM_EX:
                    data = new byte[dwBufLen];
                    Marshal.Copy(pBuf, data, 0, (int)dwBufLen);
                    for (int i = 0; i < dwBufLen; i++)
                    {
                        if (data[i] == ALARM_START) // alarm start 报警开始
                        {
                            AlarmInfo info = new AlarmInfo();
                            info.AlarmType = type;
                            info.ID = m_ID;
                            info.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            info.Channel = i;
                            info.Status = ALARM_START;
                            AlarmInfo item = m_ManangeAlarmInfo.Find(p => p.AlarmType == type && p.Channel == i);
                            if (null == item)
                            {
                                m_ManangeAlarmInfo.Add(info);
                                m_ID++;
                                this.BeginInvoke((Action<AlarmInfo>)UpdateAlarmInfo, info);
                            }
                        }
                        else //alarm stop 报警停止
                        {
                            AlarmInfo item = m_ManangeAlarmInfo.Find(p => p.AlarmType == type && p.Channel == i);
                            if (null != item)
                            {
                                AlarmInfo info = new AlarmInfo();
                                info.AlarmType = type;
                                info.ID = m_ID;
                                info.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                info.Channel = i;
                                info.Status = ALARM_STOP;
                                m_ManangeAlarmInfo.Remove(item);
                                m_ID++;
                                this.BeginInvoke((Action<AlarmInfo>)UpdateAlarmInfo, info);
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine(lCommand.ToString("X"));
                    break;
            }
           
            return true;
        }


        private void UpdateAlarmInfo(AlarmInfo info)
        {
            switch (info.AlarmType)
            {
                case EM_ALARM_TYPE.ALARM_ALARM_EX:
                    if (ALARM_START == info.Status)
                    {
                        info.Message = "External alarm Start(外部报警开始)";
                    }
                    else
                    {
                        info.Message = "External alarm Stop(外部报警结束)";
                    }
                    break;
                case EM_ALARM_TYPE.MOTION_ALARM_EX:
                    if (ALARM_START == info.Status)
                    {
                        info.Message = "Motion detection alarm Start(动态检测报警开始)";
                    }
                    else
                    {
                        info.Message = "Motion detection alarm Stop(动态检测报警结束)";
                    }
                    break;
                case EM_ALARM_TYPE.VIDEOLOST_ALARM_EX:
                    if (ALARM_START == info.Status)
                    {
                        info.Message = "Video loss alarm Start(视频丢失报警开始)";
                    }
                    else
                    {
                        info.Message = "Video loss alarm Stop(视频丢失报警结束)";
                    }
                    break;
                case EM_ALARM_TYPE.SHELTER_ALARM_EX:
                    if (ALARM_START == info.Status)
                    {
                        info.Message = "Camera masking alarm Start(视频遮挡报警开始)";
                    }
                    else
                    {
                        info.Message = "Camera masking alarm Stop(视频遮挡报警结束)";
                    }
                    break;
                case EM_ALARM_TYPE.DISKFULL_ALARM_EX:
                    if (ALARM_START == info.Status)
                    {
                        info.Message = "HDD full alarm Start(硬盘满报警开始)";
                    }
                    else
                    {
                        info.Message = "HDD full alarm Stop(硬盘满报警结束)";
                    }
                    break;
                case EM_ALARM_TYPE.DISKERROR_ALARM_EX:
                    if (ALARM_START == info.Status)
                    {
                        info.Message = "HDD error alarm Start(坏硬盘报警开始)";
                    }
                    else
                    {
                        info.Message = "HDD error alarm Stop(坏硬盘报警结束)";
                    }
                    break;
                default:
                    break;
            }

            var item = new ListViewItem();
            item.Text = info.ID.ToString();
            item.SubItems.Add(info.Time);
            item.SubItems.Add((info.Channel + 1).ToString());
            item.SubItems.Add(info.Message);

            alarm_listView.BeginUpdate();
            alarm_listView.Items.Insert(0, item);
            if (alarm_listView.Items.Count > ListViewCount)
            {
                alarm_listView.Items.RemoveAt(ListViewCount);
            }
            alarm_listView.EndUpdate();
        }
        

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke((Action)UpdateDisConnectUI);
        }

        private void UpdateDisConnectUI()
        {
            this.Text = "AlarmDemo(报警Demo) --- Offline(离线)";
        }

        private void ReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke((Action)UpdateReConnectUI);
        }
        private void UpdateReConnectUI()
        {
            this.Text = "AlarmDemo(报警Demo) --- Online(在线)";
        }

        private void port_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void InitOrLogoutUI()
        {
            m_ManangeAlarmInfo.Clear();
            alarm_listView.Items.Clear();
            startlisten_button.Enabled = false;
            m_LoginID = IntPtr.Zero;
            this.Text = "AlarmDemo(报警Demo)";
            login_button.Text = "Login(登录)";
            StopListenUI();
        }

        private void LoginUI()
        {
            this.Text = "AlarmDemo(报警Demo) --- Online(在线)";
            startlisten_button.Enabled = true;
            login_button.Text = "Logout(登出)";
        }

        private void StartListenUI()
        {
            m_IsListen = true;
            startlisten_button.Text = "StopListen(停止监听)";
        }

        private void StopListenUI()
        {
            m_IsListen = false;
            m_ManangeAlarmInfo.Clear();
            alarm_listView.Items.Clear();
            startlisten_button.Text = "StartListen(开始监听)";
        }

        private void login_button_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(port_textBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Input port error(输入端口错误)!");
                    return;
                }
                m_DeviceInfo = new NET_DEVICEINFO_Ex();
                m_LoginID = NETClient.Login(ip_textBox.Text.Trim(), port, name_textBox.Text.Trim(), pwd_textBox.Text.Trim(), EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DeviceInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                LoginUI();
            }
            else
            {
                bool result = NETClient.Logout(m_LoginID);
                if (!result)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_LoginID = IntPtr.Zero;
                InitOrLogoutUI();
            }
        }

        private void startlisten_button_Click(object sender, EventArgs e)
        {
            if (!m_IsListen)
            {
                bool ret = NETClient.StartListen(m_LoginID);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_ID = 1;
                StartListenUI();
            }
            else
            {
                bool ret = NETClient.StopListen(m_LoginID);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                StopListenUI();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NETClient.Cleanup();
        }
    }

    public class AlarmInfo
    {
        public EM_ALARM_TYPE AlarmType { get; set; }
        public Int64 ID { get; set; }
        public string Time { get; set; }
        public int Channel { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
