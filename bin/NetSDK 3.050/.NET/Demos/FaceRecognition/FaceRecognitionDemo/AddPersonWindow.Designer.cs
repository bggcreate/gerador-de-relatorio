namespace WinForm_IPC_FaceRecognition_Demo
{
    partial class AddPersonWindow
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
            this.pictureBox_person = new System.Windows.Forms.PictureBox();
            this.button_Open = new System.Windows.Forms.Button();
            this.button_picture_del = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dateTimePicker_birthday = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox_idtype = new System.Windows.Forms.ComboBox();
            this.comboBox_sex = new System.Windows.Forms.ComboBox();
            this.textBox_id = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.textBox_groupname = new System.Windows.Forms.TextBox();
            this.textBox_groupid = new System.Windows.Forms.TextBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_person)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox_person
            // 
            this.pictureBox_person.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_person.Location = new System.Drawing.Point(272, 12);
            this.pictureBox_person.Name = "pictureBox_person";
            this.pictureBox_person.Size = new System.Drawing.Size(212, 274);
            this.pictureBox_person.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_person.TabIndex = 0;
            this.pictureBox_person.TabStop = false;
            // 
            // button_Open
            // 
            this.button_Open.Location = new System.Drawing.Point(341, 126);
            this.button_Open.Name = "button_Open";
            this.button_Open.Size = new System.Drawing.Size(76, 23);
            this.button_Open.TabIndex = 14;
            this.button_Open.Text = "Open(打开)";
            this.button_Open.UseVisualStyleBackColor = true;
            this.button_Open.Click += new System.EventHandler(this.button_Open_Click);
            // 
            // button_picture_del
            // 
            this.button_picture_del.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_picture_del.FlatAppearance.BorderSize = 0;
            this.button_picture_del.Location = new System.Drawing.Point(468, 12);
            this.button_picture_del.Margin = new System.Windows.Forms.Padding(0);
            this.button_picture_del.Name = "button_picture_del";
            this.button_picture_del.Size = new System.Drawing.Size(16, 16);
            this.button_picture_del.TabIndex = 17;
            this.button_picture_del.Text = "-";
            this.button_picture_del.UseVisualStyleBackColor = false;
            this.button_picture_del.Visible = false;
            this.button_picture_del.Click += new System.EventHandler(this.button_picture_del_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dateTimePicker_birthday);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.comboBox_idtype);
            this.groupBox1.Controls.Add(this.comboBox_sex);
            this.groupBox1.Controls.Add(this.textBox_id);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.textBox_groupname);
            this.groupBox1.Controls.Add(this.textBox_groupid);
            this.groupBox1.Controls.Add(this.button_ok);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(263, 284);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operate(操作)";
            // 
            // dateTimePicker_birthday
            // 
            this.dateTimePicker_birthday.Location = new System.Drawing.Point(116, 215);
            this.dateTimePicker_birthday.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_birthday.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_birthday.Name = "dateTimePicker_birthday";
            this.dateTimePicker_birthday.ShowCheckBox = true;
            this.dateTimePicker_birthday.Size = new System.Drawing.Size(130, 21);
            this.dateTimePicker_birthday.TabIndex = 31;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 221);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 12);
            this.label7.TabIndex = 30;
            this.label7.Text = "BirthDay(生日):";
            // 
            // comboBox_idtype
            // 
            this.comboBox_idtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_idtype.FormattingEnabled = true;
            this.comboBox_idtype.Location = new System.Drawing.Point(116, 154);
            this.comboBox_idtype.Name = "comboBox_idtype";
            this.comboBox_idtype.Size = new System.Drawing.Size(130, 20);
            this.comboBox_idtype.TabIndex = 29;
            // 
            // comboBox_sex
            // 
            this.comboBox_sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sex.FormattingEnabled = true;
            this.comboBox_sex.Location = new System.Drawing.Point(116, 121);
            this.comboBox_sex.Name = "comboBox_sex";
            this.comboBox_sex.Size = new System.Drawing.Size(130, 20);
            this.comboBox_sex.TabIndex = 28;
            // 
            // textBox_id
            // 
            this.textBox_id.Location = new System.Drawing.Point(116, 184);
            this.textBox_id.Name = "textBox_id";
            this.textBox_id.Size = new System.Drawing.Size(130, 21);
            this.textBox_id.TabIndex = 27;
            this.textBox_id.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_id_KeyPress);
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(116, 90);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(130, 21);
            this.textBox_name.TabIndex = 26;
            this.textBox_name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_name_KeyPress);
            // 
            // textBox_groupname
            // 
            this.textBox_groupname.Enabled = false;
            this.textBox_groupname.Location = new System.Drawing.Point(116, 55);
            this.textBox_groupname.Name = "textBox_groupname";
            this.textBox_groupname.Size = new System.Drawing.Size(130, 21);
            this.textBox_groupname.TabIndex = 25;
            // 
            // textBox_groupid
            // 
            this.textBox_groupid.Enabled = false;
            this.textBox_groupid.Location = new System.Drawing.Point(116, 20);
            this.textBox_groupid.Name = "textBox_groupid";
            this.textBox_groupid.Size = new System.Drawing.Size(130, 21);
            this.textBox_groupid.TabIndex = 24;
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(62, 248);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(113, 23);
            this.button_ok.TabIndex = 23;
            this.button_ok.Text = "Ok(确认)";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "IDNumber(证件号):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "IDType(证件类型):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "Sex(姓别):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "Name(姓名):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "DBName(人脸库名):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "DBID(人脸库序号):";
            // 
            // AddPersonWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(490, 293);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_picture_del);
            this.Controls.Add(this.button_Open);
            this.Controls.Add(this.pictureBox_person);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddPersonWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddPerson(增加人员)";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_person)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_person;
        private System.Windows.Forms.Button button_Open;
        private System.Windows.Forms.Button button_picture_del;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePicker_birthday;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox_idtype;
        private System.Windows.Forms.ComboBox comboBox_sex;
        private System.Windows.Forms.TextBox textBox_id;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.TextBox textBox_groupname;
        private System.Windows.Forms.TextBox textBox_groupid;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}