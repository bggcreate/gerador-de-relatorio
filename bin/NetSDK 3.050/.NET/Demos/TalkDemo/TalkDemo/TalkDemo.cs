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

namespace TalkDemo
{
    public partial class TalkDemo : Form
    {
        private static fDisConnectCallBack m_DisConnectCallBack;
        private static fAudioDataCallBack m_AudioDataCallBack;
        private const int SampleRate = 8000;
        private const int AudioBit = 16;
        private const int PacketPeriod = 25;
        private const int SendAudio = 0;
        private const int ReviceAudio = 1;
        
        private IntPtr m_LoginID = IntPtr.Zero;
        private NET_DEVICEINFO_Ex m_DeviceInfo;
        private IntPtr m_TalkID = IntPtr.Zero;

        public TalkDemo()
        {
            InitializeComponent();
            this.Load += new EventHandler(TalkDemo_Load);
        }

        private void TalkDemo_Load(object sender, EventArgs e)
        {
            m_DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
            m_AudioDataCallBack = new fAudioDataCallBack(AudioDataCallBack);
            try
            {
                NETClient.Init(m_DisConnectCallBack, IntPtr.Zero, null);
                InitOrLogoutUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke((Action)UpdateDisConnectUI);
        }

        private void UpdateDisConnectUI()
        {
            if (IntPtr.Zero != m_TalkID && IntPtr.Zero != m_LoginID)
            {
                NETClient.RecordStop(m_LoginID);
                NETClient.StopTalk(m_TalkID);
                NETClient.Logout(m_LoginID);
            }
            InitOrLogoutUI();
            MessageBox.Show("device offline(设备离线)");
        }

        private void login_button_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(port_textBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Input port error(输入端口错误)!");
                    return;
                }
                m_DeviceInfo = new NET_DEVICEINFO_Ex();
                m_LoginID = NETClient.Login(ip_textBox.Text.Trim(), port, name_textBox.Text.Trim(), password_textBox.Text.Trim(), EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DeviceInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                LoginUI();
            }
            else
            {
                if (IntPtr.Zero != m_TalkID)
                {
                    NETClient.RecordStop(m_LoginID);
                    NETClient.StopTalk(m_TalkID);
                }
                bool result = NETClient.Logout(m_LoginID);
                if (!result)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_LoginID = IntPtr.Zero;
                InitOrLogoutUI();
            }
        }

