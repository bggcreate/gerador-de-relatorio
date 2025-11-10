namespace WinForm_IPC_FaceRecognition_Demo
{
    partial class OperateDBWindow
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
            this.listView_groups = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DB = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PersonCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_Add = new System.Windows.Forms.Button();
            this.button_Del = new System.Windows.Forms.Button();
            this.button_OperatePerson = new System.Windows.Forms.Button();
            this.button_Edit = new System.Windows.Forms.Button();
            this.button_Dispatch = new System.Windows.Forms.Button();
            this.button_search = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView_groups
            // 
            this.listView_groups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.DB,
            this.PersonCount});
            this.listView_groups.FullRowSelect = true;
            this.listView_groups.GridLines = true;
            this.listView_groups.Location = new System.Drawing.Point(12, 21);
            this.listView_groups.MultiSelect = false;
            this.listView_groups.Name = "listView_groups";
            this.listView_groups.Size = new System.Drawing.Size(488, 275);
            this.listView_groups.TabIndex = 0;
            this.listView_groups.UseCompatibleStateImageBehavior = false;
            this.listView_groups.View = System.Windows.Forms.View.Details;
            this.listView_groups.SelectedIndexChanged += new System.EventHandler(this.listView_groups_SelectedIndexChanged);
            // 
            // ID
            // 
            this.ID.Text = "DBID(人脸库序号)";
            this.ID.Width = 110;
            // 
            // DB
            // 
            this.DB.Text = "DBName(人脸库名称)";
            this.DB.Width = 230;
            // 
            // PersonCount
            // 
            this.PersonCount.Text = "PersonCount(人员个数)";
            this.PersonCount.Width = 170;
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(515, 59);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(153, 23);
            this.button_Add.TabIndex = 1;
            this.button_Add.Text = "Add(增加)";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_Del
            // 
            this.button_Del.Location = new System.Drawing.Point(515, 136);
            this.button_Del.Name = "button_Del";
            this.button_Del.Size = new System.Drawing.Size(153, 23);
            this.button_Del.TabIndex = 2;
            this.button_Del.Text = "Delete(删除)";
            this.button_Del.UseVisualStyleBackColor = true;
            this.button_Del.Click += new System.EventHandler(this.button_Del_Click);
            // 
            // button_OperatePerson
            // 
            this.button_OperatePerson.Location = new System.Drawing.Point(515, 273);
            this.button_OperatePerson.Name = "button_OperatePerson";
            this.button_OperatePerson.Size = new System.Drawing.Size(153, 23);
            this.button_OperatePerson.TabIndex = 3;
            this.button_OperatePerson.Text = "OperatePerson(操作人员)";
            this.button_OperatePerson.UseVisualStyleBackColor = true;
            this.button_OperatePerson.Click += new System.EventHandler(this.button_OperatePerson_Click);
            // 
            // button_Edit
            // 
            this.button_Edit.Location = new System.Drawing.Point(515, 97);
            this.button_Edit.Name = "button_Edit";
            this.button_Edit.Size = new System.Drawing.Size(153, 23);
            this.button_Edit.TabIndex = 6;
            this.button_Edit.Text = "Modify(修改)";
            this.button_Edit.UseVisualStyleBackColor = true;
            this.button_Edit.Click += new System.EventHandler(this.button_Edit_Click);
            // 
            // button_Dispatch
            // 
            this.button_Dispatch.Location = new System.Drawing.Point(515, 178);
            this.button_Dispatch.Name = "button_Dispatch";
            this.button_Dispatch.Size = new System.Drawing.Size(153, 23);
            this.button_Dispatch.TabIndex = 8;
            this.button_Dispatch.Text = "Dispatch(布控)";
            this.button_Dispatch.UseVisualStyleBackColor = true;
            this.button_Dispatch.Click += new System.EventHandler(this.button_Dispatch_Click);
            // 
            // button_search
            // 
            this.button_search.Location = new System.Drawing.Point(515, 21);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(153, 23);
            this.button_search.TabIndex = 9;
            this.button_search.Text = "Search(查找)";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // OperateDBWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(680, 309);
            this.Controls.Add(this.button_search);
            this.Controls.Add(this.button_Dispatch);
            this.Controls.Add(this.button_Edit);
            this.Controls.Add(this.button_OperatePerson);
            this.Controls.Add(this.button_Del);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.listView_groups);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OperateDBWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OperateDB(人脸库操作)";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView_groups;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button_Del;
        private System.Windows.Forms.Button button_OperatePerson;
        private System.Windows.Forms.ColumnHeader ID;
        private System.Windows.Forms.ColumnHeader DB;
        private System.Windows.Forms.ColumnHeader PersonCount;
        private System.Windows.Forms.Button button_Edit;
        private System.Windows.Forms.Button button_Dispatch;
        private System.Windows.Forms.Button button_search;
    }
}