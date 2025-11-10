namespace WinForm_IPC_FaceRecognition_Demo
{
    partial class OperatePersonWindow
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
            this.groupbox = new System.Windows.Forms.GroupBox();
            this.dateTimePicker_endtime = new System.Windows.Forms.DateTimePicker();
            this.comboBox_sex = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_id = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.textBox_groupname = new System.Windows.Forms.TextBox();
            this.textBox_groupid = new System.Windows.Forms.TextBox();
            this.comboBox_idtype = new System.Windows.Forms.ComboBox();
            this.dateTimePicker_starttime = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listView_person = new System.Windows.Forms.ListView();
            this.UID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PersonName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Sex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BirthDay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IDType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IDNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_Add = new System.Windows.Forms.Button();
            this.button_Del = new System.Windows.Forms.Button();
            this.button_Modify = new System.Windows.Forms.Button();
            this.button_Search = new System.Windows.Forms.Button();
            this.button_PrePage = new System.Windows.Forms.Button();
            this.button_NextPage = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupbox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupbox
            // 
            this.groupbox.Controls.Add(this.dateTimePicker_endtime);
            this.groupbox.Controls.Add(this.comboBox_sex);
            this.groupbox.Controls.Add(this.label7);
            this.groupbox.Controls.Add(this.textBox_id);
            this.groupbox.Controls.Add(this.label8);
            this.groupbox.Controls.Add(this.textBox_name);
            this.groupbox.Controls.Add(this.textBox_groupname);
            this.groupbox.Controls.Add(this.textBox_groupid);
            this.groupbox.Controls.Add(this.comboBox_idtype);
            this.groupbox.Controls.Add(this.dateTimePicker_starttime);
            this.groupbox.Controls.Add(this.label6);
            this.groupbox.Controls.Add(this.label5);
            this.groupbox.Controls.Add(this.label4);
            this.groupbox.Controls.Add(this.label3);
            this.groupbox.Controls.Add(this.label2);
            this.groupbox.Controls.Add(this.label1);
            this.groupbox.Location = new System.Drawing.Point(3, 7);
            this.groupbox.Name = "groupbox";
            this.groupbox.Size = new System.Drawing.Size(670, 136);
            this.groupbox.TabIndex = 0;
            this.groupbox.TabStop = false;
            this.groupbox.Text = "Information(信息)";
            // 
            // dateTimePicker_endtime
            // 
            this.dateTimePicker_endtime.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker_endtime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_endtime.Location = new System.Drawing.Point(262, 104);
            this.dateTimePicker_endtime.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_endtime.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_endtime.Name = "dateTimePicker_endtime";
            this.dateTimePicker_endtime.ShowCheckBox = true;
            this.dateTimePicker_endtime.Size = new System.Drawing.Size(105, 21);
            this.dateTimePicker_endtime.TabIndex = 18;
            // 
            // comboBox_sex
            // 
            this.comboBox_sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sex.FormattingEnabled = true;
            this.comboBox_sex.Location = new System.Drawing.Point(550, 63);
            this.comboBox_sex.Name = "comboBox_sex";
            this.comboBox_sex.Size = new System.Drawing.Size(108, 20);
            this.comboBox_sex.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(479, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "Sex(姓别):";
            // 
            // textBox_id
            // 
            this.textBox_id.Location = new System.Drawing.Point(364, 63);
            this.textBox_id.Name = "textBox_id";
            this.textBox_id.Size = new System.Drawing.Size(109, 21);
            this.textBox_id.TabIndex = 15;
            this.textBox_id.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_id_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(236, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "-";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(552, 27);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(106, 21);
            this.textBox_name.TabIndex = 13;
            this.textBox_name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_name_KeyPress);
            // 
            // textBox_groupname
            // 
            this.textBox_groupname.Enabled = false;
            this.textBox_groupname.Location = new System.Drawing.Point(364, 26);
            this.textBox_groupname.Name = "textBox_groupname";
            this.textBox_groupname.Size = new System.Drawing.Size(109, 21);
            this.textBox_groupname.TabIndex = 12;
            // 
            // textBox_groupid
            // 
            this.textBox_groupid.Enabled = false;
            this.textBox_groupid.Location = new System.Drawing.Point(119, 27);
            this.textBox_groupid.Name = "textBox_groupid";
            this.textBox_groupid.Size = new System.Drawing.Size(112, 21);
            this.textBox_groupid.TabIndex = 11;
            // 
            // comboBox_idtype
            // 
            this.comboBox_idtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_idtype.FormattingEnabled = true;
            this.comboBox_idtype.Location = new System.Drawing.Point(119, 64);
            this.comboBox_idtype.Name = "comboBox_idtype";
            this.comboBox_idtype.Size = new System.Drawing.Size(112, 20);
            this.comboBox_idtype.TabIndex = 10;
            // 
            // dateTimePicker_starttime
            // 
            this.dateTimePicker_starttime.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker_starttime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_starttime.Location = new System.Drawing.Point(119, 104);
            this.dateTimePicker_starttime.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_starttime.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_starttime.Name = "dateTimePicker_starttime";
            this.dateTimePicker_starttime.ShowCheckBox = true;
            this.dateTimePicker_starttime.Size = new System.Drawing.Size(111, 21);
            this.dateTimePicker_starttime.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "BirthDay(生日):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(260, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "IDNumber(证件号):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "IDType(证件类型):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(475, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name(姓名):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(260, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "DBName(人脸库名):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "DBID(人脸库序号):";
            // 
            // listView_person
            // 
            this.listView_person.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.UID,
            this.PersonName,
            this.Sex,
            this.BirthDay,
            this.IDType,
            this.IDNumber});
            this.listView_person.FullRowSelect = true;
            this.listView_person.GridLines = true;
            this.listView_person.Location = new System.Drawing.Point(3, 217);
            this.listView_person.Name = "listView_person";
            this.listView_person.Size = new System.Drawing.Size(670, 218);
            this.listView_person.TabIndex = 1;
            this.listView_person.UseCompatibleStateImageBehavior = false;
            this.listView_person.View = System.Windows.Forms.View.Details;
            // 
            // UID
            // 
            this.UID.Text = "UID(序号)";
            this.UID.Width = 70;
            // 
            // PersonName
            // 
            this.PersonName.Text = "Name(姓名)";
            this.PersonName.Width = 110;
            // 
            // Sex
            // 
            this.Sex.Text = "Sex(姓别)";
            this.Sex.Width = 120;
            // 
            // BirthDay
            // 
            this.BirthDay.Text = "BirthDay(生日)";
            this.BirthDay.Width = 130;
            // 
            // IDType
            // 
            this.IDType.Text = "IDType(证件类型)";
            this.IDType.Width = 110;
            // 
            // IDNumber
            // 
            this.IDNumber.Text = "IDNumber(证件号)";
            this.IDNumber.Width = 120;
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(179, 23);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(94, 23);
            this.button_Add.TabIndex = 2;
            this.button_Add.Text = "Add(增加)";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_Del
            // 
            this.button_Del.Location = new System.Drawing.Point(519, 23);
            this.button_Del.Name = "button_Del";
            this.button_Del.Size = new System.Drawing.Size(101, 23);
            this.button_Del.TabIndex = 3;
            this.button_Del.Text = "Delete(删除)";
            this.button_Del.UseVisualStyleBackColor = true;
            this.button_Del.Click += new System.EventHandler(this.button_Del_Click);
            // 
            // button_Modify
            // 
            this.button_Modify.Location = new System.Drawing.Point(345, 23);
            this.button_Modify.Name = "button_Modify";
            this.button_Modify.Size = new System.Drawing.Size(102, 23);
            this.button_Modify.TabIndex = 4;
            this.button_Modify.Text = "Modify(修改)";
            this.button_Modify.UseVisualStyleBackColor = true;
            this.button_Modify.Click += new System.EventHandler(this.button_Modify_Click);
            // 
            // button_Search
            // 
            this.button_Search.Location = new System.Drawing.Point(9, 23);
            this.button_Search.Name = "button_Search";
            this.button_Search.Size = new System.Drawing.Size(98, 23);
            this.button_Search.TabIndex = 5;
            this.button_Search.Text = "Search(查找)";
            this.button_Search.UseVisualStyleBackColor = true;
            this.button_Search.Click += new System.EventHandler(this.button_Search_Click);
            // 
            // button_PrePage
            // 
            this.button_PrePage.Location = new System.Drawing.Point(409, 441);
            this.button_PrePage.Name = "button_PrePage";
            this.button_PrePage.Size = new System.Drawing.Size(121, 23);
            this.button_PrePage.TabIndex = 6;
            this.button_PrePage.Text = "PrePage(上一页)";
            this.button_PrePage.UseVisualStyleBackColor = true;
            this.button_PrePage.Click += new System.EventHandler(this.button_PrePage_Click);
            // 
            // button_NextPage
            // 
            this.button_NextPage.Location = new System.Drawing.Point(552, 441);
            this.button_NextPage.Name = "button_NextPage";
            this.button_NextPage.Size = new System.Drawing.Size(121, 23);
            this.button_NextPage.TabIndex = 7;
            this.button_NextPage.Text = "NextPage(下一页)";
            this.button_NextPage.UseVisualStyleBackColor = true;
            this.button_NextPage.Click += new System.EventHandler(this.button_NextPage_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_Search);
            this.groupBox1.Controls.Add(this.button_Add);
            this.groupBox1.Controls.Add(this.button_Del);
            this.groupBox1.Controls.Add(this.button_Modify);
            this.groupBox1.Location = new System.Drawing.Point(3, 149);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(670, 61);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operate(操作)";
            // 
            // OperatePersonWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(677, 468);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_NextPage);
            this.Controls.Add(this.button_PrePage);
            this.Controls.Add(this.listView_person);
            this.Controls.Add(this.groupbox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OperatePersonWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OperatePerson(操作人员)";
            this.groupbox.ResumeLayout(false);
            this.groupbox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupbox;
        private System.Windows.Forms.ListView listView_person;
        private System.Windows.Forms.ColumnHeader PersonName;
        private System.Windows.Forms.ColumnHeader Sex;
        private System.Windows.Forms.ColumnHeader BirthDay;
        private System.Windows.Forms.ColumnHeader IDType;
        private System.Windows.Forms.ColumnHeader IDNumber;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button_Del;
        private System.Windows.Forms.Button button_Modify;
        private System.Windows.Forms.Button button_Search;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_id;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.TextBox textBox_groupname;
        private System.Windows.Forms.TextBox textBox_groupid;
        private System.Windows.Forms.ComboBox comboBox_idtype;
        private System.Windows.Forms.DateTimePicker dateTimePicker_starttime;
        private System.Windows.Forms.Button button_PrePage;
        private System.Windows.Forms.Button button_NextPage;
        private System.Windows.Forms.ColumnHeader UID;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox_sex;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dateTimePicker_endtime;
    }
}