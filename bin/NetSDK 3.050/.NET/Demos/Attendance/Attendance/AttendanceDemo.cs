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
using System.Text.RegularExpressions;

namespace Attendance
{
    public partial class AttendanceDemo : Form
    {
        IntPtr _LoginID = IntPtr.Zero;
        NET_DEVICEINFO_Ex _DeviceInfo = new NET_DEVICEINFO_Ex();
        fAnalyzerDataCallBack _AnalyzerDataCallBack;
        IntPtr _AttachID = IntPtr.Zero;
        const int Lines = 20;
        const int EventLines = 50;
        int _EventID = 0;
        int _InformationID = 0;
        int _CurrentCount = 0;
        int _TotalCount = 0;
        int _LastCount = 0;
        string _OldUserID = "";

        public AttendanceDemo()
        {
            InitializeComponent();
            try
            {
                NETClient.Init(null, IntPtr.Zero, null);
                _AnalyzerDataCallBack = new fAnalyzerDataCallBack(AnalyzerDataCallBack);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
                return;
            }
            this.Load += new EventHandler(AttendanceDemo_Load);
        }

        void AttendanceDemo_Load(object sender, EventArgs e)
        {
            textBox_userid.Enabled = false;
            button_search.Enabled = false;
            button_add.Enabled = false;
            button_modify.Enabled = false;
            button_delete.Enabled = false;
            button_operatebyfingerprintid.Enabled = false;
            button_attach.Enabled = false;
            button_prepage.Enabled = false;
            button_nextpage.Enabled = false;
            button_operatebyuserid.Enabled = false;         
        }

        protected override void OnShown(EventArgs e)
        {
            this.listView_information.Columns[0].Width = 61;
            this.listView_event.Columns[0].Width = 61;
            base.OnShown(e);
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == _LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(this.textBox_port.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("The port value must be 1 -65535(端口值必须是1-65535)");
                    return;
                }
                _LoginID = NETClient.Login(this.textBox_ip.Text.Trim(), port, this.textBox_name.Text.Trim(), this.textBox_password.Text, EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref _DeviceInfo);
                if (IntPtr.Zero == _LoginID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                textBox_userid.Enabled = true;
                button_search.Enabled = true;
                button_add.Enabled = true;
                button_modify.Enabled = true;
                button_delete.Enabled = true;
                button_operatebyfingerprintid.Enabled = true;
                button_operatebyuserid.Enabled = true;
                this.button_attach.Enabled = true;
                this.button_login.Text = "Logout(登出)";
            }
            else
            {
                NETClient.Logout(_LoginID);
                _LoginID = IntPtr.Zero;
                this.button_login.Text = "Login(登录)";
                this.button_attach.Enabled = false;
                this.button_attach.Text = "Attach(订阅)";
                _AttachID = IntPtr.Zero;
                textBox_userid.Text = "";
                textBox_userid.Enabled = false;
                button_search.Enabled = false;
                button_add.Enabled = false;
                button_modify.Enabled = false;
                button_delete.Enabled = false;
                button_operatebyfingerprintid.Enabled = false;
                button_operatebyuserid.Enabled = false;
                button_prepage.Enabled = false;
                button_nextpage.Enabled = false;
                this.listView_event.Items.Clear();
                this.listView_information.Items.Clear();
                _EventID = 0;
                _InformationID = 0;
            }
        }

