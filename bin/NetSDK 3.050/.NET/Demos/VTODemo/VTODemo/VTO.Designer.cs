namespace VTODemo
{
    partial class VTO
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listView_event = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button_stoplisten = new System.Windows.Forms.Button();
            this.button_startlisten = new System.Windows.Forms.Button();
            this.button_realplay = new System.Windows.Forms.Button();
            this.button_stopplay = new System.Windows.Forms.Button();
            this.button_talk = new System.Windows.Forms.Button();
            this.button_stoptalk = new System.Windows.Forms.Button();
            this.button_operatecard = new System.Windows.Forms.Button();
            this.button_open = new System.Windows.Forms.Button();
            this.button_close = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_play)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(771, 55);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(667, 20);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(98, 23);
            this.button_login.TabIndex = 8;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(561, 22);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '*';
            this.textBox_password.Size = new System.Drawing.Size(100, 21);
            this.textBox_password.TabIndex = 7;
            this.textBox_password.Text = "admin123";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(466, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password(密码):";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(387, 22);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(71, 21);
            this.textBox_name.TabIndex = 5;
            this.textBox_name.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(302, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Name(用户名):";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(257, 22);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(40, 21);
            this.textBox_port.TabIndex = 3;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(185, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port(端口):";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(80, 22);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(100, 21);
            this.textBox_ip.TabIndex = 1;
            this.textBox_ip.Text = "172.23.29.61";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备IP):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox_play);
            this.groupBox2.Location = new System.Drawing.Point(5, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 281);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PreView(预览)";
            // 
            // pictureBox_play
            // 
            this.pictureBox_play.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_play.Location = new System.Drawing.Point(11, 20);
            this.pictureBox_play.Name = "pictureBox_play";
            this.pictureBox_play.Size = new System.Drawing.Size(353, 248);
            this.pictureBox_play.TabIndex = 0;
            this.pictureBox_play.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listView_event);
            this.groupBox4.Location = new System.Drawing.Point(5, 342);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(771, 169);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "EventList(事件列表)";
            // 
            // listView_event
            // 
            this.listView_event.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView_event.FullRowSelect = true;
            this.listView_event.GridLines = true;
            this.listView_event.Location = new System.Drawing.Point(8, 17);
            this.listView_event.Name = "listView_event";
            this.listView_event.Size = new System.Drawing.Size(754, 146);
            this.listView_event.TabIndex = 0;
            this.listView_event.UseCompatibleStateImageBehavior = false;
            this.listView_event.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "RoomNo.(房间号)";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "CardNo.(卡号)";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Time(时间)";
            this.columnHeader4.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "OpenMethod(开门方式)";
            this.columnHeader5.Width = 150;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Status(状态)";
            this.columnHeader6.Width = 150;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button_stoplisten);
            this.groupBox5.Controls.Add(this.button_startlisten);
            this.groupBox5.Location = new System.Drawing.Point(381, 230);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(395, 100);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Event Operate(事件操作)";
            // 
            // button_stoplisten
            // 
            this.button_stoplisten.Location = new System.Drawing.Point(200, 48);
            this.button_stoplisten.Name = "button_stoplisten";
            this.button_stoplisten.Size = new System.Drawing.Size(167, 23);
            this.button_stoplisten.TabIndex = 4;
            this.button_stoplisten.Text = "StopListen(停止监听)";
            this.button_stoplisten.UseVisualStyleBackColor = true;
            this.button_stoplisten.Click += new System.EventHandler(this.button_stoplisten_Click);
            // 
            // button_startlisten
            // 
            this.button_startlisten.Location = new System.Drawing.Point(18, 48);
            this.button_startlisten.Name = "button_startlisten";
            this.button_startlisten.Size = new System.Drawing.Size(167, 23);
            this.button_startlisten.TabIndex = 3;
            this.button_startlisten.Text = "StartListen(开始监听)";
            this.button_startlisten.UseVisualStyleBackColor = true;
            this.button_startlisten.Click += new System.EventHandler(this.button_startlisten_Click);
            // 
            // button_realplay
            // 
            this.button_realplay.Location = new System.Drawing.Point(18, 22);
            this.button_realplay.Name = "button_realplay";
            this.button_realplay.Size = new System.Drawing.Size(167, 23);
            this.button_realplay.TabIndex = 0;
            this.button_realplay.Text = "RealPlay(监视)";
            this.button_realplay.UseVisualStyleBackColor = true;
            this.button_realplay.Click += new System.EventHandler(this.button_realplay_Click);
            // 
            // button_stopplay
            // 
            this.button_stopplay.Location = new System.Drawing.Point(200, 22);
            this.button_stopplay.Name = "button_stopplay";
            this.button_stopplay.Size = new System.Drawing.Size(167, 23);
            this.button_stopplay.TabIndex = 1;
            this.button_stopplay.Text = "StopPlay(停止监视)";
            this.button_stopplay.UseVisualStyleBackColor = true;
            this.button_stopplay.Click += new System.EventHandler(this.button_stopplay_Click);
            // 
            // button_talk
            // 
            this.button_talk.Location = new System.Drawing.Point(18, 55);
            this.button_talk.Name = "button_talk";
            this.button_talk.Size = new System.Drawing.Size(167, 23);
            this.button_talk.TabIndex = 2;
            this.button_talk.Text = "Talk(对讲)";
            this.button_talk.UseVisualStyleBackColor = true;
            this.button_talk.Click += new System.EventHandler(this.button_talk_Click);
            // 
            // button_stoptalk
            // 
            this.button_stoptalk.Location = new System.Drawing.Point(200, 55);
            this.button_stoptalk.Name = "button_stoptalk";
            this.button_stoptalk.Size = new System.Drawing.Size(167, 23);
            this.button_stoptalk.TabIndex = 3;
            this.button_stoptalk.Text = "StopTalk(停止对讲)";
            this.button_stoptalk.UseVisualStyleBackColor = true;
            this.button_stoptalk.Click += new System.EventHandler(this.button_stoptalk_Click);
            // 
            // button_operatecard
            // 
            this.button_operatecard.Location = new System.Drawing.Point(18, 120);
            this.button_operatecard.Name = "button_operatecard";
            this.button_operatecard.Size = new System.Drawing.Size(349, 23);
            this.button_operatecard.TabIndex = 4;
            this.button_operatecard.Text = "Operate(操作)";
            this.button_operatecard.UseVisualStyleBackColor = true;
            this.button_operatecard.Click += new System.EventHandler(this.button_operatecard_Click);
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(18, 87);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(167, 23);
            this.button_open.TabIndex = 7;
            this.button_open.Text = "OpenDoor(开门)";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(200, 87);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(167, 23);
            this.button_close.TabIndex = 8;
            this.button_close.Text = "CloseDoor(关门)";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_close);
            this.groupBox3.Controls.Add(this.button_open);
            this.groupBox3.Controls.Add(this.button_operatecard);
            this.groupBox3.Controls.Add(this.button_stoptalk);
            this.groupBox3.Controls.Add(this.button_talk);
            this.groupBox3.Controls.Add(this.button_stopplay);
            this.groupBox3.Controls.Add(this.button_realplay);
            this.groupBox3.Location = new System.Drawing.Point(381, 62);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(395, 162);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Operate(操作)";
            // 
            // VTO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(779, 512);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "VTO";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VTODemo(室外机Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_play)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox_play;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView listView_event;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button_stoplisten;
        private System.Windows.Forms.Button button_startlisten;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button button_realplay;
        private System.Windows.Forms.Button button_stopplay;
        private System.Windows.Forms.Button button_talk;
        private System.Windows.Forms.Button button_stoptalk;
        private System.Windows.Forms.Button button_operatecard;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Button button_close;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

