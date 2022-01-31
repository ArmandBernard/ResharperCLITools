namespace ResharperCLIToolsGUI.Tree
{
    public class FileItem : ITreeItem
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public bool IsChecked { get; set; }

        public FileItem(string name, string path)
        {
            Name = name;
            Path = path;
            IsChecked = false;
        }
    }

}
