using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RefTag
{
    public partial class Form1 : Form
    {
        //TODO: Implement tags consisting of text fields with right-click dropdown menu and a checkbox
        //TODO: implement right-click assignment of pictures to tags
        //Declaring Directory info for future use
        private DirectoryInfo _dirInfo;
        //Event Handler for button_choose_folder
        private void Button_choose_folder_Click(object sender, System.EventArgs e)
        {
            //If Folder Browser Dialog ended with a result
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                //Clearing listView and imageList before populating
                listView_folder.Clear();
                imageList_images.Images.Clear();
                //if listView view controller has no option selected, automatically select Large Images
                if (this.comboBox_view.SelectedIndex == -1)
                {
                    this.comboBox_view.SelectedIndex = 2;
                }
                //Assigning chosen directory to Directory info
                _dirInfo = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                //Iterator for assignment of images to files.
                int imageCounter = 0;
                //Iterating through files in the chosen directory
                foreach (FileInfo file in _dirInfo.GetFiles())
                {
                    //TODO: Find a way to load thousand of pictures without using 1GB RAM
                    //Trying to add images from files to the imageList
                    try
                    {
                        Image imageBeforeResizing = Image.FromStream(file.OpenRead());
                        //TODO: Implement maintaining aspect ratio to resized pictures
                        this.imageList_images.Images.Add(imageBeforeResizing);
                        Console.WriteLine("File: "+ file.FullName.ToString());
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("File is not an image: " + exception);
                    }
                    //Assigning listView items a corresponding image from imageList
                    ListViewItem item = new ListViewItem(file.Name, imageCounter);
                    //Assigning additional information into corresponding subitems of listView. Currently only visible in View Details
                    ListViewItem.ListViewSubItem[] subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
                        new ListViewItem.ListViewSubItem(item,
                            file.LastAccessTime.ToShortDateString())};

                    item.SubItems.AddRange(subItems);
                    this.listView_folder.Items.Add(item);
                    imageCounter += 1;
                }
            }
        }

        private void Combo_box_view_change(object sender, System.EventArgs e)
        {
            switch (this.comboBox_view.SelectedIndex)
            {
                case 0:
                    listView_folder.View = View.Details;
                    break;
                case 1:
                    listView_folder.View = View.SmallIcon;
                    break;
                case 2:
                    listView_folder.View = View.LargeIcon;
                    break;
            }
            
        }

        public Form1()
        {
            InitializeComponent();
            button_choose_folder.Click += new EventHandler(Button_choose_folder_Click);
            comboBox_view.SelectedIndexChanged += new EventHandler(Combo_box_view_change);
        }
    }
}
