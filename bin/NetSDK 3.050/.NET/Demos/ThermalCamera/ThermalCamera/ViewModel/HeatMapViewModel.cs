using IVSDemo.Base;
using NetSDKCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ThermalCamera.ViewModel
{
    class HeatMapViewModel:BindableObject
    {
        private IntPtr AttachHandle = IntPtr.Zero;

        #region
        private string status;

        public string Status
        {
            get { return status; }
            set 
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        private int height;

        public int Height
        {
            get { return height; }
            set 
            {
                if(height != value)
                {
                    height = value;
                    OnPropertyChanged("Height");
                }
            }
        }

        private int width;

        public int Width
        {
            get { return width; }
            set 
            {
                if (width != value)
                {
                    width = value;
                    OnPropertyChanged("Width");
                }
            }
        }

        private int channel;

        public int Channel
        {
            get { return channel; }
            set 
            {
                if (channel != value)
                {
                    channel = value;
                    OnPropertyChanged("Channel");
                }
            }
        }

        private int attachChannel;

        public int AttachChannel
        {
            get { return attachChannel; }
            set 
            {
                attachChannel = value;
                OnPropertyChanged("AttachChannel");
            }
        }


        private string time;

        public string Time
        {
            get { return time; }
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        private int length;

        public int Length
        {
            get { return length; }
            set
            {
                if (length != value)
                {
                    length = value;
                    OnPropertyChanged("Length");
                }
            }
        }

        private string sensorType;

        public string SensorType
        {
            get { return sensorType; }
            set
            {
                if (sensorType != value)
                {
                    sensorType = value;
                    OnPropertyChanged("SensorType");
                }
            }
        }

        private bool attachEn = true;

        public bool AttachEn
        {
            get { return attachEn; }
            set
            {
                if (attachEn != value)
                {
                    attachEn = value;
                    OnPropertyChanged("AttachEn");
                }
            }
        }

        private bool detachEn = false;

        public bool DetachEn
        {
            get { return detachEn; }
            set 
            {
                if (detachEn != value)
                {
                    detachEn = value;
                    OnPropertyChanged("DetachEn");
                }
            }
        }

        #endregion

        public ICommand AttachCmd { get; set; }
        public ICommand DetachCmd { get; set; }
        public ICommand FetchCmd { get; set; }
        public ICommand ClosedCmd { get; set; }
        private readonly fRadiometryAttachCB radiometryAttachCB;

        public HeatMapViewModel()
        {
            AttachCmd = new DelegateCommand(Attach);
            DetachCmd = new DelegateCommand(Detach);
            FetchCmd = new DelegateCommand(Fetch);
            ClosedCmd = new DelegateCommand(ClosedWnd);
            radiometryAttachCB = new fRadiometryAttachCB(RadiometryAttachCB);
        }

        private void ClosedWnd()
        {
            Detach();
        }

        private void RadiometryAttachCB(IntPtr lAttachHandle, IntPtr pBuf, int nBufLen, IntPtr dwUser)
        {
            if(pBuf == IntPtr.Zero)
            {
                return;
            }
            NET_RADIOMETRY_DATA data = (NET_RADIOMETRY_DATA)Marshal.PtrToStructure(pBuf, typeof(NET_RADIOMETRY_DATA));
            Dispatcher.Invoke(new Action(() =>
                {
                    Height = data.stMetaData.nHeight;
                    Width = data.stMetaData.nWidth;
                    Channel = data.stMetaData.nChannel;
                    Length = data.stMetaData.nLength;
                    Time = data.stMetaData.stTime.ToString();
                    SensorType = data.stMetaData.szSensorType;
                }));
        }

        private void Fetch()
        {
            bool result = false;
            NET_IN_RADIOMETRY_FETCH inBuf = new NET_IN_RADIOMETRY_FETCH()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_FETCH)),
                nChannel = AttachChannel
            };
            AttachChannel = inBuf.nChannel;

            NET_OUT_RADIOMETRY_FETCH outBuf = new NET_OUT_RADIOMETRY_FETCH()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_FETCH))
            };

            result = NETClient.RadiometryFetch(ThermalViewModel.lLoginID, inBuf, ref outBuf, 3000);
            if (result)
            {
                if (1 == outBuf.nStatus)
                {
                    Status = "Free(空闲)";
                }
                else if (2 == outBuf.nStatus)
                {
                    Status = "Acquiring(获取热图中)";
                }
                else
                {
                    Status = "Unknown(未知)";
                }
            }
            else
            {
                MessageBox.Show("Failed to get heap map data(获取热图数据失败)!");
            }
        }

        private void Detach()
        {
            bool result = false;
            if (AttachHandle == IntPtr.Zero)
            {
                return;
            }
            result = NETClient.RadiometryDetach(AttachHandle);
            if (result)
            {
                AttachEn = true;
                DetachEn = false;
                AttachHandle = IntPtr.Zero;

                Height = 0;
                Width = 0;
                Channel = 0;
                Length = 0;
                Time = "";
                SensorType = "";
            }
            else
            {
                MessageBox.Show("Unsubscribe failed(取消订阅失败)!");
            }
        }

        private void Attach()
        {
            if (AttachHandle != IntPtr.Zero)
            {
                return;
            }
            NET_IN_RADIOMETRY_ATTACH inBuf = new NET_IN_RADIOMETRY_ATTACH()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_ATTACH)),
                nChannel = AttachChannel,
                cbNotify = radiometryAttachCB
            };
            AttachChannel = inBuf.nChannel;

            NET_OUT_RADIOMETRY_ATTACH outBuf = new NET_OUT_RADIOMETRY_ATTACH()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_ATTACH))
            };

            AttachHandle = NETClient.RadiometryAttach(ThermalViewModel.lLoginID, inBuf, ref outBuf, 3000);
            if (AttachHandle != IntPtr.Zero)
            {
                AttachEn = false;
                DetachEn = true;
            }
            else
            {
                MessageBox.Show("Subscription failed(订阅失败)!");
            }
        }
    }
}
