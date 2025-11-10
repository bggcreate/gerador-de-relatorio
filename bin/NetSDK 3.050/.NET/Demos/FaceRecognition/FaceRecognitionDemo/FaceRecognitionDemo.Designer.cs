namespace WinForm_IPC_FaceRecognition_Demo
{
    partial class FaceRecognitionDemo
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
            this.button_login = new System.Windows.Forms.Button();
            this.textBox_pwd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_realplay = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox_channel = new System.Windows.Forms.ComboBox();
            this.button_operateDB = new System.Windows.Forms.Button();
            this.button_attach = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox_realplay = new System.Windows.Forms.PictureBox();
            this.groupBox_globalimage = new System.Windows.Forms.GroupBox();
            this.pictureBox_image = new System.Windows.Forms.PictureBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label_birthday = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label_name = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label_similarity = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label_groupname = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label_groupid = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label_id = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label_candidate_sex = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.pictureBox_candidateimage = new System.Windows.Forms.PictureBox();
            this.pictureBox_faceimage = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label_face_sex = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label_time = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label_age = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label_race = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label_eye = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label_mouth = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label_mask = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label_beard = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_realplay)).BeginInit();
            this.groupBox_globalimage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_candidateimage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_faceimage)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.button_login);
            this.groupBox1.Controls.Add(this.textBox_pwd);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_port);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_ip);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(740, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Login(设备登录)";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(630, 16);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(97, 23);
            this.button_login.TabIndex = 8;
            this.button_login.Text = "Login(登录)";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_pwd
            // 
            this.textBox_pwd.Location = new System.Drawing.Point(510, 18);
            this.textBox_pwd.Name = "textBox_pwd";
            this.textBox_pwd.Size = new System.Drawing.Size(111, 21);
            this.textBox_pwd.TabIndex = 7;
            this.textBox_pwd.Text = "admin123";
            this.textBox_pwd.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(445, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Pwd(密码):";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(375, 18);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(64, 21);
            this.textBox_name.TabIndex = 5;
            this.textBox_name.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(295, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Name(用户名):";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(247, 18);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(42, 21);
            this.textBox_port.TabIndex = 3;
            this.textBox_port.Text = "37777";
            this.textBox_port.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_port_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(179, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port(端口):";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(74, 18);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(100, 21);
            this.textBox_ip.TabIndex = 1;
            this.textBox_ip.Text = "192.168.7.61";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP(设备IP):";
            // 
            // button_realplay
            // 
            this.button_realplay.Location = new System.Drawing.Point(187, 14);
            this.button_realplay.Name = "button_realplay";
            this.button_realplay.Size = new System.Drawing.Size(145, 23);
            this.button_realplay.TabIndex = 15;
            this.button_realplay.Text = "RealPlay(监视)";
            this.button_realplay.UseVisualStyleBackColor = true;
            this.button_realplay.Click += new System.EventHandler(this.button_realplay_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "Channel(通道):";
            // 
            // comboBox_channel
            // 
            this.comboBox_channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_channel.FormattingEnabled = true;
            this.comboBox_channel.Location = new System.Drawing.Point(101, 16);
            this.comboBox_channel.Name = "comboBox_channel";
            this.comboBox_channel.Size = new System.Drawing.Size(60, 20);
            this.comboBox_channel.TabIndex = 13;
            // 
            // button_operateDB
            // 
            this.button_operateDB.Location = new System.Drawing.Point(530, 14);
            this.button_operateDB.Name = "button_operateDB";
            this.button_operateDB.Size = new System.Drawing.Size(197, 23);
            this.button_operateDB.TabIndex = 12;
            this.button_operateDB.Text = "OperateDB(操作人脸库)";
            this.button_operateDB.UseVisualStyleBackColor = true;
            this.button_operateDB.Click += new System.EventHandler(this.button_operate_Click);
            // 
            // button_attach
            // 
            this.button_attach.Location = new System.Drawing.Point(362, 14);
            this.button_attach.Name = "button_attach";
            this.button_attach.Size = new System.Drawing.Size(140, 23);
            this.button_attach.TabIndex = 10;
            this.button_attach.Text = "Attach(订阅)";
            this.button_attach.UseVisualStyleBackColor = true;
            this.button_attach.Click += new System.EventHandler(this.button_attach_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox_realplay);
            this.groupBox2.Location = new System.Drawing.Point(6, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(355, 251);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview(预览)";
            // 
            // pictureBox_realplay
            // 
            this.pictureBox_realplay.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_realplay.Location = new System.Drawing.Point(6, 14);
            this.pictureBox_realplay.Name = "pictureBox_realplay";
            this.pictureBox_realplay.Size = new System.Drawing.Size(343, 231);
            this.pictureBox_realplay.TabIndex = 0;
            this.pictureBox_realplay.TabStop = false;
            // 
            // groupBox_globalimage
            // 
            this.groupBox_globalimage.Controls.Add(this.pictureBox_image);
            this.groupBox_globalimage.Location = new System.Drawing.Point(368, 96);
            this.groupBox_globalimage.Name = "groupBox_globalimage";
            this.groupBox_globalimage.Size = new System.Drawing.Size(378, 251);
            this.groupBox_globalimage.TabIndex = 2;
            this.groupBox_globalimage.TabStop = false;
            this.groupBox_globalimage.Text = "GlobalScene_Image(全景图)";
            // 
            // pictureBox_image
            // 
            this.pictureBox_image.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_image.Location = new System.Drawing.Point(6, 14);
            this.pictureBox_image.Name = "pictureBox_image";
            this.pictureBox_image.Size = new System.Drawing.Size(363, 231);
            this.pictureBox_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_image.TabIndex = 0;
            this.pictureBox_image.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label_birthday);
            this.groupBox5.Controls.Add(this.label24);
            this.groupBox5.Controls.Add(this.label_name);
            this.groupBox5.Controls.Add(this.label26);
            this.groupBox5.Controls.Add(this.label_similarity);
            this.groupBox5.Controls.Add(this.label32);
            this.groupBox5.Controls.Add(this.label_groupname);
            this.groupBox5.Controls.Add(this.label34);
            this.groupBox5.Controls.Add(this.label_groupid);
            this.groupBox5.Controls.Add(this.label36);
            this.groupBox5.Controls.Add(this.label_id);
            this.groupBox5.Controls.Add(this.label38);
            this.groupBox5.Controls.Add(this.label_candidate_sex);
            this.groupBox5.Controls.Add(this.label40);
            this.groupBox5.Controls.Add(this.pictureBox_candidateimage);
            this.groupBox5.Location = new System.Drawing.Point(367, 353);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(378, 162);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Candidate_Image(候选人图)";
            // 
            // label_birthday
            // 
            this.label_birthday.AutoSize = true;
            this.label_birthday.Location = new System.Drawing.Point(251, 60);
            this.label_birthday.Name = "label_birthday";
            this.label_birthday.Size = new System.Drawing.Size(53, 12);
            this.label_birthday.TabIndex = 36;
            this.label_birthday.Text = "11111111";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(156, 60);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(95, 12);
            this.label24.TabIndex = 35;
            this.label24.Text = "BirthDay(生日):";
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Location = new System.Drawing.Point(251, 20);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(53, 12);
            this.label_name.TabIndex = 34;
            this.label_name.Text = "11111111";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(180, 20);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(71, 12);
            this.label26.TabIndex = 33;
            this.label26.Text = "Name(姓名):";
            // 
            // label_similarity
            // 
            this.label_similarity.AutoSize = true;
            this.label_similarity.Location = new System.Drawing.Point(251, 140);
            this.label_similarity.Name = "label_similarity";
            this.label_similarity.Size = new System.Drawing.Size(53, 12);
            this.label_similarity.TabIndex = 28;
            this.label_similarity.Text = "11111111";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(132, 140);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(119, 12);
            this.label32.TabIndex = 27;
            this.label32.Text = "Similarity(相似度):";
            // 
            // label_groupname
            // 
            this.label_groupname.AutoSize = true;
            this.label_groupname.Location = new System.Drawing.Point(251, 120);
            this.label_groupname.Name = "label_groupname";
            this.label_groupname.Size = new System.Drawing.Size(53, 12);
            this.label_groupname.TabIndex = 26;
            this.label_groupname.Text = "11111111";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(144, 120);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(107, 12);
            this.label34.TabIndex = 25;
            this.label34.Text = "DBName(人脸库名):";
            // 
            // label_groupid
            // 
            this.label_groupid.AutoSize = true;
            this.label_groupid.Location = new System.Drawing.Point(251, 100);
            this.label_groupid.Name = "label_groupid";
            this.label_groupid.Size = new System.Drawing.Size(53, 12);
            this.label_groupid.TabIndex = 24;
            this.label_groupid.Text = "11111111";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(144, 100);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(107, 12);
            this.label36.TabIndex = 23;
            this.label36.Text = "DBID(人脸库序号):";
            // 
            // label_id
            // 
            this.label_id.AutoSize = true;
            this.label_id.Location = new System.Drawing.Point(251, 80);
            this.label_id.Name = "label_id";
            this.label_id.Size = new System.Drawing.Size(53, 12);
            this.label_id.TabIndex = 22;
            this.label_id.Text = "11111111";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(144, 80);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(107, 12);
            this.label38.TabIndex = 21;
            this.label38.Text = "IDNumber(证件号):";
            // 
            // label_candidate_sex
            // 
            this.label_candidate_sex.AutoSize = true;
            this.label_candidate_sex.Location = new System.Drawing.Point(251, 40);
            this.label_candidate_sex.Name = "label_candidate_sex";
            this.label_candidate_sex.Size = new System.Drawing.Size(53, 12);
            this.label_candidate_sex.TabIndex = 20;
            this.label_candidate_sex.Text = "11111111";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(186, 40);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(65, 12);
            this.label40.TabIndex = 19;
            this.label40.Text = "Sex(姓别):";
            // 
            // pictureBox_candidateimage
            // 
            this.pictureBox_candidateimage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_candidateimage.Location = new System.Drawing.Point(9, 20);
            this.pictureBox_candidateimage.Name = "pictureBox_candidateimage";
            this.pictureBox_candidateimage.Size = new System.Drawing.Size(123, 134);
            this.pictureBox_candidateimage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_candidateimage.TabIndex = 1;
            this.pictureBox_candidateimage.TabStop = false;
            // 
            // pictureBox_faceimage
            // 
            this.pictureBox_faceimage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox_faceimage.Location = new System.Drawing.Point(7, 20);
            this.pictureBox_faceimage.Name = "pictureBox_faceimage";
            this.pictureBox_faceimage.Size = new System.Drawing.Size(123, 134);
            this.pictureBox_faceimage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_faceimage.TabIndex = 0;
            this.pictureBox_faceimage.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(148, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Sex(姓别):";
            // 
            // label_face_sex
            // 
            this.label_face_sex.AutoSize = true;
            this.label_face_sex.Location = new System.Drawing.Point(211, 40);
            this.label_face_sex.Name = "label_face_sex";
            this.label_face_sex.Size = new System.Drawing.Size(71, 12);
            this.label_face_sex.TabIndex = 2;
            this.label_face_sex.Text = "11111111111";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(142, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "Time(时间):";
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(211, 23);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(71, 12);
            this.label_time.TabIndex = 4;
            this.label_time.Text = "11111111111";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(148, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "Age(年龄):";
            // 
            // label_age
            // 
            this.label_age.AutoSize = true;
            this.label_age.Location = new System.Drawing.Point(211, 56);
            this.label_age.Name = "label_age";
            this.label_age.Size = new System.Drawing.Size(71, 12);
            this.label_age.TabIndex = 6;
            this.label_age.Text = "11111111111";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(142, 74);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 12);
            this.label14.TabIndex = 9;
            this.label14.Text = "Race(种族):";
            // 
            // label_race
            // 
            this.label_race.AutoSize = true;
            this.label_race.Location = new System.Drawing.Point(211, 74);
            this.label_race.Name = "label_race";
            this.label_race.Size = new System.Drawing.Size(71, 12);
            this.label_race.TabIndex = 10;
            this.label_race.Text = "11111111111";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(148, 92);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 11;
            this.label16.Text = "Eye(眼睛):";
            // 
            // label_eye
            // 
            this.label_eye.AutoSize = true;
            this.label_eye.Location = new System.Drawing.Point(211, 92);
            this.label_eye.Name = "label_eye";
            this.label_eye.Size = new System.Drawing.Size(71, 12);
            this.label_eye.TabIndex = 12;
            this.label_eye.Text = "11111111111";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(136, 109);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(77, 12);
            this.label18.TabIndex = 13;
            this.label18.Text = "Mouth(嘴巴):";
            // 
            // label_mouth
            // 
            this.label_mouth.AutoSize = true;
            this.label_mouth.Location = new System.Drawing.Point(211, 109);
            this.label_mouth.Name = "label_mouth";
            this.label_mouth.Size = new System.Drawing.Size(71, 12);
            this.label_mouth.TabIndex = 14;
            this.label_mouth.Text = "11111111111";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(142, 126);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(71, 12);
            this.label20.TabIndex = 15;
            this.label20.Text = "Mask(口罩):";
            // 
            // label_mask
            // 
            this.label_mask.AutoSize = true;
            this.label_mask.Location = new System.Drawing.Point(211, 126);
            this.label_mask.Name = "label_mask";
            this.label_mask.Size = new System.Drawing.Size(71, 12);
            this.label_mask.TabIndex = 16;
            this.label_mask.Text = "11111111111";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(136, 142);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(77, 12);
            this.label22.TabIndex = 17;
            this.label22.Text = "Beard(胡子):";
            // 
            // label_beard
            // 
            this.label_beard.AutoSize = true;
            this.label_beard.Location = new System.Drawing.Point(211, 142);
            this.label_beard.Name = "label_beard";
            this.label_beard.Size = new System.Drawing.Size(71, 12);
            this.label_beard.TabIndex = 18;
            this.label_beard.Text = "11111111111";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label_beard);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.label_mask);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.label_mouth);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.label_eye);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.label_race);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label_age);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label_time);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label_face_sex);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.pictureBox_faceimage);
            this.groupBox4.Location = new System.Drawing.Point(6, 353);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(355, 162);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Face_Image(人脸图)";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button_realplay);
            this.groupBox6.Controls.Add(this.button_operateDB);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.button_attach);
            this.groupBox6.Controls.Add(this.comboBox_channel);
            this.groupBox6.Location = new System.Drawing.Point(6, 52);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(740, 42);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Operate(操作)";
            // 
            // FaceRecognitionDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(751, 523);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox_globalimage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "FaceRecognitionDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FaceRecognitionDemo(人脸识别Demo)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_realplay)).EndInit();
            this.groupBox_globalimage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_candidateimage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_faceimage)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_operateDB;
        private System.Windows.Forms.Button button_attach;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.TextBox textBox_pwd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox_globalimage;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.PictureBox pictureBox_realplay;
        private System.Windows.Forms.PictureBox pictureBox_image;
        private System.Windows.Forms.PictureBox pictureBox_candidateimage;
        private System.Windows.Forms.Label label_birthday;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label_similarity;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label_groupname;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label_groupid;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label_id;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label_candidate_sex;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.PictureBox pictureBox_faceimage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_face_sex;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label_age;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label_race;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label_eye;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label_mouth;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label_mask;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label_beard;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_realplay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox_channel;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}

