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

namespace DeviceControl
{
    public partial class DeviceControl : Form
    {
        private IntPtr _LoginID = IntPtr.Zero;
        private NET_DEVICEINFO_Ex _DeviceInfo = new NET_DEVICEINFO_Ex();
        private fDisConnectCallBack _DisConnectCallBack;

        public DeviceControl()
        {
            InitializeComponent();
            try
            {
                _DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
                NETClient.Init(_DisConnectCallBack, IntPtr.Zero, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            this.Load += new EventHandler(DeviceControl_Load);
        }

        void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            if (_LoginID == lLoginID)
            {
                NETClient.Logout(_LoginID);
                _LoginID = IntPtr.Zero;
                this.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show("Device is offline(设备已离线)");
                    NETClient.Logout(_LoginID);
                    _LoginID = IntPtr.Zero;
                    this.button_login.Text = "Login(登录)";
                    this.button_reboot.Enabled = false;
                    this.dateTimePicker_set.Enabled = false;
                    this.button_get.Enabled = false;
                    this.button_set.Enabled = false;
                    this.checkBox_now.Checked = false;
                    this.checkBox_now.Enabled = false;
                }));
            }
        }

        void DeviceControl_Load(object sender, EventArgs e)
        {
            this.button_reboot.Enabled = false;
            this.dateTimePicker_set.Enabled = false;
            this.button_get.Enabled = false;
            this.button_set.Enabled = false;
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
                _LoginID = NETClient.Login(textBox_ip.Text.Trim(), port, textBox_username.Text.Trim(), textBox_passwrod.Text, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref _DeviceInfo);
                if (IntPtr.Zero == _LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_login.Text = "Logout(登出)";
                this.button_reboot.Enabled = true;
                this.dateTimePicker_set.Enabled = true;
                this.button_get.Enabled = true;
                this.button_set.Enabled = true;
                this.checkBox_now.Enabled = true;
            }
            else
            {
                NETClient.Logout(_LoginID);
                _LoginID = IntPtr.Zero;
                this.button_login.Text = "Login(登录)";
                this.button_reboot.Enabled = false;
                this.dateTimePicker_set.Enabled = false;
                this.button_get.Enabled = false;
                this.button_set.Enabled = false;
                this.checkBox_now.Checked = false;
                this.checkBox_now.Enabled = false;
            }
        }

        private void button_reboot_Click(object sender, EventArgs e)
        {
            var res =  MessageBox.Show("Confirm reboot(确认重启)", "Warning(警告)", MessageBoxButtons.YesNo);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                bool ret = NETClient.RebootDev(_LoginID);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                }
                else
                {
                    MessageBox.Show("Successfully reboot(重启成功)");
                }
            }
        }

        private void button_get_Click(object sender, EventArgs e)
        {
            NET_TIME time = new NET_TIME();
            uint ret = 0;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_TIME)));
                Marshal.StructureToPtr(time, inPtr, true);
                bool result = NETClient.GetDevConfig(_LoginID, EM_DEV_CFG_TYPE.TIMECFG, -1, inPtr, (uint)Marshal.SizeOf(typeof(NET_TIME)), ref ret, 5000);
                if (result && ret == (uint)Marshal.SizeOf(typeof(NET_TIME)))
                {
                    time = (NET_TIME)Marshal.PtrToStructure(inPtr, typeof(NET_TIME));
                    this.dateTimePicker_get.Value = time.ToDateTime();
                }
                else
                {
                    MessageBox.Show(NETClient.GetLastError());
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

        private void button_set_Click(object sender, EventArgs e)
        {
            NET_TIME time;
            if (checkBox_now.Checked)
            {
                time = NET_TIME.FromDateTime(DateTime.Now);
            }
            else
            {
                time = NET_TIME.FromDateTime(this.dateTimePicker_set.Value);
            }
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_TIME)));
                Marshal.StructureToPtr(time, inPtr, true);
                bool result = NETClient.SetDevConfig(_LoginID, EM_DEV_CFG_TYPE.TIMECFG, -1, inPtr, (uint)Marshal.SizeOf(typeof(NET_TIME)), 5000);
                if (!result)
                {
                    MessageBox.Show(NETClient.GetLastError());
                }
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

        private void checkBox_now_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_now.Checked)
            {
                this.dateTimePicker_set.Enabled = false;
            }
            else
            {
                if (_LoginID != IntPtr.Zero)
                {
                    this.dateTimePicker_set.Enabled = true;
                }
                else
                {
                    this.dateTimePicker_set.Enabled = false;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NETClient.Cleanup();
        }

       
    }
}
