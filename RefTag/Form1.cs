using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RefTag
{
    public partial class Form1 : Form
    {
        public void ListBoxAddTag(ListBox listBox, Dictionary<string, List<ListViewItem>> listBoxTagDictionary,
            string tagName)
        {
            listBox.Items.Add(tagName);
            listBoxTagDictionary.Add(tagName, new List<ListViewItem>());
        }

        public void ListBoxAddToTag(Dictionary<string, List<ListViewItem>> listBoxTagDictionary, string tagName,
            List<ListViewItem> listViewItems)
        {
            listBoxTagDictionary[tagName].AddRange(listViewItems.Except(listBoxTagDictionary[tagName]));
        }

        private void ListViewInitialView()
        {
            //if listView view controller has no option selected, automatically select Large Images
            if (comboBox_view.SelectedIndex == -1) comboBox_view.SelectedIndex = 2;
        }

        private void ListViewRefresh()
        {
            listView_folder.Items.Clear();
            if (listBox_Tags.SelectedItem != null &&
                listBoxItemListDictionary[listBox_Tags.SelectedItem.ToString()].Count > 0)
            {
                listView_folder.Items.AddRange(
                    listBoxItemListDictionary[listBox_Tags.SelectedItem.ToString()].ToArray());
            }
        }

        private void PopulateDictionaries()
        {
            foreach (var file in _dirInfo.GetFiles())
            {
                _files.Add(file.Name, file.FullName);
                _fileDates.Add(file.Name, file.LastAccessTime.ToString("g"));
            }
        }

        private void PopulateImageList(ImageList imageList)
        {
            var imageListMutex = new Mutex();
            _files.AsParallel().ForAll(file =>
            {
                var imageBeforeResizing = Image.FromFile(file.Value);
                //TODO: Implement maintaining aspect ratio to resized pictures
                var resizedImage = ResizeImage(imageBeforeResizing, 140, 140);

                imageListMutex.WaitOne();
                imageList.Images.Add(file.Key, resizedImage);
                imageListMutex.ReleaseMutex();
            });
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        //Declaring Directory info
        private DirectoryInfo _dirInfo;

        //Declaring Dictionaries
        private Dictionary<string, string> _files = new Dictionary<string, string>();
        private Dictionary<string, string> _fileDates = new Dictionary<string, string>();

        private Dictionary<string, List<ListViewItem>> listBoxItemListDictionary =
            new Dictionary<string, List<ListViewItem>>();

        private List<ListViewItem> listViewItemSelectionList = new List<ListViewItem>();
        ImageList imageList = new ImageList {ImageSize = new Size(140, 140)};

        //Event Handler for button_choose_folder
        private void Button_choose_folder_Click(object sender, EventArgs e)
        {
            //Making sure folder browser ended with a selection
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                //Clearing listView before populating
                listView_folder.Items.Clear();

                //Selecting View
                ListViewInitialView();

                //Assigning chosen directory to Directory info
                _dirInfo = new DirectoryInfo(folderBrowserDialog1.SelectedPath);

                //Clearing dictionaries
                _files.Clear();
                _fileDates.Clear();

                //Populating dictionaries
                PopulateDictionaries();

                //Populating ImageList
                PopulateImageList(imageList);

                //Assigning populated imageList to listView
                listView_folder.LargeImageList = imageList;
                listView_folder.SmallImageList = imageList;

                foreach (var file in _files)
                {
                    //Assigning listView item a name and an imageKey
                    var item = new ListViewItem(file.Key, file.Key);

                    //Assigning additional information into corresponding subItems of listView
                    var subItems = new ListViewItem.ListViewSubItem[]
                    {
                        new ListViewItem.ListViewSubItem(item, "File"),
                        new ListViewItem.ListViewSubItem(item,
                            _fileDates[file.Key])
                    };

                    //Adding subItems to item and adding item to listView
                    item.SubItems.AddRange(subItems);
                    listViewItemSelectionList.Add(item);
                }

                if (listBox_Tags.Items.Contains(_dirInfo.Name))
                {
                    listBoxItemListDictionary[_dirInfo.Name].Clear();
                    ListBoxAddToTag(listBoxItemListDictionary, _dirInfo.Name, listViewItemSelectionList);
                }
                else
                {
                    ListBoxAddTag(listBox_Tags, listBoxItemListDictionary, _dirInfo.Name);
                    ListBoxAddToTag(listBoxItemListDictionary, _dirInfo.Name, listViewItemSelectionList);
                }

                listViewItemSelectionList.Clear();
                listBox_Tags.SetSelected(listBox_Tags.Items.IndexOf(_dirInfo.Name), true);
                ListViewRefresh();
            }
        }

        private void Combo_box_view_change(object sender, EventArgs e)
        {
            switch (comboBox_view.SelectedIndex)
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


        //TODO: Implement right-click functionality to tags and pictures
        private void Button_new_tag_click(object sender, EventArgs e)
        {
            var promptValue = PopupWindows.ShowTextInputDialog("Enter Tag Name", "Add New Tag");
            if (!promptValue.StartsWith(" ") || promptValue != "")
            {
                if (!listBoxItemListDictionary.ContainsKey(promptValue))
                {
                    listBox_Tags.Items.Add(promptValue);
                    listBoxItemListDictionary.Add(promptValue, new List<ListViewItem>());
                }
                else
                {
                    //TODO: Write a warning for adding duplicate
                }
            }
        }

        //TODO: Implement saving configurations


        private void ListView_folder_item_selection(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && !listViewItemSelectionList.Contains(e.Item))
            {
                listViewItemSelectionList.Add(e.Item);
                Debug.WriteLine("Item Selected: " + e.Item.Text);
            }
            else if (!e.IsSelected && listViewItemSelectionList.Contains(e.Item))
            {
                listViewItemSelectionList.Remove(e.Item);
                Debug.WriteLine("Item Deselected: " + e.Item.Text);
            }
        }

        private void ListBox_Tags_Click_Events(object sender, MouseEventArgs e)
        {
            var tagIndexFromPoint = listBox_Tags.IndexFromPoint(e.Location);
            if (e.Button != MouseButtons.Right)
            {
                if (tagIndexFromPoint == ListBox.NoMatches)
                {
                    listView_folder.SelectedItems.Clear();
                    return;
                }

                return;
            }

            if (tagIndexFromPoint != ListBox.NoMatches)
            {
                var selectedItem = listBox_Tags.Items[tagIndexFromPoint].ToString();
                if (listViewItemSelectionList.Count > 0)
                {
                    PopupWindows.ShowContextMenu(listBox_Tags, e.Location, selectedItem, listBoxItemListDictionary,
                        listViewItemSelectionList, listView_folder);
                }
                else
                    PopupWindows.ShowContextMenu(listBox_Tags, e.Location, selectedItem, listBoxItemListDictionary,
                        listView_folder);
            }
        }

        private void ListBox_Tags_Double_Click(object sender, EventArgs e)
        {
            ListViewRefresh();
        }

        public Form1()
        {
            InitializeComponent();
            button_choose_folder.Click += Button_choose_folder_Click;
            comboBox_view.SelectedIndexChanged += Combo_box_view_change;
            button_New_Tag.Click += Button_new_tag_click;
            listView_folder.ItemSelectionChanged += ListView_folder_item_selection;
            listBox_Tags.MouseDown += ListBox_Tags_Click_Events;
            listBox_Tags.DoubleClick += ListBox_Tags_Double_Click;
        }
    }


    public static class PopupWindows
    {
        //
        public static string ShowTextInputDialog(string text, string caption)
        {
            var prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            var textLabel = new Label() {Left = 50, Top = 20, Text = text};
            var textBox = new TextBox() {Left = 50, Top = 50, Width = 400};
            var confirmation = new Button()
                {Text = @"Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK};
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        //
        public static void ShowContextMenu(ListBox listBox, Point locationPoint, string tagName,
            Dictionary<string, List<ListViewItem>> listBoxTagDictionary, List<ListViewItem> listViewItems,
            ListView listView)
        {
            var toolStripMenuItem1 = new ToolStripMenuItem {Text = @"Add to tag"};
            toolStripMenuItem1.Click += ToolStripMenuItem1Click;
            var toolStripMenuItem2 = new ToolStripMenuItem {Text = @"Remove from tag"};
            toolStripMenuItem2.Click += ToolStripMenuItem2Click;
            var toolStripMenuItem3 = new ToolStripMenuItem {Text = @"Delete tag"};
            toolStripMenuItem3.Click += ToolStripMenuItem3Click;
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3});

            void RefreshListView()
            {
                listView.Items.Clear();
                if (listBox.SelectedItem != null && listBoxTagDictionary[listBox.SelectedItem.ToString()].Count > 0)
                {
                    listView.Items.AddRange(listBoxTagDictionary[listBox.SelectedItem.ToString()].ToArray());
                }
            }

            void ToolStripMenuItem1Click(object sender, EventArgs e)
            {
                listBoxTagDictionary[tagName].AddRange(listViewItems.Except(listBoxTagDictionary[tagName]));
                if (listBox.SelectedItem.ToString() == tagName)
                {
                    RefreshListView();
                }
            }

            void ToolStripMenuItem2Click(object sender, EventArgs e)
            {
                ListViewItem[] tempListViewItems = listBoxTagDictionary[tagName].Except(listViewItems).ToArray();
                listBoxTagDictionary[tagName].Clear();
                listBoxTagDictionary[tagName].AddRange(tempListViewItems);
                if (listBox.SelectedItem.ToString() == tagName)
                {
                    RefreshListView();
                }
            }

            void ToolStripMenuItem3Click(object sender, EventArgs e)
            {
                listBox.Items.Remove(tagName);
                listBoxTagDictionary.Remove(tagName);
                if (listBox.SelectedItem.ToString() == tagName)
                {
                    listView.Items.Clear();
                }
            }

            contextMenuStrip.Show(listBox, locationPoint, ToolStripDropDownDirection.Default);
        }


        public static void ShowContextMenu(ListBox listBox, Point locationPoint, string listItem,
            Dictionary<string, List<ListViewItem>> listViewItemListDictionary, ListView listView)
        {
            var toolStripMenuItem3 = new ToolStripMenuItem {Text = @"Delete tag"};
            toolStripMenuItem3.Click += ToolStripMenuItem3Click;
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripItem[] {toolStripMenuItem3});

            void ToolStripMenuItem3Click(object sender, EventArgs e)
            {
                listBox.Items.Remove(listItem);
                listViewItemListDictionary.Remove(listItem);
            }

            contextMenuStrip.Show(listBox, locationPoint, ToolStripDropDownDirection.Default);
        }
    }
}