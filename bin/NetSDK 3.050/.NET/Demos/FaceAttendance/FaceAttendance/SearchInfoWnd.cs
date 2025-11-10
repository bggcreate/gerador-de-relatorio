using NetSDKCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace FaceAttendance
{
    public partial class SearchInfoWnd : Form
    {
        private IntPtr loginID = IntPtr.Zero;
        private IntPtr findID = IntPtr.Zero;
        private NET_FIND_RECORD_ACCESSCTLCARDREC_CONDITION_EX condition;
        public SearchInfoWnd(IntPtr _loginDI, NET_FIND_RECORD_ACCESSCTLCARDREC_CONDITION_EX find_condition)
        {
            loginID = _loginDI;
            condition = find_condition;
            InitializeComponent();
            this.Load += ShowSearchInfo;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (findID != IntPtr.Zero)
            {
                NETClient.FindRecordClose(findID);
                findID = IntPtr.Zero;
            }
            base.OnClosed(e);
        }

        private void ShowSearchInfo(object sender, EventArgs e)
        {
            searchInfo_listView.Items.Clear();

            object obj = condition;
            
            bool ret = NETClient.FindRecord(loginID, EM_NET_RECORD_TYPE.ACCESSCTLCARDREC_EX, obj, typeof(NET_FIND_RECORD_ACCESSCTLCARDREC_CONDITION_EX), ref findID, 10000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }
            Task task = new Task(new Action(() =>
            {
                while (true)
                {
                    int max = 20;
                    int retNum = 0;
                    List<object> ls = new List<object>();
                    for (int i = 0; i < max; i++)
                    {
                        NET_RECORDSET_ACCESS_CTL_CARDREC cardrec = new NET_RECORDSET_ACCESS_CTL_CARDREC();
                        cardrec.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARDREC));
                        ls.Add(cardrec);
                    }
                    NETClient.FindNextRecord(findID, max, ref retNum, ref ls, typeof(NET_RECORDSET_ACCESS_CTL_CARDREC), 10000);
                    BeginInvoke(new Action(() =>
                    {
                        foreach (var item in ls)
                        {
                            NET_RECORDSET_ACCESS_CTL_CARDREC info = (NET_RECORDSET_ACCESS_CTL_CARDREC)item;
                            var listitem = new ListViewItem();
                            listitem.Text = info.nRecNo.ToString();
                            listitem.SubItems.Add(info.szUserID);
                            listitem.SubItems.Add(info.szCardName);
                            listitem.SubItems.Add(info.stuTime.ToString());
                            listitem.SubItems.Add(GetOpenMethod(info.emMethod));
                            listitem.SubItems.Add(GetAttendanceState(info.emAttendanceState));
                            if (searchInfo_listView != null)
                            {
                                searchInfo_listView.BeginUpdate();
                                searchInfo_listView.Items.Add(listitem);
                                searchInfo_listView.EndUpdate(); 
                            }
                        }
                    }));
                    Thread.Sleep(10);
                    if (retNum < max)
                    {
                        break;
                    }
                }
            }));
            task.Start();
        }

        private string GetOpenMethod(EM_ACCESS_DOOROPEN_METHOD method)
        {
            string res = "UnKnown(未知)";
            switch (method)
            {
                case EM_ACCESS_DOOROPEN_METHOD.UNKNOWN:
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.PWD_ONLY:
                    res = "Pwd_Only(密码)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.CARD:
                    res = "Card(刷卡)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.CARD_FIRST:
                    res = "Card_First(先刷卡后密码)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.PWD_FIRST:
                    res = "Pwd_First(先密码后刷卡)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.REMOTE:
                    res = "Remote(远程)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.BUTTON:
                    res = "Button(按钮)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.FINGERPRINT:
                    res = "Fingerprint(指纹)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.PWD_CARD_FINGERPRINT:
                    res = "Pwd_Card_Fingerprint(密码+刷卡+指纹)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.PWD_FINGERPRINT:
                    res = "Pwd_Fingerprint(密码+指纹)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.CARD_FINGERPRINT:
                    res = "Card_Fingerprint(刷卡+指纹)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.PERSONS:
                    res = "Persons(多人开锁)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.KEY:
                    res = "Key(钥匙开门)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.COERCE_PWD:
                    res = "Coerce_Pwd(胁迫密码开门)";
                    break;
                case EM_ACCESS_DOOROPEN_METHOD.FACE_RECOGNITION:
                    res = "Face_Recognition(人脸识别)";
                    break;
                default:
                    break;
            }
            return res;
        }

        private string GetAttendanceState(EM_ATTENDANCESTATE state)
        {
            string res = "UnKnown(未知)";
            switch (state)
            {
                case EM_ATTENDANCESTATE.UNKNOWN:
                    break;
                case EM_ATTENDANCESTATE.SIGNIN:
                    res = "Signin(签入)";
                    break;
                case EM_ATTENDANCESTATE.GOOUT:
                    res = "Goout(外出)";
                    break;
                case EM_ATTENDANCESTATE.GOOUT_AND_RETRUN:
                    res = "Goout_And_Return(外出归来)";
                    break;
                case EM_ATTENDANCESTATE.SIGNOUT:
                    res = "Signout(签出)";
                    break;
                case EM_ATTENDANCESTATE.WORK_OVERTIME_SIGNIN:
                    res = "Work_Overtime_Signin(加班签到)";
                    break;
                case EM_ATTENDANCESTATE.WORK_OVERTIME_SIGNOUT:
                    res = "Work_Overtime_Signout(加班签出)";
                    break;
                default:
                    break;
            }
            return res;
        }
    }
}
