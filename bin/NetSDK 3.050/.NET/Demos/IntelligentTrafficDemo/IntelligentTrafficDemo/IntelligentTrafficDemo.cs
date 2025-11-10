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
using System.IO;

namespace IntelligentTrafficDemo
{
    public partial class IntelligentTrafficDemo : Form
    {
        private static fDisConnectCallBack m_DisConnectCallBack;
        private static fHaveReConnectCallBack m_ReConnectCallBack;
        private static fAnalyzerDataCallBack m_AnalyzerDataCallBack;
        private const int m_WaitTime = 5000;
        private const int ListViewCount = 100;

        private IntPtr m_LoginID = IntPtr.Zero;
        private NET_DEVICEINFO_Ex m_DeviceInfo;
        private Int64 m_ID = 1;
        private IntPtr m_RealPlayID = IntPtr.Zero;
        private IntPtr m_EventID = IntPtr.Zero;
        private ListViewItem item;

        public IntelligentTrafficDemo()
        {
            InitializeComponent();
            this.Load += new EventHandler(IntelligentTrafficDemo_Load);
        }

        private void IntelligentTrafficDemo_Load(object sender, EventArgs e)
        {
            m_DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
            m_ReConnectCallBack = new fHaveReConnectCallBack(ReConnectCallBack);
            m_AnalyzerDataCallBack = new fAnalyzerDataCallBack(AnalyzerDataCallBack);
            try
            {
                NETClient.Init(m_DisConnectCallBack, IntPtr.Zero, null);
                NETClient.SetAutoReconnect(m_ReConnectCallBack, IntPtr.Zero);
                InitOrLogoutUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke((Action)UpdateDisConnectUI);
        }

        private void UpdateDisConnectUI()
        {
            this.Text = "IntelligentTrafficDemo(智能交通Demo) --- Offline(离线)";
        }

        private void ReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke((Action)UpdateReConnectUI);
        }
        private void UpdateReConnectUI()
        {
            this.Text = "IntelligentTrafficDemo(智能交通Demo) --- Online(在线)";
        }

        private void InitOrLogoutUI()
        {
            openstrobe_button.Enabled = false;
            channel_comboBox.Items.Clear();
            login_button.Text = "Login(登录)";
            channel_comboBox.Enabled = false;
            realplay_button.Enabled = false;
            manualsnap_button.Enabled = false;
            subscribe_button.Enabled = false;
            realplay_button.Text = "StartReal(开始监视)";
            subscribe_button.Text = "SubscribeEvent(订阅事件)";
            m_LoginID = IntPtr.Zero;
            m_RealPlayID = IntPtr.Zero;
            m_EventID = IntPtr.Zero;
            realplay_pictureBox.Refresh();
            event_listView.Items.Clear();
            platetype_textBox.Text = "";
            platecolor_textBox.Text = "";
            platenumber_textBox.Text = "";
            vehiclecolor_textBox.Text = "";
            vehicletype_textBox.Text = "";
            lanenumber_textBox.Text = "";
            attach_pictureBox.Image = null;
            attach_pictureBox.Refresh();
            pic_pictureBox.Image = null;
            pic_pictureBox.Refresh();
            this.Text = "IntelligentTrafficDemo(智能交通Demo)";
        }

        private void LoginUI()
        {
            this.Text = "IntelligentTrafficDemo(智能交通Demo) --- Online(在线)";
            openstrobe_button.Enabled = true;
            login_button.Text = "Logout(登出)";
            channel_comboBox.Enabled = true;
            realplay_button.Enabled = true;
            manualsnap_button.Enabled = true;
            subscribe_button.Enabled = true;
            for (int i = 1; i <= m_DeviceInfo.nChanNum; i++)
            {
                channel_comboBox.Items.Add("channel(通道):" + i);
            }
            channel_comboBox.SelectedIndex = 0;
        }

        private void port_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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

