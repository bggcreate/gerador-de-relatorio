using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSDKCS;
using System.Text.RegularExpressions;

namespace Attendance
{
    public partial class UserInfomation : Form
    {
        string _OldUserID = "";
        string _OldUserName = "";
        string _OldCardNo = "";

        Operate_Type _Type;
        public NET_ATTENDANCE_USERINFO Data { get; set; }
        public UserInfomation(Operate_Type type, NET_ATTENDANCE_USERINFO data)
        {
            InitializeComponent();
            _Type = type;
            Data = data;
            switch (type)
            {
                case Operate_Type.Add:
                    this.Text = "Add User(增加用户)";
                    break;
                case Operate_Type.Modify:
                    this.Text = "Modify User(修改用户)";
                    this.textBox_userid.ReadOnly = true;
                    this.textBox_userid.Text = data.szUserID;
                    this.textBox_username.Text = data.szUserName;
                    this.textBox_cardno.Text = data.szCardNo;
                    break;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (this.textBox_userid.Text == "")
            {
                MessageBox.Show("Please input userid(请输入用户ID)");
                return;
            }
            switch (_Type)
            {
                case Operate_Type.Add:
                    {
                        NET_ATTENDANCE_USERINFO info = new NET_ATTENDANCE_USERINFO();
                        info.szUserID = this.textBox_userid.Text.Trim();
                        info.szUserName = this.textBox_username.Text.Trim();
                        info.szCardNo = this.textBox_cardno.Text.Trim();
                        Data = info;
                    }
                    break;
                case Operate_Type.Modify:
                    {
                        NET_ATTENDANCE_USERINFO info = new NET_ATTENDANCE_USERINFO();
                        info.szUserID = this.textBox_userid.Text.Trim();
                        info.szUserName = this.textBox_username.Text.Trim();
                        info.szCardNo = this.textBox_cardno.Text.Trim();
                        info.nPhotoLength = Data.nPhotoLength;
                        info.szPassword = Data.szPassword;
                        Data = info;
                    }
                    break;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
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

        private void textBox_username_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.UTF8.GetBytes(((TextBox)sender).Text).Length > 35)
            {
                ((TextBox)sender).Text = _OldUserName;
            }
            _OldUserName = ((TextBox)sender).Text;
            ((TextBox)sender).Select(((TextBox)sender).Text.Length, 0);
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

    public enum Operate_Type
    {
        Add,
        Modify,
    }
}
