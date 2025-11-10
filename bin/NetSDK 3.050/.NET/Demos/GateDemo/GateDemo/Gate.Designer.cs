namespace GateDemo
{
    partial class Gate
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
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox_play = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_attach = new System.Windows.Forms.Button();
            this.button_operate = new System.Windows.Forms.Button();
            this.button_play = new System.Windows.Forms.Button();
            this.comboBox_channel = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label_userid = new System.Windows.Forms.Label();
            this.label_cardno = new System.Windows.Forms.Label();
            this.label_cardname = new System.Windows.Forms.Label();
            this.label_openmethod = new System.Windows.Forms.Label();
            this.label_time = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox_image = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label_openstatus = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_play)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_login);
            this.groupBox1.Controls.Add(this.textBox_password);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_port);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_ip);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(738, 54);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登陆)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(630, 20);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(102, 23);
            this.button_login.TabIndex = 8;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(533, 23);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '*';
            this.textBox_password.Size = new System.Drawing.Size(91, 21);
            this.textBox_password.TabIndex = 7;
            this.textBox_password.Text = "admin123";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(469, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Pwd(密码):";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(389, 23);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(74, 21);
            this.textBox_name.TabIndex = 5;
            this.textBox_name.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(304, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Name(用户名):";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(253, 23);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(45, 21);
            this.textBox_port.TabIndex = 3;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port(端口):";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(78, 23);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(98, 21);
            this.textBox_ip.TabIndex = 1;
            this.textBox_ip.Text = "172.5.3.26";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备IP):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox_play);
            this.groupBox2.Location = new System.Drawing.Point(6, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(361, 377);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview(预览)";
            // 
            // pictureBox_play
            // 
            this.pictureBox_play.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_play.Location = new System.Drawing.Point(8, 16);
            this.pictureBox_play.Name = "pictureBox_play";
            this.pictureBox_play.Size = new System.Drawing.Size(344, 353);
            this.pictureBox_play.TabIndex = 0;
            this.pictureBox_play.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_attach);
            this.groupBox3.Controls.Add(this.button_operate);
            this.groupBox3.Controls.Add(this.button_play);
            this.groupBox3.Controls.Add(this.comboBox_channel);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(373, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(371, 103);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Operate(操作)";
            // 
            // button_attach
            // 
            this.button_attach.Location = new System.Drawing.Point(13, 64);
            this.button_attach.Name = "button_attach";
            this.button_attach.Size = new System.Drawing.Size(154, 23);
            this.button_attach.TabIndex = 4;
            this.button_attach.Text = "Attach(订阅)";
            this.button_attach.UseVisualStyleBackColor = true;
            this.button_attach.Click += new System.EventHandler(this.button_attach_Click);
            // 
            // button_operate
            // 
            this.button_operate.Location = new System.Drawing.Point(211, 64);
            this.button_operate.Name = "button_operate";
            this.button_operate.Size = new System.Drawing.Size(154, 23);
            this.button_operate.TabIndex = 3;
            this.button_operate.Text = "CardOperate(卡操作)";
            this.button_operate.UseVisualStyleBackColor = true;
            this.button_operate.Click += new System.EventHandler(this.button_operate_Click);
            // 
            // button_play
            // 
            this.button_play.Location = new System.Drawing.Point(211, 16);
            this.button_play.Name = "button_play";
            this.button_play.Size = new System.Drawing.Size(154, 23);
            this.button_play.TabIndex = 2;
            this.button_play.Text = "RealPlay(监视)";
            this.button_play.UseVisualStyleBackColor = true;
            this.button_play.Click += new System.EventHandler(this.button_play_Click);
            // 
            // comboBox_channel
            // 
            this.comboBox_channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_channel.FormattingEnabled = true;
            this.comboBox_channel.Location = new System.Drawing.Point(98, 19);
            this.comboBox_channel.Name = "comboBox_channel";
            this.comboBox_channel.Size = new System.Drawing.Size(69, 20);
            this.comboBox_channel.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Channel(通道):";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label_openstatus);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.label_userid);
            this.groupBox4.Controls.Add(this.label_cardno);
            this.groupBox4.Controls.Add(this.label_cardname);
            this.groupBox4.Controls.Add(this.label_openmethod);
            this.groupBox4.Controls.Add(this.label_time);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.pictureBox_image);
            this.groupBox4.Location = new System.Drawing.Point(373, 167);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(371, 268);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Image(图片)";
            // 
            // label_userid
            // 
            this.label_userid.AutoSize = true;
            this.label_userid.Location = new System.Drawing.Point(186, 209);
            this.label_userid.Name = "label_userid";
            this.label_userid.Size = new System.Drawing.Size(0, 12);
            this.label_userid.TabIndex = 10;
            // 
            // label_cardno
            // 
            this.label_cardno.AutoSize = true;
            this.label_cardno.Location = new System.Drawing.Point(186, 162);
            this.label_cardno.Name = "label_cardno";
            this.label_cardno.Size = new System.Drawing.Size(0, 12);
            this.label_cardno.TabIndex = 9;
            // 
            // label_cardname
            // 
            this.label_cardname.AutoSize = true;
            this.label_cardname.Location = new System.Drawing.Point(186, 119);
            this.label_cardname.Name = "label_cardname";
            this.label_cardname.Size = new System.Drawing.Size(0, 12);
            this.label_cardname.TabIndex = 8;
            // 
            // label_openmethod
            // 
            this.label_openmethod.AutoSize = true;
            this.label_openmethod.Location = new System.Drawing.Point(186, 76);
            this.label_openmethod.Name = "label_openmethod";
            this.label_openmethod.Size = new System.Drawing.Size(0, 12);
            this.label_openmethod.TabIndex = 7;
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(186, 36);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(0, 12);
            this.label_time.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(186, 185);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "UserID(用户ID):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(186, 98);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 12);
            this.label9.TabIndex = 4;
            this.label9.Text = "CardName(卡名):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(186, 140);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "CardNo.(卡号):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(186, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(137, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "Open Method(开门方式):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(186, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Time(时间):";
            // 
            // pictureBox_image
            // 
            this.pictureBox_image.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_image.Location = new System.Drawing.Point(8, 20);
            this.pictureBox_image.Name = "pictureBox_image";
            this.pictureBox_image.Size = new System.Drawing.Size(172, 242);
            this.pictureBox_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_image.TabIndex = 0;
            this.pictureBox_image.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(186, 228);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(137, 12);
            this.label11.TabIndex = 11;
            this.label11.Text = "Open Status(开门状态):";
            // 
            // label_openstatus
            // 
            this.label_openstatus.AutoSize = true;
            this.label_openstatus.Location = new System.Drawing.Point(186, 248);
            this.label_openstatus.Name = "label_openstatus";
            this.label_openstatus.Size = new System.Drawing.Size(0, 12);
            this.label_openstatus.TabIndex = 12;
            // 
            // Gate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(750, 439);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "Gate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GateDemo(闸机Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_play)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox_play;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_attach;
        private System.Windows.Forms.Button button_operate;
        private System.Windows.Forms.Button button_play;
        private System.Windows.Forms.ComboBox comboBox_channel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PictureBox pictureBox_image;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label_userid;
        private System.Windows.Forms.Label label_cardno;
        private System.Windows.Forms.Label label_cardname;
        private System.Windows.Forms.Label label_openmethod;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Label label_openstatus;
        private System.Windows.Forms.Label label11;
    }
}

