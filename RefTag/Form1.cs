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
    //! A form object of Windows Forms that contains all UI elements
    public partial class Form1 : Form
    {
        //! Method to populate a Tag with a given tagName with information from Directory Info
        /*!
         \param dirInfo a Directory Info instance to innumerate through directories
         \param tagName a string name of a Tag object to populate
         */
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

        //! Method that adds tags to listbox and create dictionary for the said tag
        /*!
         \param listBox a listBox Windows Forms object
         \param tagName a string name of a Tag object to create
         */
        private void ListBoxAddTag(ListBox listBox, string tagName)
        {
            listBox.Items.Add(tagName);
            _session.NewTag(_dirInfo.Name, _dirInfo.FullName);
        }

        //! Method that sets ListView View property if none was selected
        private void ListViewInitialView()
        {
            //If listView view controller has no option selected, automatically select Large Images
            if (comboBox_view.SelectedIndex == -1) comboBox_view.SelectedIndex = 2;
        }

        //! Method that populates ListView with contents of the selected Tag
        /*!
         \param selectedTag a Tag object which contents populate List View
         */
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

        //! Method that populates Image list with images from folder.
        /*!
            This method uses Mutex for maximum efficiency and runs on different thread
            than ListView, therefore imageList has to be unassigned from ListView before
            populating it.
            \param imageList an ImageList object to populate with images
            \param taggedItems a list of TaggedItem objects with paths to images
        */
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

        //! Method that resized an image while maintaining its aspect ratio
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

        //! Event Handler for a button to choose a folder
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

        //! Event Handler for a button to save configuration
        private void Button_save_configuration_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                JsonSerialization.WriteToJsonFile(saveFileDialog1.FileName, _session.Tags);
            }
        }

        //! Event Handler for a button to load configuration
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
                ListViewInitialView();
            }
        }

        //! Event Handler for a combo box controlling ListView View property
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
        //! Event Handler for a button to confirm changes in folders
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

        //! Event Handler for a button to add a new tag
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

        //! Event Handler for selection of ListView items
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

        //! Event Handler for click events for ListBox of tags
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

        //! Event Handler for click events for ListBox of folders
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

        //! Event Handler for double click event for ListBox of tags
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

        //! Event Handler for double click event for ListBox of folders
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

    //! Class for saving the state of the program and relevant data
    public class CurrentSession
    {
        //! List of Tags created in current session
        public List<Tag> Tags;

        //! Storing the tag which contents have to be shown in list view
        public Tag OpenedTag;

        //! Constructor for CurrentSession
        public CurrentSession()
        {
            Tags = new List<Tag>();
        }

        //! Function that assigns a tag for content viewing
        /*!
           \param openedTag a Tag object 
         */
        public void SetOpenedTag(Tag openedTag)
        {
            OpenedTag = openedTag;
        }

        //! Function to add a newly created tag to the list Tags
        /*!
           \param item a Tag object 
         */
        public void AddTag(Tag item)
        {
            Tags.Add(item);
        }

        //! Function to create a tag from a string
        /*!
           \param tagName a string
         */
        public void NewTag(string tagName)
        {
            AddTag(new Tag(tagName));
        }

        //! Function to create a tag from folder information
        /*!
           \param tagName a string
           \param folderPath a stringed full folder name
         */
        public void NewTag(string tagName, string folderPath)
        {
            AddTag(new Tag(tagName, folderPath));
        }

        //! Function to remove a tag and its contents
        /*!
           \param item a Tag object
         */
        public void RemoveTag(Tag item)
        {
            Tags.Find(x => x == item).ItemList.Clear();
            Tags.Remove(item);
        }
    }

    //! Class for storage of folder or tag information.
    public class Tag
    {
        //! String for naming a folder or tag
        public string TagName;  

        //! Bool isFolder is true when Tag was created with a folder path
        public bool IsFolder;

        //! Stringed Folder path to a folder the tag represents
        public string FolderPath;

        //! List of TaggedItems in this tag.
        public List<TaggedItem> ItemList = new List<TaggedItem>();

        //! Empty constructor for JSON serialization
        public Tag()
        {

        }

        //! Constructor for tag creation
        /*!
            \param tagName a string argument
         */
        public Tag(string tagName)
        {
            TagName = tagName;
        }

        //! Constructor for folder creation
        /*!
            \param tagName a string argument
            \param folderPath a string argument, which should be a stringed folder path
         */
        public Tag(string tagName, string folderPath)
        {
            TagName = tagName;
            IsFolder = true;
            FolderPath = folderPath;
        }

        //! Function to add TaggedItems to tag from different data structures
        /*!
           \param item a TaggedItem object 
         */
        public void AddToTag(TaggedItem item)
        {
            if (!ItemList.Contains(item))
            {
                ItemList.Add(item);
            }
        }

        /*!
           \param items an array of TaggedItem objects 
         */
        public void AddToTag(TaggedItem[] items)
        {
            ItemList.AddRange(items);
        }

        /*!
           \param items a list of TaggedItem objects 
         */
        public void AddToTag(List<TaggedItem> items)
        {
            ItemList.AddRange(items);
        }

        //! Overloaded function to remove TaggedItems from tag using different data structures
        /*!
           \param item a TaggedItem object 
         */
        public void RemoveFromTag(TaggedItem item)
        {
            ItemList.Remove(item);
        }

        /*!
           \param items an array of TaggedItem objects 
         */
        public void RemoveFromTag(TaggedItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                ItemList.Remove(items[i]);
            }
        }

        /*!
           \param items a list of TaggedItem objects 
         */
        public void RemoveFromTag(List<TaggedItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemList.Remove(items[i]);
            }
        }
    }

    //! Class for storage of file information
    public class TaggedItem
    {
        //! File name
        public string ItemName;
        //! File extension
        public string ItemFileExtension;
        //! File creation date
        public string ItemCreationDate;
        //! File modification date
        public string ItemModificationDate;
        //! Full file name
        public string ItemPath;

        //! A constructor that's uses information from file system
        /*!
           \param itemName a string containing a file name
           \param itemFileExtension a string containing a file extension
           \param itemCreationDate a string containing a file creation date
           \param itemModificationDate a string containing a file modification date
           \param itemPath a string containing a full file name (file path)
         */
        public TaggedItem(string itemName, string itemFileExtension, string itemCreationDate, string itemModificationDate, string itemPath)
        {
            ItemName = itemName;
            ItemFileExtension = itemFileExtension;
            ItemCreationDate = itemCreationDate;
            ItemModificationDate = itemModificationDate;
            ItemPath = itemPath;
        }
    }

    //! Class for methods to create popup windows
    public static class PopupWindows
    {
        //! A universal popup window that's used for writing a new tag
        /*!
           \param itemName a string containing a file name
           \param itemFileExtension a string containing a file extension
           \return textBox.Text a string containing a name of a new tag
         */
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

        //! A popup warning for duplicate
        /*!
           \param text a string with a warning text
         */
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

        //! A context menu for right clicking on tags
        /*!
           \param listBox a listBox Windows Forms object
           \param locationPoint a Point struct of x/y integer coordinates
           \param selectedTag a Tag object on which the context menu was invoked
           \param openedTag a Tag object currently opened in ListView
           \param session a Session object
           \param listViewItems a list of selected ListViewItems
         */
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

            //! First Item: Adding to tag
            /*!
                Event Handler of a first element of context menu.
                transfers information of selected ListViewItems from openedTag to selectedTag
             */
            void ToolStripMenuItem1Click(object sender, EventArgs e)
            {
                //! Assigning lengthy data to easy to understand variables
                var donorTag = openedTag;
                var receiverTag = selectedTag;

                //! Preventing Duplication of taggedItems
                if (donorTag != receiverTag)
                {
                    foreach (var item in listViewItems)
                    {
                        TaggedItem passedItem = donorTag.ItemList.Find(x => x.ItemName == item.Text);
                        //! Adding selected item
                        if (!receiverTag.ItemList.Contains(passedItem))
                        {
                            receiverTag.AddToTag(passedItem);
                        }
                    }
                    selectedTag.ItemList = receiverTag.ItemList;
                }
            }

            //! Second Item: Removing from tag
            /*!
                Event Handler of a second element of context menu.
                Removes information of selected ListViewItems from selectedTag
             */
            void ToolStripMenuItem2Click(object sender, EventArgs e)
            {
                foreach (var item in listViewItems)
                {
                    TaggedItem removedItem = selectedTag.ItemList.FirstOrDefault(x => x.ItemName == item.Text);
                    if (removedItem != null)
                    {
                        selectedTag.RemoveFromTag(removedItem);
                    }
                    
                }
            }

            //Third Item: Removing tag
            /*!
                Event Handler of a second element of context menu.
                Removes selectedTag
             */
            void ToolStripMenuItem3Click(object sender, EventArgs e)
            {
                session.RemoveTag(selectedTag);
                listBox.Items.Remove(selectedTag.TagName);
            }

            contextMenuStrip.Show(listBox, locationPoint, ToolStripDropDownDirection.Default);

            return true;
        }

        //! Overload of previous function for an empty tag
        public static bool ShowContextMenu(ListBox listBox, Point locationPoint, Tag selectedTag,
            CurrentSession session, List<ListViewItem> listViewItems)
        {
            var toolStripMenuItem3 = new ToolStripMenuItem {Text = @"Delete tag"};
            toolStripMenuItem3.Click += ToolStripMenuItem3Click;
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripItem[] {toolStripMenuItem3});

            //! First and only item: Removing tag
            void ToolStripMenuItem3Click(object sender, EventArgs e)
            {
                session.RemoveTag(selectedTag);
                listBox.Items.Remove(selectedTag.TagName);
            }

            contextMenuStrip.Show(listBox, locationPoint, ToolStripDropDownDirection.Default);

            return true;
        }
    }

    //! Json Serializer for saving and loading current configuration to and from a notepad file
    /*!
        Requires an installation of Newtonsoft Json Serializer
    */
    public static class JsonSerialization
    {
        //! A method to save information as a notepad file
        /*!
            \param filePath a string of a target file
            \param T a template of an object type that has to be serialized
            \param objectToWrite an object that has to be serialized
            \param append a bool to append a target file
         */
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

        //! A method to load information from serialized file
        /*!
            \param filePath a string of a target file
            \param T a template of an object type that is serialized
         */
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