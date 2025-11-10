using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using NetSDKCS;
using System.IO;

namespace GateDemo
{
    public partial class AddCard : Form
    {
        public Card NewCard { get; set; }
        public byte[] ImageData { get; set; }
        TextInfo _TextInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
        string _OldUserID = "";
        string _OldCardName = "";
        string _OldCardNo = "";
        string _OldPassword = "";

        public AddCard()
        {
            InitializeComponent();
            this.Load += new EventHandler(AddCard_Load);
        }

        void AddCard_Load(object sender, EventArgs e)
        {
            string[] cardStatus = Enum.GetNames(typeof(EM_ACCESSCTLCARD_STATE));
            foreach (string item in cardStatus)
            {
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
            
            string[] cardType = Enum.GetNames(typeof(EM_ACCESSCTLCARD_TYPE));
            foreach (string item in cardType)
            {
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
            comboBox_cardstatus.SelectedIndex = 0;
            comboBox_cardtype.SelectedIndex = 0;
            this.dateTimePicker_end.Value = DateTime.Now.AddYears(1);
            this.button_delete.Visible = false;
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
                catch(Exception ex)
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
            if (ImageData == null)
            {
                MessageBox.Show("Please upload picture(请上传图片)");
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
            NewCard = new Card();
            NewCard.CardNo = this.textBox_cardno.Text.Trim();
            NewCard.UserID = this.textBox_userid.Text.Trim();
            NewCard.CardName = this.textBox_cardname.Text.Trim();
            NewCard.CardPassword = this.textBox_cardpassword.Text;
            NewCard.CardStatus = (EM_ACCESSCTLCARD_STATE)Enum.Parse(typeof(EM_ACCESSCTLCARD_STATE), comboBox_cardstatus.SelectedItem.ToString().Substring(0, comboBox_cardstatus.SelectedItem.ToString().IndexOf('(')).ToUpper(), false);
            NewCard.CardType = (EM_ACCESSCTLCARD_TYPE)Enum.Parse(typeof(EM_ACCESSCTLCARD_TYPE), comboBox_cardtype.SelectedItem.ToString().Substring(0, comboBox_cardtype.SelectedItem.ToString().IndexOf('(')).ToUpper(), false);
            NewCard.UseTime = times;
            NewCard.IsFirstEnter = this.checkBox_isfirstcard.Checked;
            NewCard.IsValid = this.checkBox_valid.Checked;
            NewCard.StartTime = NET_TIME.FromDateTime(this.dateTimePicker_start.Value);
            NewCard.EndTime = NET_TIME.FromDateTime(this.dateTimePicker_end.Value);
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

        private void textBox_userid_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length > 31)
            {
                ((TextBox)sender).Text = _OldUserID;
            }
            _OldUserID = ((TextBox)sender).Text;
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
