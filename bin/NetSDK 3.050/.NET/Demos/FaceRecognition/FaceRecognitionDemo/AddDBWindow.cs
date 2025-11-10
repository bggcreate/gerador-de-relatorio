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
    public partial class AddDBWindow : Form
    {
        private FaceRecognitionDemo m_MainWindow;
        private IntPtr m_LoginID = IntPtr.Zero;
        private const int m_WaitTime = 3000;

        public AddDBWindow(FaceRecognitionDemo _Form,IntPtr _LoginID)
        {
            InitializeComponent();
            m_MainWindow = _Form;
            m_LoginID = _LoginID;
            m_MainWindow.DeviceDisconnected += new Action(m_MainWindow_DeviceDisconnected);
        }

        void m_MainWindow_DeviceDisconnected()
        {
            m_LoginID = IntPtr.Zero;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            NET_IN_OPERATE_FACERECONGNITION_GROUP inParam = new NET_IN_OPERATE_FACERECONGNITION_GROUP();
            IntPtr stuPtr = IntPtr.Zero;
            bool ret = false;
            try
            {
                inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_OPERATE_FACERECONGNITION_GROUP));
                inParam.emOperateType = EM_OPERATE_FACERECONGNITION_GROUP_TYPE.ADD;

                NET_ADD_FACERECONGNITION_GROUP_INFO addParam = new NET_ADD_FACERECONGNITION_GROUP_INFO();
                stuPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_ADD_FACERECONGNITION_GROUP_INFO)));
                Marshal.StructureToPtr(addParam, stuPtr, true);
                addParam = (NET_ADD_FACERECONGNITION_GROUP_INFO)Marshal.PtrToStructure(stuPtr, typeof(NET_ADD_FACERECONGNITION_GROUP_INFO));
                addParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_ADD_FACERECONGNITION_GROUP_INFO));
                addParam.stuGroupInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_FACERECONGNITION_GROUP_INFO));
                addParam.stuGroupInfo.szGroupName = this.textBox_groupname.Text.Trim();
                Marshal.StructureToPtr(addParam, stuPtr, true);
                inParam.pOPerateInfo = stuPtr;

                NET_OUT_OPERATE_FACERECONGNITION_GROUP outParam = new NET_OUT_OPERATE_FACERECONGNITION_GROUP();
                outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_OPERATE_FACERECONGNITION_GROUP));
                ret = NETClient.OperateFaceRecognitionGroup(m_LoginID, ref inParam, ref outParam, m_WaitTime);
                if (!ret)
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
                Marshal.FreeHGlobal(stuPtr);
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
