using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DeviceInit
{
    public partial class InitDeviceDialog : Form
    {
        public string UserName { get; private set; }
        public string Passwrod { get; private set; }
        public string RestStr { get; private set; }
        public bool IsEmail { get; private set; }
        public InitDeviceDialog(Device dev)
        {
            InitializeComponent();
            if ((dev.DeviceInfo.byPwdResetWay >> 1& 0x1) == 1)
            {
                label4.Text = "E-mail(邮箱):";
                IsEmail = true;
            }
            else
            {
                label4.Text = "Phone Number(手机号):";
                IsEmail = false;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (this.textBox_name.Text == "")
            {
                MessageBox.Show(this,"Please input username(请输入用户名)");
                return;
            }
            if (this.textBox_password.Text == "")
            {
                MessageBox.Show(this,"Please input password(请输入密码)");
                return;
            }
            if (this.textBox_confirm.Text == "")
            {
                MessageBox.Show(this,"Please input confirm password(请输入确认密码)");
                return;
            }
            if (this.textBox_rest.Text == "")
            {
                if (IsEmail)
                {
                    MessageBox.Show(this,"Please input E-mail(请输入邮箱)");
                    return;
                }
                else
                {
                    MessageBox.Show(this,"Please input Phone number(请输入手机号码)");
                    return;
                }
            }
            if (textBox_password.Text != textBox_confirm.Text)
            {
                MessageBox.Show(this,"The password inputed twice is inconsistent,please input again(二次输入的密码不一致，请重新输入)");
                return;
            }
            UserName = this.textBox_name.Text.Trim();
            Passwrod = this.textBox_password.Text;
            RestStr = this.textBox_rest.Text.Trim();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
