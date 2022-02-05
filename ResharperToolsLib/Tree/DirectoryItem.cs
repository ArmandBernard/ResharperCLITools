using System.Collections.Generic;

namespace ResharperToolsLib.Tree
{
    public class DirectoryItem : ITreeItem
    {
        public IList<ITreeItem> Items { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public DirectoryItem(string name, string path)
        {
            Name = name;
            Path = path;

            Items = new List<ITreeItem>();
        }

        public bool Equals(ITreeItem other)
        {
            if (other is not DirectoryItem otherDir)
            {
                return false;
            }

            if (otherDir == null) { return false; }

            if (Name != otherDir.Name || Path != otherDir.Path) { return false; }

            return true;
        }
    }
}
