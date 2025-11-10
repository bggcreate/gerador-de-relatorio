namespace AlarmDemo
{
    partial class AlarmDemo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            this.panel1 = new System.Windows.Forms.GroupBox();
            this.startlisten_button = new System.Windows.Forms.Button();
            this.login_button = new System.Windows.Forms.Button();
            this.port_textBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.name_textBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pwd_textBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ip_textBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.alarm_listView = new System.Windows.Forms.ListView();
            this.id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.channel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.startlisten_button);
            this.panel1.Controls.Add(this.login_button);
            this.panel1.Controls.Add(this.port_textBox);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.name_textBox);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.pwd_textBox);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.ip_textBox);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(560, 127);
            this.panel1.TabIndex = 5;
            this.panel1.TabStop = false;
            this.panel1.Text = "Device Login(设备登录)";
            // 
            // startlisten_button
            // 
            this.startlisten_button.Location = new System.Drawing.Point(290, 90);
            this.startlisten_button.Name = "startlisten_button";
            this.startlisten_button.Size = new System.Drawing.Size(153, 23);
            this.startlisten_button.TabIndex = 6;
            this.startlisten_button.Text = "StartListen(开始监听)";
            this.startlisten_button.UseVisualStyleBackColor = true;
            this.startlisten_button.Click += new System.EventHandler(this.startlisten_button_Click);
            // 
            // login_button
            // 
            this.login_button.Location = new System.Drawing.Point(113, 90);
            this.login_button.Name = "login_button";
            this.login_button.Size = new System.Drawing.Size(153, 23);
            this.login_button.TabIndex = 5;
            this.login_button.Text = "Login(登录)";
            this.login_button.UseVisualStyleBackColor = true;
            this.login_button.Click += new System.EventHandler(this.login_button_Click);
            // 
            // port_textBox
            // 
            this.port_textBox.Location = new System.Drawing.Point(376, 23);
            this.port_textBox.Name = "port_textBox";
            this.port_textBox.Size = new System.Drawing.Size(140, 23);
            this.port_textBox.TabIndex = 2;
            this.port_textBox.Text = "37777";
            this.port_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.port_textBox_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(288, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "Port(端口):";
            // 
            // name_textBox
            // 
            this.name_textBox.Location = new System.Drawing.Point(126, 50);
            this.name_textBox.Name = "name_textBox";
            this.name_textBox.Size = new System.Drawing.Size(140, 23);
            this.name_textBox.TabIndex = 3;
            this.name_textBox.Text = "admin";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(38, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "Name(用户):";
            // 
            // pwd_textBox
            // 
            this.pwd_textBox.Location = new System.Drawing.Point(376, 53);
            this.pwd_textBox.Name = "pwd_textBox";
            this.pwd_textBox.Size = new System.Drawing.Size(140, 23);
            this.pwd_textBox.TabIndex = 4;
            this.pwd_textBox.Text = "admin123";
            this.pwd_textBox.UseSystemPasswordChar = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(288, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "Pwd(密码):";
            // 
            // ip_textBox
            // 
            this.ip_textBox.Location = new System.Drawing.Point(126, 23);
            this.ip_textBox.Name = "ip_textBox";
            this.ip_textBox.Size = new System.Drawing.Size(140, 23);
            this.ip_textBox.TabIndex = 1;
            this.ip_textBox.Text = "172.23.1.102";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "IP(设备IP):";
            // 
            // alarm_listView
            // 
            this.alarm_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.id,
            this.time,
            this.channel,
            this.message});
            this.alarm_listView.FullRowSelect = true;
            this.alarm_listView.GridLines = true;
            this.alarm_listView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.alarm_listView.Location = new System.Drawing.Point(12, 145);
            this.alarm_listView.MultiSelect = false;
            this.alarm_listView.Name = "alarm_listView";
            this.alarm_listView.Size = new System.Drawing.Size(560, 285);
            this.alarm_listView.TabIndex = 7;
            this.alarm_listView.TabStop = false;
            this.alarm_listView.UseCompatibleStateImageBehavior = false;
            this.alarm_listView.View = System.Windows.Forms.View.Details;
            // 
            // id
            // 
            this.id.Text = "ID(序列号)";
            this.id.Width = 80;
            // 
            // time
            // 
            this.time.Text = "Time(时间)";
            this.time.Width = 140;
            // 
            // channel
            // 
            this.channel.Text = "channel(通道)";
            this.channel.Width = 100;
            // 
            // message
            // 
            this.message.Text = "AlarmMessage(报警信息)";
            this.message.Width = 300;
            // 
            // AlarmDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(584, 442);
            this.Controls.Add(this.alarm_listView);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft New Tai Lue", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximizeBox = false;
            this.Name = "AlarmDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AlarmDemo(报警Demo)";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox panel1;
        private System.Windows.Forms.Button startlisten_button;
        private System.Windows.Forms.Button login_button;
        private System.Windows.Forms.TextBox port_textBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox name_textBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox pwd_textBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ip_textBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView alarm_listView;
        private System.Windows.Forms.ColumnHeader id;
        private System.Windows.Forms.ColumnHeader time;
        private System.Windows.Forms.ColumnHeader channel;
        private System.Windows.Forms.ColumnHeader message;
    }
}

