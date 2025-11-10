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

namespace WinForm_IPC_FaceRecognition_Demo
{
    public partial class ModifyDBWindow : Form
    {
        private FaceRecognitionDemo m_MainWindow;
        private IntPtr m_LoginID = IntPtr.Zero;
        private NET_FACERECONGNITION_GROUP_INFO m_GroupInfo;
        private const int m_WaitTime = 3000;

        public ModifyDBWindow(FaceRecognitionDemo _Form, IntPtr _LoginID, NET_FACERECONGNITION_GROUP_INFO _GroupInfo)
        {
            InitializeComponent();
            m_MainWindow = _Form;
            m_LoginID = _LoginID;
            m_GroupInfo = _GroupInfo;
            m_MainWindow.DeviceDisconnected += new Action(m_MainWindow_DeviceDisconnected);
            this.Load += new EventHandler(ModifyDBWindow_Load);
        }

        void m_MainWindow_DeviceDisconnected()
        {
            m_LoginID = IntPtr.Zero;
        }

        void ModifyDBWindow_Load(object sender, EventArgs e)
        {
            this.textBox_groupname.Text = m_GroupInfo.szGroupName;
        }

        private void textBox_value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            IntPtr modifyPtr = IntPtr.Zero;
            bool ret = false;
            try
            {
                NET_IN_OPERATE_FACERECONGNITION_GROUP inParam = new NET_IN_OPERATE_FACERECONGNITION_GROUP();
                inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_OPERATE_FACERECONGNITION_GROUP));
                inParam.emOperateType = EM_OPERATE_FACERECONGNITION_GROUP_TYPE.MODIFY;

                NET_MODIFY_FACERECONGNITION_GROUP_INFO modifyInfo = new NET_MODIFY_FACERECONGNITION_GROUP_INFO();
                modifyInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_MODIFY_FACERECONGNITION_GROUP_INFO));
                modifyInfo.stuGroupInfo = m_GroupInfo;
                modifyInfo.stuGroupInfo.szGroupName = this.textBox_groupname.Text.Trim();
                modifyPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_MODIFY_FACERECONGNITION_GROUP_INFO)));
                Marshal.StructureToPtr(modifyInfo, modifyPtr, true);

                inParam.pOPerateInfo = modifyPtr;

                NET_OUT_OPERATE_FACERECONGNITION_GROUP outParam = new NET_OUT_OPERATE_FACERECONGNITION_GROUP();
                outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_OPERATE_FACERECONGNITION_GROUP));
                ret = NETClient.OperateFaceRecognitionGroup(m_LoginID, ref inParam, ref outParam, m_WaitTime);
                if (!ret)
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
                Marshal.FreeHGlobal(modifyPtr);
            }
            if (ret)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void textBox_groupname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Encoding.Default.GetBytes(((TextBox)sender).Text).Length > 125 && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
    }
}
