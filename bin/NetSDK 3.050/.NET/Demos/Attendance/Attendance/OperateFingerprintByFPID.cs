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

namespace Attendance
{
    public partial class OperateFingerprintByFPID : Form
    {
        IntPtr _LoginID;
        public OperateFingerprintByFPID(IntPtr loginID)
        {
            InitializeComponent();
            _LoginID = loginID;
            this.label_userid.Text = "";
            this.label_fingerprintdata.Text = "";
        }

        private void textBox_id_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button_get_Click(object sender, EventArgs e)
        {
            if (textBox_id.Text == "")
            {
                MessageBox.Show("Please input fingerprint ID(请输入指纹ID)");
                return;
            }
            int id = 0;
            try
            {
                id = Convert.ToInt32(this.textBox_id.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Input error(输入错误)");
                return;
            }
            this.label_fingerprintdata.Text = "";
            this.label_userid.Text = "";
            NET_CTRL_IN_FINGERPRINT_GET inParam = new NET_CTRL_IN_FINGERPRINT_GET();
            NET_CTRL_OUT_FINGERPRINT_GET outParam = new NET_CTRL_OUT_FINGERPRINT_GET();
            try
            {
                inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_IN_FINGERPRINT_GET));
                inParam.nFingerPrintID = id;
                outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_OUT_FINGERPRINT_GET));
                outParam.nMaxFingerDataLength = 1024 * 100;
                outParam.szFingerPrintInfo = Marshal.AllocHGlobal(100 * 1024);
                bool ret = NETClient.Attendance_GetFingerRecord(_LoginID, ref inParam, ref outParam, 5000);
                if (!ret)
                {
                    MessageBox.Show("Search Failed(查询失败)");
                }
                else
                {
                    if (outParam.nRetLength > 0)
                    {
                        this.label_userid.Text = outParam.szUserID;
                        byte[] data = new byte[outParam.nRetLength];
                        Marshal.Copy(outParam.szFingerPrintInfo, data, 0, outParam.nRetLength);
                        this.label_fingerprintdata.Text = Convert.ToBase64String(data);
                    }
                    else
                    {
                        this.label_userid.Text = "";
                        this.label_fingerprintdata.Text = "";
                        MessageBox.Show("No fingerprint data(没有指纹数据)");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(outParam.szFingerPrintInfo);
            }
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (textBox_id.Text == "")
            {
                MessageBox.Show("Please input fingerprint ID(请输入指纹ID)");
                return;
            }
            int id = 0;
            try
            {
                id = Convert.ToInt32(this.textBox_id.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Input error(输入错误)");
                return;
            }

            NET_CTRL_IN_FINGERPRINT_REMOVE inParam = new NET_CTRL_IN_FINGERPRINT_REMOVE();
            NET_CTRL_OUT_FINGERPRINT_REMOVE outParam = new NET_CTRL_OUT_FINGERPRINT_REMOVE();
            inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_IN_FINGERPRINT_REMOVE));
            outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_OUT_FINGERPRINT_REMOVE));
            inParam.nFingerPrintID = id;
            bool ret = NETClient.Attendance_RemoveFingerRecord(_LoginID, ref inParam, ref outParam, 5000);
            if (!ret)
            {
                MessageBox.Show("Delete fingerprint Failed(删除指纹失败)");
            }
            else
            {
                MessageBox.Show("Delete fingerprint Successfully(删除指纹成功)");
                this.label_userid.Text = "";
                this.label_fingerprintdata.Text = "";
            }
        }


    }
}
