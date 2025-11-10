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
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace DeviceInit
{
    public partial class DeviceInitDemo : Form
    {
        private IntPtr _SearchID = IntPtr.Zero;
        private fSearchDevicesCB _SearchDevicesCB;
        private List<Device> _DeviceList = new List<Device>();
        private int _DeviceCount = 0;

        public DeviceInitDemo()
        {
            InitializeComponent();
            try
            {
                _SearchDevicesCB = new fSearchDevicesCB(SearchDevicesCB);
                NETClient.Init(null, IntPtr.Zero, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            _DeviceList.Clear();
            listView_device.Items.Clear();
            _DeviceCount = 0;
            _SearchID = NETClient.StartSearchDevice(_SearchDevicesCB, IntPtr.Zero, IntPtr.Zero);
            if (_SearchID == IntPtr.Zero)
            {
                MessageBox.Show(NETClient.GetLastError());
            }
        }

        private void SearchDevicesCB(IntPtr pDevNetInfo, IntPtr pUserData)
        {
            DEVICE_NET_INFO_EX info = (DEVICE_NET_INFO_EX)Marshal.PtrToStructure(pDevNetInfo, typeof(DEVICE_NET_INFO_EX));
            this.BeginInvoke(new Action<DEVICE_NET_INFO_EX>(UpdateSearchUI), info);
        }

        private void UpdateSearchUI(DEVICE_NET_INFO_EX info)
        {
            Device item = _DeviceList.Find(p => p.DeviceInfo.szMac == info.szMac);
            if (item == null)
            {
                _DeviceCount++;
                Device dev = new Device();
                dev.DeviceInfo = info;
                _DeviceList.Add(dev);

                var viewItem = new ListViewItem();
                viewItem.Text = _DeviceCount.ToString();
                if ((info.byInitStatus & 0x1) == 1)
                {
                    viewItem.BackColor = Color.Red;
                    viewItem.SubItems.Add("Uninitialized(未初始化)");
                }
                else
                {
                    viewItem.BackColor = Color.White;
                    viewItem.SubItems.Add("Initialized(已初始化)");
                }
                viewItem.SubItems.Add(info.iIPVersion.ToString());
                viewItem.SubItems.Add(info.szIP);
                viewItem.SubItems.Add(info.nPort.ToString());
                viewItem.SubItems.Add(info.szSubmask);
                viewItem.SubItems.Add(info.szGateway);
                viewItem.SubItems.Add(info.szMac);
                viewItem.SubItems.Add(info.szDeviceType);
                viewItem.SubItems.Add(info.szNewDetailType);
                viewItem.SubItems.Add(info.nHttpPort.ToString());
                listView_device.BeginUpdate();
                listView_device.Items.Add(viewItem);
                listView_device.EndUpdate();
            }
        }

        private void button_init_Click(object sender, EventArgs e)
        {
            if (listView_device.SelectedItems.Count == 0)
            {
                MessageBox.Show(this,"Please select an Uninitialized device(请选择一个未初始化的设备)");
                return;
            }
            Device dev = _DeviceList[listView_device.SelectedItems[0].Index];
            if ((dev.DeviceInfo.byInitStatus & 0x1) != 1)
            {
                MessageBox.Show(this, "Please select an Uninitialized device(请选择一个未初始化的设备)");
                return;
            }
            InitDeviceDialog initDeviceDialog = new InitDeviceDialog(dev);
            var ret = initDeviceDialog.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                Task task = new Task(() => 
                {
                    NET_IN_INIT_DEVICE_ACCOUNT inParam = new NET_IN_INIT_DEVICE_ACCOUNT();
                    inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_INIT_DEVICE_ACCOUNT));
                    if (initDeviceDialog.IsEmail)
                    {
                        inParam.szMail = initDeviceDialog.RestStr;
                    }
                    else
                    {
                        inParam.szCellPhone = initDeviceDialog.RestStr;
                    }
                    inParam.szMac = dev.DeviceInfo.szMac;
                    inParam.szUserName = initDeviceDialog.UserName;
                    if (initDeviceDialog.Passwrod.Length > 127)
                    {
                        string password = initDeviceDialog.Passwrod.Substring(0, 127);
                        inParam.szPwd = password;
                    }
                    else
                    {
                        inParam.szPwd = initDeviceDialog.Passwrod;
                    }
                    inParam.byPwdResetWay = dev.DeviceInfo.byPwdResetWay;
                    NET_OUT_INIT_DEVICE_ACCOUNT outParam = new NET_OUT_INIT_DEVICE_ACCOUNT();
                    outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_INIT_DEVICE_ACCOUNT));
                    bool res = NETClient.InitDevAccount(inParam, ref outParam, 5000, null);
                    if (!res)
                    {
                        string errormsg = NETClient.GetLastError();
                        this.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(this,errormsg);
                        }));
                    }
                    else
                    {
                        this.BeginInvoke(new Action(() => 
                        {
                            listView_device.SelectedItems[0].BackColor = Color.White;
                            listView_device.SelectedItems[0].SubItems[1].Text = "Initialized(已初始化)";
                        }));
                        DEVICE_NET_INFO_EX info = new DEVICE_NET_INFO_EX();
                        info = dev.DeviceInfo;
                        info.byInitStatus = 2;
                        Device device = new Device();
                        device.DeviceInfo = info;
                        _DeviceList.Remove(dev);
                        _DeviceList.Insert(listView_device.SelectedItems[0].Index, device);
                    }
                });
                task.Start();
            }
            initDeviceDialog.Dispose();
        }

        private void button_searchbypoint_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero != _SearchID)
            {
                NETClient.StopSearchDevice(_SearchID);
            }
            PointSetDialog setIPDialog = new PointSetDialog();
            var ret = setIPDialog.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                _DeviceList.Clear();
                listView_device.Items.Clear();
                _DeviceCount = 0;
                NET_DEVICE_IP_SEARCH_INFO info = new NET_DEVICE_IP_SEARCH_INFO();
                info.dwSize = (uint)Marshal.SizeOf(typeof(NET_DEVICE_IP_SEARCH_INFO));
                info.nIpNum = setIPDialog.IPCount;
                info.szIPs = new NET_IPADDRESS[256];
                for (int i = 0; i < setIPDialog.IPCount; i++)
                {
                    info.szIPs[i].szIP = setIPDialog.IPList[i];
                }
                Task task = new Task(() => 
                {
                    bool res = NETClient.SearchDevicesByIPs(info, _SearchDevicesCB, IntPtr.Zero, null, 10000);
                    if (!res)
                    {
                        string errormsg = NETClient.GetLastError();
                        this.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(this,errormsg);
                        }));
                        return;
                    }
                });
                task.Start();
            }
            setIPDialog.Dispose();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (IntPtr.Zero != _SearchID)
            {
                NETClient.StopSearchDevice(_SearchID);
            }
            NETClient.Cleanup();
        }
    }

    public class Device
    {
        public DEVICE_NET_INFO_EX DeviceInfo { get; set; }
    }

}
