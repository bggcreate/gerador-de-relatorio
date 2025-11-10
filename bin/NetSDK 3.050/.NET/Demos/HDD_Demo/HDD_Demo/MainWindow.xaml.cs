using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using NetSDKCS;

namespace HDD_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr m_LoginID = IntPtr.Zero;
        NET_DEVICEINFO_Ex m_DeviceInfo = new NET_DEVICEINFO_Ex();
        public ObservableCollection<DISKINFO> DiskList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                NETClient.Init(null, IntPtr.Zero, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            btn_Get.IsEnabled = false;
            DiskList = new ObservableCollection<DISKINFO>();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NETClient.Cleanup();
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            if(m_LoginID == IntPtr.Zero)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(tb_port.Text);
                }
                catch
                {
                    MessageBox.Show("Please input number and value is 1-65535(请输入1-65535的值)");
                    return;
                }
                m_LoginID = NETClient.Login(tb_ip.Text.Trim(), port, tb_name.Text.Trim(), tb_password.Password, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DeviceInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                btn_login.Content = "Logout(登出)";
                btn_Get.IsEnabled = true;
            }
            else
            {
                NETClient.Logout(m_LoginID);
                m_LoginID = IntPtr.Zero;
                btn_login.Content = "Login(登录)";
                btn_Get.IsEnabled = false;
                DiskList.Clear();
                list.ItemsSource = null;
            }
        }

        private void btn_Get_Click(object sender, RoutedEventArgs e)
        {
            DiskList.Clear();
            list.ItemsSource = null;
            NET_HARDDISK_STATE nET_HARDDISK_STATE = new NET_HARDDISK_STATE();
            object obj = nET_HARDDISK_STATE;
            bool ret = NETClient.QueryDevState(m_LoginID, EM_DEVICE_STATE.DISK, ref obj, typeof(NET_HARDDISK_STATE), 5000);
            if(!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            nET_HARDDISK_STATE = (NET_HARDDISK_STATE)obj;
            for(int i =0; i < nET_HARDDISK_STATE.dwDiskNum; i++)
            {
                DISKINFO dISKINFO = new DISKINFO();
                dISKINFO.DiskNumber = nET_HARDDISK_STATE.stDisks[i].bDiskNum.ToString();
                dISKINFO.FreeSpace = nET_HARDDISK_STATE.stDisks[i].dwFreeSpace.ToString();
                dISKINFO.TotalSpace = nET_HARDDISK_STATE.stDisks[i].dwVolume.ToString();
                dISKINFO.DiskType = Enum.GetName(typeof(EM_DISK_TYPE), nET_HARDDISK_STATE.stDisks[i].dwStatus >> 4 & 0xF) + GetChnString((byte)(nET_HARDDISK_STATE.stDisks[i].dwStatus >> 4 & 0xF));
                if((nET_HARDDISK_STATE.stDisks[i].dwStatus & 0xF) == 0)
                {
                    dISKINFO.DiskStatus = "Hiberation(休眠)";
                }
                else if ((nET_HARDDISK_STATE.stDisks[i].dwStatus & 0xF) == 1)
                {
                    dISKINFO.DiskStatus = "Active(活动)";
                }
                else if ((nET_HARDDISK_STATE.stDisks[i].dwStatus & 0xF) == 2)
                {
                    dISKINFO.DiskStatus = "Malfunction(故障)";
                }
                dISKINFO.SubareaNumber = nET_HARDDISK_STATE.stDisks[i].bSubareaNum.ToString();
                if(nET_HARDDISK_STATE.stDisks[i].bSignal == 0)
                {
                    dISKINFO.Signal = "Local(本地)";
                }
                else if(nET_HARDDISK_STATE.stDisks[i].bSignal == 1)
                {
                    dISKINFO.Signal = "Remote(远程)";
                }
                DiskList.Add(dISKINFO);
            }
            list.ItemsSource = DiskList;
        }

        string GetChnString(byte value)
        {
            switch(value)
            {
                case (byte)EM_DISK_TYPE.READ_WRITE:
                    return "(读写)";
                case (byte)EM_DISK_TYPE.READ_ONLY:
                    return "(只读)";
                case (byte)EM_DISK_TYPE.BACKUP:
                    return "(备份)";
                case (byte)EM_DISK_TYPE.REDUNDANT:
                    return "(冗余)";
                case (byte)EM_DISK_TYPE.SNAPSHOT:
                    return "(快照)";
                default:
                    return "";
            }
        }
    }

    public class DISKINFO
    {
        public string DiskNumber { get; set; }
        public string FreeSpace { get; set; }
        public string TotalSpace { get; set; }
        public string DiskType { get; set; }
        public string DiskStatus { get; set; }
        public string SubareaNumber { get; set; }
        public string Signal { get; set; }
    }
}
