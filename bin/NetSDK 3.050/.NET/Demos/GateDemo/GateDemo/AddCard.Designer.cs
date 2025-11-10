namespace GateDemo
{
    partial class AddCard
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
            this.checkBox_valid = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dateTimePicker_end = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_start = new System.Windows.Forms.DateTimePicker();
            this.checkBox_isfirstcard = new System.Windows.Forms.CheckBox();
            this.textBox_usetime = new System.Windows.Forms.TextBox();
            this.comboBox_cardtype = new System.Windows.Forms.ComboBox();
            this.comboBox_cardstatus = new System.Windows.Forms.ComboBox();
            this.textBox_cardpassword = new System.Windows.Forms.TextBox();
            this.textBox_cardname = new System.Windows.Forms.TextBox();
            this.textBox_userid = new System.Windows.Forms.TextBox();
            this.textBox_cardno = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_open = new System.Windows.Forms.Button();
            this.pictureBox_face = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_face)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_valid);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.dateTimePicker_end);
            this.groupBox1.Controls.Add(this.dateTimePicker_start);
            this.groupBox1.Controls.Add(this.checkBox_isfirstcard);
            this.groupBox1.Controls.Add(this.textBox_usetime);
            this.groupBox1.Controls.Add(this.comboBox_cardtype);
            this.groupBox1.Controls.Add(this.comboBox_cardstatus);
            this.groupBox1.Controls.Add(this.textBox_cardpassword);
            this.groupBox1.Controls.Add(this.textBox_cardname);
            this.groupBox1.Controls.Add(this.textBox_userid);
            this.groupBox1.Controls.Add(this.textBox_cardno);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(559, 261);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Information(信息)";
            // 
            // checkBox_valid
            // 
            this.checkBox_valid.AutoSize = true;
            this.checkBox_valid.Location = new System.Drawing.Point(9, 215);
            this.checkBox_valid.Name = "checkBox_valid";
            this.checkBox_valid.Size = new System.Drawing.Size(96, 16);
            this.checkBox_valid.TabIndex = 21;
            this.checkBox_valid.Text = "Enable(使能)";
            this.checkBox_valid.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(387, 218);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "-";
            // 
            // dateTimePicker_end
            // 
            this.dateTimePicker_end.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker_end.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_end.Location = new System.Drawing.Point(407, 212);
            this.dateTimePicker_end.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_end.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_end.Name = "dateTimePicker_end";
            this.dateTimePicker_end.ShowUpDown = true;
            this.dateTimePicker_end.Size = new System.Drawing.Size(144, 21);
            this.dateTimePicker_end.TabIndex = 19;
            // 
            // dateTimePicker_start
            // 
            this.dateTimePicker_start.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker_start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_start.Location = new System.Drawing.Point(237, 212);
            this.dateTimePicker_start.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker_start.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_start.Name = "dateTimePicker_start";
            this.dateTimePicker_start.ShowUpDown = true;
            this.dateTimePicker_start.Size = new System.Drawing.Size(144, 21);
            this.dateTimePicker_start.TabIndex = 18;
            // 
            // checkBox_isfirstcard
            // 
            this.checkBox_isfirstcard.AutoSize = true;
            this.checkBox_isfirstcard.Location = new System.Drawing.Point(401, 165);
            this.checkBox_isfirstcard.Name = "checkBox_isfirstcard";
            this.checkBox_isfirstcard.Size = new System.Drawing.Size(156, 16);
            this.checkBox_isfirstcard.TabIndex = 17;
            this.checkBox_isfirstcard.Text = "IsFirstEnter(是否首卡)";
            this.checkBox_isfirstcard.UseVisualStyleBackColor = true;
            // 
            // textBox_usetime
            // 
            this.textBox_usetime.Location = new System.Drawing.Point(128, 165);
            this.textBox_usetime.Name = "textBox_usetime";
            this.textBox_usetime.Size = new System.Drawing.Size(147, 21);
            this.textBox_usetime.TabIndex = 16;
            this.textBox_usetime.Text = "0";
            this.textBox_usetime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_usertime_KeyPress);
            // 
            // comboBox_cardtype
            // 
            this.comboBox_cardtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_cardtype.FormattingEnabled = true;
            this.comboBox_cardtype.Location = new System.Drawing.Point(412, 120);
            this.comboBox_cardtype.Name = "comboBox_cardtype";
            this.comboBox_cardtype.Size = new System.Drawing.Size(139, 20);
            this.comboBox_cardtype.TabIndex = 15;
            // 
            // comboBox_cardstatus
            // 
            this.comboBox_cardstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_cardstatus.FormattingEnabled = true;
            this.comboBox_cardstatus.Location = new System.Drawing.Point(128, 120);
            this.comboBox_cardstatus.Name = "comboBox_cardstatus";
            this.comboBox_cardstatus.Size = new System.Drawing.Size(147, 20);
            this.comboBox_cardstatus.TabIndex = 14;
            // 
            // textBox_cardpassword
            // 
            this.textBox_cardpassword.Location = new System.Drawing.Point(412, 71);
            this.textBox_cardpassword.Name = "textBox_cardpassword";
            this.textBox_cardpassword.PasswordChar = '*';
            this.textBox_cardpassword.Size = new System.Drawing.Size(139, 21);
            this.textBox_cardpassword.TabIndex = 13;
            this.textBox_cardpassword.TextChanged += new System.EventHandler(this.textBox_cardpassword_TextChanged);
            // 
            // textBox_cardname
            // 
            this.textBox_cardname.Location = new System.Drawing.Point(128, 71);
            this.textBox_cardname.Name = "textBox_cardname";
            this.textBox_cardname.Size = new System.Drawing.Size(147, 21);
            this.textBox_cardname.TabIndex = 12;
            this.textBox_cardname.TextChanged += new System.EventHandler(this.textBox_cardname_TextChanged);
            // 
            // textBox_userid
            // 
            this.textBox_userid.Location = new System.Drawing.Point(412, 29);
            this.textBox_userid.Name = "textBox_userid";
            this.textBox_userid.Size = new System.Drawing.Size(139, 21);
            this.textBox_userid.TabIndex = 11;
            this.textBox_userid.TextChanged += new System.EventHandler(this.textBox_userid_TextChanged);
            // 
            // textBox_cardno
            // 
            this.textBox_cardno.Location = new System.Drawing.Point(128, 29);
            this.textBox_cardno.Name = "textBox_cardno";
            this.textBox_cardno.Size = new System.Drawing.Size(147, 21);
            this.textBox_cardno.TabIndex = 10;
            this.textBox_cardno.TextChanged += new System.EventHandler(this.textBox_cardno_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(126, 216);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "Period(有效期):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "UseTime(使用次数):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(302, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "CardType(卡类型):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "CardStatus(卡状态):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(284, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "CardPasswod(卡密码):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(302, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "UserID(用户序号):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "CardName(卡名):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "CardNo(卡编号):";
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(234, 288);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(125, 23);
            this.button_ok.TabIndex = 1;
            this.button_ok.Text = "OK(确定)";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(401, 288);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(117, 23);
            this.button_cancel.TabIndex = 2;
            this.button_cancel.Text = "Cancel(取消)";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_delete);
            this.groupBox2.Controls.Add(this.button_open);
            this.groupBox2.Controls.Add(this.pictureBox_face);
            this.groupBox2.Location = new System.Drawing.Point(573, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 261);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image(图片)";
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(178, 20);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(16, 16);
            this.button_delete.TabIndex = 2;
            this.button_delete.Text = "-";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(64, 117);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(75, 23);
            this.button_open.TabIndex = 1;
            this.button_open.Text = "Open(打开)";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // pictureBox_face
            // 
            this.pictureBox_face.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_face.Location = new System.Drawing.Point(6, 20);
            this.pictureBox_face.Name = "pictureBox_face";
            this.pictureBox_face.Size = new System.Drawing.Size(188, 235);
            this.pictureBox_face.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_face.TabIndex = 0;
            this.pictureBox_face.TabStop = false;
            // 
            // AddCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(779, 323);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddCard";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddCard(增加卡)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_face)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_cardno;
        private System.Windows.Forms.TextBox textBox_userid;
        private System.Windows.Forms.TextBox textBox_cardname;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dateTimePicker_end;
        private System.Windows.Forms.DateTimePicker dateTimePicker_start;
        private System.Windows.Forms.CheckBox checkBox_isfirstcard;
        private System.Windows.Forms.TextBox textBox_usetime;
        private System.Windows.Forms.ComboBox comboBox_cardtype;
        private System.Windows.Forms.ComboBox comboBox_cardstatus;
        private System.Windows.Forms.TextBox textBox_cardpassword;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox_face;
        private System.Windows.Forms.CheckBox checkBox_valid;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_open;
    }
}