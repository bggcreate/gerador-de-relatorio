using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using NetSDKCS;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;

namespace VTODemo
{
    public partial class OperateManager : Form
    {
        IntPtr loginID = IntPtr.Zero;
        TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
        IntPtr findID = IntPtr.Zero;
        Task task;
        bool isCanncel = false;
        VTO mainWindow;
        string oldCardNo;

        public OperateManager(IntPtr id, VTO main)
        {
            InitializeComponent();
            loginID = id;
            mainWindow = main;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (task != null && task.IsCompleted == false)
            {
                isCanncel = true;
            }
            base.OnClosed(e);
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            this.button_search.Enabled = false;
            this.dataGridView_card.Rows.Clear();
            if (IntPtr.Zero != findID)
            {
                NETClient.FindRecordClose(findID);
                findID = IntPtr.Zero;
            }
            NET_FIND_RECORD_ACCESSCTLCARD_CONDITION condition = new NET_FIND_RECORD_ACCESSCTLCARD_CONDITION();
            condition.dwSize = (uint)Marshal.SizeOf(typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION));
            if (this.textBox_cardno.Text != "")
            {
                condition.abCardNo = true;
                condition.szCardNo = this.textBox_cardno.Text.Trim();
            }
            object obj = condition;
            bool ret = NETClient.FindRecord(loginID, EM_NET_RECORD_TYPE.ACCESSCTLCARD, obj, typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION), ref findID, 10000);
            if (!ret)
            {
                MessageBox.Show(NETClient.GetLastError());
                this.button_search.Enabled = true;
                return;
            }

            task = new Task(() =>
            {
                while (true)
                {
                    if (isCanncel)
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
                    NETClient.FindNextRecord(findID, max, ref retNum, ref ls, typeof(NET_RECORDSET_ACCESS_CTL_CARD), 10000);
                    if (isCanncel)
                    {
                        break;
                    }
                    foreach (var item in ls)
                    {
                        if (isCanncel)
                        {
                            break;
                        }
                        NET_RECORDSET_ACCESS_CTL_CARD info = (NET_RECORDSET_ACCESS_CTL_CARD)item;
                        IntPtr infoPtr = IntPtr.Zero;
                        try
                        {

                            info.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                            info.bEnableExtended = true;
                            info.stuFingerPrintInfoEx.nPacketLen = 2048;
                            info.stuFingerPrintInfoEx.pPacketData = Marshal.AllocHGlobal(2048);

                            NET_CTRL_RECORDSET_PARAM inp = new NET_CTRL_RECORDSET_PARAM();
                            inp.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM));
                            inp.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
                            infoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD)));
                            Marshal.StructureToPtr(info, infoPtr, true);
                            inp.pBuf = infoPtr;
                            inp.nBufLen = Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                            object objInp = inp;

