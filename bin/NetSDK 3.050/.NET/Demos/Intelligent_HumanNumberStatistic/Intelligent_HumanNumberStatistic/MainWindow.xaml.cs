using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetSDKCS;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace Intelligent_HumanNumberStatistic
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        NET_DEVICEINFO_Ex m_DevicInfo = new NET_DEVICEINFO_Ex();
        IntPtr m_LoginID = IntPtr.Zero;
        IntPtr m_PlayID = IntPtr.Zero;
        IntPtr m_AttactID = IntPtr.Zero;
        fVideoStatSumCallBack m_VideoStatSumCallBack;

        public ObservableCollection<FlowInfo> InformationList
        {
            get;
            set;
        }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                m_VideoStatSumCallBack = new fVideoStatSumCallBack(VideoStatSumCallBack);
                NETClient.Init(null, IntPtr.Zero, null);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InformationList = new ObservableCollection<FlowInfo>();
            this.combox.IsEnabled = false;
            this.btn_realplay.IsEnabled = false;
            this.btn_attach.IsEnabled = false;
            this.btn_clear.IsEnabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if(m_AttactID != IntPtr.Zero)
            {
                NETClient.DetachVideoStatSummary(m_AttactID);
            }
            NETClient.Cleanup();
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            if(IntPtr.Zero == m_LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(tb_port.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Please input number and the value is 1-65535(请输入1-65535的值)");
                    return;
                }
                m_LoginID = NETClient.Login(tb_ip.Text.Trim(), port, tb_name.Text.Trim(), tb_password.Password, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DevicInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                for (int i = 0; i < m_DevicInfo.nChanNum; i++)
                {
                    this.combox.Items.Add(i);
                }
                this.combox.SelectedIndex = 0;
                this.combox.IsEnabled = true;
                btn_realplay.IsEnabled = true;
                btn_attach.IsEnabled = true;
                btn_clear.IsEnabled = true;
                btn_login.Content = "Logout(登出)";
            }
            else
            {
                if(IntPtr.Zero != m_AttactID)
                {
                    NETClient.DetachVideoStatSummary(m_AttactID);
                }
                NETClient.Logout(m_LoginID);
                m_LoginID = IntPtr.Zero;
                m_PlayID = IntPtr.Zero;
                m_AttactID = IntPtr.Zero;
                combox.IsEnabled = false;
                combox.Items.Clear();
                btn_realplay.IsEnabled = false;
                btn_realplay.Content = "RealPlay(监视)";
                btn_attach.IsEnabled = false;
                btn_attach.Content = "Attach(订阅)";
                btn_clear.IsEnabled = false;
                btn_login.Content = "Login(登录)";
                pic.Refresh();
                InformationList.Clear();
                listview.ItemsSource = null;
            }
        }

        private void btn_realplay_Click(object sender, RoutedEventArgs e)
        {
            if(IntPtr.Zero == m_PlayID)
            {
                m_PlayID = NETClient.RealPlay(m_LoginID, combox.SelectedIndex, pic.Handle);
                if(IntPtr.Zero == m_PlayID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                NETClient.RenderPrivateData(m_PlayID, true);
                btn_realplay.Content = "StopRealPlay(停止监视)";
                combox.IsEnabled = false;
            }
            else
            {
                NETClient.RenderPrivateData(m_PlayID, false);
                NETClient.StopRealPlay(m_PlayID);
                m_PlayID = IntPtr.Zero;
                btn_realplay.Content = "RealPlay(监视)";
                combox.IsEnabled = true;
                pic.Refresh();
            }
        }

        private void btn_attach_Click(object sender, RoutedEventArgs e)
        {
            if(m_AttactID == IntPtr.Zero)
            {
                NET_IN_ATTACH_VIDEOSTAT_SUM inParam = new NET_IN_ATTACH_VIDEOSTAT_SUM();
                inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ATTACH_VIDEOSTAT_SUM));
                inParam.nChannel = combox.SelectedIndex;
                inParam.cbVideoStatSum = m_VideoStatSumCallBack;
                NET_OUT_ATTACH_VIDEOSTAT_SUM outParam = new NET_OUT_ATTACH_VIDEOSTAT_SUM();
                outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ATTACH_VIDEOSTAT_SUM));
                m_AttactID = NETClient.AttachVideoStatSummary(m_LoginID, ref inParam, ref outParam, 5000);
                if(IntPtr.Zero == m_AttactID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                btn_attach.Content = "Detach(取消订阅)";
            }
            else
            {
                NETClient.DetachVideoStatSummary(m_AttactID);
                m_AttactID = IntPtr.Zero;
                btn_attach.Content = "Attach(订阅)";
            }
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            NET_CTRL_CLEAR_SECTION_STAT_INFO info = new NET_CTRL_CLEAR_SECTION_STAT_INFO();
            info.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_CLEAR_SECTION_STAT_INFO));
            info.nChannel = combox.SelectedIndex;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_CLEAR_SECTION_STAT_INFO)));
                Marshal.StructureToPtr(info, inPtr, true);
                bool ret = NETClient.ControlDevice(m_LoginID, EM_CtrlType.CLEAR_SECTION_STAT, inPtr, 5000);
                if(!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                }
                else
                {
                    MessageBox.Show("Clear Successfully(成功清空)");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
            }
            
        }

        void VideoStatSumCallBack(IntPtr lAttachHandle, IntPtr pBuf, uint dwBufLen, IntPtr dwUser)
        {
            if(lAttachHandle == m_AttactID)
            {
                NET_VIDEOSTAT_SUMMARY info = (NET_VIDEOSTAT_SUMMARY)Marshal.PtrToStructure(pBuf, typeof(NET_VIDEOSTAT_SUMMARY));
                FlowInfo flowInfo = new FlowInfo();
                flowInfo.Channel = info.nChannelID;
                flowInfo.OSDIn = info.stuEnteredSubtotal.nOSD;
                flowInfo.OSDOut = info.stuExitedSubtotal.nOSD;
                flowInfo.TodayIn = info.stuEnteredSubtotal.nToday;
                flowInfo.TodayOut = info.stuExitedSubtotal.nToday;
                flowInfo.TotalIn = info.stuEnteredSubtotal.nTotal;
                flowInfo.TotalOut = info.stuExitedSubtotal.nTotal;
                this.Dispatcher.BeginInvoke(new Action(() => 
                {
                    var item = InformationList.ToList().Find(p => p.Channel == info.nChannelID);
                    if (item == null)
                    {
                        InformationList.Add(flowInfo);
                    }
                    else
                    {
                        int index = InformationList.IndexOf(item);
                        InformationList.RemoveAt(index);
                        InformationList.Insert(index, flowInfo);
                    }
                    listview.ItemsSource = InformationList;
                }));
                
            }
        }
    }

    public class FlowInfo
    {
        public int Channel { get; set; }
        public int OSDIn { get; set; }
        public int OSDOut { get; set; }
        public int TodayIn { get; set; }
        public int TodayOut { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
    }
}