        private void talk_button_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == m_TalkID)
            {
                IntPtr talkEncodePointer = IntPtr.Zero;
                IntPtr talkSpeakPointer = IntPtr.Zero;
                IntPtr talkTransferPointer = IntPtr.Zero;
                IntPtr channelPointer = IntPtr.Zero;

                NET_DEV_TALKDECODE_INFO talkCodeInfo = new NET_DEV_TALKDECODE_INFO();
                talkCodeInfo.encodeType = EM_TALK_CODING_TYPE.PCM;
                talkCodeInfo.dwSampleRate = SampleRate;
                talkCodeInfo.nAudioBit = AudioBit;
                talkCodeInfo.nPacketPeriod = PacketPeriod;
                talkCodeInfo.reserved = new byte[60];

                talkEncodePointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_DEV_TALKDECODE_INFO)));
                Marshal.StructureToPtr(talkCodeInfo, talkEncodePointer, true);
                // set talk encode type 设置对讲编码类型
                NETClient.SetDeviceMode(m_LoginID, EM_USEDEV_MODE.TALK_ENCODE_TYPE, talkEncodePointer);

                NET_SPEAK_PARAM speak = new NET_SPEAK_PARAM();
                speak.dwSize = (uint)Marshal.SizeOf(typeof(NET_SPEAK_PARAM));
                speak.nMode = 0;
                speak.bEnableWait = false;
                speak.nSpeakerChannel = 0;
                talkSpeakPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_SPEAK_PARAM)));
                Marshal.StructureToPtr(speak, talkSpeakPointer, true);
                //set talk speak mode 设置对讲模式
                NETClient.SetDeviceMode(m_LoginID, EM_USEDEV_MODE.TALK_SPEAK_PARAM, talkSpeakPointer);

                NET_TALK_TRANSFER_PARAM transfer = new NET_TALK_TRANSFER_PARAM();
                transfer.dwSize = (uint)Marshal.SizeOf(typeof(NET_TALK_TRANSFER_PARAM));
                if (local_radioButton.Checked)
                {
                    transfer.bTransfer = false;
                }
                else
                {
                    transfer.bTransfer = true;
                    channelPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
                    Marshal.WriteInt32(channelPointer, channel_comboBox.SelectedIndex);
                    NETClient.SetDeviceMode(m_LoginID, EM_USEDEV_MODE.TALK_TALK_CHANNEL, channelPointer);
                }
                talkTransferPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_TALK_TRANSFER_PARAM)));
                Marshal.StructureToPtr(transfer, talkTransferPointer, true);
                //set talk transfer mode 设置对讲是否转发模式
                NETClient.SetDeviceMode(m_LoginID, EM_USEDEV_MODE.TALK_TRANSFER_MODE, talkTransferPointer);

                m_TalkID = NETClient.StartTalk(m_LoginID, m_AudioDataCallBack, IntPtr.Zero);
                Marshal.FreeHGlobal(talkEncodePointer);
                Marshal.FreeHGlobal(talkSpeakPointer);
                Marshal.FreeHGlobal(talkTransferPointer);
                Marshal.FreeHGlobal(channelPointer);
                if(IntPtr.Zero == m_TalkID)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                bool ret = NETClient.RecordStart(m_LoginID);
                if(!ret)
                {
                    NETClient.StopTalk(m_TalkID);
                    m_TalkID = IntPtr.Zero;
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                talk_button.Text = "StopTalk(停止对讲)";
            }
            else
            {
                NETClient.RecordStop(m_LoginID);
                NETClient.StopTalk(m_TalkID);
                m_TalkID = IntPtr.Zero;
                talk_button.Text = "StartTalk(开始对讲)";
            }
          
        }

        private void AudioDataCallBack(IntPtr lTalkHandle, IntPtr pDataBuf, uint dwBufSize, byte byAudioFlag, IntPtr dwUser)
        {
            if (lTalkHandle == m_TalkID)
            {
                if (SendAudio == byAudioFlag)
                {
                    //send talk data 发送语音数据
                    NETClient.TalkSendData(lTalkHandle, pDataBuf, dwBufSize);
                }
                else if (ReviceAudio == byAudioFlag)
                {
                    //here call netsdk decode audio,or can send data to other user.这里调用netsdk解码语音数据，或也可以把语音数据发送给另外的用户
                    try
                    {
                        NETClient.AudioDec(pDataBuf, dwBufSize);
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void InitOrLogoutUI()
        {
            local_radioButton.Enabled = false;
            remote_radioButton.Enabled = false;
            channel_comboBox.Items.Clear();
            channel_comboBox.Enabled = false;
            talk_button.Enabled = false;
            login_button.Text = "Login(登录)";
            talk_button.Text = "StartTalk(开始对讲)";
            local_radioButton.Checked = false;
            remote_radioButton.Checked = false;
            m_TalkID = IntPtr.Zero;
            m_LoginID = IntPtr.Zero;
            this.Text = "TalkDemo(对讲Demo)";
        }

        private void LoginUI()
        {
            login_button.Text = "Logout(登出)";
            talk_button.Enabled = true;
            local_radioButton.Enabled = true;
            remote_radioButton.Enabled = true;
            local_radioButton.Checked = true;
        }

        private void remote_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (remote_radioButton.Checked)
            {
                channel_comboBox.Items.Clear();
                channel_comboBox.Enabled = true;
                for (int i = 1; i <= m_DeviceInfo.nChanNum; i++)
                {
                    channel_comboBox.Items.Add(i);
                }
                channel_comboBox.SelectedIndex = 0;
            }
            else
            {
                channel_comboBox.Items.Clear();
                channel_comboBox.Enabled = false;
            }
        }

        private void port_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (IntPtr.Zero != m_TalkID && IntPtr.Zero != m_LoginID)
            {
                NETClient.RecordStop(m_LoginID);
                NETClient.StopTalk(m_TalkID);
            }
            NETClient.Cleanup();
        }

    }
}
