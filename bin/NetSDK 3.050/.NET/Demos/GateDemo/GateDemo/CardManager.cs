using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSDKCS;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace GateDemo
{
    public partial class CardManager : Form
    {
        IntPtr _LoginID;
        TextInfo _TextInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
        IntPtr _FindID = IntPtr.Zero;
        Task _Task;
        bool _IsCanncel = false;
        string _OldCardNo = "";

        public CardManager(IntPtr loginID)
        {
            InitializeComponent();
            _LoginID = loginID;
        }

        protected override void OnShown(EventArgs e)
        {
            this.listView_card.Columns[0].Width = 100;//show scrollbar
            base.OnShown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_Task != null && _Task.IsCompleted == false)
            {
                _IsCanncel = true;
            }
            base.OnClosed(e);
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            this.button_search.Enabled = false;
            this.listView_card.Items.Clear();
            if (IntPtr.Zero != _FindID)
            {
                NETClient.FindRecordClose(_FindID);
                _FindID = IntPtr.Zero;
            }
            NET_FIND_RECORD_ACCESSCTLCARD_CONDITION condition = new NET_FIND_RECORD_ACCESSCTLCARD_CONDITION();
            condition.dwSize = (uint)Marshal.SizeOf(typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION));
            if (this.textBox_cardno.Text != "")
            {
                condition.abCardNo = true;
                condition.szCardNo = this.textBox_cardno.Text.Trim();
            }
            object obj = condition;
            bool ret = NETClient.FindRecord(_LoginID, EM_NET_RECORD_TYPE.ACCESSCTLCARD, obj, typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION), ref _FindID, 10000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                this.button_search.Enabled = true;
                return;
            }

            _Task = new Task(() =>
            {
                while (true)
                {
                    if (_IsCanncel)
                    {
                        break;
                    }
                    int max = 20;
                    int retNum = 0;
                    List<object> ls = new List<object>();
                    for (int i = 0; i < max; i++)
                    {
                        NET_RECORDSET_ACCESS_CTL_CARD card = new NET_RECORDSET_ACCESS_CTL_CARD();
                        card.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                        ls.Add(card);
                    }
                    NETClient.FindNextRecord(_FindID, max, ref retNum, ref ls, typeof(NET_RECORDSET_ACCESS_CTL_CARD), 10000);
                    if (_IsCanncel)
                    {
                        break;
                    }
                    this.BeginInvoke(new Action(() =>
                    {
                        foreach (var item in ls)
                        {
                            if (_IsCanncel)
                            {
                                break;
                            }
                            NET_RECORDSET_ACCESS_CTL_CARD info = (NET_RECORDSET_ACCESS_CTL_CARD)item;
                            ListViewItem listViewItem = new ListViewItem();
                            listViewItem.Text = info.nRecNo.ToString();
                            listViewItem.SubItems.Add(info.szCardNo);
                            listViewItem.SubItems.Add(info.szCardName);
                            listViewItem.SubItems.Add(info.szUserID);
                            if (Enum.GetName(typeof(EM_ACCESSCTLCARD_STATE), info.emStatus) == null)
                            {
                                listViewItem.SubItems.Add("Unknown(未知)");
                            }
                            else
                            {
                                listViewItem.SubItems.Add(_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_ACCESSCTLCARD_STATE), info.emStatus).ToLower()) + GetChnStatus(info.emStatus));
                            }
                            if (Enum.GetName(typeof(EM_ACCESSCTLCARD_TYPE), info.emType) == null)
                            {
                                listViewItem.SubItems.Add("Unknown(未知)");
                            }
                            else
                            {
                                listViewItem.SubItems.Add(_TextInfo.ToTitleCase(Enum.GetName(typeof(EM_ACCESSCTLCARD_TYPE), info.emType).ToLower()) + GetChnType(info.emType));
                            }
                            listViewItem.SubItems.Add(info.szPsw);
                            listViewItem.SubItems.Add(info.bIsValid.ToString() + GetChnValid(info.bIsValid));
                            listViewItem.SubItems.Add(info.nUseTime.ToString());
                            listViewItem.SubItems.Add(info.bFirstEnter.ToString() + GetChnFirstEnter(info.bFirstEnter));
                            listViewItem.SubItems.Add(info.stuValidStartTime.ToString());
                            listViewItem.SubItems.Add(info.stuValidEndTime.ToString());
                            listViewItem.Tag = info;
                            if (_IsCanncel)
                            {
                                break;
                            }
                            listView_card.BeginUpdate();
                            listView_card.Items.Add(listViewItem);
                            listView_card.EndUpdate();
                        }

                    }));
                    if (retNum < max)
                    {
                        break;
                    }
                }
                if (_IsCanncel == false)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.button_search.Enabled = true;
                    }));
                }

            });
            _Task.Start();
        }

        private string GetChnStatus(EM_ACCESSCTLCARD_STATE status)
        {
            string res = "(未知)";
            switch (status)
            {
                case EM_ACCESSCTLCARD_STATE.UNKNOWN:
                    break;
                case EM_ACCESSCTLCARD_STATE.NORMAL:
                    res = "(正常)";
                    break;
                case EM_ACCESSCTLCARD_STATE.LOSE:
                    res = "(挂失)";
                    break;
                case EM_ACCESSCTLCARD_STATE.LOGOFF:
                    res = "(注销)";
                    break;
                case EM_ACCESSCTLCARD_STATE.FREEZE:
                    res = "(冻结)";
                    break;
                case EM_ACCESSCTLCARD_STATE.ARREARAGE:
                    res = "(欠费)";
                    break;
                case EM_ACCESSCTLCARD_STATE.OVERDUE:
                    res = "(逾期)";
                    break;
                case EM_ACCESSCTLCARD_STATE.PREARREARAGE:
                    res = "(预欠费)";
                    break;
                default:
                    break;
            }
            return res;
        }

        private string GetChnType(EM_ACCESSCTLCARD_TYPE type)
        {
            string res = "(未知)";
            switch (type)
            {
                case EM_ACCESSCTLCARD_TYPE.UNKNOWN:
                    break;
                case EM_ACCESSCTLCARD_TYPE.GENERAL:
                    res = "(一般卡)";
                    break;
                case EM_ACCESSCTLCARD_TYPE.VIP:
                    res = "(贵宾卡)";
                    break;
                case EM_ACCESSCTLCARD_TYPE.GUEST:
                    res = "(来宾卡)";
                    break;
                case EM_ACCESSCTLCARD_TYPE.PATROL:
                    res = "(巡逻卡)";
                    break;
                case EM_ACCESSCTLCARD_TYPE.BLACKLIST:
                    res = "(黑名单卡)";
                    break;
                case EM_ACCESSCTLCARD_TYPE.CORCE:
                    res = "(胁迫卡)";
                    break;
                case EM_ACCESSCTLCARD_TYPE.POLLING:
                    res = "(巡检卡)";
                    break;
                case EM_ACCESSCTLCARD_TYPE.MOTHERCARD:
                    res = "(母卡)";
                    break;
                default:
                    break;
            }
            return res;
        }

        private string GetChnValid(bool value)
        {
            if (value)
            {
                return "(有效)";
            }
            else
            {
                return "(无效)";
            }
        }

        private string GetChnFirstEnter(bool value)
        {
            if (value)
            {
                return "(首卡)";
            }
            else
            {
                return "(不是首卡)";
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            AddCard addCard = new AddCard();
            var ret = addCard.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //insert card 下发卡
                    NET_CTRL_RECORDSET_INSERT_PARAM inParam = new NET_CTRL_RECORDSET_INSERT_PARAM();
                    inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_PARAM));
                    inParam.stuCtrlRecordSetInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_IN));
                    inParam.stuCtrlRecordSetResult.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_OUT));
                    inParam.stuCtrlRecordSetInfo.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
                    NET_RECORDSET_ACCESS_CTL_CARD cardInfo = new NET_RECORDSET_ACCESS_CTL_CARD();
                    cardInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                    cardInfo.nDoorNum = 2;
                    cardInfo.sznDoors = new int[32];
                    cardInfo.sznDoors[0] = 0;
                    cardInfo.sznDoors[1] = 1;
                    cardInfo.nTimeSectionNum = 2;
                    cardInfo.sznTimeSectionNo = new int[32];
                    cardInfo.sznTimeSectionNo[0] = 255;
                    cardInfo.sznTimeSectionNo[1] = 255;
                    cardInfo.szCardNo = addCard.NewCard.CardNo;
                    cardInfo.szCardName = addCard.NewCard.CardName;
                    cardInfo.szUserID = addCard.NewCard.UserID;
                    cardInfo.emStatus = addCard.NewCard.CardStatus;
                    cardInfo.emType = addCard.NewCard.CardType;
                    cardInfo.szPsw = addCard.NewCard.CardPassword;
                    cardInfo.nUseTime = addCard.NewCard.UseTime;
                    cardInfo.bFirstEnter = addCard.NewCard.IsFirstEnter;
                    cardInfo.bIsValid = addCard.NewCard.IsValid;
                    cardInfo.stuValidStartTime = addCard.NewCard.StartTime;
                    cardInfo.stuValidEndTime = addCard.NewCard.EndTime;
                    IntPtr inPtr = IntPtr.Zero;
                    IntPtr ptr = IntPtr.Zero;
                    try
                    {
                        inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD)));
                        Marshal.StructureToPtr(cardInfo, inPtr, true);
                        inParam.stuCtrlRecordSetInfo.pBuf = inPtr;
                        inParam.stuCtrlRecordSetInfo.nBufLen = Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                        ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_PARAM)));
                        Marshal.StructureToPtr(inParam, ptr, true);
                        bool res = NETClient.ControlDevice(_LoginID, EM_CtrlType.RECORDSET_INSERT, ptr, 10000);
                        if (!res)
                        {
                            MessageBox.Show("Add card failed(增加卡失败)");
                            addCard.Dispose();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        addCard.Dispose();
                        return;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(inPtr);
                        Marshal.FreeHGlobal(ptr);
                    }
                    // add face image 增加人脸图片
                    IntPtr inFacePtr = IntPtr.Zero;
                    IntPtr outFacePtr = IntPtr.Zero;
                    IntPtr facePtr = IntPtr.Zero;
                    try
                    {
                        NET_IN_ADD_FACE_INFO inAddFaceInfo = new NET_IN_ADD_FACE_INFO();
                        inAddFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ADD_FACE_INFO));
                        inAddFaceInfo.szUserID = addCard.NewCard.UserID;
                        inAddFaceInfo.stuFaceInfo.szUserName = addCard.NewCard.CardName;
                        inAddFaceInfo.stuFaceInfo.nFacePhoto = 1;
                        inAddFaceInfo.stuFaceInfo.nFacePhotoLen = new int[5];
                        inAddFaceInfo.stuFaceInfo.pszFacePhoto = new IntPtr[5];
                        inAddFaceInfo.stuFaceInfo.nFacePhotoLen[0] = addCard.ImageData.Length;
                        facePtr = Marshal.AllocHGlobal(addCard.ImageData.Length);
                        Marshal.Copy(addCard.ImageData, 0, facePtr, addCard.ImageData.Length);
                        inAddFaceInfo.stuFaceInfo.pszFacePhoto[0] = facePtr;

                        inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_ADD_FACE_INFO)));
                        Marshal.StructureToPtr(inAddFaceInfo, inFacePtr, true);

                        NET_OUT_ADD_FACE_INFO outAddFaceInfo = new NET_OUT_ADD_FACE_INFO();
                        outAddFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ADD_FACE_INFO));
                        outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_ADD_FACE_INFO)));
                        Marshal.StructureToPtr(outAddFaceInfo, outFacePtr, true);
                        bool result = NETClient.FaceInfoOpreate(_LoginID, EM_FACEINFO_OPREATE_TYPE.ADD, inFacePtr, outFacePtr, 5000);
                        if (!result)
                        {
                            MessageBox.Show("Add face failed(增加人脸失败)");
                            addCard.Dispose();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        addCard.Dispose();
                        return;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(facePtr);
                        Marshal.FreeHGlobal(inFacePtr);
                        Marshal.FreeHGlobal(outFacePtr);
                    }
                    MessageBox.Show("Add successfully(增加成功)");
                }
                catch (Exception ex)
                {
                    addCard.Dispose();
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            addCard.Dispose();
        }

        private void button_modify_Click(object sender, EventArgs e)
        {
            if (listView_card.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one card frist(请先选择一张卡)");
                return;
            }
            NET_RECORDSET_ACCESS_CTL_CARD card = (NET_RECORDSET_ACCESS_CTL_CARD)listView_card.SelectedItems[0].Tag;
            ModifyCard modifyCard = new ModifyCard(card);
            var ret = modifyCard.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //modify card 修改卡
                    NET_CTRL_RECORDSET_PARAM inParam = new NET_CTRL_RECORDSET_PARAM();
                    inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM));
                    inParam.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
                    NET_RECORDSET_ACCESS_CTL_CARD cardInfo = new NET_RECORDSET_ACCESS_CTL_CARD();
                    cardInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                    cardInfo.nDoorNum = 2;
                    cardInfo.sznDoors = new int[32];
                    cardInfo.sznDoors[0] = 0;
                    cardInfo.sznDoors[1] = 1;
                    cardInfo.nTimeSectionNum = 2;
                    cardInfo.sznTimeSectionNo = new int[32];
                    cardInfo.sznTimeSectionNo[0] = 255;
                    cardInfo.sznTimeSectionNo[1] = 255;

                    cardInfo.nRecNo = card.nRecNo;
                    cardInfo.szCardNo = modifyCard.EditCard.CardNo;
                    cardInfo.szCardName = modifyCard.EditCard.CardName;
                    cardInfo.szUserID = modifyCard.EditCard.UserID;
                    cardInfo.emStatus = modifyCard.EditCard.CardStatus;
                    cardInfo.emType = modifyCard.EditCard.CardType;
                    cardInfo.szPsw = modifyCard.EditCard.CardPassword;
                    cardInfo.nUseTime = modifyCard.EditCard.UseTime;
                    cardInfo.bFirstEnter = modifyCard.EditCard.IsFirstEnter;
                    cardInfo.bIsValid = modifyCard.EditCard.IsValid;
                    cardInfo.stuValidStartTime = modifyCard.EditCard.StartTime;
                    cardInfo.stuValidEndTime = modifyCard.EditCard.EndTime;
                    IntPtr updatePtr = IntPtr.Zero;
                    IntPtr inPtr = IntPtr.Zero;
                    try
                    {
                        updatePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD)));
                        Marshal.StructureToPtr(cardInfo, updatePtr, true);
                        inParam.pBuf = updatePtr;
                        inParam.nBufLen = Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                        inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)));
                        Marshal.StructureToPtr(inParam, inPtr, true);
                        bool res = NETClient.ControlDevice(_LoginID, EM_CtrlType.RECORDSET_UPDATEEX, inPtr, 10000);
                        if (!res)
                        {
                            MessageBox.Show("Modify card failed(修改卡失败)");
                            modifyCard.Dispose();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        modifyCard.Dispose();
                        return;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(updatePtr);
                        Marshal.FreeHGlobal(inPtr);
                    }
                    //mofidy face image 修改人脸图片
                    if (modifyCard.ImageData != null)
                    {
                        IntPtr inFacePtr = IntPtr.Zero;
                        IntPtr outFacePtr = IntPtr.Zero;
                        IntPtr facePtr = IntPtr.Zero;
                        try
                        {
                            NET_IN_UPDATE_FACE_INFO inUpdateFaceInfo = new NET_IN_UPDATE_FACE_INFO();
                            inUpdateFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_UPDATE_FACE_INFO));
                            inUpdateFaceInfo.szUserID = modifyCard.EditCard.UserID;
                            inUpdateFaceInfo.stuFaceInfo.szUserName = modifyCard.EditCard.CardName;
                            inUpdateFaceInfo.stuFaceInfo.nFacePhotoLen = new int[5];
                            inUpdateFaceInfo.stuFaceInfo.pszFacePhoto = new IntPtr[5];
                            inUpdateFaceInfo.stuFaceInfo.nFacePhoto = 1;
                            inUpdateFaceInfo.stuFaceInfo.nFacePhotoLen[0] = modifyCard.ImageData.Length;
                            facePtr = Marshal.AllocHGlobal(modifyCard.ImageData.Length);
                            Marshal.Copy(modifyCard.ImageData, 0, facePtr, modifyCard.ImageData.Length);
                            inUpdateFaceInfo.stuFaceInfo.pszFacePhoto[0] = facePtr;
                            inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_UPDATE_FACE_INFO)));
                            Marshal.StructureToPtr(inUpdateFaceInfo, inFacePtr, true);
                            NET_OUT_UPDATE_FACE_INFO outUpdateFaceInfo = new NET_OUT_UPDATE_FACE_INFO();
                            outUpdateFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_UPDATE_FACE_INFO));
                            outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_UPDATE_FACE_INFO)));
                            Marshal.StructureToPtr(outUpdateFaceInfo, outFacePtr, true);
                            bool result = NETClient.FaceInfoOpreate(_LoginID, EM_FACEINFO_OPREATE_TYPE.UPDATE, inFacePtr, outFacePtr, 5000);
                            if (!result)
                            {
                                MessageBox.Show("Modify face failed(修改人脸失败)");
                                modifyCard.Dispose();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            modifyCard.Dispose();
                            return;
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(facePtr);
                            Marshal.FreeHGlobal(inFacePtr);
                            Marshal.FreeHGlobal(outFacePtr);
                        }
                    }
                    MessageBox.Show("Modify successfully(修改成功)");
                }
                catch (Exception ex)
                {
                    modifyCard.Dispose();
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            modifyCard.Dispose();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (listView_card.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one card frist(请先选择一张卡)");
                return;
            }
            NET_RECORDSET_ACCESS_CTL_CARD card = (NET_RECORDSET_ACCESS_CTL_CARD)listView_card.SelectedItems[0].Tag;
            //delete face image 删除人脸图片
            IntPtr inFacePtr = IntPtr.Zero;
            IntPtr outFacePtr = IntPtr.Zero;
            try
            {
                NET_IN_REMOVE_FACE_INFO inRemoveFaceInfo = new NET_IN_REMOVE_FACE_INFO();
                inRemoveFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_REMOVE_FACE_INFO));
                inRemoveFaceInfo.szUserID = card.szUserID;
                NET_OUT_REMOVE_FACE_INFO outRemoveFaceInfo = new NET_OUT_REMOVE_FACE_INFO();
                outRemoveFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_REMOVE_FACE_INFO));
                inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_REMOVE_FACE_INFO)));
                Marshal.StructureToPtr(inRemoveFaceInfo, inFacePtr, true);
                outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_REMOVE_FACE_INFO)));
                Marshal.StructureToPtr(outRemoveFaceInfo, outFacePtr, true);
                bool result = NETClient.FaceInfoOpreate(_LoginID, EM_FACEINFO_OPREATE_TYPE.REMOVE, inFacePtr, outFacePtr, 5000);
                if (!result)
                {
                    MessageBox.Show("Delete face failed(删除人脸失败)");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Marshal.FreeHGlobal(inFacePtr);
                Marshal.FreeHGlobal(outFacePtr);
            }
            //delete card 删除卡
            NET_CTRL_RECORDSET_PARAM inParam = new NET_CTRL_RECORDSET_PARAM();
            inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM));
            inParam.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
            IntPtr recPtr = IntPtr.Zero;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                recPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
                Marshal.StructureToPtr(card.nRecNo, recPtr, true);
                inParam.pBuf = recPtr;
                inParam.nBufLen = Marshal.SizeOf(typeof(int));
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)));
                Marshal.StructureToPtr(inParam, inPtr, true);
                bool ret = NETClient.ControlDevice(_LoginID, EM_CtrlType.RECORDSET_REMOVE, inPtr, 10000);
                if (!ret)
                {
                    MessageBox.Show("Delete card failed(删除卡失败)");
                }
                else
                {
                    MessageBox.Show("Delete successfully(删除成功)");
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
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            //clear face image 清空人脸图片
            IntPtr inFacePtr = IntPtr.Zero;
            IntPtr outFacePtr = IntPtr.Zero;
            try
            {
                NET_IN_CLEAR_FACE_INFO inClearFaceInfo = new NET_IN_CLEAR_FACE_INFO();
                inClearFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_CLEAR_FACE_INFO));
                NET_OUT_CLEAR_FACE_INFO outClearFaceInfo = new NET_OUT_CLEAR_FACE_INFO();
                outClearFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_CLEAR_FACE_INFO));
                inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_CLEAR_FACE_INFO)));
                Marshal.StructureToPtr(inClearFaceInfo, inFacePtr, true);
                outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_CLEAR_FACE_INFO)));
                Marshal.StructureToPtr(outClearFaceInfo, outFacePtr, true);
                bool result = NETClient.FaceInfoOpreate(_LoginID, EM_FACEINFO_OPREATE_TYPE.CLEAR, inFacePtr, outFacePtr, 5000);
                if (!result)
                {
                    MessageBox.Show("Clear face failed(清空人脸失败)");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Marshal.FreeHGlobal(inFacePtr);
                Marshal.FreeHGlobal(outFacePtr);
            }
            //clear card 清空卡
            NET_CTRL_RECORDSET_PARAM inParam = new NET_CTRL_RECORDSET_PARAM();
            inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM));
            inParam.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)));
                Marshal.StructureToPtr(inParam, inPtr, true);
                bool ret = NETClient.ControlDevice(_LoginID, EM_CtrlType.RECORDSET_CLEAR, inPtr, 10000);
                if (!ret)
                {
                    MessageBox.Show("Clear card failed(清空卡失败)");
                }
                else
                {
                    MessageBox.Show("Clear successfully(清空成功)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(inPtr);
            }
        }

        private void textBox_cardno_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length > 31)
            {
                ((TextBox)sender).Text = _OldCardNo;
            }
            _OldCardNo = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

    }

    public class Card
    {
        public string CardNo { get; set; }
        public string UserID { get; set; }
        public string CardName { get; set; }
        public string CardPassword { get; set; }
        public EM_ACCESSCTLCARD_STATE CardStatus { get; set; }
        public EM_ACCESSCTLCARD_TYPE CardType { get; set; }
        public int UseTime { get; set; }
        public bool IsFirstEnter { get; set; }
        public bool IsValid { get; set; }
        public NET_TIME StartTime { get; set; }
        public NET_TIME EndTime { get; set; }
    }
}
