﻿namespace RefTag
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.comboBox_view = new System.Windows.Forms.ComboBox();
            this.button_save_configuration = new System.Windows.Forms.Button();
            this.button_choose_folder = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBox_tags = new System.Windows.Forms.ListBox();
            this.listView_folder = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList_images = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.comboBox_view);
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
            this.comboBox_view.Location = new System.Drawing.Point(217, 0);
            this.comboBox_view.MaximumSize = new System.Drawing.Size(130, 0);
            this.comboBox_view.MinimumSize = new System.Drawing.Size(10, 0);
            this.comboBox_view.Name = "comboBox_view";
            this.comboBox_view.Size = new System.Drawing.Size(100, 21);
            this.comboBox_view.TabIndex = 2;
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
            this.splitContainer2.Panel1.Controls.Add(this.listBox_tags);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listView_folder);
            this.splitContainer2.Size = new System.Drawing.Size(800, 413);
            this.splitContainer2.SplitterDistance = 266;
            this.splitContainer2.TabIndex = 1;
            // 
            // listBox_tags
            // 
            this.listBox_tags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_tags.FormattingEnabled = true;
            this.listBox_tags.Location = new System.Drawing.Point(0, 0);
            this.listBox_tags.MaximumSize = new System.Drawing.Size(260, 0);
            this.listBox_tags.MinimumSize = new System.Drawing.Size(100, 0);
            this.listBox_tags.Name = "listBox_tags";
            this.listBox_tags.Size = new System.Drawing.Size(260, 413);
            this.listBox_tags.TabIndex = 0;
            // 
            // listView_folder
            // 
            this.listView_folder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_folder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_folder.HideSelection = false;
            this.listView_folder.LargeImageList = this.imageList_images;
            this.listView_folder.Location = new System.Drawing.Point(0, 0);
            this.listView_folder.Name = "listView_folder";
            this.listView_folder.Size = new System.Drawing.Size(530, 413);
            this.listView_folder.SmallImageList = this.imageList_images;
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
            // imageList_images
            // 
            this.imageList_images.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList_images.ImageSize = new System.Drawing.Size(140, 140);
            this.imageList_images.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
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
        private System.Windows.Forms.ImageList imageList_images;
        private System.Windows.Forms.Button button_save_configuration;
        private System.Windows.Forms.Button button_choose_folder;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView listView_folder;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ComboBox comboBox_view;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ListBox listBox_tags;
    }
}

