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
    public partial class ModifyPersonWindow : Form
    {
        private FaceRecognitionDemo m_MainWindow;
        private const int m_WaitTime = 3000;
        private IntPtr m_LoginID = IntPtr.Zero;
        private NET_FACERECONGNITION_GROUP_INFO m_GroupInfo;
        private NET_CANDIDATE_INFO m_PersonInfo;
        private string m_Path = "";
        private string m_FilePath = AppDomain.CurrentDomain.BaseDirectory + "temp.jpg";

        public ModifyPersonWindow(FaceRecognitionDemo _Form, IntPtr _LoginID, NET_FACERECONGNITION_GROUP_INFO _GroupInfo, NET_CANDIDATE_INFO _PersonInfo)
        {
            InitializeComponent();
            m_MainWindow = _Form;
            m_LoginID = _LoginID;
            m_GroupInfo = _GroupInfo;
            m_PersonInfo = _PersonInfo;
            m_MainWindow.DeviceDisconnected += new Action(m_MainWindow_DeviceDisconnected);
            this.Load += new EventHandler(ModifyPersonWindow_Load);
        }

        void m_MainWindow_DeviceDisconnected()
        {
            m_LoginID = IntPtr.Zero;
        }

        void ModifyPersonWindow_Load(object sender, EventArgs e)
        {
            this.comboBox_sex.Items.Add("");
            this.comboBox_sex.Items.Add("Male(男)");
            this.comboBox_sex.Items.Add("Female(女)");
            this.comboBox_idtype.Items.Add("");
            this.comboBox_idtype.Items.Add("ID Card(身份证)");
            this.comboBox_idtype.Items.Add("Passport(护照)");
            this.textBox_groupid.Text = m_GroupInfo.szGroupId;
            this.textBox_groupname.Text = m_GroupInfo.szGroupName;
            this.textBox_uid.Text = m_PersonInfo.stPersonInfo.szUID;
            this.textBox_name.Text = m_PersonInfo.stPersonInfo.szPersonNameEx;
            this.textBox_id.Text = m_PersonInfo.stPersonInfo.szID;
            this.comboBox_sex.SelectedIndex = m_PersonInfo.stPersonInfo.bySex;
            this.comboBox_idtype.SelectedIndex = m_PersonInfo.stPersonInfo.byIDType;
            try
            {
                this.dateTimePicker_birthday.Checked = true;
                this.dateTimePicker_birthday.Value = new DateTime(m_PersonInfo.stPersonInfo.wYear, m_PersonInfo.stPersonInfo.byMonth, m_PersonInfo.stPersonInfo.byDay);
            }
            catch
            {
                this.dateTimePicker_birthday.Checked = false;
            }
            DownloadAndShowPic();
        }

        private void DownloadAndShowPic()
        {
            NET_IN_DOWNLOAD_REMOTE_FILE inParam = new NET_IN_DOWNLOAD_REMOTE_FILE();
            inParam.pszFileName = m_PersonInfo.stPersonInfo.szFacePicInfo[0].pszFilePath;
            inParam.pszFileDst = Marshal.StringToHGlobalAnsi(m_FilePath);
            bool ret = NETClient.DownloadRemoteFile(m_LoginID, inParam, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            byte[] picInfo = File.ReadAllBytes(m_FilePath);
            try
            {
                FileInfo fi = new FileInfo(m_FilePath);
                fi.Delete();
                m_FilePath = "";
            }
            catch
            {
            }
            using (MemoryStream stream = new MemoryStream(picInfo))
            {
                try // add try catch for catch exception when the stream is not image format,and the stream is from device.
                {
                    this.pictureBox_person.Image = Image.FromStream(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            this.button_Open.Visible = false;
            this.button_picture_del.Visible = true;
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
                stuInParam.emOperateType = EM_OPERATE_FACERECONGNITIONDB_TYPE.MODIFY;//operate
                stuInParam.stPersonInfo.szPersonNameEx = this.textBox_name.Text.Trim();
                stuInParam.stPersonInfo.szID = this.textBox_id.Text.Trim();
                stuInParam.stPersonInfo.bySex = (byte)this.comboBox_sex.SelectedIndex;
                stuInParam.stPersonInfo.pszGroupID = Marshal.StringToHGlobalAnsi(m_GroupInfo.szGroupId);
                stuInParam.stPersonInfo.bGroupIdLen = (byte)m_GroupInfo.szGroupId.Length;
                stuInParam.stPersonInfo.pszGroupName = Marshal.StringToHGlobalAnsi(m_GroupInfo.szGroupName);
                stuInParam.stPersonInfo.byIDType = (byte)this.comboBox_idtype.SelectedIndex;
                if (dateTimePicker_birthday.Checked)
                {
                    stuInParam.stPersonInfo.wYear = (ushort)dateTimePicker_birthday.Value.Year;
                    stuInParam.stPersonInfo.byMonth = (byte)dateTimePicker_birthday.Value.Month;
                    stuInParam.stPersonInfo.byDay = (byte)dateTimePicker_birthday.Value.Day;
                }
                stuInParam.stPersonInfo.szUID = this.textBox_uid.Text.Trim();
                if ("" != m_Path)
                {
                    stuInParam.stPersonInfo.wFacePicNum = 1;
                    if (false == File.Exists(m_Path))
                    {
                        MessageBox.Show("Picture path error(图片路径错误)!");
                        return;
                    }
                    byte[] data = File.ReadAllBytes(m_Path);
                    FileInfo fileInfo = new FileInfo(m_Path);
                    stuInParam.stPersonInfo.szFacePicInfo = new NET_PIC_INFO[48];
                    for (int i = 0; i < 48; i++)
                    {
                        stuInParam.stPersonInfo.szFacePicInfo[i] = new NET_PIC_INFO();
                    }
                    stuInParam.stPersonInfo.szFacePicInfo[0].dwFileLenth = (uint)fileInfo.Length;
                    stuInParam.stPersonInfo.szFacePicInfo[0].dwOffSet = 0;
                    stuInParam.nBufferLen = (int)fileInfo.Length;
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
                    stuInParam.stPersonInfo.wFacePicNum = 0;
                    stuInParam.pBuffer = IntPtr.Zero;
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
            if (pictureBox_person.Image != null)
            {
                pictureBox_person.Image.Dispose();
            }
            pictureBox_person.Image = null;
            pictureBox_person.Refresh();
            this.button_Open.Visible = true;
            this.button_picture_del.Visible = false;
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (this.pictureBox_person.Image != null)
            {
                this.pictureBox_person.Image.Dispose();
            }
            this.pictureBox_person.Image = null;
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
