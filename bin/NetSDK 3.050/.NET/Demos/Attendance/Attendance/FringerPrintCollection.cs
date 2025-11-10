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
    public partial class FringerPrintCollection : Form
    {
        IntPtr _LoginID;
        fMessCallBackEx _MessCallBackEx;
        public int PacketLen { get; set; }
        public byte[] FingerPrintInfo { get; set; }
        bool _IsCollection = false;
        Timer _Timer;
        bool _IsCollectionFailed = false;
        bool _IsListen = false;

        public FringerPrintCollection(IntPtr loginID)
        {
            InitializeComponent();
            _LoginID = loginID;
            this.label_result.Text = "";
            _Timer = new Timer();
            _Timer.Interval = 30000;
            _Timer.Tick += new EventHandler(Timer_Tick);
            _MessCallBackEx = new fMessCallBackEx(MessCallBackEx);
            NETClient.SetDVRMessCallBack(_MessCallBackEx, IntPtr.Zero);
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            _Timer.Stop();
            this.label_result.Text = "Collection failed(采集失败)";
            this.button_startcollection.Enabled = true;
            _IsCollection = false;
            _IsCollectionFailed = true;
            if (_IsListen)
            {
                NETClient.StopListen(_LoginID);
                _IsListen = false;
            }
        }

        bool MessCallBackEx(int lCommand, IntPtr lLoginID, IntPtr pBuf, uint dwBufLen, IntPtr pchDVRIP, int nDVRPort, bool bAlarmAckFlag, int nEventID, IntPtr dwUser)
        {
            if ((EM_ALARM_TYPE)lCommand == EM_ALARM_TYPE.ALARM_FINGER_PRINT)
            {
                NET_ALARM_CAPTURE_FINGER_PRINT_INFO info = (NET_ALARM_CAPTURE_FINGER_PRINT_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_ALARM_CAPTURE_FINGER_PRINT_INFO));
                byte[] data = new byte[info.nPacketLen * info.nPacketNum];
                Marshal.Copy(info.szFingerPrintInfo, data, 0, data.Length);
                PacketLen = info.nPacketLen * info.nPacketNum;
                FingerPrintInfo = data;
                this.BeginInvoke(new Action(() => 
                {
                    _Timer.Stop();
                    this.label_result.Text = "Collection Completed(采集完成)";
                    this.button_startcollection.Enabled = true;
                    _IsCollection = false;
                    if (_IsListen)
                    {
                        NETClient.StopListen(_LoginID);
                        _IsListen = false;
                    }
                }));
            }
            return true;
        }

        private void button_startcollection_Click(object sender, EventArgs e)
        {
            _IsListen = NETClient.StartListen(_LoginID);
            if (_IsListen == false)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            _IsCollection = true;
            PacketLen = 0;
            FingerPrintInfo = null;
            NET_CTRL_CAPTURE_FINGER_PRINT capture = new NET_CTRL_CAPTURE_FINGER_PRINT();
            capture.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_CAPTURE_FINGER_PRINT));
            capture.nChannelID = 0;
            capture.szReaderID = "1";
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_CAPTURE_FINGER_PRINT)));
                Marshal.StructureToPtr(capture, inPtr, true);
                bool ret = NETClient.ControlDevice(_LoginID, EM_CtrlType.CAPTURE_FINGER_PRINT, inPtr, 100000);
                if (!ret)
                {
                    MessageBox.Show("Start collection failed(开始采集失败)");
                    return;
                }
                _Timer.Start();
                this.label_result.Text = "Start Collection(开始采集)";
                this.button_startcollection.Enabled = false;
                _IsCollectionFailed = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (FingerPrintInfo == null || PacketLen == 0)
            {
                if (_IsCollectionFailed)
                {
                    MessageBox.Show("No fingerprint data,because collection failed(没有指纹数据,因为采集失败)");
                    return;
                }
                if (_IsCollection == false)
                {
                    MessageBox.Show("Did not start collecting(没有开始采集)");
                    return;
                }
                MessageBox.Show("In the collection(采集中)");
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            NETClient.SetDVRMessCallBack(null, IntPtr.Zero);
            if (_IsListen)
            {
                NETClient.StopListen(_LoginID);
                _IsListen = false;
            }
            _Timer.Stop();
            _Timer.Dispose();
            base.OnClosed(e);
        }
    }
}
