using IVSDemo.Base;
using NetSDKCS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ThermalCamera.ViewModel
{
    class TemperItemViewModel : RadioMetryInfo
    {

        #region
        private int presetID;

        public int PresetID
        {
            get { return presetID; }
            set 
            {
                presetID = value;
                OnPropertyChanged("PresetID");
            }
        }

        private int ruleId;

        public int RuleId
        {
            get { return ruleId; }
            set
            {
                ruleId = value;
                OnPropertyChanged("RuleId");
            }
        }

        private int queryMeterType;

        public int QueryMeterType
        {
            get { return queryMeterType; }
            set
            {
                if (queryMeterType != value)
                {
                    queryMeterType = value;
                    OnPropertyChanged("QueryMeterType");
                }
            }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
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
        
        #endregion

        public ICommand QueryCmd { get; set; }
        public ObservableCollection<string> MeterTypeList { get; set; }

        public TemperItemViewModel()
        {
            QueryCmd = new DelegateCommand(QueryTemperItemInfo);
            MeterTypeList = new ObservableCollection<string>();

            MeterTypeList.Add("Spot(点)");
            MeterTypeList.Add("Line(线)");
            MeterTypeList.Add("Area(区域)");
        }

        private void QueryTemperItemInfo()
        {
            bool result = false;
            string s = Type;

            NET_IN_RADIOMETRY_GETTEMPER inBuf = new NET_IN_RADIOMETRY_GETTEMPER()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_GETTEMPER)),
                stCondition = new NET_RADIOMETRY_CONDITION()
                {
                    nPresetId = PresetID,
                    nRuleId = RuleId,
                    nMeterType = QueryMeterType + 1,
                    nChannel = Channel
                },
            };
            PresetID = inBuf.stCondition.nPresetId;
            RuleId = inBuf.stCondition.nRuleId;
            Channel = inBuf.stCondition.nChannel;

            NET_OUT_RADIOMETRY_GETTEMPER outBuf = new NET_OUT_RADIOMETRY_GETTEMPER()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_GETTEMPER)),
                stTempInfo = new NET_RADIOMETRYINFO(),
            };

            IntPtr inPtr = IntPtr.Zero;
            IntPtr outPtr = IntPtr.Zero;

            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_GETTEMPER)));
                outPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_GETTEMPER)));
                Marshal.StructureToPtr(inBuf, inPtr, true);
                Marshal.StructureToPtr(outBuf, outPtr, true);
                result = NETClient.QueryDevInfo(ThermalViewModel.lLoginID, EM_QUERY_DEV_INFO.RADIOMETRY_TEMPER, inPtr, outPtr, 3000);
                if (result)
                {
                    outBuf = (NET_OUT_RADIOMETRY_GETTEMPER)Marshal.PtrToStructure(outPtr, typeof(NET_OUT_RADIOMETRY_GETTEMPER));
                    switch (outBuf.stTempInfo.nMeterType)
                    {
                        case 1:
                            MeterType = "Spot(点)";
                            break;
                        case 2:
                            MeterType = "Line(线)";
                            break;
                        case 3:
                            MeterType = "Area(区域)";
                            break;
                        default:
                            MeterType = "UnKnown(未知)";
                            break;
                    }
                    switch (outBuf.stTempInfo.nTemperUnit)
                    {
                        case 1:
                            TemperUnit = "Centigrade(摄氏度)";
                            break;
                        case 2:
                            TemperUnit = "Fahrenheit(华氏度)";
                            break;
                        default:
                            TemperUnit = "UnKnown(未知)";
                            break;
                    }
                    TemperAver = outBuf.stTempInfo.fTemperAver;
                    TemperMax = outBuf.stTempInfo.fTemperMax;
                    TemperMin = outBuf.stTempInfo.fTemperMin;
                    TemperMid = outBuf.stTempInfo.fTemperMid;
                    TemperStd = outBuf.stTempInfo.fTemperStd;
                }
                else
                {
                    MessageBox.Show("Querying the temperature measurement item failed(查询测温项的参数值失败)!");
                    MeterType = "";
                    TemperUnit = "";
                    TemperAver = 0;
                    TemperMax = 0;
                    TemperMin = 0;
                    TemperMid = 0;
                    TemperStd = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
                Marshal.FreeHGlobal(outPtr);
            }
        }

    }
}
