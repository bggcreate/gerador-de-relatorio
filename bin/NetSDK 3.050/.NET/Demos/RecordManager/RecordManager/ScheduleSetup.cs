using NetSDKCSControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RecordManager
{
    public partial class ScheduleSetup : Form
    {
        private Em_Week _Day;
        public List<Time_Info> _TimeList { get; set; }
        private bool _IsLoaded = false;

        public ScheduleSetup(Em_Week day, List<Time_Info> timeList)
        {
            InitializeComponent();
            _Day = day;
            _TimeList = timeList;
            this.Load += ScheduleSetup_Load;
        }

        void ScheduleSetup_Load(object sender, EventArgs e)
        {
            switch(_Day)
            {
                case Em_Week.Sunday:
                    this.checkBox_sunday.Checked = true;
                    break;
                case Em_Week.Monday:
                    this.checkBox_monday.Checked = true;
                    break;
                case Em_Week.Tuesday:
                    this.checkBox_tuesday.Checked = true;
                    break;
                case Em_Week.Wednesday:
                    this.checkBox_wednesday.Checked = true;
                    break;
                case Em_Week.Thursday:
                    this.checkBox_thursday.Checked = true;
                    break;
                case Em_Week.Friday:
                    this.checkBox_friday.Checked = true;
                    break;
                case Em_Week.Saturday:
                    this.checkBox_saturday.Checked = true;
                    break;
            }
            foreach(var item in _TimeList)
            {
                switch(item.Type)
                {
                    case Em_Type.General:
                        for (int i = 0; i < item.Times.Length; i++ )
                        {
                            if(item.Times[i] != null)
                            {
                                if(item.Times[i].EndHour == 24)
                                {
                                    item.Times[i].EndHour = 23;
                                    item.Times[i].EndMinute = 59;
                                    item.Times[i].EndSecond = 59;
                                }
                                if(i == 0)
                                {
                                    this.checkBox_general1.Checked = true;
                                    this.dateTimePicker_start1.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end1.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 1)
                                {
                                    this.checkBox_general2.Checked = true;
                                    this.dateTimePicker_start2.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end2.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 2)
                                {
                                    this.checkBox_general3.Checked = true;
                                    this.dateTimePicker_start3.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end3.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 3)
                                {
                                    this.checkBox_general4.Checked = true;
                                    this.dateTimePicker_start4.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end4.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 4)
                                {
                                    this.checkBox_general5.Checked = true;
                                    this.dateTimePicker_start5.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end5.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 5)
                                {
                                    this.checkBox_general6.Checked = true;
                                    this.dateTimePicker_start6.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end6.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                            }
                        }
                        break;
                    case Em_Type.Motion:
                        for (int i = 0; i < item.Times.Length; i++)
                        {
                            if (item.Times[i] != null)
                            {
                                if (item.Times[i].EndHour == 24)
                                {
                                    item.Times[i].EndHour = 23;
                                    item.Times[i].EndMinute = 59;
                                    item.Times[i].EndSecond = 59;
                                }
                                if (i == 0)
                                {
                                    this.checkBox_motion1.Checked = true;
                                    this.dateTimePicker_start1.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end1.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 1)
                                {
                                    this.checkBox_motion2.Checked = true;
                                    this.dateTimePicker_start2.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end2.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 2)
                                {
                                    this.checkBox_motion3.Checked = true;
                                    this.dateTimePicker_start3.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end3.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 3)
                                {
                                    this.checkBox_motion4.Checked = true;
                                    this.dateTimePicker_start4.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end4.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 4)
                                {
                                    this.checkBox_motion5.Checked = true;
                                    this.dateTimePicker_start5.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end5.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 5)
                                {
                                    this.checkBox_motion6.Checked = true;
                                    this.dateTimePicker_start6.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end6.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                            }
                        }
                        break;
                    case Em_Type.Alarm:
                        for (int i = 0; i < item.Times.Length; i++)
                        {
                            if (item.Times[i] != null)
                            {
                                if (item.Times[i].EndHour == 24)
                                {
                                    item.Times[i].EndHour = 23;
                                    item.Times[i].EndMinute = 59;
                                    item.Times[i].EndSecond = 59;
                                }
                                if (i == 0)
                                {
                                    this.checkBox_alarm1.Checked = true;
                                    this.dateTimePicker_start1.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end1.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 1)
                                {
                                    this.checkBox_alarm2.Checked = true;
                                    this.dateTimePicker_start2.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end2.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 2)
                                {
                                    this.checkBox_alarm3.Checked = true;
                                    this.dateTimePicker_start3.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end3.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 3)
                                {
                                    this.checkBox_alarm4.Checked = true;
                                    this.dateTimePicker_start4.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end4.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 4)
                                {
                                    this.checkBox_alarm5.Checked = true;
                                    this.dateTimePicker_start5.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end5.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 5)
                                {
                                    this.checkBox_alarm6.Checked = true;
                                    this.dateTimePicker_start6.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end6.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                            }
                        }
                        break;
                    case Em_Type.MotionAlarm:
                        for (int i = 0; i < item.Times.Length; i++)
                        {
                            if (item.Times[i] != null)
                            {
                                if (item.Times[i].EndHour == 24)
                                {
                                    item.Times[i].EndHour = 23;
                                    item.Times[i].EndMinute = 59;
                                    item.Times[i].EndSecond = 59;
                                }
                                if (i == 0)
                                {
                                    this.checkBox__ma1.Checked = true;
                                    this.checkBox_motion1.Enabled = false;
                                    this.checkBox_alarm1.Enabled = false;
                                    this.dateTimePicker_start1.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end1.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 1)
                                {
                                    this.checkBox__ma2.Checked = true;
                                    this.checkBox_motion2.Enabled = false;
                                    this.checkBox_alarm2.Enabled = false;
                                    this.dateTimePicker_start2.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end2.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 2)
                                {
                                    this.checkBox__ma3.Checked = true;
                                    this.checkBox_motion3.Enabled = false;
                                    this.checkBox_alarm3.Enabled = false;
                                    this.dateTimePicker_start3.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end3.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 3)
                                {
                                    this.checkBox__ma4.Checked = true;
                                    this.checkBox_motion4.Enabled = false;
                                    this.checkBox_alarm4.Enabled = false;
                                    this.dateTimePicker_start4.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end4.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 4)
                                {
                                    this.checkBox__ma5.Checked = true;
                                    this.checkBox_motion5.Enabled = false;
                                    this.checkBox_alarm5.Enabled = false;
                                    this.dateTimePicker_start5.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end5.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                                if (i == 5)
                                {
                                    this.checkBox__ma6.Checked = true;
                                    this.checkBox_motion6.Enabled = false;
                                    this.checkBox_alarm6.Enabled = false;
                                    this.dateTimePicker_start6.Value = new DateTime(2000, 1, 1, item.Times[i].BeginHour, item.Times[i].BeginMinute, item.Times[i].BeginSecond);
                                    this.dateTimePicker_end6.Value = new DateTime(2000, 1, 1, item.Times[i].EndHour, item.Times[i].EndMinute, item.Times[i].EndSecond);
                                }
                            }
                        }
                        break;
                }
            }
            _IsLoaded = true;
        }

        protected override void OnShown(EventArgs e)
        {
            this.button_ok.Focus();
            base.OnShown(e);
        }

        private void checkBox__ma1_CheckedChanged(object sender, EventArgs e)
        {
            if(!_IsLoaded)
            {
                return;
            }
            if(this.checkBox__ma1.Checked == true)
            {
                this.checkBox_motion1.Enabled = false;
                this.checkBox_alarm1.Enabled = false;
                this.checkBox_motion1.Checked = false;
                this.checkBox_alarm1.Checked = false;
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start1.Value.Hour,
                    BeginMinute = dateTimePicker_start1.Value.Minute,
                    BeginSecond = dateTimePicker_start1.Value.Second,
                    EndHour = dateTimePicker_end1.Value.Hour,
                    EndMinute = dateTimePicker_end1.Value.Minute,
                    EndSecond = dateTimePicker_end1.Value.Second
                };
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                if(this.checkBox_alarm2.Checked || this.checkBox_motion2.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    if(start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if(end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm2.Checked = false;
                                    this.checkBox_motion2.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }

                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm2.Checked = false;
                                        this.checkBox_motion2.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm3.Checked || this.checkBox_motion3.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm3.Checked = false;
                                    this.checkBox_motion3.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm3.Checked = false;
                                        this.checkBox_motion3.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm4.Checked || this.checkBox_motion4.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if(end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm4.Checked = false;
                                    this.checkBox_motion4.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm4.Checked = false;
                                        this.checkBox_motion4.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                    
                }
                if (this.checkBox_alarm5.Checked || this.checkBox_motion5.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm5.Checked = false;
                                    this.checkBox_motion5.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm5.Checked = false;
                                        this.checkBox_motion5.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm6.Checked || this.checkBox_motion6.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm6.Checked = false;
                                    this.checkBox_motion6.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm6.Checked = false;
                                        this.checkBox_motion6.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                this.checkBox_motion1.Enabled = true;
                this.checkBox_alarm1.Enabled = true;
            }
        }

        private void checkBox__ma2_CheckedChanged(object sender, EventArgs e)
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (this.checkBox__ma2.Checked == true)
            {
                this.checkBox_motion2.Enabled = false;
                this.checkBox_alarm2.Enabled = false;
                this.checkBox_motion2.Checked = false;
                this.checkBox_alarm2.Checked = false;
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start2.Value.Hour,
                    BeginMinute = dateTimePicker_start2.Value.Minute,
                    BeginSecond = dateTimePicker_start2.Value.Second,
                    EndHour = dateTimePicker_end2.Value.Hour,
                    EndMinute = dateTimePicker_end2.Value.Minute,
                    EndSecond = dateTimePicker_end2.Value.Second
                };
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                if (this.checkBox_alarm1.Checked || this.checkBox_motion1.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm1.Checked = false;
                                    this.checkBox_motion1.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm1.Checked = false;
                                        this.checkBox_motion1.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm3.Checked || this.checkBox_motion3.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm3.Checked = false;
                                    this.checkBox_motion3.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm3.Checked = false;
                                        this.checkBox_motion3.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm4.Checked || this.checkBox_motion4.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm4.Checked = false;
                                    this.checkBox_motion4.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm4.Checked = false;
                                        this.checkBox_motion4.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm5.Checked || this.checkBox_motion5.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm5.Checked = false;
                                    this.checkBox_motion5.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm5.Checked = false;
                                        this.checkBox_motion5.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm6.Checked || this.checkBox_motion6.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm6.Checked = false;
                                    this.checkBox_motion6.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm6.Checked = false;
                                        this.checkBox_motion6.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            } 
                        }
                    }
                }
            }
            else
            {
                this.checkBox_motion2.Enabled = true;
                this.checkBox_alarm2.Enabled = true;
            }
        }

        private void checkBox__ma3_CheckedChanged(object sender, EventArgs e)
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (this.checkBox__ma3.Checked == true)
            {
                this.checkBox_motion3.Enabled = false;
                this.checkBox_alarm3.Enabled = false;
                this.checkBox_motion3.Checked = false;
                this.checkBox_alarm3.Checked = false;
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start3.Value.Hour,
                    BeginMinute = dateTimePicker_start3.Value.Minute,
                    BeginSecond = dateTimePicker_start3.Value.Second,
                    EndHour = dateTimePicker_end3.Value.Hour,
                    EndMinute = dateTimePicker_end3.Value.Minute,
                    EndSecond = dateTimePicker_end3.Value.Second
                };
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                if (this.checkBox_alarm1.Checked || this.checkBox_motion1.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm1.Checked = false;
                                    this.checkBox_motion1.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if(temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm1.Checked = false;
                                        this.checkBox_motion1.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }                            
                        }
                    }
                }
                if (this.checkBox_alarm2.Checked || this.checkBox_motion2.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm2.Checked = false;
                                    this.checkBox_motion2.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (start.CompareTo(temp1) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm2.Checked = false;
                                        this.checkBox_motion2.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm4.Checked || this.checkBox_motion4.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm4.Checked = false;
                                    this.checkBox_motion4.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm4.Checked = false;
                                        this.checkBox_motion4.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm5.Checked || this.checkBox_motion5.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm5.Checked = false;
                                    this.checkBox_motion5.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm5.Checked = false;
                                        this.checkBox_motion5.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm6.Checked || this.checkBox_motion6.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm6.Checked = false;
                                    this.checkBox_motion6.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if(temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm6.Checked = false;
                                        this.checkBox_motion6.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                this.checkBox_motion3.Enabled = true;
                this.checkBox_alarm3.Enabled = true;
            }
        }

        private void checkBox__ma4_CheckedChanged(object sender, EventArgs e)
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (this.checkBox__ma4.Checked == true)
            {
                this.checkBox_motion4.Enabled = false;
                this.checkBox_alarm4.Enabled = false;
                this.checkBox_motion4.Checked = false;
                this.checkBox_alarm4.Checked = false;
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start4.Value.Hour,
                    BeginMinute = dateTimePicker_start4.Value.Minute,
                    BeginSecond = dateTimePicker_start4.Value.Second,
                    EndHour = dateTimePicker_end4.Value.Hour,
                    EndMinute = dateTimePicker_end4.Value.Minute,
                    EndSecond = dateTimePicker_end4.Value.Second
                };
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                if (this.checkBox_alarm1.Checked || this.checkBox_motion1.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm1.Checked = false;
                                    this.checkBox_motion1.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm1.Checked = false;
                                        this.checkBox_motion1.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm2.Checked || this.checkBox_motion2.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm2.Checked = false;
                                    this.checkBox_motion2.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm2.Checked = false;
                                        this.checkBox_motion2.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm3.Checked || this.checkBox_motion3.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm3.Checked = false;
                                    this.checkBox_motion3.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm3.Checked = false;
                                        this.checkBox_motion3.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm5.Checked || this.checkBox_motion5.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm5.Checked = false;
                                    this.checkBox_motion5.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm5.Checked = false;
                                        this.checkBox_motion5.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm6.Checked || this.checkBox_motion6.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm6.Checked = false;
                                    this.checkBox_motion6.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm6.Checked = false;
                                        this.checkBox_motion6.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                this.checkBox_motion4.Enabled = true;
                this.checkBox_alarm4.Enabled = true;
            }
        }

        private void checkBox__ma5_CheckedChanged(object sender, EventArgs e)
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (this.checkBox__ma5.Checked == true)
            {
                this.checkBox_motion5.Enabled = false;
                this.checkBox_alarm5.Enabled = false;
                this.checkBox_motion5.Checked = false;
                this.checkBox_alarm5.Checked = false;
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start5.Value.Hour,
                    BeginMinute = dateTimePicker_start5.Value.Minute,
                    BeginSecond = dateTimePicker_start5.Value.Second,
                    EndHour = dateTimePicker_end5.Value.Hour,
                    EndMinute = dateTimePicker_end5.Value.Minute,
                    EndSecond = dateTimePicker_end5.Value.Second
                };
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                if (this.checkBox_alarm1.Checked || this.checkBox_motion1.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm1.Checked = false;
                                    this.checkBox_motion1.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm1.Checked = false;
                                        this.checkBox_motion1.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm2.Checked || this.checkBox_motion2.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm2.Checked = false;
                                    this.checkBox_motion2.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm2.Checked = false;
                                        this.checkBox_motion2.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm3.Checked || this.checkBox_motion3.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm3.Checked = false;
                                    this.checkBox_motion3.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm3.Checked = false;
                                        this.checkBox_motion3.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm4.Checked || this.checkBox_motion4.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm4.Checked = false;
                                    this.checkBox_motion4.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm4.Checked = false;
                                        this.checkBox_motion4.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm6.Checked || this.checkBox_motion6.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm6.Checked = false;
                                    this.checkBox_motion6.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm6.Checked = false;
                                        this.checkBox_motion6.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm6.Checked = false;
                                this.checkBox_motion6.Checked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                this.checkBox_motion5.Enabled = true;
                this.checkBox_alarm5.Enabled = true;
            }
        }

        private void checkBox__ma6_CheckedChanged(object sender, EventArgs e)
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (this.checkBox__ma6.Checked == true)
            {
                this.checkBox_motion6.Enabled = false;
                this.checkBox_alarm6.Enabled = false;
                this.checkBox_motion6.Checked = false;
                this.checkBox_alarm6.Checked = false;
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start6.Value.Hour,
                    BeginMinute = dateTimePicker_start6.Value.Minute,
                    BeginSecond = dateTimePicker_start6.Value.Second,
                    EndHour = dateTimePicker_end6.Value.Hour,
                    EndMinute = dateTimePicker_end6.Value.Minute,
                    EndSecond = dateTimePicker_end6.Value.Second
                };
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                if (this.checkBox_alarm1.Checked || this.checkBox_motion1.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm1.Checked = false;
                                    this.checkBox_motion1.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm1.Checked = false;
                                        this.checkBox_motion1.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm1.Checked = false;
                                this.checkBox_motion1.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm2.Checked || this.checkBox_motion2.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm2.Checked = false;
                                    this.checkBox_motion2.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm2.Checked = false;
                                        this.checkBox_motion2.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm2.Checked = false;
                                this.checkBox_motion2.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm3.Checked || this.checkBox_motion3.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm3.Checked = false;
                                    this.checkBox_motion3.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm3.Checked = false;
                                        this.checkBox_motion3.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm3.Checked = false;
                                this.checkBox_motion3.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm4.Checked || this.checkBox_motion4.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm4.Checked = false;
                                    this.checkBox_motion4.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm4.Checked = false;
                                        this.checkBox_motion4.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm4.Checked = false;
                                this.checkBox_motion4.Checked = false;
                            }
                        }
                    }
                }
                if (this.checkBox_alarm5.Checked || this.checkBox_motion5.Checked)
                {
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) != 0)
                                {
                                    this.checkBox_alarm5.Checked = false;
                                    this.checkBox_motion5.Checked = false;
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(start) != 0)
                                {
                                    if (temp1.CompareTo(end) <= 0)
                                    {
                                        this.checkBox_alarm5.Checked = false;
                                        this.checkBox_motion5.Checked = false;
                                    }
                                }
                            }
                            else
                            {
                                this.checkBox_alarm5.Checked = false;
                                this.checkBox_motion5.Checked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                this.checkBox_motion6.Enabled = true;
                this.checkBox_alarm6.Enabled = true;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            _TimeList = new List<Time_Info>();
            Time_Info generalInfo = new Time_Info();
            generalInfo.Type = Em_Type.General;
            RecordTime[] rts = new RecordTime[6];
            for (int i = 0; i < 6;i++)
            {
                if(checkBox_general1.Checked && i == 0)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start1.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start1.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start1.Value.Second;
                    rts[i].EndHour = dateTimePicker_end1.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end1.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end1.Value.Second;
                }
                if (checkBox_general2.Checked && i == 1)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start2.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start2.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start2.Value.Second;
                    rts[i].EndHour = dateTimePicker_end2.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end2.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end2.Value.Second;
                }
                if (checkBox_general3.Checked && i == 2)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start3.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start3.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start3.Value.Second;
                    rts[i].EndHour = dateTimePicker_end3.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end3.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end3.Value.Second;
                }
                if (checkBox_general4.Checked && i == 3)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start4.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start4.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start4.Value.Second;
                    rts[i].EndHour = dateTimePicker_end4.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end4.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end4.Value.Second;
                }
                if (checkBox_general5.Checked && i == 4)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start5.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start5.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start5.Value.Second;
                    rts[i].EndHour = dateTimePicker_end5.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end5.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end5.Value.Second;
                }
                if (checkBox_general6.Checked && i == 5)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start6.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start6.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start6.Value.Second;
                    rts[i].EndHour = dateTimePicker_end6.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end6.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end6.Value.Second;
                }
            }
            generalInfo.Times = rts;

            Time_Info motionInfo = new Time_Info();
            motionInfo.Type = Em_Type.Motion;
            rts = new RecordTime[6];
            for (int i = 0; i < 6; i++)
            {
                if (checkBox_motion1.Checked && i == 0)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start1.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start1.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start1.Value.Second;
                    rts[i].EndHour = dateTimePicker_end1.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end1.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end1.Value.Second;
                }
                if (checkBox_motion2.Checked && i == 1)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start2.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start2.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start2.Value.Second;
                    rts[i].EndHour = dateTimePicker_end2.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end2.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end2.Value.Second;
                }
                if (checkBox_motion3.Checked && i == 2)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start3.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start3.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start3.Value.Second;
                    rts[i].EndHour = dateTimePicker_end3.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end3.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end3.Value.Second;
                }
                if (checkBox_motion4.Checked && i == 3)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start4.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start4.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start4.Value.Second;
                    rts[i].EndHour = dateTimePicker_end4.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end4.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end4.Value.Second;
                }
                if (checkBox_motion5.Checked && i == 4)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start5.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start5.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start5.Value.Second;
                    rts[i].EndHour = dateTimePicker_end5.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end5.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end5.Value.Second;
                }
                if (checkBox_motion6.Checked && i == 5)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start6.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start6.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start6.Value.Second;
                    rts[i].EndHour = dateTimePicker_end6.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end6.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end6.Value.Second;
                }
            }
            motionInfo.Times = rts;

            Time_Info alarmInfo = new Time_Info();
            alarmInfo.Type = Em_Type.Alarm;
            rts = new RecordTime[6];
            for (int i = 0; i < 6; i++)
            {
                if (checkBox_alarm1.Checked && i == 0)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start1.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start1.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start1.Value.Second;
                    rts[i].EndHour = dateTimePicker_end1.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end1.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end1.Value.Second;
                }
                if (checkBox_alarm2.Checked && i == 1)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start2.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start2.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start2.Value.Second;
                    rts[i].EndHour = dateTimePicker_end2.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end2.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end2.Value.Second;
                }
                if (checkBox_alarm3.Checked && i == 2)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start3.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start3.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start3.Value.Second;
                    rts[i].EndHour = dateTimePicker_end3.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end3.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end3.Value.Second;
                }
                if (checkBox_alarm4.Checked && i == 3)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start4.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start4.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start4.Value.Second;
                    rts[i].EndHour = dateTimePicker_end4.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end4.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end4.Value.Second;
                }
                if (checkBox_alarm5.Checked && i == 4)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start5.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start5.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start5.Value.Second;
                    rts[i].EndHour = dateTimePicker_end5.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end5.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end5.Value.Second;
                }
                if (checkBox_alarm6.Checked && i == 5)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start6.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start6.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start6.Value.Second;
                    rts[i].EndHour = dateTimePicker_end6.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end6.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end6.Value.Second;
                }
            }
            alarmInfo.Times = rts;

            Time_Info motionAlarmInfo = new Time_Info();
            motionAlarmInfo.Type = Em_Type.MotionAlarm;
            rts = new RecordTime[6];
            for (int i = 0; i < 6; i++)
            {
                if (checkBox__ma1.Checked && i == 0)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start1.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start1.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start1.Value.Second;
                    rts[i].EndHour = dateTimePicker_end1.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end1.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end1.Value.Second;
                }
                if (checkBox__ma2.Checked && i == 1)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start2.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start2.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start2.Value.Second;
                    rts[i].EndHour = dateTimePicker_end2.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end2.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end2.Value.Second;
                }
                if (checkBox__ma3.Checked && i == 2)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start3.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start3.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start3.Value.Second;
                    rts[i].EndHour = dateTimePicker_end3.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end3.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end3.Value.Second;
                }
                if (checkBox__ma4.Checked && i == 3)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start4.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start4.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start4.Value.Second;
                    rts[i].EndHour = dateTimePicker_end4.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end4.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end4.Value.Second;
                }
                if (checkBox__ma5.Checked && i == 4)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start5.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start5.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start5.Value.Second;
                    rts[i].EndHour = dateTimePicker_end5.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end5.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end5.Value.Second;
                }
                if (checkBox__ma6.Checked && i == 5)
                {
                    rts[i] = new RecordTime();
                    rts[i].BeginHour = dateTimePicker_start6.Value.Hour;
                    rts[i].BeginMinute = dateTimePicker_start6.Value.Minute;
                    rts[i].BeginSecond = dateTimePicker_start6.Value.Second;
                    rts[i].EndHour = dateTimePicker_end6.Value.Hour;
                    rts[i].EndMinute = dateTimePicker_end6.Value.Minute;
                    rts[i].EndSecond = dateTimePicker_end6.Value.Second;
                }
            }
            motionAlarmInfo.Times = rts;

            _TimeList.Add(generalInfo);
            _TimeList.Add(motionInfo);
            _TimeList.Add(alarmInfo);
            _TimeList.Add(motionAlarmInfo);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void checkBox_motion1_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm1();
        }

        private void checkBox_alarm1_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm1();
        }

        private void CheckMotionAndAlarm1()
        {
            if (!_IsLoaded)
            {
                return;
            }
            if(checkBox_motion1.Checked || checkBox_alarm1.Checked)
            {
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start1.Value.Hour,
                    BeginMinute = dateTimePicker_start1.Value.Minute,
                    BeginSecond = dateTimePicker_start1.Value.Second,
                    EndHour = dateTimePicker_end1.Value.Hour,
                    EndMinute = dateTimePicker_end1.Value.Minute,
                    EndSecond = dateTimePicker_end1.Value.Second
                };

                if(checkBox__ma2.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma3.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma4.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma5.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma6.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                }

            }
        }

        private void checkBox_motion2_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm2();
        }

        private void checkBox_alarm2_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm2();
        }

        private void CheckMotionAndAlarm2()
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (checkBox_motion2.Checked || checkBox_alarm2.Checked)
            {
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start2.Value.Hour,
                    BeginMinute = dateTimePicker_start2.Value.Minute,
                    BeginSecond = dateTimePicker_start2.Value.Second,
                    EndHour = dateTimePicker_end2.Value.Hour,
                    EndMinute = dateTimePicker_end2.Value.Minute,
                    EndSecond = dateTimePicker_end2.Value.Second
                };

                if (checkBox__ma1.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma3.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma4.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma5.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion1.Checked = false;
                                    checkBox_alarm1.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion1.Checked = false;
                                checkBox_alarm1.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma6.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion2.Checked = false;
                                    checkBox_alarm2.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion2.Checked = false;
                                checkBox_alarm2.Checked = false;
                            }
                        }
                    }
                }

            }
        }

        private void checkBox_motion3_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm3();
        }

        private void checkBox_alarm3_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm3();
        }

        private void CheckMotionAndAlarm3()
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (checkBox_motion3.Checked || checkBox_alarm3.Checked)
            {
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start3.Value.Hour,
                    BeginMinute = dateTimePicker_start3.Value.Minute,
                    BeginSecond = dateTimePicker_start3.Value.Second,
                    EndHour = dateTimePicker_end3.Value.Hour,
                    EndMinute = dateTimePicker_end3.Value.Minute,
                    EndSecond = dateTimePicker_end3.Value.Second
                };

                if (checkBox__ma1.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma2.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma4.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma5.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma6.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion3.Checked = false;
                                    checkBox_alarm3.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion3.Checked = false;
                                checkBox_alarm3.Checked = false;
                            }
                        }
                    }
                }

            }
        }

        private void checkBox_motion4_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm4();
        }

        private void checkBox_alarm4_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm4();
        }

        private void CheckMotionAndAlarm4()
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (checkBox_motion4.Checked || checkBox_alarm4.Checked)
            {
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start4.Value.Hour,
                    BeginMinute = dateTimePicker_start4.Value.Minute,
                    BeginSecond = dateTimePicker_start4.Value.Second,
                    EndHour = dateTimePicker_end4.Value.Hour,
                    EndMinute = dateTimePicker_end4.Value.Minute,
                    EndSecond = dateTimePicker_end4.Value.Second
                };

                if (checkBox__ma1.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma2.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma3.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma5.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma6.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion4.Checked = false;
                                    checkBox_alarm4.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion4.Checked = false;
                                checkBox_alarm4.Checked = false;
                            }
                        }
                    }
                }

            }
        }

        private void checkBox_motion5_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm5();
        }

        private void checkBox_alarm5_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm5();
        }

        private void CheckMotionAndAlarm5()
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (checkBox_motion5.Checked || checkBox_alarm5.Checked)
            {
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start5.Value.Hour,
                    BeginMinute = dateTimePicker_start5.Value.Minute,
                    BeginSecond = dateTimePicker_start5.Value.Second,
                    EndHour = dateTimePicker_end5.Value.Hour,
                    EndMinute = dateTimePicker_end5.Value.Minute,
                    EndSecond = dateTimePicker_end5.Value.Second
                };

                if (checkBox__ma1.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma2.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma3.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma4.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma6.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start6.Value.Hour,
                        BeginMinute = dateTimePicker_start6.Value.Minute,
                        BeginSecond = dateTimePicker_start6.Value.Second,
                        EndHour = dateTimePicker_end6.Value.Hour,
                        EndMinute = dateTimePicker_end6.Value.Minute,
                        EndSecond = dateTimePicker_end6.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion5.Checked = false;
                                    checkBox_alarm5.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion5.Checked = false;
                                checkBox_alarm5.Checked = false;
                            }
                        }
                    }
                }

            }
        }

        private void checkBox_motion6_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm6();
        }

        private void checkBox_alarm6_CheckedChanged(object sender, EventArgs e)
        {
            CheckMotionAndAlarm6();
        }

        private void CheckMotionAndAlarm6()
        {
            if (!_IsLoaded)
            {
                return;
            }
            if (checkBox_motion6.Checked || checkBox_alarm6.Checked)
            {
                DateTime start = new DateTime(2000, 1, 1, dateTimePicker_start6.Value.Hour, dateTimePicker_start6.Value.Minute, dateTimePicker_start6.Value.Second);
                DateTime end = new DateTime(2000, 1, 1, dateTimePicker_end6.Value.Hour, dateTimePicker_end6.Value.Minute, dateTimePicker_end6.Value.Second);
                RecordTime rt = new RecordTime()
                {
                    BeginHour = dateTimePicker_start6.Value.Hour,
                    BeginMinute = dateTimePicker_start6.Value.Minute,
                    BeginSecond = dateTimePicker_start6.Value.Second,
                    EndHour = dateTimePicker_end6.Value.Hour,
                    EndMinute = dateTimePicker_end6.Value.Minute,
                    EndSecond = dateTimePicker_end6.Value.Second
                };

                if (checkBox__ma1.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start1.Value.Hour, dateTimePicker_start1.Value.Minute, dateTimePicker_start1.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end1.Value.Hour, dateTimePicker_end1.Value.Minute, dateTimePicker_end1.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start1.Value.Hour,
                        BeginMinute = dateTimePicker_start1.Value.Minute,
                        BeginSecond = dateTimePicker_start1.Value.Second,
                        EndHour = dateTimePicker_end1.Value.Hour,
                        EndMinute = dateTimePicker_end1.Value.Minute,
                        EndSecond = dateTimePicker_end1.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma2.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start2.Value.Hour, dateTimePicker_start2.Value.Minute, dateTimePicker_start2.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end2.Value.Hour, dateTimePicker_end2.Value.Minute, dateTimePicker_end2.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start2.Value.Hour,
                        BeginMinute = dateTimePicker_start2.Value.Minute,
                        BeginSecond = dateTimePicker_start2.Value.Second,
                        EndHour = dateTimePicker_end2.Value.Hour,
                        EndMinute = dateTimePicker_end2.Value.Minute,
                        EndSecond = dateTimePicker_end2.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma3.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start3.Value.Hour, dateTimePicker_start3.Value.Minute, dateTimePicker_start3.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end3.Value.Hour, dateTimePicker_end3.Value.Minute, dateTimePicker_end3.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start3.Value.Hour,
                        BeginMinute = dateTimePicker_start3.Value.Minute,
                        BeginSecond = dateTimePicker_start3.Value.Second,
                        EndHour = dateTimePicker_end3.Value.Hour,
                        EndMinute = dateTimePicker_end3.Value.Minute,
                        EndSecond = dateTimePicker_end3.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma4.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start4.Value.Hour, dateTimePicker_start4.Value.Minute, dateTimePicker_start4.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end4.Value.Hour, dateTimePicker_end4.Value.Minute, dateTimePicker_end4.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start4.Value.Hour,
                        BeginMinute = dateTimePicker_start4.Value.Minute,
                        BeginSecond = dateTimePicker_start4.Value.Second,
                        EndHour = dateTimePicker_end4.Value.Hour,
                        EndMinute = dateTimePicker_end4.Value.Minute,
                        EndSecond = dateTimePicker_end4.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) <= 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                }
                if (checkBox__ma5.Checked)
                {
                    DateTime temp = new DateTime(2000, 1, 1, dateTimePicker_start5.Value.Hour, dateTimePicker_start5.Value.Minute, dateTimePicker_start5.Value.Second);
                    DateTime temp1 = new DateTime(2000, 1, 1, dateTimePicker_end5.Value.Hour, dateTimePicker_end5.Value.Minute, dateTimePicker_end5.Value.Second);
                    RecordTime rt1 = new RecordTime()
                    {
                        BeginHour = dateTimePicker_start5.Value.Hour,
                        BeginMinute = dateTimePicker_start5.Value.Minute,
                        BeginSecond = dateTimePicker_start5.Value.Second,
                        EndHour = dateTimePicker_end5.Value.Hour,
                        EndMinute = dateTimePicker_end5.Value.Minute,
                        EndSecond = dateTimePicker_end5.Value.Second
                    };
                    if (start.CompareTo(temp) <= 0)
                    {
                        if (TimeProgressBar.Contains(rt, rt1) == 0 || TimeProgressBar.Contains(rt, rt1) == 1)
                        {
                            if (TimeProgressBar.Contains(rt, rt1) == 0)
                            {
                                if (end.CompareTo(temp) > 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        if (TimeProgressBar.Contains(rt1, rt) == 0 || TimeProgressBar.Contains(rt1, rt) == 1)
                        {
                            if (TimeProgressBar.Contains(rt1, rt) == 0)
                            {
                                if (temp1.CompareTo(end) != 0)
                                {
                                    checkBox_motion6.Checked = false;
                                    checkBox_alarm6.Checked = false;
                                }
                            }
                            else
                            {
                                checkBox_motion6.Checked = false;
                                checkBox_alarm6.Checked = false;
                            }
                        }
                    }
                }

            }
        }

    }
}
