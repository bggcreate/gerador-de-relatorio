using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSDKCS;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace VTODemo
{
    public partial class OperateInfo : Form
    {
        VTO mainWindow;
        Operate_Type type;
        public NET_RECORDSET_ACCESS_CTL_CARD card;
        public int packetLen = 0;
        public byte[] packetData;
        public byte[] imageData;
        string oldCardNo = "";
        string oldRoomNo = "";
        IntPtr loginID = IntPtr.Zero;

        public OperateInfo(IntPtr id, Operate_Type t, NET_RECORDSET_ACCESS_CTL_CARD c, VTO main, byte[] fingerprintdata)
        {
            InitializeComponent();
            loginID = id;
            mainWindow = main;
            type = t;
            card = c;
            packetLen = c.stuFingerPrintInfoEx.nRealPacketLen;
            packetData = fingerprintdata;
            this.label_isGet.Text = "";
            switch (type)
            {
                case Operate_Type.Add:
                    {
                        this.button_delete.Visible = false;
                        this.Text = "Add Info(增加信息)";
                    }
                    break;
                case Operate_Type.Modify:
                    {
                        this.button_delete.Visible = false;
                        this.textBox_cardno.Text = card.szCardNo;
                        this.textBox_roomNo.Text = card.szUserID;
                        this.textBox_roomNo.ReadOnly = true;
                        this.textBox_cardno.ReadOnly = true;
                        this.Text = "Modify Info(修改信息)";
                    }
                    break;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (this.textBox_cardno.Text == "")
            {
                MessageBox.Show("Please input cardNo.(请输入卡号)");
                return;
            }
            if (this.textBox_roomNo.Text == "")
            {
                MessageBox.Show("Please input roomNo.(请输入房间号)");
                return;
            }
            switch (type)
            {
                case Operate_Type.Add:
                    {
                        if (packetLen == 0 || packetData == null)
                        {
                            MessageBox.Show("Please input fingerprint(请输入指纹)");
                            return;
                        }
                        if (imageData == null)
                        {
                            MessageBox.Show("Please select one picture(请选择一张图片)");
                            return;
                        }
                        IntPtr findID = IntPtr.Zero;
                        NET_FIND_RECORD_ACCESSCTLCARD_CONDITION condition = new NET_FIND_RECORD_ACCESSCTLCARD_CONDITION();
                        condition.dwSize = (uint)Marshal.SizeOf(typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION));
                        condition.abCardNo = true;
                        condition.szCardNo = this.textBox_cardno.Text.Trim();
                        object obj = condition;
                        bool ret = NETClient.FindRecord(loginID, EM_NET_RECORD_TYPE.ACCESSCTLCARD, obj, typeof(NET_FIND_RECORD_ACCESSCTLCARD_CONDITION), ref findID, 10000);
                        if (!ret)
                        {
                            MessageBox.Show("Check if the cardNo exists failed(检查卡号是否存在失败)");
                            return;
                        }
                        else
                        {
                            int max = 1;
                            int retNum = 0;
                            List<object> ls = new List<object>();
                            NET_RECORDSET_ACCESS_CTL_CARD card1 = new NET_RECORDSET_ACCESS_CTL_CARD();
                            card1.dwSize = (uint)Marshal.SizeOf(typeof(NET_RECORDSET_ACCESS_CTL_CARD));
                            ls.Add(card1);
                            NETClient.FindNextRecord(findID, max, ref retNum, ref ls, typeof(NET_RECORDSET_ACCESS_CTL_CARD), 10000);
                            if (retNum != 0)
                            {
                                MessageBox.Show("The cardNo already exists(卡号已存在)");
                                return;
                            }
                            else
                            {
                                card = new NET_RECORDSET_ACCESS_CTL_CARD();
                                card.szCardNo = this.textBox_cardno.Text.Trim();
                                card.szUserID = this.textBox_roomNo.Text.Trim();
                            }
                        }
                    }
                    break;
                case Operate_Type.Modify:
                    card.szCardNo = this.textBox_cardno.Text.Trim();
                    card.szUserID = this.textBox_roomNo.Text.Trim();
                    break;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button_get_Click(object sender, EventArgs e)
        {
            ColltectionFingerPrint cfp = new ColltectionFingerPrint(mainWindow);
            var ret = cfp.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                if (cfp.PacketLen > 0 && cfp.PactetData != null)
                {
                    packetLen = cfp.PacketLen;
                    packetData = cfp.PactetData;
                    this.label_isGet.Text = "Has get fingerprint(已经获取到指纹)";
                }
            }
            cfp.Dispose();
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG|*.jpg";
            var ret = openFileDialog.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string path = openFileDialog.FileName;
                    imageData = File.ReadAllBytes(path);
                    using (MemoryStream stream = new MemoryStream(imageData))
                    {
                        Image image = Image.FromStream(stream);
                        this.pictureBox_face.Image = image;
                        this.pictureBox_face.Refresh();
                        this.button_open.Visible = false;
                        this.button_delete.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            openFileDialog.Dispose();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            this.imageData = null;
            this.pictureBox_face.Image = null;
            this.pictureBox_face.Refresh();
            this.button_delete.Visible = false;
            this.button_open.Visible = true;
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

        private void textBox_roomNo_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length > 31)
            {
                ((TextBox)sender).Text = oldRoomNo;
            }
            oldRoomNo = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }
    }
}
