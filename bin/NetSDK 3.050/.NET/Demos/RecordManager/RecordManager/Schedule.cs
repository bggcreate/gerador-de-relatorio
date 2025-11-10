using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSDKCS;
using NetSDKCSControl;

namespace RecordManager
{
    public partial class Schedule : Form
    {
        private const string CFG_CMD_RECORD = "Record";
        private IntPtr _LoginID = IntPtr.Zero;
        private int _ChannelCount = 0;
        ONEDAY_CFG_TIME_SECTION[] _TimeSections = new ONEDAY_CFG_TIME_SECTION[7];
        List<Time_Info> _TimeList = new List<Time_Info>();
        List<Time_Info> _SundayTimeList = new List<Time_Info>();
        List<Time_Info> _MondayTimeList = new List<Time_Info>();
        List<Time_Info> _TuesdayTimeList = new List<Time_Info>();
        List<Time_Info> _WednesdayTimeList = new List<Time_Info>();
        List<Time_Info> _ThursdayTimeList = new List<Time_Info>();
        List<Time_Info> _FridayTimeList = new List<Time_Info>();
        List<Time_Info> _SaturdayTimeList = new List<Time_Info>();

        public Schedule(IntPtr loginID, int channelCount)
        {
            InitializeComponent();
            _LoginID = loginID;
            _ChannelCount = channelCount;
            for (int i = 0; i < channelCount; i++)
            {
                comboBox_channel.Items.Add(i + 1);
            }
            comboBox_channel.SelectedIndex = 0;
            GetInformation(0);
        }

