namespace DeviceControl
{
    partial class DeviceControl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_login = new System.Windows.Forms.Button();
            this.textBox_passwrod = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBox_now = new System.Windows.Forms.CheckBox();
            this.button_set = new System.Windows.Forms.Button();
            this.button_get = new System.Windows.Forms.Button();
            this.dateTimePicker_set = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_get = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_reboot = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_login);
            this.groupBox1.Controls.Add(this.textBox_passwrod);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_username);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_port);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_ip);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(585, 143);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(登录设备)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(222, 104);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(120, 23);
            this.button_login.TabIndex = 8;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_passwrod
            // 
            this.textBox_passwrod.Location = new System.Drawing.Point(385, 69);
            this.textBox_passwrod.Name = "textBox_passwrod";
            this.textBox_passwrod.PasswordChar = '*';
            this.textBox_passwrod.Size = new System.Drawing.Size(100, 21);
            this.textBox_passwrod.TabIndex = 7;
            this.textBox_passwrod.Text = "admin123";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(290, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Password(密码):";
            // 
            // textBox_username
            // 
            this.textBox_username.Location = new System.Drawing.Point(183, 69);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(100, 21);
            this.textBox_username.TabIndex = 5;
            this.textBox_username.Text = "admin";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(74, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "UserName(用户名):";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(385, 28);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(100, 21);
            this.textBox_port.TabIndex = 3;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(314, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port(端口):";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(183, 28);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(100, 21);
            this.textBox_ip.TabIndex = 1;
            this.textBox_ip.Text = "172.23.12.11";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(110, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备IP):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(13, 162);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(585, 129);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operate(操作)";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBox_now);
            this.groupBox4.Controls.Add(this.button_set);
            this.groupBox4.Controls.Add(this.button_get);
            this.groupBox4.Controls.Add(this.dateTimePicker_set);
            this.groupBox4.Controls.Add(this.dateTimePicker_get);
            this.groupBox4.Location = new System.Drawing.Point(181, 21);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(397, 96);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Check Time(校时)";
            // 
            // checkBox_now
            // 
            this.checkBox_now.AutoSize = true;
            this.checkBox_now.Location = new System.Drawing.Point(8, 63);
            this.checkBox_now.Name = "checkBox_now";
            this.checkBox_now.Size = new System.Drawing.Size(126, 16);
            this.checkBox_now.TabIndex = 4;
            this.checkBox_now.Text = "NowTime(当前时间)";
            this.checkBox_now.UseVisualStyleBackColor = true;
            this.checkBox_now.CheckedChanged += new System.EventHandler(this.checkBox_now_CheckedChanged);
            // 
            // button_set
            // 
            this.button_set.Location = new System.Drawing.Point(310, 57);
            this.button_set.Name = "button_set";
            this.button_set.Size = new System.Drawing.Size(75, 23);
            this.button_set.TabIndex = 3;
            this.button_set.Text = "Set(设置)";
            this.button_set.UseVisualStyleBackColor = true;
            this.button_set.Click += new System.EventHandler(this.button_set_Click);
            // 
            // button_get
            // 
            this.button_get.Location = new System.Drawing.Point(310, 21);
            this.button_get.Name = "button_get";
            this.button_get.Size = new System.Drawing.Size(75, 23);
            this.button_get.TabIndex = 2;
            this.button_get.Text = "Get(获取)";
            this.button_get.UseVisualStyleBackColor = true;
            this.button_get.Click += new System.EventHandler(this.button_get_Click);
            // 
            // dateTimePicker_set
            // 
            this.dateTimePicker_set.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker_set.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_set.Location = new System.Drawing.Point(140, 59);
            this.dateTimePicker_set.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_set.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_set.Name = "dateTimePicker_set";
            this.dateTimePicker_set.Size = new System.Drawing.Size(151, 21);
            this.dateTimePicker_set.TabIndex = 1;
            // 
            // dateTimePicker_get
            // 
            this.dateTimePicker_get.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker_get.Enabled = false;
            this.dateTimePicker_get.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_get.Location = new System.Drawing.Point(140, 20);
            this.dateTimePicker_get.Name = "dateTimePicker_get";
            this.dateTimePicker_get.Size = new System.Drawing.Size(151, 21);
            this.dateTimePicker_get.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_reboot);
            this.groupBox3.Location = new System.Drawing.Point(7, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(168, 96);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Device Reboot(设备重启)";
            // 
            // button_reboot
            // 
            this.button_reboot.Location = new System.Drawing.Point(26, 39);
            this.button_reboot.Name = "button_reboot";
            this.button_reboot.Size = new System.Drawing.Size(115, 23);
            this.button_reboot.TabIndex = 0;
            this.button_reboot.Text = "Reboot(重启)";
            this.button_reboot.UseVisualStyleBackColor = true;
            this.button_reboot.Click += new System.EventHandler(this.button_reboot_Click);
            // 
            // DeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(611, 302);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "DeviceControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DeviceControlDemo(设备控制Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_passwrod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_set;
        private System.Windows.Forms.Button button_get;
        private System.Windows.Forms.DateTimePicker dateTimePicker_set;
        private System.Windows.Forms.DateTimePicker dateTimePicker_get;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_reboot;
        private System.Windows.Forms.CheckBox checkBox_now;
    }
}

