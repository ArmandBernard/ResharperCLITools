using System.Collections.Generic;

namespace ResharperToolsLib.Tree
{
    public class DirectoryItem : ITreeItem
    {
        public List<ITreeItem> Items { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public DirectoryItem(string name, string path)
        {
            Name = name;
            Path = path;

            Items = new List<ITreeItem>();
        }
    }
}
