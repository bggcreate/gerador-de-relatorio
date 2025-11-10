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
using System.Threading;

namespace WinForm_IPC_FaceRecognition_Demo
{
    public partial class OperatePersonWindow : Form
    {
        private FaceRecognitionDemo m_MainWindow;
        private IntPtr m_LoginID;
        private NET_FACERECONGNITION_GROUP_INFO m_GroupInfo;
        private const int m_WaitTime = 3000;
        private int m_WaitToken = 0; 
        private IntPtr m_AttachFindStateID = IntPtr.Zero;
        private bool m_IsFindFinishFlag = false;
        private IntPtr m_FindID = IntPtr.Zero;
        private const int m_ShowLine = 20;
        private int m_TotalCount = 0;
        private ListViewItem m_ListViewItem;
        private int m_CurrentCount = 0;
        private List<NET_CANDIDATE_INFO> m_CurrentPersonInfoList = new List<NET_CANDIDATE_INFO>();

        public OperatePersonWindow(FaceRecognitionDemo _Form, IntPtr _LoginID, NET_FACERECONGNITION_GROUP_INFO _GroupInfo)
        {
            InitializeComponent();
            m_MainWindow = _Form;
            m_LoginID = _LoginID;
            m_GroupInfo = _GroupInfo;
            m_MainWindow.DeviceDisconnected += new Action(m_MainWindow_DeviceDisconnected);
            this.Load += new EventHandler(OperatePersonWindow_Load);
        }

        void m_MainWindow_DeviceDisconnected()
        {
            m_LoginID = IntPtr.Zero;
        }

        void OperatePersonWindow_Load(object sender, EventArgs e)
        {
            this.button_PrePage.Enabled = false;
            this.button_NextPage.Enabled = false;
            this.textBox_groupid.Text = m_GroupInfo.szGroupId;
            this.textBox_groupname.Text = m_GroupInfo.szGroupName;
            this.comboBox_sex.Items.Add("All(所有)");
            this.comboBox_sex.Items.Add("Male(男)");
            this.comboBox_sex.Items.Add("Female(女)");
            this.comboBox_sex.SelectedIndex = 0;
            this.comboBox_idtype.Items.Add("All(所有)");
            this.comboBox_idtype.Items.Add("ID Card(身份证)");
            this.comboBox_idtype.Items.Add("Passport(护照)");
            this.comboBox_idtype.SelectedIndex = 0;
            this.dateTimePicker_starttime.Value = new DateTime(1900, 01, 01);
        }

