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
using NetSDKCS;

namespace FaceAttendance
{
    public partial class AddInfoWnd : Form
    {
        private IntPtr lLoginID = IntPtr.Zero;
        private string update_userID;
        private NET_RECORDSET_ACCESS_CTL_CARD card_info;
        private NET_OUT_GET_FACE_INFO get_face;
        private int nRec = 0;
        private fMessCallBackEx m_AlarmCallBack;
        private bool ret = false;
        private string[] CardStatus;
        private string[] CardTypes;
        private bool GetFaceData = false;
        private NET_ALARM_CAPTURE_FINGER_PRINT_INFO finger_info;
        public byte[] FingerPrintInfo { get; set; }
        public int PacketLen { get; set; }
        string _OldUserID = "";
        string _OldCardName = "";
        string _OldCardNo = "";
        string _OldPassword = "";

        public AddInfoWnd(IntPtr _LoginID, string userID="")
        {
            InitializeComponent();
            lLoginID = _LoginID;
            update_userID = userID;
            if (update_userID != "")
            {
                this.Text = "UpdateInfo(更新)";
                add_userID_textBox.Enabled = false;
            }
            get_face = new NET_OUT_GET_FACE_INFO()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_GET_FACE_INFO)),
                szFaceData = new byte[20*2048]
            };
            
            CardStatus = new string[]
            {
                "UnKnown(未知)",
                "Normal(正常)",
                "Lose(挂失)",
                "LogOff(注销)",
                "Freeze(冻结)",
                "Arrearage(欠费)",
                "Overdue(逾期)",
                "Prearrearage(预欠费)"
            };
            CardTypes = new string[]
            {
                "UnKnown(未知)",
                "General(一般卡)",
                "VIP(VIP卡)",
                "Guest(来宾卡)",
                "Patrol(巡逻卡)",
                "BlackList(黑名单卡)",
                "Corce(胁迫卡)",
                "Polling(巡检卡)",
                "Mothercard(母卡)"
            };
            finger_info = new NET_ALARM_CAPTURE_FINGER_PRINT_INFO()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_ALARM_CAPTURE_FINGER_PRINT_INFO))
            };

            this.Load += AddInfo_Load;
        }

        private void AddInfo_Load(object sender, EventArgs e)
        {
            isFinger.Checked = false;
            isFaceData.Checked = false;
            GetFaceData_button.Enabled = false;
            StartCollect_button.Enabled = false;
            get_user_textBox.Enabled = false;
            ChannelID_textBox.Enabled = false;
            ReaderID_textBox.Enabled = false;
            foreach (string status in CardStatus)
            {
                cardStatus_comboBox.Items.Add(status);
            }

            foreach (string type in CardTypes)
            {
                cardType_comboBox.Items.Add(type);
            }

            if (update_userID == "")
            {
                cardType_comboBox.SelectedIndex = 0;
                cardStatus_comboBox.SelectedIndex = 0;
            }
            else
            {
                SetToControl();
            }

            m_AlarmCallBack = new fMessCallBackEx(MessCallBackEx);
            NETClient.SetDVRMessCallBack(m_AlarmCallBack, IntPtr.Zero);
            ret = NETClient.StartListen(lLoginID);
            if (!ret)
            {
                MessageBox.Show("Listening failure(监听失败)");
            }
        }

        private bool MessCallBackEx(int lCommand, IntPtr lLoginID, IntPtr pBuf, uint dwBufLen, IntPtr pchDVRIP, int nDVRPort, bool bAlarmAckFlag, int nEventID, IntPtr dwUser)
        {
            if ((EM_ALARM_TYPE)lCommand == EM_ALARM_TYPE.ALARM_FINGER_PRINT)
            {
                finger_info = (NET_ALARM_CAPTURE_FINGER_PRINT_INFO)Marshal.PtrToStructure(pBuf, typeof(NET_ALARM_CAPTURE_FINGER_PRINT_INFO));
                byte[] data = new byte[finger_info.nPacketLen * finger_info.nPacketNum];
                Marshal.Copy(finger_info.szFingerPrintInfo, data, 0, data.Length);
                FingerPrintInfo = data;
                PacketLen = finger_info.nPacketLen * finger_info.nPacketNum;
                this.BeginInvoke(new Action(() =>
                {
                    is_collect_label.Text = "Successfully collected(采集成功)";
                }));
            }

            return true;
        }

        private void SetToControl()
        {
            NET_FIND_RECORD_ACCESSCTLCARD_CONDITION condition = new NET_FIND_RECORD_ACCESSCTLCARD_CONDITION
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION)),
                abUserID = true,
                szUserID = update_userID
            };
            object obj = condition;
            IntPtr findID = IntPtr.Zero;
            bool ret = NETClient.FindRecord(lLoginID, EM_NET_RECORD_TYPE.ACCESSCTLCARD, obj, typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION), ref findID, 10000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                return;
            }

            Task task = new Task(() =>
            {
                int max = 1;
                int retNum = 0;
                List<object> ls = new List<object>();
                NET_RECORDSET_ACCESS_CTL_CARD card = new NET_RECORDSET_ACCESS_CTL_CARD
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD))
                };
                ls.Add(card);
                try
                {
                    NETClient.FindNextRecord(findID, max, ref retNum, ref ls, typeof(NET_RECORDSET_ACCESS_CTL_CARD), 10000);
                    BeginInvoke(new Action(() =>
                    {
                        if (ls.Count >= 1)
                        {
                            card_info = (NET_RECORDSET_ACCESS_CTL_CARD)ls[0];
                            nRec = card_info.nRecNo;
                            cardStatus_comboBox.SelectedIndex = cardStatus_comboBox.FindString(card_info.emStatus.ToString());
                            cardType_comboBox.SelectedIndex = cardType_comboBox.FindString(card_info.emType.ToString());
                            useTime_textBox.Text = card_info.nUseTime.ToString();
                            add_userID_textBox.Text = card_info.szUserID.ToString();
                            cardNo_textBox.Text = card_info.szCardNo;
                            cardName_textBox.Text = card_info.szCardName;
                            cardPwd_textBox.Text = card_info.szPsw;

                            try
                            {
                                start_dateTimePicker.Value = card_info.stuValidStartTime.ToDateTime();
                            }
                            catch
                            {
                                start_dateTimePicker.Value = start_dateTimePicker.MinDate;
                            }

                            try
                            {
                                end_dateTimePicker.Value = card_info.stuValidEndTime.ToDateTime();
                            }
                            catch
                            {
                                end_dateTimePicker.Value = end_dateTimePicker.MaxDate;
                            }
                        }
                    }));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            task.Start();
            if (task.IsCompleted)
            {
                NETClient.FindRecordClose(findID);
                findID = IntPtr.Zero;
            }
        }

        private EM_ACCESSCTLCARD_STATE CardStatusSwitch(int index)
        {
            EM_ACCESSCTLCARD_STATE state = new EM_ACCESSCTLCARD_STATE();
            switch (index-1)
            { 
                case 0:
                    state = EM_ACCESSCTLCARD_STATE.NORMAL;
                    break;
                case 1:
                    state = EM_ACCESSCTLCARD_STATE.LOSE;
                    break;
                case 2:
                    state = EM_ACCESSCTLCARD_STATE.LOGOFF;
                    break;
                case 3:
                    state = EM_ACCESSCTLCARD_STATE.FREEZE;
                    break;
                case 4:
                    state = EM_ACCESSCTLCARD_STATE.ARREARAGE;
                    break;
                case 5:
                    state = EM_ACCESSCTLCARD_STATE.OVERDUE;
                    break;
                case 6:
                    state = EM_ACCESSCTLCARD_STATE.PREARREARAGE;
                    break;
                default:
                    state = EM_ACCESSCTLCARD_STATE.UNKNOWN;
                    break;
            }
            return state;
        }

        private EM_ACCESSCTLCARD_TYPE CardTypeSwitch(int index)
        {
            EM_ACCESSCTLCARD_TYPE type = new EM_ACCESSCTLCARD_TYPE();
            switch (index-1)
            {
                case 0:
                    type = EM_ACCESSCTLCARD_TYPE.GENERAL;
                    break;
                case 1:
                    type = EM_ACCESSCTLCARD_TYPE.VIP;
                    break;
                case 2:
                    type = EM_ACCESSCTLCARD_TYPE.GUEST;
                    break;
                case 3:
                    type = EM_ACCESSCTLCARD_TYPE.PATROL;
                    break;
                case 4:
                    type = EM_ACCESSCTLCARD_TYPE.BLACKLIST;
                    break;
                case 5:
                    type = EM_ACCESSCTLCARD_TYPE.CORCE;
                    break;
                case 6: 
                    type = EM_ACCESSCTLCARD_TYPE.POLLING;
                    break;
                case 7:
                    type = EM_ACCESSCTLCARD_TYPE.MOTHERCARD;
                    break;
                default:
                    type = EM_ACCESSCTLCARD_TYPE.UNKNOWN;
                    break;
            }
            return type;
        }

        private NET_RECORDSET_ACCESS_CTL_CARD GetFromControl(out bool ret)
        {
            ret = true;
            NET_RECORDSET_ACCESS_CTL_CARD cardInfo = new NET_RECORDSET_ACCESS_CTL_CARD();
            cardInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
            int times = 0;
            try
            {
                cardInfo.szUserID = add_userID_textBox.Text;
                cardInfo.szCardNo = cardNo_textBox.Text;
                cardInfo.szCardName = cardName_textBox.Text;
                cardInfo.emStatus = CardStatusSwitch(cardStatus_comboBox.SelectedIndex);
                cardInfo.emType = CardTypeSwitch(cardType_comboBox.SelectedIndex);
                cardInfo.szPsw = cardPwd_textBox.Text;
                times = Convert.ToInt32(this.useTime_textBox.Text.Trim());
                cardInfo.nUseTime = times;
            }
            catch
            {
                cardInfo.nUseTime = 0;
                ret = false;
            }
            
            cardInfo.stuValidStartTime = NET_TIME.FromDateTime(start_dateTimePicker.Value);
            cardInfo.stuValidEndTime = NET_TIME.FromDateTime(end_dateTimePicker.Value);
            cardInfo.bIsValid = true;
            cardInfo.nRecNo = nRec;

            return cardInfo;
        }

        private void InsertRecord()
        {
            bool ret = false;
            bool Res = false;
            string error_msg = "";
            NET_CTRL_RECORDSET_INSERT_PARAM inParam = new NET_CTRL_RECORDSET_INSERT_PARAM()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_PARAM)),
                stuCtrlRecordSetInfo = new NET_CTRL_RECORDSET_INSERT_IN()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_IN)),
                    emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD
                },
                stuCtrlRecordSetResult = new NET_CTRL_RECORDSET_INSERT_OUT()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_OUT)),
                },
            };

            NET_RECORDSET_ACCESS_CTL_CARD cardInfo = GetFromControl(out Res);
            if (cardInfo.szUserID == "")
            {
                MessageBox.Show("Please add UserID(请添加用户ID)");
                return;
            }
            if (!Res || cardInfo.nUseTime < 0)
            {
                useTime_textBox.Text = "0";
                MessageBox.Show("The usetime's value must be 0 - 2147483647(使用次数的值必须是0-2147483647)");
                return;
            }

            if (isFinger.Checked)
            {
                cardInfo.bEnableExtended = true;
                cardInfo.stuFingerPrintInfoEx.nLength = finger_info.nPacketLen;
                if (cardInfo.stuFingerPrintInfoEx.nLength == 0)
                {
                    MessageBox.Show("Please collect fingerprints(请采集指纹数据)");
                    return;
                }
                cardInfo.stuFingerPrintInfoEx.nCount = finger_info.nPacketNum;
                cardInfo.stuFingerPrintInfoEx.nPacketLen = finger_info.nPacketLen * finger_info.nPacketNum;
                try
                {
                    cardInfo.stuFingerPrintInfoEx.pPacketData = Marshal.AllocHGlobal(PacketLen);
                    Marshal.Copy(FingerPrintInfo, 0, cardInfo.stuFingerPrintInfoEx.pPacketData, PacketLen);
                }
                catch(Exception ex)
                {
                    Marshal.FreeHGlobal(cardInfo.stuFingerPrintInfoEx.pPacketData);
                    MessageBox.Show(ex.Message);
                    return;
                }       
            }
            
            IntPtr RecordInfoPtr = IntPtr.Zero;
            IntPtr ParamPtr = IntPtr.Zero;
            try
            {
                RecordInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD)));
                Marshal.StructureToPtr(cardInfo, RecordInfoPtr, true);
                inParam.stuCtrlRecordSetInfo.pBuf = RecordInfoPtr;
                inParam.stuCtrlRecordSetInfo.nBufLen = Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                ParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_PARAM)));
                Marshal.StructureToPtr(inParam, ParamPtr, true);
                if (isFinger.Checked)
                {
                    ret = NETClient.ControlDevice(lLoginID, EM_CtrlType.RECORDSET_INSERTEX, ParamPtr, 10000);
                }
                else
                {
                    ret = NETClient.ControlDevice(lLoginID, EM_CtrlType.RECORDSET_INSERT, ParamPtr, 10000);
                }
                if (!ret)
                {
                    error_msg = "Add Card Failed(添加卡失败);";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Marshal.FreeHGlobal(cardInfo.stuFingerPrintInfoEx.pPacketData);
                Marshal.FreeHGlobal(RecordInfoPtr);
                Marshal.FreeHGlobal(ParamPtr);
            }
            if (error_msg.Length == 0 && isFaceData.Checked) 
            {
                if (GetFaceData)
                {
                    ret = AddFaceData();
                    if (!ret)
                    {
                        error_msg = "Add face data failed(添加人脸数据失败)";
                    }
                }
            }
            
            if (error_msg == "")
            {
                MessageBox.Show("Add success(添加成功)");
            }
            else
            {
                MessageBox.Show(error_msg);
            }

        }

        private void UpdateRecord()
        {
            bool ret = false;
            bool res = false;
            string errror_msg = "";
            NET_CTRL_RECORDSET_PARAM inParam = new NET_CTRL_RECORDSET_PARAM()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)),
                emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD,
            };

            NET_RECORDSET_ACCESS_CTL_CARD cardInfo = GetFromControl(out res);
            if (!res || cardInfo.nUseTime < 0)
            {
                MessageBox.Show("The usetime's value must be 0 - 2147483647(使用次数的值必须是0-2147483647)");
                return;
            }

            if (isFinger.Checked)
            {
                cardInfo.bEnableExtended = true;
                cardInfo.stuFingerPrintInfoEx.nLength = finger_info.nPacketLen;
                if (cardInfo.stuFingerPrintInfoEx.nLength == 0)
                {
                    MessageBox.Show("Please collect fingerprints(请采集指纹数据)");
                    return;
                }
                cardInfo.stuFingerPrintInfoEx.nCount = finger_info.nPacketNum;
                cardInfo.stuFingerPrintInfoEx.nPacketLen = finger_info.nPacketLen * finger_info.nPacketNum;
                try
                {
                    cardInfo.stuFingerPrintInfoEx.pPacketData = Marshal.AllocHGlobal(PacketLen);
                    Marshal.Copy(FingerPrintInfo, 0, cardInfo.stuFingerPrintInfoEx.pPacketData, PacketLen);
                }
                catch (Exception ex)
                {
                    Marshal.FreeHGlobal(cardInfo.stuFingerPrintInfoEx.pPacketData);
                    MessageBox.Show(ex.Message);
                    return;
                } 
            }
            else 
            {
                NET_IN_FINGERPRINT_GETBYUSER finger_in = new NET_IN_FINGERPRINT_GETBYUSER()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_FINGERPRINT_GETBYUSER)),
                    szUserID = update_userID
                };
                NET_OUT_FINGERPRINT_GETBYUSER finger_out = new NET_OUT_FINGERPRINT_GETBYUSER()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_FINGERPRINT_GETBYUSER)),
                    nSinglePacketLength = 1024,
                    nMaxFingerDataLength = 100 * 1024,
                    pbyFingerData = Marshal.AllocHGlobal(100 * 1024),
                };
                bool result = NETClient.Attendance_GetFingerByUserID(lLoginID, ref finger_in, ref finger_out, 3000);
                if (result) 
                {
                    cardInfo.bEnableExtended = true;
                    cardInfo.stuFingerPrintInfoEx.nLength = finger_out.nSinglePacketLength;
                    cardInfo.stuFingerPrintInfoEx.nCount = finger_out.nRetFingerPrintCount;
                    cardInfo.stuFingerPrintInfoEx.nPacketLen = finger_out.nSinglePacketLength * finger_out.nRetFingerPrintCount;
                    cardInfo.stuFingerPrintInfoEx.pPacketData = finger_out.pbyFingerData;
                }
            }

            IntPtr RecordInfoPtr = IntPtr.Zero;
            IntPtr ParamPtr = IntPtr.Zero;
            try
            {
                RecordInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD)));
                Marshal.StructureToPtr(cardInfo, RecordInfoPtr, true);
                inParam.pBuf = RecordInfoPtr;
                ParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)));
                Marshal.StructureToPtr(inParam, ParamPtr, true);
                ret = NETClient.ControlDevice(lLoginID, EM_CtrlType.RECORDSET_UPDATEEX, ParamPtr, 10000);
                if (!ret)
                {
                    errror_msg = "Update card failed(更新卡失败);";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Marshal.FreeHGlobal(RecordInfoPtr);
                Marshal.FreeHGlobal(ParamPtr);
            }

            if (isFaceData.Checked)
            {
                if (GetFaceData)
                {
                    ret = UpdateFaceData();
                    if (!ret)
                    {
                        errror_msg += "Failed to update face data(更新人脸数据失败)";
                    }
                }
            }
            if (errror_msg == "")
            {
                MessageBox.Show("Update success(更新成功)");
            }
            else
            {
                MessageBox.Show(errror_msg);
            }
        }

        private bool AddFaceData()
        {
            bool bRet = false;
            IntPtr inFacePtr = IntPtr.Zero;
            IntPtr outFacePtr = IntPtr.Zero;
            try
            {
                NET_IN_ADD_FACE_INFO inAddFaceInfo = new NET_IN_ADD_FACE_INFO
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ADD_FACE_INFO)),
                    szUserID = add_userID_textBox.Text,
                    stuFaceInfo = new NET_FACE_RECORD_INFO()
                    {
                        nFaceData = get_face.nFaceData,
                        szFaceData = get_face.szFaceData
                    }
                };

                inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_ADD_FACE_INFO)));
                outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_ADD_FACE_INFO)));
                Marshal.StructureToPtr(inAddFaceInfo, inFacePtr, true);

                NET_OUT_ADD_FACE_INFO outAddFaceInfo = new NET_OUT_ADD_FACE_INFO
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ADD_FACE_INFO))
                };
                
                Marshal.StructureToPtr(outAddFaceInfo, outFacePtr, true);
                bRet = NETClient.FaceInfoOpreate(lLoginID, EM_FACEINFO_OPREATE_TYPE.ADD, inFacePtr, outFacePtr, 5000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);  
            }
            finally
            {
                Marshal.FreeHGlobal(inFacePtr);
                Marshal.FreeHGlobal(outFacePtr);
            }
            return bRet;
        }

        private bool UpdateFaceData()
        {
            bool bRet = false;
            if(get_face.nFaceData <= 0)
            {
                return bRet;
            }
            IntPtr inFacePtr = IntPtr.Zero;
            IntPtr outFacePtr = IntPtr.Zero;
            try
            {
                NET_IN_UPDATE_FACE_INFO inUpdateFaceInfo = new NET_IN_UPDATE_FACE_INFO
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_UPDATE_FACE_INFO)),
                    szUserID = add_userID_textBox.Text,
                    stuFaceInfo = new NET_FACE_RECORD_INFO()
                    {
                        nFaceData = get_face.nFaceData,
                        szFaceData = get_face.szFaceData,
                    }
                };

                inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_UPDATE_FACE_INFO)));
                outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_UPDATE_FACE_INFO)));
                Marshal.StructureToPtr(inUpdateFaceInfo, inFacePtr, true);

                NET_OUT_UPDATE_FACE_INFO outUpdateFaceInfo = new NET_OUT_UPDATE_FACE_INFO
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_UPDATE_FACE_INFO))
                };
                
                Marshal.StructureToPtr(outUpdateFaceInfo, outFacePtr, true);
                bRet = NETClient.FaceInfoOpreate(lLoginID, EM_FACEINFO_OPREATE_TYPE.UPDATE, inFacePtr, outFacePtr, 5000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inFacePtr);
                Marshal.FreeHGlobal(outFacePtr);
            }
            return bRet;
        }

        private void GetFaceData_button_Click(object sender, EventArgs e)
        {
            IntPtr inFacePtr = IntPtr.Zero; 
            IntPtr outFacePtr = IntPtr.Zero; 

            try
            {
                inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_GET_FACE_INFO)));
                outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_GET_FACE_INFO)));
                NET_IN_GET_FACE_INFO inBuf = new NET_IN_GET_FACE_INFO()
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_GET_FACE_INFO)),
                    szUserID = get_user_textBox.Text
                };
                if (inBuf.szUserID == "")
                {
                    MessageBox.Show("Please enter userID(请输入UserID)");
                    return;
                }

                Marshal.StructureToPtr(inBuf, inFacePtr, true);
                Marshal.StructureToPtr(get_face, outFacePtr, true);

                GetFaceData = NETClient.FaceInfoOpreate(lLoginID, EM_FACEINFO_OPREATE_TYPE.GET, inFacePtr, outFacePtr, 5000);
                if (!GetFaceData)
                {
                    MessageBox.Show("Get face Data fail(获取人脸数据失败)");
                    return;
                }
                else
                {
                    MessageBox.Show("Get face Data success(获取人脸数据成功)");
                }
                get_face = (NET_OUT_GET_FACE_INFO)Marshal.PtrToStructure(outFacePtr, typeof(NET_OUT_GET_FACE_INFO));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inFacePtr);
                Marshal.FreeHGlobal(outFacePtr);
            }
        }

        private void StartCollect_button_Click(object sender, EventArgs e)
        {
            bool result = false;
            FingerPrintInfo = null;
            NET_CTRL_CAPTURE_FINGER_PRINT capture = new NET_CTRL_CAPTURE_FINGER_PRINT()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_CAPTURE_FINGER_PRINT)),
                szReaderID = ReaderID_textBox.Text
            };
            if (capture.szReaderID == "" || ChannelID_textBox.Text =="")
            {
                MessageBox.Show("Please enter ChannelID and ReaderID(请输入通道号及读卡器ID)");
                return;
            }

            try
            { 
                int ReaderID = Convert.ToInt32(capture.szReaderID);
                capture.nChannelID = Convert.ToInt32(ChannelID_textBox.Text);
            }
            catch
            {
                MessageBox.Show("Channel and ReaderID must be numbers(通道号和卡号必须为数字)");
                return;
            }

            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_CAPTURE_FINGER_PRINT)));
                Marshal.StructureToPtr(capture, inPtr, true);
                result = NETClient.ControlDevice(lLoginID, EM_CtrlType.CAPTURE_FINGER_PRINT, inPtr, 10000);
                if (!result)
                {
                    MessageBox.Show(NETClient.GetLastError());
                    return;
                }
                is_collect_label.Text = "Collecting(正在采集)...";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
            }
        }

        private void isFinger_CheckedChanged(object sender, EventArgs e)
        {
            if (isFinger.Checked)
            {
                StartCollect_button.Enabled = true;
                ChannelID_textBox.Enabled = true;
                ReaderID_textBox.Enabled = true;
            }
            else
            {
                ChannelID_textBox.Text = "";
                ReaderID_textBox.Text = "";
                StartCollect_button.Enabled = false;
                ChannelID_textBox.Enabled = false;
                ReaderID_textBox.Enabled = false;
            }
        }

        private void Ok_button_Click(object sender, EventArgs e)
        {
            if (update_userID == "")
            {
                InsertRecord();
            }
            else
            {
                UpdateRecord();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (ret)
            {
                NETClient.SetDVRMessCallBack(null, IntPtr.Zero);
                NETClient.StopListen(lLoginID);
                ret = false;
            }
        }

        private void add_userID_textBox_TextChanged(object sender, EventArgs e)
        {
            if(Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length >= 32)
            {
                ((TextBox)sender).Text = _OldUserID;
            }
            _OldUserID = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

        private void cardNo_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length >= 32)
            {
                ((TextBox)sender).Text = _OldCardNo;
            }
            _OldCardNo = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

        private void cardName_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length >= 64)
            {
                ((TextBox)sender).Text = _OldCardName;
            }
            _OldCardName = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

        private void cardPwd_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length >= 64)
            {
                ((TextBox)sender).Text = _OldPassword;
            }
            _OldPassword = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

        private void isFaceData_CheckedChanged(object sender, EventArgs e)
        {
            if (isFaceData.Checked)
            {
                GetFaceData_button.Enabled = true;
                get_user_textBox.Enabled = true;
            }
            else
            {
                get_user_textBox.Text = "";
                GetFaceData_button.Enabled = false;
                get_user_textBox.Enabled = false;
            }
        }
    }
}
