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
using System.IO;

namespace WinForm_IPC_FaceRecognition_Demo
{
    public partial class AddPersonWindow : Form
    {
        private FaceRecognitionDemo m_MainWindow;
        private IntPtr m_LoginID;
        private NET_FACERECONGNITION_GROUP_INFO m_GroupInfo;
        private string m_Path = "";
        private const int m_WaitTime = 3000;

        public AddPersonWindow(FaceRecognitionDemo _Form,IntPtr _LoginID, NET_FACERECONGNITION_GROUP_INFO _GroupInfo)
        {
            InitializeComponent();
            m_MainWindow = _Form;
            m_LoginID = _LoginID;
            m_GroupInfo = _GroupInfo;
            m_MainWindow.DeviceDisconnected += new Action(m_MainWindow_DeviceDisconnected);
            this.Load += new EventHandler(AddPersonWindow_Load);
        }

        void m_MainWindow_DeviceDisconnected()
        {
            m_LoginID = IntPtr.Zero;
        }

        void AddPersonWindow_Load(object sender, EventArgs e)
        {
            this.textBox_groupid.Text = m_GroupInfo.szGroupId;
            this.textBox_groupname.Text = m_GroupInfo.szGroupName;
            this.comboBox_sex.Items.Add("Male(男)");
            this.comboBox_sex.Items.Add("Female(女)");
            this.comboBox_sex.SelectedIndex = 0;
            this.comboBox_idtype.Items.Add("ID Card(身份证)");
            this.comboBox_idtype.Items.Add("Passport(护照)");
            this.comboBox_idtype.SelectedIndex = 0;
        }

        private void button_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG|*.jpg";
            var ret = openFileDialog.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    m_Path = openFileDialog.FileName;
                    Image image = Image.FromFile(m_Path);
                    pictureBox_person.Image = image;
                    pictureBox_person.Refresh();
                    this.button_Open.Visible = false;
                    this.button_picture_del.Visible = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            openFileDialog.Dispose();
        }

        private void button_picture_del_Click(object sender, EventArgs e)
        {
            m_Path = "";
            pictureBox_person.Image = null;
            this.button_Open.Visible = true;
            this.button_picture_del.Visible = false;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            if (this.textBox_name.Text == "")
            {
                MessageBox.Show("Please input Name(请输入姓名)");
                return;
            }
            bool ret = false;

            NET_IN_OPERATE_FACERECONGNITIONDB stuInParam = new NET_IN_OPERATE_FACERECONGNITIONDB();
            try
            {
                stuInParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_OPERATE_FACERECONGNITIONDB));
                stuInParam.emOperateType = EM_OPERATE_FACERECONGNITIONDB_TYPE.ADD;//operate
                stuInParam.stPersonInfo.szPersonNameEx = this.textBox_name.Text.Trim();
                stuInParam.stPersonInfo.szID = this.textBox_id.Text.Trim();
                stuInParam.stPersonInfo.bySex = (byte)(this.comboBox_sex.SelectedIndex + 1);
                stuInParam.stPersonInfo.pszGroupID = Marshal.StringToHGlobalAnsi(m_GroupInfo.szGroupId);
                stuInParam.stPersonInfo.bGroupIdLen = (byte)m_GroupInfo.szGroupId.Length;
                stuInParam.stPersonInfo.pszGroupName = Marshal.StringToHGlobalAnsi(m_GroupInfo.szGroupName);
                stuInParam.stPersonInfo.byIDType = (byte)(this.comboBox_idtype.SelectedIndex + 1);
                if (dateTimePicker_birthday.Checked)
                {
                    stuInParam.stPersonInfo.wYear = (ushort)dateTimePicker_birthday.Value.Year;
                    stuInParam.stPersonInfo.byMonth = (byte)dateTimePicker_birthday.Value.Month;
                    stuInParam.stPersonInfo.byDay = (byte)dateTimePicker_birthday.Value.Day;
                }
                if ("" != m_Path)
                {
                    stuInParam.stPersonInfo.wFacePicNum = 1;
                    if (false == File.Exists(m_Path))
                    {
                        MessageBox.Show("Picture path error(图片路径错误)!");
                        return;
                    }
                    byte[] data = File.ReadAllBytes(m_Path);
                    stuInParam.stPersonInfo.szFacePicInfo = new NET_PIC_INFO[48];
                    for (int i = 0; i < 48; i++)
                    {
                        stuInParam.stPersonInfo.szFacePicInfo[i] = new NET_PIC_INFO();
                    }
                    stuInParam.stPersonInfo.szFacePicInfo[0].dwFileLenth = (uint)data.Length;
                    stuInParam.stPersonInfo.szFacePicInfo[0].dwOffSet = 0;
                    stuInParam.nBufferLen = data.Length;
                    if (0 == stuInParam.nBufferLen)
                    {
                        stuInParam.pBuffer = IntPtr.Zero;
                    }
                    else
                    {
                        stuInParam.pBuffer = Marshal.AllocHGlobal(stuInParam.nBufferLen);
                        Marshal.Copy(data, 0, stuInParam.pBuffer, stuInParam.nBufferLen);
                    }
                }
                else
                {
                    MessageBox.Show("Please upload picture(请上传图片)!");
                    return;
                }


                NET_OUT_OPERATE_FACERECONGNITIONDB stuOutParam = new NET_OUT_OPERATE_FACERECONGNITIONDB();
                stuOutParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_OPERATE_FACERECONGNITIONDB));

                ret = NETClient.OperateFaceRecognitionDB(m_LoginID, ref stuInParam, ref stuOutParam, m_WaitTime);
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
                Marshal.FreeHGlobal(stuInParam.stPersonInfo.pszGroupID);
                Marshal.FreeHGlobal(stuInParam.stPersonInfo.pszGroupName);
                Marshal.FreeHGlobal(stuInParam.pBuffer);
            }
            
            if (ret)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void textBox_name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Encoding.Default.GetBytes(((TextBox)sender).Text).Length > 61 && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBox_id_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Encoding.Default.GetBytes(((TextBox)sender).Text).Length > 29 && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
    }
}
