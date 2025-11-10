namespace TalkDemo
{
    partial class TalkDemo
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
            this.login_button = new System.Windows.Forms.Button();
            this.password_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.name_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.port_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ip_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.remote_radioButton = new System.Windows.Forms.RadioButton();
            this.local_radioButton = new System.Windows.Forms.RadioButton();
            this.talk_button = new System.Windows.Forms.Button();
            this.channel_comboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.login_button);
            this.groupBox1.Controls.Add(this.password_textBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.name_textBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.port_textBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ip_textBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 145);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // login_button
            // 
            this.login_button.Location = new System.Drawing.Point(121, 106);
            this.login_button.Name = "login_button";
            this.login_button.Size = new System.Drawing.Size(124, 23);
            this.login_button.TabIndex = 4;
            this.login_button.Text = "Login(登录)";
            this.login_button.UseVisualStyleBackColor = true;
            this.login_button.Click += new System.EventHandler(this.login_button_Click);
            // 
            // password_textBox
            // 
            this.password_textBox.Location = new System.Drawing.Point(261, 69);
            this.password_textBox.Name = "password_textBox";
            this.password_textBox.Size = new System.Drawing.Size(100, 23);
            this.password_textBox.TabIndex = 3;
            this.password_textBox.Text = "admin";
            this.password_textBox.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(191, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Pwd(密码):";
            // 
            // name_textBox
            // 
            this.name_textBox.Location = new System.Drawing.Point(85, 66);
            this.name_textBox.Name = "name_textBox";
            this.name_textBox.Size = new System.Drawing.Size(100, 23);
            this.name_textBox.TabIndex = 2;
            this.name_textBox.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Name(用户):";
            // 
            // port_textBox
            // 
            this.port_textBox.Location = new System.Drawing.Point(261, 30);
            this.port_textBox.Name = "port_textBox";
            this.port_textBox.Size = new System.Drawing.Size(100, 23);
            this.port_textBox.TabIndex = 1;
            this.port_textBox.Text = "37777";
            this.port_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.port_textBox_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port(端口):";
            // 
            // ip_textBox
            // 
            this.ip_textBox.Location = new System.Drawing.Point(85, 30);
            this.ip_textBox.Name = "ip_textBox";
            this.ip_textBox.Size = new System.Drawing.Size(100, 23);
            this.ip_textBox.TabIndex = 0;
            this.ip_textBox.Text = "172.23.1.112";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备IP):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.remote_radioButton);
            this.groupBox2.Controls.Add(this.local_radioButton);
            this.groupBox2.Controls.Add(this.talk_button);
            this.groupBox2.Controls.Add(this.channel_comboBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(12, 165);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(382, 138);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Talk(对讲)";
            // 
            // remote_radioButton
            // 
            this.remote_radioButton.AutoSize = true;
            this.remote_radioButton.Location = new System.Drawing.Point(68, 46);
            this.remote_radioButton.Name = "remote_radioButton";
            this.remote_radioButton.Size = new System.Drawing.Size(227, 16);
            this.remote_radioButton.TabIndex = 6;
            this.remote_radioButton.TabStop = true;
            this.remote_radioButton.Text = "Device transfer Mode(设备转发模式)";
            this.remote_radioButton.UseVisualStyleBackColor = true;
            this.remote_radioButton.CheckedChanged += new System.EventHandler(this.remote_radioButton_CheckedChanged);
            // 
            // local_radioButton
            // 
            this.local_radioButton.AutoSize = true;
            this.local_radioButton.Location = new System.Drawing.Point(68, 20);
            this.local_radioButton.Name = "local_radioButton";
            this.local_radioButton.Size = new System.Drawing.Size(263, 16);
            this.local_radioButton.TabIndex = 5;
            this.local_radioButton.TabStop = true;
            this.local_radioButton.Text = "Direct connect device Mode(直连设备模式)";
            this.local_radioButton.UseVisualStyleBackColor = true;
            // 
            // talk_button
            // 
            this.talk_button.Location = new System.Drawing.Point(115, 104);
            this.talk_button.Name = "talk_button";
            this.talk_button.Size = new System.Drawing.Size(141, 23);
            this.talk_button.TabIndex = 8;
            this.talk_button.Text = "StartTalk(开始对讲)";
            this.talk_button.UseVisualStyleBackColor = true;
            this.talk_button.Click += new System.EventHandler(this.talk_button_Click);
            // 
            // channel_comboBox
            // 
            this.channel_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channel_comboBox.FormattingEnabled = true;
            this.channel_comboBox.Location = new System.Drawing.Point(163, 73);
            this.channel_comboBox.Name = "channel_comboBox";
            this.channel_comboBox.Size = new System.Drawing.Size(144, 20);
            this.channel_comboBox.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(66, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "Channel(通道):";
            // 
            // TalkDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(406, 315);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft New Tai Lue", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximizeBox = false;
            this.Name = "TalkDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TalkDemo(对讲Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button login_button;
        private System.Windows.Forms.TextBox password_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox name_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox port_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ip_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button talk_button;
        private System.Windows.Forms.ComboBox channel_comboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton remote_radioButton;
        private System.Windows.Forms.RadioButton local_radioButton;
    }
}

