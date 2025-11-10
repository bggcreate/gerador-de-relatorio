namespace VTODemo
{
    partial class OperateManager
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
            this.button_search = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_cardno = new System.Windows.Forms.TextBox();
            this.button_add = new System.Windows.Forms.Button();
            this.button_modify = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_clear = new System.Windows.Forms.Button();
            this.dataGridView_card = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FingerprintID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FingerprintData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_card)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView_card);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(448, 336);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CardList(卡记录集)";
            // 
            // button_search
            // 
            this.button_search.Location = new System.Drawing.Point(477, 84);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(140, 23);
            this.button_search.TabIndex = 1;
            this.button_search.Text = "Search(查询)";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(475, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "CardNo.(卡号):";
            // 
            // textBox_cardno
            // 
            this.textBox_cardno.Location = new System.Drawing.Point(477, 46);
            this.textBox_cardno.Name = "textBox_cardno";
            this.textBox_cardno.Size = new System.Drawing.Size(140, 21);
            this.textBox_cardno.TabIndex = 3;
            this.textBox_cardno.TextChanged += new System.EventHandler(this.textBox_cardno_TextChanged);
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(477, 138);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(140, 23);
            this.button_add.TabIndex = 4;
            this.button_add.Text = "Add(增加)";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // button_modify
            // 
            this.button_modify.Location = new System.Drawing.Point(477, 195);
            this.button_modify.Name = "button_modify";
            this.button_modify.Size = new System.Drawing.Size(140, 23);
            this.button_modify.TabIndex = 5;
            this.button_modify.Text = "Modify(修改)";
            this.button_modify.UseVisualStyleBackColor = true;
            this.button_modify.Click += new System.EventHandler(this.button_modify_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(477, 255);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(140, 23);
            this.button_delete.TabIndex = 6;
            this.button_delete.Text = "Delete(删除)";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_clear
            // 
            this.button_clear.Location = new System.Drawing.Point(477, 314);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(140, 23);
            this.button_clear.TabIndex = 7;
            this.button_clear.Text = "Clear(清空)";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // dataGridView_card
            // 
            this.dataGridView_card.AllowUserToAddRows = false;
            this.dataGridView_card.AllowUserToResizeRows = false;
            this.dataGridView_card.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView_card.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView_card.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView_card.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView_card.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.UserID,
            this.FingerprintID,
            this.FingerprintData});
            this.dataGridView_card.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_card.Location = new System.Drawing.Point(6, 20);
            this.dataGridView_card.MultiSelect = false;
            this.dataGridView_card.Name = "dataGridView_card";
            this.dataGridView_card.ReadOnly = true;
            this.dataGridView_card.RowHeadersVisible = false;
            this.dataGridView_card.RowTemplate.Height = 23;
            this.dataGridView_card.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_card.Size = new System.Drawing.Size(436, 310);
            this.dataGridView_card.TabIndex = 2;
            // 
            // ID
            // 
            this.ID.HeaderText = "RecNo.(编号)";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Width = 102;
            // 
            // UserID
            // 
            this.UserID.HeaderText = "RoomNo.(房间号)";
            this.UserID.Name = "UserID";
            this.UserID.ReadOnly = true;
            this.UserID.Width = 120;
            // 
            // FingerprintID
            // 
            this.FingerprintID.HeaderText = "CardNo.(卡号)";
            this.FingerprintID.Name = "FingerprintID";
            this.FingerprintID.ReadOnly = true;
            this.FingerprintID.Width = 108;
            // 
            // FingerprintData
            // 
            this.FingerprintData.HeaderText = "FingerprintData(指纹数据)";
            this.FingerprintData.Name = "FingerprintData";
            this.FingerprintData.ReadOnly = true;
            this.FingerprintData.Width = 180;
            // 
            // OperateManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(636, 360);
            this.Controls.Add(this.button_clear);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.button_modify);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.textBox_cardno);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_search);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OperateManager";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Card&Fingerprint&FaceManager(卡与指纹与人脸管理)";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_card)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_cardno;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.Button button_modify;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.DataGridView dataGridView_card;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserID;
        private System.Windows.Forms.DataGridViewTextBoxColumn FingerprintID;
        private System.Windows.Forms.DataGridViewTextBoxColumn FingerprintData;
    }
}