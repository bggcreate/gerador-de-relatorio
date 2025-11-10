namespace FaceAttendance
{
    partial class AddInfoWnd
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cardNo_textBox = new System.Windows.Forms.TextBox();
            this.cardName_textBox = new System.Windows.Forms.TextBox();
            this.cardStatus_comboBox = new System.Windows.Forms.ComboBox();
            this.cardType_comboBox = new System.Windows.Forms.ComboBox();
            this.cardPwd_textBox = new System.Windows.Forms.TextBox();
            this.useTime_textBox = new System.Windows.Forms.TextBox();
            this.start_dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.end_dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.GetFaceData_button = new System.Windows.Forms.Button();
            this.Ok_button = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.add_userID_textBox = new System.Windows.Forms.TextBox();
            this.get_user_textBox = new System.Windows.Forms.TextBox();
            this.FaceData = new System.Windows.Forms.GroupBox();
            this.isFaceData = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ReaderID_textBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.ChannelID_textBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.is_collect_label = new System.Windows.Forms.Label();
            this.isFinger = new System.Windows.Forms.CheckBox();
            this.StartCollect_button = new System.Windows.Forms.Button();
            this.FaceData.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "CardNo(卡号):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "CardName(卡名):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "CardStatus(卡状态):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "CardType(卡类型):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "CardPwd(卡密码):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 192);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "UseTime(次数):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 221);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "StartTime(开始时间):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 248);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "EndTime(结束时间):";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "UserID(用户ID):";
            // 
            // cardNo_textBox
            // 
            this.cardNo_textBox.Location = new System.Drawing.Point(174, 47);
            this.cardNo_textBox.MaxLength = 32;
            this.cardNo_textBox.Name = "cardNo_textBox";
            this.cardNo_textBox.Size = new System.Drawing.Size(153, 21);
            this.cardNo_textBox.TabIndex = 1;
            this.cardNo_textBox.TextChanged += new System.EventHandler(this.cardNo_textBox_TextChanged);
            // 
            // cardName_textBox
            // 
            this.cardName_textBox.Location = new System.Drawing.Point(174, 74);
            this.cardName_textBox.MaxLength = 64;
            this.cardName_textBox.Name = "cardName_textBox";
            this.cardName_textBox.Size = new System.Drawing.Size(153, 21);
            this.cardName_textBox.TabIndex = 2;
            this.cardName_textBox.TextChanged += new System.EventHandler(this.cardName_textBox_TextChanged);
            // 
            // cardStatus_comboBox
            // 
            this.cardStatus_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardStatus_comboBox.FormattingEnabled = true;
            this.cardStatus_comboBox.Location = new System.Drawing.Point(174, 101);
            this.cardStatus_comboBox.Name = "cardStatus_comboBox";
            this.cardStatus_comboBox.Size = new System.Drawing.Size(153, 20);
            this.cardStatus_comboBox.TabIndex = 3;
            // 
            // cardType_comboBox
            // 
            this.cardType_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardType_comboBox.FormattingEnabled = true;
            this.cardType_comboBox.Location = new System.Drawing.Point(174, 127);
            this.cardType_comboBox.Name = "cardType_comboBox";
            this.cardType_comboBox.Size = new System.Drawing.Size(153, 20);
            this.cardType_comboBox.TabIndex = 4;
            // 
            // cardPwd_textBox
            // 
            this.cardPwd_textBox.Location = new System.Drawing.Point(174, 158);
            this.cardPwd_textBox.MaxLength = 64;
            this.cardPwd_textBox.Name = "cardPwd_textBox";
            this.cardPwd_textBox.Size = new System.Drawing.Size(153, 21);
            this.cardPwd_textBox.TabIndex = 5;
            this.cardPwd_textBox.TextChanged += new System.EventHandler(this.cardPwd_textBox_TextChanged);
            // 
            // useTime_textBox
            // 
            this.useTime_textBox.Location = new System.Drawing.Point(174, 188);
            this.useTime_textBox.Name = "useTime_textBox";
            this.useTime_textBox.Size = new System.Drawing.Size(153, 21);
            this.useTime_textBox.TabIndex = 6;
            this.useTime_textBox.Text = "0";
            // 
            // start_dateTimePicker
            // 
            this.start_dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.start_dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.start_dateTimePicker.Location = new System.Drawing.Point(174, 217);
            this.start_dateTimePicker.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.start_dateTimePicker.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.start_dateTimePicker.Name = "start_dateTimePicker";
            this.start_dateTimePicker.ShowUpDown = true;
            this.start_dateTimePicker.Size = new System.Drawing.Size(153, 21);
            this.start_dateTimePicker.TabIndex = 7;
            // 
            // end_dateTimePicker
            // 
            this.end_dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.end_dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.end_dateTimePicker.Location = new System.Drawing.Point(174, 246);
            this.end_dateTimePicker.MaxDate = new System.DateTime(2037, 12, 31, 0, 0, 0, 0);
            this.end_dateTimePicker.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.end_dateTimePicker.Name = "end_dateTimePicker";
            this.end_dateTimePicker.ShowUpDown = true;
            this.end_dateTimePicker.Size = new System.Drawing.Size(153, 21);
            this.end_dateTimePicker.TabIndex = 8;
            // 
            // GetFaceData_button
            // 
            this.GetFaceData_button.Location = new System.Drawing.Point(196, 41);
            this.GetFaceData_button.Name = "GetFaceData_button";
            this.GetFaceData_button.Size = new System.Drawing.Size(95, 27);
            this.GetFaceData_button.TabIndex = 9;
            this.GetFaceData_button.Text = "Get(获取)";
            this.GetFaceData_button.UseVisualStyleBackColor = true;
            this.GetFaceData_button.Click += new System.EventHandler(this.GetFaceData_button_Click);
            // 
            // Ok_button
            // 
            this.Ok_button.Location = new System.Drawing.Point(120, 513);
            this.Ok_button.Name = "Ok_button";
            this.Ok_button.Size = new System.Drawing.Size(79, 31);
            this.Ok_button.TabIndex = 10;
            this.Ok_button.Text = "OK";
            this.Ok_button.UseVisualStyleBackColor = true;
            this.Ok_button.Click += new System.EventHandler(this.Ok_button_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(34, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 12);
            this.label11.TabIndex = 21;
            this.label11.Text = "UserID(用户ID):";
            // 
            // add_userID_textBox
            // 
            this.add_userID_textBox.Location = new System.Drawing.Point(174, 19);
            this.add_userID_textBox.MaxLength = 32;
            this.add_userID_textBox.Name = "add_userID_textBox";
            this.add_userID_textBox.Size = new System.Drawing.Size(153, 21);
            this.add_userID_textBox.TabIndex = 0;
            this.add_userID_textBox.TextChanged += new System.EventHandler(this.add_userID_textBox_TextChanged);
            // 
            // get_user_textBox
            // 
            this.get_user_textBox.Location = new System.Drawing.Point(109, 43);
            this.get_user_textBox.Name = "get_user_textBox";
            this.get_user_textBox.Size = new System.Drawing.Size(50, 21);
            this.get_user_textBox.TabIndex = 22;
            // 
            // FaceData
            // 
            this.FaceData.Controls.Add(this.isFaceData);
            this.FaceData.Controls.Add(this.label10);
            this.FaceData.Controls.Add(this.get_user_textBox);
            this.FaceData.Controls.Add(this.GetFaceData_button);
            this.FaceData.Location = new System.Drawing.Point(36, 273);
            this.FaceData.Name = "FaceData";
            this.FaceData.Size = new System.Drawing.Size(300, 74);
            this.FaceData.TabIndex = 23;
            this.FaceData.TabStop = false;
            this.FaceData.Text = "FaceData(人脸模板数据)";
            // 
            // isFaceData
            // 
            this.isFaceData.AutoSize = true;
            this.isFaceData.Location = new System.Drawing.Point(6, 20);
            this.isFaceData.Name = "isFaceData";
            this.isFaceData.Size = new System.Drawing.Size(234, 16);
            this.isFaceData.TabIndex = 23;
            this.isFaceData.Text = "IsAddFaceData(是否添加人脸模板数据)";
            this.isFaceData.UseVisualStyleBackColor = true;
            this.isFaceData.CheckedChanged += new System.EventHandler(this.isFaceData_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ReaderID_textBox);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.ChannelID_textBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.is_collect_label);
            this.groupBox1.Controls.Add(this.isFinger);
            this.groupBox1.Controls.Add(this.StartCollect_button);
            this.groupBox1.Location = new System.Drawing.Point(36, 352);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 155);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FingerPrints(指纹数据)";
            // 
            // ReaderID_textBox
            // 
            this.ReaderID_textBox.Location = new System.Drawing.Point(138, 76);
            this.ReaderID_textBox.Name = "ReaderID_textBox";
            this.ReaderID_textBox.Size = new System.Drawing.Size(153, 21);
            this.ReaderID_textBox.TabIndex = 7;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 79);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(125, 12);
            this.label12.TabIndex = 6;
            this.label12.Text = "ReaderID(读卡器ID)：";
            // 
            // ChannelID_textBox
            // 
            this.ChannelID_textBox.Location = new System.Drawing.Point(138, 49);
            this.ChannelID_textBox.Name = "ChannelID_textBox";
            this.ChannelID_textBox.Size = new System.Drawing.Size(153, 21);
            this.ChannelID_textBox.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "Channel(通道)：";
            // 
            // is_collect_label
            // 
            this.is_collect_label.AutoSize = true;
            this.is_collect_label.Location = new System.Drawing.Point(93, 117);
            this.is_collect_label.Name = "is_collect_label";
            this.is_collect_label.Size = new System.Drawing.Size(131, 12);
            this.is_collect_label.TabIndex = 3;
            this.is_collect_label.Text = "No Collection(未采集)";
            // 
            // isFinger
            // 
            this.isFinger.AutoSize = true;
            this.isFinger.Location = new System.Drawing.Point(12, 22);
            this.isFinger.Name = "isFinger";
            this.isFinger.Size = new System.Drawing.Size(228, 16);
            this.isFinger.TabIndex = 2;
            this.isFinger.Text = "IsAddFingerPrint(是否添加指纹数据)";
            this.isFinger.UseVisualStyleBackColor = true;
            this.isFinger.CheckedChanged += new System.EventHandler(this.isFinger_CheckedChanged);
            // 
            // StartCollect_button
            // 
            this.StartCollect_button.AllowDrop = true;
            this.StartCollect_button.Location = new System.Drawing.Point(8, 103);
            this.StartCollect_button.Name = "StartCollect_button";
            this.StartCollect_button.Size = new System.Drawing.Size(75, 40);
            this.StartCollect_button.TabIndex = 1;
            this.StartCollect_button.Text = "Collection(采集)";
            this.StartCollect_button.UseVisualStyleBackColor = true;
            this.StartCollect_button.Click += new System.EventHandler(this.StartCollect_button_Click);
            // 
            // AddInfoWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 554);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.FaceData);
            this.Controls.Add(this.add_userID_textBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Ok_button);
            this.Controls.Add(this.end_dateTimePicker);
            this.Controls.Add(this.start_dateTimePicker);
            this.Controls.Add(this.useTime_textBox);
            this.Controls.Add(this.cardPwd_textBox);
            this.Controls.Add(this.cardType_comboBox);
            this.Controls.Add(this.cardStatus_comboBox);
            this.Controls.Add(this.cardName_textBox);
            this.Controls.Add(this.cardNo_textBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddInfoWnd";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddInfo(增加)";
            this.FaceData.ResumeLayout(false);
            this.FaceData.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox cardNo_textBox;
        private System.Windows.Forms.TextBox cardName_textBox;
        private System.Windows.Forms.ComboBox cardStatus_comboBox;
        private System.Windows.Forms.ComboBox cardType_comboBox;
        private System.Windows.Forms.TextBox cardPwd_textBox;
        private System.Windows.Forms.TextBox useTime_textBox;
        private System.Windows.Forms.DateTimePicker start_dateTimePicker;
        private System.Windows.Forms.DateTimePicker end_dateTimePicker;
        private System.Windows.Forms.Button GetFaceData_button;
        private System.Windows.Forms.Button Ok_button;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox add_userID_textBox;
        private System.Windows.Forms.TextBox get_user_textBox;
        private System.Windows.Forms.GroupBox FaceData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button StartCollect_button;
        private System.Windows.Forms.CheckBox isFinger;
        private System.Windows.Forms.Label is_collect_label;
        private System.Windows.Forms.TextBox ChannelID_textBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ReaderID_textBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox isFaceData;
    }
}