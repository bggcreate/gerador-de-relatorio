namespace WinForm_IPC_FaceRecognition_Demo
{
    partial class AddDBWindow
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
            this.textBox_groupname = new System.Windows.Forms.TextBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "DBName(人脸库名称):";
            // 
            // textBox_groupname
            // 
            this.textBox_groupname.Location = new System.Drawing.Point(144, 31);
            this.textBox_groupname.Name = "textBox_groupname";
            this.textBox_groupname.Size = new System.Drawing.Size(133, 21);
            this.textBox_groupname.TabIndex = 1;
            this.textBox_groupname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_groupname_KeyPress);
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(283, 31);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(109, 23);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "Ok(确认)";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // AddDBWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(404, 82);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.textBox_groupname);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddDBWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddDB(增加人脸库)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_groupname;
        private System.Windows.Forms.Button button_ok;
    }
}