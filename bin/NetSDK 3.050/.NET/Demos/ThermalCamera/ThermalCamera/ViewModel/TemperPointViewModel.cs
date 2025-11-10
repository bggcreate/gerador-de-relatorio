using IVSDemo.Base;
using NetSDKCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ThermalCamera.ViewModel
{
    class TemperPointViewModel : RadioMetryInfo
    {
        // 
        #region
        private int queryChannel;

        public int QueryChannel
        {
            get { return queryChannel; }
            set 
            {
                queryChannel = value;
                OnPropertyChanged("QueryChannel");
            }
        }

        private short coordinateX;

        public short CoordinateX
        {
            get { return coordinateX; }
            set 
            {
                coordinateX = value;
                OnPropertyChanged("CoordinateX");
            }
        }

        private short coordinateY;

        public short CoordinateY
        {
            get { return coordinateY; }
            set 
            {
                coordinateY = value;
                OnPropertyChanged("CoordinateY");
            }
        }
        
        #endregion

        public ICommand QueryCmd { get; set; }

        public TemperPointViewModel()
        {
            QueryCmd = new DelegateCommand(QueryTemperPointInfo);
        }

        private void QueryTemperPointInfo()
        {
            bool result = false;
            NET_IN_RADIOMETRY_GETPOINTTEMPER inBuf = new NET_IN_RADIOMETRY_GETPOINTTEMPER()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_GETPOINTTEMPER)),
                nChannel = QueryChannel,
                stCoordinate = new NET_POINT(){
                    nx = CoordinateX,
                    ny = CoordinateY
                },
            };
            QueryChannel = inBuf.nChannel;
            CoordinateX = inBuf.stCoordinate.nx;
            CoordinateY = inBuf.stCoordinate.ny;

            NET_OUT_RADIOMETRY_GETPOINTTEMPER outBuf = new NET_OUT_RADIOMETRY_GETPOINTTEMPER()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_GETPOINTTEMPER)),
                stPointTempInfo = new NET_RADIOMETRYINFO(),
            };

            IntPtr inPtr = IntPtr.Zero;
            IntPtr outPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_RADIOMETRY_GETPOINTTEMPER)));
                outPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_RADIOMETRY_GETPOINTTEMPER)));
                Marshal.StructureToPtr(inBuf, inPtr, true);
                Marshal.StructureToPtr(outBuf, outPtr, true);
                result = NETClient.QueryDevInfo(ThermalViewModel.lLoginID, EM_QUERY_DEV_INFO.RADIOMETRY_POINT_TEMPER, inPtr, outPtr, 3000);
                if (result)
                {
                    outBuf = (NET_OUT_RADIOMETRY_GETPOINTTEMPER)Marshal.PtrToStructure(outPtr, typeof(NET_OUT_RADIOMETRY_GETPOINTTEMPER));
                    switch(outBuf.stPointTempInfo.nMeterType)
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
                    //MeterType = Enum.GetName(typeof(EM_RADIOMETRY_METERTYPE), outBuf.stPointTempInfo.nMeterType);
                    switch(outBuf.stPointTempInfo.nTemperUnit)
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
                    TemperAver = outBuf.stPointTempInfo.fTemperAver;
                    TemperMax = outBuf.stPointTempInfo.fTemperMax;
                    TemperMin = outBuf.stPointTempInfo.fTemperMin;
                    TemperMid = outBuf.stPointTempInfo.fTemperMid;
                    TemperStd = outBuf.stPointTempInfo.fTemperStd;
                }
                else
                {
                    MessageBox.Show("Querying the temperature measurement point failed(查询测温点参数值失败)!");
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
