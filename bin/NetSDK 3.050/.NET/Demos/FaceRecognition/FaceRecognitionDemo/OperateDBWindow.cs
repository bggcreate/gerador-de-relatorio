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
    public partial class OperateDBWindow : Form
    {
        private FaceRecognitionDemo m_MainWindow;
        private IntPtr m_LoginID = IntPtr.Zero;
        private int m_ChannelNumber = 0;
        private const int m_WaitTime = 3000;
        private ListViewItem item;
        private NET_FACERECONGNITION_GROUP_INFO[] m_Groups;
        private int m_SelectIndex = -1;

        public OperateDBWindow(FaceRecognitionDemo _Form,int channelNumber, IntPtr _LoginID)
        {
            InitializeComponent();
            m_MainWindow = _Form;
            m_LoginID = _LoginID;
            m_ChannelNumber = channelNumber;
            m_MainWindow.DeviceDisconnected += new Action(m_MainWindow_DeviceDisconnected);
        }

        void m_MainWindow_DeviceDisconnected()
        {
            m_LoginID = IntPtr.Zero;
        }

        private void RefreshGroup()
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            this.listView_groups.Items.Clear();
            m_Groups = FindGroups();
            if (null != m_Groups)
            {
                foreach (var info in m_Groups)
                {
                    item = new ListViewItem();
                    item.Text = info.szGroupId;
                    item.SubItems.Add(info.szGroupName);
                    item.SubItems.Add(info.nGroupSize.ToString());

                    listView_groups.BeginUpdate();
                    listView_groups.Items.Add(item);
                    listView_groups.EndUpdate();
                }
            }
            m_SelectIndex = -1;
        }

        private NET_FACERECONGNITION_GROUP_INFO[] FindGroups()
        {
            int nMax = 20;
            bool bRet = false;
            NET_IN_FIND_GROUP_INFO stuIn = new NET_IN_FIND_GROUP_INFO();
            NET_OUT_FIND_GROUP_INFO stuOut = new NET_OUT_FIND_GROUP_INFO();
            stuIn.dwSize = (uint)Marshal.SizeOf(stuIn);
            stuOut.dwSize = (uint)Marshal.SizeOf(stuOut);
            stuOut.nMaxGroupNum = nMax;
            try
            {
                stuOut.pGroupInfos = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_FACERECONGNITION_GROUP_INFO)) * nMax);
                NET_FACERECONGNITION_GROUP_INFO stuGroup = new NET_FACERECONGNITION_GROUP_INFO();
                stuGroup.dwSize = (uint)Marshal.SizeOf(stuGroup);
                for (int i = 0; i < nMax; i++)
                {
                    IntPtr pAdd = IntPtr.Add(stuOut.pGroupInfos, (int)stuGroup.dwSize * i);
                    Marshal.StructureToPtr(stuGroup, pAdd, true);
                }

                bRet = NETClient.FindGroupInfo(m_LoginID, ref stuIn, ref stuOut, m_WaitTime);
                if (bRet)
                {
                    NET_FACERECONGNITION_GROUP_INFO[] stuGroups = new NET_FACERECONGNITION_GROUP_INFO[stuOut.nRetGroupNum];
                    for (int i = 0; i < stuOut.nRetGroupNum; i++)
                    {
                        IntPtr pAdd = IntPtr.Add(stuOut.pGroupInfos, (int)Marshal.SizeOf(typeof(NET_FACERECONGNITION_GROUP_INFO)) * i);
                        stuGroups[i] = (NET_FACERECONGNITION_GROUP_INFO)Marshal.PtrToStructure(pAdd, typeof(NET_FACERECONGNITION_GROUP_INFO));
                    }
                    return stuGroups;
                }
                else
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
                Marshal.FreeHGlobal(stuOut.pGroupInfos);
            }
            return null;
        }

        private void listView_groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView_groups.SelectedItems.Count > 0)
            {
                m_SelectIndex = this.listView_groups.SelectedItems[0].Index;
            }
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            AddDBWindow addDB = new AddDBWindow(m_MainWindow, m_LoginID);
            var ret = addDB.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Successfully add DB (成功增加数据库)");
                RefreshGroup();
            }
        }

        private void button_Edit_Click(object sender, EventArgs e)
        {
            if (-1 == m_SelectIndex)
            {
                MessageBox.Show("Please select a DB(请选择一个数据库)");
                return;
            }
            ModifyDBWindow modifyDB = new ModifyDBWindow(m_MainWindow, m_LoginID, m_Groups[m_SelectIndex]);
            var ret = modifyDB.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Successfully modify DB's name(成功修改数据库名称)");
                RefreshGroup();
            }
        }

        private void button_Del_Click(object sender, EventArgs e)
        {
            if (-1 == m_SelectIndex)
            {
                MessageBox.Show("Please select a DB(请选择一个数据库)");
                return;
            }
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
                inParam.emOperateType = EM_OPERATE_FACERECONGNITION_GROUP_TYPE.DELETE;
                NET_DELETE_FACERECONGNITION_GROUP_INFO delParam = new NET_DELETE_FACERECONGNITION_GROUP_INFO();
                delParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_DELETE_FACERECONGNITION_GROUP_INFO));
                delParam.szGroupId = m_Groups[m_SelectIndex].szGroupId;
                stuPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_DELETE_FACERECONGNITION_GROUP_INFO)));
                Marshal.StructureToPtr(delParam, stuPtr, true);
                inParam.pOPerateInfo = stuPtr;

                NET_OUT_OPERATE_FACERECONGNITION_GROUP outParam = new NET_OUT_OPERATE_FACERECONGNITION_GROUP();
                outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_OPERATE_FACERECONGNITION_GROUP));
                ret = NETClient.OperateFaceRecognitionGroup(m_LoginID, ref inParam, ref outParam, m_WaitTime);
                if (!ret)
                {
                    MessageBox.Show(NETClient.GetLastError());
                }
                else
                {
                    RefreshGroup();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(stuPtr);
            }
        }

        private void button_OperatePerson_Click(object sender, EventArgs e)
        {
            if (-1 == m_SelectIndex)
            {
                MessageBox.Show("Please select a DB(请选择一个数据库)");
                return;
            }

            OperatePersonWindow operatePersonWindow = new OperatePersonWindow(m_MainWindow,m_LoginID, m_Groups[m_SelectIndex]);
            operatePersonWindow.ShowDialog();
            RefreshGroup();
        }

        private void button_Dispatch_Click(object sender, EventArgs e)
        {
            if (-1 == m_SelectIndex)
            {
                MessageBox.Show("Please select a DB(请选择一个数据库)");
                return;
            }
            SimilarityOperateWindow similarityOperateWindow = new SimilarityOperateWindow(m_MainWindow, m_LoginID, m_ChannelNumber, m_Groups[m_SelectIndex]);
            similarityOperateWindow.ShowDialog();
            RefreshGroup();
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            RefreshGroup();
        }
        
    }
}
