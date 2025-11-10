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

namespace VTODemo
{
    public partial class VTO : Form
    {
        IntPtr loginID = IntPtr.Zero;
        NET_DEVICEINFO_Ex deviceInfo = new NET_DEVICEINFO_Ex();
        IntPtr playID = IntPtr.Zero;
        IntPtr talkID = IntPtr.Zero;
        fAudioDataCallBack audioDataCallBack;
        bool isListen = false;
        fMessCallBackEx alarmCallBack;
        const int Line = 50;


        public event Action<IntPtr, NET_ALARM_CAPTURE_FINGER_PRINT_INFO, byte[]> MessCallBackEx;
        private void OnMessageCallBackEx(IntPtr id, NET_ALARM_CAPTURE_FINGER_PRINT_INFO message, byte[] data)
        {
            if (MessCallBackEx != null)
            {
                MessCallBackEx(id, message, data);
            }
        }

        public VTO()
        {
            InitializeComponent();
            try
            {
                NETClient.Init(null, IntPtr.Zero, null);
                audioDataCallBack = new fAudioDataCallBack(AudioDataCallBack);
                alarmCallBack = new fMessCallBackEx(MessCallBack);
                NETClient.SetDVRMessCallBack(alarmCallBack, IntPtr.Zero);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            this.Load += new EventHandler(VTO_Load);
        }

        void VTO_Load(object sender, EventArgs e)
        {
            this.button_realplay.Enabled = false;
            this.button_stopplay.Enabled = false;
            this.button_talk.Enabled = false;
            this.button_stoptalk.Enabled = false;
            this.button_operatecard.Enabled = false;
            this.button_open.Enabled = false;
            this.button_close.Enabled = false;
            this.button_startlisten.Enabled = false;
            this.button_stoplisten.Enabled = false;
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == loginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(this.textBox_port.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Port error(端口错误)");
                    return;
                }
                loginID = NETClient.Login(this.textBox_ip.Text.Trim(), port, this.textBox_name.Text.Trim(), this.textBox_password.Text, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref deviceInfo);
                if (loginID == IntPtr.Zero)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_login.Text = "Logout(登出)";
                this.button_realplay.Enabled = true;
                this.button_talk.Enabled = true;
                this.button_operatecard.Enabled = true;
                this.button_open.Enabled = true;
                this.button_close.Enabled = true;
                this.button_startlisten.Enabled = true;
            }
            else
            {
                if (isListen)
                {
                    NETClient.StopListen(loginID);
                    isListen = false;
                }
                if (talkID != IntPtr.Zero)
                {
                    NETClient.RecordStop(loginID);
                    NETClient.StopTalk(talkID);
                    talkID = IntPtr.Zero;
                }
                NETClient.Logout(loginID);
                loginID = IntPtr.Zero;
                playID = IntPtr.Zero;
                this.button_login.Text = "Login(登录)";
                this.button_realplay.Enabled = false;
                this.button_stopplay.Enabled = false;
                this.button_talk.Enabled = false;
                this.button_stoptalk.Enabled = false;
                this.button_operatecard.Enabled = false;
                this.button_open.Enabled = false;
                this.button_close.Enabled = false;
                this.button_startlisten.Enabled = false;
                this.button_stoplisten.Enabled = false;
                this.pictureBox_play.Refresh();
                this.listView_event.Items.Clear();
            }
        }

        private void button_realplay_Click(object sender, EventArgs e)
        {
            playID = NETClient.RealPlay(loginID, 0, this.pictureBox_play.Handle);
            if (playID == IntPtr.Zero)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            this.button_realplay.Enabled = false;
            this.button_stopplay.Enabled = true;
        }

        private void button_stopplay_Click(object sender, EventArgs e)
        {
            NETClient.StopRealPlay(playID);
            playID = IntPtr.Zero;
            this.button_realplay.Enabled = true;
            this.button_stopplay.Enabled = false;
            this.pictureBox_play.Refresh();
        }