        private void button_Search_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            m_CurrentCount = 0;
            this.button_PrePage.Enabled = false;
            this.button_NextPage.Enabled = false;
            if (IntPtr.Zero != m_FindID)
            {
                NETClient.StopFindFaceRecognition(m_FindID);
            }
            NET_IN_STARTFIND_FACERECONGNITION stuInParam = new NET_IN_STARTFIND_FACERECONGNITION();
            IntPtr stuPtr = IntPtr.Zero;
            bool ret = false;
            NET_OUT_STARTFIND_FACERECONGNITION stuOutParam = new NET_OUT_STARTFIND_FACERECONGNITION();
            stuOutParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_STARTFIND_FACERECONGNITION));
            try
            {
                stuPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_STARTFIND_FACERECONGNITION)));
                Marshal.StructureToPtr(stuInParam, stuPtr, true);
                stuInParam = (NET_IN_STARTFIND_FACERECONGNITION)Marshal.PtrToStructure(stuPtr, typeof(NET_IN_STARTFIND_FACERECONGNITION));

                stuInParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_STARTFIND_FACERECONGNITION));
                stuInParam.bPersonEnable = true;
                stuInParam.stMatchOptions.dwSize = (uint)Marshal.SizeOf(typeof(NET_FACE_MATCH_OPTIONS));
                stuInParam.stFilterInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_FACE_FILTER_CONDTION));
                stuInParam.stFilterInfo.nGroupIdNum = 1;
                stuInParam.stFilterInfo.szGroupId[0].szGroupId = m_GroupInfo.szGroupId;
                if (dateTimePicker_starttime.Checked)
                {
                    stuInParam.stFilterInfo.stBirthdayRangeStart = NET_TIME.FromDateTime(new DateTime(dateTimePicker_starttime.Value.Year, dateTimePicker_starttime.Value.Month, dateTimePicker_starttime.Value.Day));
                }
                if (dateTimePicker_endtime.Checked)
                {
                    stuInParam.stFilterInfo.stBirthdayRangeEnd = NET_TIME.FromDateTime(new DateTime(dateTimePicker_endtime.Value.Year, dateTimePicker_endtime.Value.Month, dateTimePicker_endtime.Value.Day));
                }
                stuInParam.nChannelID = 0;
                stuInParam.stPerson.pszGroupID = Marshal.StringToHGlobalAnsi(m_GroupInfo.szGroupId);
                stuInParam.stPerson.szID = this.textBox_id.Text.Trim();
                stuInParam.stPerson.szPersonNameEx = this.textBox_name.Text.Trim();
                stuInParam.stPerson.bySex = (byte)this.comboBox_sex.SelectedIndex;
                stuInParam.stPerson.byIDType = (byte)this.comboBox_idtype.SelectedIndex;
                ret = NETClient.StartFindFaceRecognition(m_LoginID, ref stuInParam, ref stuOutParam, m_WaitTime);
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
                Marshal.FreeHGlobal(stuPtr);
                Marshal.FreeHGlobal(stuInParam.stPerson.pszGroupID);
            }
            if (!ret)
            {
                return;
            }
            if (0 == stuOutParam.nTotalCount)
            {
                listView_person.Items.Clear();
                NETClient.StopFindFaceRecognition(stuOutParam.lFindHandle);
                MessageBox.Show("No data found(没有数据)!");
                return;
            }
            m_TotalCount = stuOutParam.nTotalCount;
            if (-1 == stuOutParam.nTotalCount)
            {
                if (!WaitProcessSuccess(stuOutParam.nToken))
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
            }
            m_FindID = stuOutParam.lFindHandle;
            FindFaceAndShow(stuOutParam.lFindHandle, 0);
            if (m_ShowLine > stuOutParam.nTotalCount)
            {
                this.button_PrePage.Enabled = false;
                this.button_NextPage.Enabled = false;
                NETClient.StopFindFaceRecognition(stuOutParam.lFindHandle);
                m_FindID = IntPtr.Zero;
            }
            else
            {
                this.button_PrePage.Enabled = false;
                this.button_NextPage.Enabled = true;
            }
        }

        private int FindFaceAndShow(IntPtr findID, int startNumber)
        {
            listView_person.Items.Clear();
            foreach (NET_CANDIDATE_INFO info in m_CurrentPersonInfoList)
            {
                if (info.stPersonInfo.szFacePicInfo[0].pszFilePath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(info.stPersonInfo.szFacePicInfo[0].pszFilePath);
                }
            }
            m_CurrentPersonInfoList.Clear();
            NET_IN_DOFIND_FACERECONGNITION stuFindIn = new NET_IN_DOFIND_FACERECONGNITION();
            stuFindIn.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_DOFIND_FACERECONGNITION));

            stuFindIn.lFindHandle = findID;
            stuFindIn.nCount = m_ShowLine;
            stuFindIn.nBeginNum = startNumber; 

            NET_OUT_DOFIND_FACERECONGNITION stuFindOut = new NET_OUT_DOFIND_FACERECONGNITION();
            stuFindOut.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_DOFIND_FACERECONGNITION));
            stuFindOut.stCadidateInfo = new NET_CANDIDATE_INFO[20];
            for (int i = 0; i < m_ShowLine; i++)
            {
                stuFindOut.stCadidateInfo[i].stPersonInfo.szFacePicInfo = new NET_PIC_INFO[48];
                stuFindOut.stCadidateInfo[i].stPersonInfo.szFacePicInfo[0].pszFilePath = Marshal.AllocHGlobal(256);
                stuFindOut.stCadidateInfo[i].stPersonInfo.szFacePicInfo[0].nFilePathLen = 256;
            }
            
            bool res = NETClient.DoFindFaceRecognition(ref stuFindIn, ref stuFindOut, m_WaitTime); 
            if (!res)
            {
                MessageBox.Show(NETClient.GetLastError());
                return -1;
            }
            for(int i = 0 ; i < stuFindOut.nCadidateNum; i++)
            {
                m_CurrentPersonInfoList.Add(stuFindOut.stCadidateInfo[i]);
                m_ListViewItem = new ListViewItem();
                m_ListViewItem.Text = stuFindOut.stCadidateInfo[i].stPersonInfo.szUID;
                m_ListViewItem.SubItems.Add(stuFindOut.stCadidateInfo[i].stPersonInfo.szPersonNameEx);
                if (1 == stuFindOut.stCadidateInfo[i].stPersonInfo.bySex)
                {
                    m_ListViewItem.SubItems.Add("Male(男)");
                }
                else if (2 == stuFindOut.stCadidateInfo[i].stPersonInfo.bySex)
                {
                    m_ListViewItem.SubItems.Add("Female(女)");
                }
                else
                {
                    m_ListViewItem.SubItems.Add("");
                }
                m_ListViewItem.SubItems.Add(string.Format("{0}-{1}-{2}",stuFindOut.stCadidateInfo[i].stPersonInfo.wYear.ToString("D4"),stuFindOut.stCadidateInfo[i].stPersonInfo.byMonth.ToString("D2"),stuFindOut.stCadidateInfo[i].stPersonInfo.byDay.ToString("D2")));
                if (1 == stuFindOut.stCadidateInfo[i].stPersonInfo.byIDType)
                {
                    m_ListViewItem.SubItems.Add("ID Card(身份证)");
                }
                else if (2 == stuFindOut.stCadidateInfo[i].stPersonInfo.byIDType)
                {
                    m_ListViewItem.SubItems.Add("Passport(护照)");
                }
                else
                {
                    m_ListViewItem.SubItems.Add("");
                }
                m_ListViewItem.SubItems.Add(stuFindOut.stCadidateInfo[i].stPersonInfo.szID);

                listView_person.BeginUpdate();
                listView_person.Items.Add(m_ListViewItem);
                listView_person.EndUpdate();
            }
            return stuFindOut.nCadidateNum;
        }

        private bool WaitProcessSuccess(int nToken)
        {
            m_WaitToken = nToken;
            NET_IN_FACE_FIND_STATE stInParam = new NET_IN_FACE_FIND_STATE();
            try
            {
                stInParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_FACE_FIND_STATE));
                stInParam.cbFaceFindState = new fFaceFindStateCallBack(FaceFindState);
                stInParam.nTokenNum = 1;
                int[] iTmp = new int[stInParam.nTokenNum];
                iTmp[0] = nToken;
                stInParam.nTokens = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * stInParam.nTokenNum);
                for (int i = 0; i < stInParam.nTokenNum; ++i)
                {
                    Marshal.StructureToPtr(iTmp[i], stInParam.nTokens + i * Marshal.SizeOf(typeof(int)), true);
                }

                NET_OUT_FACE_FIND_STATE stOutParam = new NET_OUT_FACE_FIND_STATE();
                stOutParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_FACE_FIND_STATE));

                m_AttachFindStateID = NETClient.AttachFaceFindState(m_LoginID, ref stInParam, ref stOutParam, m_WaitTime);
                if (IntPtr.Zero == m_AttachFindStateID)
                {
                    return false;
                }

                while (!m_IsFindFinishFlag)
                {
                    Thread.Sleep(500);
                }
                m_IsFindFinishFlag = false;
                NETClient.DetachFaceFindState(m_AttachFindStateID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(stInParam.nTokens);
            }
            return true;
        }

        private void FaceFindState(IntPtr lLoginID, IntPtr lAttachHandle, IntPtr pstStates, Int32 nStateNum, UInt32 dwUser)
        {
            if (lAttachHandle == m_AttachFindStateID)
            {
                NET_CB_FACE_FIND_STATE[] sTmp = new NET_CB_FACE_FIND_STATE[nStateNum];
                for (Int32 i = 0; i < nStateNum; ++i)
                {
                    sTmp[i] = (NET_CB_FACE_FIND_STATE)Marshal.PtrToStructure(pstStates + i * Marshal.SizeOf(typeof(NET_CB_FACE_FIND_STATE)), typeof(NET_CB_FACE_FIND_STATE));
                    if (m_WaitToken == sTmp[i].nToken)
                    {
                        if (100 == sTmp[i].nProgress)
                        {
                            m_IsFindFinishFlag = true;
                            m_TotalCount = sTmp[i].nCurrentCount;
                            break;
                        }
                    }
                }
            }
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            AddPersonWindow addPersonWindow = new AddPersonWindow(m_MainWindow,m_LoginID, m_GroupInfo);
            var ret = addPersonWindow.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Successfully Add Person information(成功增加人员信息)!");
                button_Search_Click(null, null);
            }
        }

        private void button_Del_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            if (listView_person.SelectedItems.Count > 0)
            {
                bool ret = false;
                ListViewItem item = listView_person.SelectedItems[0];
                NET_IN_OPERATE_FACERECONGNITIONDB stuInParam = new NET_IN_OPERATE_FACERECONGNITIONDB();
                try
                {
                    stuInParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_OPERATE_FACERECONGNITIONDB));
                    stuInParam.emOperateType = EM_OPERATE_FACERECONGNITIONDB_TYPE.DELETE;
                    stuInParam.stPersonInfo.pszGroupID = Marshal.StringToHGlobalAnsi(m_GroupInfo.szGroupId);
                    stuInParam.stPersonInfo.bGroupIdLen = (byte)m_GroupInfo.szGroupId.Length;
                    stuInParam.stPersonInfo.szUID = item.Text.Trim();

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
                }
                if (ret)
                {
                    MessageBox.Show("Successfully delete person information(成功删除人员信息)!");
                    button_Search_Click(null, null);
                }
            }
        }

        private void button_Modify_Click(object sender, EventArgs e)
        {
            if (listView_person.SelectedItems.Count > 0)
            {
                ModifyPersonWindow modifyPersonWindow = new ModifyPersonWindow(m_MainWindow,m_LoginID, m_GroupInfo, m_CurrentPersonInfoList[listView_person.SelectedItems[0].Index]);
                var ret = modifyPersonWindow.ShowDialog();
                if (ret == System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("Successfully modify Person information(成功修改人员信息)!");
                    button_Search_Click(null, null);
                }
            }
        }

        private void button_PrePage_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            if (IntPtr.Zero != m_FindID)
            {
                m_CurrentCount -= m_ShowLine;
                int count = FindFaceAndShow(m_FindID, m_CurrentCount);
                if (m_CurrentCount - m_ShowLine < 0)
                {
                    this.button_PrePage.Enabled = false;
                }
                if (m_CurrentCount + m_ShowLine <= m_TotalCount)
                {
                    this.button_NextPage.Enabled = true;
                }
            }
        }

        private void button_NextPage_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            if(IntPtr.Zero != m_FindID)
            {
                m_CurrentCount += m_ShowLine;
                int count = FindFaceAndShow(m_FindID, m_CurrentCount);
                if (m_CurrentCount + count >= m_TotalCount)
                {
                    this.button_NextPage.Enabled = false;
                }
                if (m_CurrentCount == m_ShowLine)
                {
                    this.button_PrePage.Enabled = true;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            foreach (NET_CANDIDATE_INFO info in m_CurrentPersonInfoList)
            {
                if (info.stPersonInfo.szFacePicInfo[0].pszFilePath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(info.stPersonInfo.szFacePicInfo[0].pszFilePath);
                }
            }
            base.OnClosed(e);
            if (IntPtr.Zero != m_FindID)
            {
                NETClient.StopFindFaceRecognition(m_FindID);
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
