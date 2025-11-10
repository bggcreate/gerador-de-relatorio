namespace WinForm_IPC_FaceRecognition_Demo
{
    partial class SimilarityOperateWindow
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
            this.comboBox_channel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView_si = new System.Windows.Forms.ListView();
            this.button_delete = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_groupid = new System.Windows.Forms.TextBox();
            this.textBox_groupname = new System.Windows.Forms.TextBox();
            this.comboBox_value = new System.Windows.Forms.ComboBox();
            this.button_add = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_channel
            // 
            this.comboBox_channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_channel.FormattingEnabled = true;
            this.comboBox_channel.Location = new System.Drawing.Point(119, 56);
            this.comboBox_channel.Name = "comboBox_channel";
            this.comboBox_channel.Size = new System.Drawing.Size(117, 20);
            this.comboBox_channel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Channel(通道):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(242, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Similarity(相似度):";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Channel(通道)";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Similarity Value(相似度值)";
            this.columnHeader2.Width = 200;
            // 
            // listView_si
            // 
            this.listView_si.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_si.FullRowSelect = true;
            this.listView_si.GridLines = true;
            this.listView_si.Location = new System.Drawing.Point(14, 83);
            this.listView_si.Name = "listView_si";
            this.listView_si.Size = new System.Drawing.Size(348, 231);
            this.listView_si.TabIndex = 17;
            this.listView_si.UseCompatibleStateImageBehavior = false;
            this.listView_si.View = System.Windows.Forms.View.Details;
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(367, 164);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(120, 23);
            this.button_delete.TabIndex = 20;
            this.button_delete.Text = "Delete(删除)";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "DBID(人脸库序号):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(254, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "DBName(人脸库名):";
            // 
            // textBox_groupid
            // 
            this.textBox_groupid.Enabled = false;
            this.textBox_groupid.Location = new System.Drawing.Point(119, 14);
            this.textBox_groupid.Name = "textBox_groupid";
            this.textBox_groupid.Size = new System.Drawing.Size(117, 21);
            this.textBox_groupid.TabIndex = 15;
            // 
            // textBox_groupname
            // 
            this.textBox_groupname.Enabled = false;
            this.textBox_groupname.Location = new System.Drawing.Point(368, 14);
            this.textBox_groupname.Name = "textBox_groupname";
            this.textBox_groupname.Size = new System.Drawing.Size(120, 21);
            this.textBox_groupname.TabIndex = 16;
            // 
            // comboBox_value
            // 
            this.comboBox_value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_value.FormattingEnabled = true;
            this.comboBox_value.Location = new System.Drawing.Point(367, 56);
            this.comboBox_value.Name = "comboBox_value";
            this.comboBox_value.Size = new System.Drawing.Size(121, 20);
            this.comboBox_value.TabIndex = 21;
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(368, 106);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(120, 23);
            this.button_add.TabIndex = 22;
            this.button_add.Text = "Add(增加)";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // SimilarityOperateWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(502, 329);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.comboBox_value);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.listView_si);
            this.Controls.Add(this.textBox_groupname);
            this.Controls.Add(this.textBox_groupid);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_channel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimilarityOperateWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DispatchOperate(布控操作)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_channel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView listView_si;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_groupid;
        private System.Windows.Forms.TextBox textBox_groupname;
        private System.Windows.Forms.ComboBox comboBox_value;
        private System.Windows.Forms.Button button_add;

    }
}