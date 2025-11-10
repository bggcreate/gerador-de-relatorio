using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IVSDemo.Base;
using System.Windows.Input;
using NetSDKCS;
using System.Collections.ObjectModel;
using ThermalCamera.window;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows;

namespace ThermalCamera.ViewModel
{
    class ThermalViewModel:BindableObject
    {
        static public IntPtr lLoginID = IntPtr.Zero;
        private IntPtr playHandle = IntPtr.Zero;
        private IntPtr hWnd = IntPtr.Zero;
        private int currentChannel = 0;
        private NET_DEVICEINFO_Ex DeviceInfo;
        public ObservableCollection<int> Channels { get; set; }

        private string wndTitle = "ThermalCamera(热成像Demo)--Offline(离线)";

        public string WndTitle
        {
            get { return wndTitle; }
            set 
            {
                if (wndTitle != value)
                {
                    wndTitle = value;
                    OnPropertyChanged("WndTitle");
                }
                
            }
        }
        
        //  login
        #region
        private string ip = "172.32.103.3";

        public string IP
        {
            get { return ip; }
            set 
            {
                if (ip != value)
                {
                    ip = value;
                    OnPropertyChanged("IP");
                }
            }
        }

        private ushort port = 37777;

        public ushort Port
        {
            get { return port; }
            set 
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged("Port");
                }
            }
        }

        private string user = "admin";

        public string User
        {
            get { return user; }
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged("User");
                }
            }
        }

        private string pwd = "admin12345";

        public string Pwd
        {
            get { return pwd; }
            set 
            {
                if (pwd != value)
                {
                    pwd = value;
                    OnPropertyChanged("Pwd");
                }
            }
        }

        private string loginState = "Login(登陆)";

        public string LoginState
        {
            get { return loginState; }
            set
            {
                if (loginState != value)
                {
                    loginState = value;
                    OnPropertyChanged("LoginState");
                }
            }
        }

        private string playBtnText = "Play(监视)";

        public string PlayBtnText
        {
            get { return playBtnText; }
            set 
            {
                if (playBtnText != value)
                {
                    playBtnText = value;
                    OnPropertyChanged("PlayBtnText");
                }
            }
        }

        private int playChannel;

        public int PlayChannel
        {
            get { return playChannel; }
            set 
            {
                if (playChannel != value)
                {
                    playChannel = value; 
                    OnPropertyChanged("PlayChannel");
                }
            }
        }
        #endregion

        // button enable
        #region
        private bool temperPointEn = false;

        public bool TemperPointEn
        {
            get { return temperPointEn; }
            set 
            {
                if (temperPointEn != value)
                {
                    temperPointEn = value;
                    OnPropertyChanged("TemperPointEn");
                }
            }
        }

        private bool temperItemEn = false;

        public bool TemperItemEn
        {
            get { return temperItemEn; }
            set
            {
                if (temperItemEn != value)
                {
                    temperItemEn = value;
                    OnPropertyChanged("TemperItemEn");
                }
            }
        }

        private bool temperQueryEn = false;

        public bool TemperQueryEn
        {
            get { return temperQueryEn; }
            set
            {
                if (temperQueryEn != value)
                {
                    temperQueryEn = value;
                    OnPropertyChanged("TemperQueryEn");
                }
            }
        }

        private bool subscribeEn = false;

        public bool SubscribeEn
        {
            get { return subscribeEn; }
            set
            {
                if (subscribeEn != value)
                {
                    subscribeEn = value;
                    OnPropertyChanged("SubscribeEn");
                }
            }
        }

        private bool playEn = false;

        public bool PlayEn
        {
            get { return playEn; }
            set
            {
                if (playEn != value)
                {
                    playEn = value;
                    OnPropertyChanged("PlayEn");
                }
            }
        }

        private bool cmbEn;

        public bool CmbEn
        {
            get { return cmbEn; }
            set
            {
                if (cmbEn != value)
                {
                    cmbEn = value;
                    OnPropertyChanged("CmbEn");
                }
            }
        }

        private bool wndEnable;

        public bool WndEnable
        {
            get { return wndEnable; }
            set 
            { 
                wndEnable = value;
                OnPropertyChanged("WndEnable");
            }
        }



        #endregion

        public ICommand LoginCmd { get; set; }
        public ICommand PlayCmd { get; set; }
        public ICommand PlayChannelSelectionChangedCmd { get; set; }
        public ICommand TemperPointCmd { get; set; }
        public ICommand TemperItemCmd { get; set; }
        public ICommand TemperQueryCmd { get; set; }
        public ICommand SubscriptionCmd { get; set; }

        private fDisConnectCallBack disConnectCallback;
        private fHaveReConnectCallBack haveReConnectCallBack;

        public ThermalViewModel()
        {
            DeviceInfo = new NET_DEVICEINFO_Ex();
            Channels = new ObservableCollection<int>();

            LoginCmd = new DelegateCommand(Login);
            PlayCmd = new DelegateCommand<object>(RealPlay);
            PlayChannelSelectionChangedCmd = new DelegateCommand<int>(PlayChannelSelectionChanged);
            TemperPointCmd = new DelegateCommand(TemperPointDlg);
            TemperItemCmd = new DelegateCommand(TemperItemDlg);
            TemperQueryCmd = new DelegateCommand(TemperQueryDlg);
            SubscriptionCmd = new DelegateCommand(SubscriptionDlg);

            disConnectCallback = new fDisConnectCallBack(DisConnectCallBack);
            haveReConnectCallBack = new fHaveReConnectCallBack(HaveReConnectCallBack);
            try
            {
                NETClient.Init(disConnectCallback, IntPtr.Zero, null);
                NETClient.SetAutoReconnect(haveReConnectCallBack, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
        }

        ~ThermalViewModel()
        {
            if (playHandle != IntPtr.Zero)
            {
                NETClient.StopRealPlay(playHandle);
                playHandle = IntPtr.Zero;
            }

            if (lLoginID != IntPtr.Zero)
            {
                NETClient.Logout(lLoginID);
            }
            NETClient.Cleanup();
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateUI(false);
            }));
        }

        private void HaveReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateUI(true);
            }));
        }

        private void UpdateUI(bool isOnline)
        {
            if (isOnline)
            {
                WndTitle = "ThermalCamera(热成像Demo)--Online(在线)";
                LoginState = "Logout(登出)";
                TemperPointEn = true;
                TemperItemEn = true;
                TemperQueryEn = true;
                SubscribeEn = true;
                PlayEn = true;
                CmbEn = true;
            }
            else
            {
                LoginState = "Login(登陆)";
                WndTitle = "ThermalCamera(热成像Demo)--Offline(离线)";
                TemperPointEn = false;
                TemperItemEn = false;
                TemperQueryEn = false;
                SubscribeEn = false;
                PlayEn = false;
                CmbEn = false;
            }
        }

        private void SubscriptionDlg()
        {
            HeatMapWnd heapmapWnd = new HeatMapWnd();
            heapmapWnd.Owner = App.Current.MainWindow;
            heapmapWnd.ShowDialog();
        }

        private void TemperQueryDlg()
        {
            TemperQueryWnd queryWnd = new TemperQueryWnd();
            queryWnd.Owner = App.Current.MainWindow;
            queryWnd.ShowDialog();
        }

        private void TemperItemDlg()
        {
            TemperItemWnd itemWnd = new TemperItemWnd();
            itemWnd.Owner = App.Current.MainWindow;
            itemWnd.ShowDialog();
        }

        private void TemperPointDlg()
        {
            TemperPointWnd pointWnd = new TemperPointWnd();
            pointWnd.Owner = App.Current.MainWindow;
            pointWnd.ShowDialog();
        }

        private void RealPlay(object obj)
        {
            hWnd = (IntPtr)obj;
            if (playHandle == IntPtr.Zero)
            {
                playHandle = NETClient.RealPlay(lLoginID, PlayChannel, hWnd);
                if (playHandle != IntPtr.Zero)
                {
                    currentChannel = PlayChannel;
                    PlayBtnText = "Stop(停止)";
                }
                else
                {
                    MessageBox.Show("Real-Play failed(实时监视失败)!");
                    return;
                }
            }
            else
            {
                NETClient.StopRealPlay(playHandle);
                playHandle = IntPtr.Zero;
                PlayBtnText = "Play(监视)";
                WndEnable = false;
            }
        }

        private void PlayChannelSelectionChanged(int obj)
        {
            int channel = obj;
            if (playHandle != IntPtr.Zero && channel != currentChannel)
            {
                NETClient.StopRealPlay(playHandle);
                playHandle = NETClient.RealPlay(lLoginID, channel, hWnd);
                if (playHandle != IntPtr.Zero)
                {
                    currentChannel = channel;
                    PlayBtnText = "Stop(停止)";
                }
                else
                {
                    MessageBox.Show("Real-Play failed(实时监视失败)!");
                }
            }
        }
    
        public void Login()
        {
            if (lLoginID == IntPtr.Zero)
            {
                lLoginID = NETClient.Login(IP, Port, User, Pwd, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref DeviceInfo);
                if (lLoginID != IntPtr.Zero)
                {
                    Channels.Clear();
                    for (int i = 0; i < DeviceInfo.nChanNum; i++)
                    {
                        Channels.Add(i);
                    }
                    UpdateUI(true);
                }
                else
                {
                    MessageBox.Show("Login failed(登陆失败)!");
                }
            }
            else
            {
                if (playHandle != IntPtr.Zero)
                {
                    NETClient.StopRealPlay(playHandle);
                    playHandle = IntPtr.Zero;
                    PlayBtnText = "Play(监视)";
                    WndEnable = false;
                }
                
                NETClient.Logout(lLoginID);
                lLoginID = IntPtr.Zero;
                UpdateUI(false);
            }
        }

    }
}
