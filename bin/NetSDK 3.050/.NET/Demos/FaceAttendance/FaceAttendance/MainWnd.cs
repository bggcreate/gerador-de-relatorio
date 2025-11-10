using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetSDKCS;
using System.Threading;

namespace FaceAttendance
{
    public partial class MainWnd : Form
    {
        IntPtr loginID = IntPtr.Zero;
        IntPtr realLoadID = IntPtr.Zero;
        IntPtr findID = IntPtr.Zero;
        fDisConnectCallBack disConnectCallBack;
        fHaveReConnectCallBack haveReConnectCallBack;
        fAnalyzerDataCallBack analyzerDataCallBack;
        NET_DEVICEINFO_Ex device;
        private int pic_index = 0;
        string _GetCardNo = "";
        string _SearchCardNo = "";

        public MainWnd()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (findID != IntPtr.Zero)
            {
                NETClient.FindRecordClose(findID);
                findID = IntPtr.Zero;
            }
            if (realLoadID != IntPtr.Zero)
            {
                NETClient.StopLoadPic(realLoadID);
                realLoadID = IntPtr.Zero;
            }
            if (loginID != IntPtr.Zero)
            {
                NETClient.Logout(loginID);
                loginID = IntPtr.Zero;
            }
            NETClient.Cleanup();
            base.OnClosed(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            disConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
            haveReConnectCallBack = new fHaveReConnectCallBack(ReConnectCallBack);
            analyzerDataCallBack = new fAnalyzerDataCallBack(AnalyzerDataCallBack);
            try
            {
                NETClient.Init(disConnectCallBack, IntPtr.Zero, null);
                NETClient.SetAutoReconnect(haveReConnectCallBack, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            OfflineUpdateUI();
        }

        private int AnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            if(realLoadID != lAnalyzerHandle)
            {
                return 0;
            }
            switch (dwEventType)
            {
                case (uint)EM_EVENT_IVS_TYPE.ACCESS_CTL:
                    NET_DEV_EVENT_ACCESS_CTL_INFO info = (NET_DEV_EVENT_ACCESS_CTL_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_DEV_EVENT_ACCESS_CTL_INFO));
                    this.BeginInvoke(new Action(() =>
                    {
                        var list_item = new ListViewItem();
                        list_item.Text = info.UTC.ToString();
                        list_item.SubItems.Add(info.szUserID);
                        list_item.SubItems.Add(info.szCardName);
                        list_item.SubItems.Add(GetOpenMethod(info.emOpenMethod));
                        list_item.SubItems.Add(GetAttendanceState(info.emAttendanceState));

                        find_listView.BeginUpdate();
                        find_listView.Items.Add(list_item);
                        find_listView.EndUpdate();
                    }));
                    if(IntPtr.Zero != pBuffer && dwBufSize > 0)
                    {
                        byte[] pic = new byte[dwBufSize];
                        Marshal.Copy(pBuffer, pic, 0, (int)dwBufSize);
                        using(MemoryStream stream = new MemoryStream(pic))
                        {
                            try
                            {
                                Image image = Image.FromStream(stream);
                                realload_pictureBox.Image = image;
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }

                    break;
                default:
                    break;
            }

            return 0;
        }

        private void ReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke((Action)UpdateReConnectedUI);
        }

        private void UpdateReConnectedUI()
        {
            this.Text = "FaceAttendance(人脸考勤机)--OnLine(在线)";
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            this.BeginInvoke((Action)UpdateDisConnectedUI);
        }

        private void UpdateDisConnectedUI()
        {
            this.Text = "FaceAttendance(人脸考勤机)--OffLine(离线)";
        }

        private void OnlineUpdateUI()
        {
            this.Text = "FaceAttendance(人脸考勤机)--OnLine(在线)";
            LoginDevice.Text = "Logout(登出)";

            Find_button.Enabled = true;
            Add_button.Enabled = true;
            Remove_button.Enabled = true;
            Update_button.Enabled = true;
            Search_button.Enabled = true;
            OpenDoor.Enabled = true;
            Snap_button.Enabled = true;
            RealLoad.Enabled = true;
            card_search_radioButton.Enabled = true;
            time_search_radioButton.Enabled = true;
        }

        private void OfflineUpdateUI()
        {
            this.Text = "FaceAttendance(人脸考勤机)--OffLine(离线)";
            LoginDevice.Text = "Login(登陆)";

            Find_button.Enabled = false;
            Add_button.Enabled = false;
            Remove_button.Enabled = false;
            Update_button.Enabled = false;
            Search_button.Enabled = false;
            OpenDoor.Enabled = false;
            Snap_button.Enabled = false;
            RealLoad.Enabled = false;
            card_search_radioButton.Enabled = false;
            time_search_radioButton.Enabled = false;
            cardno_textBox.Text = "";
            RealLoad.Text = "Attach(订阅)";
        }

        private void LoginDevice_Click(object sender, EventArgs e)
        {
            if (IntPtr.Zero == loginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(port_textBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Input port error(输入端口有误)");
                    return;
                }
                device = new NET_DEVICEINFO_Ex();

                loginID = NETClient.Login(ip_textBox.Text.Trim(), port, user_textBox.Text.Trim(), pwd_textBox.Text.Trim(), EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref device);
                if (IntPtr.Zero == loginID)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                OnlineUpdateUI();
            }
            else
            {
                if (findID != IntPtr.Zero)
                {
                    NETClient.FindRecordClose(findID);
                    findID = IntPtr.Zero;
                }
                if (realLoadID != IntPtr.Zero)
                {
                    NETClient.StopLoadPic(realLoadID);
                    realLoadID = IntPtr.Zero;
                }
                bool result = NETClient.Logout(loginID);
                if (!result)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                loginID = IntPtr.Zero;
                search_listView.Items.Clear();
                find_listView.Items.Clear();
                snap_pictureBox.Image = null;
                snap_pictureBox.Refresh();
                realload_pictureBox.Image = null;
                realload_pictureBox.Refresh();
                OfflineUpdateUI();
            }
        }

        private void RealLoad_Click(object sender, EventArgs e)
        {
            if(realLoadID == IntPtr.Zero)
            {
                realLoadID = NETClient.RealLoadPicture(loginID, 0, (uint)EM_EVENT_IVS_TYPE.ACCESS_CTL, true, analyzerDataCallBack, IntPtr.Zero, IntPtr.Zero);
                if (realLoadID == IntPtr.Zero)
                {
                    MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                RealLoad.Text = "Detach(停止订阅)";
            }
            else
            {
                if (NETClient.StopLoadPic(realLoadID))
                {
                    realLoadID = IntPtr.Zero;
                    RealLoad.Text = "Attach(订阅)";
                    realload_pictureBox.Image = null;
                    realload_pictureBox.Refresh();
                    find_listView.Items.Clear();
                }
                else 
                {
                    MessageBox.Show("Detach fail(取消订阅失败)");
                }
            }
        }

        private void Snap_button_Click(object sender, EventArgs e)
        {
            NET_IN_SNAP_PIC_TO_FILE_PARAM inParam = new NET_IN_SNAP_PIC_TO_FILE_PARAM()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_SNAP_PIC_TO_FILE_PARAM)),
                stuParam = new NET_SNAP_PARAMS()
                {
                    Channel = 0,
                    Quality = 2,
                    mode = 0
                },
                szFilePath = string.Format("./{0}_image.jpg", pic_index)
            };
            NET_OUT_SNAP_PIC_TO_FILE_PARAM outParam = new NET_OUT_SNAP_PIC_TO_FILE_PARAM()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_SNAP_PIC_TO_FILE_PARAM)),
                dwPicBufLen = 1024000,
                szPicBuf = Marshal.AllocHGlobal(1024000),
            };

            bool result = NETClient.SnapPictureToFile(loginID, ref inParam, ref outParam, 3000);
            if (!result)
            {
                MessageBox.Show("Snap picture failed(抓图失败)");
                return;
            }
            byte[] pic = new byte[outParam.dwPicBufRetLen];
            Marshal.Copy(outParam.szPicBuf, pic, 0, (int)outParam.dwPicBufRetLen);
            using (MemoryStream stream = new MemoryStream(pic))
            {
                try
                {
                    Image image = Image.FromStream(stream);
                    snap_pictureBox.Image = image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            Marshal.FreeHGlobal(outParam.szPicBuf);
            ++pic_index;
        }

        private void OpenDoor_Click(object sender, EventArgs e)
        {
            IntPtr intPtr = IntPtr.Zero;
            try
            {
                intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_ACCESS_OPEN)));
                NET_CTRL_ACCESS_OPEN open = new NET_CTRL_ACCESS_OPEN()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_ACCESS_OPEN)),
                    nChannelID = 0,
                    szTargetID = IntPtr.Zero,
                    emOpenDoorType = EM_OPEN_DOOR_TYPE.REMOTE
                };
                
                Marshal.StructureToPtr(open, intPtr, true);
                bool result = NETClient.ControlDevice(loginID, EM_CtrlType.ACCESS_OPEN, intPtr, 3000);
                if(result)
                {
                    MessageBox.Show("Successful opening(开门成功)");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
        }

        private void Search_button_Click(object sender, EventArgs e)
        {
            NET_FIND_RECORD_ACCESSCTLCARDREC_CONDITION_EX condition = new NET_FIND_RECORD_ACCESSCTLCARDREC_CONDITION_EX();
            condition.dwSize = (uint)Marshal.SizeOf(typeof(NET_FIND_RECORD_ACCESSCTLCARDREC_CONDITION_EX));
            if (card_search_radioButton.Checked)
            {
                condition.bCardNoEnable = true;
                condition.szCardNo = cardno_textBox.Text;
                if (condition.szCardNo == "")
                {
                    MessageBox.Show("Please enter the card number(请输入卡号)");
                    return;
                }
            }
            if (time_search_radioButton.Checked)
            {
                condition.bTimeEnable = true;
                condition.stStartTime = NET_TIME.FromDateTime(start_dateTimePicker.Value);
                condition.stEndTime = NET_TIME.FromDateTime(end_dateTimePicker.Value);
            }
            SearchInfoWnd info = new SearchInfoWnd(loginID, condition);
            info.ShowDialog();
            info.Dispose();
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            AddInfoWnd addInfo = new AddInfoWnd(loginID);
            addInfo.ShowDialog();
            addInfo.Dispose();
        }

        private void Remove_button_Click(object sender, EventArgs e)
        {
            bool ret = false;
            if (search_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a card(请选择卡)");
                return;
            }

            NET_RECORDSET_ACCESS_CTL_CARD info = (NET_RECORDSET_ACCESS_CTL_CARD)search_listView.SelectedItems[0].Tag;

            // delete facedata
            NET_IN_REMOVE_FACE_INFO face_in = new NET_IN_REMOVE_FACE_INFO()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_REMOVE_FACE_INFO)),
                szUserID = info.szUserID
            };
            NET_OUT_REMOVE_FACE_INFO face_out = new NET_OUT_REMOVE_FACE_INFO()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_REMOVE_FACE_INFO)),
            };
            IntPtr intPtr = IntPtr.Zero;
            IntPtr outPtr = IntPtr.Zero;
            try
            {
                intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_REMOVE_FACE_INFO)));
                outPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_REMOVE_FACE_INFO)));
                Marshal.StructureToPtr(face_in, intPtr, true);
                Marshal.StructureToPtr(face_out, outPtr, true);
                ret = NETClient.FaceInfoOpreate(loginID, EM_FACEINFO_OPREATE_TYPE.REMOVE, intPtr, outPtr, 5000);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
                Marshal.FreeHGlobal(outPtr);
            } 

            // delete card info
            NET_CTRL_RECORDSET_PARAM inParam = new NET_CTRL_RECORDSET_PARAM
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)),
                emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD
            };
            IntPtr recPtr = IntPtr.Zero;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                recPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
                Marshal.StructureToPtr(info.nRecNo, recPtr, true);
                inParam.pBuf = recPtr;
                inParam.nBufLen = Marshal.SizeOf(typeof(int));
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)));
                Marshal.StructureToPtr(inParam, inPtr, true);
                ret = NETClient.ControlDevice(loginID, EM_CtrlType.RECORDSET_REMOVE, inPtr, 10000);
                if (!ret)
                {
                    MessageBox.Show("Deleting card data failed(删除卡数据失败)");
                }
                else
                {
                    MessageBox.Show("Deleting card data successfully(删除卡数据成功)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(recPtr);
                Marshal.FreeHGlobal(inPtr);
            }
            GetRecord();
        }

        private void Update_button_Click(object sender, EventArgs e)
        {
            if(search_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one(请选择一项)!");
                return;
            }
            string userID = search_listView.FocusedItem.SubItems[0].Text;
            AddInfoWnd addInfo = new AddInfoWnd(loginID, userID);
            addInfo.ShowDialog();
            addInfo.Dispose();
        }

        private void GetRecord()
        {
            this.Find_button.Enabled = false;
            bool result = false;
            if (findID != IntPtr.Zero) 
            {
                NETClient.FindRecordClose(findID);
                findID = IntPtr.Zero;
            }
            search_listView.Items.Clear();
            var item = new ListViewItem();

            NET_FIND_RECORD_ACCESSCTLCARD_CONDITION condition = new NET_FIND_RECORD_ACCESSCTLCARD_CONDITION()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION))
            };
            
            if (searchcardno_textBox.Text != "")
            {
                condition.abCardNo = true;
                condition.szCardNo = searchcardno_textBox.Text;
            }
            object obj = condition;
            
            bool ret = NETClient.FindRecord(loginID, EM_NET_RECORD_TYPE.ACCESSCTLCARD, obj, typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION), ref findID, 10000);
            if (!ret)
            {
                MessageBox.Show("Get record failed(获取记录失败)");
                this.Find_button.Enabled = true;
                return;
            }

            Task task = new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        int max = 20;
                        int retNum = 0;
                        List<object> ls = new List<object>();
                        for (int i = 0; i < max; i++)
                        {
                            NET_RECORDSET_ACCESS_CTL_CARD card = new NET_RECORDSET_ACCESS_CTL_CARD
                            {
                                dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD)),
                                bEnableExtended = false,
                            };
                            ls.Add(card);
                        }

                        NETClient.FindNextRecord(findID, max, ref retNum, ref ls, typeof(NET_RECORDSET_ACCESS_CTL_CARD), 10000);
                        foreach (var it in ls)
                        {
                            NET_RECORDSET_ACCESS_CTL_CARD info = (NET_RECORDSET_ACCESS_CTL_CARD)it;

                            NET_IN_FINGERPRINT_GETBYUSER finger_in = new NET_IN_FINGERPRINT_GETBYUSER()
                            {
                                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_FINGERPRINT_GETBYUSER)),
                                szUserID = info.szUserID
                            };
                            NET_OUT_FINGERPRINT_GETBYUSER finger_out = new NET_OUT_FINGERPRINT_GETBYUSER()
                            {
                                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_FINGERPRINT_GETBYUSER)),
                                nSinglePacketLength = 1024,
                                nMaxFingerDataLength = 100 * 1024,
                            };

                            IntPtr inFacePtr = IntPtr.Zero;
                            IntPtr outFacePtr = IntPtr.Zero;

                            NET_IN_GET_FACE_INFO inFaceBuf = new NET_IN_GET_FACE_INFO()
                            {
                                dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_GET_FACE_INFO)),
                                szUserID = info.szUserID
                            };

                            NET_OUT_GET_FACE_INFO outFaceBuf = new NET_OUT_GET_FACE_INFO()
                            {
                                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_GET_FACE_INFO)),
                            };

                            string strFingerData = "";
                            string strFaceData = "";
                            try
                            {
                                // find fingerprint
                                finger_out.pbyFingerData = Marshal.AllocHGlobal(finger_out.nMaxFingerDataLength);
                                result = NETClient.Attendance_GetFingerByUserID(loginID, ref finger_in, ref finger_out, 3000);
                                if (finger_out.nRetFingerPrintCount > 0 && finger_out.nRetFingerDataLength > 0)
                                {
                                    byte[] data = new byte[finger_out.nRetFingerDataLength];
                                    Marshal.Copy(finger_out.pbyFingerData, data, 0, finger_out.nRetFingerDataLength);
                                    strFingerData = Convert.ToBase64String(data);
                                }

                                // find face
                                inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_GET_FACE_INFO)));
                                outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_GET_FACE_INFO)));
                                Marshal.StructureToPtr(inFaceBuf, inFacePtr, true);
                                Marshal.StructureToPtr(outFaceBuf, outFacePtr, true);

                                result = NETClient.FaceInfoOpreate(loginID, EM_FACEINFO_OPREATE_TYPE.GET, inFacePtr, outFacePtr, 5000);
                                outFaceBuf = (NET_OUT_GET_FACE_INFO)Marshal.PtrToStructure(outFacePtr, typeof(NET_OUT_GET_FACE_INFO));
                                if (outFaceBuf.nFaceData > 0)
                                {
                                    strFaceData = Convert.ToBase64String(outFaceBuf.szFaceData);
                                }
                            }
                            catch
                            {

                            }
                            finally
                            {
                                Marshal.FreeHGlobal(finger_out.pbyFingerData);
                                Marshal.FreeHGlobal(inFacePtr);
                                Marshal.FreeHGlobal(outFacePtr);
                            }

                            BeginInvoke(new Action(() =>
                            {
                                var list_item = new ListViewItem();
                                list_item.Text = info.szUserID.ToString();
                                list_item.SubItems.Add(info.szCardNo);
                                list_item.SubItems.Add(info.szCardName);
                                list_item.SubItems.Add(info.szPsw);
                                if (info.nUserType == 0)
                                {
                                    list_item.SubItems.Add("General User(普通用户)");
                                }
                                else if (info.nUserType == 1)
                                {
                                    list_item.SubItems.Add("Blacklist User(黑名单用户)");
                                }
                                list_item.SubItems.Add(strFingerData);
                                list_item.SubItems.Add(strFaceData);
                                list_item.Tag = info;

                                search_listView.BeginUpdate();
                                search_listView.Items.Insert(0, list_item);
                                search_listView.EndUpdate();
                            }));
                        }

                        Thread.Sleep(10);
                        if (retNum < max)
                        {
                            break;
                        }
                    }
                    this.BeginInvoke(new Action(() =>
                    {
                        this.Find_button.Enabled = true;
                    }));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            task.Start();
        }

        private void Find_button_Click(object sender, EventArgs e)
        {
            GetRecord();
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

        private void searchcardno_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length >= 32)
            {
                ((TextBox)sender).Text = _GetCardNo;
            }
            _GetCardNo = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

        private void cardno_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length >= 32)
            {
                ((TextBox)sender).Text = _SearchCardNo;
            }
            _SearchCardNo = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }
    }
}
