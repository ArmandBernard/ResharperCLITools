using System;

namespace ResharperToolsLib.Tree
{
    public interface ITreeItem : IEquatable<ITreeItem>
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

}
