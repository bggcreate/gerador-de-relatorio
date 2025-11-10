using IVSDemo.Base;
using NetSDKCS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ThermalCamera.ViewModel
{
    class QueryInfo
    {
        public string Time { get; set; }
        public int PresetID { get; set; }
        public int RuleID { get; set; }
        public string Name { get; set; }
        public int Channel { get; set; }
        public string Coordinate { get; set; }
        public string MeterType { get; set; }
        public float TemperAver { get; set; }
        public float TemperMax { get; set; }
        public float TemperMin { get; set; }
    }

    class TemperQueryViewModel:BindableObject
    {
        #region

        // start time and end time

        private int queryType;

        public int QueryType
        {
            get { return queryType; }
            set 
            {
                if (queryType != value)
                {
                    queryType = value;
                    OnPropertyChanged("QueryType");
                }
            }
        }

        private int channel;

        public int Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                OnPropertyChanged("Channel");
            }
        }

        private int totalCount;

        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                if (totalCount != value)
                {
                    totalCount = value;
                    OnPropertyChanged("TotalCount");
                }
            }
        }

        private DateTime startTime = DateTime.Now;

        public DateTime StartTime
        {
            get { return startTime; }
            set 
            {
                if (startTime != value)
                {
                    startTime = value;
                    OnPropertyChanged("StartTime");
                }
            }
        }

        private DateTime endTime = DateTime.Now;

        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    endTime = value;
                    OnPropertyChanged("EndTime");
                }
            }
        }

        private bool queryEn = true;

        public bool QueryEn
        {
            get { return queryEn; }
            set 
            {
                if (queryEn != value)
                {
                    queryEn = value;
                    OnPropertyChanged("QueryEn");
                } 
            }
        }

        #endregion

        private int FindHandle;
        public ICommand QueryCmd { get; set; }
        public ICommand ClosedCmd { get; set; }
        public ObservableCollection<string> MeterTypeList { get; set; }
        public ObservableCollection<QueryInfo> QueryList { get; set; }
        private CancellationTokenSource CancelToken;

        public TemperQueryViewModel()
        {
            QueryCmd = new DelegateCommand(QueryRadioMetry);
            ClosedCmd = new DelegateCommand(ClosedWnd);
            QueryList = new ObservableCollection<QueryInfo>();
            MeterTypeList = new ObservableCollection<string>();

            MeterTypeList.Add("Spot(点)");
            MeterTypeList.Add("Line(线)");
            MeterTypeList.Add("Area(区域)");
        }

        private void ClosedWnd()
        {
            if (CancelToken != null)
            {
                CancelToken.Cancel();
            }
            
            if (FindHandle != 0)
            {
                StopFind();
            }
        }

        private void QueryRadioMetry()
        {
            bool result = false;
            QueryList.Clear();

            NET_IN_RADIOMETRY_STARTFIND inBuf = new NET_IN_RADIOMETRY_STARTFIND()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_STARTFIND)),
                stStartTime = new NET_TIME(),
                stEndTime = new NET_TIME(),
                nMeterType = QueryType + 1,
                nChannel = Channel,
                emPeriod = EM_RADIOMETRY_PERIOD.PERIOD_5
            };
            Channel = inBuf.nChannel;

            inBuf.stStartTime = NET_TIME.FromDateTime(StartTime);
            inBuf.stEndTime = NET_TIME.FromDateTime(EndTime);

            NET_OUT_RADIOMETRY_STARTFIND outBuf = new NET_OUT_RADIOMETRY_STARTFIND()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_STARTFIND))
            };

            IntPtr inPtr = IntPtr.Zero;
            IntPtr outPtr = IntPtr.Zero;
            IntPtr DoInPtr = IntPtr.Zero;
            IntPtr DoOutPtr = IntPtr.Zero;
            
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_STARTFIND)));
                outPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_STARTFIND)));
                Marshal.StructureToPtr(inBuf, inPtr, true);
                Marshal.StructureToPtr(outBuf, outPtr, true);
                result = NETClient.StartFind(ThermalViewModel.lLoginID, EM_FIND.RADIOMETRY, inPtr, outPtr, 3000);
                if (!result)
                {
                    QueryEn = true;
                    MessageBox.Show("Start find failed(开始查询失败)!");
                    return;
                }
                outBuf = (NET_OUT_RADIOMETRY_STARTFIND)Marshal.PtrToStructure(outPtr, typeof(NET_OUT_RADIOMETRY_STARTFIND));
                QueryEn = false;
                TotalCount = outBuf.nTotalCount;
                FindHandle = outBuf.nFinderHanle;
                NET_IN_RADIOMETRY_DOFIND doInBuf = new NET_IN_RADIOMETRY_DOFIND
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_DOFIND)),
                    nFinderHanle = FindHandle,
                    nBeginNumber = 0,
                    nCount = 20
                };
                NET_OUT_RADIOMETRY_DOFIND doOutBuf = new NET_OUT_RADIOMETRY_DOFIND()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_DOFIND))
                };

                CancelToken = new CancellationTokenSource();
                
                Task taskFind = new Task(new Action(()=>
                {
                    try
                    {
                        DoInPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_DOFIND)));
                        DoOutPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_DOFIND)));
                        int FindCount = 0;
                        while (true)
                        {
                            if (CancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                            Marshal.StructureToPtr(doInBuf, DoInPtr, true);
                            Marshal.StructureToPtr(doOutBuf, DoOutPtr, true);
                            if (doInBuf.nBeginNumber >= TotalCount)
                            {
                                break;
                            }
                            result = NETClient.DoFind(ThermalViewModel.lLoginID, EM_FIND.RADIOMETRY, DoInPtr, DoOutPtr, 3000);
                            if (result)
                            {
                                doOutBuf = (NET_OUT_RADIOMETRY_DOFIND)Marshal.PtrToStructure(DoOutPtr, typeof(NET_OUT_RADIOMETRY_DOFIND));
                                for (int i = 0; i < doOutBuf.nFound; i++)
                                {
                                    QueryInfo info = new QueryInfo()
                                    {
                                        Name = doOutBuf.stInfo[i].szName,
                                        Time = doOutBuf.stInfo[i].stTime.ToString(),
                                        PresetID = doOutBuf.stInfo[i].nPresetId,
                                        RuleID = doOutBuf.stInfo[i].nRuleId,
                                        Channel = doOutBuf.stInfo[i].nChannel,
                                        Coordinate = "(" + doOutBuf.stInfo[i].stCoordinate.nx.ToString() + ", " + doOutBuf.stInfo[i].stCoordinate.ny.ToString() + ")",
                                        TemperAver = doOutBuf.stInfo[i].stTemperInfo.fTemperAver,
                                        TemperMax = doOutBuf.stInfo[i].stTemperInfo.fTemperMax,
                                        TemperMin = doOutBuf.stInfo[i].stTemperInfo.fTemperMin
                                    };
                                    switch (doOutBuf.stInfo[i].stTemperInfo.nMeterType)
                                    {
                                        case 1:
                                            info.MeterType = "Spot(点)";
                                            break;
                                        case 2:
                                            info.MeterType = "Line(线)";
                                            break;
                                        case 3:
                                            info.MeterType = "Area(区域)";
                                            break;
                                        default:
                                            info.MeterType = "Unknown(未知)";
                                            break;
                                    }

                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        QueryList.Add(info);
                                    }));

                                }
                                doInBuf.nBeginNumber += doOutBuf.nFound;
                            }
                            else
                            {
                                if (++FindCount > 3)
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        MessageBox.Show("Do find failed(查询失败)");
                                    }));
                                    break;
                                }
                            }
                            Thread.Sleep(10);
                        }
                    }
                    catch(Exception ex)
                    {
                        string message = ex.Message;
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(message);

                        }));
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(DoInPtr);
                        Marshal.FreeHGlobal(DoOutPtr);
                    }
                }), CancelToken.Token);
                taskFind.Start();
                Task TaskStopFind = new Task(new Action(() =>
                {
                    while (true)
                    {
                        if (CancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        if (taskFind.IsCompleted)
                        {
                            StopFind();
                            break;
                        }
                        Thread.Sleep(20);
                    }
                }), CancelToken.Token);
                TaskStopFind.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
                Marshal.FreeHGlobal(outPtr);
            }
        }

        private void StopFind()
        {
            NET_IN_RADIOMETRY_STOPFIND stopIndBuf = new NET_IN_RADIOMETRY_STOPFIND()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_STOPFIND)),
                nFinderHanle = FindHandle
            };
            NET_OUT_RADIOMETRY_STOPFIND stopOutBuf = new NET_OUT_RADIOMETRY_STOPFIND()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_STOPFIND))
            };
            IntPtr StopInPtr = IntPtr.Zero;
            IntPtr StopOutPtr = IntPtr.Zero;
            try
            { 
                StopInPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_STOPFIND)));
                StopOutPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_STOPFIND)));
                Marshal.StructureToPtr(stopIndBuf, StopInPtr, true);
                Marshal.StructureToPtr(stopOutBuf, StopOutPtr, true);
                bool result = NETClient.StopFind(ThermalViewModel.lLoginID, EM_FIND.RADIOMETRY, StopInPtr, StopOutPtr, 3000);
                QueryEn = true;
                FindHandle = 0;
                if (!result)
                {
                    MessageBox.Show("Stop find failed(结束查询失败)!");
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(StopInPtr);
                Marshal.FreeHGlobal(StopOutPtr);
            }
        }
    }

    
}
