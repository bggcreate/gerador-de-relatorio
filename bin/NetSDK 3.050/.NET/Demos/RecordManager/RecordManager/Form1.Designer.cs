namespace RecordManager
{
    partial class Form1
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.listView_alarm = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_schedule = new System.Windows.Forms.Button();
            this.button_listen = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(770, 52);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(665, 22);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(99, 23);
            this.button_login.TabIndex = 8;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(568, 24);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '*';
            this.textBox_password.Size = new System.Drawing.Size(81, 21);
            this.textBox_password.TabIndex = 7;
            this.textBox_password.Text = "admin456";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(467, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password(密码):";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(366, 24);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(95, 21);
            this.textBox_name.TabIndex = 5;
            this.textBox_name.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(288, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Name(用户名):";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(237, 24);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(45, 21);
            this.textBox_port.TabIndex = 3;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port(端口):";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(79, 24);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(84, 21);
            this.textBox_ip.TabIndex = 1;
            this.textBox_ip.Text = "172.23.1.102";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(IP地址):";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.listView_alarm);
            this.groupBox5.Controls.Add(this.button_schedule);
            this.groupBox5.Controls.Add(this.button_listen);
            this.groupBox5.Location = new System.Drawing.Point(12, 79);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(770, 219);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "RecordManager(录像管理)";
            // 
            // listView_alarm
            // 
            this.listView_alarm.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView_alarm.GridLines = true;
            this.listView_alarm.Location = new System.Drawing.Point(7, 50);
            this.listView_alarm.Name = "listView_alarm";
            this.listView_alarm.Size = new System.Drawing.Size(757, 163);
            this.listView_alarm.TabIndex = 2;
            this.listView_alarm.UseCompatibleStateImageBehavior = false;
            this.listView_alarm.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time(时间)";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "EventName(事件名)";
            this.columnHeader5.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Channel(通道)";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Operater Name(操作者)";
            this.columnHeader3.Width = 140;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Message(信息)";
            this.columnHeader4.Width = 270;
            // 
            // button_schedule
            // 
            this.button_schedule.Location = new System.Drawing.Point(532, 20);
            this.button_schedule.Name = "button_schedule";
            this.button_schedule.Size = new System.Drawing.Size(232, 23);
            this.button_schedule.TabIndex = 1;
            this.button_schedule.Text = " Record Schedule(录像计划)";
            this.button_schedule.UseVisualStyleBackColor = true;
            this.button_schedule.Click += new System.EventHandler(this.button_schedule_Click);
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(196, 20);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(175, 23);
            this.button_listen.TabIndex = 0;
            this.button_listen.Text = "Start Listen(开始监听)";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(794, 315);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RecordManager(录像管理)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView listView_alarm;
        private System.Windows.Forms.Button button_schedule;
        private System.Windows.Forms.Button button_listen;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}

