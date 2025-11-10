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

namespace Attendance
{
    public partial class OperateFingerprintByUserID : Form
    {
        string _UserID;
        IntPtr _LoginID;
        int _Count = 0;
        int[] _FingerPrintIDs;
        public OperateFingerprintByUserID(IntPtr loginID,string userid)
        {
            InitializeComponent();
            _LoginID = loginID;
            _UserID = userid;
            this.label_userid.Text = userid;
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            _Count = 0;
            this.dataGridView1.Rows.Clear();
            _FingerPrintIDs = null;
            NET_IN_FINGERPRINT_GETBYUSER inFingerPrint = new NET_IN_FINGERPRINT_GETBYUSER();
            NET_OUT_FINGERPRINT_GETBYUSER outFingerPrint = new NET_OUT_FINGERPRINT_GETBYUSER();
            try
            {
                inFingerPrint.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_FINGERPRINT_GETBYUSER));
                inFingerPrint.szUserID = _UserID;
                outFingerPrint.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_FINGERPRINT_GETBYUSER));
                outFingerPrint.nMaxFingerDataLength = 100 * 1024;
                outFingerPrint.pbyFingerData = Marshal.AllocHGlobal(outFingerPrint.nMaxFingerDataLength);
                bool ret = NETClient.Attendance_GetFingerByUserID(_LoginID, ref inFingerPrint, ref outFingerPrint, 5000);
                if (ret)
                {
                    if (outFingerPrint.nRetFingerPrintCount == 0)
                    {
                        MessageBox.Show("No fingerprint data(没有指纹数据)");
                    }
                    else
                    {
                        _FingerPrintIDs = outFingerPrint.nFingerPrintIDs;
                        for (int i = 0; i < outFingerPrint.nRetFingerPrintCount; i++)
                        {
                            _Count++;
                            DataGridViewRow row = new DataGridViewRow();
                            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                            cell1.Value = _Count.ToString();
                            row.Cells.Add(cell1);
                            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                            cell2.Value = _UserID;
                            row.Cells.Add(cell2);
                            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                            cell3.Value = outFingerPrint.nFingerPrintIDs[i].ToString();
                            row.Cells.Add(cell3);
                            DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                            byte[] data = new byte[outFingerPrint.nSinglePacketLength];
                            Marshal.Copy(IntPtr.Add(outFingerPrint.pbyFingerData, i * outFingerPrint.nSinglePacketLength), data, 0, outFingerPrint.nSinglePacketLength);
                            string text = Convert.ToBase64String(data);
                            cell4.Value = text;
                            row.Cells.Add(cell4);

                            this.dataGridView1.Rows.Add(row);
                        }
                        this.dataGridView1.ClearSelection();
                    }
                }
                else
                {
                    MessageBox.Show("No fingerprint data(没有指纹数据)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(outFingerPrint.pbyFingerData);
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
            FringerPrintCollection fpc = new FringerPrintCollection(_LoginID);
            var ret = fpc.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                NET_IN_FINGERPRINT_INSERT_BY_USERID inParam = new NET_IN_FINGERPRINT_INSERT_BY_USERID();
                try
                {
                    inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_IN_FINGERPRINT_INSERT_BY_USERID));
                    inParam.szUserID = _UserID;
                    inParam.nPacketCount = 1;
                    inParam.nSinglePacketLen = fpc.PacketLen;
                    inParam.szFingerPrintInfo = Marshal.AllocHGlobal(fpc.PacketLen);
                    Marshal.Copy(fpc.FingerPrintInfo, 0, inParam.szFingerPrintInfo, fpc.PacketLen);
                    NET_OUT_FINGERPRINT_INSERT_BY_USERID outParam = new NET_OUT_FINGERPRINT_INSERT_BY_USERID();
                    outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_OUT_FINGERPRINT_INSERT_BY_USERID));

                    bool res = NETClient.Attendance_InsertFingerByUserID(_LoginID, ref inParam, ref outParam, 5000);
                    if (res)
                    {
                        MessageBox.Show("Add FingerPrint Successfully(增加指纹成功)");
                    }
                    else
                    {
                        MessageBox.Show("Add FingerPrint Failed(增加指纹失败)");
                    }
                }
                catch (Exception ex)
                {
                    fpc.Dispose();
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Marshal.FreeHGlobal(inParam.szFingerPrintInfo);
                }
            }
            fpc.Dispose();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
            NET_CTRL_IN_FINGERPRINT_REMOVE_BY_USERID inParam = new NET_CTRL_IN_FINGERPRINT_REMOVE_BY_USERID();
            NET_CTRL_OUT_FINGERPRINT_REMOVE_BY_USERID outParam = new NET_CTRL_OUT_FINGERPRINT_REMOVE_BY_USERID();
            inParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_IN_FINGERPRINT_REMOVE_BY_USERID));
            inParam.szUserID = _UserID;
            outParam.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_OUT_FINGERPRINT_REMOVE_BY_USERID));
            bool ret = NETClient.Attendance_RemoveFingerByUserID(_LoginID, ref inParam, ref outParam, 5000);
            if (!ret)
            {
                MessageBox.Show("Delete Fingerprint Failed(删除指纹失败)");
                return;
            }
            MessageBox.Show("Delete Fingerprint Successfully(删除指纹成功)");
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

    }
}
