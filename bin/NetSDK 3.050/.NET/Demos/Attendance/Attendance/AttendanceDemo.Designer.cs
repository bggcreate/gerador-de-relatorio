namespace Attendance
{
    partial class AttendanceDemo
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
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_operatebyuserid = new System.Windows.Forms.Button();
            this.button_operatebyfingerprintid = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_modify = new System.Windows.Forms.Button();
            this.button_add = new System.Windows.Forms.Button();
            this.button_search = new System.Windows.Forms.Button();
            this.textBox_userid = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_prepage = new System.Windows.Forms.Button();
            this.button_nextpage = new System.Windows.Forms.Button();
            this.listView_information = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listView_event = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_attach = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_login);
            this.groupBox1.Controls.Add(this.textBox_password);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.textBox_port);
            this.groupBox1.Controls.Add(this.textBox_ip);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(763, 65);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(663, 25);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(90, 23);
            this.button_login.TabIndex = 7;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(561, 27);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '*';
            this.textBox_password.Size = new System.Drawing.Size(83, 21);
            this.textBox_password.TabIndex = 6;
            this.textBox_password.Text = "admin123";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(379, 27);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(78, 21);
            this.textBox_name.TabIndex = 5;
            this.textBox_name.Text = "admin";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(248, 27);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(41, 21);
            this.textBox_port.TabIndex = 4;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(74, 27);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(96, 21);
            this.textBox_ip.TabIndex = 3;
            this.textBox_ip.Text = "172.23.29.60";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(463, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Passwrod(密码):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(295, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name(用户名):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port(端口):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备IP):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_operatebyuserid);
            this.groupBox2.Controls.Add(this.button_operatebyfingerprintid);
            this.groupBox2.Controls.Add(this.button_delete);
            this.groupBox2.Controls.Add(this.button_modify);
            this.groupBox2.Controls.Add(this.button_add);
            this.groupBox2.Controls.Add(this.button_search);
            this.groupBox2.Controls.Add(this.textBox_userid);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(4, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(614, 85);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Function(功能)";
            // 
            // button_operatebyuserid
            // 
            this.button_operatebyuserid.Location = new System.Drawing.Point(6, 51);
            this.button_operatebyuserid.Name = "button_operatebyuserid";
            this.button_operatebyuserid.Size = new System.Drawing.Size(313, 23);
            this.button_operatebyuserid.TabIndex = 10;
            this.button_operatebyuserid.Text = "Operate Fingerprint By UserID(通过用户ID操作指纹)";
            this.button_operatebyuserid.UseVisualStyleBackColor = true;
            this.button_operatebyuserid.Click += new System.EventHandler(this.button_operatebyuserid_Click);
            // 
            // button_operatebyfingerprintid
            // 
            this.button_operatebyfingerprintid.Location = new System.Drawing.Point(325, 51);
            this.button_operatebyfingerprintid.Name = "button_operatebyfingerprintid";
            this.button_operatebyfingerprintid.Size = new System.Drawing.Size(283, 23);
            this.button_operatebyfingerprintid.TabIndex = 9;
            this.button_operatebyfingerprintid.Text = "Operate Fingerprint By FPID(通过指纹ID操作)";
            this.button_operatebyfingerprintid.UseVisualStyleBackColor = true;
            this.button_operatebyfingerprintid.Click += new System.EventHandler(this.button_operatebyfingerprintid_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(518, 19);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(90, 23);
            this.button_delete.TabIndex = 7;
            this.button_delete.Text = "Delete(删除)";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_modify
            // 
            this.button_modify.Location = new System.Drawing.Point(422, 19);
            this.button_modify.Name = "button_modify";
            this.button_modify.Size = new System.Drawing.Size(90, 23);
            this.button_modify.TabIndex = 6;
            this.button_modify.Text = "Modify(修改)";
            this.button_modify.UseVisualStyleBackColor = true;
            this.button_modify.Click += new System.EventHandler(this.button_modify_Click);
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(326, 19);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(90, 23);
            this.button_add.TabIndex = 5;
            this.button_add.Text = "Add(增加)";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // button_search
            // 
            this.button_search.Location = new System.Drawing.Point(227, 19);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(90, 23);
            this.button_search.TabIndex = 4;
            this.button_search.Text = "Search(查询)";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // textBox_userid
            // 
            this.textBox_userid.Location = new System.Drawing.Point(109, 21);
            this.textBox_userid.Name = "textBox_userid";
            this.textBox_userid.Size = new System.Drawing.Size(112, 21);
            this.textBox_userid.TabIndex = 3;
            this.textBox_userid.TextChanged += new System.EventHandler(this.textBox_userid_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "UserID(用户ID):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_prepage);
            this.groupBox3.Controls.Add(this.button_nextpage);
            this.groupBox3.Controls.Add(this.listView_information);
            this.groupBox3.Location = new System.Drawing.Point(4, 163);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(373, 284);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "InformationList(信息列表)";
            // 
            // button_prepage
            // 
            this.button_prepage.Location = new System.Drawing.Point(5, 255);
            this.button_prepage.Name = "button_prepage";
            this.button_prepage.Size = new System.Drawing.Size(140, 23);
            this.button_prepage.TabIndex = 10;
            this.button_prepage.Text = "PrePage(上一页)";
            this.button_prepage.UseVisualStyleBackColor = true;
            this.button_prepage.Click += new System.EventHandler(this.button_prepage_Click);
            // 
            // button_nextpage
            // 
            this.button_nextpage.Location = new System.Drawing.Point(227, 255);
            this.button_nextpage.Name = "button_nextpage";
            this.button_nextpage.Size = new System.Drawing.Size(140, 23);
            this.button_nextpage.TabIndex = 11;
            this.button_nextpage.Text = "NextPage(下一页)";
            this.button_nextpage.UseVisualStyleBackColor = true;
            this.button_nextpage.Click += new System.EventHandler(this.button_nextpage_Click);
            // 
            // listView_information
            // 
            this.listView_information.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView_information.Font = new System.Drawing.Font("宋体", 9F);
            this.listView_information.FullRowSelect = true;
            this.listView_information.GridLines = true;
            this.listView_information.Location = new System.Drawing.Point(5, 21);
            this.listView_information.Name = "listView_information";
            this.listView_information.Size = new System.Drawing.Size(362, 228);
            this.listView_information.TabIndex = 0;
            this.listView_information.UseCompatibleStateImageBehavior = false;
            this.listView_information.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID(序号)";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "UserID(用户ID)";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "UserName(用户名)";
            this.columnHeader3.Width = 110;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "CardNo.(卡号)";
            this.columnHeader4.Width = 90;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listView_event);
            this.groupBox4.Location = new System.Drawing.Point(383, 163);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(384, 284);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Event Information(事件信息)";
            // 
            // listView_event
            // 
            this.listView_event.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.listView_event.FullRowSelect = true;
            this.listView_event.GridLines = true;
            this.listView_event.Location = new System.Drawing.Point(7, 21);
            this.listView_event.Name = "listView_event";
            this.listView_event.Size = new System.Drawing.Size(371, 254);
            this.listView_event.TabIndex = 0;
            this.listView_event.UseCompatibleStateImageBehavior = false;
            this.listView_event.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ID(序号)";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "UserID(用户ID)";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "CardNo.(卡号)";
            this.columnHeader9.Width = 100;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Time(时间)";
            this.columnHeader10.Width = 120;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "OpenMethod(开门方式)";
            this.columnHeader11.Width = 160;
            // 
            // button_attach
            // 
            this.button_attach.Location = new System.Drawing.Point(13, 34);
            this.button_attach.Name = "button_attach";
            this.button_attach.Size = new System.Drawing.Size(120, 23);
            this.button_attach.TabIndex = 10;
            this.button_attach.Text = "Attach(订阅)";
            this.button_attach.UseVisualStyleBackColor = true;
            this.button_attach.Click += new System.EventHandler(this.button_attach_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button_attach);
            this.groupBox6.Location = new System.Drawing.Point(624, 72);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(143, 85);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Operate(操作)";
            // 
            // AttendanceDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(769, 454);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "AttendanceDemo";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AttendanceDemo(考勤机Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_operatebyfingerprintid;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_modify;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.TextBox textBox_userid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView listView_information;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView listView_event;
        private System.Windows.Forms.Button button_attach;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button button_prepage;
        private System.Windows.Forms.Button button_nextpage;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Button button_operatebyuserid;
    }
}

