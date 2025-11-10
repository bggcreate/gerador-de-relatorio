namespace FaceAttendance
{
    partial class MainWnd
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LoginDevice = new System.Windows.Forms.Button();
            this.pwd_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.user_textBox = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.Label();
            this.port_textBox = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.Label();
            this.ip_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.realload_pictureBox = new System.Windows.Forms.PictureBox();
            this.snap_pictureBox = new System.Windows.Forms.PictureBox();
            this.search_listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.find_listView = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Add_button = new System.Windows.Forms.Button();
            this.RealLoad = new System.Windows.Forms.Button();
            this.Remove_button = new System.Windows.Forms.Button();
            this.Update_button = new System.Windows.Forms.Button();
            this.Find_button = new System.Windows.Forms.Button();
            this.OpenDoor = new System.Windows.Forms.Button();
            this.Search_button = new System.Windows.Forms.Button();
            this.Snap_button = new System.Windows.Forms.Button();
            this.cardno_textBox = new System.Windows.Forms.TextBox();
            this.start_dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.开始 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.end_dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.card_search_radioButton = new System.Windows.Forms.RadioButton();
            this.time_search_radioButton = new System.Windows.Forms.RadioButton();
            this.searchcardno_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.realload_pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.snap_pictureBox)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LoginDevice);
            this.groupBox1.Controls.Add(this.pwd_textBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.user_textBox);
            this.groupBox1.Controls.Add(this.username);
            this.groupBox1.Controls.Add(this.port_textBox);
            this.groupBox1.Controls.Add(this.port);
            this.groupBox1.Controls.Add(this.ip_textBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1009, 61);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login Info(登陆信息)";
            // 
            // LoginDevice
            // 
            this.LoginDevice.Location = new System.Drawing.Point(838, 20);
            this.LoginDevice.Name = "LoginDevice";
            this.LoginDevice.Size = new System.Drawing.Size(103, 35);
            this.LoginDevice.TabIndex = 10;
            this.LoginDevice.Text = "Login(登陆)";
            this.LoginDevice.UseVisualStyleBackColor = true;
            this.LoginDevice.Click += new System.EventHandler(this.LoginDevice_Click);
            // 
            // pwd_textBox
            // 
            this.pwd_textBox.Location = new System.Drawing.Point(585, 26);
            this.pwd_textBox.Name = "pwd_textBox";
            this.pwd_textBox.Size = new System.Drawing.Size(111, 21);
            this.pwd_textBox.TabIndex = 7;
            this.pwd_textBox.Text = "admin123";
            this.pwd_textBox.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(484, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Password(密码):";
            // 
            // user_textBox
            // 
            this.user_textBox.Location = new System.Drawing.Point(394, 26);
            this.user_textBox.Name = "user_textBox";
            this.user_textBox.Size = new System.Drawing.Size(80, 21);
            this.user_textBox.TabIndex = 5;
            this.user_textBox.Text = "admin";
            // 
            // username
            // 
            this.username.AutoSize = true;
            this.username.Location = new System.Drawing.Point(323, 31);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(71, 12);
            this.username.TabIndex = 4;
            this.username.Text = "User(用户):";
            // 
            // port_textBox
            // 
            this.port_textBox.Location = new System.Drawing.Point(266, 26);
            this.port_textBox.Name = "port_textBox";
            this.port_textBox.Size = new System.Drawing.Size(50, 21);
            this.port_textBox.TabIndex = 3;
            this.port_textBox.Text = "37777";
            // 
            // port
            // 
            this.port.AutoSize = true;
            this.port.Location = new System.Drawing.Point(189, 31);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(71, 12);
            this.port.TabIndex = 2;
            this.port.Text = "Port(端口):";
            // 
            // ip_textBox
            // 
            this.ip_textBox.Location = new System.Drawing.Point(88, 28);
            this.ip_textBox.Name = "ip_textBox";
            this.ip_textBox.Size = new System.Drawing.Size(95, 21);
            this.ip_textBox.TabIndex = 1;
            this.ip_textBox.Text = "172.23.28.63";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备IP):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.realload_pictureBox);
            this.groupBox2.Location = new System.Drawing.Point(709, 373);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(297, 184);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RealLoadPicture(订阅图片)";
            // 
            // realload_pictureBox
            // 
            this.realload_pictureBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.realload_pictureBox.Location = new System.Drawing.Point(7, 14);
            this.realload_pictureBox.Name = "realload_pictureBox";
            this.realload_pictureBox.Size = new System.Drawing.Size(281, 164);
            this.realload_pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.realload_pictureBox.TabIndex = 0;
            this.realload_pictureBox.TabStop = false;
            // 
            // snap_pictureBox
            // 
            this.snap_pictureBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.snap_pictureBox.Location = new System.Drawing.Point(7, 18);
            this.snap_pictureBox.Name = "snap_pictureBox";
            this.snap_pictureBox.Size = new System.Drawing.Size(282, 169);
            this.snap_pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.snap_pictureBox.TabIndex = 1;
            this.snap_pictureBox.TabStop = false;
            // 
            // search_listView
            // 
            this.search_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.search_listView.FullRowSelect = true;
            this.search_listView.GridLines = true;
            this.search_listView.Location = new System.Drawing.Point(12, 168);
            this.search_listView.MultiSelect = false;
            this.search_listView.Name = "search_listView";
            this.search_listView.Size = new System.Drawing.Size(673, 199);
            this.search_listView.TabIndex = 2;
            this.search_listView.TabStop = false;
            this.search_listView.UseCompatibleStateImageBehavior = false;
            this.search_listView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "UserID(用户ID)";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "CradNo(卡号)";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "CardName(用户名)";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 110;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Pwd(密码)";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 80;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "UserLevel(用户等级)";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 150;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Finger(指纹数据)";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 130;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "FaceData(人脸模板数据)";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 200;
            // 
            // find_listView
            // 
            this.find_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12});
            this.find_listView.FullRowSelect = true;
            this.find_listView.GridLines = true;
            this.find_listView.Location = new System.Drawing.Point(13, 373);
            this.find_listView.Name = "find_listView";
            this.find_listView.Size = new System.Drawing.Size(674, 184);
            this.find_listView.TabIndex = 3;
            this.find_listView.UseCompatibleStateImageBehavior = false;
            this.find_listView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Time(时间)";
            this.columnHeader8.Width = 175;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "UserID(用户ID)";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 100;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "CardName(卡名)";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader10.Width = 100;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "OpenMethod(开门方式)";
            this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader11.Width = 150;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "AttendanceState(考勤状态)";
            this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader12.Width = 185;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.snap_pictureBox);
            this.groupBox3.Location = new System.Drawing.Point(710, 168);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(298, 199);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Capture Picture(抓图)";
            // 
            // Add_button
            // 
            this.Add_button.Location = new System.Drawing.Point(18, 57);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(85, 30);
            this.Add_button.TabIndex = 0;
            this.Add_button.Text = "Add(增加)";
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.Add_button_Click);
            // 
            // RealLoad
            // 
            this.RealLoad.Location = new System.Drawing.Point(883, 53);
            this.RealLoad.Name = "RealLoad";
            this.RealLoad.Size = new System.Drawing.Size(120, 35);
            this.RealLoad.TabIndex = 11;
            this.RealLoad.Text = "Attach(订阅)";
            this.RealLoad.UseVisualStyleBackColor = true;
            this.RealLoad.Click += new System.EventHandler(this.RealLoad_Click);
            // 
            // Remove_button
            // 
            this.Remove_button.Location = new System.Drawing.Point(111, 58);
            this.Remove_button.Name = "Remove_button";
            this.Remove_button.Size = new System.Drawing.Size(85, 30);
            this.Remove_button.TabIndex = 1;
            this.Remove_button.Text = "Delete(删除)";
            this.Remove_button.UseVisualStyleBackColor = true;
            this.Remove_button.Click += new System.EventHandler(this.Remove_button_Click);
            // 
            // Update_button
            // 
            this.Update_button.Location = new System.Drawing.Point(202, 58);
            this.Update_button.Name = "Update_button";
            this.Update_button.Size = new System.Drawing.Size(85, 30);
            this.Update_button.TabIndex = 2;
            this.Update_button.Text = "Update(更新)";
            this.Update_button.UseVisualStyleBackColor = true;
            this.Update_button.Click += new System.EventHandler(this.Update_button_Click);
            // 
            // Find_button
            // 
            this.Find_button.Location = new System.Drawing.Point(202, 19);
            this.Find_button.Name = "Find_button";
            this.Find_button.Size = new System.Drawing.Size(85, 31);
            this.Find_button.TabIndex = 3;
            this.Find_button.Text = "Get(获取)";
            this.Find_button.UseVisualStyleBackColor = true;
            this.Find_button.Click += new System.EventHandler(this.Find_button_Click);
            // 
            // OpenDoor
            // 
            this.OpenDoor.Location = new System.Drawing.Point(883, 16);
            this.OpenDoor.Name = "OpenDoor";
            this.OpenDoor.Size = new System.Drawing.Size(120, 35);
            this.OpenDoor.TabIndex = 12;
            this.OpenDoor.Text = "OpenDoor(开门)";
            this.OpenDoor.UseVisualStyleBackColor = true;
            this.OpenDoor.Click += new System.EventHandler(this.OpenDoor_Click);
            // 
            // Search_button
            // 
            this.Search_button.Location = new System.Drawing.Point(776, 16);
            this.Search_button.Name = "Search_button";
            this.Search_button.Size = new System.Drawing.Size(101, 35);
            this.Search_button.TabIndex = 14;
            this.Search_button.Text = "Search(查询)";
            this.Search_button.UseVisualStyleBackColor = true;
            this.Search_button.Click += new System.EventHandler(this.Search_button_Click);
            // 
            // Snap_button
            // 
            this.Snap_button.Location = new System.Drawing.Point(776, 53);
            this.Snap_button.Name = "Snap_button";
            this.Snap_button.Size = new System.Drawing.Size(101, 35);
            this.Snap_button.TabIndex = 13;
            this.Snap_button.Text = "Capture(抓图)";
            this.Snap_button.UseVisualStyleBackColor = true;
            this.Snap_button.Click += new System.EventHandler(this.Snap_button_Click);
            // 
            // cardno_textBox
            // 
            this.cardno_textBox.Location = new System.Drawing.Point(702, 22);
            this.cardno_textBox.MaxLength = 31;
            this.cardno_textBox.Name = "cardno_textBox";
            this.cardno_textBox.Size = new System.Drawing.Size(64, 21);
            this.cardno_textBox.TabIndex = 18;
            this.cardno_textBox.TextChanged += new System.EventHandler(this.cardno_textBox_TextChanged);
            // 
            // start_dateTimePicker
            // 
            this.start_dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.start_dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.start_dateTimePicker.Location = new System.Drawing.Point(373, 61);
            this.start_dateTimePicker.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.start_dateTimePicker.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.start_dateTimePicker.Name = "start_dateTimePicker";
            this.start_dateTimePicker.ShowUpDown = true;
            this.start_dateTimePicker.Size = new System.Drawing.Size(158, 21);
            this.start_dateTimePicker.TabIndex = 19;
            // 
            // 开始
            // 
            this.开始.AutoSize = true;
            this.开始.Location = new System.Drawing.Point(293, 65);
            this.开始.Name = "开始";
            this.开始.Size = new System.Drawing.Size(77, 12);
            this.开始.TabIndex = 20;
            this.开始.Text = "Start(开始):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(539, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 21;
            this.label4.Text = "End(结束):";
            // 
            // end_dateTimePicker
            // 
            this.end_dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.end_dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.end_dateTimePicker.Location = new System.Drawing.Point(608, 61);
            this.end_dateTimePicker.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.end_dateTimePicker.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.end_dateTimePicker.Name = "end_dateTimePicker";
            this.end_dateTimePicker.ShowUpDown = true;
            this.end_dateTimePicker.Size = new System.Drawing.Size(158, 21);
            this.end_dateTimePicker.TabIndex = 22;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.card_search_radioButton);
            this.groupBox4.Controls.Add(this.time_search_radioButton);
            this.groupBox4.Controls.Add(this.searchcardno_textBox);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.end_dateTimePicker);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.开始);
            this.groupBox4.Controls.Add(this.start_dateTimePicker);
            this.groupBox4.Controls.Add(this.cardno_textBox);
            this.groupBox4.Controls.Add(this.Snap_button);
            this.groupBox4.Controls.Add(this.Search_button);
            this.groupBox4.Controls.Add(this.OpenDoor);
            this.groupBox4.Controls.Add(this.Find_button);
            this.groupBox4.Controls.Add(this.Update_button);
            this.groupBox4.Controls.Add(this.Remove_button);
            this.groupBox4.Controls.Add(this.RealLoad);
            this.groupBox4.Controls.Add(this.Add_button);
            this.groupBox4.Location = new System.Drawing.Point(13, 68);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1009, 94);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            // 
            // card_search_radioButton
            // 
            this.card_search_radioButton.AutoSize = true;
            this.card_search_radioButton.Location = new System.Drawing.Point(517, 26);
            this.card_search_radioButton.Name = "card_search_radioButton";
            this.card_search_radioButton.Size = new System.Drawing.Size(179, 16);
            this.card_search_radioButton.TabIndex = 26;
            this.card_search_radioButton.TabStop = true;
            this.card_search_radioButton.Text = "Search By Card(按卡号查询)";
            this.card_search_radioButton.UseVisualStyleBackColor = true;
            // 
            // time_search_radioButton
            // 
            this.time_search_radioButton.AutoSize = true;
            this.time_search_radioButton.Location = new System.Drawing.Point(299, 26);
            this.time_search_radioButton.Name = "time_search_radioButton";
            this.time_search_radioButton.Size = new System.Drawing.Size(191, 16);
            this.time_search_radioButton.TabIndex = 25;
            this.time_search_radioButton.TabStop = true;
            this.time_search_radioButton.Text = "Search By Period(按时间查询)";
            this.time_search_radioButton.UseVisualStyleBackColor = true;
            // 
            // searchcardno_textBox
            // 
            this.searchcardno_textBox.Location = new System.Drawing.Point(111, 20);
            this.searchcardno_textBox.MaxLength = 31;
            this.searchcardno_textBox.Name = "searchcardno_textBox";
            this.searchcardno_textBox.Size = new System.Drawing.Size(72, 21);
            this.searchcardno_textBox.TabIndex = 24;
            this.searchcardno_textBox.TextChanged += new System.EventHandler(this.searchcardno_textBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 23;
            this.label5.Text = "CardNo(卡号): ";
            // 
            // MainWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 572);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.find_listView);
            this.Controls.Add(this.search_listView);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWnd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FaceAttendance(人脸考勤机)--OffLine(离线)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.realload_pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.snap_pictureBox)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button LoginDevice;
        private System.Windows.Forms.TextBox pwd_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox user_textBox;
        private System.Windows.Forms.Label username;
        private System.Windows.Forms.TextBox port_textBox;
        private System.Windows.Forms.Label port;
        private System.Windows.Forms.TextBox ip_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox snap_pictureBox;
        private System.Windows.Forms.PictureBox realload_pictureBox;
        private System.Windows.Forms.ListView search_listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ListView find_listView;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Add_button;
        private System.Windows.Forms.Button RealLoad;
        private System.Windows.Forms.Button Remove_button;
        private System.Windows.Forms.Button Update_button;
        private System.Windows.Forms.Button Find_button;
        private System.Windows.Forms.Button OpenDoor;
        private System.Windows.Forms.Button Search_button;
        private System.Windows.Forms.Button Snap_button;
        private System.Windows.Forms.TextBox cardno_textBox;
        private System.Windows.Forms.DateTimePicker start_dateTimePicker;
        private System.Windows.Forms.Label 开始;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker end_dateTimePicker;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox searchcardno_textBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.RadioButton card_search_radioButton;
        private System.Windows.Forms.RadioButton time_search_radioButton;
    }
}

