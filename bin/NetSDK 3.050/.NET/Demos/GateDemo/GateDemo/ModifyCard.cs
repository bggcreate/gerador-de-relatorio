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

namespace GateDemo
{
    public partial class ModifyCard : Form
    {
        public Card EditCard { get; set; }
        public byte[] ImageData { get; set; }
        TextInfo _TextInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
        string _OldCardName = "";
        string _OldCardNo = "";
        string _OldPassword = "";

        public ModifyCard(NET_RECORDSET_ACCESS_CTL_CARD card)
        {
            InitializeComponent();
            EditCard = new Card();
            EditCard.CardNo = card.szCardNo;
            EditCard.CardName = card.szCardName;
            EditCard.UserID = card.szUserID;
            EditCard.CardPassword = card.szPsw;
            EditCard.CardStatus = card.emStatus;
            EditCard.CardType = card.emType;
            EditCard.UseTime = card.nUseTime;
            EditCard.IsFirstEnter = card.bFirstEnter;
            EditCard.IsValid = card.bIsValid;
            EditCard.StartTime = card.stuValidStartTime;
            EditCard.EndTime = card.stuValidEndTime;
            this.Load += new EventHandler(ModifyCard_Load);
        }

        void ModifyCard_Load(object sender, EventArgs e)
        {
            string[] cardStatus = Enum.GetNames(typeof(EM_ACCESSCTLCARD_STATE));
            string statusString = Enum.GetName(typeof(EM_ACCESSCTLCARD_STATE), EditCard.CardStatus);
            int statusIndex = 7;
            foreach (string item in cardStatus)
            {
                if (item == statusString)
                {
                    statusIndex = cardStatus.ToList().IndexOf(item);
                }
                string chn = "";
                if (cardStatus.ToList().IndexOf(item) == 0)
                {
                    chn = "(正常)";
                }
                if (cardStatus.ToList().IndexOf(item) == 1)
                {
                    chn = "(挂失)";
                }
                if (cardStatus.ToList().IndexOf(item) == 2)
                {
                    chn = "(注销)";
                }
                if (cardStatus.ToList().IndexOf(item) == 3)
                {
                    chn = "(冻结)";
                }
                if (cardStatus.ToList().IndexOf(item) == 4)
                {
                    chn = "(欠费)";
                }
                if (cardStatus.ToList().IndexOf(item) == 5)
                {
                    chn = "(逾期)";
                }
                if (cardStatus.ToList().IndexOf(item) == 6)
                {
                    chn = "(预欠费)";
                }
                if (cardStatus.ToList().IndexOf(item) == 7)
                {
                    chn = "(未知)";
                }
                comboBox_cardstatus.Items.Add(_TextInfo.ToTitleCase(item.ToLower()) + chn);
            }
            comboBox_cardstatus.SelectedIndex = statusIndex;
            
            string[] cardType = Enum.GetNames(typeof(EM_ACCESSCTLCARD_TYPE));
            string typeString = Enum.GetName(typeof(EM_ACCESSCTLCARD_TYPE), EditCard.CardType);
            int typeIndex = 7;
            foreach (string item in cardType)
            {
                if (item == typeString)
                {
                    typeIndex = cardType.ToList().IndexOf(item);
                }
                string chn = "";
                if (cardType.ToList().IndexOf(item) == 0)
                {
                    chn = "(一般卡)";
                }
                if (cardType.ToList().IndexOf(item) == 1)
                {
                    chn = "(贵宾卡)";
                }
                if (cardType.ToList().IndexOf(item) == 2)
                {
                    chn = "(来宾卡)";
                }
                if (cardType.ToList().IndexOf(item) == 3)
                {
                    chn = "(巡逻卡)";
                }
                if (cardType.ToList().IndexOf(item) == 4)
                {
                    chn = "(黑名单卡)";
                }
                if (cardType.ToList().IndexOf(item) == 5)
                {
                    chn = "(胁迫卡)";
                }
                if (cardType.ToList().IndexOf(item) == 6)
                {
                    chn = "(巡检卡)";
                }
                if (cardType.ToList().IndexOf(item) == 7)
                {
                    chn = "(母卡)";
                }
                if (cardType.ToList().IndexOf(item) == 8)
                {
                    chn = "(未知)";
                }
                comboBox_cardtype.Items.Add(_TextInfo.ToTitleCase(item.ToLower()) + chn);
            }
            comboBox_cardtype.SelectedIndex = typeIndex;
            this.button_delete.Visible = false;
            this.textBox_cardno.Text = EditCard.CardNo;
            this.textBox_userid.Text = EditCard.UserID;
            this.textBox_cardname.Text = EditCard.CardName;
            this.textBox_cardpassword.Text = EditCard.CardPassword;
            this.textBox_usetime.Text = EditCard.UseTime.ToString();
            this.checkBox_isfirstcard.Checked = EditCard.IsFirstEnter;
            this.checkBox_valid.Checked = EditCard.IsValid;
            this.dateTimePicker_start.Value = EditCard.StartTime.ToDateTime();
            this.dateTimePicker_end.Value = EditCard.EndTime.ToDateTime();
        }

