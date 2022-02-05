namespace ResharperToolsLib.Tree
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

        public bool Equals(ITreeItem other)
        {
            if (other is not FileItem otherFile)
            {
                return false;
            }

            if (otherFile == null) { return false; }

            if (Name != otherFile.Name || Path != otherFile.Path) { return false; }

            return true;
        }
    }

}