        private void button_talk_Click(object sender, EventArgs e)
        {
            IntPtr talkEncodePointer = IntPtr.Zero;
            IntPtr talkSpeakPointer = IntPtr.Zero;

            NET_DEV_TALKDECODE_INFO talkCodeInfo = new NET_DEV_TALKDECODE_INFO();
            talkCodeInfo.encodeType = EM_TALK_CODING_TYPE.PCM;
            talkCodeInfo.dwSampleRate = 8000;
            talkCodeInfo.nAudioBit = 16;
            talkCodeInfo.nPacketPeriod = 25;
            talkCodeInfo.reserved = new byte[60];

            talkEncodePointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_DEV_TALKDECODE_INFO)));
            Marshal.StructureToPtr(talkCodeInfo, talkEncodePointer, true);
            // set talk encode type 设置对讲编码类型
            NETClient.SetDeviceMode(loginID, EM_USEDEV_MODE.TALK_ENCODE_TYPE, talkEncodePointer);

            NET_SPEAK_PARAM speak = new NET_SPEAK_PARAM();
            speak.dwSize = (uint)Marshal.SizeOf(typeof(NET_SPEAK_PARAM));
            speak.nMode = 0;
            speak.bEnableWait = false;
            speak.nSpeakerChannel = 0;
            talkSpeakPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_SPEAK_PARAM)));
            Marshal.StructureToPtr(speak, talkSpeakPointer, true);
            //set talk speak mode 设置对讲模式
            NETClient.SetDeviceMode(loginID, EM_USEDEV_MODE.TALK_SPEAK_PARAM, talkSpeakPointer);

            talkID = NETClient.StartTalk(loginID, audioDataCallBack, IntPtr.Zero);
            Marshal.FreeHGlobal(talkEncodePointer);
            Marshal.FreeHGlobal(talkSpeakPointer);
            if (IntPtr.Zero == talkID)
            {
                MessageBox.Show(this, NETClient.GetLastError());
                return;
            }
            bool ret = NETClient.RecordStart(loginID);
            if (!ret)
            {
                NETClient.StopTalk(talkID);
                talkID = IntPtr.Zero;
                MessageBox.Show(this, NETClient.GetLastError());
                return;
            }
            this.button_talk.Enabled = false;
            this.button_stoptalk.Enabled = true;
        }

        private void AudioDataCallBack(IntPtr lTalkHandle, IntPtr pDataBuf, uint dwBufSize, byte byAudioFlag, IntPtr dwUser)
        {
            if (lTalkHandle != talkID)
            {
                return;
            }
            if (0 == byAudioFlag)
            {
                //send talk data 发送语音数据
                NETClient.TalkSendData(lTalkHandle, pDataBuf, dwBufSize);
            }
            if (1 == byAudioFlag || 2 == byAudioFlag)
            {
                try
                {
                    NETClient.AudioDecEx(lTalkHandle, pDataBuf, dwBufSize);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void button_stoptalk_Click(object sender, EventArgs e)
        {
            NETClient.RecordStop(loginID);
            NETClient.StopTalk(talkID);
            talkID = IntPtr.Zero;
            this.button_talk.Enabled = true;
            this.button_stoptalk.Enabled = false;
        }

        private void button_operatecard_Click(object sender, EventArgs e)
        {
            OperateManager cm = new OperateManager(loginID, this);
            cm.ShowDialog();
            cm.Dispose();
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            NET_CTRL_ACCESS_OPEN openInfo = new NET_CTRL_ACCESS_OPEN();
            openInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_ACCESS_OPEN));
            openInfo.nChannelID = 0;
            openInfo.szTargetID = IntPtr.Zero;
            openInfo.emOpenDoorType = EM_OPEN_DOOR_TYPE.REMOTE;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_ACCESS_OPEN)));
                Marshal.StructureToPtr(openInfo, inPtr, true);
                bool ret = NETClient.ControlDevice(loginID, EM_CtrlType.ACCESS_OPEN, inPtr, 10000);
                if (!ret)
                {
                    MessageBox.Show("Open door failed(开门失败)");
                    return;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
            }
            MessageBox.Show("Open door successfully(开门成功)");
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            NET_CTRL_ACCESS_CLOSE closeInfo = new NET_CTRL_ACCESS_CLOSE();
            closeInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_ACCESS_CLOSE));
            closeInfo.nChannelID = 0;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_ACCESS_CLOSE)));
                Marshal.StructureToPtr(closeInfo, inPtr, true);
                bool ret = NETClient.ControlDevice(loginID, EM_CtrlType.ACCESS_CLOSE, inPtr, 10000);
                if (!ret)
                {
                    MessageBox.Show("Close door failed(关门失败)");
                    return;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
            }
            MessageBox.Show("Close door successfully(关门成功)");
        }

        private void button_startlisten_Click(object sender, EventArgs e)
        {
            if (isListen == false)
            {
                NETClient.StartListen(loginID);
                isListen = true;
                this.button_startlisten.Enabled = false;
                this.button_stoplisten.Enabled = true;
            }
        }

        private void button_stoplisten_Click(object sender, EventArgs e)
        {
            if (isListen)
            {
                NETClient.StopListen(loginID);
                isListen = false;
                this.button_startlisten.Enabled = true;
                this.button_stoplisten.Enabled = false;
                this.listView_event.Items.Clear();
            }
        }

        private bool MessCallBack(int lCommand, IntPtr lLoginID, IntPtr pBuf, uint dwBufLen, IntPtr pchDVRIP, int nDVRPort, bool bAlarmAckFlag, int nEventID, IntPtr dwUser)
        {
            if ((EM_ALARM_TYPE)lCommand == EM_ALARM_TYPE.ALARM_ACCESS_CTL_EVENT)
            {
                if (lLoginID != loginID)
                {
                    return true;
                }
                NET_ALARM_ACCESS_CTL_EVENT_INFO info = (NET_ALARM_ACCESS_CTL_EVENT_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_ALARM_ACCESS_CTL_EVENT_INFO));
                this.BeginInvoke(new Action(() =>
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = Encoding.Default.GetString(info.szUserID);
                    item.SubItems.Add(info.szCardNo);
                    item.SubItems.Add(info.stuTime.ToString());
                    switch (info.emOpenMethod)
                    {
                        case EM_ACCESS_DOOROPEN_METHOD.CARD:
                            item.SubItems.Add("Card(卡)");
                            break;
                        case EM_ACCESS_DOOROPEN_METHOD.FACE_RECOGNITION:
                            item.SubItems.Add("Face recognition(人脸识别)");
                            break;
                        case EM_ACCESS_DOOROPEN_METHOD.FINGERPRINT:
                            item.SubItems.Add("Fingerprint(指纹)");
                            break;
                        case EM_ACCESS_DOOROPEN_METHOD.REMOTE:
                            item.SubItems.Add("Remote(远程)");
                            break;
                        default:
                            item.SubItems.Add("Unknown(未知)");
                            break;
                    }
                    if (info.bStatus)
                    {
                        item.SubItems.Add("True(成功)");
                    }
                    else
                    {
                        item.SubItems.Add("False(失败)");
                    }

                    listView_event.BeginUpdate();
                    listView_event.Items.Insert(0, item);
                    if (listView_event.Items.Count > Line)
                    {
                        listView_event.Items.RemoveAt(Line);
                    }
                    listView_event.EndUpdate();

                }));
            }
            if ((EM_ALARM_TYPE)lCommand == EM_ALARM_TYPE.ALARM_FINGER_PRINT)
            {
                NET_ALARM_CAPTURE_FINGER_PRINT_INFO info = (NET_ALARM_CAPTURE_FINGER_PRINT_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_ALARM_CAPTURE_FINGER_PRINT_INFO));
                byte[] data = new byte[info.nPacketLen * info.nPacketNum];
                Marshal.Copy(info.szFingerPrintInfo, data, 0, data.Length);
                OnMessageCallBackEx(lLoginID, info, data);
            }
            return true;
        }

        private void textBox_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (talkID != IntPtr.Zero)
            {
                NETClient.RecordStop(loginID);
                NETClient.StopTalk(talkID);
            }
            NETClient.Cleanup();
        }

    }
}