        private void textBox_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button_attach_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == _AttachID)
            {
                _AttachID = NETClient.RealLoadPicture(_LoginID, -1, (uint)EM_EVENT_IVS_TYPE.ALL, true, _AnalyzerDataCallBack, IntPtr.Zero, IntPtr.Zero);
                if (IntPtr.Zero == _AttachID)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                this.button_attach.Text = "Detach(取消订阅)";
            }
            else
            {
                NETClient.StopLoadPic(_AttachID);
                _AttachID = IntPtr.Zero;
                this.button_attach.Text = "Attach(订阅)";
                this.listView_event.Items.Clear();
                _EventID = 0;
            }
        }

        private int AnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            EM_EVENT_IVS_TYPE type = (EM_EVENT_IVS_TYPE)dwEventType;
            switch (type)
            {
                case EM_EVENT_IVS_TYPE.ACCESS_CTL:
                    NET_DEV_EVENT_ACCESS_CTL_INFO info = (NET_DEV_EVENT_ACCESS_CTL_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_ACCESS_CTL_INFO));
                    this.BeginInvoke(new Action(() =>
                    {
                        _EventID++;
                        ListViewItem item = new ListViewItem();
                        item.Text = _EventID.ToString();
                        item.SubItems.Add(info.szUserID);
                        item.SubItems.Add(info.szCardNo);
                        item.SubItems.Add(info.UTC.ToShortString());
                        switch (info.emOpenMethod)
                        {
                            case EM_ACCESS_DOOROPEN_METHOD.CARD:
                                item.SubItems.Add("Card(卡)");
                                break;
                            case EM_ACCESS_DOOROPEN_METHOD.FINGERPRINT:
                                item.SubItems.Add("FingerPrint(指纹)");
                                break;
                            default:
                                item.SubItems.Add("Unknown(未知)");
                                break;
                        }

                        listView_event.BeginUpdate();
                        listView_event.Items.Insert(0, item);
                        if (listView_event.Items.Count > EventLines)
                        {
                            listView_event.Items.RemoveAt(EventLines);
                        }
                        listView_event.EndUpdate();
                        
                    }));
                    break;
                default:
                    break;
            }
            return 0;
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            listView_information.Items.Clear();
            _InformationID = 0;
            if (this.textBox_userid.Text == "")
            {
                _CurrentCount = 0;
                _LastCount = 0;
                this.button_nextpage.Enabled = false;
                this.button_prepage.Enabled = false;
                FindUser(0);
            }
            else
            {
                this.button_prepage.Enabled = false;
                this.button_nextpage.Enabled = false;
                _InformationID ++;
                NET_IN_ATTENDANCE_GetUSER inParam = new NET_IN_ATTENDANCE_GetUSER();
                NET_OUT_ATTENDANCE_GetUSER outParam = new NET_OUT_ATTENDANCE_GetUSER();
                try
                {
                    inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ATTENDANCE_GetUSER));
                    inParam.szUserID = this.textBox_userid.Text.Trim();
                    outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ATTENDANCE_GetUSER));
                    outParam.nMaxLength = 1024;
                    outParam.pbyPhotoData = Marshal.AllocHGlobal(1024);

                    bool result = NETClient.Attendance_GetUser(_LoginID, inParam, ref outParam, 3000);
                    if (result)
                    {
                        ListViewItem item = new ListViewItem();

                        item.Text = _InformationID.ToString();
                        item.SubItems.Add(outParam.stuUserInfo.szUserID);
                        item.SubItems.Add(outParam.stuUserInfo.szUserName);
                        item.SubItems.Add(outParam.stuUserInfo.szCardNo);
                        item.Tag = outParam.stuUserInfo;

                        listView_information.BeginUpdate();
                        listView_information.Items.Add(item);
                        listView_information.EndUpdate();
                    }
                    else
                    {
                        MessageBox.Show("Get User information failed(获取用户信息失败)");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Marshal.FreeHGlobal(outParam.pbyPhotoData);
                }
            }
        }

        private int FindUser(int start)
        {
            NET_IN_ATTENDANCE_FINDUSER findUser = new NET_IN_ATTENDANCE_FINDUSER
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ATTENDANCE_FINDUSER))
            };

            findUser.nOffset = start;
            findUser.nPagedQueryCount = Lines;

            NET_OUT_ATTENDANCE_FINDUSER outInfo = new NET_OUT_ATTENDANCE_FINDUSER()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ATTENDANCE_FINDUSER)),
                nMaxUserCount = Lines,
            };
            try
            {
                outInfo.stuUserInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_ATTENDANCE_USERINFO)) * outInfo.nMaxUserCount);
                outInfo.nMaxPhotoDataLength = 1024;
                outInfo.pbyPhotoData = Marshal.AllocHGlobal(outInfo.nRetPhoteLength);

                bool result = NETClient.Attendance_FindUser(_LoginID, findUser, ref outInfo, 10000);
                if (result)
                {
                    _TotalCount = outInfo.nTotalUser;
                    this.listView_information.Items.Clear();
                    for (int i = 0; i < outInfo.nRetUserCount; ++i)
                    {
                        _InformationID++;
                        NET_ATTENDANCE_USERINFO info = (NET_ATTENDANCE_USERINFO)Marshal.PtrToStructure(outInfo.stuUserInfo + i * Marshal.SizeOf(typeof(NET_ATTENDANCE_USERINFO)), typeof(NET_ATTENDANCE_USERINFO));
                        ListViewItem item = new ListViewItem();
                        item.Text = _InformationID.ToString();
                        item.SubItems.Add(info.szUserID);
                        item.SubItems.Add(info.szUserName);
                        item.SubItems.Add(info.szCardNo);
                        item.Tag = info;

                        listView_information.BeginUpdate();
                        listView_information.Items.Add(item);
                        listView_information.EndUpdate();
                        
                    }
                    if (outInfo.nTotalUser > Lines)
                    {
                        this.button_nextpage.Enabled = true;
                    }
                    else
                    {
                        this.button_nextpage.Enabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("Get User information failed(获取用户信息失败)");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
            finally
            {
                Marshal.FreeHGlobal(outInfo.stuUserInfo);
                Marshal.FreeHGlobal(outInfo.pbyPhotoData);
            }

            return outInfo.nRetUserCount;
        }

        private void button_nextpage_Click(object sender, EventArgs e)
        {
            _CurrentCount += Lines;
            int count = FindUser(_CurrentCount);
            if (_CurrentCount + count >= _TotalCount)
            {
                this.button_nextpage.Enabled = false;
                _LastCount = count;
            }
            if (_CurrentCount == Lines)
            {
                this.button_prepage.Enabled = true;
            }
        }

        private void button_prepage_Click(object sender, EventArgs e)
        {
            _CurrentCount -= Lines;
            if (_InformationID == _TotalCount)
            {
                _InformationID = _InformationID - _LastCount - Lines;
                _LastCount = 0;
            }
            else
            {
                _InformationID -= 2 * Lines;
            }
            int count = FindUser(_CurrentCount);
            if (_CurrentCount - Lines < 0)
            {
                this.button_prepage.Enabled = false;
            }
            if (_CurrentCount + Lines <= _TotalCount)
            {
                this.button_nextpage.Enabled = true;
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            UserInfomation ui = new UserInfomation(Operate_Type.Add, new NET_ATTENDANCE_USERINFO());
            var ret = ui.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    NET_IN_ATTENDANCE_ADDUSER InaddUser = new NET_IN_ATTENDANCE_ADDUSER();
                    NET_OUT_ATTENDANCE_ADDUSER OutaddUser = new NET_OUT_ATTENDANCE_ADDUSER();
                    InaddUser.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ATTENDANCE_ADDUSER));
                    OutaddUser.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ATTENDANCE_ADDUSER));

                    InaddUser.stuUserInfo.szCardNo = ui.Data.szCardNo;
                    InaddUser.stuUserInfo.szUserName = ui.Data.szUserName;
                    InaddUser.stuUserInfo.szUserID = ui.Data.szUserID;

                    bool result = NETClient.Attendance_AddUser(_LoginID, InaddUser, ref OutaddUser, 3000);
                    if (!result)
                    {
                        MessageBox.Show("Add Failed(增加失败)");
                    }
                    else
                    {
                        MessageBox.Show("Add Successfully(增加成功)");
                    }
                }
                catch (Exception ex)
                {
                    ui.Dispose();
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            ui.Dispose();
        }

        private void button_modify_Click(object sender, EventArgs e)
        {
            if (listView_information.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one user to modify(请选择一个用户去修改)");
                return;
            }
            UserInfomation ui = new UserInfomation(Operate_Type.Modify, (NET_ATTENDANCE_USERINFO)listView_information.SelectedItems[0].Tag);
            var ret = ui.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    NET_IN_ATTENDANCE_ModifyUSER InModifyUser = new NET_IN_ATTENDANCE_ModifyUSER();
                    NET_OUT_ATTENDANCE_ModifyUSER OutModifyUser = new NET_OUT_ATTENDANCE_ModifyUSER();
                    InModifyUser.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ATTENDANCE_ModifyUSER));
                    OutModifyUser.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ATTENDANCE_ModifyUSER));

                    InModifyUser.stuUserInfo.szCardNo = ui.Data.szCardNo;
                    InModifyUser.stuUserInfo.szUserID = ui.Data.szUserID;
                    InModifyUser.stuUserInfo.szUserName = ui.Data.szUserName;

                    bool result = NETClient.Attendance_ModifyUser(_LoginID, InModifyUser, ref OutModifyUser, 3000);
                    if (result)
                    {
                        MessageBox.Show("Modify Successfully(修改成功)");
                    }
                    else
                    {
                        MessageBox.Show("Modify Failed(修改失败)");
                    }
                }
                catch (Exception ex)
                {
                    ui.Dispose();
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            ui.Dispose();
            this.listView_information.Focus();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (listView_information.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one user to delete(请选择一个用户去删除)");
                return;
            }
            NET_ATTENDANCE_USERINFO ud = (NET_ATTENDANCE_USERINFO)listView_information.SelectedItems[0].Tag;
            NET_CTRL_IN_FINGERPRINT_REMOVE_BY_USERID inRemoveByUserID = new NET_CTRL_IN_FINGERPRINT_REMOVE_BY_USERID();
            inRemoveByUserID.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_IN_FINGERPRINT_REMOVE_BY_USERID));
            inRemoveByUserID.szUserID = ud.szUserID;
            NET_CTRL_OUT_FINGERPRINT_REMOVE_BY_USERID outRemoveByUserID = new NET_CTRL_OUT_FINGERPRINT_REMOVE_BY_USERID();
            outRemoveByUserID.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_OUT_FINGERPRINT_REMOVE_BY_USERID));
            // delete fingerprint
            NETClient.Attendance_RemoveFingerByUserID(_LoginID, ref inRemoveByUserID, ref outRemoveByUserID, 5000);

            NET_IN_ATTENDANCE_DELUSER InDelUser = new NET_IN_ATTENDANCE_DELUSER();
            NET_OUT_ATTENDANCE_DELUSER OutDelUser = new NET_OUT_ATTENDANCE_DELUSER();
            InDelUser.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ATTENDANCE_DELUSER));
            OutDelUser.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ATTENDANCE_DELUSER));
            InDelUser.szUserID = ud.szUserID;
            // delete user
            bool result = NETClient.Attendance_DelUser(_LoginID, InDelUser, ref OutDelUser, 3000);
            if (result)
            {
                MessageBox.Show("Delete Successfully(删除成功)");
            }
            else
            {
                MessageBox.Show("Delete Failed(删除失败)");
            }
            this.listView_information.Focus();
        }

        private void button_operatebyuserid_Click(object sender, EventArgs e)
        {
            if (listView_information.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one user to operate fingerprint(请选择一个用户去操作指纹)");
                return;
            }
            NET_ATTENDANCE_USERINFO ud = (NET_ATTENDANCE_USERINFO)listView_information.SelectedItems[0].Tag;
            OperateFingerprintByUserID ofbu = new OperateFingerprintByUserID(_LoginID, ud.szUserID);
            ofbu.ShowDialog();
            ofbu.Dispose();
            this.listView_information.Focus();
        }

        private void button_operatebyfingerprintid_Click(object sender, EventArgs e)
        {
            OperateFingerprintByFPID ofbf = new OperateFingerprintByFPID(_LoginID);
            ofbf.ShowDialog();
            ofbf.Dispose();
        }

        private void textBox_userid_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length > 31)
            {
                ((TextBox)sender).Text = _OldUserID;
            }
            _OldUserID = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

        
    }

}
