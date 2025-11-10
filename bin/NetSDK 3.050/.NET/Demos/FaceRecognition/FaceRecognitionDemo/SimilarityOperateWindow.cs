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
    public partial class SimilarityOperateWindow : Form
    {
        private FaceRecognitionDemo m_MainWindow;
        private IntPtr m_LoginID;
        private int m_ChannelNumber;
        private NET_FACERECONGNITION_GROUP_INFO m_GroupInfo;
        private const int m_WaitTime = 3000;
        private ListViewItem item;

        public SimilarityOperateWindow(FaceRecognitionDemo _Form, IntPtr _LoginID, int channelNumber, NET_FACERECONGNITION_GROUP_INFO _GroupInfo)
        {
            InitializeComponent();
            m_MainWindow = _Form;
            m_LoginID = _LoginID;
            m_ChannelNumber = channelNumber;
            m_GroupInfo = _GroupInfo;
            m_MainWindow.DeviceDisconnected += new Action(m_MainWindow_DeviceDisconnected);
            this.Load += new EventHandler(SimilarityOperateWindow_Load);
        }

        void m_MainWindow_DeviceDisconnected()
        {
            m_LoginID = IntPtr.Zero;
        }

        void SimilarityOperateWindow_Load(object sender, EventArgs e)
        {
            this.textBox_groupid.Text = m_GroupInfo.szGroupId;
            this.textBox_groupname.Text = m_GroupInfo.szGroupName;
            for (int i = 0; i < m_ChannelNumber; i++)
            {
                comboBox_channel.Items.Add(i + 1);
            }
            for (int i = 0; i <= 100; i++)
            {
                comboBox_value.Items.Add(i);
            }
            for (int i = 0; i < m_GroupInfo.nRetChnCount; i++)
            {
                item = new ListViewItem();
                item.Text = (m_GroupInfo.nChannel[i] + 1).ToString();
                item.SubItems.Add(m_GroupInfo.nSimilarity[i].ToString());

                listView_si.BeginUpdate();
                listView_si.Items.Add(item);
                listView_si.EndUpdate();
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            if (comboBox_channel.SelectedItem == null)
            {
                MessageBox.Show("Please select one channel(请选择一个通道)");
                return;
            }
            if (comboBox_value.SelectedItem == null)
            {
                MessageBox.Show("Please select Similarity value(请选择相似度值)");
                return;
            }
            NET_IN_FACE_RECOGNITION_PUT_DISPOSITION_INFO inParam = new NET_IN_FACE_RECOGNITION_PUT_DISPOSITION_INFO();
            inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_FACE_RECOGNITION_PUT_DISPOSITION_INFO));
            inParam.nDispositionChnNum = 1;
            inParam.szGroupId = m_GroupInfo.szGroupId;
            inParam.stuDispositionChnInfo = new NET_DISPOSITION_CHANNEL_INFO[1024];
            inParam.stuDispositionChnInfo[0].nChannelID = comboBox_channel.SelectedIndex;
            inParam.stuDispositionChnInfo[0].nSimilary = comboBox_value.SelectedIndex;
            NET_OUT_FACE_RECOGNITION_PUT_DISPOSITION_INFO outParam = new NET_OUT_FACE_RECOGNITION_PUT_DISPOSITION_INFO();
            outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_FACE_RECOGNITION_PUT_DISPOSITION_INFO));
            bool ret = NETClient.FaceRecognitionPutDisposition(m_LoginID, inParam, ref outParam, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }

            foreach (ListViewItem listitem in listView_si.Items)
            {
                if (listitem.Text == (comboBox_channel.SelectedIndex + 1).ToString())
                {
                    listView_si.BeginUpdate();
                    listitem.SubItems.Clear();
                    listitem.Text = (inParam.stuDispositionChnInfo[0].nChannelID + 1).ToString();
                    listitem.SubItems.Add(inParam.stuDispositionChnInfo[0].nSimilary.ToString());
                    listView_si.EndUpdate();
                    return;
                }
            }

            item = new ListViewItem();
            item.Text = (inParam.stuDispositionChnInfo[0].nChannelID + 1).ToString();
            item.SubItems.Add(inParam.stuDispositionChnInfo[0].nSimilary.ToString());

            listView_si.BeginUpdate();
            listView_si.Items.Add(item);
            listView_si.EndUpdate();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                MessageBox.Show("Device is offline(设备已离线)!");
                return;
            }
            if (listView_si.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a row of the list(请选择一个列表的行)!");
                return;
            }
            NET_IN_FACE_RECOGNITION_DEL_DISPOSITION_INFO inParam = new NET_IN_FACE_RECOGNITION_DEL_DISPOSITION_INFO();
            inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_FACE_RECOGNITION_DEL_DISPOSITION_INFO));
            inParam.nDispositionChnNum = 1;
            inParam.szGroupId = m_GroupInfo.szGroupId;
            inParam.nDispositionChn = new int[1024];
            inParam.nDispositionChn[0] = int.Parse(listView_si.SelectedItems[0].Text) - 1;
            NET_OUT_FACE_RECOGNITION_DEL_DISPOSITION_INFO outParam = new NET_OUT_FACE_RECOGNITION_DEL_DISPOSITION_INFO();
            outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_FACE_RECOGNITION_DEL_DISPOSITION_INFO));
            bool ret = NETClient.FaceRecognitionDelDisposition(m_LoginID, inParam, ref outParam, m_WaitTime);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            listView_si.BeginUpdate();
            listView_si.Items.Remove(listView_si.SelectedItems[0]);
            listView_si.EndUpdate();
        }
    }
}