                            bool re = NETClient.QueryDevState(loginID, (int)EM_DEVICE_STATE.DEV_RECORDSET_EX, ref objInp, typeof(NET_CTRL_RECORDSET_PARAM), 5000);
                            if (isCanncel)
                            {
                                break;
                            }
                            if (re)
                            {
                                inp = (NET_CTRL_RECORDSET_PARAM)objInp;
                                info = (NET_RECORDSET_ACCESS_CTL_CARD)Marshal.PtrToStructure(inp.pBuf, typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                                byte[] fpData = new byte[info.stuFingerPrintInfoEx.nRealPacketLen];
                                Marshal.Copy(info.stuFingerPrintInfoEx.pPacketData, fpData, 0, info.stuFingerPrintInfoEx.nRealPacketLen);
                                if (isCanncel)
                                {
                                    break;
                                }
                                this.BeginInvoke(new Action(() =>
                                {
                                    DataGridViewRow row = new DataGridViewRow();
                                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                                    cell1.Value = info.nRecNo.ToString();
                                    row.Cells.Add(cell1);
                                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                                    cell2.Value = info.szUserID;
                                    row.Cells.Add(cell2);
                                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                                    cell3.Value = info.szCardNo;
                                    row.Cells.Add(cell3);
                                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                                    cell4.Value = Convert.ToBase64String(fpData);
                                    row.Cells.Add(cell4);
                                    Card c = new Card();
                                    c.FingerprintData = fpData;
                                    c.Info = info;
                                    row.Tag = c;

                                    this.dataGridView_card.Rows.Add(row);
                                }));
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(info.stuFingerPrintInfoEx.pPacketData);
                            Marshal.FreeHGlobal(infoPtr);
                        }
                    }
                    if (retNum < max)
                    {
                        break;
                    }
                }
                if (isCanncel == false)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.button_search.Enabled = true;
                        this.dataGridView_card.ClearSelection();
                    }));
                }

            });
            task.Start();
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            OperateInfo operateInfo = new OperateInfo(loginID, Operate_Type.Add, new NET_RECORDSET_ACCESS_CTL_CARD(), mainWindow, null);
            var ret = operateInfo.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    // add card
                    NET_RECORDSET_ACCESS_CTL_CARD card = operateInfo.card;
                    NET_CTRL_RECORDSET_INSERT_PARAM inParam = new NET_CTRL_RECORDSET_INSERT_PARAM();
                    inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_PARAM));
                    inParam.stuCtrlRecordSetInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_IN));
                    inParam.stuCtrlRecordSetResult.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_INSERT_OUT));
                    inParam.stuCtrlRecordSetInfo.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
                    NET_RECORDSET_ACCESS_CTL_CARD cardInfo = new NET_RECORDSET_ACCESS_CTL_CARD();
                    cardInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                    cardInfo.szCardNo = card.szCardNo;
                    cardInfo.szUserID = card.szUserID;
                    cardInfo.nDoorNum = 1;
                    cardInfo.sznDoors = new int[32];
                    cardInfo.sznDoors[0] = 0;

                    cardInfo.bEnableExtended = true;
                    cardInfo.stuFingerPrintInfoEx.nCount = 1;
                    cardInfo.stuFingerPrintInfoEx.nLength = operateInfo.packetLen;
                    cardInfo.stuFingerPrintInfoEx.nPacketLen = operateInfo.packetLen;
                    cardInfo.stuFingerPrintInfoEx.pPacketData = Marshal.AllocHGlobal(cardInfo.stuFingerPrintInfoEx.nPacketLen);
                    Marshal.Copy(operateInfo.packetData, 0, cardInfo.stuFingerPrintInfoEx.pPacketData, cardInfo.stuFingerPrintInfoEx.nPacketLen);

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
                        bool res = NETClient.ControlDevice(loginID, EM_CtrlType.RECORDSET_INSERTEX, ptr, 10000);
                        if (!res)
                        {
                            MessageBox.Show("Add card failed(增加卡失败)");
                            operateInfo.Dispose();
                            return;
                        }
                        inParam = (NET_CTRL_RECORDSET_INSERT_PARAM)Marshal.PtrToStructure(ptr, typeof(NET_CTRL_RECORDSET_INSERT_PARAM));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        operateInfo.Dispose();
                        return;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(cardInfo.stuFingerPrintInfoEx.pPacketData);
                        Marshal.FreeHGlobal(inPtr);
                        Marshal.FreeHGlobal(ptr);
                    }
                    //add face image

                    // add face image 增加人脸图片
                    IntPtr inFacePtr = IntPtr.Zero;
                    IntPtr outFacePtr = IntPtr.Zero;
                    IntPtr facePtr = IntPtr.Zero;
                    try
                    {
                        NET_IN_ADD_FACE_INFO inAddFaceInfo = new NET_IN_ADD_FACE_INFO();
                        inAddFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_ADD_FACE_INFO));
                        inAddFaceInfo.szUserID = card.szUserID;
                        inAddFaceInfo.stuFaceInfo.nFacePhoto = 1;
                        inAddFaceInfo.stuFaceInfo.nFacePhotoLen = new int[5];
                        inAddFaceInfo.stuFaceInfo.pszFacePhoto = new IntPtr[5];
                        inAddFaceInfo.stuFaceInfo.nFacePhotoLen[0] = operateInfo.imageData.Length;
                        facePtr = Marshal.AllocHGlobal(operateInfo.imageData.Length);
                        Marshal.Copy(operateInfo.imageData, 0, facePtr, operateInfo.imageData.Length);
                        inAddFaceInfo.stuFaceInfo.pszFacePhoto[0] = facePtr;
                        inAddFaceInfo.stuFaceInfo.nRoom = 1;
                        inAddFaceInfo.stuFaceInfo.szRoomNo = new NET_ROOM[32];
                        inAddFaceInfo.stuFaceInfo.szRoomNo[0].szRoomNo = card.szUserID;

                        inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_ADD_FACE_INFO)));
                        Marshal.StructureToPtr(inAddFaceInfo, inFacePtr, true);

                        NET_OUT_ADD_FACE_INFO outAddFaceInfo = new NET_OUT_ADD_FACE_INFO();
                        outAddFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_ADD_FACE_INFO));
                        outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_ADD_FACE_INFO)));
                        Marshal.StructureToPtr(outAddFaceInfo, outFacePtr, true);
                        bool result = NETClient.FaceInfoOpreate(loginID, EM_FACEINFO_OPREATE_TYPE.ADD, inFacePtr, outFacePtr, 5000);
                        if (!result)
                        {
                            MessageBox.Show("Add face failed(增加人脸失败)");
                            operateInfo.Dispose();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        operateInfo.Dispose();
                        return;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(facePtr);
                        Marshal.FreeHGlobal(inFacePtr);
                        Marshal.FreeHGlobal(outFacePtr);
                    }
                    this.dataGridView_card.ClearSelection();
                    MessageBox.Show("Add successfully(增加成功)");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    operateInfo.Dispose();
                    return;
                }
            }
            operateInfo.Dispose();
        }

        private void button_modify_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_card.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select one card(请选择一张卡)");
                return;
            }
            Card modifyCard = (Card)this.dataGridView_card.SelectedRows[0].Tag;
            OperateInfo operateInfo = new OperateInfo(loginID, Operate_Type.Modify, modifyCard.Info, mainWindow, modifyCard.FingerprintData);
            var ret = operateInfo.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                //modify card
                NET_RECORDSET_ACCESS_CTL_CARD card = operateInfo.card;
                NET_CTRL_RECORDSET_PARAM inParam = new NET_CTRL_RECORDSET_PARAM();
                inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM));
                inParam.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
                NET_RECORDSET_ACCESS_CTL_CARD cardInfo = new NET_RECORDSET_ACCESS_CTL_CARD();
                cardInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                cardInfo.nDoorNum = 1;
                cardInfo.sznDoors = new int[32];
                cardInfo.sznDoors[0] = 0;
                cardInfo.szCardNo = card.szCardNo;
                cardInfo.szUserID = card.szUserID;

                cardInfo.bEnableExtended = true;
                cardInfo.stuFingerPrintInfoEx.nCount = 1;
                cardInfo.stuFingerPrintInfoEx.nLength = operateInfo.packetLen;
                cardInfo.stuFingerPrintInfoEx.nPacketLen = operateInfo.packetLen;
                cardInfo.stuFingerPrintInfoEx.pPacketData = Marshal.AllocHGlobal(cardInfo.stuFingerPrintInfoEx.nPacketLen);
                Marshal.Copy(operateInfo.packetData, 0, cardInfo.stuFingerPrintInfoEx.pPacketData, cardInfo.stuFingerPrintInfoEx.nPacketLen);

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
                    bool res = NETClient.ControlDevice(loginID, EM_CtrlType.RECORDSET_UPDATEEX, inPtr, 10000);
                    if (!res)
                    {
                        MessageBox.Show("Modify card failed(修改卡失败)");
                        operateInfo.Dispose();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    operateInfo.Dispose();
                    return;
                }
                finally
                {
                    Marshal.FreeHGlobal(cardInfo.stuFingerPrintInfoEx.pPacketData);
                    Marshal.FreeHGlobal(updatePtr);
                    Marshal.FreeHGlobal(inPtr);
                }

                //modify face image
                if (operateInfo.imageData != null)
                {
                    IntPtr inFacePtr = IntPtr.Zero;
                    IntPtr outFacePtr = IntPtr.Zero;
                    IntPtr facePtr = IntPtr.Zero;
                    try
                    {
                        NET_IN_UPDATE_FACE_INFO inUpdateFaceInfo = new NET_IN_UPDATE_FACE_INFO();
                        inUpdateFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_UPDATE_FACE_INFO));
                        inUpdateFaceInfo.szUserID = card.szUserID;
                        inUpdateFaceInfo.stuFaceInfo.nFacePhotoLen = new int[5];
                        inUpdateFaceInfo.stuFaceInfo.pszFacePhoto = new IntPtr[5];
                        inUpdateFaceInfo.stuFaceInfo.nFacePhoto = 1;
                        inUpdateFaceInfo.stuFaceInfo.nFacePhotoLen[0] = operateInfo.imageData.Length;
                        facePtr = Marshal.AllocHGlobal(operateInfo.imageData.Length);
                        Marshal.Copy(operateInfo.imageData, 0, facePtr, operateInfo.imageData.Length);
                        inUpdateFaceInfo.stuFaceInfo.pszFacePhoto[0] = facePtr;
                        inUpdateFaceInfo.stuFaceInfo.nRoom = 1;
                        inUpdateFaceInfo.stuFaceInfo.szRoomNo = new NET_ROOM[32];
                        inUpdateFaceInfo.stuFaceInfo.szRoomNo[0].szRoomNo = card.szUserID;

                        inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_UPDATE_FACE_INFO)));
                        Marshal.StructureToPtr(inUpdateFaceInfo, inFacePtr, true);
                        NET_OUT_UPDATE_FACE_INFO outUpdateFaceInfo = new NET_OUT_UPDATE_FACE_INFO();
                        outUpdateFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_UPDATE_FACE_INFO));
                        outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_UPDATE_FACE_INFO)));
                        Marshal.StructureToPtr(outUpdateFaceInfo, outFacePtr, true);
                        bool result = NETClient.FaceInfoOpreate(loginID, EM_FACEINFO_OPREATE_TYPE.UPDATE, inFacePtr, outFacePtr, 5000);
                        if (!result)
                        {
                            MessageBox.Show("Modify face failed(修改人脸失败)");
                            operateInfo.Dispose();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        operateInfo.Dispose();
                        return;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(facePtr);
                        Marshal.FreeHGlobal(inFacePtr);
                        Marshal.FreeHGlobal(outFacePtr);
                    }
                }
                this.dataGridView_card.ClearSelection();
                MessageBox.Show("Modify successfully(修改成功)");
            }
            operateInfo.Dispose();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_card.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select one card(请选择一张卡)");
                return;
            }
            Card deleteCard = (Card)this.dataGridView_card.SelectedRows[0].Tag;
            //delete card
            NET_CTRL_RECORDSET_PARAM inParam = new NET_CTRL_RECORDSET_PARAM();
            inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM));
            inParam.emType = EM_NET_RECORD_TYPE.ACCESSCTLCARD;
            IntPtr recPtr = IntPtr.Zero;
            IntPtr inPtr = IntPtr.Zero;
            try
            {
                recPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
                Marshal.StructureToPtr(deleteCard.Info.nRecNo, recPtr, true);
                inParam.pBuf = recPtr;
                inParam.nBufLen = Marshal.SizeOf(typeof(int));
                inPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_RECORDSET_PARAM)));
                Marshal.StructureToPtr(inParam, inPtr, true);
                bool ret = NETClient.ControlDevice(loginID, EM_CtrlType.RECORDSET_REMOVE, inPtr, 10000);
                if (!ret)
                {
                    MessageBox.Show("Delete card failed(删除卡失败)");
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
                Marshal.FreeHGlobal(recPtr);
                Marshal.FreeHGlobal(inPtr);
            }

            //delete face image
            IntPtr inFacePtr = IntPtr.Zero;
            IntPtr outFacePtr = IntPtr.Zero;
            try
            {
                NET_IN_REMOVE_FACE_INFO inRemoveFaceInfo = new NET_IN_REMOVE_FACE_INFO();
                inRemoveFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_REMOVE_FACE_INFO));
                inRemoveFaceInfo.szUserID = deleteCard.Info.szUserID;
                NET_OUT_REMOVE_FACE_INFO outRemoveFaceInfo = new NET_OUT_REMOVE_FACE_INFO();
                outRemoveFaceInfo.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_REMOVE_FACE_INFO));
                inFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_IN_REMOVE_FACE_INFO)));
                Marshal.StructureToPtr(inRemoveFaceInfo, inFacePtr, true);
                outFacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_OUT_REMOVE_FACE_INFO)));
                Marshal.StructureToPtr(outRemoveFaceInfo, outFacePtr, true);
                bool result = NETClient.FaceInfoOpreate(loginID, EM_FACEINFO_OPREATE_TYPE.REMOVE, inFacePtr, outFacePtr, 5000);
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
            this.dataGridView_card.ClearSelection();
            MessageBox.Show("Delete successfully(删除成功)");
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
                bool result = NETClient.FaceInfoOpreate(loginID, EM_FACEINFO_OPREATE_TYPE.CLEAR, inFacePtr, outFacePtr, 5000);
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
                bool ret = NETClient.ControlDevice(loginID, EM_CtrlType.RECORDSET_CLEAR, inPtr, 10000);
                if (!ret)
                {
                    MessageBox.Show("Clear card failed(清空卡失败)");
                }
                else
                {
                    this.dataGridView_card.ClearSelection();
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
                ((TextBox)sender).Text = oldCardNo;
            }
            oldCardNo = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

    }

    public enum Operate_Type
    {
        Add,
        Modify,
    }

    public class Card
    {
        public NET_RECORDSET_ACCESS_CTL_CARD Info { get; set; }
        public byte[] FingerprintData { get; set; }
    }
}
