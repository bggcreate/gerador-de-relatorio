namespace IntelligentTrafficDemo
{
    partial class IntelligentTrafficDemo
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.port_textBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.login_button = new System.Windows.Forms.Button();
            this.name_textBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pwd_textBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ip_textBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.event_listView = new System.Windows.Forms.ListView();
            this.id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eventtype = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.count = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PlateNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.platetype = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.platecolor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.vehicletype = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.vehiclecolor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.vehiclesize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lanenumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openstrobe_button = new System.Windows.Forms.Button();
            this.subscribe_button = new System.Windows.Forms.Button();
            this.manualsnap_button = new System.Windows.Forms.Button();
            this.channel_comboBox = new System.Windows.Forms.ComboBox();
            this.realplay_button = new System.Windows.Forms.Button();
            this.realplay_pictureBox = new System.Windows.Forms.PictureBox();
            this.pic_pictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.GroupBox();
            this.lanenumber_textBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.platenumber_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.vehiclecolor_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.vehicletype_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.platecolor_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.platetype_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.attach_pictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.realplay_pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_pictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attach_pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.port_textBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.login_button);
            this.groupBox1.Controls.Add(this.name_textBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.pwd_textBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.ip_textBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(769, 60);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // port_textBox
            // 
            this.port_textBox.Location = new System.Drawing.Point(250, 27);
            this.port_textBox.Name = "port_textBox";
            this.port_textBox.Size = new System.Drawing.Size(43, 23);
            this.port_textBox.TabIndex = 1;
            this.port_textBox.Text = "37777";
            this.port_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.port_textBox_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(179, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 12);
            this.label9.TabIndex = 18;
            this.label9.Text = "Port(端口):";
            // 
            // login_button
            // 
            this.login_button.Location = new System.Drawing.Point(625, 25);
            this.login_button.Name = "login_button";
            this.login_button.Size = new System.Drawing.Size(115, 23);
            this.login_button.TabIndex = 4;
            this.login_button.Text = "Login(登录)";
            this.login_button.UseVisualStyleBackColor = true;
            this.login_button.Click += new System.EventHandler(this.login_button_Click);
            // 
            // name_textBox
            // 
            this.name_textBox.Location = new System.Drawing.Point(367, 27);
            this.name_textBox.Name = "name_textBox";
            this.name_textBox.Size = new System.Drawing.Size(53, 23);
            this.name_textBox.TabIndex = 2;
            this.name_textBox.Text = "admin";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(296, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 12);
            this.label8.TabIndex = 13;
            this.label8.Text = "Name(用户):";
            // 
            // pwd_textBox
            // 
            this.pwd_textBox.Location = new System.Drawing.Point(497, 27);
            this.pwd_textBox.Name = "pwd_textBox";
            this.pwd_textBox.Size = new System.Drawing.Size(103, 23);
            this.pwd_textBox.TabIndex = 3;
            this.pwd_textBox.Text = "admin123";
            this.pwd_textBox.UseSystemPasswordChar = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(426, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "Pwd(密码):";
            // 
            // ip_textBox
            // 
            this.ip_textBox.Location = new System.Drawing.Point(83, 27);
            this.ip_textBox.Name = "ip_textBox";
            this.ip_textBox.Size = new System.Drawing.Size(92, 23);
            this.ip_textBox.TabIndex = 0;
            this.ip_textBox.Text = "192.168.2.67";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "IP(设备IP):";
            // 
            // event_listView
            // 
            this.event_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.id,
            this.time,
            this.eventtype,
            this.groupid,
            this.index,
            this.count,
            this.PlateNumber,
            this.platetype,
            this.platecolor,
            this.vehicletype,
            this.vehiclecolor,
            this.vehiclesize,
            this.lanenumber,
            this.address});
            this.event_listView.FullRowSelect = true;
            this.event_listView.GridLines = true;
            this.event_listView.Location = new System.Drawing.Point(3, 383);
            this.event_listView.MultiSelect = false;
            this.event_listView.Name = "event_listView";
            this.event_listView.Size = new System.Drawing.Size(769, 138);
            this.event_listView.TabIndex = 3;
            this.event_listView.TabStop = false;
            this.event_listView.UseCompatibleStateImageBehavior = false;
            this.event_listView.View = System.Windows.Forms.View.Details;
            // 
            // id
            // 
            this.id.Text = "ID(序列号)";
            this.id.Width = 80;
            // 
            // time
            // 
            this.time.Text = "Time(时间)";
            this.time.Width = 100;
            // 
            // eventtype
            // 
            this.eventtype.Text = "Event(事件)";
            this.eventtype.Width = 140;
            // 
            // groupid
            // 
            this.groupid.Text = "GroupID(组ID)";
            this.groupid.Width = 100;
            // 
            // index
            // 
            this.index.Text = "Index(编号)";
            this.index.Width = 80;
            // 
            // count
            // 
            this.count.Text = "Count(总数)";
            this.count.Width = 80;
            // 
            // PlateNumber
            // 
            this.PlateNumber.Text = "PlateNumber(车牌号)";
            this.PlateNumber.Width = 100;
            // 
            // platetype
            // 
            this.platetype.Text = "PlateType(车牌类型)";
            this.platetype.Width = 100;
            // 
            // platecolor
            // 
            this.platecolor.Text = "PlateColor(车牌颜色)";
            this.platecolor.Width = 100;
            // 
            // vehicletype
            // 
            this.vehicletype.Text = "VehicleType(车身类型)";
            this.vehicletype.Width = 120;
            // 
            // vehiclecolor
            // 
            this.vehiclecolor.Text = "VehicleColor(车身颜色)";
            this.vehiclecolor.Width = 120;
            // 
            // vehiclesize
            // 
            this.vehiclesize.Text = "VehicleSize(车身大小)";
            this.vehiclesize.Width = 100;
            // 
            // lanenumber
            // 
            this.lanenumber.Text = "LaneNumber(车道号)";
            this.lanenumber.Width = 100;
            // 
            // address
            // 
            this.address.Text = "Illegal Address(违规地点)";
            this.address.Width = 300;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.openstrobe_button);
            this.groupBox2.Controls.Add(this.subscribe_button);
            this.groupBox2.Controls.Add(this.manualsnap_button);
            this.groupBox2.Controls.Add(this.channel_comboBox);
            this.groupBox2.Controls.Add(this.realplay_button);
            this.groupBox2.Location = new System.Drawing.Point(3, 79);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(769, 57);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operate(操作)";
            // 
            // openstrobe_button
            // 
            this.openstrobe_button.Location = new System.Drawing.Point(273, 20);
            this.openstrobe_button.Name = "openstrobe_button";
            this.openstrobe_button.Size = new System.Drawing.Size(136, 23);
            this.openstrobe_button.TabIndex = 7;
            this.openstrobe_button.Text = "OpenStrobe(打开道闸)";
            this.openstrobe_button.UseVisualStyleBackColor = true;
            this.openstrobe_button.Click += new System.EventHandler(this.openstrobe_button_Click);
            // 
            // subscribe_button
            // 
            this.subscribe_button.Location = new System.Drawing.Point(413, 20);
            this.subscribe_button.Name = "subscribe_button";
            this.subscribe_button.Size = new System.Drawing.Size(194, 23);
            this.subscribe_button.TabIndex = 8;
            this.subscribe_button.Text = "SubscribeEvent(订阅事件)";
            this.subscribe_button.UseVisualStyleBackColor = true;
            this.subscribe_button.Click += new System.EventHandler(this.subscribe_button_Click);
            // 
            // manualsnap_button
            // 
            this.manualsnap_button.Location = new System.Drawing.Point(610, 20);
            this.manualsnap_button.Name = "manualsnap_button";
            this.manualsnap_button.Size = new System.Drawing.Size(144, 23);
            this.manualsnap_button.TabIndex = 9;
            this.manualsnap_button.Text = "ManualSnap(手动抓拍)";
            this.manualsnap_button.UseVisualStyleBackColor = true;
            this.manualsnap_button.Click += new System.EventHandler(this.manualsnap_button_Click);
            // 
            // channel_comboBox
            // 
            this.channel_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channel_comboBox.FormattingEnabled = true;
            this.channel_comboBox.Location = new System.Drawing.Point(6, 22);
            this.channel_comboBox.Name = "channel_comboBox";
            this.channel_comboBox.Size = new System.Drawing.Size(125, 20);
            this.channel_comboBox.TabIndex = 5;
            // 
            // realplay_button
            // 
            this.realplay_button.Location = new System.Drawing.Point(137, 20);
            this.realplay_button.Name = "realplay_button";
            this.realplay_button.Size = new System.Drawing.Size(130, 23);
            this.realplay_button.TabIndex = 6;
            this.realplay_button.Text = "StartReal(开始监视)";
            this.realplay_button.UseVisualStyleBackColor = true;
            this.realplay_button.Click += new System.EventHandler(this.realplay_button_Click);
            // 
            // realplay_pictureBox
            // 
            this.realplay_pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.realplay_pictureBox.Location = new System.Drawing.Point(3, 143);
            this.realplay_pictureBox.Name = "realplay_pictureBox";
            this.realplay_pictureBox.Size = new System.Drawing.Size(283, 234);
            this.realplay_pictureBox.TabIndex = 5;
            this.realplay_pictureBox.TabStop = false;
            // 
            // pic_pictureBox
            // 
            this.pic_pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_pictureBox.Location = new System.Drawing.Point(499, 143);
            this.pic_pictureBox.Name = "pic_pictureBox";
            this.pic_pictureBox.Size = new System.Drawing.Size(273, 234);
            this.pic_pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_pictureBox.TabIndex = 6;
            this.pic_pictureBox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lanenumber_textBox);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.platenumber_textBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.vehiclecolor_textBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.vehicletype_textBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.platecolor_textBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.platetype_textBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.attach_pictureBox);
            this.panel1.Location = new System.Drawing.Point(293, 143);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 234);
            this.panel1.TabIndex = 7;
            this.panel1.TabStop = false;
            this.panel1.Text = "Vehicle Info(车辆信息)";
            // 
            // lanenumber_textBox
            // 
            this.lanenumber_textBox.Location = new System.Drawing.Point(79, 202);
            this.lanenumber_textBox.Name = "lanenumber_textBox";
            this.lanenumber_textBox.ReadOnly = true;
            this.lanenumber_textBox.Size = new System.Drawing.Size(114, 23);
            this.lanenumber_textBox.TabIndex = 12;
            this.lanenumber_textBox.TabStop = false;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(3, 199);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 24);
            this.label10.TabIndex = 11;
            this.label10.Text = "LaneNumber车道号";
            // 
            // platenumber_textBox
            // 
            this.platenumber_textBox.Location = new System.Drawing.Point(79, 172);
            this.platenumber_textBox.Name = "platenumber_textBox";
            this.platenumber_textBox.ReadOnly = true;
            this.platenumber_textBox.Size = new System.Drawing.Size(114, 23);
            this.platenumber_textBox.TabIndex = 10;
            this.platenumber_textBox.TabStop = false;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 24);
            this.label5.TabIndex = 9;
            this.label5.Text = "PlateNumber车牌号";
            // 
            // vehiclecolor_textBox
            // 
            this.vehiclecolor_textBox.Location = new System.Drawing.Point(79, 145);
            this.vehiclecolor_textBox.Name = "vehiclecolor_textBox";
            this.vehiclecolor_textBox.ReadOnly = true;
            this.vehiclecolor_textBox.Size = new System.Drawing.Size(114, 23);
            this.vehiclecolor_textBox.TabIndex = 8;
            this.vehiclecolor_textBox.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 24);
            this.label4.TabIndex = 7;
            this.label4.Text = "VehicleColor车身颜色";
            // 
            // vehicletype_textBox
            // 
            this.vehicletype_textBox.Location = new System.Drawing.Point(79, 118);
            this.vehicletype_textBox.Name = "vehicletype_textBox";
            this.vehicletype_textBox.ReadOnly = true;
            this.vehicletype_textBox.Size = new System.Drawing.Size(114, 23);
            this.vehicletype_textBox.TabIndex = 6;
            this.vehicletype_textBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "VehicleType车身类型";
            // 
            // platecolor_textBox
            // 
            this.platecolor_textBox.Location = new System.Drawing.Point(79, 91);
            this.platecolor_textBox.Name = "platecolor_textBox";
            this.platecolor_textBox.ReadOnly = true;
            this.platecolor_textBox.Size = new System.Drawing.Size(114, 23);
            this.platecolor_textBox.TabIndex = 4;
            this.platecolor_textBox.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "PlateColor车牌颜色";
            // 
            // platetype_textBox
            // 
            this.platetype_textBox.Location = new System.Drawing.Point(79, 64);
            this.platetype_textBox.Name = "platetype_textBox";
            this.platetype_textBox.ReadOnly = true;
            this.platetype_textBox.Size = new System.Drawing.Size(114, 23);
            this.platetype_textBox.TabIndex = 2;
            this.platetype_textBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Tai Le", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(3, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "PlateType车牌类型";
            // 
            // attach_pictureBox
            // 
            this.attach_pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.attach_pictureBox.Location = new System.Drawing.Point(3, 20);
            this.attach_pictureBox.Name = "attach_pictureBox";
            this.attach_pictureBox.Size = new System.Drawing.Size(194, 36);
            this.attach_pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.attach_pictureBox.TabIndex = 0;
            this.attach_pictureBox.TabStop = false;
            // 
            // IntelligentTrafficDemo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(774, 522);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pic_pictureBox);
            this.Controls.Add(this.realplay_pictureBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.event_listView);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft New Tai Lue", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximizeBox = false;
            this.Name = "IntelligentTrafficDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IntelligentTrafficDemo(智能交通Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.realplay_pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_pictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attach_pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox port_textBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button login_button;
        private System.Windows.Forms.TextBox name_textBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox pwd_textBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ip_textBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView event_listView;
        private System.Windows.Forms.ColumnHeader id;
        private System.Windows.Forms.ColumnHeader time;
        private System.Windows.Forms.ColumnHeader groupid;
        private System.Windows.Forms.ColumnHeader eventtype;
        private System.Windows.Forms.ColumnHeader address;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button manualsnap_button;
        private System.Windows.Forms.ComboBox channel_comboBox;
        private System.Windows.Forms.Button realplay_button;
        private System.Windows.Forms.PictureBox realplay_pictureBox;
        private System.Windows.Forms.PictureBox pic_pictureBox;
        private System.Windows.Forms.GroupBox panel1;
        private System.Windows.Forms.TextBox lanenumber_textBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox platenumber_textBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox vehiclecolor_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox vehicletype_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox platecolor_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox platetype_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox attach_pictureBox;
        private System.Windows.Forms.Button subscribe_button;
        private System.Windows.Forms.ColumnHeader index;
        private System.Windows.Forms.ColumnHeader PlateNumber;
        private System.Windows.Forms.ColumnHeader count;
        private System.Windows.Forms.ColumnHeader platetype;
        private System.Windows.Forms.ColumnHeader platecolor;
        private System.Windows.Forms.ColumnHeader vehicletype;
        private System.Windows.Forms.ColumnHeader vehiclecolor;
        private System.Windows.Forms.ColumnHeader vehiclesize;
        private System.Windows.Forms.ColumnHeader lanenumber;
        private System.Windows.Forms.Button openstrobe_button;
    }
}