        private void realplay_button_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_RealPlayID)
            {
                m_RealPlayID = NETClient.RealPlay(m_LoginID, channel_comboBox.SelectedIndex, realplay_pictureBox.Handle);;
                if (IntPtr.Zero == m_RealPlayID)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                channel_comboBox.Enabled = false;
                realplay_button.Text = "StopReal(停止监视)";
            }
            else
            {
                bool ret = NETClient.StopRealPlay(m_RealPlayID);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_RealPlayID = IntPtr.Zero;
                channel_comboBox.Enabled = true;
                realplay_button.Text = "StartReal(开始监视)";
                realplay_pictureBox.Refresh();
            }
        }

        private void openstrobe_button_Click(object sender, EventArgs e)
        {
            NET_CTRL_OPEN_STROBE openStrobe = new NET_CTRL_OPEN_STROBE();
            openStrobe.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_OPEN_STROBE));
            openStrobe.nChannelId = channel_comboBox.SelectedIndex;
            openStrobe.szPlateNumber = "";
            IntPtr pOpenStrobe = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_OPEN_STROBE)));
            Marshal.StructureToPtr(openStrobe, pOpenStrobe, true);
            bool ret = NETClient.ControlDevice(m_LoginID, EM_CtrlType.OPEN_STROBE, pOpenStrobe, m_WaitTime);
            Marshal.FreeHGlobal(pOpenStrobe);
            if (!ret)
            {
                MessageBox.Show(this, NETClient.GetLastError());
                return;
            }
            MessageBox.Show(this, "open strobe success(打开道闸成功)!");
        }

        private void manualsnap_button_Click(object sender, EventArgs e)
        {
            NET_MANUAL_SNAP_PARAMETER par = new NET_MANUAL_SNAP_PARAMETER();
            par.byReserved = new byte[60];
            par.nChannel = channel_comboBox.SelectedIndex;
            par.bySequence = "";
            IntPtr parPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_MANUAL_SNAP_PARAMETER)));
            Marshal.StructureToPtr(par, parPtr, true);

            bool ret = NETClient.ControlDevice(m_LoginID, EM_CtrlType.MANUAL_SNAP, parPtr, m_WaitTime);
            Marshal.FreeHGlobal(parPtr);
            if (!ret)
            {
                MessageBox.Show(this, NETClient.GetLastError());
                return;
            }
        }

        private void subscribe_button_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_EventID)
            {
                m_ID = 1;
                m_EventID = NETClient.RealLoadPicture(m_LoginID, channel_comboBox.SelectedIndex, (uint)EM_EVENT_IVS_TYPE.ALL, true, m_AnalyzerDataCallBack, m_LoginID, IntPtr.Zero);
                if (IntPtr.Zero == m_EventID)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                subscribe_button.Text = "UnSubscribeEvent(取消订阅事件)";
            }
            else
            {
                bool ret = NETClient.StopLoadPic(m_EventID);
                if (!ret)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_EventID = IntPtr.Zero;
                subscribe_button.Text = "SubscribeEvent(订阅事件)";
                event_listView.Items.Clear();
                platetype_textBox.Text = "";
                platecolor_textBox.Text = "";
                platenumber_textBox.Text = "";
                vehiclecolor_textBox.Text = "";
                vehicletype_textBox.Text = "";
                lanenumber_textBox.Text = "";
                attach_pictureBox.Image = null;
                attach_pictureBox.Refresh();
                pic_pictureBox.Image = null;
                pic_pictureBox.Refresh();
            }
        }

        private int AnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            EM_EVENT_IVS_TYPE type = (EM_EVENT_IVS_TYPE)dwEventType;
            switch (type)
            {
                case EM_EVENT_IVS_TYPE.TRAFFIC_RUNREDLIGHT:
                    {
                        NET_DEV_EVENT_TRAFFIC_RUNREDLIGHT_INFO info = (NET_DEV_EVENT_TRAFFIC_RUNREDLIGHT_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_RUNREDLIGHT_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "run red light(闯红灯)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFICJUNCTION:
                    {
                        NET_DEV_EVENT_TRAFFICJUNCTION_INFO info = (NET_DEV_EVENT_TRAFFICJUNCTION_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFICJUNCTION_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "junction(卡口)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_OVERSPEED:
                    {
                        NET_DEV_EVENT_TRAFFIC_OVERSPEED_INFO info = (NET_DEV_EVENT_TRAFFIC_OVERSPEED_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_OVERSPEED_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "over speed(超速)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_UNDERSPEED:
                    {
                        NET_DEV_EVENT_TRAFFIC_UNDERSPEED_INFO info = (NET_DEV_EVENT_TRAFFIC_UNDERSPEED_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_UNDERSPEED_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "under speed(低速)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_MANUALSNAP:
                    {
                        NET_DEV_EVENT_TRAFFIC_MANUALSNAP_INFO info = (NET_DEV_EVENT_TRAFFIC_MANUALSNAP_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_MANUALSNAP_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "manual snap(手动抓拍)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_PARKINGSPACEPARKING:
                    {
                        NET_DEV_EVENT_TRAFFIC_PARKINGSPACEPARKING_INFO info = (NET_DEV_EVENT_TRAFFIC_PARKINGSPACEPARKING_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_PARKINGSPACEPARKING_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "parking space(车位有车)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_PARKINGSPACENOPARKING:
                    {
                        NET_DEV_EVENT_TRAFFIC_PARKINGSPACENOPARKING_INFO info = (NET_DEV_EVENT_TRAFFIC_PARKINGSPACENOPARKING_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_PARKINGSPACENOPARKING_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "no spaces(车位无车)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_OVERLINE:
                    {
                        NET_DEV_EVENT_TRAFFIC_OVERLINE_INFO info = (NET_DEV_EVENT_TRAFFIC_OVERLINE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_OVERLINE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "over line(压车道线)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_RETROGRADE:
                    {
                        NET_DEV_EVENT_TRAFFIC_RETROGRADE_INFO info = (NET_DEV_EVENT_TRAFFIC_RETROGRADE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_RETROGRADE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "retrograde(逆行)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                //case EM_EVENT_IVS_TYPE.TRAFFIC_TURNLEFT:
                //    {
                //        NET_DEV_EVENT_TRAFFIC_TURNLEFT_INFO info = (NET_DEV_EVENT_TRAFFIC_TURNLEFT_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_TURNLEFT_INFO));
                //        EventInfo eventInfo = new EventInfo();
                //        eventInfo.ID = m_ID.ToString();
                //        eventInfo.Time = info.UTC.ToString();
                //        eventInfo.Type = "turn left illegally(违章左转)";
                //        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                //        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                //        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                //        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                //        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                //        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                //        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                //        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                //        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                //        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                //        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                //        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                //        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                //        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                //        {
                //            eventInfo.Buffer = new byte[dwBufSize];
                //            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                //        }
                //        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                //        m_ID++;
                //        break;
                //    }
                //case EM_EVENT_IVS_TYPE.TRAFFIC_TURNRIGHT:
                //    {
                //        NET_DEV_EVENT_TRAFFIC_TURNRIGHT_INFO info = (NET_DEV_EVENT_TRAFFIC_TURNRIGHT_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_TURNRIGHT_INFO));
                //        EventInfo eventInfo = new EventInfo();
                //        eventInfo.ID = m_ID.ToString();
                //        eventInfo.Time = info.UTC.ToString();
                //        eventInfo.Type = "turn right illegally(违章右转)";
                //        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                //        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                //        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                //        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                //        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                //        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                //        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                //        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                //        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                //        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                //        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                //        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                //        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                //        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                //        {
                //            eventInfo.Buffer = new byte[dwBufSize];
                //            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                //        }
                //        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                //        m_ID++;
                //        break;
                //    }
                //case EM_EVENT_IVS_TYPE.TRAFFIC_UTURN:
                //    {
                //        NET_DEV_EVENT_TRAFFIC_UTURN_INFO info = (NET_DEV_EVENT_TRAFFIC_UTURN_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_UTURN_INFO));
                //        EventInfo eventInfo = new EventInfo();
                //        eventInfo.ID = m_ID.ToString();
                //        eventInfo.Time = info.UTC.ToString();
                //        eventInfo.Type = "U-turn illegally(违章掉头)";
                //        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                //        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                //        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                //        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                //        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                //        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                //        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                //        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                //        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                //        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                //        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                //        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                //        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                //        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                //        {
                //            eventInfo.Buffer = new byte[dwBufSize];
                //            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                //        }
                //        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                //        m_ID++;
                //        break;
                //    }
                //case EM_EVENT_IVS_TYPE.TRAFFIC_PARKING:
                //    {
                //        NET_DEV_EVENT_TRAFFIC_PARKING_INFO info = (NET_DEV_EVENT_TRAFFIC_PARKING_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_PARKING_INFO));
                //        EventInfo eventInfo = new EventInfo();
                //        eventInfo.ID = m_ID.ToString();
                //        eventInfo.Time = info.UTC.ToString();
                //        eventInfo.Type = "parking illegally(违章停车)";
                //        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                //        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                //        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                //        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                //        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                //        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                //        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                //        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                //        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                //        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                //        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                //        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                //        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                //        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                //        {
                //            eventInfo.Buffer = new byte[dwBufSize];
                //            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                //        }
                //        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                //        m_ID++;
                //        break;
                //    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_WRONGROUTE:
                    {
                        NET_DEV_EVENT_TRAFFIC_WRONGROUTE_INFO info = (NET_DEV_EVENT_TRAFFIC_WRONGROUTE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_WRONGROUTE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "wrong route(不按车道行)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_CROSSLANE:
                    {
                        NET_DEV_EVENT_TRAFFIC_CROSSLANE_INFO info = (NET_DEV_EVENT_TRAFFIC_CROSSLANE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_CROSSLANE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "lane change illegally(违章变道)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stuTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stuTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stuTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stuTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stuTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stuTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stuTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_OVERYELLOWLINE:
                    {
                        NET_DEV_EVENT_TRAFFIC_OVERYELLOWLINE_INFO info = (NET_DEV_EVENT_TRAFFIC_OVERYELLOWLINE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_OVERYELLOWLINE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "over yellow line(压黄线)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_YELLOWPLATEINLANE:
                    {
                        NET_DEV_EVENT_TRAFFIC_YELLOWPLATEINLANE_INFO info = (NET_DEV_EVENT_TRAFFIC_YELLOWPLATEINLANE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_YELLOWPLATEINLANE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "yellow plate in line(黄牌车占道)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stuTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stuTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stuTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stuTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stuTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stuTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stuTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_PEDESTRAINPRIORITY:
                    {
                        NET_DEV_EVENT_TRAFFIC_PEDESTRAINPRIORITY_INFO info = (NET_DEV_EVENT_TRAFFIC_PEDESTRAINPRIORITY_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_PEDESTRAINPRIORITY_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "not courteous(未礼让行人)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_VEHICLEINROUTE:
                    {
                        NET_DEV_EVENT_TRAFFIC_VEHICLEINROUTE_INFO info = (NET_DEV_EVENT_TRAFFIC_VEHICLEINROUTE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_VEHICLEINROUTE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "vehicle in route(有车占道)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_VEHICLEINBUSROUTE:
                    {
                        NET_DEV_EVENT_TRAFFIC_VEHICLEINBUSROUTE_INFO info = (NET_DEV_EVENT_TRAFFIC_VEHICLEINBUSROUTE_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_VEHICLEINBUSROUTE_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "vehicle in bus route(占用公交车道)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_BACKING:
                    {
                        NET_DEV_EVENT_IVS_TRAFFIC_BACKING_INFO info = (NET_DEV_EVENT_IVS_TRAFFIC_BACKING_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_IVS_TRAFFIC_BACKING_INFO));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "backing(违章倒车)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                case EM_EVENT_IVS_TYPE.TRAFFIC_WITHOUT_SAFEBELT:
                    {
                        NET_DEV_EVENT_TRAFFIC_WITHOUT_SAFEBELT info = (NET_DEV_EVENT_TRAFFIC_WITHOUT_SAFEBELT)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_TRAFFIC_WITHOUT_SAFEBELT));
                        EventInfo eventInfo = new EventInfo();
                        eventInfo.ID = m_ID.ToString();
                        eventInfo.Time = info.UTC.ToString();
                        eventInfo.Type = "without safe belt(未系安全带)";
                        eventInfo.GroupID = info.stuFileInfo.nGroupId.ToString();
                        eventInfo.Index = info.stuFileInfo.bIndex.ToString();
                        eventInfo.Count = info.stuFileInfo.bCount.ToString();
                        eventInfo.PlateNumber = info.stuTrafficCar.szPlateNumber;
                        eventInfo.PlateColor = info.stuTrafficCar.szPlateColor;
                        eventInfo.PlateType = info.stuTrafficCar.szPlateType;
                        eventInfo.VehicleColor = info.stuTrafficCar.szVehicleColor;
                        eventInfo.VehicleSize = GetVehicleSize(info.stuTrafficCar.nVehicleSize);
                        eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
                        eventInfo.LaneNumber = info.stuTrafficCar.nLane.ToString();
                        eventInfo.Address = Marshal.PtrToStringAnsi(info.stuTrafficCar.szDeviceAddress);
                        eventInfo.FileLenth = info.stuObject.stPicInfo.dwFileLenth;
                        eventInfo.OffSet = info.stuObject.stPicInfo.dwOffSet;
                        if (IntPtr.Zero != pBuffer && dwBufSize > 0)
                        {
                            eventInfo.Buffer = new byte[dwBufSize];
                            Marshal.Copy(pBuffer, eventInfo.Buffer, 0, (int)dwBufSize);
                        }
                        this.BeginInvoke((Action<EventInfo>)UpdateUI, eventInfo);
                        m_ID++;
                        break;
                    }
                default:
                    Console.WriteLine(dwEventType.ToString("X"));
                    break;
            }
            return 0;
        }

        private void UpdateUI(EventInfo info)
        {
            ShowAndSavePicture(info.Buffer, info.FileLenth, info.OffSet, info.GroupID, info.Index, info.ID);
            platetype_textBox.Text = info.PlateType;
            platecolor_textBox.Text = info.PlateColor;
            platenumber_textBox.Text = info.PlateNumber;
            vehiclecolor_textBox.Text = info.VehicleColor;
            vehicletype_textBox.Text = info.VehicleType;
            lanenumber_textBox.Text = info.LaneNumber;
            item = new ListViewItem();
            item.Text = info.ID;
            item.SubItems.Add(info.Time);
            item.SubItems.Add(info.Type);
            item.SubItems.Add(info.GroupID);
            item.SubItems.Add(info.Index);
            item.SubItems.Add(info.Count);
            item.SubItems.Add(info.PlateNumber);
            item.SubItems.Add(info.PlateType);
            item.SubItems.Add(info.PlateColor);
            item.SubItems.Add(info.VehicleType);
            item.SubItems.Add(info.VehicleColor);
            item.SubItems.Add(info.VehicleSize);
            item.SubItems.Add(info.LaneNumber);
            item.SubItems.Add(info.Address);

            event_listView.BeginUpdate();
            event_listView.Items.Insert(0, item);
            if (event_listView.Items.Count > ListViewCount)
            {
                event_listView.Items.RemoveAt(ListViewCount);
            }
            event_listView.EndUpdate();
        }

        private void ShowAndSavePicture(byte[] buffer, uint fileLenth, uint offSet, string groupID, string index,string id)
        {
            if (null == buffer)
            {
                return;
            }
            byte[] picInfo;
            if (fileLenth > 0 && offSet > 0 && buffer.Length >= offSet + fileLenth)
            {
                picInfo = new byte[offSet];
                Array.Copy(buffer, 0, picInfo, 0, offSet - 1);
                byte[] attachPic = new byte[fileLenth];
                Array.Copy(buffer, offSet, attachPic, 0, fileLenth);
                using (MemoryStream stream = new MemoryStream(attachPic))
                {
                    try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                    {   //这里增加异常捕获是为了当stream不是图片格式的时候出现的异常,stream的数据来自设备，如出现此现像设备上报的数据格式不是图片格式，无法转换成图片。
                        Image attachImage = Image.FromStream(stream);
                        attach_pictureBox.Image = attachImage;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    
                }
            }
            else
            {
                attach_pictureBox.Image = null;
                picInfo = buffer;
            }
            using (MemoryStream stream = new MemoryStream(picInfo))
            {
                try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                {   //这里增加异常捕获是为了当stream不是图片格式的时候出现的异常,stream的数据来自设备，如出现此现像设备上报的数据格式不是图片格式，无法转换成图片。
                    Image picImage = Image.FromStream(stream);
                    pic_pictureBox.Image = picImage;
                    SavePicture(picInfo, groupID, index, id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void SavePicture(byte[] buffer, string groupID, string index,string id)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Capture\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = id+ "-" +groupID + "-"+ index + ".jpg";
            string filePath = path + "\\" + fileName;
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                fileStream.Write(buffer, 0, buffer.Length);
                fileStream.Flush();
                fileStream.Dispose();
            }
        }

        private string GetVehicleSize(EM_VehicleSizeType nVehicleSize)
        {
            string result = "UnKnow(未知)";
            switch(nVehicleSize)
            {
                case  EM_VehicleSizeType.UnKnow:
                    break;
                case EM_VehicleSizeType.Light_Duty:
                    result = "Light-duty(小型车)";
                    break;
                case EM_VehicleSizeType.Medium:
                    result = "Medium(中型车)";
                    break;
                case EM_VehicleSizeType.Oversize:
                    result = "Oversize(大型车)";
                    break;
                case EM_VehicleSizeType.Minisize:
                    result = "Minisize(微型车)";
                    break;
                case EM_VehicleSizeType.Largesize:
                    result = "Largesize(长车)";
                    break;
                default:
                    break;
            }
            return result;
        }

        private string GetPlateNumber(byte[] szText)
        {
            string platenumber = System.Text.Encoding.Default.GetString(szText);
            string[] plate = platenumber.Split('\0');
            if (plate.Length > 0)
            {
                if (plate[0] == "")
                {
                    return "Unknow(未知)";
                }
                return plate[0];
            }
            return "Unknow(未知)";
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NETClient.Cleanup();
        }
    }

    public class EventInfo
    {
        public string ID { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string GroupID { get; set; }
        public string Index { get; set; }
        public string Count { get; set; }
        public string PlateNumber { get; set; }
        public string PlateType { get; set; }
        public string PlateColor { get; set; }
        public string VehicleType { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleSize { get; set; }
        public string LaneNumber { get; set; }
        public string Address { get; set; }
        public uint FileLenth { get;set; }
        public uint OffSet { get;set; }
        public byte[] Buffer { get; set; }
    }
}
