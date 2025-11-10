namespace Attendance
{
    partial class OperateFingerprintByFPID
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
            this.textBox_id = new System.Windows.Forms.TextBox();
            this.button_get = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_fingerprintdata = new System.Windows.Forms.Label();
            this.label_userid = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "FingerPrintID(指纹ID):";
            // 
            // textBox_id
            // 
            this.textBox_id.Location = new System.Drawing.Point(179, 28);
            this.textBox_id.Name = "textBox_id";
            this.textBox_id.Size = new System.Drawing.Size(117, 21);
            this.textBox_id.TabIndex = 1;
            this.textBox_id.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_id_KeyPress);
            // 
            // button_get
            // 
            this.button_get.Location = new System.Drawing.Point(335, 26);
            this.button_get.Name = "button_get";
            this.button_get.Size = new System.Drawing.Size(95, 23);
            this.button_get.TabIndex = 2;
            this.button_get.Text = "Get(获取)";
            this.button_get.UseVisualStyleBackColor = true;
            this.button_get.Click += new System.EventHandler(this.button_get_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(442, 28);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(95, 23);
            this.button_delete.TabIndex = 3;
            this.button_delete.Text = "Delete(删除)";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "UserID(用户ID):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "FingerprintData(指纹数据):";
            // 
            // label_fingerprintdata
            // 
            this.label_fingerprintdata.Location = new System.Drawing.Point(38, 132);
            this.label_fingerprintdata.Name = "label_fingerprintdata";
            this.label_fingerprintdata.Size = new System.Drawing.Size(497, 248);
            this.label_fingerprintdata.TabIndex = 6;
            this.label_fingerprintdata.Text = "label4";
            // 
            // label_userid
            // 
            this.label_userid.AutoSize = true;
            this.label_userid.Location = new System.Drawing.Point(138, 78);
            this.label_userid.Name = "label_userid";
            this.label_userid.Size = new System.Drawing.Size(41, 12);
            this.label_userid.TabIndex = 7;
            this.label_userid.Text = "label5";
            // 
            // OperateFingerprintByFPID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(547, 393);
            this.Controls.Add(this.label_userid);
            this.Controls.Add(this.label_fingerprintdata);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.button_get);
            this.Controls.Add(this.textBox_id);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OperateFingerprintByFPID";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OperateFingerprintByFPID(通过指纹ID操作指纹)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_id;
        private System.Windows.Forms.Button button_get;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_fingerprintdata;
        private System.Windows.Forms.Label label_userid;
    }
}