        private void GetInformation(int channelNo)
        {
            CFG_RECORD_INFO info = new CFG_RECORD_INFO();
            info.nChannelID = channelNo;
            object obj = info;
            bool ret = NETClient.GetNewDevConfig(_LoginID, channelNo, CFG_CMD_RECORD, ref obj, typeof(CFG_RECORD_INFO), 5000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            info = (CFG_RECORD_INFO)obj;
            _TimeSections = info.stuTimeSections;
            for (int i = 0; i < 7; i++)
            {

                RecordTime[] generalTime = new RecordTime[6];
                RecordTime[] motionTime = new RecordTime[6];
                RecordTime[] alarmTime = new RecordTime[6];
                RecordTime[] motionalarmTime = new RecordTime[6];
                for (int j = 0; j < 6; j++)
                {
                    if (info.stuTimeSections[i].stuTimeSection[j].dwRecordMask != 0)
                    {
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask >> 2 & 0x1) != 0)//general
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            generalTime[j] = rt;
                        }
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask & 0x1) != 0)//motion
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            motionTime[j] = rt;
                        }
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask >> 1 & 0x1) != 0)//alarm
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            alarmTime[j] = rt;
                        }
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask >> 3 & 0x1) != 0)//motion and alarm
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            motionalarmTime[j] = rt;
                        }
                    }
                }
                _TimeList.Clear();
                for(int k = 0;k < 4; k++)
                {
                    Time_Info ti = new Time_Info();
                    ti.Type = (Em_Type)k;
                    switch(ti.Type)
                    {
                        case Em_Type.General:
                            ti.Times = generalTime;
                            break;
                        case Em_Type.Motion:
                            ti.Times = motionTime;
                            break;
                        case Em_Type.Alarm:
                            ti.Times = alarmTime;
                            break;
                        case Em_Type.MotionAlarm:
                            ti.Times = motionalarmTime;
                            break;
                    }
                    _TimeList.Add(ti);
                }
                switch ((Em_Week)i)
                {
                    case Em_Week.Sunday:
                        _SundayTimeList.Clear();
                        foreach(var item in _TimeList)
                        {
                            _SundayTimeList.Add(item);
                        }
                        this.timeProgressBar_sunday.TimeInfoList = _SundayTimeList;
                        break;
                    case Em_Week.Monday:
                        _MondayTimeList.Clear();
                        foreach(var item in _TimeList)
                        {
                            _MondayTimeList.Add(item);
                        }
                        this.timeProgressBar_monday.TimeInfoList = _MondayTimeList;
                        break;
                    case Em_Week.Tuesday:
                        _TuesdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _TuesdayTimeList.Add(item);
                        }
                        this.timeProgressBar_tuesday.TimeInfoList = _TuesdayTimeList;
                        break;
                    case Em_Week.Wednesday:
                        _WednesdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _WednesdayTimeList.Add(item);
                        }
                        this.timeProgressBar_wednesday.TimeInfoList = _WednesdayTimeList;
                        break;
                    case Em_Week.Thursday:
                        _ThursdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _ThursdayTimeList.Add(item);
                        }
                        this.timeProgressBar_thursday.TimeInfoList = _ThursdayTimeList;
                        break;
                    case Em_Week.Friday:
                        _FridayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _FridayTimeList.Add(item);
                        }
                        this.timeProgressBar_friday.TimeInfoList = _FridayTimeList;
                        break;
                    case Em_Week.Saturday:
                        _SaturdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _SaturdayTimeList.Add(item);
                        }
                        this.timeProgressBar_saturday.TimeInfoList = _SaturdayTimeList;
                        break;
                }
            }
        }

        private void RefreshInformation(int channelNo)
        {
            CFG_RECORD_INFO info = new CFG_RECORD_INFO();
            info.nChannelID = channelNo;
            object obj = info;
            bool ret = NETClient.GetNewDevConfig(_LoginID, channelNo, CFG_CMD_RECORD, ref obj, typeof(CFG_RECORD_INFO), 3000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            info = (CFG_RECORD_INFO)obj;
            _TimeSections = info.stuTimeSections;
            for (int i = 0; i < 7; i++)
            {

                RecordTime[] generalTime = new RecordTime[6];
                RecordTime[] motionTime = new RecordTime[6];
                RecordTime[] alarmTime = new RecordTime[6];
                RecordTime[] motionalarmTime = new RecordTime[6];
                for (int j = 0; j < 6; j++)
                {
                    if (info.stuTimeSections[i].stuTimeSection[j].dwRecordMask != 0)
                    {
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask >> 2 & 0x1) != 0)//general
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            generalTime[j] = rt;
                        }
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask & 0x1) != 0)//motion
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            motionTime[j] = rt;
                        }
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask >> 1 & 0x1) != 0)//alarm
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            alarmTime[j] = rt;
                        }
                        if ((info.stuTimeSections[i].stuTimeSection[j].dwRecordMask >> 3 & 0x1) != 0)//motion and alarm
                        {
                            RecordTime rt = new RecordTime();
                            rt.BeginHour = info.stuTimeSections[i].stuTimeSection[j].nBeginHour;
                            rt.BeginMinute = info.stuTimeSections[i].stuTimeSection[j].nBeginMin;
                            rt.BeginSecond = info.stuTimeSections[i].stuTimeSection[j].nBeginSec;
                            rt.EndHour = info.stuTimeSections[i].stuTimeSection[j].nEndHour;
                            rt.EndMinute = info.stuTimeSections[i].stuTimeSection[j].nEndMin;
                            rt.EndSecond = info.stuTimeSections[i].stuTimeSection[j].nEndSec;
                            motionalarmTime[j] = rt;
                        }
                    }
                }
                _TimeList.Clear();
                for (int k = 0; k < 4; k++)
                {
                    Time_Info ti = new Time_Info();
                    ti.Type = (Em_Type)k;
                    switch (ti.Type)
                    {
                        case Em_Type.General:
                            ti.Times = generalTime;
                            break;
                        case Em_Type.Motion:
                            ti.Times = motionTime;
                            break;
                        case Em_Type.Alarm:
                            ti.Times = alarmTime;
                            break;
                        case Em_Type.MotionAlarm:
                            ti.Times = motionalarmTime;
                            break;
                    }
                    _TimeList.Add(ti);
                }
                switch ((Em_Week)i)
                {
                    case Em_Week.Sunday:
                        _SundayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _SundayTimeList.Add(item);
                        }
                        this.timeProgressBar_sunday.SetTimeInfo(_SundayTimeList);
                        break;
                    case Em_Week.Monday:
                        _MondayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _MondayTimeList.Add(item);
                        }
                        this.timeProgressBar_monday.SetTimeInfo(_MondayTimeList);
                        break;
                    case Em_Week.Tuesday:
                        _TuesdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _TuesdayTimeList.Add(item);
                        }
                        this.timeProgressBar_tuesday.SetTimeInfo(_TuesdayTimeList);
                        break;
                    case Em_Week.Wednesday:
                        _WednesdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _WednesdayTimeList.Add(item);
                        }
                        this.timeProgressBar_wednesday.SetTimeInfo(_WednesdayTimeList);
                        break;
                    case Em_Week.Thursday:
                        _ThursdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _ThursdayTimeList.Add(item);
                        }
                        this.timeProgressBar_thursday.SetTimeInfo(_ThursdayTimeList);
                        break;
                    case Em_Week.Friday:
                        _FridayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _FridayTimeList.Add(item);
                        }
                        this.timeProgressBar_friday.SetTimeInfo(_FridayTimeList);
                        break;
                    case Em_Week.Saturday:
                        _SaturdayTimeList.Clear();
                        foreach (var item in _TimeList)
                        {
                            _SaturdayTimeList.Add(item);
                        }
                        this.timeProgressBar_saturday.SetTimeInfo(_SaturdayTimeList);
                        break;
                }
            }
        }

        private void comboBox_channel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInformation(comboBox_channel.SelectedIndex);
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            RefreshInformation(comboBox_channel.SelectedIndex);
        }

        private void button_sundaysetup_Click(object sender, EventArgs e)
        {
            ScheduleSetup sst = new ScheduleSetup(Em_Week.Sunday, _SundayTimeList);
            var ret = sst.ShowDialog();
            if(ret == System.Windows.Forms.DialogResult.OK)
            {
                _SundayTimeList = sst._TimeList;
                this.timeProgressBar_sunday.SetTimeInfo(sst._TimeList);
            }
            sst.Dispose();
        }

        private void button_mondaysetup_Click(object sender, EventArgs e)
        {
            ScheduleSetup sst = new ScheduleSetup(Em_Week.Monday, _MondayTimeList);
            var ret = sst.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                _MondayTimeList = sst._TimeList;
                this.timeProgressBar_monday.SetTimeInfo(sst._TimeList);
            }
            sst.Dispose();
        }

        private void button_tuesdaysetup_Click(object sender, EventArgs e)
        {
            ScheduleSetup sst = new ScheduleSetup(Em_Week.Tuesday, _TuesdayTimeList);
            var ret = sst.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                _TuesdayTimeList = sst._TimeList;
                this.timeProgressBar_tuesday.SetTimeInfo(sst._TimeList);
            }
            sst.Dispose();
        }

        private void button_wednesdaysetup_Click(object sender, EventArgs e)
        {
            ScheduleSetup sst = new ScheduleSetup(Em_Week.Wednesday, _WednesdayTimeList);
            var ret = sst.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                _WednesdayTimeList = sst._TimeList;
                this.timeProgressBar_wednesday.SetTimeInfo(sst._TimeList);
            }
            sst.Dispose();
        }

        private void button_thursdaysetup_Click(object sender, EventArgs e)
        {
            ScheduleSetup sst = new ScheduleSetup(Em_Week.Thursday, _ThursdayTimeList);
            var ret = sst.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                _ThursdayTimeList = sst._TimeList;
                this.timeProgressBar_thursday.SetTimeInfo(sst._TimeList);
            }
            sst.Dispose();
        }

        private void button_fridaysetup_Click(object sender, EventArgs e)
        {
            ScheduleSetup sst = new ScheduleSetup(Em_Week.Friday, _FridayTimeList);
            var ret = sst.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                _FridayTimeList = sst._TimeList;
                this.timeProgressBar_friday.SetTimeInfo(sst._TimeList);
            }
            sst.Dispose();
        }

        private void button_saturdaysetup_Click(object sender, EventArgs e)
        {
            ScheduleSetup sst = new ScheduleSetup(Em_Week.Saturday, _SaturdayTimeList);
            var ret = sst.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                _SaturdayTimeList = sst._TimeList;
                this.timeProgressBar_saturday.SetTimeInfo(sst._TimeList);
            }
            sst.Dispose();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            ONEDAY_CFG_TIME_SECTION[] weekTimes = new ONEDAY_CFG_TIME_SECTION[7];
            for(int i =0; i < weekTimes.Length; i++)
            {
                weekTimes[i] = new ONEDAY_CFG_TIME_SECTION();
                weekTimes[i].stuTimeSection = new CFG_TIME_SECTION[6];
                for(int j =0; j< weekTimes[i].stuTimeSection.Length; j++)
                {
                    switch((Em_Week)i)
                    {
                        case Em_Week.Sunday:
                            for(int m = 0;m < _SundayTimeList.Count; m++)
                            {
                                switch((Em_Type)m)
                                {
                                    case  Em_Type.General:
                                        if(_SundayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x4;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SundayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SundayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SundayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SundayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SundayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SundayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Motion:
                                        if (_SundayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x1;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SundayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SundayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SundayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SundayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SundayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SundayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Alarm:
                                        if (_SundayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x2;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SundayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SundayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SundayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SundayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SundayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SundayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.MotionAlarm:
                                        if (_SundayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x8;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SundayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SundayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SundayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SundayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SundayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SundayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                }
                            }
                            break;
                        case Em_Week.Monday:
                            for (int m = 0; m < _MondayTimeList.Count; m++)
                            {
                                switch ((Em_Type)m)
                                {
                                    case Em_Type.General:
                                        if (_MondayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x4;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _MondayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _MondayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _MondayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _MondayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _MondayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _MondayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Motion:
                                        if (_MondayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x1;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _MondayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _MondayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _MondayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _MondayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _MondayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _MondayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Alarm:
                                        if (_MondayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x2;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _MondayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _MondayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _MondayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _MondayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _MondayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _MondayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.MotionAlarm:
                                        if (_MondayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x8;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _MondayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _MondayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _MondayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _MondayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _MondayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _MondayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                }
                            }
                            break;
                        case Em_Week.Tuesday:
                            for (int m = 0; m < _TuesdayTimeList.Count; m++)
                            {
                                switch ((Em_Type)m)
                                {
                                    case Em_Type.General:
                                        if (_TuesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x4;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _TuesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _TuesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _TuesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _TuesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _TuesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _TuesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Motion:
                                        if (_TuesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x1;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _TuesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _TuesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _TuesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _TuesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _TuesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _TuesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Alarm:
                                        if (_TuesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x2;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _TuesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _TuesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _TuesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _TuesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _TuesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _TuesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.MotionAlarm:
                                        if (_TuesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x8;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _TuesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _TuesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _TuesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _TuesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _TuesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _TuesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                }
                            }
                            break;
                        case Em_Week.Wednesday:
                            for (int m = 0; m < _WednesdayTimeList.Count; m++)
                            {
                                switch ((Em_Type)m)
                                {
                                    case Em_Type.General:
                                        if (_WednesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x4;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _WednesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _WednesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _WednesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _WednesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _WednesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _WednesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Motion:
                                        if (_WednesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x1;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _WednesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _WednesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _WednesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _WednesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _WednesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _WednesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Alarm:
                                        if (_WednesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x2;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _WednesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _WednesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _WednesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _WednesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _WednesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _WednesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.MotionAlarm:
                                        if (_WednesdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x8;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _WednesdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _WednesdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _WednesdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _WednesdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _WednesdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _WednesdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                }
                            }
                            break;
                        case Em_Week.Thursday:
                            for (int m = 0; m < _ThursdayTimeList.Count; m++)
                            {
                                switch ((Em_Type)m)
                                {
                                    case Em_Type.General:
                                        if (_ThursdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x4;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _ThursdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _ThursdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _ThursdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _ThursdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _ThursdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _ThursdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Motion:
                                        if (_ThursdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x1;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _ThursdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _ThursdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _ThursdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _ThursdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _ThursdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _ThursdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Alarm:
                                        if (_ThursdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x2;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _ThursdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _ThursdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _ThursdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _ThursdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _ThursdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _ThursdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.MotionAlarm:
                                        if (_ThursdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x8;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _ThursdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _ThursdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _ThursdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _ThursdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _ThursdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _ThursdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                }
                            }
                            break;
                        case Em_Week.Friday:
                            for (int m = 0; m < _FridayTimeList.Count; m++)
                            {
                                switch ((Em_Type)m)
                                {
                                    case Em_Type.General:
                                        if (_FridayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x4;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _FridayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _FridayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _FridayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _FridayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _FridayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _FridayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Motion:
                                        if (_FridayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x1;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _FridayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _FridayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _FridayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _FridayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _FridayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _FridayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Alarm:
                                        if (_FridayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x2;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _FridayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _FridayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _FridayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _FridayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _FridayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _FridayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.MotionAlarm:
                                        if (_FridayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x8;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _FridayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _FridayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _FridayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _FridayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _FridayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _FridayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                }
                            }
                            break;
                        case Em_Week.Saturday:
                            for (int m = 0; m < _SaturdayTimeList.Count; m++)
                            {
                                switch ((Em_Type)m)
                                {
                                    case Em_Type.General:
                                        if (_SaturdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x4;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SaturdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SaturdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SaturdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SaturdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SaturdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SaturdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Motion:
                                        if (_SaturdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x1;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SaturdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SaturdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SaturdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SaturdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SaturdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SaturdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.Alarm:
                                        if (_SaturdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x2;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SaturdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SaturdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SaturdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SaturdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SaturdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SaturdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                    case Em_Type.MotionAlarm:
                                        if (_SaturdayTimeList[m].Times[j] != null)
                                        {
                                            weekTimes[i].stuTimeSection[j].dwRecordMask |= 0x8;
                                            weekTimes[i].stuTimeSection[j].nBeginHour = _SaturdayTimeList[m].Times[j].BeginHour;
                                            weekTimes[i].stuTimeSection[j].nBeginMin = _SaturdayTimeList[m].Times[j].BeginMinute;
                                            weekTimes[i].stuTimeSection[j].nBeginSec = _SaturdayTimeList[m].Times[j].BeginSecond;
                                            weekTimes[i].stuTimeSection[j].nEndHour = _SaturdayTimeList[m].Times[j].EndHour;
                                            weekTimes[i].stuTimeSection[j].nEndMin = _SaturdayTimeList[m].Times[j].EndMinute;
                                            weekTimes[i].stuTimeSection[j].nEndSec = _SaturdayTimeList[m].Times[j].EndSecond;
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                    
                }
            }

            CFG_RECORD_INFO info = new CFG_RECORD_INFO();
            info.nChannelID = comboBox_channel.SelectedIndex;
            info.stuTimeSections = weekTimes;
            object obj = info;
            bool ret = NETClient.SetNewDevConfig(_LoginID, comboBox_channel.SelectedIndex, CFG_CMD_RECORD, obj, typeof(CFG_RECORD_INFO), 5000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            MessageBox.Show("Set successfully(设置成功)");
        }
    }

    public enum Em_Week
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
    }
}
