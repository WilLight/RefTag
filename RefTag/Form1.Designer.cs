namespace RefTag
{

    partial class Form1
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.comboBox_view = new System.Windows.Forms.ComboBox();
            this.button_load_configuration = new System.Windows.Forms.Button();
            this.button_save_configuration = new System.Windows.Forms.Button();
            this.button_choose_folder = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBox_Tags = new System.Windows.Forms.ListBox();
            this.button_New_Tag = new System.Windows.Forms.Button();
            this.listBox_Folders = new System.Windows.Forms.ListBox();
            this.button_Force_Folder_Changes = new System.Windows.Forms.Button();
            this.listView_folder = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.comboBox_view);
            this.splitContainer1.Panel1.Controls.Add(this.button_load_configuration);
            this.splitContainer1.Panel1.Controls.Add(this.button_save_configuration);
            this.splitContainer1.Panel1.Controls.Add(this.button_choose_folder);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 33;
            this.splitContainer1.TabIndex = 0;
            // 
            // comboBox_view
            // 
            this.comboBox_view.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboBox_view.FormattingEnabled = true;
            this.comboBox_view.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.comboBox_view.ItemHeight = 13;
            this.comboBox_view.Items.AddRange(new object[] {
            "Details",
            "Small Icons",
            "Large Icons"});
            this.comboBox_view.Location = new System.Drawing.Point(328, 0);
            this.comboBox_view.MaximumSize = new System.Drawing.Size(130, 0);
            this.comboBox_view.MinimumSize = new System.Drawing.Size(10, 0);
            this.comboBox_view.Name = "comboBox_view";
            this.comboBox_view.Size = new System.Drawing.Size(106, 21);
            this.comboBox_view.TabIndex = 5;
            // 
            // button_load_configuration
            // 
            this.button_load_configuration.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_load_configuration.Location = new System.Drawing.Point(217, 0);
            this.button_load_configuration.Name = "button_load_configuration";
            this.button_load_configuration.Size = new System.Drawing.Size(111, 33);
            this.button_load_configuration.TabIndex = 2;
            this.button_load_configuration.Text = "Load Configuration";
            this.button_load_configuration.UseVisualStyleBackColor = true;
            // 
            // button_save_configuration
            // 
            this.button_save_configuration.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_save_configuration.Location = new System.Drawing.Point(106, 0);
            this.button_save_configuration.Name = "button_save_configuration";
            this.button_save_configuration.Size = new System.Drawing.Size(111, 33);
            this.button_save_configuration.TabIndex = 1;
            this.button_save_configuration.Text = "Save Configuration";
            this.button_save_configuration.UseVisualStyleBackColor = true;
            // 
            // button_choose_folder
            // 
            this.button_choose_folder.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_choose_folder.Location = new System.Drawing.Point(0, 0);
            this.button_choose_folder.Name = "button_choose_folder";
            this.button_choose_folder.Size = new System.Drawing.Size(106, 33);
            this.button_choose_folder.TabIndex = 0;
            this.button_choose_folder.Text = "Choose Folder";
            this.button_choose_folder.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listBox_Tags);
            this.splitContainer2.Panel1.Controls.Add(this.button_New_Tag);
            this.splitContainer2.Panel1.Controls.Add(this.listBox_Folders);
            this.splitContainer2.Panel1.Controls.Add(this.button_Force_Folder_Changes);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listView_folder);
            this.splitContainer2.Size = new System.Drawing.Size(800, 413);
            this.splitContainer2.SplitterDistance = 266;
            this.splitContainer2.TabIndex = 1;
            // 
            // listBox_Tags
            // 
            this.listBox_Tags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Tags.FormattingEnabled = true;
            this.listBox_Tags.Location = new System.Drawing.Point(0, 141);
            this.listBox_Tags.Name = "listBox_Tags";
            this.listBox_Tags.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_Tags.Size = new System.Drawing.Size(266, 272);
            this.listBox_Tags.TabIndex = 8;
            // 
            // button_New_Tag
            // 
            this.button_New_Tag.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_New_Tag.Location = new System.Drawing.Point(0, 118);
            this.button_New_Tag.Name = "button_New_Tag";
            this.button_New_Tag.Size = new System.Drawing.Size(266, 23);
            this.button_New_Tag.TabIndex = 7;
            this.button_New_Tag.Text = "New Tag";
            this.button_New_Tag.UseVisualStyleBackColor = true;
            // 
            // listBox_Folders
            // 
            this.listBox_Folders.Dock = System.Windows.Forms.DockStyle.Top;
            this.listBox_Folders.FormattingEnabled = true;
            this.listBox_Folders.Location = new System.Drawing.Point(0, 23);
            this.listBox_Folders.Name = "listBox_Folders";
            this.listBox_Folders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_Folders.Size = new System.Drawing.Size(266, 95);
            this.listBox_Folders.TabIndex = 6;
            // 
            // button_Force_Folder_Changes
            // 
            this.button_Force_Folder_Changes.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_Force_Folder_Changes.Location = new System.Drawing.Point(0, 0);
            this.button_Force_Folder_Changes.Name = "button_Force_Folder_Changes";
            this.button_Force_Folder_Changes.Size = new System.Drawing.Size(266, 23);
            this.button_Force_Folder_Changes.TabIndex = 5;
            this.button_Force_Folder_Changes.Text = "Force Folder Changes";
            this.button_Force_Folder_Changes.UseVisualStyleBackColor = true;
            // 
            // listView_folder
            // 
            this.listView_folder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_folder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_folder.HideSelection = false;
            this.listView_folder.Location = new System.Drawing.Point(0, 0);
            this.listView_folder.Name = "listView_folder";
            this.listView_folder.Size = new System.Drawing.Size(530, 413);
            this.listView_folder.TabIndex = 0;
            this.listView_folder.UseCompatibleStateImageBehavior = false;
            this.listView_folder.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Modified";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "txt";
            this.saveFileDialog1.Filter = "text files|*.txt|All files|*.*";
            this.saveFileDialog1.FilterIndex = 2;
            this.saveFileDialog1.RestoreDirectory = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "txt";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "text files|*.txt|All files|*.*";
            this.openFileDialog1.FilterIndex = 2;
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "RefTag";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button_save_configuration;
        private System.Windows.Forms.Button button_choose_folder;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView listView_folder;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button_load_configuration;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox comboBox_view;
        private System.Windows.Forms.ListBox listBox_Tags;
        private System.Windows.Forms.Button button_New_Tag;
        private System.Windows.Forms.ListBox listBox_Folders;
        private System.Windows.Forms.Button button_Force_Folder_Changes;
    }
}