        private void textBox_usertime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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
                    ImageData = File.ReadAllBytes(path);
                    using (MemoryStream stream = new MemoryStream(ImageData))
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
            ImageData = null;
            this.pictureBox_face.Image = null;
            this.pictureBox_face.Refresh();
            this.button_open.Visible = true;
            this.button_delete.Visible = false;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (this.textBox_cardno.Text == "")
            {
                MessageBox.Show("Please input cardNo.(请输入卡编号)");
                return;
            }
            if (this.textBox_userid.Text == "")
            {
                MessageBox.Show("Please input userID.(请输入用户序号)");
                return;
            }
            if (this.textBox_cardname.Text == "")
            {
                MessageBox.Show("Please input cardName.(请输入卡名)");
                return;
            }
            int times = 0;
            try
            {
                times = Convert.ToInt32(this.textBox_usetime.Text.Trim());
            }
            catch
            {
                MessageBox.Show("The usetime's value must be 0 - 2147483647(使用次数的值必须是0-2147483647)");
                return;
            }
            EditCard.CardNo = this.textBox_cardno.Text.Trim();
            EditCard.UserID = this.textBox_userid.Text.Trim();
            EditCard.CardName = this.textBox_cardname.Text.Trim();
            EditCard.CardPassword = this.textBox_cardpassword.Text;
            EditCard.CardStatus = (EM_ACCESSCTLCARD_STATE)Enum.Parse(typeof(EM_ACCESSCTLCARD_STATE), comboBox_cardstatus.SelectedItem.ToString().Substring(0, comboBox_cardstatus.SelectedItem.ToString().IndexOf('(')).ToUpper(), false);
            EditCard.CardType = (EM_ACCESSCTLCARD_TYPE)Enum.Parse(typeof(EM_ACCESSCTLCARD_TYPE), comboBox_cardtype.SelectedItem.ToString().Substring(0, comboBox_cardtype.SelectedItem.ToString().IndexOf('(')).ToUpper(), false);
            EditCard.UseTime = times;
            EditCard.IsFirstEnter = this.checkBox_isfirstcard.Checked;
            EditCard.IsValid = this.checkBox_valid.Checked;
            EditCard.StartTime = NET_TIME.FromDateTime(this.dateTimePicker_start.Value);
            EditCard.EndTime = NET_TIME.FromDateTime(this.dateTimePicker_end.Value);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
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

        private void textBox_cardname_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length > 63)
            {
                ((TextBox)sender).Text = _OldCardName;
            }
            _OldCardName = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

        private void textBox_cardpassword_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length > 63)
            {
                ((TextBox)sender).Text = _OldPassword;
            }
            _OldPassword = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
        }

    }
}
