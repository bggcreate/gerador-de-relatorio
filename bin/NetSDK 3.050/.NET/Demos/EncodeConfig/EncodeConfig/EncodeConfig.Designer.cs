namespace EncodeConfig
{
    partial class EncodeConfig
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
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_value = new System.Windows.Forms.TextBox();
            this.label_value = new System.Windows.Forms.Label();
            this.comboBox_channel = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_i = new System.Windows.Forms.TextBox();
            this.comboBox_frame = new System.Windows.Forms.ComboBox();
            this.comboBox_stream = new System.Windows.Forms.ComboBox();
            this.comboBox_encodingtype = new System.Windows.Forms.ComboBox();
            this.button_set = new System.Windows.Forms.Button();
            this.button_get = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_login);
            this.groupBox1.Controls.Add(this.textBox_password);
            this.groupBox1.Controls.Add(this.textBox_username);
            this.groupBox1.Controls.Add(this.textBox_port);
            this.groupBox1.Controls.Add(this.textBox_ip);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(244, 234);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(68, 161);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(100, 23);
            this.button_login.TabIndex = 8;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(119, 114);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '*';
            this.textBox_password.Size = new System.Drawing.Size(112, 21);
            this.textBox_password.TabIndex = 7;
            this.textBox_password.Text = "admin123";
            // 
            // textBox_username
            // 
            this.textBox_username.Location = new System.Drawing.Point(119, 84);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(112, 21);
            this.textBox_username.TabIndex = 6;
            this.textBox_username.Text = "admin";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(119, 57);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(112, 21);
            this.textBox_port.TabIndex = 5;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(119, 26);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(112, 21);
            this.textBox_ip.TabIndex = 4;
            this.textBox_ip.Text = "172.23.12.12";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Password(密码):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "UserName(用户名):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port(端口):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备地址):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_value);
            this.groupBox2.Controls.Add(this.label_value);
            this.groupBox2.Controls.Add(this.comboBox_channel);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.textBox_i);
            this.groupBox2.Controls.Add(this.comboBox_frame);
            this.groupBox2.Controls.Add(this.comboBox_stream);
            this.groupBox2.Controls.Add(this.comboBox_encodingtype);
            this.groupBox2.Controls.Add(this.button_set);
            this.groupBox2.Controls.Add(this.button_get);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(260, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(313, 234);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Video Attribute(视频属性)";
            // 
            // textBox_value
            // 
            this.textBox_value.Location = new System.Drawing.Point(160, 169);
            this.textBox_value.Name = "textBox_value";
            this.textBox_value.Size = new System.Drawing.Size(145, 21);
            this.textBox_value.TabIndex = 13;
            this.textBox_value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_value_KeyPress);
            // 
            // label_value
            // 
            this.label_value.AutoSize = true;
            this.label_value.Location = new System.Drawing.Point(89, 172);
            this.label_value.Name = "label_value";
            this.label_value.Size = new System.Drawing.Size(65, 12);
            this.label_value.TabIndex = 12;
            this.label_value.Text = "Value(值):";
            // 
            // comboBox_channel
            // 
            this.comboBox_channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_channel.FormattingEnabled = true;
            this.comboBox_channel.Location = new System.Drawing.Point(160, 19);
            this.comboBox_channel.Name = "comboBox_channel";
            this.comboBox_channel.Size = new System.Drawing.Size(147, 20);
            this.comboBox_channel.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(65, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 10;
            this.label9.Text = "Channel(通道):";
            // 
            // textBox_i
            // 
            this.textBox_i.Location = new System.Drawing.Point(160, 111);
            this.textBox_i.Name = "textBox_i";
            this.textBox_i.Size = new System.Drawing.Size(145, 21);
            this.textBox_i.TabIndex = 9;
            this.textBox_i.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_i_KeyPress);
            // 
            // comboBox_frame
            // 
            this.comboBox_frame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_frame.FormattingEnabled = true;
            this.comboBox_frame.Location = new System.Drawing.Point(160, 83);
            this.comboBox_frame.Name = "comboBox_frame";
            this.comboBox_frame.Size = new System.Drawing.Size(146, 20);
            this.comboBox_frame.TabIndex = 8;
            // 
            // comboBox_stream
            // 
            this.comboBox_stream.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_stream.FormattingEnabled = true;
            this.comboBox_stream.Location = new System.Drawing.Point(159, 140);
            this.comboBox_stream.Name = "comboBox_stream";
            this.comboBox_stream.Size = new System.Drawing.Size(146, 20);
            this.comboBox_stream.TabIndex = 7;
            this.comboBox_stream.SelectedIndexChanged += new System.EventHandler(this.comboBox_stream_SelectedIndexChanged);
            // 
            // comboBox_encodingtype
            // 
            this.comboBox_encodingtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_encodingtype.FormattingEnabled = true;
            this.comboBox_encodingtype.Location = new System.Drawing.Point(160, 52);
            this.comboBox_encodingtype.Name = "comboBox_encodingtype";
            this.comboBox_encodingtype.Size = new System.Drawing.Size(147, 20);
            this.comboBox_encodingtype.TabIndex = 6;
            // 
            // button_set
            // 
            this.button_set.Location = new System.Drawing.Point(182, 201);
            this.button_set.Name = "button_set";
            this.button_set.Size = new System.Drawing.Size(75, 23);
            this.button_set.TabIndex = 5;
            this.button_set.Text = "Set(设置)";
            this.button_set.UseVisualStyleBackColor = true;
            this.button_set.Click += new System.EventHandler(this.button_set_Click);
            // 
            // button_get
            // 
            this.button_get.Location = new System.Drawing.Point(79, 201);
            this.button_get.Name = "button_get";
            this.button_get.Size = new System.Drawing.Size(75, 23);
            this.button_get.TabIndex = 4;
            this.button_get.Text = "Get(获取)";
            this.button_get.UseVisualStyleBackColor = true;
            this.button_get.Click += new System.EventHandler(this.button_get_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "I Interval(I帧间隔):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(77, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "Frame(帧率):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Stream-Rate(码率):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(149, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Encoding Type(编码类型):";
            // 
            // EncodeConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(585, 252);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "EncodeConfig";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EncodeConfig(编码配置)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.TextBox textBox_i;
        private System.Windows.Forms.ComboBox comboBox_frame;
        private System.Windows.Forms.ComboBox comboBox_stream;
        private System.Windows.Forms.ComboBox comboBox_encodingtype;
        private System.Windows.Forms.Button button_set;
        private System.Windows.Forms.Button button_get;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_channel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_value;
        private System.Windows.Forms.Label label_value;
    }
}

