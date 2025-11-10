namespace CaptureDemo
{
    partial class CaptureDemo
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
            this.button_span = new System.Windows.Forms.Button();
            this.button_realplay = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_channel = new System.Windows.Forms.ComboBox();
            this.button_local = new System.Windows.Forms.Button();
            this.button_remote = new System.Windows.Forms.Button();
            this.pictureBox_image = new System.Windows.Forms.PictureBox();
            this.pictureBox_preview = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_preview)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(6, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(751, 52);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(644, 20);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(99, 23);
            this.button_login.TabIndex = 8;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(557, 22);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '*';
            this.textBox_password.Size = new System.Drawing.Size(81, 21);
            this.textBox_password.TabIndex = 7;
            this.textBox_password.Text = "admin456";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(486, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Pwd(密码):";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(385, 24);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(95, 21);
            this.textBox_name.TabIndex = 5;
            this.textBox_name.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(296, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Name(用户名):";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(246, 24);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(45, 21);
            this.textBox_port.TabIndex = 3;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(174, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port(端口):";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(86, 24);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(84, 21);
            this.textBox_ip.TabIndex = 1;
            this.textBox_ip.Text = "172.23.8.95";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备地址):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_span);
            this.groupBox2.Controls.Add(this.button_realplay);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.comboBox_channel);
            this.groupBox2.Controls.Add(this.button_local);
            this.groupBox2.Controls.Add(this.button_remote);
            this.groupBox2.Location = new System.Drawing.Point(7, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(751, 76);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operate(操作)";
            // 
            // button_span
            // 
            this.button_span.Location = new System.Drawing.Point(201, 44);
            this.button_span.Name = "button_span";
            this.button_span.Size = new System.Drawing.Size(205, 23);
            this.button_span.TabIndex = 5;
            this.button_span.Text = "Span Capture(定时抓图)";
            this.button_span.UseVisualStyleBackColor = true;
            this.button_span.Click += new System.EventHandler(this.button_span_Click);
            // 
            // button_realplay
            // 
            this.button_realplay.Location = new System.Drawing.Point(201, 15);
            this.button_realplay.Name = "button_realplay";
            this.button_realplay.Size = new System.Drawing.Size(205, 23);
            this.button_realplay.TabIndex = 4;
            this.button_realplay.Text = "RealPlay(监视)";
            this.button_realplay.UseVisualStyleBackColor = true;
            this.button_realplay.Click += new System.EventHandler(this.button_realplay_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "Channel(通道):";
            // 
            // comboBox_channel
            // 
            this.comboBox_channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_channel.FormattingEnabled = true;
            this.comboBox_channel.Location = new System.Drawing.Point(106, 17);
            this.comboBox_channel.Name = "comboBox_channel";
            this.comboBox_channel.Size = new System.Drawing.Size(89, 20);
            this.comboBox_channel.TabIndex = 2;
            // 
            // button_local
            // 
            this.button_local.Location = new System.Drawing.Point(415, 15);
            this.button_local.Name = "button_local";
            this.button_local.Size = new System.Drawing.Size(159, 23);
            this.button_local.TabIndex = 1;
            this.button_local.Text = "Local Capture(本地抓图)";
            this.button_local.UseVisualStyleBackColor = true;
            this.button_local.Click += new System.EventHandler(this.button_local_Click);
            // 
            // button_remote
            // 
            this.button_remote.Location = new System.Drawing.Point(13, 44);
            this.button_remote.Name = "button_remote";
            this.button_remote.Size = new System.Drawing.Size(182, 23);
            this.button_remote.TabIndex = 0;
            this.button_remote.Text = "Remote Capture(远程抓图)";
            this.button_remote.UseVisualStyleBackColor = true;
            this.button_remote.Click += new System.EventHandler(this.button_remote_Click);
            // 
            // pictureBox_image
            // 
            this.pictureBox_image.Location = new System.Drawing.Point(6, 19);
            this.pictureBox_image.Name = "pictureBox_image";
            this.pictureBox_image.Size = new System.Drawing.Size(361, 327);
            this.pictureBox_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_image.TabIndex = 2;
            this.pictureBox_image.TabStop = false;
            // 
            // pictureBox_preview
            // 
            this.pictureBox_preview.Location = new System.Drawing.Point(6, 20);
            this.pictureBox_preview.Name = "pictureBox_preview";
            this.pictureBox_preview.Size = new System.Drawing.Size(358, 327);
            this.pictureBox_preview.TabIndex = 3;
            this.pictureBox_preview.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureBox_preview);
            this.groupBox3.Location = new System.Drawing.Point(7, 151);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(370, 353);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Preview(预览)";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.pictureBox_image);
            this.groupBox4.Location = new System.Drawing.Point(383, 152);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(375, 352);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Image(图片)";
            // 
            // CaptureDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(762, 510);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "CaptureDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CaptureDemo(抓图Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_preview)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
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
        private System.Windows.Forms.Button button_local;
        private System.Windows.Forms.Button button_remote;
        private System.Windows.Forms.PictureBox pictureBox_image;
        private System.Windows.Forms.Button button_realplay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_channel;
        private System.Windows.Forms.PictureBox pictureBox_preview;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_span;
    }
}

