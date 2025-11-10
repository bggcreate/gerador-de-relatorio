namespace VTODemo
{
    partial class OperateInfo
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
            this.textBox_roomNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_cardno = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_get = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label_isGet = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_open = new System.Windows.Forms.Button();
            this.pictureBox_face = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_face)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_roomNo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox_cardno);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 87);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CardInfo(卡信息)";
            // 
            // textBox_roomNo
            // 
            this.textBox_roomNo.Location = new System.Drawing.Point(135, 50);
            this.textBox_roomNo.Name = "textBox_roomNo";
            this.textBox_roomNo.Size = new System.Drawing.Size(145, 21);
            this.textBox_roomNo.TabIndex = 32;
            this.textBox_roomNo.TextChanged += new System.EventHandler(this.textBox_roomNo_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 31;
            this.label3.Text = "RoomNo.(门间号):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(405, 188);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 27;
            this.label8.Text = "-";
            // 
            // textBox_cardno
            // 
            this.textBox_cardno.Location = new System.Drawing.Point(135, 21);
            this.textBox_cardno.Name = "textBox_cardno";
            this.textBox_cardno.Size = new System.Drawing.Size(145, 21);
            this.textBox_cardno.TabIndex = 20;
            this.textBox_cardno.TextChanged += new System.EventHandler(this.textBox_cardno_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "CardNo.(卡号):";
            // 
            // button_get
            // 
            this.button_get.Location = new System.Drawing.Point(135, 37);
            this.button_get.Name = "button_get";
            this.button_get.Size = new System.Drawing.Size(145, 23);
            this.button_get.TabIndex = 29;
            this.button_get.Text = "Get(获取)";
            this.button_get.UseVisualStyleBackColor = true;
            this.button_get.Click += new System.EventHandler(this.button_get_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 12);
            this.label9.TabIndex = 28;
            this.label9.Text = "Fingerprint(指纹):";
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(208, 223);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(98, 23);
            this.button_ok.TabIndex = 1;
            this.button_ok.Text = "OK(确定)";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(168, 15);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(16, 16);
            this.button_delete.TabIndex = 2;
            this.button_delete.Text = "-";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label_isGet);
            this.groupBox3.Controls.Add(this.button_get);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(8, 107);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(307, 100);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "FingerPrint Info(指纹信息)";
            // 
            // label_isGet
            // 
            this.label_isGet.AutoSize = true;
            this.label_isGet.Location = new System.Drawing.Point(85, 73);
            this.label_isGet.Name = "label_isGet";
            this.label_isGet.Size = new System.Drawing.Size(41, 12);
            this.label_isGet.TabIndex = 30;
            this.label_isGet.Text = "label1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button_open);
            this.groupBox4.Controls.Add(this.button_delete);
            this.groupBox4.Controls.Add(this.pictureBox_face);
            this.groupBox4.Location = new System.Drawing.Point(322, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(194, 200);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Face Info(人脸信息)";
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(59, 85);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(75, 23);
            this.button_open.TabIndex = 3;
            this.button_open.Text = "打开(Open)";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // pictureBox_face
            // 
            this.pictureBox_face.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_face.Location = new System.Drawing.Point(6, 15);
            this.pictureBox_face.Name = "pictureBox_face";
            this.pictureBox_face.Size = new System.Drawing.Size(178, 179);
            this.pictureBox_face.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_face.TabIndex = 0;
            this.pictureBox_face.TabStop = false;
            // 
            // OperateInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(530, 261);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OperateInfo";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add(增加)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_face)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_cardno;
        private System.Windows.Forms.Button button_get;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.TextBox textBox_roomNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.PictureBox pictureBox_face;
        private System.Windows.Forms.Label label_isGet;
    }
}