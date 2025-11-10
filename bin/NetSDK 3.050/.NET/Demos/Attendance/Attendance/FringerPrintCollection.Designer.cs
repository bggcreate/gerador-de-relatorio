namespace Attendance
{
    partial class FringerPrintCollection
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
            this.button_startcollection = new System.Windows.Forms.Button();
            this.label_result = new System.Windows.Forms.Label();
            this.button_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_startcollection
            // 
            this.button_startcollection.Location = new System.Drawing.Point(57, 35);
            this.button_startcollection.Name = "button_startcollection";
            this.button_startcollection.Size = new System.Drawing.Size(172, 23);
            this.button_startcollection.TabIndex = 0;
            this.button_startcollection.Text = "Start Collection(开始采集)";
            this.button_startcollection.UseVisualStyleBackColor = true;
            this.button_startcollection.Click += new System.EventHandler(this.button_startcollection_Click);
            // 
            // label_result
            // 
            this.label_result.AutoSize = true;
            this.label_result.Location = new System.Drawing.Point(55, 98);
            this.label_result.Name = "label_result";
            this.label_result.Size = new System.Drawing.Size(185, 12);
            this.label_result.TabIndex = 1;
            this.label_result.Text = "Collection Completed(采集完成)";
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(86, 145);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(92, 23);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "OK(确定)";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // FringerPrintCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 209);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.label_result);
            this.Controls.Add(this.button_startcollection);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FringerPrintCollection";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FringerPrintCollection(采集指纹)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_startcollection;
        private System.Windows.Forms.Label label_result;
        private System.Windows.Forms.Button button_ok;
    }
}