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
        private void ListBoxAddTag(ListBox listBox, Dictionary<string, Dictionary<string, string>> listBoxTagDictionary,
            string tagName)
        {
            listBox.Items.Add(tagName);
            listBoxTagDictionary.Add(tagName, new Dictionary<string, string>());
        }

        private void ListBoxAddToTag(Dictionary<string, Dictionary<string,string>> listBoxTagDictionary, string tagName,
            Dictionary<string,string> filesDictionary)
        {
            foreach(var file in filesDictionary)
            {
                if (!listBoxTagDictionary[tagName].ContainsKey(file.Key))
                {
                    listBoxTagDictionary[tagName].Add(file.Key, file.Value);
                }
            }
        }

        private void ListViewInitialView()
        {
            //if listView view controller has no option selected, automatically select Large Images
            if (comboBox_view.SelectedIndex == -1) comboBox_view.SelectedIndex = 2;
        }

        private void PopulateDictionaries()
        {
            foreach (var file in _dirInfo.GetFiles())
            {
                _files.Add(file.Name, file.FullName);
                if (!_fileDates.ContainsKey(file.Name))
                {
                    _fileDates.Add(file.Name, file.LastAccessTime.ToString("g"));
                }
            }
        }

        private void PopulateListView(Dictionary<string,string> filesDictionary)
        {
            listView_folder.Items.Clear();
            foreach (var file in filesDictionary)
            {
                //Assigning listView item a name and an imageKey
                var item = new ListViewItem(file.Key, file.Key);

                //Assigning additional information into corresponding subItems of listView
                var subItems = new ListViewItem.ListViewSubItem[1];
                if (_fileDates.ContainsKey(file.Key))
                {
                    subItems[0] = new ListViewItem.ListViewSubItem(item,
                        _fileDates[file.Key]);
                }

                //Adding subItems to item and adding item to listView
                item.SubItems.AddRange(subItems);
                listView_folder.Items.Add(item);
            }
        }

        private void PopulateImageList(ImageList imageList, Dictionary<string, string> filesDictionary)
        {
            var imageListMutex = new Mutex();
            filesDictionary.AsParallel().ForAll(file =>
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

        //Declarations
        private DirectoryInfo _dirInfo;
        //Dictionary with keys of file names and values of file paths
        private Dictionary<string, string> _files = new Dictionary<string, string>();
        //Dictionary with keys of file names and values of file dates
        private Dictionary<string, string> _fileDates = new Dictionary<string, string>();
        //Dictionary with keys of tag names and values of file names
        private Dictionary<string, Dictionary<string, string>> _listBoxItemDictionary =
            new Dictionary<string, Dictionary<string, string>>();

        private List<ListViewItem> _listViewItemSelectionList = new List<ListViewItem>();
        private ImageList _imageList = new ImageList {ImageSize = new Size(140, 140)};

        //Event Handlers
        private void Button_choose_folder_Click(object sender, EventArgs e)
        {
            //Making sure folder browser ended with a selection
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                listView_folder.Items.Clear();

                //Selecting View
                ListViewInitialView();

                //Assigning chosen directory to Directory info
                _dirInfo = new DirectoryInfo(folderBrowserDialog1.SelectedPath);

                //Clearing dictionaries
                _files.Clear();

                //Populating dictionaries
                PopulateDictionaries();

                //Populating ImageList
                PopulateImageList(_imageList, _files);

                //Assigning populated imageList to listView
                listView_folder.LargeImageList = _imageList;
                listView_folder.SmallImageList = _imageList;

                PopulateListView(_files);

                if (listBox_Tags.Items.Contains(_dirInfo.Name))
                {
                    _listBoxItemDictionary[_dirInfo.Name].Clear();
                    ListBoxAddToTag(_listBoxItemDictionary, _dirInfo.Name, _files);
                }
                else
                {
                    ListBoxAddTag(listBox_Tags, _listBoxItemDictionary, _dirInfo.Name);
                    ListBoxAddToTag(_listBoxItemDictionary, _dirInfo.Name, _files);
                }
                _listViewItemSelectionList.Clear();
                listBox_Tags.SetSelected(listBox_Tags.Items.IndexOf(_dirInfo.Name), true);
                PopulateListView(_listBoxItemDictionary[listBox_Tags.SelectedItem.ToString()]);
            }
        }

        //TODO: Implement saving configurations
        private void Button_save_configuration_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ConfigurationData configuration = new ConfigurationData()
                {
                    fileDates = _fileDates, listBoxItemListDictionary = _listBoxItemDictionary
                };
                JsonSerialization.WriteToJsonFile<ConfigurationData>(saveFileDialog1.FileName, configuration);
            }
        }

        private void Button_load_configuration_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ConfigurationData configuration = JsonSerialization.ReadFromJsonFile<ConfigurationData>(openFileDialog1.FileName);
                _fileDates = configuration.fileDates;
                _listBoxItemDictionary = configuration.listBoxItemListDictionary;
                _imageList.Images.Clear();
                listBox_Tags.Items.Clear();
                listView_folder.Items.Clear();
                foreach (var tag in _listBoxItemDictionary)
                {
                    listBox_Tags.Items.Add(tag.Key);
                    PopulateImageList(_imageList, _listBoxItemDictionary[tag.Key]);
                }
                listView_folder.LargeImageList = _imageList;
                listView_folder.SmallImageList = _imageList;
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


        private void Button_new_tag_click(object sender, EventArgs e)
        {
            var promptValue = PopupWindows.ShowTextInputDialog("Enter Tag Name", "Add New Tag");
            if (!promptValue.StartsWith(" ") || promptValue != "")
            {
                if (!_listBoxItemDictionary.ContainsKey(promptValue))
                {
                    listBox_Tags.Items.Add(promptValue);
                    _listBoxItemDictionary.Add(promptValue, new Dictionary<string, string>());
                }
                else
                {
                    //TODO: Write a warning for adding duplicate
                }
            }
        }

        private void ListView_folder_item_selection(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && !_listViewItemSelectionList.Contains(e.Item))
            {
                _listViewItemSelectionList.Add(e.Item);
                Debug.WriteLine("Item Selected: " + e.Item.Text);
            }
            else if (!e.IsSelected && _listViewItemSelectionList.Contains(e.Item))
            {
                _listViewItemSelectionList.Remove(e.Item);
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
                if (_listViewItemSelectionList.Count > 0)
                {
                    PopupWindows.ShowContextMenu(listBox_Tags, e.Location, selectedItem, _listBoxItemDictionary,
                        _fileDates, _listViewItemSelectionList, listView_folder);
                }
                else
                    PopupWindows.ShowContextMenu(listBox_Tags, e.Location, selectedItem, _listBoxItemDictionary,
                        listView_folder);
            }
        }

        private void ListBox_Tags_Double_Click(object sender, EventArgs e)
        {
            if (listBox_Tags.SelectedItem != null)
            {
                PopulateListView(_listBoxItemDictionary[listBox_Tags.SelectedItem.ToString()]);
            }
            else
            {
                listView_folder.Clear();
            }
            
        }

        public Form1()
        {
            InitializeComponent();
            button_choose_folder.Click += Button_choose_folder_Click;
            button_save_configuration.Click += Button_save_configuration_Click;
            button_load_configuration.Click += Button_load_configuration_Click;
            comboBox_view.SelectedIndexChanged += Combo_box_view_change;
            button_New_Tag.Click += Button_new_tag_click;
            listView_folder.ItemSelectionChanged += ListView_folder_item_selection;
            listBox_Tags.MouseDown += ListBox_Tags_Click_Events;
            listBox_Tags.DoubleClick += ListBox_Tags_Double_Click;
        }
    }


    public class ConfigurationData
    {
        public Dictionary<string, string> fileDates = new Dictionary<string, string>();

        public Dictionary<string, Dictionary<string, string>> listBoxItemListDictionary =
            new Dictionary<string, Dictionary<string, string>>();
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
            Dictionary<string, Dictionary<string, string>> listBoxTagDictionary, Dictionary<string,string> fileDates, List<ListViewItem> listViewItems,
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
                foreach (var file in listBoxTagDictionary[listBox.SelectedItem.ToString()])
                {
                    //Assigning listView item a name and an imageKey
                    var item = new ListViewItem(file.Key, file.Key);

                    //Assigning additional information into corresponding subItems of listView
                    var subItems = new ListViewItem.ListViewSubItem[1];
                    if (fileDates.ContainsKey(file.Key))
                    {
                        subItems[0] = new ListViewItem.ListViewSubItem(item,
                            fileDates[file.Key]);
                    }

                    //Adding subItems to item and adding item to listView
                    item.SubItems.AddRange(subItems);
                    listView.Items.Add(item);
                }
            }

            void ToolStripMenuItem1Click(object sender, EventArgs e)
            {
                foreach (var item in listViewItems)
                {
                    listBoxTagDictionary[tagName].Add(item.Text, item.ImageKey);
                }
                if (listBox.SelectedItem.ToString() == tagName)
                {
                    RefreshListView();
                }
            }

            void ToolStripMenuItem2Click(object sender, EventArgs e)
            {
                foreach (var item in listViewItems)
                {
                    listBoxTagDictionary[tagName].Remove(item.Text);
                }
                if (listBox.SelectedItem.ToString() == tagName)
                {
                    RefreshListView();
                }
            }

            void ToolStripMenuItem3Click(object sender, EventArgs e)
            {
                listBoxTagDictionary.Remove(tagName);
                if (listBox.SelectedItem.ToString() == tagName)
                {
                    listView.Items.Clear();
                }
                listBox.Items.Remove(tagName);
            }

            contextMenuStrip.Show(listBox, locationPoint, ToolStripDropDownDirection.Default);
        }


        public static void ShowContextMenu(ListBox listBox, Point locationPoint, string listItem,
            Dictionary<string, Dictionary<string, string>> listViewItemListDictionary, ListView listView)
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


    public static class JsonSerialization
    {
        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = Newtonsoft.Json.JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}