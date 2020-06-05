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
        void PopulateTag(DirectoryInfo dirInfo, string tagName)
        {
            foreach (var file in dirInfo.GetFiles())
            {
                var fileItem = new TaggedItem(file.Name, file.Extension, file.CreationTime.ToString("g"),
                    file.LastAccessTime.ToString("g"), file.FullName);
                if (!_session.Tags.Find(x => x.TagName.Equals(tagName)).ItemList.Contains(fileItem))
                {
                    _session.Tags.Find(x => x.TagName.Equals(tagName)).
                        AddToTag(fileItem);
                }
            }
        }
        //A function to add tags to listbox and create dictionary for the said tag
        private void ListBoxAddTag(ListBox listBox, string tagName)
        {
            listBox.Items.Add(tagName);
            _session.NewTag(_dirInfo.Name, _dirInfo.FullName);
        }

        private void ListViewInitialView()
        {
            //if listView view controller has no option selected, automatically select Large Images
            if (comboBox_view.SelectedIndex == -1) comboBox_view.SelectedIndex = 2;
        }

        private void PopulateListView(Tag selectedTag)
        {
            listView_folder.Items.Clear();
            foreach (var file in selectedTag.ItemList)
            {
                //Assigning listView item a name and an imageKey
                var item = new ListViewItem(file.ItemName, file.ItemName);

                //Assigning additional information into corresponding subItems of listView
                var subItems = new ListViewItem.ListViewSubItem[2];
                subItems[0] = new ListViewItem.ListViewSubItem(item, file.ItemFileExtension);
                subItems[1] = new ListViewItem.ListViewSubItem(item, file.ItemModificationDate);

                //Adding subItems to item and adding item to listView
                item.SubItems.AddRange(subItems);
                listView_folder.Items.Add(item);
            }
        }

        private void PopulateImageList(ImageList imageList, List<TaggedItem> taggedItems)
        {
            var imageListMutex = new Mutex();
            int iteration = 0;
            taggedItems.AsParallel().ForAll(file =>
            {
                Image imageBeforeResizing;
                using (FileStream stream = new FileStream(file.ItemPath, FileMode.Open, FileAccess.Read))
                {
                    imageBeforeResizing = Image.FromStream(stream);
                }
                var resizedImage = ResizeImage(imageBeforeResizing, 140, 140);

                imageListMutex.WaitOne();
                imageList.Images.Add(file.ItemName, resizedImage);
                imageListMutex.ReleaseMutex();

                if (iteration > 100)
                {
                    GC.Collect();
                    Interlocked.Exchange(ref iteration, 0);
                }

                Interlocked.Increment(ref iteration);
            });
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            float aspectRatio;
            int tempHeight;
            int tempWidth;
            if (image.Width >= image.Height)
            {
                aspectRatio = (float) image.Height / image.Width;
                tempHeight = (int) Math.Floor(height * aspectRatio);
                tempWidth = width;
            }
            else
            {
                aspectRatio = (float) image.Width / image.Height;
                tempWidth = (int)Math.Floor(width * aspectRatio);
                tempHeight = height;
            }
            Rectangle destRect = new Rectangle((width - tempWidth)/2, (height - tempHeight)/2, tempWidth, tempHeight);
            Bitmap destImage = new Bitmap(width, height);
            try
            {
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
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel,
                            wrapMode);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Insert placeholder picture");
            }

            return destImage;
        }

        //Declarations
        private DirectoryInfo _dirInfo;

        private readonly CurrentSession _session = new CurrentSession();

        private readonly List<ListViewItem> _listViewItemSelectionList = new List<ListViewItem>();
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

                if (listBox_Folders.Items.Contains(_dirInfo.Name))
                {
                    PopulateTag(_dirInfo, _dirInfo.Name);
                }
                else
                {
                    ListBoxAddTag(listBox_Folders, _dirInfo.Name);
                    PopulateTag(_dirInfo, _dirInfo.Name);
                }
                _listViewItemSelectionList.Clear();
                listBox_Folders.SetSelected(listBox_Folders.Items.IndexOf(_dirInfo.Name), true);

                //Selecting tag
                _session.SetOpenedTag(_session.Tags.Find(x => x.TagName == _dirInfo.Name));

                //Populating ImageList
                PopulateImageList(_imageList, _session.OpenedTag.ItemList);

                //Assigning populated imageList to listView
                listView_folder.LargeImageList = _imageList;
                listView_folder.SmallImageList = _imageList;

                PopulateListView(_session.OpenedTag);
            }
        }

        private void Button_save_configuration_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                JsonSerialization.WriteToJsonFile(saveFileDialog1.FileName, _session.Tags);
            }
        }

        private void Button_load_configuration_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<Tag> configuration = JsonSerialization.ReadFromJsonFile<List<Tag>>(openFileDialog1.FileName);
                _session.Tags = configuration;
                _imageList.Images.Clear();
                listBox_Folders.Items.Clear();
                listBox_Tags.Items.Clear();
                listView_folder.Items.Clear();
                foreach (var tag in _session.Tags)
                {
                    if (tag.IsFolder)
                    {
                        listBox_Folders.Items.Add(tag.TagName);
                    }
                    else
                    {
                        listBox_Tags.Items.Add(tag.TagName);
                    }
                    PopulateImageList(_imageList, tag.ItemList);
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

        private void Button_Force_Folder_Changes_Click(object sender, EventArgs e)
        {
            List<string> deletionList = new List<string>();
            foreach (var tag in _session.Tags)
            {
                if (tag.IsFolder)
                {
                    _dirInfo = new DirectoryInfo(tag.FolderPath);
                    List<TaggedItem> tempItems = new List<TaggedItem>(tag.ItemList);
                    foreach (var file in _dirInfo.GetFiles())
                    {
                        if (tempItems.FirstOrDefault(x => x.ItemName == file.Name) is var tempItem && tempItem != null)
                        {
                            tempItems.Remove(tempItem);
                        }
                    }

                    if (tempItems.Count > 0)
                    {
                        foreach (var item in tempItems)
                        {
                            File.Copy(item.ItemPath, Path.Combine(_dirInfo.FullName, item.ItemName));
                            tag.ItemList.First(x => x.ItemName == item.ItemName).ItemPath = Path.Combine(_dirInfo.FullName, item.ItemName);
                        }
                    }

                    foreach (var file in _dirInfo.GetFiles())
                    {
                        if (tag.ItemList.FirstOrDefault(x => x.ItemName == file.Name) == null)
                        {
                            deletionList.Add(file.FullName);
                        }
                    }
                }
            }

            foreach (var path in deletionList)
            {
                File.Delete(path);
            }
        }

        private void Button_new_tag_click(object sender, EventArgs e)
        {
            var promptValue = PopupWindows.ShowTextInputDialog("Enter Tag Name", "Add New Tag");
            if (_session.Tags.Find(x => x.TagName == promptValue) == null)
            {
                listBox_Tags.Items.Add(promptValue);
                _session.NewTag(promptValue);
            }
            else
            {
                PopupWindows.ShowDuplicateWarning(promptValue);
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
                    PopupWindows.ShowContextMenu(listBox_Tags, e.Location, _session.Tags.Find(x => x.TagName == selectedItem), 
                        _session.OpenedTag, _session, _listViewItemSelectionList);
                }
                else
                    PopupWindows.ShowContextMenu(listBox_Tags, e.Location, _session.Tags.Find(x => x.TagName == selectedItem), 
                        _session, _listViewItemSelectionList);
            }
        }

        private void ListBox_Folders_Click_Events(object sender, MouseEventArgs e)
        {
            var tagIndexFromPoint = listBox_Folders.IndexFromPoint(e.Location);
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
                var selectedItem = listBox_Folders.Items[tagIndexFromPoint].ToString();
                if (_listViewItemSelectionList.Count > 0)
                {
                    PopupWindows.ShowContextMenu(listBox_Folders, e.Location, _session.Tags.Find(x => x.TagName == selectedItem),
                        _session.OpenedTag, _session, _listViewItemSelectionList);
                }
                else
                    PopupWindows.ShowContextMenu(listBox_Tags, e.Location, _session.Tags.Find(x => x.TagName == selectedItem), 
                        _session, _listViewItemSelectionList);
            }
        }

        private void ListBox_Tags_Double_Click(object sender, EventArgs e)
        {
            if (listBox_Tags.SelectedItem != null)
            {
                _session.SetOpenedTag(_session.Tags.Find(x => x.TagName == listBox_Tags.SelectedItem.ToString()));
                PopulateListView(_session.OpenedTag);
            }
            else
            {
                listView_folder.Clear();
            }
            listBox_Folders.ClearSelected();
            _listViewItemSelectionList.Clear();
        }

        private void ListBox_Folders_Double_Click(object sender, EventArgs e)
        {
            if (listBox_Folders.SelectedItem != null)
            {
                _session.SetOpenedTag(_session.Tags.Find(x => x.TagName == listBox_Folders.SelectedItem.ToString()));
                PopulateListView(_session.OpenedTag);
            }
            else
            {
                listView_folder.Clear();
            }
            listBox_Tags.ClearSelected();
            _listViewItemSelectionList.Clear();
        }

        public Form1()
        {
            InitializeComponent();
            button_choose_folder.Click += Button_choose_folder_Click;
            button_save_configuration.Click += Button_save_configuration_Click;
            button_load_configuration.Click += Button_load_configuration_Click;
            comboBox_view.SelectedIndexChanged += Combo_box_view_change;
            button_Force_Folder_Changes.Click += Button_Force_Folder_Changes_Click;
            button_New_Tag.Click += Button_new_tag_click;
            listView_folder.ItemSelectionChanged += ListView_folder_item_selection;
            listBox_Tags.MouseDown += ListBox_Tags_Click_Events;
            listBox_Tags.DoubleClick += ListBox_Tags_Double_Click;
            listBox_Folders.MouseDown += ListBox_Folders_Click_Events;
            listBox_Folders.DoubleClick += ListBox_Folders_Double_Click;
        }
    }

    public class CurrentSession
    {
        public List<Tag> Tags;
        public Tag OpenedTag;

        public CurrentSession()
        {
            Tags = new List<Tag>();
        }

        public void SetOpenedTag(Tag openedTag)
        {
            OpenedTag = openedTag;
        }

        public void AddTag(Tag item)
        {
            Tags.Add(item);
        }

        public void NewTag(string tagName)
        {
            AddTag(new Tag(tagName));
        }

        public void NewTag(string tagName, string folderPath)
        {
            AddTag(new Tag(tagName, folderPath));
        }

        public void RemoveTag(Tag item)
        {
            Tags.Find(x => x == item).ItemList.Clear();
            Tags.Remove(item);
        }
    }


    public class Tag
    {
        public string TagName;
        public bool IsFolder;
        public string FolderPath;
        public List<TaggedItem> ItemList = new List<TaggedItem>();

        public Tag()
        {

        }

        public Tag(string tagName)
        {
            TagName = tagName;
        }

        public Tag(string tagName, string folderPath)
        {
            TagName = tagName;
            IsFolder = true;
            FolderPath = folderPath;
        }

        public void AddToTag(TaggedItem item)
        {
            if (!ItemList.Contains(item))
            {
                ItemList.Add(item);
            }
        }

        public void AddToTag(TaggedItem[] items)
        {
            ItemList.AddRange(items);
        }

        public void AddToTag(List<TaggedItem> items)
        {
            ItemList.AddRange(items);
        }

        public void RemoveFromTag(TaggedItem item)
        {
            ItemList.Remove(item);
        }

        public void RemoveFromTag(TaggedItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                ItemList.Remove(items[i]);
            }
        }

        public void RemoveFromTag(List<TaggedItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemList.Remove(items[i]);
            }
        }
    }

    public class TaggedItem
    {
        public string ItemName;
        public string ItemFileExtension;
        public string ItemCreationDate;
        public string ItemModificationDate;
        public string ItemPath;

        public TaggedItem(string itemName, string itemFileExtension, string itemCreationDate, string itemModificationDate, string itemPath)
        {
            ItemName = itemName;
            ItemFileExtension = itemFileExtension;
            ItemCreationDate = itemCreationDate;
            ItemModificationDate = itemModificationDate;
            ItemPath = itemPath;
        }
    }


    public static class PopupWindows
    {
        //A popup window for writing a new tag
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
            var textLabel = new Label() {Left = 50, Top = 20, AutoSize = true, Text = text};
            var textBox = new TextBox() {Left = 50, Top = 50, Width = 400};
            var confirmation = new Button()
                {Text = @"Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK};
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

             if (prompt.ShowDialog() == DialogResult.OK)
             {
                 if (textBox.Text == "")
                 {
                     textBox.Text = @"New tag";
                 }
                 return textBox.Text;
             }
             else
             {
                 return "New tag";
            }
        }

        public static void ShowDuplicateWarning(string text)
        {
            var prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = @"Duplicate Warning",
                StartPosition = FormStartPosition.CenterScreen
            };
            var textLabel = new Label() { Left = 50, Top = 20, AutoSize = true, Text = @"Can not add duplicate tag"};
            var confirmation = new Button()
                { Text = @"Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            prompt.ShowDialog();
        }

        //A context menu for right clicking on tags
        public static bool ShowContextMenu(ListBox listBox, Point locationPoint, Tag selectedTag, Tag openedTag,
            CurrentSession session, List<ListViewItem> listViewItems)
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

            //First Item: Adding to tag
            void ToolStripMenuItem1Click(object sender, EventArgs e)
            {
                //Assigning lengthy data to easy to understand variables
                var donorTag = openedTag;
                var receiverTag = selectedTag;

                //Preventing Duplication of taggedItems
                if (donorTag != receiverTag)
                {
                    foreach (var item in listViewItems)
                    {
                        TaggedItem passedItem = donorTag.ItemList.Find(x => x.ItemName == item.Text);
                        //Adding selected item
                        if (!receiverTag.ItemList.Contains(passedItem))
                        {
                            receiverTag.AddToTag(passedItem);
                        }
                    }
                    selectedTag.ItemList = receiverTag.ItemList;
                }
            }

            //Second Item: Removing from tag
            void ToolStripMenuItem2Click(object sender, EventArgs e)
            {
                foreach (var item in listViewItems)
                {
                    TaggedItem removedItem = openedTag.ItemList.Find(x => x.ItemName == item.Text);
                    openedTag.RemoveFromTag(removedItem);
                }
            }

            //Third Item: Removing tag
            void ToolStripMenuItem3Click(object sender, EventArgs e)
            {
                session.RemoveTag(selectedTag);
                listBox.Items.Remove(selectedTag.TagName);
            }

            contextMenuStrip.Show(listBox, locationPoint, ToolStripDropDownDirection.Default);

            return true;
        }


        public static bool ShowContextMenu(ListBox listBox, Point locationPoint, Tag selectedTag,
            CurrentSession session, List<ListViewItem> listViewItems)
        {
            var toolStripMenuItem3 = new ToolStripMenuItem {Text = @"Delete tag"};
            toolStripMenuItem3.Click += ToolStripMenuItem3Click;
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripItem[] {toolStripMenuItem3});

            //First and only item: Removing tag
            void ToolStripMenuItem3Click(object sender, EventArgs e)
            {
                session.RemoveTag(selectedTag);
                listBox.Items.Remove(selectedTag.TagName);
            }

            contextMenuStrip.Show(listBox, locationPoint, ToolStripDropDownDirection.Default);

            return true;
        }
    }


    public static class JsonSerialization
    {
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
                writer?.Close();
            }
        }

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
                reader?.Close();
            }
        }
    }
}